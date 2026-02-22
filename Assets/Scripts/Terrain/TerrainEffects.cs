using UnityEngine;

public static class TerrainEffects
{
    private const float RiverCrossingWidth = 0.08f;
    private const float ElevationAdvantageThreshold = 2f;
    private const float ElevationBonusPerUnit = 0.05f;

    public static float GetMovementMultiplier(Vector3 position, UnitCategory category)
    {
        TerrainType terrain = SampleTerrainAt(position);
        TerrainDefinition def = TerrainDatabase.Get(terrain);
        if (def == null) return 1f;

        bool isCavalry = category == UnitCategory.HeavyCavalry || category == UnitCategory.LightCavalry || category == UnitCategory.Elephant;
        float baseMultiplier = isCavalry ? def.cavalrySpeedMultiplier : def.movementMultiplier;

        if (RiverGenerator.IsRiverCrossing(position, TerrainGenerator.Instance?.RiverPath, RiverCrossingWidth))
            baseMultiplier *= 0.5f;

        return Mathf.Clamp(baseMultiplier, 0.4f, 1.4f);
    }

    public static float GetDefenseBonus(Vector3 position, UnitCategory category)
    {
        TerrainType terrain = SampleTerrainAt(position);
        TerrainDefinition def = TerrainDatabase.Get(terrain);
        if (def == null) return 0f;

        bool isInfantry = category == UnitCategory.HeavyInfantry || category == UnitCategory.LightInfantry;
        return isInfantry ? def.infantryDefenseBonus : 0f;
    }

    public static float GetRangedAccuracy(Vector3 position)
    {
        TerrainType terrain = SampleTerrainAt(position);
        TerrainDefinition def = TerrainDatabase.Get(terrain);
        return def != null ? def.rangedAccuracyModifier : 1f;
    }

    public static float GetVisibilityRange(Vector3 position)
    {
        TerrainType terrain = SampleTerrainAt(position);
        TerrainDefinition def = TerrainDatabase.Get(terrain);
        return def != null ? def.visibilityRange : 1f;
    }

    public static bool IsElevated(Vector3 position, float threshold)
    {
        if (TerrainGenerator.Instance == null) return false;
        float height = TerrainGenerator.Instance.GetHeightAtPosition(position);
        return height >= threshold;
    }

    public static float GetElevationAdvantage(Vector3 attacker, Vector3 defender)
    {
        float attackerHeight = TerrainGenerator.Instance != null
            ? TerrainGenerator.Instance.GetHeightAtPosition(attacker)
            : attacker.y;
        float defenderHeight = TerrainGenerator.Instance != null
            ? TerrainGenerator.Instance.GetHeightAtPosition(defender)
            : defender.y;
        float diff = attackerHeight - defenderHeight;

        if (diff < ElevationAdvantageThreshold) return 0f;
        return (diff - ElevationAdvantageThreshold) * ElevationBonusPerUnit;
    }

    public static TerrainType SampleTerrainAt(Vector3 position)
    {
        if (TerrainGenerator.Instance == null) return TerrainType.Plains;
        return TerrainGenerator.Instance.GetTerrainTypeAtPosition(position);
    }

}
