using System;
using UnityEngine;

[Serializable]
public class CityDefinition
{
    public string id;
    public string displayName;
    public int garrison;
    public Vector2 normalizedPosition;
    public TerrainType primaryTerrain;
    public TerrainType secondaryTerrain;
    public bool isCapital;
}
