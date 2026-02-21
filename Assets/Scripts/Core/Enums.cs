using UnityEngine;

/// <summary>
/// High-level game flow states for the WorldWars 2.0 state machine.
/// </summary>
public enum GameFlowState
{
    MainMenu,
    WorldMap,
    FactionSelect,
    BattleSetup,
    BattleSimulation,
    BattleResults,
    CampaignSetup,
    CampaignMap,
    CampaignBattle,
    CampaignTurnResolve,
    CampaignVictory,
    CampaignDefeat
}

/// <summary>
/// Sub-phases that occur inside an active battle.
/// </summary>
public enum BattlePhase
{
    Loading,
    Placement,
    Countdown,
    Simulating,
    Ended,
    Results
}

public enum TerrainType
{
    Plains,
    Forest,
    Hills,
    Mountains,
    Steppe,
    Desert,
    RiverValley,
    Coast,
    Wetlands,
    Jungle
}

public enum UnitCategory
{
    HeavyInfantry,
    LightInfantry,
    HeavyCavalry,
    LightCavalry,
    Ranged,
    Siege,
    Elephant,
    Naval,
    Special
}

public enum ArmorStyle { None, Light, Medium, Heavy, Robes, Fur }
public enum HelmetStyle { None, Hood, Conical, Nasal, Spectacle, Turban, Straw, Feathered, Crown, Wrapped }
public enum WeaponStyle { Sword, Axe, Spear, Bow, Crossbow, Club, Mace, Javelin, Sling, DualAxe, DualSword, Atlatl, Elephant, None }
public enum ShieldStyle { Round, Kite, Tower, Buckler, None }
public enum MaterialPreset { Skin, Chainmail, Steel, Gold, Leather, Fur, Wood, Bone, Cloth, Silk, Lacquer, Bronze, Obsidian, Cotton }
public enum Region { Europe, MiddleEast, SouthAsia, EastAsia, SoutheastAsia, Africa, Americas, Oceania }
public enum AbilityTrigger { OnLowHP, OnNearEnemy, OnCooldown, OnKill, Passive, OnAttack }
public enum Faction
{
    // New battle-system naming
    Attacker,
    Defender,

    // Legacy naming kept during migration phases to avoid hard breakage.
    North,
    South
}
