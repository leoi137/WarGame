using UnityEngine;

public enum VegetationStyle
{
    ShortRounded,
    TallDense,
    Pine,
    Cacti,
    Mangrove,
    Palm
}

public struct BiomeConfig
{
    public Color groundColor;
    public Color groundColorAlt;
    public Color cliffColor;
    public Color waterColor;
    public VegetationStyle vegetationType;
    public float vegetationDensity;
    public float maxElevation;
    public float noiseScale;
    public int noiseOctaves;
    public float persistence;
    public float lacunarity;
    public float riverProbability;
    public float rockFrequency;
    public Color ambientLightColor;
    public float fogDensity;
    public Color skyColor;
}

public static class BiomeDefinitions
{
    public static BiomeConfig GetConfig(TerrainType type)
    {
        return type switch
        {
            TerrainType.Plains => new BiomeConfig
            {
                groundColor = new Color(0.45f, 0.65f, 0.25f),
                groundColorAlt = new Color(0.55f, 0.72f, 0.30f),
                cliffColor = new Color(0.55f, 0.45f, 0.30f),
                waterColor = new Color(0.2f, 0.4f, 0.75f),
                vegetationType = VegetationStyle.ShortRounded,
                vegetationDensity = 0.05f,
                maxElevation = 0.15f,
                noiseScale = 0.04f,
                noiseOctaves = 3,
                persistence = 0.5f,
                lacunarity = 2f,
                riverProbability = 0.2f,
                rockFrequency = 0.02f,
                ambientLightColor = new Color(0.6f, 0.65f, 0.7f),
                fogDensity = 0.01f,
                skyColor = new Color(0.5f, 0.7f, 0.9f)
            },
            TerrainType.Forest => new BiomeConfig
            {
                groundColor = new Color(0.22f, 0.42f, 0.15f),
                groundColorAlt = new Color(0.18f, 0.35f, 0.12f),
                cliffColor = new Color(0.45f, 0.38f, 0.25f),
                waterColor = new Color(0.18f, 0.35f, 0.6f),
                vegetationType = VegetationStyle.TallDense,
                vegetationDensity = 0.8f,
                maxElevation = 0.35f,
                noiseScale = 0.03f,
                noiseOctaves = 4,
                persistence = 0.55f,
                lacunarity = 2.2f,
                riverProbability = 0.15f,
                rockFrequency = 0.1f,
                ambientLightColor = new Color(0.35f, 0.45f, 0.4f),
                fogDensity = 0.03f,
                skyColor = new Color(0.4f, 0.55f, 0.7f)
            },
            TerrainType.Hills => new BiomeConfig
            {
                groundColor = new Color(0.52f, 0.55f, 0.32f),
                groundColorAlt = new Color(0.45f, 0.42f, 0.28f),
                cliffColor = new Color(0.55f, 0.52f, 0.48f),
                waterColor = new Color(0.2f, 0.4f, 0.7f),
                vegetationType = VegetationStyle.ShortRounded,
                vegetationDensity = 0.2f,
                maxElevation = 0.65f,
                noiseScale = 0.025f,
                noiseOctaves = 5,
                persistence = 0.6f,
                lacunarity = 2.5f,
                riverProbability = 0.1f,
                rockFrequency = 0.35f,
                ambientLightColor = new Color(0.55f, 0.58f, 0.62f),
                fogDensity = 0.02f,
                skyColor = new Color(0.5f, 0.65f, 0.85f)
            },
            TerrainType.Mountains => new BiomeConfig
            {
                groundColor = new Color(0.55f, 0.55f, 0.55f),
                groundColorAlt = new Color(0.70f, 0.70f, 0.72f),
                cliffColor = new Color(0.5f, 0.5f, 0.52f),
                waterColor = new Color(0.15f, 0.35f, 0.65f),
                vegetationType = VegetationStyle.Pine,
                vegetationDensity = 0.1f,
                maxElevation = 1f,
                noiseScale = 0.02f,
                noiseOctaves = 6,
                persistence = 0.65f,
                lacunarity = 2.8f,
                riverProbability = 0.05f,
                rockFrequency = 0.6f,
                ambientLightColor = new Color(0.65f, 0.68f, 0.75f),
                fogDensity = 0.04f,
                skyColor = new Color(0.55f, 0.65f, 0.9f)
            },
            TerrainType.Steppe => new BiomeConfig
            {
                groundColor = new Color(0.72f, 0.68f, 0.35f),
                groundColorAlt = new Color(0.78f, 0.72f, 0.38f),
                cliffColor = new Color(0.65f, 0.58f, 0.4f),
                waterColor = new Color(0.25f, 0.45f, 0.7f),
                vegetationType = VegetationStyle.ShortRounded,
                vegetationDensity = 0.02f,
                maxElevation = 0.12f,
                noiseScale = 0.05f,
                noiseOctaves = 2,
                persistence = 0.45f,
                lacunarity = 1.8f,
                riverProbability = 0.08f,
                rockFrequency = 0.03f,
                ambientLightColor = new Color(0.75f, 0.72f, 0.65f),
                fogDensity = 0.005f,
                skyColor = new Color(0.6f, 0.75f, 0.95f)
            },
            TerrainType.Desert => new BiomeConfig
            {
                groundColor = new Color(0.85f, 0.78f, 0.55f),
                groundColorAlt = new Color(0.80f, 0.72f, 0.48f),
                cliffColor = new Color(0.7f, 0.65f, 0.5f),
                waterColor = new Color(0.3f, 0.55f, 0.85f),
                vegetationType = VegetationStyle.Cacti,
                vegetationDensity = 0f,
                maxElevation = 0.25f,
                noiseScale = 0.035f,
                noiseOctaves = 4,
                persistence = 0.5f,
                lacunarity = 2.2f,
                riverProbability = 0.02f,
                rockFrequency = 0.08f,
                ambientLightColor = new Color(0.9f, 0.85f, 0.75f),
                fogDensity = 0.008f,
                skyColor = new Color(0.7f, 0.8f, 0.95f)
            },
            TerrainType.RiverValley => new BiomeConfig
            {
                groundColor = new Color(0.35f, 0.55f, 0.32f),
                groundColorAlt = new Color(0.30f, 0.50f, 0.55f),
                cliffColor = new Color(0.48f, 0.42f, 0.32f),
                waterColor = new Color(0.2f, 0.45f, 0.7f),
                vegetationType = VegetationStyle.ShortRounded,
                vegetationDensity = 0.3f,
                maxElevation = 0.18f,
                noiseScale = 0.045f,
                noiseOctaves = 3,
                persistence = 0.48f,
                lacunarity = 2f,
                riverProbability = 1f,
                rockFrequency = 0.05f,
                ambientLightColor = new Color(0.55f, 0.65f, 0.7f),
                fogDensity = 0.015f,
                skyColor = new Color(0.5f, 0.68f, 0.88f)
            },
            TerrainType.Coast => new BiomeConfig
            {
                groundColor = new Color(0.78f, 0.75f, 0.55f),
                groundColorAlt = new Color(0.45f, 0.60f, 0.72f),
                cliffColor = new Color(0.6f, 0.55f, 0.45f),
                waterColor = new Color(0.25f, 0.5f, 0.8f),
                vegetationType = VegetationStyle.Palm,
                vegetationDensity = 0.1f,
                maxElevation = 0.08f,
                noiseScale = 0.06f,
                noiseOctaves = 2,
                persistence = 0.4f,
                lacunarity = 1.6f,
                riverProbability = 0.3f,
                rockFrequency = 0.04f,
                ambientLightColor = new Color(0.7f, 0.75f, 0.85f),
                fogDensity = 0.012f,
                skyColor = new Color(0.55f, 0.72f, 0.92f)
            },
            TerrainType.Wetlands => new BiomeConfig
            {
                groundColor = new Color(0.32f, 0.45f, 0.28f),
                groundColorAlt = new Color(0.28f, 0.38f, 0.30f),
                cliffColor = new Color(0.42f, 0.38f, 0.28f),
                waterColor = new Color(0.22f, 0.42f, 0.55f),
                vegetationType = VegetationStyle.Mangrove,
                vegetationDensity = 0.15f,
                maxElevation = 0.08f,
                noiseScale = 0.055f,
                noiseOctaves = 2,
                persistence = 0.42f,
                lacunarity = 1.7f,
                riverProbability = 0.4f,
                rockFrequency = 0.02f,
                ambientLightColor = new Color(0.4f, 0.5f, 0.5f),
                fogDensity = 0.035f,
                skyColor = new Color(0.45f, 0.58f, 0.72f)
            },
            TerrainType.Jungle => new BiomeConfig
            {
                groundColor = new Color(0.15f, 0.38f, 0.12f),
                groundColorAlt = new Color(0.12f, 0.32f, 0.10f),
                cliffColor = new Color(0.35f, 0.32f, 0.22f),
                waterColor = new Color(0.18f, 0.38f, 0.55f),
                vegetationType = VegetationStyle.TallDense,
                vegetationDensity = 0.9f,
                maxElevation = 0.45f,
                noiseScale = 0.028f,
                noiseOctaves = 5,
                persistence = 0.58f,
                lacunarity = 2.4f,
                riverProbability = 0.25f,
                rockFrequency = 0.12f,
                ambientLightColor = new Color(0.25f, 0.35f, 0.3f),
                fogDensity = 0.05f,
                skyColor = new Color(0.35f, 0.5f, 0.65f)
            },
            _ => GetConfig(TerrainType.Plains)
        };
    }
}
