using System.Collections.Generic;

/// <summary>
/// Flat registry of all unit types across all factions, built from FactionDatabase.
/// </summary>
public static class UnitDatabase
{
    private static readonly Dictionary<string, UnitTypeDefinition> _byId = new();
    private static readonly Dictionary<string, List<UnitTypeDefinition>> _byFaction = new();
    private static readonly List<UnitTypeDefinition> _all = new();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        _byId.Clear();
        _byFaction.Clear();
        _all.Clear();

        if (FactionDatabase.FactionCount == 0)
            FactionDatabase.Initialize();

        foreach (var faction in FactionDatabase.GetAll())
        {
            if (faction.unitTypes == null) continue;

            var factionUnits = new List<UnitTypeDefinition>(faction.unitTypes.Count);
            foreach (var unit in faction.unitTypes)
            {
                _byId[unit.id] = unit;
                _all.Add(unit);
                factionUnits.Add(unit);
            }
            _byFaction[faction.id] = factionUnits;
        }
    }

    public static UnitTypeDefinition Get(string id)
    {
        if (!_initialized) Initialize();
        return _byId.TryGetValue(id, out var u) ? u : null;
    }

    public static List<UnitTypeDefinition> GetAll()
    {
        if (!_initialized) Initialize();
        return _all;
    }

    public static List<UnitTypeDefinition> GetForFaction(string factionId)
    {
        if (!_initialized) Initialize();
        return _byFaction.TryGetValue(factionId, out var list) ? list : new List<UnitTypeDefinition>();
    }
}
