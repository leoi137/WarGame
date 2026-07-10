using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor-only debug tools for rapid battle testing and balance analysis.
/// </summary>
public class BattleDebugTools : Editor
{
    [MenuItem("WorldWars/Debug/Quick Battle (Byzantine vs Song)", false, 200)]
    static void QuickBattleByzantineVsSong()
    {
        QuickBattle("byzantine", "song");
    }

    [MenuItem("WorldWars/Debug/Quick Battle (Viking vs Viking)", false, 201)]
    static void QuickBattleMirror()
    {
        QuickBattle("north_sea_empire", "north_sea_empire");
    }

    static void QuickBattle(string factionAId, string factionBId)
    {
        InitDatabases();

        var a = FactionDatabase.Get(factionAId);
        var b = FactionDatabase.Get(factionBId);
        if (a == null) { Debug.LogError($"Faction not found: {factionAId}"); return; }
        if (b == null) { Debug.LogError($"Faction not found: {factionBId}"); return; }

        var config = BattleConfiguration.Create(a, b);
        Debug.Log($"Battle configured: {a.displayName} ({config.attackerUnitBudget} units) vs {b.displayName} ({config.defenderUnitBudget} units) on {config.primaryTerrain}");
    }

    [MenuItem("WorldWars/Debug/Stress Test (100 battles)", false, 210)]
    static void StressTest()
    {
        InitDatabases();

        var factions = FactionDatabase.GetAll();
        var rng = new BattleRandom(42);
        int success = 0;
        int errors = 0;

        for (int i = 0; i < 100; i++)
        {
            var a = factions[rng.Range(0, factions.Count)];
            var b = factions[rng.Range(0, factions.Count)];
            try
            {
                var config = BattleConfiguration.Create(a, b);
                if (config != null && config.attackerUnitBudget > 0 && config.defenderUnitBudget > 0)
                    success++;
                else
                    errors++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Stress test battle {i}: {a.id} vs {b.id} — {e.Message}");
                errors++;
            }
        }

        Debug.Log($"<color=cyan>Stress test complete: {success}/100 succeeded, {errors} failed.</color>");
    }

    [MenuItem("WorldWars/Debug/Balance Test (all vs all)", false, 211)]
    static void BalanceTest()
    {
        InitDatabases();

        var factions = FactionDatabase.GetAll();
        var report = "=== Balance Matrix (Budget) ===\n\n";
        report += $"{"Faction",-25} {"Budget",8}\n";
        report += new string('-', 35) + "\n";

        foreach (var f in factions.OrderByDescending(f => f.GetBattleUnitBudget()))
        {
            report += $"{f.displayName,-25} {f.GetBattleUnitBudget(),8}\n";
        }

        int min = factions.Min(f => f.GetBattleUnitBudget());
        int max = factions.Max(f => f.GetBattleUnitBudget());
        report += $"\nRange: {min} — {max} (ratio {(float)max / min:F1}×)";

        Debug.Log(report);
    }

    static void InitDatabases()
    {
        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();
    }
}
