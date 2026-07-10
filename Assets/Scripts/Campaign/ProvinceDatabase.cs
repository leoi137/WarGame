using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Builds province data from FactionDatabase. Each faction's cities become provinces.
/// Adjacency: same-faction cities are adjacent; cross-faction when distance &lt; 0.15.
/// </summary>
public static class ProvinceDatabase
{
    private static readonly List<ProvinceDefinition> _all = new();
    private static readonly Dictionary<string, List<ProvinceDefinition>> _byRegion = new();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        _all.Clear();
        _byRegion.Clear();

        if (FactionDatabase.FactionCount == 0)
            FactionDatabase.Initialize();

        var allProvinces = new List<ProvinceDefinition>();
        foreach (var faction in FactionDatabase.GetAll())
        {
            if (faction.cities == null) continue;

            foreach (var city in faction.cities)
            {
                if (city == null || string.IsNullOrEmpty(city.id)) continue;

                var province = new ProvinceDefinition
                {
                    id = city.id,
                    displayName = city.displayName ?? city.id,
                    region = faction.region.ToString(),
                    ownerFactionId = faction.id,
                    baseIncome = Mathf.Max(1, city.garrison / 10),
                    garrison = city.garrison,
                    terrain = city.primaryTerrain,
                    normalizedPosition = city.normalizedPosition,
                    adjacentProvinceIds = new List<string>()
                };

                allProvinces.Add(province);
            }
        }

        ComputeAdjacency(allProvinces);

        foreach (var p in allProvinces)
        {
            _all.Add(p);
            var regionKey = p.region ?? "";
            if (!_byRegion.TryGetValue(regionKey, out var list))
            {
                list = new List<ProvinceDefinition>();
                _byRegion[regionKey] = list;
            }
            list.Add(p);
        }
    }

    private static void ComputeAdjacency(List<ProvinceDefinition> provinces)
    {
        var byId = provinces.ToDictionary(p => p.id);

        for (int i = 0; i < provinces.Count; i++)
        {
            var a = provinces[i];
            var aOwner = a.ownerFactionId;

            for (int j = i + 1; j < provinces.Count; j++)
            {
                var b = provinces[j];
                var bOwner = b.ownerFactionId;

                bool adjacent = aOwner == bOwner
                    ? true
                    : Vector2.Distance(a.normalizedPosition, b.normalizedPosition) < 0.15f;

                if (adjacent)
                {
                    if (!a.adjacentProvinceIds.Contains(b.id))
                        a.adjacentProvinceIds.Add(b.id);
                    if (!b.adjacentProvinceIds.Contains(a.id))
                        b.adjacentProvinceIds.Add(a.id);
                }
            }
        }
    }

    public static List<ProvinceDefinition> GetAllProvinces()
    {
        if (!_initialized) Initialize();
        return _all;
    }

    public static List<ProvinceDefinition> GetProvincesForRegion(string region)
    {
        if (!_initialized) Initialize();
        return _byRegion.TryGetValue(region ?? "", out var list) ? list : new List<ProvinceDefinition>();
    }
}
