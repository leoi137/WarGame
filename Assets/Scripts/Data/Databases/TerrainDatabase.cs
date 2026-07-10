using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static registry of terrain type definitions with combat modifiers.
/// Values sourced from code-todo.md terrain balance table.
/// </summary>
public static class TerrainDatabase
{
    private static readonly Dictionary<TerrainType, TerrainDefinition> _byType = new();
    private static readonly List<TerrainDefinition> _all = new();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        _byType.Clear();
        _all.Clear();

        Register(new TerrainDefinition {
            type = TerrainType.Plains, displayName = "Plains",
            movementMultiplier = 1.0f, cavalrySpeedMultiplier = 1.2f,
            infantryDefenseBonus = 0f, rangedAccuracyModifier = 1.0f,
            visibilityRange = 1.0f, elevationScale = 0.2f,
            groundColor = new Color(0.45f, 0.65f, 0.25f),
            accentColor = new Color(0.55f, 0.72f, 0.30f),
            treeDensity = 0.05f, rockDensity = 0.02f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Forest, displayName = "Forest",
            movementMultiplier = 0.7f, cavalrySpeedMultiplier = 0.5f,
            infantryDefenseBonus = 3f, rangedAccuracyModifier = 0.7f,
            visibilityRange = 0.5f, elevationScale = 0.4f,
            groundColor = new Color(0.22f, 0.42f, 0.15f),
            accentColor = new Color(0.18f, 0.35f, 0.12f),
            treeDensity = 0.8f, rockDensity = 0.1f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Hills, displayName = "Hills",
            movementMultiplier = 0.8f, cavalrySpeedMultiplier = 0.8f,
            infantryDefenseBonus = 2f, rangedAccuracyModifier = 1.1f,
            visibilityRange = 1.2f, elevationScale = 0.7f,
            groundColor = new Color(0.52f, 0.55f, 0.32f),
            accentColor = new Color(0.45f, 0.42f, 0.28f),
            treeDensity = 0.2f, rockDensity = 0.35f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Mountains, displayName = "Mountains",
            movementMultiplier = 0.5f, cavalrySpeedMultiplier = 0.3f,
            infantryDefenseBonus = 5f, rangedAccuracyModifier = 1.2f,
            visibilityRange = 1.5f, elevationScale = 1.0f,
            groundColor = new Color(0.55f, 0.55f, 0.55f),
            accentColor = new Color(0.70f, 0.70f, 0.72f),
            treeDensity = 0.1f, rockDensity = 0.6f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Steppe, displayName = "Steppe",
            movementMultiplier = 1.0f, cavalrySpeedMultiplier = 1.4f,
            infantryDefenseBonus = 0f, rangedAccuracyModifier = 1.0f,
            visibilityRange = 1.0f, elevationScale = 0.15f,
            groundColor = new Color(0.72f, 0.68f, 0.35f),
            accentColor = new Color(0.78f, 0.72f, 0.38f),
            treeDensity = 0.02f, rockDensity = 0.03f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Desert, displayName = "Desert",
            movementMultiplier = 0.8f, cavalrySpeedMultiplier = 1.0f,
            infantryDefenseBonus = -1f, rangedAccuracyModifier = 1.0f,
            visibilityRange = 1.3f, elevationScale = 0.3f,
            groundColor = new Color(0.85f, 0.78f, 0.55f),
            accentColor = new Color(0.80f, 0.72f, 0.48f),
            treeDensity = 0f, rockDensity = 0.08f
        });

        Register(new TerrainDefinition {
            type = TerrainType.RiverValley, displayName = "River Valley",
            movementMultiplier = 0.6f, cavalrySpeedMultiplier = 0.6f,
            infantryDefenseBonus = 1f, rangedAccuracyModifier = 0.9f,
            visibilityRange = 0.8f, elevationScale = 0.2f,
            groundColor = new Color(0.35f, 0.55f, 0.32f),
            accentColor = new Color(0.30f, 0.50f, 0.55f),
            treeDensity = 0.3f, rockDensity = 0.05f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Coast, displayName = "Coast",
            movementMultiplier = 0.9f, cavalrySpeedMultiplier = 0.9f,
            infantryDefenseBonus = 0f, rangedAccuracyModifier = 1.0f,
            visibilityRange = 1.0f, elevationScale = 0.1f,
            groundColor = new Color(0.78f, 0.75f, 0.55f),
            accentColor = new Color(0.45f, 0.60f, 0.72f),
            treeDensity = 0.1f, rockDensity = 0.04f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Wetlands, displayName = "Wetlands",
            movementMultiplier = 0.5f, cavalrySpeedMultiplier = 0.3f,
            infantryDefenseBonus = 1f, rangedAccuracyModifier = 0.8f,
            visibilityRange = 0.6f, elevationScale = 0.1f,
            groundColor = new Color(0.32f, 0.45f, 0.28f),
            accentColor = new Color(0.28f, 0.38f, 0.30f),
            treeDensity = 0.15f, rockDensity = 0.02f
        });

        Register(new TerrainDefinition {
            type = TerrainType.Jungle, displayName = "Jungle",
            movementMultiplier = 0.6f, cavalrySpeedMultiplier = 0.4f,
            infantryDefenseBonus = 4f, rangedAccuracyModifier = 0.6f,
            visibilityRange = 0.3f, elevationScale = 0.5f,
            groundColor = new Color(0.15f, 0.38f, 0.12f),
            accentColor = new Color(0.12f, 0.32f, 0.10f),
            treeDensity = 0.9f, rockDensity = 0.12f
        });
    }

    private static void Register(TerrainDefinition def)
    {
        _byType[def.type] = def;
        _all.Add(def);
    }

    public static TerrainDefinition Get(TerrainType type)
    {
        if (!_initialized) Initialize();
        return _byType.TryGetValue(type, out var def) ? def : null;
    }

    public static List<TerrainDefinition> GetAll()
    {
        if (!_initialized) Initialize();
        return _all;
    }
}
