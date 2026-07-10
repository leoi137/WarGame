using System.Collections.Generic;
using UnityEngine;

public static class TerrainAnalyzer
{
    const int SampleGridStep = 10;
    const float ChokeElevationDelta = 0.15f;

    public static Vector3 FindBestDefensivePosition(Vector3 near, float radius, UnitCategory category)
    {
        if (TerrainGenerator.Instance == null) return near;
        Vector3 best = near;
        float bestScore = EvaluatePositionScore(near, category, null);
        int steps = Mathf.Max(2, Mathf.RoundToInt(radius / 5f));
        for (int x = -steps; x <= steps; x++)
        {
            for (int z = -steps; z <= steps; z++)
            {
                Vector3 candidate = near + new Vector3(x * 5f, 0, z * 5f);
                float score = EvaluatePositionScore(candidate, category, null);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }
        }
        return best;
    }

    public static Vector3 FindBestRangedPosition(Vector3 near, List<Unit> enemies, float range)
    {
        if (TerrainGenerator.Instance == null) return near;
        Vector3 best = near;
        float bestScore = float.MinValue;
        int steps = Mathf.Max(2, Mathf.RoundToInt(range / 8f));
        for (int x = -steps; x <= steps; x++)
        {
            for (int z = -steps; z <= steps; z++)
            {
                Vector3 candidate = near + new Vector3(x * 4f, 0, z * 4f);
                float score = EvaluatePositionScore(candidate, UnitCategory.Ranged, enemies);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }
        }
        return best;
    }

    public static bool HasTerrainAdvantage(Vector3 position, UnitCategory category)
    {
        float defBonus = TerrainEffects.GetDefenseBonus(position, category);
        TerrainType t = TerrainEffects.SampleTerrainAt(position);
        bool isForest = t == TerrainType.Forest;
        bool isInfantry = category == UnitCategory.HeavyInfantry || category == UnitCategory.LightInfantry;
        return defBonus > 0f || (isForest && isInfantry);
    }

    public static float EvaluatePositionScore(Vector3 position, UnitCategory category, List<Unit> enemies)
    {
        float score = 0f;
        if (TerrainGenerator.Instance != null)
        {
            float height = TerrainGenerator.Instance.GetHeightAtPosition(position);
            score += height * 2f;
        }
        score += TerrainEffects.GetDefenseBonus(position, category) * 3f;
        score += TerrainEffects.GetRangedAccuracy(position) * 0.5f;

        if (enemies != null && enemies.Count > 0)
        {
            float avgDist = 0f;
            foreach (Unit e in enemies)
            {
                if (e == null || e.isDead) continue;
                avgDist += Vector3.Distance(position, e.transform.position);
            }
            avgDist /= enemies.Count;
            if (category == UnitCategory.Ranged && avgDist > 10f) score += 1f;
        }
        return score;
    }

    public static List<Vector3> FindChokePoints(int mapSize)
    {
        var chokePoints = new List<Vector3>();
        if (TerrainGenerator.Instance == null) return chokePoints;

        int steps = mapSize / SampleGridStep;
        for (int x = 1; x < steps; x++)
        {
            for (int z = 1; z < steps; z++)
            {
                Vector3 pos = new Vector3(x * SampleGridStep, 0, z * SampleGridStep);
                float h = TerrainGenerator.Instance.GetHeightAtPosition(pos);
                float hN = TerrainGenerator.Instance.GetHeightAtPosition(pos + Vector3.forward * SampleGridStep);
                float hS = TerrainGenerator.Instance.GetHeightAtPosition(pos - Vector3.forward * SampleGridStep);
                float hE = TerrainGenerator.Instance.GetHeightAtPosition(pos + Vector3.right * SampleGridStep);
                float hW = TerrainGenerator.Instance.GetHeightAtPosition(pos - Vector3.right * SampleGridStep);
                float deltaN = Mathf.Abs(h - hN);
                float deltaS = Mathf.Abs(h - hS);
                float deltaE = Mathf.Abs(h - hE);
                float deltaW = Mathf.Abs(h - hW);
                if (deltaN > ChokeElevationDelta || deltaS > ChokeElevationDelta ||
                    deltaE > ChokeElevationDelta || deltaW > ChokeElevationDelta)
                    chokePoints.Add(pos);
            }
        }
        return chokePoints;
    }

    public static Vector3 FindForestCover(Vector3 near, float radius)
    {
        if (TerrainGenerator.Instance == null) return near;
        int steps = Mathf.Max(2, Mathf.RoundToInt(radius / 4f));
        for (int x = -steps; x <= steps; x++)
        {
            for (int z = -steps; z <= steps; z++)
            {
                Vector3 candidate = near + new Vector3(x * 4f, 0, z * 4f);
                if (TerrainEffects.SampleTerrainAt(candidate) == TerrainType.Forest)
                    return candidate;
            }
        }
        return near;
    }
}
