using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Immutable-style faction payload used by world map, battle setup, and campaign systems.
/// </summary>
[Serializable]
public class FactionDefinition
{
    public string id;
    public string displayName;
    public Region region;
    public string capitalCityId;
    public Color primaryColor;
    public Color secondaryColor;
    public int estimatedMilitary;
    public string rulerName;
    public string rulerBonus;
    public string factionTrait;
    public string factionTraitDescription;
    public string strategicAsset;
    public string strategicAssetDescription;
    public Dictionary<TerrainType, float> terrainDistribution = new();
    public List<CityDefinition> cities = new();
    public List<UnitTypeDefinition> unitTypes = new();

    public int GetBattleUnitBudget()
    {
        return Mathf.Clamp(estimatedMilitary / GameConfig.UnitBudgetScaleFactor, 48, GameConfig.MaxUnitsPerSide);
    }

    public CityDefinition GetCapital()
    {
        if (string.IsNullOrWhiteSpace(capitalCityId) || cities == null)
        {
            return null;
        }

        return cities.Find(c => c != null && string.Equals(c.id, capitalCityId, StringComparison.Ordinal));
    }

    public TerrainType GetDominantTerrain()
    {
        if (terrainDistribution == null || terrainDistribution.Count == 0)
        {
            return TerrainType.Plains;
        }

        return terrainDistribution.OrderByDescending(kv => kv.Value).First().Key;
    }
}
