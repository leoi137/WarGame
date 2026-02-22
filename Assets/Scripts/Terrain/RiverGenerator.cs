using UnityEngine;

public static class RiverGenerator
{
    public static Vector2[] GenerateRiverPath(int mapSize, int seed)
    {
        System.Random rng = new System.Random(seed);
        int pointCount = 20 + rng.Next(0, 15);
        Vector2[] path = new Vector2[pointCount];

        float startX = (float)rng.NextDouble() * 0.3f + 0.35f;
        path[0] = new Vector2(startX, 0f);

        for (int i = 1; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float x = path[i - 1].x + (float)(rng.NextDouble() - 0.5) * 0.15f;
            x = Mathf.Clamp(x, 0.2f, 0.8f);
            path[i] = new Vector2(x, t);
        }

        path[pointCount - 1] = new Vector2(path[pointCount - 1].x, 1f);
        return path;
    }

    public static void CreateRiverVisuals(Terrain terrain, Vector2[] path, float width, Color waterColor)
    {
        if (terrain == null || path == null || path.Length < 2) return;

        TerrainData data = terrain.terrainData;
        Vector3 terrainSize = data.size;
        float terrainWidth = terrainSize.x;
        float terrainLength = terrainSize.z;

        GameObject riverParent = new GameObject("River");
        riverParent.transform.SetParent(terrain.transform);

        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector2 a = path[i];
            Vector2 b = path[i + 1];
            Vector3 worldA = new Vector3(a.x * terrainWidth, 0f, a.y * terrainLength);
            Vector3 worldB = new Vector3(b.x * terrainWidth, 0f, b.y * terrainLength);
            worldA.y = terrain.SampleHeight(worldA) + 0.05f;
            worldB.y = terrain.SampleHeight(worldB) + 0.05f;

            float segLen = Vector3.Distance(worldA, worldB);
            if (segLen < 0.1f) continue;

            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Quad);
            segment.name = $"RiverSegment_{i}";
            segment.transform.SetParent(riverParent.transform);
            segment.transform.position = (worldA + worldB) * 0.5f;
            segment.transform.LookAt(worldB);
            segment.transform.Rotate(90f, 0f, 0f);
            segment.transform.localScale = new Vector3(width * terrainWidth * 0.5f, segLen * 0.5f, 1f);

            Renderer rend = segment.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(new Color(waterColor.r, waterColor.g, waterColor.b, 0.85f));
            Object.Destroy(segment.GetComponent<Collider>());
        }
    }

    public static bool IsRiverCrossing(Vector3 position, Vector2[] riverPath, float width)
    {
        if (riverPath == null || riverPath.Length < 2) return false;

        Terrain terrain = TerrainGenerator.Instance != null ? TerrainGenerator.Instance.GeneratedTerrain : null;
        if (terrain == null) return false;

        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;
        float nx = (position.x - terrainPos.x) / terrainSize.x;
        float nz = (position.z - terrainPos.z) / terrainSize.z;
        Vector2 pt = new Vector2(nx, nz);

        for (int i = 0; i < riverPath.Length - 1; i++)
        {
            float dist = DistanceToSegment(pt, riverPath[i], riverPath[i + 1]);
            if (dist <= width * 0.5f) return true;
        }

        return false;
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab));
        Vector2 closest = a + t * ab;
        return Vector2.Distance(p, closest);
    }
}
