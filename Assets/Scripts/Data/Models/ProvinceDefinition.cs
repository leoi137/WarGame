using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProvinceDefinition
{
    public string id;
    public string displayName;
    public string region;
    public string ownerFactionId;
    public int baseIncome;
    public int garrison;
    public TerrainType terrain;
    public Vector2 normalizedPosition;
    public List<string> adjacentProvinceIds = new List<string>();
}
