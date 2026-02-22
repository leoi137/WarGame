using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Aggregates all regional faction files into a single queryable registry.
/// Call Initialize() once at startup before any lookups.
/// </summary>
public static class FactionDatabase
{
    private static readonly Dictionary<string, FactionDefinition> _byId = new();
    private static readonly Dictionary<Region, List<FactionDefinition>> _byRegion = new();
    private static readonly List<FactionDefinition> _all = new();
    private static bool _initialized;

    public static int FactionCount => _all.Count;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        _byId.Clear();
        _byRegion.Clear();
        _all.Clear();

        RegisterBatch(EuropeFactions.Create());
        RegisterBatch(MiddleEastFactions.Create());
        RegisterBatch(SouthAsiaFactions.Create());
        RegisterBatch(EastAsiaFactions.Create());
        RegisterBatch(SoutheastAsiaFactions.Create());
        RegisterBatch(AfricaFactions.Create());
        RegisterBatch(AmericasFactions.Create());
        RegisterBatch(OceaniaFactions.Create());
    }

    private static void RegisterBatch(List<FactionDefinition> factions)
    {
        foreach (var f in factions)
        {
            _byId[f.id] = f;
            _all.Add(f);

            if (!_byRegion.TryGetValue(f.region, out var list))
            {
                list = new List<FactionDefinition>();
                _byRegion[f.region] = list;
            }
            list.Add(f);
        }
    }

    public static FactionDefinition Get(string id)
    {
        if (!_initialized) Initialize();
        return _byId.TryGetValue(id, out var f) ? f : null;
    }

    public static List<FactionDefinition> GetAll()
    {
        if (!_initialized) Initialize();
        return _all;
    }

    public static List<FactionDefinition> GetByRegion(Region region)
    {
        if (!_initialized) Initialize();
        return _byRegion.TryGetValue(region, out var list) ? list : new List<FactionDefinition>();
    }
}
