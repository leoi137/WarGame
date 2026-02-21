using System;
using UnityEngine;

[Serializable]
public class TerrainDefinition
{
    public TerrainType type;
    public string displayName;
    public float movementMultiplier;
    public float cavalrySpeedMultiplier;
    public float infantryDefenseBonus;
    public float rangedAccuracyModifier;
    public float visibilityRange;
    public float elevationScale;
    public Color groundColor;
    public Color accentColor;
    public float treeDensity;
    public float rockDensity;
}
