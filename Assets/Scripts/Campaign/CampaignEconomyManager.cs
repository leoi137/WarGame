using System.Collections.Generic;

/// <summary>
/// Economy logic for campaign: income, recruitment, upkeep.
/// </summary>
public static class CampaignEconomyManager
{
    private const int UpkeepPerUnit = 2;

    public static void CollectIncome(FactionCampaignState faction, ProvinceManager provinceManager)
    {
        if (faction == null || faction.isEliminated) return;
        faction.gold += provinceManager.CalculateIncome(faction.factionId);
    }

    public static bool CanRecruit(FactionCampaignState faction, string unitTypeId, int count)
    {
        if (faction == null || faction.isEliminated || count <= 0) return false;
        int cost = GetRecruitCost(unitTypeId) * count;
        return faction.gold >= cost;
    }

    public static void Recruit(FactionCampaignState faction, string unitTypeId, int count)
    {
        if (!CanRecruit(faction, unitTypeId, count)) return;

        int cost = GetRecruitCost(unitTypeId) * count;
        faction.gold -= cost;

        var dict = faction.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        if (!dict.TryGetValue(unitTypeId, out var current))
            current = 0;
        dict[unitTypeId] = current + count;
        faction.armyComposition.FromDictionary(dict);
    }

    public static void PayUpkeep(FactionCampaignState faction)
    {
        if (faction == null || faction.isEliminated) return;
        int upkeep = GetUpkeepCost(faction);
        faction.gold = System.Math.Max(0, faction.gold - upkeep);
    }

    public static int GetRecruitCost(string unitTypeId)
    {
        var def = UnitDatabase.Get(unitTypeId);
        if (def == null) return 0;
        return (int)(def.maxHP + def.attackDamage * 2 + def.armor * 3);
    }

    public static int GetUpkeepCost(FactionCampaignState faction)
    {
        if (faction?.armyComposition == null) return 0;
        var dict = faction.armyComposition.ToDictionary();
        int totalUnits = 0;
        foreach (var kv in dict)
            totalUnits += kv.Value;
        return totalUnits * UpkeepPerUnit;
    }
}
