using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; set; }

    [Header("Terrain Settings")]
    public int mapSize = 100;
    public float maxHeight = 3f;
    public float noiseScale = 0.03f;

    [Header("Trees")]
    public int treeCount = 40;

    [Header("Runtime")]
    public Terrain terrain;
    public NavMeshSurface navMeshSurface;

    void Awake()
    {
        Instance = this;
    }

    public void Generate()
    {
        CreateTerrain();
        CreateRiver();
        CreateTrees();
        CreateLighting();
        BakeNavMesh();
    }

    void CreateTerrain()
    {
        // Create terrain data
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = 257;
        terrainData.size = new Vector3(mapSize, maxHeight, mapSize);

        // Generate heightmap with perlin noise
        int res = terrainData.heightmapResolution;
        float[,] heights = new float[res, res];
        float seed = Random.Range(0f, 1000f);

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float nx = (float)x / res;
                float nz = (float)z / res;

                float h = Mathf.PerlinNoise(nx / noiseScale + seed, nz / noiseScale + seed) * 0.15f;

                // Flatten the center (river area)
                float centerDist = Mathf.Abs(nx - 0.5f) * 2f;
                if (centerDist < 0.15f)
                {
                    h *= centerDist / 0.15f;
                }

                // Raise edges slightly for hills
                float edgeFactor = Mathf.Max(
                    Mathf.Abs(nx - 0.5f),
                    Mathf.Abs(nz - 0.5f)
                ) * 2f;
                if (edgeFactor > 0.7f)
                {
                    h += (edgeFactor - 0.7f) * 0.3f;
                }

                heights[z, x] = h;
            }
        }

        terrainData.SetHeights(0, 0, heights);

        // Create terrain layers (grass + dirt)
        TerrainLayer grassLayer = new TerrainLayer();
        Texture2D grassTex = CreateSolidTexture(new Color(0.25f, 0.55f, 0.2f));
        grassLayer.diffuseTexture = grassTex;
        grassLayer.tileSize = new Vector2(10, 10);

        TerrainLayer dirtLayer = new TerrainLayer();
        Texture2D dirtTex = CreateSolidTexture(new Color(0.45f, 0.35f, 0.2f));
        dirtLayer.diffuseTexture = dirtTex;
        dirtLayer.tileSize = new Vector2(10, 10);

        terrainData.terrainLayers = new TerrainLayer[] { grassLayer, dirtLayer };

        // Paint splatmap
        int splatRes = terrainData.alphamapResolution;
        float[,,] splatmap = new float[splatRes, splatRes, 2];

        for (int x = 0; x < splatRes; x++)
        {
            for (int z = 0; z < splatRes; z++)
            {
                float nx = (float)x / splatRes;
                float nz = (float)z / splatRes;

                float dirtAmount = 0f;

                // Path from north to south
                float pathDist = Mathf.Abs(nz - 0.5f);
                if (pathDist < 0.03f)
                    dirtAmount = 1f - pathDist / 0.03f;

                // Some noise for natural look
                float noise = Mathf.PerlinNoise(nx * 5f, nz * 5f);
                if (noise > 0.7f)
                    dirtAmount = Mathf.Max(dirtAmount, (noise - 0.7f) * 2f);

                splatmap[x, z, 0] = 1f - dirtAmount;
                splatmap[x, z, 1] = dirtAmount;
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);

        // Instantiate terrain
        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.name = "Terrain";
        terrainObj.transform.position = Vector3.zero;
        terrain = terrainObj.GetComponent<Terrain>();

        // Add NavMeshSurface for runtime baking
        navMeshSurface = terrainObj.AddComponent<NavMeshSurface>();
    }

    void CreateRiver()
    {
        GameObject river = GameObject.CreatePrimitive(PrimitiveType.Plane);
        river.name = "River";

        float riverX = mapSize * 0.5f;
        river.transform.position = new Vector3(riverX, 0.15f, mapSize * 0.5f);
        river.transform.localScale = new Vector3(0.6f, 1f, mapSize * 0.1f);

        Renderer rend = river.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0.2f, 0.4f, 0.75f, 0.8f));
        rend.material = mat;

        // Make it non-walkable by NavMesh (remove collider, NavMesh won't include it)
        Destroy(river.GetComponent<Collider>());
    }

    void CreateTrees()
    {
        GameObject treeParent = new GameObject("Trees");

        for (int i = 0; i < treeCount; i++)
        {
            float x, z;
            int attempts = 0;
            do
            {
                x = Random.Range(5f, mapSize - 5f);
                z = Random.Range(5f, mapSize - 5f);
                attempts++;
            } while (Mathf.Abs(x - mapSize * 0.5f) < 8f && attempts < 20);

            float y = 0;
            if (terrain != null)
            {
                y = terrain.SampleHeight(new Vector3(x, 0, z));
            }

            CreateBlockTree(treeParent.transform, new Vector3(x, y, z));
        }
    }

    void CreateBlockTree(Transform parent, Vector3 position)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.SetParent(parent);
        tree.transform.position = position;

        // Trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.4f, 3f, 0.4f);

        Renderer trunkRend = trunk.GetComponent<Renderer>();
        Material trunkMat = ShaderHelper.CreateMaterial(new Color(0.45f, 0.3f, 0.15f));
        trunkRend.material = trunkMat;

        // Leaves (stack of cubes)
        float[] leafSizes = { 2.2f, 1.8f, 1.2f };
        float[] leafHeights = { 2.8f, 3.6f, 4.2f };
        Color[] leafColors = {
            new Color(0.15f, 0.5f, 0.15f),
            new Color(0.2f, 0.6f, 0.2f),
            new Color(0.25f, 0.55f, 0.25f)
        };

        for (int i = 0; i < leafSizes.Length; i++)
        {
            GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaf.name = $"Leaves_{i}";
            leaf.transform.SetParent(tree.transform);
            leaf.transform.localPosition = new Vector3(0, leafHeights[i], 0);
            leaf.transform.localScale = new Vector3(leafSizes[i], 0.8f, leafSizes[i]);

            Renderer leafRend = leaf.GetComponent<Renderer>();
            Material leafMat = ShaderHelper.CreateMaterial(leafColors[i]);
            leafRend.material = leafMat;
        }

        // Mark tree as static for NavMesh avoidance
        tree.isStatic = true;
        foreach (Transform child in tree.transform)
        {
            child.gameObject.isStatic = true;
        }
    }

    void CreateLighting()
    {
        // Directional light (sun)
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1f, 0.95f, 0.85f);
        light.intensity = 1.2f;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.6f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    /// <summary>
    /// Call after camera is set up to apply sky color.
    /// </summary>
    public static void ApplySkyToCamera(Camera cam)
    {
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.5f, 0.7f, 0.9f);
        }
    }

    void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.collectObjects = CollectObjects.All;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.BuildNavMesh();
        }
    }

    Texture2D CreateSolidTexture(Color color)
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++)
            pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
