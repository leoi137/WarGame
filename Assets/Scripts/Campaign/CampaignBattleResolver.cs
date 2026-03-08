using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auto-resolves AI-vs-AI campaign battles.
/// </summary>
public static class CampaignBattleResolver
{
    public static BattleResult ResolveAutomatic(CampaignAction action, FactionCampaignState attacker, FactionCampaignState defender, ProvinceDefinition province, BattleRandom rng)
    {
        var attackerUnits = action?.committedUnits?.ToDictionary() ?? new Dictionary<string, int>();
        var defenderUnits = CampaignAI.AllocateArmy(defender, 0.5f);

        var terrain = province?.terrain ?? TerrainType.Plains;
        float attackerPower = CalculateArmyPower(attackerUnits, terrain);
        float defenderPower = province != null ? province.garrison * 10f : 0f;
        defenderPower += CalculateArmyPower(defenderUnits, terrain);

        bool attackerWins = attackerPower >= defenderPower;
        string winnerId = attackerWins ? attacker.factionId : defender.factionId;
        string loserId = attackerWins ? defender.factionId : attacker.factionId;

        int winnerStart = attackerWins ? CountUnits(attackerUnits) : CountUnits(defenderUnits);
        int loserStart = attackerWins ? CountUnits(defenderUnits) : CountUnits(attackerUnits);

        float powerRatio = attackerWins ? defenderPower / Mathf.Max(0.01f, attackerPower) : attackerPower / Mathf.Max(0.01f, defenderPower);
        float casualtyRatio = Mathf.Clamp01(powerRatio * 0.5f + rng.Range(0f, 0.1f));
        int winnerCasualties = Mathf.RoundToInt(winnerStart * casualtyRatio);
        int winnerSurvivors = Mathf.Max(0, winnerStart - winnerCasualties);

        return new BattleResult
        {
            winnerFactionId = winnerId,
            loserFactionId = loserId,
            winningSide = attackerWins ? Faction.Attacker : Faction.Defender,
            winnerSurvivors = winnerSurvivors,
            winnerStartCount = winnerStart,
            loserStartCount = loserStart,
            battleDurationSeconds = 0f,
            totalCasualties = winnerCasualties + loserStart
        };
    }

    public static float CalculateArmyPower(Dictionary<string, int> units, TerrainType terrain)
    {
        float total = 0f;
        if (units == null) return total;

        foreach (var kv in units)
        {
            float mod = GetTerrainModifier(kv.Key, terrain);
            var def = UnitDatabase.Get(kv.Key);
            if (def != null)
                total += (def.maxHP + def.attackDamage * 2 + def.armor) * mod * kv.Value;
        }
        return total;
    }

    public static float GetTerrainModifier(string unitTypeId, TerrainType terrain)
    {
        var def = UnitDatabase.Get(unitTypeId);
        if (def == null) return 1f;

        switch (terrain)
        {
            case TerrainType.Forest:
            case TerrainType.Jungle:
                return def.category == UnitCategory.LightInfantry || def.category == UnitCategory.Ranged ? 1.1f : 0.9f;
            case TerrainType.Mountains:
            case TerrainType.Hills:
                return def.category == UnitCategory.HeavyCavalry || def.category == UnitCategory.LightCavalry ? 0.8f : 1f;
            case TerrainType.Coast:
            case TerrainType.RiverValley:
                return def.category == UnitCategory.Naval ? 1.2f : 1f;
            case TerrainType.Desert:
            case TerrainType.Steppe:
                return def.category == UnitCategory.LightCavalry ? 1.1f : 0.95f;
            default:
                return 1f;
        }
    }

    private static int CountUnits(Dictionary<string, int> units)
    {
        if (units == null) return 0;
        int n = 0;
        foreach (var kv in units) n += kv.Value;
        return n;
    }
}
