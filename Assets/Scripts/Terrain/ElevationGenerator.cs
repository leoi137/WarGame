using UnityEngine;

public static class ElevationGenerator
{
    public static float[,] Generate(int resolution, float scale, int octaves, float persistence, float lacunarity, int seed)
    {
        float[,] heights = new float[resolution, resolution];
        float maxNoise = 0f;
        float seedOffset = seed * 0.001f;

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseValue = 0f;

                for (int o = 0; o < octaves; o++)
                {
                    float offsetX = x * scale * frequency / resolution + seedOffset + o * 137.5f;
                    float offsetZ = z * scale * frequency / resolution + seedOffset * 97f + o * 89.3f;
                    float sample = Mathf.PerlinNoise(offsetX, offsetZ);
                    noiseValue += sample * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, z] = Mathf.Clamp01(noiseValue);
                if (noiseValue > maxNoise) maxNoise = noiseValue;
            }
        }

        if (maxNoise > 0f)
        {
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    heights[x, z] /= maxNoise;
                }
            }
        }

        return heights;
    }

    public static float[,] ApplyBiomeProfile(float[,] heights, BiomeConfig config)
    {
        int w = heights.GetLength(0);
        int h = heights.GetLength(1);

        for (int x = 0; x < w; x++)
        {
            for (int z = 0; z < h; z++)
            {
                float raw = heights[x, z];
                heights[x, z] = raw * config.maxElevation;
            }
        }

        return heights;
    }

    public static float[,] CarveRiverBed(float[,] heights, Vector2[] riverPath, float width, float depth)
    {
        int res = heights.GetLength(0);

        foreach (var point in riverPath)
        {
            int cx = Mathf.RoundToInt(point.x * (res - 1));
            int cz = Mathf.RoundToInt(point.y * (res - 1));
            int halfWidth = Mathf.Max(1, Mathf.RoundToInt(width * res * 0.5f));

            for (int dx = -halfWidth; dx <= halfWidth; dx++)
            {
                for (int dz = -halfWidth; dz <= halfWidth; dz++)
                {
                    int x = cx + dx;
                    int z = cz + dz;
                    if (x < 0 || x >= res || z < 0 || z >= res) continue;

                    float dist = Mathf.Sqrt(dx * dx + dz * dz) / halfWidth;
                    if (dist > 1f) continue;

                    float falloff = 1f - dist;
                    float carve = depth * falloff;
                    heights[x, z] = Mathf.Max(0f, heights[x, z] - carve);
                }
            }
        }

        return heights;
    }
}
