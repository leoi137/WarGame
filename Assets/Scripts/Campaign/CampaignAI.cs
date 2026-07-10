using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AI faction decisions during campaign turns. Symmetric for all factions.
/// </summary>
public static class CampaignAI
{
    private const float DefaultCommitRatio = 0.5f;

    public static CampaignAction DecideAction(FactionCampaignState faction, CampaignState state, ProvinceManager provinceManager, BattleRandom rng)
    {
        if (faction == null || faction.isEliminated) return null;

        float attackScore = EvaluateAttack(null, faction, provinceManager);
        float defenseScore = EvaluateDefense(faction, provinceManager, state);

        if (defenseScore > attackScore)
        {
            return new CampaignAction { type = CampaignAction.ActionType.Defend, sourceFactionId = faction.factionId };
        }

        var enemies = provinceManager.GetAdjacentEnemyProvinces(faction.factionId);
        if (enemies.Count == 0)
        {
            if (TryRecruit(faction, out var recruitAction))
                return recruitAction;
            return new CampaignAction { type = CampaignAction.ActionType.Defend, sourceFactionId = faction.factionId };
        }

        var weakest = enemies
            .OrderBy(e => provinceManager.GetProvince(e.id)?.garrison ?? 0)
            .ThenBy(e => EvaluateAttack(e.id, faction, provinceManager))
            .FirstOrDefault();

        if (weakest == null) return new CampaignAction { type = CampaignAction.ActionType.Defend, sourceFactionId = faction.factionId };

        var committed = AllocateArmy(faction, DefaultCommitRatio);
        if (committed.Count == 0)
        {
            if (TryRecruit(faction, out var recruitAction))
                return recruitAction;
            return new CampaignAction { type = CampaignAction.ActionType.Defend, sourceFactionId = faction.factionId };
        }

        var action = new CampaignAction
        {
            type = CampaignAction.ActionType.Attack,
            sourceFactionId = faction.factionId,
            targetProvinceId = weakest.id,
            committedUnits = new SerializableKeyValueList()
        };
        action.committedUnits.FromDictionary(committed);
        return action;
    }

    public static float EvaluateAttack(string targetProvinceId, FactionCampaignState faction, ProvinceManager provinceManager)
    {
        var army = faction?.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        if (army.Count == 0) return 0f;

        float ourPower = 0f;
        foreach (var kv in army)
        {
            var def = UnitDatabase.Get(kv.Key);
            if (def != null)
                ourPower += (def.maxHP + def.attackDamage * 2 + def.armor) * kv.Value;
        }

        if (string.IsNullOrEmpty(targetProvinceId)) return ourPower;

        var prov = provinceManager.GetProvince(targetProvinceId);
        if (prov == null) return ourPower;

        float theirStrength = prov.garrison * 10f;
        return ourPower - theirStrength;
    }

    public static float EvaluateDefense(FactionCampaignState faction, ProvinceManager provinceManager, CampaignState state)
    {
        var ourProvinces = provinceManager.GetProvincesForFaction(faction.factionId);
        var enemyProvinces = provinceManager.GetAdjacentEnemyProvinces(faction.factionId);

        float armyPower = 0f;
        var army = faction?.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        foreach (var kv in army)
        {
            var def = UnitDatabase.Get(kv.Key);
            if (def != null)
                armyPower += (def.maxHP + def.attackDamage * 2 + def.armor) * kv.Value;
        }

        float provinceScore = ourProvinces.Count * 20f;
        float threatScore = enemyProvinces.Count * -15f;
        return armyPower + provinceScore + threatScore;
    }

    public static Dictionary<string, int> AllocateArmy(FactionCampaignState faction, float commitRatio)
    {
        var result = new Dictionary<string, int>();
        var army = faction?.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();

        foreach (var kv in army)
        {
            int commit = (int)(kv.Value * commitRatio);
            if (commit > 0)
                result[kv.Key] = commit;
        }

        return result;
    }

    private static bool TryRecruit(FactionCampaignState faction, out CampaignAction action)
    {
        action = null;
        var unitTypes = UnitDatabase.GetForFaction(faction.factionId);
        if (unitTypes == null || unitTypes.Count == 0) return false;

        foreach (var ut in unitTypes)
        {
            int cost = CampaignEconomyManager.GetRecruitCost(ut.id);
            if (cost <= 0) continue;
            int affordable = faction.gold / cost;
            if (affordable > 0)
            {
                action = new CampaignAction
                {
                    type = CampaignAction.ActionType.Recruit,
                    sourceFactionId = faction.factionId,
                    recruitUnitTypeId = ut.id,
                    recruitCount = System.Math.Min(affordable, 5)
                };
                return true;
            }
        }
        return false;
    }
}
