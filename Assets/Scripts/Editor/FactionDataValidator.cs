using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor tool for validating faction data integrity and exporting balance reports.
/// </summary>
public class FactionDataValidator : Editor
{
    [MenuItem("WorldWars/Validate All Factions", false, 51)]
    static void ValidateAllFactions()
    {
        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        var factions = FactionDatabase.GetAll();
        int errors = 0;

        foreach (var f in factions)
            errors += ValidateFaction(f);

        if (errors == 0)
            Debug.Log($"<color=green>All {factions.Count} factions validated successfully.</color>");
        else
            Debug.LogError($"Faction validation found {errors} issues across {factions.Count} factions.");
    }

    static int ValidateFaction(FactionDefinition faction)
    {
        int issues = 0;

        if (string.IsNullOrEmpty(faction.id))
        { Debug.LogError("Faction has null/empty ID"); issues++; }

        if (faction.unitTypes == null || faction.unitTypes.Count < 4)
        { Debug.LogError($"{faction.id}: fewer than 4 unit types"); issues++; }

        if (faction.cities == null || faction.cities.Count < 3)
        { Debug.LogError($"{faction.id}: fewer than 3 cities"); issues++; }

        if (faction.cities != null)
        {
            int capitals = faction.cities.Count(c => c.isCapital);
            if (capitals != 1)
            { Debug.LogError($"{faction.id}: {capitals} capitals (expected 1)"); issues++; }
        }

        if (faction.estimatedMilitary <= 0)
        { Debug.LogError($"{faction.id}: estimatedMilitary <= 0"); issues++; }

        return issues;
    }

    [MenuItem("WorldWars/Print Faction Summary", false, 52)]
    static void PrintFactionSummary()
    {
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        var factions = FactionDatabase.GetAll();
        var summary = "=== Faction Summary ===\n\n";
        summary += $"{"Faction",-25} {"Region",-15} {"Military",10} {"Budget",8} {"Units",6} {"Cities",7}\n";
        summary += new string('-', 75) + "\n";

        foreach (var f in factions.OrderBy(f => f.region.ToString()).ThenBy(f => f.displayName))
        {
            summary += $"{f.displayName,-25} {f.region,-15} {f.estimatedMilitary,10:N0} {f.GetBattleUnitBudget(),8} {f.unitTypes?.Count ?? 0,6} {f.cities?.Count ?? 0,7}\n";
        }

        summary += $"\nTotal: {factions.Count} factions, {UnitDatabase.GetAll().Count} unit types";
        Debug.Log(summary);
    }

    [MenuItem("WorldWars/Export Balance Report", false, 53)]
    static void ExportBalanceReport()
    {
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        var units = UnitDatabase.GetAll();
        var report = "=== Unit Balance Report ===\n\n";

        var byCategory = units.GroupBy(u => u.category).OrderBy(g => g.Key);
        foreach (var group in byCategory)
        {
            float avgHP = group.Average(u => u.maxHP);
            float avgATK = group.Average(u => u.attackDamage);
            float avgArmor = group.Average(u => u.armor);
            float avgSpeed = group.Average(u => u.moveSpeed);
            report += $"{group.Key,-20} Count={group.Count(),3}  HP={avgHP,6:F1}  ATK={avgATK,5:F1}  ARM={avgArmor,5:F1}  SPD={avgSpeed,4:F1}\n";
        }

        Debug.Log(report);
    }
}
