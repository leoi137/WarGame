using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance { get; private set; }

    public Terrain GeneratedTerrain { get; private set; }
    public NavMeshSurface NavMeshSurface { get; private set; }
    public Vector2[] RiverPath => _riverPath;

    private TerrainType _primaryTerrain;
    private TerrainType _secondaryTerrain;
    private int _mapSize;
    private Vector2[] _riverPath;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Generate(BattleConfiguration config)
    {
        GenerateTerrain(
            config.primaryTerrain,
            config.secondaryTerrain,
            config.mapSize > 0 ? config.mapSize : GameConfig.DefaultMapSize,
            config.randomSeed
        );
    }

    public void GenerateTerrain(TerrainType primary, TerrainType secondary, int mapSize, int seed)
    {
        _primaryTerrain = primary;
        _secondaryTerrain = secondary;
        _mapSize = mapSize;

        BiomeConfig config = BiomeDefinitions.GetConfig(primary);
        int resolution = 257;
        float heightScale = GameConfig.TerrainHeightScale;

        float[,] heights = ElevationGenerator.Generate(
            resolution, config.noiseScale * 100f, config.noiseOctaves,
            config.persistence, config.lacunarity, seed
        );
        heights = ElevationGenerator.ApplyBiomeProfile(heights, config);

        if (config.riverProbability > 0.5f)
        {
            _riverPath = RiverGenerator.GenerateRiverPath(mapSize, seed);
            heights = ElevationGenerator.CarveRiverBed(heights, _riverPath, 0.08f, 0.15f);
        }
        else
        {
            _riverPath = null;
        }

        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(mapSize, heightScale, mapSize);
        terrainData.SetHeights(0, 0, heights);

        TerrainLayer groundLayer = new TerrainLayer();
        groundLayer.diffuseTexture = CreateSolidTexture(config.groundColor);
        groundLayer.tileSize = new Vector2(10, 10);

        TerrainLayer altLayer = new TerrainLayer();
        altLayer.diffuseTexture = CreateSolidTexture(config.groundColorAlt);
        altLayer.tileSize = new Vector2(10, 10);

        TerrainLayer cliffLayer = new TerrainLayer();
        cliffLayer.diffuseTexture = CreateSolidTexture(config.cliffColor);
        cliffLayer.tileSize = new Vector2(10, 10);

        terrainData.terrainLayers = new TerrainLayer[] { groundLayer, altLayer, cliffLayer };

        int splatRes = terrainData.alphamapResolution;
        float[,,] splatmap = new float[splatRes, splatRes, 3];
        for (int x = 0; x < splatRes; x++)
        {
            for (int z = 0; z < splatRes; z++)
            {
                float nx = (float)x / splatRes;
                float nz = (float)z / splatRes;
                int hx = Mathf.Clamp(Mathf.FloorToInt(nx * (resolution - 1)), 0, resolution - 1);
                int hz = Mathf.Clamp(Mathf.FloorToInt(nz * (resolution - 1)), 0, resolution - 1);
                float h = heights[hx, hz];
                float cliff = h > 0.7f ? (h - 0.7f) * 3.33f : 0f;
                float alt = Mathf.PerlinNoise(nx * 8f + seed * 0.01f, nz * 8f) > 0.6f ? 0.5f : 0f;
                float ground = Mathf.Clamp01(1f - cliff - alt);
                float sum = ground + alt + cliff;
                if (sum > 0.001f)
                {
                    splatmap[x, z, 0] = ground / sum;
                    splatmap[x, z, 1] = alt / sum;
                    splatmap[x, z, 2] = cliff / sum;
                }
                else
                {
                    splatmap[x, z, 0] = 1f;
                    splatmap[x, z, 1] = 0f;
                    splatmap[x, z, 2] = 0f;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmap);

        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.name = "Terrain";
        terrainObj.transform.position = Vector3.zero;
        GeneratedTerrain = terrainObj.GetComponent<Terrain>();

        NavMeshSurface = terrainObj.AddComponent<NavMeshSurface>();

        if (_riverPath != null)
            RiverGenerator.CreateRiverVisuals(GeneratedTerrain, _riverPath, 0.08f, config.waterColor);

        GameObject vegetationParent = new GameObject("Vegetation");
        vegetationParent.transform.SetParent(terrainObj.transform);
        VegetationGenerator.Generate(GeneratedTerrain, config, seed, vegetationParent.transform);

        CreateLighting(config);
        BakeNavMesh();
    }

    public float GetHeightAtPosition(Vector3 worldPos)
    {
        if (GeneratedTerrain == null) return 0f;
        return GeneratedTerrain.SampleHeight(worldPos);
    }

    public TerrainType GetTerrainTypeAtPosition(Vector3 worldPos)
    {
        return _primaryTerrain;
    }

    public void BakeNavMesh()
    {
        if (NavMeshSurface != null)
        {
            NavMeshSurface.collectObjects = CollectObjects.All;
            NavMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            NavMeshSurface.BuildNavMesh();
        }
    }

    public void CreateLighting(BiomeConfig config)
    {
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = config.ambientLightColor;
        light.intensity = 1.2f;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.6f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        RenderSettings.ambientLight = config.ambientLightColor;
        RenderSettings.fog = config.fogDensity > 0.01f;
        RenderSettings.fogDensity = config.fogDensity;
        RenderSettings.fogColor = config.skyColor;
    }

    private Texture2D CreateSolidTexture(Color color)
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
