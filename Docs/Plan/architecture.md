# WorldWars 2.0 — Technical Architecture

## System Architecture

This document defines the complete technical architecture for the 1016 AD Global War Simulator. It covers data models (exact C# class definitions), system interactions, game flow, and the file structure with descriptions.

---

## Game Flow State Machine

### Quick Battle Flow
```
┌──────────┐   Quick Battle   ┌──────────────┐   Select A   ┌─────────────┐
│          │ ───────────────> │              │ ──────────> │             │
│ MainMenu │                  │ FactionSelect│             │ FactionSel. │
│          │ <─────────────── │  (Attacker)  │ <────────── │ (Defender)  │
└──────────┘   Back           └──────────────┘   Back      └─────────────┘
     │                                                           │
     │  World Map                                                │ Confirm
     ▼                                                           ▼
┌──────────┐   Pick Factions  ┌──────────────┐   Confirm   ┌─────────────┐
│          │ ───────────────> │              │ ──────────> │   Battle    │
│ WorldMap │                  │ BattleSetup  │             │ Simulation  │
│          │ <─────────────── │ (Placement)  │             │  (AI vs AI) │
└──────────┘   Back           └──────────────┘             └─────────────┘
     ▲                                                           │
     │                        ┌──────────────┐                   │ One side
     └─────── Return ──────── │   Battle     │ <─────────────────┘ eliminated
                              │   Results    │
                              └──────────────┘
```

### Campaign Conquest Flow
```
┌──────────┐   Campaign    ┌──────────────┐   Pick Faction  ┌─────────────┐
│          │ ────────────> │  Campaign    │ ─────────────> │  Campaign   │
│ MainMenu │               │  Setup       │                │  Map        │ ◄────────┐
│          │               └──────────────┘                └─────────────┘          │
└──────────┘                                                    │                   │
                                                                │ Attack            │
                                                                ▼                   │
                                                          ┌─────────────┐          │
                                                          │ BattleSetup │          │
                                                          │ + Simulation│          │
                                                          └─────────────┘          │
                                                                │                   │
                                                                │ Result            │
                                                                ▼                   │
                                                          ┌─────────────┐          │
                                                          │  Campaign   │──────────┘
                                                          │ TurnResolve │   (next turn)
                                                          └─────────────┘
                                                                │ Victory/Defeat
                                                                ▼
                                                          ┌─────────────┐
                                                          │  Campaign   │
                                                          │  Victory/   │ → MainMenu
                                                          │  Defeat     │
                                                          └─────────────┘
```

---

## Data Model Specifications

These are the exact C# class definitions to implement. All classes are plain C# (not MonoBehaviour) unless noted.

### Core Enums (Enums.cs)

```csharp
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
    Plains,      // PL — flat grassland, fast movement
    Forest,      // FO — dense trees, slow cav, good inf defense
    Hills,       // HI — rolling terrain, moderate bonuses
    Mountains,   // MT — dramatic peaks, very slow, high defense
    Steppe,      // ST — open grass, cavalry paradise
    Desert,      // DE — sand dunes, hot, no cover
    RiverValley, // RV — central river, crossing penalty
    Coast,       // CO — beach, water edge, naval bonus
    Wetlands,    // SW — marshy, slow everything, esp cavalry
    Jungle       // JN — dense canopy, very slow, high defense
}

public enum UnitCategory
{
    HeavyInfantry,  // Tanks, shield walls, slow and tough
    LightInfantry,  // Fast melee, flankers, skirmishers
    HeavyCavalry,   // Armored riders, devastating charges
    LightCavalry,   // Horse archers, scouts, harassers
    Ranged,          // Archers, crossbowmen, slingers
    Siege,           // Slow, high damage, area effect
    Elephant,        // Massive HP, terror, area knockback
    Naval,           // Coast-bonus units, boarding
    Special          // Commanders, support, unique
}

public enum ArmorStyle { None, Light, Medium, Heavy, Robes, Fur }
public enum HelmetStyle { None, Hood, Conical, Nasal, Spectacle, Turban, Straw, Feathered, Crown, Wrapped }
public enum WeaponStyle { Sword, Axe, Spear, Bow, Crossbow, Club, Mace, Javelin, Sling, DualAxe, DualSword, Atlatl, Elephant, None }
public enum ShieldStyle { Round, Kite, Tower, Buckler, None }
public enum MaterialPreset { Skin, Chainmail, Steel, Gold, Leather, Fur, Wood, Bone, Cloth, Silk, Lacquer, Bronze, Obsidian, Cotton }
public enum Region { Europe, MiddleEast, SouthAsia, EastAsia, SoutheastAsia, Africa, Americas, Oceania }
public enum AbilityTrigger { OnLowHP, OnNearEnemy, OnCooldown, OnKill, Passive, OnAttack }
public enum Faction { Attacker, Defender }
```

### FactionDefinition

```csharp
[System.Serializable]
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
    public Dictionary<TerrainType, float> terrainDistribution;
    public List<CityDefinition> cities;
    public List<UnitTypeDefinition> unitTypes;

    public int GetBattleUnitBudget()
    {
        return Mathf.Clamp(estimatedMilitary / GameConfig.UnitBudgetScaleFactor, 10, 40);
    }

    public CityDefinition GetCapital()
    {
        return cities.Find(c => c.id == capitalCityId);
    }

    public TerrainType GetDominantTerrain()
    {
        return terrainDistribution.OrderByDescending(kv => kv.Value).First().Key;
    }
}
```

### CityDefinition

```csharp
[System.Serializable]
public class CityDefinition
{
    public string id;
    public string displayName;
    public int garrison;
    public Vector2 normalizedPosition; // x=0-1 (longitude), y=0-1 (latitude)
    public TerrainType primaryTerrain;
    public TerrainType secondaryTerrain;
    public bool isCapital;
}
```

### UnitTypeDefinition

```csharp
[System.Serializable]
public class UnitTypeDefinition
{
    public string id;          // e.g. "nse_huscarl"
    public string factionId;   // owning faction
    public string displayName; // e.g. "Huscarl"
    public UnitCategory category;
    public float maxHP;        // 40-200
    public float attackDamage; // 5-30
    public float attackRange;  // 1.5-15.0
    public float attackCooldown; // 0.5-2.0
    public float moveSpeed;    // 2.0-5.0
    public float armor;        // 0-15
    public string abilityId;   // nullable, ref to AbilityDatabase
    public UnitVisualConfig visualConfig;
    public string description; // short tooltip text
}
```

### UnitVisualConfig

```csharp
[System.Serializable]
public class UnitVisualConfig
{
    public float bodyScale = 1.0f;
    public float shoulderWidth = 0.28f;
    public float hipWidth = 0.12f;
    public ArmorStyle armorStyle = ArmorStyle.Medium;
    public HelmetStyle helmetStyle = HelmetStyle.None;
    public WeaponStyle primaryWeapon = WeaponStyle.Sword;
    public WeaponStyle secondaryWeapon = WeaponStyle.None;
    public ShieldStyle shieldStyle = ShieldStyle.None;
    public bool hasCape = false;
    public bool hasBackItem = false;
    public Color armorTint = new Color(0.48f, 0.5f, 0.52f);
    public Color clothTint = new Color(0.4f, 0.4f, 0.4f);
    public Color skinTint = new Color(0.85f, 0.7f, 0.55f);
    public MaterialPreset armorMaterial = MaterialPreset.Chainmail;
    public MaterialPreset weaponMaterial = MaterialPreset.Steel;
}
```

### AbilityDefinition

```csharp
[System.Serializable]
public class AbilityDefinition
{
    public string id;
    public string displayName;
    public AbilityTrigger trigger;
    public float triggerThreshold;
    public float cooldown;
    public float duration;
    public float damageModifier = 1.0f;
    public float speedModifier = 1.0f;
    public float armorModifier = 0f;
    public float incomingDamageModifier = 1.0f;
    public string description;

    // Condition fields — these limit when/against whom the ability activates
    // Null means no restriction (applies to all targets / all terrains)
    public UnitCategory[] targetCategoryCondition; // e.g., {HeavyCavalry, LightCavalry} for pike_brace
    public TerrainType[] terrainCondition;         // e.g., {Coast} for naval_boarding, {Forest, Jungle} for ambush
    public string specialCondition;                // Free-form: "AllyWithin5m", "FirstAttack", etc.
}
```

### TerrainDefinition

```csharp
[System.Serializable]
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
```

### BattleConfiguration

```csharp
[System.Serializable]
public class BattleConfiguration
{
    public FactionDefinition attackerFaction;
    public FactionDefinition defenderFaction;
    public CityDefinition battleLocation;
    public TerrainType primaryTerrain;
    public TerrainType secondaryTerrain;
    public int attackerUnitBudget;
    public int defenderUnitBudget;
    public int mapSize;
    public int randomSeed;

    public static BattleConfiguration Create(
        FactionDefinition attacker,
        FactionDefinition defender,
        CityDefinition location = null)
    {
        var config = new BattleConfiguration();
        config.attackerFaction = attacker;
        config.defenderFaction = defender;
        config.battleLocation = location ?? defender.GetCapital();
        config.primaryTerrain = config.battleLocation.primaryTerrain;
        config.secondaryTerrain = config.battleLocation.secondaryTerrain;
        config.attackerUnitBudget = attacker.GetBattleUnitBudget();
        config.defenderUnitBudget = defender.GetBattleUnitBudget();
        config.mapSize = GameConfig.DefaultMapSize;
        config.randomSeed = System.Environment.TickCount;
        return config;
    }
}
```

### BattleResult

```csharp
[System.Serializable]
public class BattleResult
{
    public string winnerFactionId;
    public string loserFactionId;
    public Faction winningSide;
    public int winnerSurvivors;
    public int winnerStartCount;
    public int loserStartCount;
    public float battleDurationSeconds;
    public int totalCasualties;
    public Dictionary<string, int> attackerUnitLosses;
    public Dictionary<string, int> defenderUnitLosses;
    public Dictionary<string, int> attackerUnitKills;
    public Dictionary<string, int> defenderUnitKills;
}
```

---

## Event System

The EventBus uses a dictionary of `Type → List<Delegate>` for type-safe pub/sub:

```csharp
public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> _handlers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Delegate>();
        _handlers[type].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.ContainsKey(type))
            _handlers[type].Remove(handler);
    }

    public static void Publish<T>(T eventData)
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type)) return;
        foreach (var handler in _handlers[type].ToList())
            ((Action<T>)handler)(eventData);
    }

    public static void Clear()
    {
        _handlers.Clear();
    }
}
```

### Event Types

```csharp
public struct GameStateChangedEvent { public GameFlowState oldState, newState; }
public struct FactionSelectedEvent { public FactionDefinition faction; public bool isAttacker; }
public struct BattleStartedEvent { public BattleConfiguration config; }
public struct BattleEndedEvent { public BattleResult result; }
public struct UnitDiedEvent { public Unit unit; }
public struct UnitDamagedEvent { public Unit unit; public float damage; public bool isMarked; }

// Campaign Events
public struct CampaignPlayerTurnStartedEvent { public int turn; public int year; }
public struct CampaignTurnResolvedEvent { public int turn; public List<BattleResult> results; }
public struct CampaignProvinceTransferredEvent { public string provinceId; public string oldOwner; public string newOwner; }
public struct CampaignBattleStartedEvent { public string attackerFactionId; public string defenderFactionId; public string provinceId; }
public struct CampaignBattleAutoResolvedEvent { public BattleResult result; }
public struct CampaignWonEvent { public string factionId; public int finalTurn; }
public struct CampaignLostEvent { public string factionId; public int finalTurn; }
public struct CampaignFactionEliminatedEvent { public string factionId; public string eliminatedBy; }
public struct CampaignSavedEvent { public string campaignId; }
public struct CampaignLoadedEvent { public string campaignId; }
public struct AbilityActivatedEvent { public Unit unit; public AbilityDefinition ability; }
public struct PlacementConfirmedEvent { public Faction side; }
```

---

## System Interaction Diagram

```
User Input
    │
    ├──> WorldMapInput ──> WorldMapManager ──> FactionInfoPanel
    │                           │
    │                           ▼
    │                     GameManager ──────────> EventBus
    │                      │       │                 │
    ├──> BattleSetup ──────┘       │                 ├──> BattleHUD
    │    (placement)               │                 ├──> BattleResultsUI
    │                              │                 └──> FactionSelectUI
    │                              ▼
    │                        BattleManager
    │                         │        │
    │                         │        ▼
    │                         │   BattleSimulator
    │                         │     │         │
    │                         │     ▼         ▼
    │                         │  SimulationAI SimulationAI
    │                         │  (Attacker)   (Defender)
    │                         │     │              │
    │                         │     └──────┬───────┘
    │                         │            ▼
    │                         │     Unit Combat Loop
    │                         │     ├── UnitCombat
    │                         │     ├── UnitMovement
    │                         │     ├── AbilitySystem
    │                         │     ├── TerrainEffects
    │                         │     └── Projectile
    │                         │            │
    │                         │            ▼
    │                         │     VFX / Animation
    │                         │     ├── UnitAnimator
    │                         │     ├── TrailEffect
    │                         │     ├── DamagePopup
    │                         │     └── CameraShake
    │                         │
    │                         ▼
    │                   TerrainGenerator
    │                   ├── ElevationGenerator
    │                   ├── RiverGenerator
    │                   ├── VegetationGenerator
    │                   └── BiomeDefinitions
    │
    └──> BattleCamera / WorldMapCamera
```

---

## Combat Damage Formula

```
baseDamage = attacker.attackDamage
effectiveDamage = baseDamage * AbilitySystem.GetDamageMultiplier(attacker)
armorReduction = target.armor + AbilitySystem.GetArmorModifier(target) + TerrainEffects.GetDefenseBonus(target.position, target.category)
elevationBonus = TerrainEffects.GetElevationAdvantage(attacker.position, target.position)
markMultiplier = target.isMarked ? 1.4 : 1.0

finalDamage = max(1, (effectiveDamage - armorReduction) * markMultiplier * (1 + elevationBonus))
```

---

## AI Decision Tree (per unit, per tick)

```
1. Am I dead? → Skip
2. Do I have an active ability to process? → AbilitySystem.ProcessAbilities()
3. Am I at low HP and should retreat? → TacticalDecisionMaker.ShouldRetreat()
   → YES: Move toward retreat position
4. Do I have a current target?
   → YES: Is target still alive and in range?
     → YES: Attack (UnitCombat handles the rest)
     → NO: Clear target, go to 5
5. Select new target → TargetSelector.SelectTarget()
   → Per category scoring:
     - HeavyInfantry: nearest enemy, prefer other melee
     - LightInfantry: weak/isolated enemies, flank
     - HeavyCavalry: ranged/weak enemies, charge
     - LightCavalry: ranged enemies, kite and harass
     - Ranged: marked > low HP > nearest, avoid shields
     - Elephant: clustered enemies, charge through
6. Move toward target → UnitMovement.MoveToTarget()
7. If formation is broken → FormationController.ReformFormation()
```

---

## Unit Category Stat Guidelines

These ranges ensure balance across all factions:

| Category | HP | ATK | DEF | SPD | Range | CD |
|----------|-----|-----|-----|-----|-------|------|
| HeavyInfantry | 100-160 | 8-15 | 5-12 | 2.5-3.5 | 2.0-3.0 | 0.8-1.6 |
| LightInfantry | 60-90 | 12-22 | 1-4 | 3.5-4.5 | 2.0-3.0 | 0.5-0.9 |
| HeavyCavalry | 90-130 | 15-25 | 4-8 | 4.0-5.0 | 2.5-3.5 | 0.8-1.2 |
| LightCavalry | 55-80 | 10-16 | 1-3 | 4.5-5.5 | 3.0-8.0 | 0.9-1.4 |
| Ranged | 40-65 | 8-14 | 0-2 | 3.0-4.5 | 10-16 | 1.0-1.6 |
| Siege | 150-250 | 25-40 | 2-5 | 1.5-2.5 | 12-18 | 2.5-4.0 |
| Elephant | 180-300 | 20-30 | 6-10 | 2.0-3.0 | 3.0-4.0 | 1.5-2.0 |
| Naval | 70-100 | 10-15 | 2-5 | 3.5-4.5 | 2.5-3.5 | 0.8-1.2 |
| Special | 60-100 | 5-12 | 2-6 | 3.0-4.0 | 2.0-10 | 1.0-2.0 |

---

## Regional Visual Config Defaults

Each region has a default visual style for its units. Individual factions override as needed.

### Europe
- **Skin:** (0.85, 0.7, 0.55)
- **Armor:** Chainmail/Steel, Nasal/Spectacle helms, Kite/Round shields
- **Weapons:** Swords, axes, spears, longbows, crossbows
- **Materials:** Chainmail, Steel, Leather, Fur, Cloth
- **Capes:** Yes (faction colored)

### Middle East / Central Asia
- **Skin:** (0.75, 0.6, 0.45)
- **Armor:** Lamellar (Medium), Robes, Turban/Conical helms
- **Weapons:** Curved swords, bows, spears, maces
- **Materials:** Steel, Silk, Leather, Gold
- **Capes:** Robes instead

### South Asia
- **Skin:** (0.65, 0.45, 0.3)
- **Armor:** Light/Medium, Conical helms
- **Weapons:** Swords, spears, bows, clubs (elephants)
- **Materials:** Bronze, Silk, Cotton, Gold
- **Capes:** Light cloth

### East Asia
- **Skin:** (0.9, 0.78, 0.6)
- **Armor:** Lacquered Medium/Heavy, Conical/Straw helms
- **Weapons:** Swords, spears, crossbows, pikes
- **Materials:** Lacquer, Silk, Steel, Wood
- **Capes:** No (armor-focused)

### Southeast Asia
- **Skin:** (0.7, 0.5, 0.35)
- **Armor:** Light/None, Wrapped/Crown helms
- **Weapons:** Spears, swords, bows, clubs
- **Materials:** Bronze, Wood, Cloth, Gold
- **Capes:** Light cloth

### Africa
- **Skin:** (0.45, 0.3, 0.2)
- **Armor:** None/Light, Wrapped helms
- **Weapons:** Spears, swords, bows, clubs, javelins
- **Materials:** Leather, Wood, Bone, Gold
- **Capes:** Fur/hide

### Americas
- **Skin:** (0.7, 0.5, 0.3)
- **Armor:** Cotton/None, Feathered helms
- **Weapons:** Clubs, atlatls, slings, obsidian swords
- **Materials:** Obsidian, Cotton, Wood, Bone, Gold
- **Capes:** Feathered cloaks

### Oceania
- **Skin:** (0.55, 0.38, 0.25)
- **Armor:** None, Wrapped helms
- **Weapons:** Clubs, spears, slings
- **Materials:** Wood, Bone
- **Capes:** No (minimal armor)

---

## World Map Coordinate System

The world map uses a normalized 0-1 coordinate system:
- **X axis:** Longitude (0 = leftmost, ~-170°W, 1 = rightmost, ~170°E)
- **Y axis:** Latitude (0 = bottom, ~-60°S, 1 = top, ~75°N)

This maps to world space: `worldX = normalized.x * WorldMapWidth`, `worldZ = normalized.y * WorldMapHeight`.

### Continental Position Anchors

| Region | Approx X Range | Approx Y Range |
|--------|---------------|---------------|
| Europe | 0.45-0.55 | 0.60-0.80 |
| Middle East | 0.53-0.65 | 0.45-0.65 |
| Central Asia | 0.60-0.72 | 0.55-0.70 |
| South Asia | 0.65-0.72 | 0.35-0.55 |
| East Asia | 0.72-0.85 | 0.45-0.70 |
| Southeast Asia | 0.73-0.82 | 0.25-0.40 |
| North Africa | 0.45-0.55 | 0.45-0.55 |
| Sub-Saharan Africa | 0.47-0.57 | 0.25-0.45 |
| Mesoamerica | 0.15-0.22 | 0.40-0.50 |
| South America | 0.20-0.30 | 0.15-0.35 |
| Oceania | 0.85-0.95 | 0.15-0.25 |

---

## Singleton Pattern (for MonoBehaviour singletons)

All singletons should follow this pattern for consistency:

```csharp
public class ExampleManager : MonoBehaviour
{
    public static ExampleManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
```

---

## Elephant Unit Model Specification

Elephants are NOT humanoid — they do not use the standard 14-pivot skeleton. They are built as a large multi-primitive animal with a rider on top.

### Elephant Skeleton (8 pivots)

```
Elephant Root (transform, scale 2.5x)
  +-- Pivot_Body (Y=0.8)                    -- main body box
  |     +-- Pivot_Head (Z=+0.6, Y=+0.1)    -- head box
  |     |     +-- Pivot_Trunk (Z=+0.2, Y=-0.1) -- trunk cylinder
  |     |     +-- Tusk_L (Z=+0.15, X=-0.15)
  |     |     +-- Tusk_R (Z=+0.15, X=+0.15)
  |     +-- Pivot_Rider (Y=+0.5)            -- rider on top (simplified humanoid)
  |     +-- Pivot_Tail (Z=-0.6, Y=+0.1)     -- tail cylinder
  +-- Pivot_FrontL (X=-0.3, Z=+0.3)         -- front left leg cylinder
  +-- Pivot_FrontR (X=+0.3, Z=+0.3)         -- front right leg cylinder
  +-- Pivot_RearL (X=-0.3, Z=-0.3)          -- rear left leg cylinder
  +-- Pivot_RearR (X=+0.3, Z=-0.3)          -- rear right leg cylinder
```

### Elephant Primitives

| Part | Primitive | Scale | Material | Notes |
|------|-----------|-------|----------|-------|
| Body | Cube | (1.2, 0.8, 1.6) | Leather (gray) | Main mass |
| Head | Sphere | (0.6, 0.5, 0.5) | Leather (gray) | Slightly smaller than body |
| Trunk | Cylinder | (0.08, 0.3, 0.08) | Leather (gray) | Hangs from head |
| Tusks | Cylinder | (0.03, 0.2, 0.03) | Bone (white) | Curved outward, rotated 30° |
| Ears | Cube | (0.01, 0.25, 0.2) | Leather (gray) | Thin slabs on head sides |
| Legs (x4) | Cylinder | (0.15, 0.4, 0.15) | Leather (gray) | Thick columns |
| Tail | Cylinder | (0.03, 0.2, 0.03) | Leather (gray) | Behind body |
| Rider | Cube | (0.2, 0.35, 0.15) | Faction armor | Simplified torso on top |
| Rider Head | Sphere | (0.15, 0.16, 0.15) | Skin | On rider torso |
| Howdah | Cube | (0.5, 0.2, 0.4) | Wood | Platform on elephant back |
| War Paint | — | — | Faction color emission | Stripes on body |

### Elephant Animation

- **Idle:** Subtle body sway, trunk swing, ear flap
- **Walk:** Legs alternate (FR+RL, FL+RR), body rocks side-to-side, trunk swings
- **Attack:** Head lowers, trunk swings forward (knockback), stomp front legs
- **Death:** Legs buckle sequentially, body tilts and falls sideways, rider thrown off

### Elephant Combat

- Melee range 3.5 (larger body hitbox)
- `elephant_charge` ability: on first attack, 2x damage + area knockback + speed burst
- Fear passive: enemies within 5m move 15% slower
- No weapon model — the elephant IS the weapon

---

## Seeded Random Pattern

**CRITICAL:** Battles must be deterministic. Given the same seed and starting positions, the outcome must be identical every time.

### Implementation Rules

1. **Never use `UnityEngine.Random`** for any gameplay-affecting randomness. UnityEngine.Random has global state that is non-deterministic across frames.
2. **Use `System.Random` with explicit seed** for all battle randomness.
3. **One `System.Random` instance per battle**, created from `BattleConfiguration.randomSeed`.
4. **Pass the random instance** to all systems that need randomness (AI, combat, abilities).

### Code Pattern

```csharp
public class BattleRandom
{
    private System.Random _rng;

    public BattleRandom(int seed)
    {
        _rng = new System.Random(seed);
    }

    public float Range(float min, float max)
    {
        return (float)(_rng.NextDouble() * (max - min) + min);
    }

    public int Range(int min, int maxExclusive)
    {
        return _rng.Next(min, maxExclusive);
    }

    public bool Chance(float probability)
    {
        return _rng.NextDouble() < probability;
    }

    public float Value => (float)_rng.NextDouble();
}
```

### Where BattleRandom Is Used

- `SimulationAI` — target selection tie-breaking, stance evaluation noise
- `UnitCombat` — proc chance for abilities (dual_strike 30%, poison_arrow 30%, etc.)
- `AbilitySystem` — trigger evaluation noise
- `BattlefieldGenerator` — terrain generation (Perlin noise seed)
- `VegetationGenerator` — tree/rock placement

### Where BattleRandom Is NOT Used

- UI effects (damage popup drift, camera shake noise) — these are cosmetic and can use `UnityEngine.Random`
- Particle effects, trail rendering — cosmetic
- World map — no randomness needed

---

## Campaign Mode Data Models

The game supports a multi-battle conquest campaign where you conquer the map over time.

### CampaignState

```csharp
[System.Serializable]
public class CampaignState
{
    public string playerFactionId;
    public int currentTurn;
    public int year; // starts at 1016
    public Dictionary<string, string> provinceOwnership; // provinceId → factionId
    public Dictionary<string, FactionCampaignState> factionStates;
    public List<string> defeatedFactions;
    public List<BattleResult> battleHistory;
    public string activeBattleId; // null if not in battle
}
```

### FactionCampaignState

```csharp
[System.Serializable]
public class FactionCampaignState
{
    public string factionId;
    public int goldIncome;
    public int currentGold;
    public int legitimacy; // 0-100
    public int totalArmyStrength;
    public List<string> controlledCityIds;
    public List<string> adjacentEnemyCityIds;
    public Dictionary<string, int> armyComposition; // unitTypeId → count
    public bool isPlayerControlled;
    public bool isDefeated;
    public int turnsAlive;
}
```

### ProvinceDefinition

```csharp
[System.Serializable]
public class ProvinceDefinition
{
    public string id;
    public string displayName;
    public string ownerFactionId; // starting owner
    public Vector2 normalizedPosition;
    public TerrainType primaryTerrain;
    public int baseIncome;
    public bool hasCity;
    public string cityId; // nullable, links to CityDefinition
    public List<string> adjacentProvinceIds;
}
```

### CampaignAction

```csharp
[System.Serializable]
public class CampaignAction
{
    public string factionId;
    public string actionType; // "attack", "reinforce", "recruit", "defend", "diplomacy"
    public string sourceProvinceId;
    public string targetProvinceId;
    public Dictionary<string, int> unitCommitment; // unitTypeId → count
}
```

### Campaign Turn Flow

```
1. Player reviews map → sees controlled provinces, armies, income
2. Player selects a province to attack (or defend, recruit, etc.)
3. Player commits units from garrison to the attack
4. AI factions simultaneously make their moves
5. Battles resolve (player battles are interactive placement + simulation)
6. AI-vs-AI battles resolve automatically with result summary
7. Income collected, units recruited, provinces change hands
8. Check victory conditions (control X% of map or all factions defeated)
9. Next turn → year advances
```

---

## Supabase Persistence Layer (Interface-Ready)

The game must be structured to support Supabase backend persistence in the future. Do NOT implement Supabase yet — instead, define interfaces that the game uses for all save/load operations. The current implementation uses local in-memory storage behind these interfaces.

### IPersistenceService

```csharp
public interface IPersistenceService
{
    Task<bool> SaveCampaignState(CampaignState state);
    Task<CampaignState> LoadCampaignState(string campaignId);
    Task<List<CampaignState>> ListCampaigns();
    Task<bool> DeleteCampaign(string campaignId);
    Task<bool> SaveBattleResult(BattleResult result, string campaignId);
    Task<List<BattleResult>> GetBattleHistory(string campaignId);
    Task<bool> SavePlayerProfile(PlayerProfile profile);
    Task<PlayerProfile> LoadPlayerProfile(string playerId);
}
```

### PlayerProfile

```csharp
[System.Serializable]
public class PlayerProfile
{
    public string playerId;
    public string displayName;
    public int totalBattles;
    public int totalWins;
    public int totalLosses;
    public string favoriteFactionId;
    public List<string> completedCampaigns;
    public Dictionary<string, int> factionWinCounts; // factionId → wins
}
```

### LocalPersistenceService (Initial Implementation)

```csharp
public class LocalPersistenceService : IPersistenceService
{
    // Uses PlayerPrefs or JSON files in Application.persistentDataPath
    // Implements all interface methods with local storage
    // This is swapped for SupabasePersistenceService later
}
```

### Supabase Table Schema (Reference for Future)

```sql
-- campaigns
CREATE TABLE campaigns (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    player_id UUID REFERENCES profiles(id),
    faction_id TEXT NOT NULL,
    current_turn INT DEFAULT 1,
    year INT DEFAULT 1016,
    state JSONB NOT NULL,
    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);

-- battle_results
CREATE TABLE battle_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    campaign_id UUID REFERENCES campaigns(id),
    attacker_faction_id TEXT NOT NULL,
    defender_faction_id TEXT NOT NULL,
    winner_faction_id TEXT NOT NULL,
    duration_seconds FLOAT,
    total_casualties INT,
    details JSONB,
    created_at TIMESTAMPTZ DEFAULT now()
);

-- profiles
CREATE TABLE profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    display_name TEXT,
    total_battles INT DEFAULT 0,
    total_wins INT DEFAULT 0,
    stats JSONB,
    created_at TIMESTAMPTZ DEFAULT now()
);
```

---

## Performance Budgets

| Metric | Target | Notes |
|--------|--------|-------|
| Battle FPS | 30+ | With 40 units per side (80 total) |
| World Map FPS | 60 | Static scene, no combat |
| Unit spawn time | < 50ms each | Procedural model construction |
| Terrain generation | < 2s | Including NavMesh bake |
| Memory | < 500MB | All factions loaded |
| AI decision time | < 1ms per unit | 80 units × 0.05s interval |
| Battle completion | 30-120s sim time | At 1x speed |

---

## File Naming Conventions

- **Scripts:** PascalCase matching class name (e.g., `BattleManager.cs`)
- **Faction data files:** RegionFactions (e.g., `EuropeFactions.cs`)
- **Test files:** SystemNameTests (e.g., `CombatMathTests.cs`)
- **Folders:** PascalCase (e.g., `WorldMap/`, `Battle/`)
- **Faction IDs:** snake_case (e.g., `north_sea_empire`)
- **Unit type IDs:** factionprefix_unitname (e.g., `nse_huscarl`, `byz_cataphract`)
- **City IDs:** factionprefix_cityname (e.g., `nse_london`, `byz_constantinople`)
- **Ability IDs:** snake_case (e.g., `berserker_rage`, `shield_wall`)
