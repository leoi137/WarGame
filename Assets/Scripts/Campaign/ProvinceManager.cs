using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages runtime province ownership and queries.
/// </summary>
public class ProvinceManager
{
    private readonly List<ProvinceDefinition> _provinces = new();
    private readonly Dictionary<string, ProvinceDefinition> _byId = new();
    private readonly Dictionary<string, string> _ownership = new();

    public void Initialize(List<ProvinceDefinition> provinces)
    {
        _provinces.Clear();
        _byId.Clear();
        _ownership.Clear();

        foreach (var p in provinces)
        {
            if (p == null || string.IsNullOrEmpty(p.id)) continue;
            _provinces.Add(p);
            _byId[p.id] = p;
            _ownership[p.id] = p.ownerFactionId ?? "";
        }
    }

    public List<ProvinceDefinition> GetProvincesForFaction(string factionId)
    {
        return _provinces.Where(p => _ownership.TryGetValue(p.id, out var owner) && owner == factionId).ToList();
    }

    public List<ProvinceDefinition> GetAdjacentEnemyProvinces(string factionId)
    {
        var owned = new HashSet<string>(GetProvincesForFaction(factionId).Select(p => p.id));
        var result = new HashSet<ProvinceDefinition>();

        foreach (var myProvince in GetProvincesForFaction(factionId))
        {
            foreach (var adjId in myProvince.adjacentProvinceIds ?? new List<string>())
            {
                if (owned.Contains(adjId)) continue;
                if (_byId.TryGetValue(adjId, out var adj) && _ownership.TryGetValue(adjId, out var owner) && owner != factionId && !string.IsNullOrEmpty(owner))
                    result.Add(adj);
            }
        }

        return result.ToList();
    }

    public void TransferProvince(string provinceId, string newOwnerId)
    {
        if (_ownership.ContainsKey(provinceId))
            _ownership[provinceId] = newOwnerId ?? "";
    }

    public int CalculateIncome(string factionId)
    {
        return GetProvincesForFaction(factionId).Sum(p => p.baseIncome);
    }

    public bool HasPathToCapital(string factionId, string provinceId)
    {
        var capital = GetCapitalProvince(factionId);
        if (capital == null || provinceId == capital.id) return true;

        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(provinceId);
        visited.Add(provinceId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == capital.id) return true;

            if (!_byId.TryGetValue(current, out var prov)) continue;
            foreach (var adjId in prov.adjacentProvinceIds ?? new List<string>())
            {
                if (visited.Contains(adjId)) continue;
                if (!_ownership.TryGetValue(adjId, out var owner) || owner != factionId) continue;
                visited.Add(adjId);
                queue.Enqueue(adjId);
            }
        }

        return false;
    }

    public string GetOwner(string provinceId)
    {
        return _ownership.TryGetValue(provinceId, out var owner) ? owner : null;
    }

    public ProvinceDefinition GetProvince(string provinceId)
    {
        return _byId.TryGetValue(provinceId, out var p) ? p : null;
    }

    private ProvinceDefinition GetCapitalProvince(string factionId)
    {
        var faction = FactionDatabase.Get(factionId);
        var capital = faction?.GetCapital();
        if (capital == null) return null;
        return _byId.TryGetValue(capital.id, out var prov) ? prov : null;
    }
}
