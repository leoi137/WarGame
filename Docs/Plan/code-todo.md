# WorldWars 2.0 — Master Implementation Plan

## 1016 AD Global War Simulator

**Target:** Transform the existing Viking skirmish game into a full-scale 1016 AD global war simulator with 43 playable factions, world map, campaign conquest mode, terrain-specific battlefields, and pure AI-vs-AI battle simulation. Designed Supabase-ready for future backend persistence.

**Estimated Time:** ~12 hours autonomous coding

**Date Created:** 2026-02-21

---

## How to Use This Document

This is the **linear execution guide**. Work through each phase in order. Each phase lists:

1. **Goal** — What this phase accomplishes
2. **Prerequisites** — Phases that must be complete first
3. **Overview** — Implementation approach (2-4 sentences)
4. **Files** — Every file to create or modify, with function signatures and descriptions
5. **Tests** — Test names with 5-10 word behavior summaries
6. **Checklist** — Markable items to track completion

**Supporting Documents:**
- `architecture.md` — Complete file structure, data models, system diagrams
- `faction-data.md` — All 43 factions with units, cities, terrain (implementation-ready data)
- `Docs/Ideas/gpt5.2.md` — Detailed historical reference (43 factions, terrain codes, army estimates)
- `Docs/Ideas/grok4.2.md` — Epic gameplay packages (heroes, traits, strategic assets)

**Existing Code Reference:**
- `Docs/character-building.md` — Procedural unit construction system (skeleton, pivots, materials)

---

## Executive Summary

The game transforms from a single-battlefield Viking RTS into a **WC4-style global strategy simulator** set in 1016 AD. Two gameplay modes:

### Quick Battle Mode
1. **World Map** → Browse 43 historical factions across the globe
2. **Faction Select** → Pick two factions to pit against each other
3. **Battle Setup** → Place units on a terrain-specific battlefield (both sides)
4. **Simulate** → Press play; identical AI controls both armies
5. **Results** → Winner determined by strategy (composition, positioning, terrain)

### Campaign Conquest Mode
1. **Campaign Setup** → Pick your faction from the world map
2. **Campaign Map** → View your provinces, armies, income, adjacency
3. **Plan Turn** → Select a province to attack (or recruit, reinforce, defend)
4. **Battle** → Player-controlled battles use interactive setup + simulation; AI-vs-AI battles auto-resolve
5. **Post-Battle** → Provinces change hands, income collected, units recruited
6. **Victory** → Conquer the map (or X% threshold) across multiple turns/years

The existing combat engine (Unit, UnitCombat, UnitMovement, Projectile, etc.) is preserved and generalized. The Viking units become the **North Sea Empire** faction — one of 43.

**Key principles:**
- Strategy is in the setup, execution is automated and equal
- Campaign supports multi-battle conquest over time
- Architecture is Supabase-ready for future backend persistence (interfaces now, swap implementation later)
- All save/load operations go through `IPersistenceService`

---

## Architecture Overview

```
┌─────────────────────────────────────────────────┐
│                   GameManager                    │
│         (State Machine: Menu→Map→Battle)         │
├──────────┬──────────┬──────────┬────────────────┤
│ WorldMap │  Battle  │    UI    │     Core       │
│  Layer   │  Layer   │  Layer   │    Layer       │
├──────────┼──────────┼──────────┼────────────────┤
│ WorldMap │ Battle   │ MainMenu │ EventBus       │
│ Generator│ Manager  │ UI       │ GameConfig     │
│ Province │ Battle   │ Faction  │ FactionDB      │
│ Renderer │ Setup    │ SelectUI │ UnitDB         │
│ WorldMap │ Battle   │ Battle   │ TerrainDB      │
│ Camera   │ Simulator│ SetupUI  │ AbilityDB      │
│ WorldMap │ Bfield   │ Battle   │                │
│ Input    │ Generator│ HUD      │                │
│ Faction  │ Battle   │ Results  │                │
│ InfoPanel│ Camera   │ UI       │                │
│ CityMark │ TimeCtrl │ Tooltip  │                │
├──────────┼──────────┼──────────┼────────────────┤
│          │          Units Layer                  │
│          │  Unit / UnitFactory / UnitMovement    │
│          │  UnitCombat / UnitAnimator / Ability  │
│          │  UnitModelBuilder / HealthBar / Proj  │
├──────────┼──────────┼──────────┼────────────────┤
│          │     AI Layer        │  Terrain Layer  │
│          │  SimulationAI       │ TerrainGenerator│
│          │  TacticalDecision   │ TerrainEffects  │
│          │  TargetSelector     │ BiomeDefinitions│
│          │  FormationCtrl      │ RiverGen/MtnGen │
│          │  TerrainAnalyzer    │ VegetationGen   │
├──────────┴──────────┴──────────┴────────────────┤
│              Rendering / VFX Layer               │
│  ShaderHelper / TrailEffect / DamagePopup       │
│  CameraShake / FactionColors / MinimapRenderer  │
└─────────────────────────────────────────────────┘
```

**Game Flow State Machine:**
```
                    ┌──────────────────────────────────────────────────────┐
                    │                                                      │
MainMenu ─┬→ WorldMap → FactionSelect → BattleSetup → Simulation → Results
           │     ↑                                                    │
           │     └────────────────────────────────────────────────────┘
           │
           └→ CampaignSetup → CampaignMap ──→ CampaignBattle → BattleSetup → Simulation → Results
                                 ↑                                                           │
                                 │     CampaignTurnResolve ← ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─┘
                                 └──────────┘
                                 (loop until victory/defeat)
```

**Key Architectural Decisions:**
- **Data-driven factions**: All 43 factions defined in static C# data classes, not ScriptableObjects (keeps the fully-procedural paradigm)
- **Generic unit system**: `Unit.cs` refactored to build ANY unit type from a `UnitVisualConfig`, not just 4 hardcoded Vikings
- **Symmetric AI**: Both battle sides use the exact same `SimulationAI` class — zero advantage to either side
- **Terrain-aware battles**: Battlefield terrain generated from faction/city terrain data; terrain affects movement, defense, and combat
- **Campaign conquest**: Multi-battle campaign over turns, provinces change hands, income/recruitment cycle
- **Supabase-ready persistence**: All save/load operations go through `IPersistenceService` interface. Local implementation now, Supabase swap later
- **Existing code preserved**: Viking units become North Sea Empire faction; core combat systems (UnitCombat, Projectile, etc.) generalized, not rewritten
- **Event-driven architecture**: `EventBus` decouples systems; no more direct singleton references where avoidable

---

## Complete File Structure

See `architecture.md` for detailed descriptions of every file. Summary tree:

```
Assets/
  Scripts/
    Core/
      GameManager.cs              (NEW) State machine for game flow
      GameConfig.cs               (NEW) Global constants and tuning
      EventBus.cs                 (NEW) Pub/sub event system
      Enums.cs                    (NEW) All shared enums (GameFlowState, BattlePhase, TerrainType, etc.)
      BattleRandom.cs             (NEW) Seeded deterministic random (wraps System.Random)
      IPersistenceService.cs      (NEW) Interface for save/load operations
      LocalPersistenceService.cs  (NEW) JSON file-based local persistence
    Data/
      Models/
        FactionDefinition.cs      (NEW) Faction data class
        CityDefinition.cs         (NEW) City data class
        UnitTypeDefinition.cs     (NEW) Unit type data class
        UnitVisualConfig.cs       (NEW) Visual configuration for procedural unit models
        AbilityDefinition.cs      (NEW) Ability data class (includes condition fields)
        TerrainDefinition.cs      (NEW) Terrain type data class
        BattleConfiguration.cs    (NEW) Battle setup parameters
        BattleResult.cs           (NEW) Battle outcome data
        CampaignState.cs          (NEW) Campaign persistence models
        PlayerProfile.cs          (NEW) Player profile data class
      Databases/
        FactionDatabase.cs        (NEW) Registry of all 43 factions
        UnitDatabase.cs           (NEW) Registry of all unit types
        TerrainDatabase.cs        (NEW) Terrain type definitions and modifiers
        AbilityDatabase.cs        (NEW) All ability definitions
      Factions/
        EuropeFactions.cs         (NEW) 11 European faction definitions
        MiddleEastFactions.cs     (NEW) 8 Middle East/Central Asia factions
        SouthAsiaFactions.cs      (NEW) 4 South Asian factions
        EastAsiaFactions.cs       (NEW) 5 East Asian factions
        SoutheastAsiaFactions.cs  (NEW) 5 Southeast Asian factions
        AfricaFactions.cs         (NEW) 4 African factions
        AmericasFactions.cs       (NEW) 5 American factions
        OceaniaFactions.cs        (NEW) 1 Oceanian faction
    WorldMap/
      WorldMapManager.cs          (NEW) World map scene controller
      WorldMapGenerator.cs        (NEW) Procedural 2D world map with terrain coloring
      ProvinceRenderer.cs         (NEW) Renders faction territories with borders
      WorldMapCamera.cs           (NEW) Pan/zoom for world map
      WorldMapInput.cs            (NEW) Click-to-select factions and cities
      FactionInfoPanel.cs         (NEW) Info overlay when faction is selected
      CityMarker.cs               (NEW) City dot/icon on world map
    Battle/
      BattleManager.cs            (NEW) Battle scene master controller
      BattleSetup.cs              (NEW) Pre-battle unit placement phase
      BattleSimulator.cs          (NEW) Core simulation loop
      BattlefieldGenerator.cs     (NEW) Biome-aware terrain generation
      BattleCamera.cs             (NEW) Auto-follow battle camera
      BattleTimeController.cs     (NEW) Play/pause/speed controls
      BattleResultsScreen.cs      (NEW) Post-battle statistics and outcome
    Units/
      Unit.cs                     (MODIFY) Refactor to data-driven generic unit
      UnitFactory.cs              (NEW) Creates units from UnitTypeDefinition
      UnitModelBuilder.cs         (NEW) Extracted from Unit.cs — procedural model builder
      UnitMovement.cs             (KEEP) Minor updates for terrain speed modifiers
      UnitCombat.cs               (MODIFY) Generalize for any unit type and ability
      UnitAnimator.cs             (MODIFY) Support new weapon/armor styles via config
      HealthBar.cs                (KEEP) Minor updates for new unit names
      Projectile.cs               (KEEP) Already generic enough
      AbilitySystem.cs            (NEW) Generic ability trigger and effect processor
    AI/
      SimulationAI.cs             (NEW) Master AI — identical for both sides
      TacticalDecisionMaker.cs    (NEW) High-level decisions (advance, retreat, flank, hold)
      TargetSelector.cs           (NEW) Optimal target selection per unit category
      FormationController.cs      (NEW) Arranges units in formations
      TerrainAnalyzer.cs          (NEW) Evaluates terrain for positioning advantages
    Terrain/
      TerrainGenerator.cs         (NEW) Biome-specific procedural terrain (replaces MapGenerator for battles)
      TerrainEffects.cs           (NEW) Runtime terrain modifier calculations
      BiomeDefinitions.cs         (NEW) Visual + gameplay properties per biome
      RiverGenerator.cs           (NEW) Procedural river placement
      ElevationGenerator.cs       (NEW) Hills and mountains
      VegetationGenerator.cs      (NEW) Trees and vegetation per biome
    UI/
      MainMenuUI.cs               (NEW) Redesigned main menu
      FactionSelectUI.cs          (NEW) Faction picker with details
      BattleSetupUI.cs            (NEW) Unit placement controls
      BattleHUD.cs                (NEW) In-battle overlay
      BattleResultsUI.cs          (NEW) Results screen
      WorldMapHUD.cs              (NEW) World map overlay
      UIThemeManager.cs           (NEW) Consistent medieval UI styling
      TooltipSystem.cs            (NEW) Hover tooltips
      CampaignSetupUI.cs          (NEW) Campaign faction selection
      CampaignMapUI.cs            (NEW) Campaign world map with province ownership
      CampaignVictoryUI.cs        (NEW) Campaign victory/defeat screen
      CampaignHUD.cs              (NEW) Campaign persistent HUD (gold, turn, year)
      SaveLoadUI.cs               (NEW) Save slot management UI
    Campaign/
      CampaignManager.cs          (NEW) Campaign turn-based orchestrator
      CampaignAI.cs               (NEW) AI decisions for non-player factions
      CampaignBattleResolver.cs   (NEW) Auto-resolve AI-vs-AI battles
      ProvinceManager.cs          (NEW) Province ownership and adjacency
      ProvinceDatabase.cs         (NEW) Static province data with adjacency
      CampaignEconomyManager.cs   (NEW) Income, recruitment, upkeep
      CampaignAutoSave.cs         (NEW) Auto-save on turn resolve
      CampaignBalanceConfig.cs    (NEW) Campaign balance constants
    VFX/
      TrailEffect.cs              (KEEP) Weapon trails
      DamagePopup.cs              (KEEP) Floating damage numbers
      CameraShake.cs              (KEEP) Screen shake
      CombatVFX.cs                (NEW) Expanded combat visual effects
    Rendering/
      ShaderHelper.cs             (MODIFY) Add material presets for new cultures
      FactionColorPalette.cs      (NEW) Faction-specific color schemes
      MinimapRenderer.cs          (NEW) Battle minimap
    Legacy/
      GameBootstrap.cs            (MODIFY) Redirect to GameManager
      MapGenerator.cs             (KEEP) Preserved as fallback, used by TerrainGenerator
      SelectionManager.cs         (MODIFY) Repurpose for battle setup placement
      CommandManager.cs           (MODIFY) Repurpose for battle setup positioning
      FactionManager.cs           (MODIFY) Extend to support 43 factions
      AIController.cs             (KEEP) Preserved as reference, replaced by SimulationAI
      UnitSpawner.cs              (MODIFY) Generalize for any faction composition
      GameUI.cs                   (KEEP) Preserved as reference, replaced by new UI scripts
      UnitViewer.cs               (MODIFY) Support viewing any faction's units
    Editor/
      WorldWarsSetup.cs           (MODIFY) Update for new scene structure
      WorldWarsValidator.cs       (MODIFY) Validate all 43 factions and new systems
      FactionDataValidator.cs     (NEW) Validates all faction data integrity
      BattleDebugTools.cs         (NEW) Debug tools for battle testing
  Tests/
    EditMode/
      DataValidationTests.cs      (NEW) Faction, unit, city data integrity
      CombatMathTests.cs          (NEW) Damage, armor, ability calculations
      TerrainEffectTests.cs       (NEW) Terrain modifier calculations
      AIDecisionTests.cs          (NEW) AI targeting and formation logic
      AbilityTests.cs             (NEW) Ability triggers and effects
      FactionBalanceTests.cs      (NEW) Cross-faction balance metrics
      BattleResultTests.cs        (NEW) Battle outcome data integrity
      CampaignLogicTests.cs       (NEW) Campaign state, economy, AI, persistence
    PlayMode/
      BattleSimulationTests.cs    (NEW) Full battle simulation runs
      UnitSpawnTests.cs           (NEW) Unit creation from data
      TerrainGenerationTests.cs   (NEW) Biome terrain generation
      UINavigationTests.cs        (NEW) UI flow navigation
      IntegrationTests.cs         (NEW) End-to-end game flow
      WorldMapTests.cs            (NEW) World map rendering and interaction
      CampaignFlowTests.cs        (NEW) Campaign setup, turns, save/load
```

**File Count:**
- New files: ~80 (includes Campaign/, persistence, campaign UI, BattleRandom)
- Modified files: ~15
- Kept as-is: ~7
- Test files: ~15
- **Total: ~117 files**

---

## Phase 0: Project Restructure (~20 min)

**Goal:** Create the folder structure and move existing files to their new locations.

**Prerequisites:** None.

### Overview

Create all new directories under `Assets/Scripts/`. **IMPORTANT:** Existing scripts remain at their current physical path (`Assets/Scripts/*.cs`) — do NOT move them to a `Legacy/` subfolder. Moving files changes Unity `.meta` GUIDs and breaks scene references. The file structure tree shows them under `Legacy/` only for organizational clarity. The `code-todo.md` file references say `Assets/Scripts/Legacy/GameBootstrap.cs` but the executing AI should read this as "the existing file at `Assets/Scripts/GameBootstrap.cs`". New scripts go into their organized folders. The `Tests/` folder is created under `Assets/`.

### Tasks

#### Directory Creation

Create these directories:
```
Assets/Scripts/Core/
Assets/Scripts/Data/Models/
Assets/Scripts/Data/Databases/
Assets/Scripts/Data/Factions/
Assets/Scripts/WorldMap/
Assets/Scripts/Battle/
Assets/Scripts/Units/
Assets/Scripts/AI/
Assets/Scripts/Terrain/
Assets/Scripts/UI/
Assets/Scripts/VFX/
Assets/Scripts/Rendering/
Assets/Scripts/Campaign/
Assets/Scripts/Editor/
Assets/Tests/
Assets/Tests/EditMode/
Assets/Tests/PlayMode/
```

#### Assembly Definitions

Create assembly definitions for test isolation:

**`Assets/Tests/EditMode/EditModeTests.asmdef`**
```json
{
  "name": "EditModeTests",
  "rootNamespace": "WorldWars.Tests.EditMode",
  "references": ["WorldWars.Core", "WorldWars.Data", "WorldWars.AI", "WorldWars.Battle", "WorldWars.Units", "WorldWars.Terrain"],
  "includePlatforms": ["Editor"],
  "defineConstraints": ["UNITY_INCLUDE_TESTS"],
  "precompiledReferences": ["nunit.framework.dll"],
  "overrideReferences": true
}
```

**`Assets/Tests/PlayMode/PlayModeTests.asmdef`**
```json
{
  "name": "PlayModeTests",
  "rootNamespace": "WorldWars.Tests.PlayMode",
  "references": ["WorldWars.Core", "WorldWars.Data", "WorldWars.Battle", "WorldWars.Units", "WorldWars.Terrain", "WorldWars.AI", "UnityEngine.TestRunner", "UnityEditor.TestRunner"],
  "includePlatforms": [],
  "defineConstraints": ["UNITY_INCLUDE_TESTS"],
  "precompiledReferences": ["nunit.framework.dll"],
  "overrideReferences": true
}
```

> **Note on assembly definitions:** If assembly definitions cause circular dependency issues with the existing flat script structure, skip them initially and add all test files to the default assembly. The existing scripts don't use assembly definitions and adding them would require updating ALL existing scripts. The safer approach is to keep ALL game scripts in the default Assembly-CSharp and only use asmdef for test folders.

### Tests

- [ ] `Test_ProjectCompilesAfterRestructure` — Zero compile errors after creating directories and assembly definitions
- [ ] `Test_EditModeAssemblyExists` — EditModeTests.asmdef exists at Assets/Tests/EditMode/ and references correct assemblies
- [ ] `Test_PlayModeAssemblyExists` — PlayModeTests.asmdef exists at Assets/Tests/PlayMode/ and references correct assemblies

### Checklist

- [x] Create all 18 directories listed above (Core, Data/Models, Data/Databases, Data/Factions, WorldMap, Battle, Units, AI, Terrain, UI, VFX, Rendering, Campaign, Editor, Tests, Tests/EditMode, Tests/PlayMode)
- [x] Create `Assets/Tests/EditMode/EditModeTests.asmdef` with Editor platform and NUnit references
- [x] Create `Assets/Tests/PlayMode/PlayModeTests.asmdef` with NUnit and TestRunner references
- [ ] Verify project compiles with zero errors after restructure
- [ ] Run existing Viking battle scene and confirm units spawn, fight, and die correctly (no regressions)
- [ ] All Phase 0 tests written and passing
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 0: Project restructure — directories, assembly definitions, compile verified"`

---

## Phase 1: Core Foundation (~40 min)

**Goal:** Create the shared enums, event system, configuration, and game state machine that all other systems depend on.

**Prerequisites:** Phase 0.

### Overview

Define all shared enums in a single file to avoid circular dependencies. Create a lightweight pub/sub EventBus for decoupled communication. GameManager replaces GameBootstrap as the entry point, managing transitions between game states. GameConfig holds all tuning constants.

### Files

#### `Assets/Scripts/Core/Enums.cs` (NEW)

All shared enumerations used across the project:

- `enum GameFlowState { MainMenu, WorldMap, FactionSelect, BattleSetup, BattleSimulation, BattleResults, CampaignSetup, CampaignMap, CampaignBattle, CampaignTurnResolve, CampaignVictory, CampaignDefeat }` — Game state machine states (includes campaign states).
- `enum BattlePhase { Loading, Placement, Countdown, Simulating, Ended, Results }` — Sub-states within a battle.
- `enum TerrainType { Plains, Forest, Hills, Mountains, Steppe, Desert, RiverValley, Coast, Wetlands, Jungle }` — 10 biome types matching the terrain codes from faction data.
- `enum UnitCategory { HeavyInfantry, LightInfantry, HeavyCavalry, LightCavalry, Ranged, Siege, Elephant, Naval, Special }` — Unit classification for AI and combat logic.
- `enum ArmorStyle { None, Light, Medium, Heavy, Robes, Fur }` — Visual armor configuration for procedural models.
- `enum HelmetStyle { None, Hood, Conical, Nasal, Spectacle, Turban, Straw, Feathered, Crown, Wrapped }` — Head gear visual style.
- `enum WeaponStyle { Sword, Axe, Spear, Bow, Crossbow, Club, Mace, Javelin, Sling, DualAxe, DualSword, Atlatl, Elephant, None }` — Weapon types.
- `enum ShieldStyle { Round, Kite, Tower, Buckler, None }` — Shield types.
- `enum MaterialPreset { Skin, Chainmail, Steel, Gold, Leather, Fur, Wood, Bone, Cloth, Silk, Lacquer, Bronze, Obsidian, Cotton }` — Extended material presets.
- `enum Region { Europe, MiddleEast, SouthAsia, EastAsia, SoutheastAsia, Africa, Americas, Oceania }` — Geographic region grouping.
- `enum AbilityTrigger { OnLowHP, OnNearEnemy, OnCooldown, OnKill, Passive, OnAttack }` — When abilities activate.
- `enum Faction { Attacker, Defender }` — Replaces old North/South. Two sides of any battle.

#### `Assets/Scripts/Core/EventBus.cs` (NEW)

Lightweight pub/sub system for decoupled communication between systems:

- `static void Subscribe<T>(Action<T> handler)` — Register a handler for event type T.
- `static void Unsubscribe<T>(Action<T> handler)` — Remove a handler for event type T.
- `static void Publish<T>(T eventData)` — Fire an event to all registered handlers.
- `static void Clear()` — Remove all subscriptions (call on scene transitions).

Event types (defined as structs in EventBus.cs or a separate Events.cs):
- `struct BattleStartedEvent { BattleConfiguration config; }`
- `struct BattleEndedEvent { BattleResult result; }`
- `struct UnitDiedEvent { Unit unit; }`
- `struct UnitDamagedEvent { Unit unit; float damage; }`
- `struct FactionSelectedEvent { FactionDefinition faction; }`
- `struct GameStateChangedEvent { GameFlowState oldState; GameFlowState newState; }`
- `struct PlacementConfirmedEvent { Faction side; }`
- `struct AbilityActivatedEvent { Unit unit; AbilityDefinition ability; }`
- `struct CampaignPlayerTurnStartedEvent { int turn; int year; }`
- `struct CampaignTurnResolvedEvent { int turn; List<BattleResult> results; }`
- `struct CampaignProvinceTransferredEvent { string provinceId; string oldOwner; string newOwner; }`
- `struct CampaignBattleStartedEvent { string attackerFactionId; string defenderFactionId; string provinceId; }`
- `struct CampaignBattleAutoResolvedEvent { BattleResult result; }`
- `struct CampaignWonEvent { string factionId; int finalTurn; }`
- `struct CampaignLostEvent { string factionId; int finalTurn; }`
- `struct CampaignFactionEliminatedEvent { string factionId; string eliminatedBy; }`
- `struct CampaignSavedEvent { string campaignId; }`
- `struct CampaignLoadedEvent { string campaignId; }`

#### `Assets/Scripts/Core/GameConfig.cs` (NEW)

Global configuration constants. All values as `public static` fields for easy tuning:

- `static int DefaultMapSize = 120` — Battle map dimensions.
- `static float SimulationTickRate = 0.05f` — AI decision interval during simulation.
- `static float DefaultBattleSpeed = 1.0f` — Time scale for simulation.
- `static float MaxBattleSpeed = 4.0f` — Maximum fast-forward speed.
- `static float PlacementZoneDepth = 0.35f` — Fraction of map available for unit placement per side.
- `static int MaxUnitsPerSide = 40` — Unit cap per army in a battle.
- `static float BaseDetectionRange = 12f` — Default enemy detection range.
- `static float WorldMapWidth = 200f` — World map X dimension.
- `static float WorldMapHeight = 100f` — World map Z dimension.
- `static int UnitBudgetScaleFactor = 5000` — Divide faction military estimate by this for battle unit count.
- `static float TerrainHeightScale = 4f` — Max terrain elevation.
- `static float CountdownDuration = 3f` — Pre-simulation countdown seconds.

#### `Assets/Scripts/Core/BattleRandom.cs` (NEW)

Deterministic random number generator for gameplay. See architecture.md "Seeded Random Pattern" for full implementation:

- `BattleRandom(int seed)` — Constructor wrapping `System.Random` with explicit seed.
- `float Range(float min, float max)` — Random float in range.
- `int Range(int min, int maxExclusive)` — Random int in range.
- `bool Chance(float probability)` — Returns true with given probability.
- `float Value` — Random float 0-1.

**CRITICAL:** Never use `UnityEngine.Random` for gameplay. `BattleRandom` is the ONLY source of randomness for battles, terrain generation, AI decisions, and ability procs. Cosmetic effects (particle drift, camera shake) may use `UnityEngine.Random`.

#### `Assets/Scripts/Core/GameManager.cs` (NEW)

Top-level state machine replacing GameBootstrap as the primary entry point:

- `static GameManager Instance` — Singleton accessor.
- `GameFlowState CurrentState { get; private set; }` — Current game state.
- `FactionDefinition SelectedAttacker` — First selected faction for battle.
- `FactionDefinition SelectedDefender` — Second selected faction for battle.
- `BattleConfiguration CurrentBattle` — Active battle configuration.
- `BattleResult LastBattleResult` — Result of most recent battle.
- `void Awake()` — Initialize singleton, set state to MainMenu.
- `void TransitionTo(GameFlowState newState)` — Cleanup current state, initialize new state, publish GameStateChangedEvent.
- `void StartQuickBattle(FactionDefinition attacker, FactionDefinition defender)` — Configure battle and transition to BattleSetup.
- `void StartBattleSimulation()` — Transition from BattleSetup to BattleSimulation.
- `void EndBattle(BattleResult result)` — Store result and transition to BattleResults.
- `void ReturnToWorldMap()` — Transition to WorldMap state.
- `void ReturnToMainMenu()` — Transition to MainMenu state.
- `void CleanupState(GameFlowState state)` — Destroy objects associated with a state.
- `void InitializeState(GameFlowState state)` — Create objects for a state.

### Tests

**EventBus tests:**
- [ ] `Test_EventBusSubscribeAndPublish` — Subscriber receives published event data correctly
- [ ] `Test_EventBusUnsubscribe` — Unsubscribed handler stops receiving events
- [ ] `Test_EventBusMultipleSubscribers` — Multiple handlers all receive same event
- [ ] `Test_EventBusClear` — Clear removes all subscriptions completely
- [ ] `Test_EventBusNoSubscribersNoError` — Publishing with no subscribers does not throw

**GameConfig tests:**
- [ ] `Test_GameConfigDefaultValues` — All config values > 0: MapSize > 0, TickRate > 0, MaxUnits > 0, etc.
- [ ] `Test_GameConfigMaxBattleSpeedAboveDefault` — MaxBattleSpeed >= DefaultBattleSpeed

**Enums tests:**
- [ ] `Test_EnumsHaveExpectedValues` — TerrainType has 10 values, UnitCategory has 9 values
- [ ] `Test_GameFlowStateHas12Values` — GameFlowState enum contains all 12 states (6 base + 6 campaign)

**BattleRandom tests:**
- [ ] `Test_BattleRandomDeterministic` — Same seed produces identical sequence of 100 values
- [ ] `Test_BattleRandomRange` — Range(min,max) over 1000 calls always returns within [min,max)
- [ ] `Test_BattleRandomChance` — Chance(0) always false, Chance(1) always true over 100 calls

**GameManager tests:**
- [ ] `Test_GameManagerSingletonNotNull` — Instance returns non-null after Awake
- [ ] `Test_GameManagerInitialStateIsMainMenu` — CurrentState is MainMenu after initialization
- [ ] `Test_GameManagerTransitionToPublishesEvent` — TransitionTo fires GameStateChangedEvent with correct old/new state
- [ ] `Test_GameManagerStartQuickBattle` — StartQuickBattle stores attacker/defender and transitions to BattleSetup

### Checklist

- [x] `Enums.cs` created with all 12 enumerations (GameFlowState with 12 values, BattlePhase, TerrainType with 10, UnitCategory with 9, ArmorStyle, HelmetStyle, WeaponStyle, ShieldStyle, MaterialPreset, Region, AbilityTrigger, Faction)
- [x] `EventBus.cs` created with Subscribe/Unsubscribe/Publish/Clear and all 19 event structs (10 base + 9 campaign)
- [x] `GameConfig.cs` created with all 12 static constants (DefaultMapSize, SimulationTickRate, DefaultBattleSpeed, MaxBattleSpeed, PlacementZoneDepth, MaxUnitsPerSide, BaseDetectionRange, WorldMapWidth, WorldMapHeight, UnitBudgetScaleFactor, TerrainHeightScale, CountdownDuration)
- [x] `GameManager.cs` created with singleton, state machine, TransitionTo, StartQuickBattle, EndBattle, ReturnToWorldMap, ReturnToMainMenu
- [x] `BattleRandom.cs` created wrapping System.Random with Range(float), Range(int), Chance(float), Value
- [ ] All Phase 1 tests written and passing (16 tests total)
- [ ] Project compiles with no errors
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 1: Core foundation — Enums, EventBus, GameConfig, GameManager, BattleRandom"`

---

## Phase 2: Data Models & Faction Database (~90 min)

**Goal:** Create all data model classes and populate the complete database of 43 factions with their cities, units, terrain, abilities, heroes, and traits.

**Prerequisites:** Phase 1.

### Overview

Define C# data classes for factions, cities, unit types, abilities, terrain, and visual configs. Then create static data files organized by region that instantiate all 43 factions with implementation-ready data. The `faction-data.md` companion document provides the exact values for each faction. Each regional file returns a `List<FactionDefinition>`. The databases aggregate these into queryable registries.

**CRITICAL:** Reference `Docs/Plan/faction-data.md` for all faction data values. Reference `Docs/Ideas/gpt5.2.md` for terrain distributions, military estimates, and city garrisons. Reference `Docs/Ideas/grok4.2.md` for faction traits, heroes, and strategic assets.

### Files

#### `Assets/Scripts/Data/Models/FactionDefinition.cs` (NEW)

```csharp
[System.Serializable]
public class FactionDefinition
```

- `string id` — Unique snake_case identifier (e.g., "north_sea_empire").
- `string displayName` — Human-readable name (e.g., "North Sea Empire").
- `Region region` — Geographic region enum.
- `string capitalCityId` — ID of the capital city.
- `Color primaryColor` — Main faction color for units and UI.
- `Color secondaryColor` — Accent color for units and UI.
- `int estimatedMilitary` — Total military estimate from historical data.
- `string rulerName` — Name of the 1016 AD ruler/leader.
- `string rulerBonus` — Gameplay bonus description for the ruler.
- `string factionTrait` — Unique faction trait name.
- `string factionTraitDescription` — Description of the trait effect.
- `string strategicAsset` — Key strategic asset name.
- `string strategicAssetDescription` — Description of the asset bonus.
- `Dictionary<TerrainType, float> terrainDistribution` — Terrain % breakdown (sums to ~100).
- `List<CityDefinition> cities` — All cities in this faction.
- `List<UnitTypeDefinition> unitTypes` — All unit types available to this faction (4-8).
- `int GetBattleUnitBudget()` — Returns estimatedMilitary / GameConfig.UnitBudgetScaleFactor, clamped to 10-40.
- `CityDefinition GetCapital()` — Returns the city matching capitalCityId.
- `TerrainType GetDominantTerrain()` — Returns the terrain type with the highest distribution %.

#### `Assets/Scripts/Data/Models/CityDefinition.cs` (NEW)

- `string id` — Unique snake_case identifier.
- `string displayName` — Human-readable city name.
- `int garrison` — Troop garrison estimate.
- `Vector2 normalizedPosition` — Position on world map (0-1 range, x=longitude, y=latitude).
- `TerrainType primaryTerrain` — Main terrain type at this city.
- `TerrainType secondaryTerrain` — Secondary terrain type (for mixed battlefields).
- `bool isCapital` — Whether this is the faction capital.

#### `Assets/Scripts/Data/Models/UnitTypeDefinition.cs` (NEW)

- `string id` — Unique identifier (e.g., "nse_huscarl").
- `string displayName` — Display name (e.g., "Huscarl").
- `UnitCategory category` — Unit classification.
- `float maxHP` — Hit points (40-200 range).
- `float attackDamage` — Base attack damage (5-30 range).
- `float attackRange` — Attack reach (1.5 melee, up to 15 ranged).
- `float attackCooldown` — Seconds between attacks (0.5-2.0).
- `float moveSpeed` — Movement speed (2.0-5.0).
- `float armor` — Flat damage reduction (0-15).
- `string abilityId` — Reference to AbilityDefinition (null if none).
- `UnitVisualConfig visualConfig` — Visual appearance configuration.
- `string description` — Short lore description for tooltips.

#### `Assets/Scripts/Data/Models/UnitVisualConfig.cs` (NEW)

Configuration for the procedural unit model builder:

- `float bodyScale` — Overall body scale multiplier (0.8-1.3).
- `float shoulderWidth` — Shoulder pivot offset X (0.26-0.35).
- `float hipWidth` — Hip pivot offset X (0.12-0.16).
- `ArmorStyle armorStyle` — Type of armor geometry.
- `HelmetStyle helmetStyle` — Head gear type.
- `WeaponStyle primaryWeapon` — Main hand weapon.
- `WeaponStyle secondaryWeapon` — Off-hand weapon or DualWield.
- `ShieldStyle shieldStyle` — Shield type (None if no shield).
- `bool hasCape` — Whether to generate a cape.
- `bool hasBackItem` — Whether there's a back item (quiver, etc.).
- `Color armorTint` — Tint for armor material.
- `Color clothTint` — Tint for cloth/fabric.
- `Color skinTint` — Skin tone.
- `MaterialPreset armorMaterial` — Material preset for armor.
- `MaterialPreset weaponMaterial` — Material preset for weapon.

#### `Assets/Scripts/Data/Models/AbilityDefinition.cs` (NEW)

- `string id` — Unique identifier (e.g., "berserker_rage").
- `string displayName` — Display name (e.g., "Berserker Rage").
- `AbilityTrigger trigger` — When the ability activates.
- `float triggerThreshold` — Threshold for trigger (e.g., 0.4 = 40% HP).
- `float cooldown` — Cooldown in seconds (0 for one-time triggers).
- `float duration` — Effect duration in seconds.
- `float damageModifier` — Multiplier to attack damage (1.0 = no change, 1.6 = +60%).
- `float speedModifier` — Multiplier to move speed.
- `float armorModifier` — Added/subtracted armor.
- `float incomingDamageModifier` — Multiplier to incoming damage (1.4 = +40% damage taken).
- `UnitCategory[] targetCategoryCondition` — Null = applies to all targets. Non-null = only activates vs these categories (e.g., pike_brace vs HeavyCavalry/LightCavalry).
- `TerrainType[] terrainCondition` — Null = applies everywhere. Non-null = only activates on these terrains (e.g., naval_boarding on Coast, ambush on Forest/Jungle).
- `string specialCondition` — Free-form condition string (e.g., "AllyWithin5m" for formation_discipline, "FirstAttack" for ambush). Null = no special condition.
- `string description` — Tooltip description.

#### `Assets/Scripts/Data/Models/TerrainDefinition.cs` (NEW)

- `TerrainType type` — Enum value.
- `string displayName` — Human-readable name.
- `float movementMultiplier` — Base movement speed multiplier (0.4-1.0).
- `float cavalrySpeedMultiplier` — Additional cavalry speed modifier.
- `float infantryDefenseBonus` — Defense bonus for infantry on this terrain.
- `float rangedAccuracyModifier` — Accuracy modifier for ranged units.
- `float visibilityRange` — How far units can see.
- `float elevationScale` — How high terrain features are.
- `Color groundColor` — Base ground color for rendering.
- `Color accentColor` — Secondary ground color.
- `float treeDensity` — Tree density (0-1).
- `float rockDensity` — Rock/boulder density (0-1).

#### `Assets/Scripts/Data/Models/BattleConfiguration.cs` (NEW)

- `FactionDefinition attackerFaction` — Attacking faction data.
- `FactionDefinition defenderFaction` — Defending faction data.
- `CityDefinition battleLocation` — City near which battle occurs (determines terrain).
- `TerrainType primaryTerrain` — Main battlefield terrain.
- `TerrainType secondaryTerrain` — Secondary terrain (mixed biomes).
- `int attackerUnitBudget` — Max units for attacker.
- `int defenderUnitBudget` — Max units for defender.
- `int mapSize` — Battlefield dimensions.
- `int randomSeed` — Seed for deterministic terrain generation.

#### `Assets/Scripts/Data/Models/BattleResult.cs` (NEW)

- `string winnerFactionId` — ID of the winning faction.
- `string loserFactionId` — ID of the losing faction.
- `Faction winningSide` — Attacker or Defender.
- `int winnerSurvivors` — Surviving units on winning side.
- `int winnerStartCount` — Initial unit count for winner.
- `int loserStartCount` — Initial unit count for loser.
- `float battleDurationSeconds` — Real-time duration of the battle.
- `int totalCasualties` — Total units killed both sides.
- `Dictionary<string, int> attackerUnitLosses` — Unit type ID → count lost.
- `Dictionary<string, int> defenderUnitLosses` — Unit type ID → count lost.
- `Dictionary<string, int> attackerUnitKills` — Unit type ID → kills scored.
- `Dictionary<string, int> defenderUnitKills` — Unit type ID → kills scored.

#### `Assets/Scripts/Data/Databases/AbilityDatabase.cs` (NEW)

Static registry of all abilities. ~25-30 shared abilities across all factions:

- `static List<AbilityDefinition> GetAll()` — Returns all ability definitions.
- `static AbilityDefinition Get(string id)` — Lookup ability by ID.
- `static void Initialize()` — Populates the ability list.

Core abilities to define (from existing game + new):
- `parry` — Block 60% damage for 1.5s every 8s (Swordsman-type units).
- `mark` — Mark target for +40% damage from all sources for 5s every 6s (Ranged units).
- `berserker_rage` — At 40% HP: +60% damage, +30% speed, -40% armor for 6s.
- `shield_wall` — Near enemies: +15 armor, -70% speed (Tank units).
- `dual_strike` — 30% chance of bonus attack at 50% damage (Dual-wielders).
- `shield_bash` — Push enemies back on hit (Shield tanks).
- `fear_aura` — Slow enemies below 30% HP (Terror units).
- `elephant_charge` — Massive knockback and area damage on first contact.
- `horse_archer_kite` — Auto-retreat while attacking when enemies close in.
- `pike_brace` — +100% damage vs cavalry charges.
- `volley_fire` — Crossbow/archer mass volley ability (area damage).
- `fire_lance` — Short-range fire damage (Song dynasty gunpowder).
- `naval_boarding` — Bonus damage when fighting on coast terrain.
- `war_cry` — Temporary speed + damage boost for nearby allies.
- `feigned_retreat` — Temporarily flee then turn and attack with bonus damage.
- `fortify` — Stand ground: +50% defense but cannot move.
- `skirmish` — Hit-and-run: attack then auto-disengage.
- `inspire` — Nearby allies gain +20% damage for a duration (commander units).
- `javelin_throw` — Ranged attack before melee engagement.
- `sling_barrage` — Long-range area suppression.
- `atlatl_volley` — Medium-range area attack.
- `poison_arrow` — Damage-over-time on hit.
- `zealot_charge` — Ignores armor on first attack.
- `formation_discipline` — Reduced damage when near allied units.
- `ambush` — First attack from forest terrain does double damage.
- `monsoon_tactics` — Bonus in wetlands/jungle terrain.

#### `Assets/Scripts/Data/Databases/TerrainDatabase.cs` (NEW)

Static registry of terrain type definitions:

- `static List<TerrainDefinition> GetAll()` — Returns all 10 terrain definitions.
- `static TerrainDefinition Get(TerrainType type)` — Lookup by enum.
- `static void Initialize()` — Populates terrain data.

Terrain balance values:

| Terrain | Move | Cavalry SPD | Infantry DEF | Ranged ACC | Visibility | Trees | Elevation |
|---------|------|-------------|-------------|------------|------------|-------|-----------|
| Plains | 1.0 | 1.2 | 0 | 1.0 | 1.0 | 0.05 | 0.2 |
| Forest | 0.7 | 0.5 | +3 | 0.7 | 0.5 | 0.8 | 0.4 |
| Hills | 0.8 | 0.8 | +2 | 1.1 | 1.2 | 0.2 | 0.7 |
| Mountains | 0.5 | 0.3 | +5 | 1.2 | 1.5 | 0.1 | 1.0 |
| Steppe | 1.0 | 1.4 | 0 | 1.0 | 1.0 | 0.02 | 0.15 |
| Desert | 0.8 | 1.0 | -1 | 1.0 | 1.3 | 0.0 | 0.3 |
| RiverValley | 0.6 | 0.6 | +1 | 0.9 | 0.8 | 0.3 | 0.2 |
| Coast | 0.9 | 0.9 | 0 | 1.0 | 1.0 | 0.1 | 0.1 |
| Wetlands | 0.5 | 0.3 | +1 | 0.8 | 0.6 | 0.15 | 0.1 |
| Jungle | 0.6 | 0.4 | +4 | 0.6 | 0.3 | 0.9 | 0.5 |

#### `Assets/Scripts/Data/Databases/FactionDatabase.cs` (NEW)

Aggregates all regional faction files into a single queryable registry:

- `static List<FactionDefinition> GetAll()` — Returns all 43 factions.
- `static FactionDefinition Get(string id)` — Lookup by faction ID.
- `static List<FactionDefinition> GetByRegion(Region region)` — Filter by region.
- `static void Initialize()` — Calls all regional initializers and validates data.
- `static int FactionCount` — Returns 43.

#### `Assets/Scripts/Data/Databases/UnitDatabase.cs` (NEW)

Flat registry of all unit types across all factions:

- `static List<UnitTypeDefinition> GetAll()` — Returns all 216 unit types.
- `static UnitTypeDefinition Get(string id)` — Lookup by unit type ID.
- `static List<UnitTypeDefinition> GetForFaction(string factionId)` — Returns units for a specific faction.
- `static void Initialize()` — Built from FactionDatabase.

#### `Assets/Scripts/Data/Factions/EuropeFactions.cs` (NEW)

Returns `List<FactionDefinition>` for 11 European factions:
1. North Sea Empire (Cnut) — Huscarls, Berserkers, Norse Hunters, Shieldbearers, Ship Crews
2. Kingdom of Norway — Leidang levy, spearmen, axemen, archers, noble retainers
3. Kingdom of Sweden — Retainers, levy spearmen, archers, axemen, ship crews
4. Kievan Rus' — Druzhina cavalry, town militia, spearmen, archers, river flotilla
5. Kingdom of Poland — Drużyna retinue, levy spearmen, cavalry, archers, fort garrisons
6. Kingdom of Hungary — Light cavalry archers, lancers, infantry levy, border guards, noble retinues
7. Holy Roman Empire — Feudal knights, men-at-arms, spear levies, crossbowmen, siege crews
8. Kingdom of France — Knights, infantry levies, archers, castle garrisons, mercenaries
9. Byzantine Empire — Cataphracts, Varangian Guard, thematic infantry, archers, tagmata cavalry
10. Christian Iberian North — Knights, jinetes cavalry, infantry levies, archers, fortress troops
11. Córdoba / Muslim Iberia — Cavalry, archers, spearmen, guard infantry, Berber contingents

**Refer to `faction-data.md` for exact stats, colors, city positions, and unit definitions.**

#### `Assets/Scripts/Data/Factions/MiddleEastFactions.cs` (NEW)

8 factions: Fatimid Caliphate, Abbasid Caliphate, Buyid Emirates, Ghaznavid Empire, Kara-Khanid Khanate, Khwarazm, Georgia, Armenian Kingdoms.

#### `Assets/Scripts/Data/Factions/SouthAsiaFactions.cs` (NEW)

4 factions: Chola Empire, Western Chalukya Empire, Pala Empire, Rajput States.

#### `Assets/Scripts/Data/Factions/EastAsiaFactions.cs` (NEW)

5 factions: Song Empire, Liao Dynasty, Goryeo, Heian Japan, Dali Kingdom.

#### `Assets/Scripts/Data/Factions/SoutheastAsiaFactions.cs` (NEW)

5 factions: Khmer Empire, Srivijaya, Đại Cồ Việt, Champa, Pagan.

#### `Assets/Scripts/Data/Factions/AfricaFactions.cs` (NEW)

4 factions: Ghana Empire, Makuria, Ethiopian Highland Kingdoms, Kanem.

#### `Assets/Scripts/Data/Factions/AmericasFactions.cs` (NEW)

5 factions: Toltec Sphere, Maya City-States, Oaxaca States, Tiwanaku Sphere, Wari Successor Sphere.

#### `Assets/Scripts/Data/Factions/OceaniaFactions.cs` (NEW)

1 faction: Tu'i Tonga Empire.

### Tests

**Faction validation tests:**
- [ ] `Test_AllFactionsHaveValidId` — Every faction ID is non-null, non-empty, unique snake_case string
- [ ] `Test_AllFactionsHave4To8UnitTypes` — Unit count per faction within 4-8 range inclusive
- [ ] `Test_AllFactionsHaveCities` — Every faction has at least 3 cities
- [ ] `Test_AllFactionsHaveCapital` — Every faction has exactly one city with isCapital=true matching capitalCityId
- [ ] `Test_AllCitiesHavePositiveGarrison` — No city has garrison <= 0
- [ ] `Test_AllCityPositionsInRange` — All normalizedPosition.x and .y are between 0.0 and 1.0
- [ ] `Test_NoDuplicateFactionIds` — No two factions share the same ID string
- [ ] `Test_NoDuplicateUnitTypeIds` — No two unit types across all factions share the same ID
- [ ] `Test_FactionDatabaseHas43Factions` — Exactly 43 factions loaded from all regional files
- [ ] `Test_AllRegionsHaveFactions` — Every Region enum value has at least 1 faction

**Unit type validation tests:**
- [ ] `Test_AllUnitTypesHavePositiveStats` — HP > 0, ATK > 0, armor >= 0, moveSpeed > 0 for all 216 units
- [ ] `Test_AllUnitTypesHaveValidCategory` — Category is a valid UnitCategory enum for all units
- [ ] `Test_AllUnitTypesHaveVisualConfig` — Every unit has a non-null visualConfig with valid enums

**Terrain validation tests:**
- [ ] `Test_AllTerrainDistributionsSumNear100` — Each faction's terrain % sums to 95-105
- [ ] `Test_TerrainDatabaseHas10Types` — Exactly 10 terrain definitions, one per TerrainType enum
- [ ] `Test_FactionColorUniqueness` — No two factions have the same primaryColor (Euclidean distance > 0.05)

**Ability validation tests:**
- [ ] `Test_AllAbilitiesHaveValidId` — Ability IDs are non-null, non-empty, and unique
- [ ] `Test_AllUnitAbilityReferencesExist` — Every unit's abilityId (when non-null) exists in AbilityDatabase
- [ ] `Test_AbilityTargetCategoryConditionValid` — When targetCategoryCondition is non-null, all values are valid UnitCategory enums
- [ ] `Test_AbilityTerrainConditionValid` — When terrainCondition is non-null, all values are valid TerrainType enums
- [ ] `Test_PikeBraceHasCavalryTargetCondition` — pike_brace ability's targetCategoryCondition includes HeavyCavalry and LightCavalry
- [ ] `Test_NavalBoardingHasCoastTerrainCondition` — naval_boarding ability's terrainCondition includes Coast
- [ ] `Test_AmbushHasForestTerrainCondition` — ambush ability's terrainCondition includes Forest and Jungle

**Data model tests:**
- [ ] `Test_GetBattleUnitBudgetClampedTo10And40` — GetBattleUnitBudget returns value between 10-40 for any valid estimatedMilitary
- [ ] `Test_GetCapitalReturnsMatchingCity` — GetCapital returns the city whose ID matches capitalCityId
- [ ] `Test_GetDominantTerrainReturnsHighest` — GetDominantTerrain returns the TerrainType with the largest distribution %
- [ ] `Test_BattleConfigurationStoresFactionData` — BattleConfiguration correctly stores attacker and defender factions
- [ ] `Test_BattleResultStoresWinnerAndCasualties` — BattleResult correctly stores winner ID, loser ID, and casualty counts

**Spot-check tests (verify specific data is correct):**
- [ ] `Test_ByzantineEmpireDataCorrect` — Byzantine has Cataphracts unit, capital is Constantinople, region is Europe
- [ ] `Test_SongEmpireDataCorrect` — Song has crossbow corps unit, capital is Kaifeng, region is EastAsia
- [ ] `Test_NorthSeaEmpirePreservesExistingUnits` — North Sea Empire has Huscarl, Berserker, Hunter, Shieldbearer, Ship Crew with matching stats

### Checklist

- [ ] `FactionDefinition.cs` created with all fields (id, displayName, region, capitalCityId, colors, estimatedMilitary, ruler, trait, asset, terrainDistribution, cities, unitTypes) and methods (GetBattleUnitBudget, GetCapital, GetDominantTerrain)
- [ ] `CityDefinition.cs` created with all fields (id, displayName, garrison, normalizedPosition, primaryTerrain, secondaryTerrain, isCapital)
- [ ] `UnitTypeDefinition.cs` created with all fields (id, displayName, category, maxHP, attackDamage, attackRange, attackCooldown, moveSpeed, armor, abilityId, visualConfig, description)
- [ ] `UnitVisualConfig.cs` created with all fields (bodyScale, shoulderWidth, hipWidth, armorStyle, helmetStyle, primaryWeapon, secondaryWeapon, shieldStyle, hasCape, hasBackItem, armorTint, clothTint, skinTint, armorMaterial, weaponMaterial)
- [ ] `AbilityDefinition.cs` created with all fields including targetCategoryCondition, terrainCondition, specialCondition
- [ ] `TerrainDefinition.cs` created with all fields (type, displayName, movementMultiplier, cavalrySpeedMultiplier, infantryDefenseBonus, rangedAccuracyModifier, visibilityRange, elevationScale, groundColor, accentColor, treeDensity, rockDensity)
- [ ] `BattleConfiguration.cs` created with attacker/defender factions, location, terrain types, budgets, mapSize, randomSeed
- [ ] `BattleResult.cs` created with winner/loser IDs, sides, survivors, start counts, duration, casualties, unit losses/kills dictionaries
- [ ] `AbilityDatabase.cs` created with 26 abilities (all listed in Files section), each with correct conditions
- [ ] `TerrainDatabase.cs` created with 10 terrain types matching the balance table values exactly
- [ ] `EuropeFactions.cs` created with 11 factions
- [ ] `MiddleEastFactions.cs` created with 8 factions
- [ ] `SouthAsiaFactions.cs` created with 4 factions
- [ ] `EastAsiaFactions.cs` created with 5 factions
- [ ] `SoutheastAsiaFactions.cs` created with 5 factions
- [ ] `AfricaFactions.cs` created with 4 factions
- [ ] `AmericasFactions.cs` created with 5 factions
- [ ] `OceaniaFactions.cs` created with 1 faction
- [ ] `FactionDatabase.cs` aggregates all 43 factions with Get, GetAll, GetByRegion methods
- [ ] `UnitDatabase.cs` indexes all 216 unit types with Get, GetAll, GetForFaction methods
- [ ] All Phase 2 tests written and passing (31 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 2: Data models and faction database — 43 factions, 216 units, abilities, terrain definitions"`

---

## Phase 3: Terrain & Battlefield Generation (~50 min)

**Goal:** Create a biome-aware terrain generation system that produces visually distinct battlefields for each of the 10 terrain types.

**Prerequisites:** Phase 1, Phase 2 (TerrainDatabase).

### Overview

Replace the single Viking-themed MapGenerator with a modular terrain generation system. BiomeDefinitions maps each TerrainType to visual properties (colors, vegetation, features). TerrainGenerator orchestrates sub-generators for elevation, rivers, and vegetation. The existing MapGenerator's core Perlin noise approach is preserved but parameterized by biome. TerrainEffects provides runtime combat modifier queries.

### Files

#### `Assets/Scripts/Terrain/BiomeDefinitions.cs` (NEW)

Visual and structural properties for each biome:

- `static BiomeConfig GetConfig(TerrainType type)` — Returns full biome configuration.
- `struct BiomeConfig` — Contains: groundColor, groundColorAlt, cliffColor, waterColor, vegetationType (tree style), vegetationDensity, maxElevation, noiseScale, noiseOctaves, riverProbability, rockFrequency, ambientLightColor, fogDensity, skyColor.

Each biome creates a visually distinct battlefield:
- **Plains:** Flat green fields, scattered short trees, gentle hills
- **Forest:** Dense tall trees, varied greens, moderate hills, low visibility
- **Hills:** Rolling terrain, grass and exposed rock, sparse trees
- **Mountains:** Dramatic peaks, rocky terrain, snow caps, passes between peaks
- **Steppe:** Flat golden grassland, vast open space, minimal vegetation
- **Desert:** Sand-colored terrain, dunes, oasis spots, no trees
- **RiverValley:** Central river, fertile green banks, flat with river crossings
- **Coast:** Beach/sand edge, water on one side, flat with tidal pools
- **Wetlands:** Low flat terrain, pools of water, reeds, muddy ground
- **Jungle:** Dense canopy trees, vines, very hilly, maximum vegetation

#### `Assets/Scripts/Terrain/TerrainGenerator.cs` (NEW)

Master terrain generator that replaces MapGenerator for battle scenes:

- `static TerrainGenerator Instance` — Singleton.
- `void Generate(BattleConfiguration config)` — Generates complete battlefield from battle config.
- `void GenerateTerrain(TerrainType primary, TerrainType secondary, int mapSize, int seed)` — Core generation method.
- `float GetHeightAtPosition(Vector3 worldPos)` — Query terrain height at any point.
- `TerrainType GetTerrainTypeAtPosition(Vector3 worldPos)` — Query which terrain type is at a point (for mixed biome maps).
- `void BakeNavMesh()` — Bake navigation mesh after terrain is generated.
- `void CreateLighting(BiomeConfig config)` — Set up directional light, ambient, and fog per biome.
- `Terrain GeneratedTerrain` — Reference to the Unity Terrain object.

Internal flow: Create heightmap → apply biome colors → place vegetation → place features (rocks, water) → create lighting → bake NavMesh.

#### `Assets/Scripts/Terrain/ElevationGenerator.cs` (NEW)

Procedural elevation using multi-octave Perlin noise:

- `static float[,] Generate(int resolution, float scale, int octaves, float persistence, float lacunarity, int seed)` — Returns heightmap array.
- `static float[,] ApplyBiomeProfile(float[,] heights, BiomeConfig config)` — Modifies heights to match biome characteristics (flatten for plains, amplify for mountains).
- `static float[,] CarveRiverBed(float[,] heights, Vector2[] riverPath, float width, float depth)` — Carves a river channel into the heightmap.

#### `Assets/Scripts/Terrain/RiverGenerator.cs` (NEW)

Procedural river placement:

- `static Vector2[] GenerateRiverPath(int mapSize, int seed)` — Creates a winding river path across the battlefield.
- `static void CreateRiverVisuals(Terrain terrain, Vector2[] path, float width, Color waterColor)` — Paints the river onto terrain and creates water surface.
- `static bool IsRiverCrossing(Vector3 position, Vector2[] riverPath, float width)` — Query whether a position is a river crossing (for movement penalties).

#### `Assets/Scripts/Terrain/VegetationGenerator.cs` (NEW)

Biome-appropriate vegetation:

- `static void Generate(Terrain terrain, BiomeConfig config, int seed)` — Places vegetation on the terrain.
- `static void CreateTree(Transform parent, Vector3 position, BiomeConfig config)` — Creates a single procedural tree matching the biome style.
- `static void CreateRock(Transform parent, Vector3 position, BiomeConfig config)` — Creates a procedural rock/boulder.
- `static void CreateBush(Transform parent, Vector3 position, BiomeConfig config)` — Creates low vegetation.

Tree styles per biome (using the existing Minecraft-style block tree approach but varied):
- Plains/Steppe: Short rounded trees, scattered
- Forest/Jungle: Tall, dense, varied heights
- Desert: Cacti and scrub brush
- Mountains: Pine/conifer shapes
- Wetlands: Mangrove-style, short with wide roots
- Coast: Palm-style trees

#### `Assets/Scripts/Terrain/TerrainEffects.cs` (NEW)

Runtime combat modifiers based on terrain:

- `static float GetMovementMultiplier(Vector3 position, UnitCategory category)` — Returns speed multiplier for a unit at a position based on terrain and unit type.
- `static float GetDefenseBonus(Vector3 position, UnitCategory category)` — Returns defense bonus for a unit at a position.
- `static float GetRangedAccuracy(Vector3 position)` — Returns accuracy modifier at a position.
- `static float GetVisibilityRange(Vector3 position)` — Returns detection range modifier.
- `static bool IsElevated(Vector3 position, float threshold)` — Whether position is on high ground.
- `static float GetElevationAdvantage(Vector3 attacker, Vector3 defender)` — Returns damage bonus for high-ground attacker.
- `static TerrainType SampleTerrainAt(Vector3 position)` — Get the terrain type at a world position.

### Tests

**Biome visual tests:**
- [ ] `Test_PlainsTerrainIsFlat` — Plains heightmap standard deviation < 0.1 (normalized)
- [ ] `Test_MountainsTerrainHasPeaks` — Mountains max elevation exceeds 80% of TerrainHeightScale
- [ ] `Test_ForestHasHighTreeDensity` — Forest biome config treeDensity >= 0.6
- [ ] `Test_DesertHasNoTrees` — Desert biome config treeDensity == 0.0
- [ ] `Test_EachBiomeProducesDistinctTerrain` — 10 biomes produce heightmaps with different mean/stddev statistics

**River tests:**
- [ ] `Test_RiverPathCrossesMap` — River path start-to-end spans at least 70% of map width
- [ ] `Test_RiverPathIsContinuous` — All consecutive river path points are within 2 cells of each other (no gaps)

**Elevation tests:**
- [ ] `Test_ElevationGeneratorReturnsValidHeightmap` — Generated heightmap is 2D array with all values in [0.0, 1.0]

**Terrain combat modifier tests:**
- [ ] `Test_AllMovementMultipliersInRange` — All terrain movementMultiplier values between 0.4 and 1.4 (matching TerrainDatabase table)
- [ ] `Test_CavalrySlowInMountains` — GetMovementMultiplier for HeavyCavalry on Mountains returns <= 0.3
- [ ] `Test_CavalryFastOnSteppe` — GetMovementMultiplier for HeavyCavalry on Steppe returns >= 1.4
- [ ] `Test_InfantryDefenseBonusInForest` — GetDefenseBonus for HeavyInfantry on Forest returns >= 3.0
- [ ] `Test_HighGroundDamageBonus` — GetElevationAdvantage returns > 0 when attacker is 2+ units higher than defender

**Determinism tests:**
- [ ] `Test_TerrainGenerationDeterministic` — Same seed and biome produces identical heightmap array

### Checklist

- [ ] `BiomeDefinitions.cs` created with BiomeConfig for all 10 biomes (groundColor, vegetationType, vegetationDensity, maxElevation, noiseScale, noiseOctaves, etc.)
- [ ] `TerrainGenerator.cs` created with Generate(BattleConfiguration), GetHeightAtPosition, GetTerrainTypeAtPosition, BakeNavMesh
- [ ] `ElevationGenerator.cs` created with Generate (multi-octave Perlin noise), ApplyBiomeProfile, CarveRiverBed
- [ ] `RiverGenerator.cs` created with GenerateRiverPath, CreateRiverVisuals, IsRiverCrossing
- [ ] `VegetationGenerator.cs` created with Generate, CreateTree, CreateRock, CreateBush (6 tree styles per biome)
- [ ] `TerrainEffects.cs` created with GetMovementMultiplier, GetDefenseBonus, GetRangedAccuracy, GetVisibilityRange, IsElevated, GetElevationAdvantage, SampleTerrainAt
- [ ] All Phase 3 tests written and passing (14 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 3: Terrain and battlefield generation — 10 biomes, elevation, rivers, vegetation, combat modifiers"`

---

## Phase 4: Unit System Refactor (~60 min)

**Goal:** Transform the hardcoded 4-Viking-type unit system into a generic data-driven system that can build any of the 216 unit types from any faction.

**Prerequisites:** Phase 1, Phase 2.

### Overview

Extract the procedural model builder from `Unit.cs` into a standalone `UnitModelBuilder.cs`. Refactor `Unit.cs` to read its stats and config from `UnitTypeDefinition` instead of a switch statement. Create `UnitFactory.cs` as the entry point for spawning any unit. Create `AbilitySystem.cs` to process abilities generically instead of hardcoded if/else chains. The visual config system uses ArmorStyle, HelmetStyle, and WeaponStyle enums to generate culturally diverse unit models.

**CRITICAL PRESERVATION:** The existing 4 Viking unit models (Huscarl, Hunter, Berserker, Shieldbearer) MUST be preserved exactly. They become the North Sea Empire's units. The refactored model builder must produce identical results for these units. All other faction units are built using the same primitives-based system but with different visual configs.

### Files

#### `Assets/Scripts/Units/UnitModelBuilder.cs` (NEW)

Extracted from Unit.cs. Builds procedural 3D models from UnitVisualConfig:

- `static void BuildModel(Unit unit, UnitVisualConfig config, Color factionPrimary, Color factionSecondary)` — Master method: calls BuildSkeleton, then builds body, armor, weapon, helmet, accessories per config.
- `static void BuildSkeleton(Unit unit, UnitVisualConfig config)` — Creates 14-pivot skeleton hierarchy with proportions from config.
- `static void BuildBody(Unit unit, UnitVisualConfig config, Color skinTint)` — Creates torso, head, arms, legs from primitives.
- `static void BuildArmor(Unit unit, UnitVisualConfig config, Color factionPrimary)` — Adds armor geometry based on ArmorStyle enum.
- `static void BuildHelmet(Unit unit, UnitVisualConfig config, Color metalColor)` — Adds helmet based on HelmetStyle enum.
- `static void BuildWeapon(Unit unit, UnitVisualConfig config)` — Creates primary weapon extending Z+ from right hand pivot.
- `static void BuildSecondaryWeapon(Unit unit, UnitVisualConfig config)` — Creates shield or off-hand weapon on left hand pivot.
- `static void BuildAccessories(Unit unit, UnitVisualConfig config, Color factionPrimary)` — Adds cape, back items, belts, etc.
- `static void BuildLegacyViking(Unit unit, string legacyType, Color primary, Color secondary)` — Preserves exact existing Viking models by calling the original Build*Model methods.

Armor style implementations:
- **None:** Bare skin (Berserker-type)
- **Light:** Leather vest and bracers
- **Medium:** Chainmail or lamellar
- **Heavy:** Full plate/mail with surcoat
- **Robes:** Flowing cloth robes (Islamic, Asian)
- **Fur:** Fur and hide armor (steppe, tribal)

Helmet style implementations:
- **None:** Bare head or hair
- **Hood:** Cloth or leather hood
- **Conical:** Pointed conical helm (Central Asian, some European)
- **Nasal:** Viking nasal helm (existing)
- **Spectacle:** Spectacle helm (existing Shieldbearer)
- **Turban:** Wrapped cloth turban (Islamic)
- **Straw:** Straw/woven hat (East Asian peasant)
- **Feathered:** Feathered headdress (American)
- **Crown:** Noble/royal crown (commanders)
- **Wrapped:** Cloth head wrap (African, desert)

Weapon style implementations:
- Each weapon style defines: grip position, blade/head direction (Z+), materials, scale, weaponTip position.
- Melee weapons: Sword, Axe, Spear, Club, Mace, DualAxe, DualSword
- Ranged weapons: Bow, Crossbow, Javelin, Sling, Atlatl
- Special: Elephant (no hand weapon, unit IS the elephant)

#### `Assets/Scripts/Units/UnitFactory.cs` (NEW)

Central factory for creating units from data:

- `static Unit CreateUnit(UnitTypeDefinition typeDef, FactionDefinition faction, Faction side, Vector3 position, Quaternion rotation)` — Creates a complete unit with model, stats, components, and health bar.
- `static void AttachComponents(GameObject unitObj, UnitTypeDefinition typeDef)` — Adds UnitMovement, UnitCombat, UnitAnimator, HealthBar components.
- `static void WireAnimator(Unit unit, UnitAnimator animator)` — Connects skeleton pivots between Unit and UnitAnimator.
- `static void ApplyStats(Unit unit, UnitTypeDefinition typeDef)` — Sets HP, ATK, DEF, SPD, etc. from definition.

#### `Assets/Scripts/Units/Unit.cs` (MODIFY)

Refactor to be data-driven:

- **Remove:** `UnitType` enum (moved to Enums.cs as UnitCategory), `BuildBlockModel()`, `BuildSkeleton()`, `BuildSwordsmanModel()`, `BuildArcherModel()`, `BuildBerserkerModel()`, `BuildShieldbearerModel()`, `ApplyStats()` switch statement.
- **Add:** `UnitTypeDefinition typeDefinition` — Reference to the data definition.
- **Add:** `FactionDefinition factionDefinition` — Reference to the owning faction.
- **Add:** `string UnitTypeId` — Quick access to type ID.
- **Keep:** All public fields (health, damage, armor, speed, etc.) — now set by UnitFactory.
- **Keep:** All pivot Transform references — now set by UnitModelBuilder.
- **Keep:** `TakeDamage()`, `SetSelected()`, `Die()` — these are runtime behavior, not data.
- **Keep:** `isEnraged`, `isShieldWalling`, `isMarked` — replaced by generic ability state tracking.
- **Add:** `Dictionary<string, float> activeEffects` — Tracks active ability effects by ability ID.
- **Add:** `bool HasEffect(string effectId)` — Check if an ability effect is active.
- **Modify:** `Initialize()` — No longer calls BuildBlockModel/ApplyStats (handled by UnitFactory).

#### `Assets/Scripts/Units/AbilitySystem.cs` (NEW)

Generic ability processing replaces hardcoded rage/wall/mark/parry logic:

- `static void ProcessAbilities(Unit unit, List<Unit> nearbyEnemies, float deltaTime)` — Main update loop: checks triggers, activates abilities, tracks cooldowns.
- `static bool ShouldActivate(AbilityDefinition ability, Unit unit, List<Unit> nearbyEnemies)` — Evaluates trigger conditions.
- `static void ActivateAbility(Unit unit, AbilityDefinition ability)` — Applies ability effects to unit stats.
- `static void DeactivateAbility(Unit unit, AbilityDefinition ability)` — Removes ability effects.
- `static float GetModifiedDamage(Unit attacker, Unit target, float baseDamage)` — Applies all active ability modifiers to damage calculation.
- `static float GetModifiedSpeed(Unit unit)` — Returns speed with all active modifiers.
- `static float GetModifiedArmor(Unit unit)` — Returns armor with all active modifiers.

#### `Assets/Scripts/Units/UnitCombat.cs` (MODIFY)

Generalize for any unit type:

- **Remove:** `AttackAsSwordsman()`, `AttackAsArcher()`, `AttackAsBerserker()`, `AttackAsShieldbearer()` — replaced by generic `Attack()`.
- **Modify:** `Attack(Unit target)` — Uses UnitCategory to determine attack behavior (melee vs ranged, single vs dual, etc.).
- **Add:** `void AttackMelee(Unit target)` — Generic melee attack with weapon trail.
- **Add:** `void AttackRanged(Unit target)` — Generic ranged attack spawning appropriate projectile.
- **Modify:** Parry/rage/mark logic → delegate to `AbilitySystem.ProcessAbilities()`.
- **Keep:** `FindNearestEnemy()`, detection range logic, `LookAtTarget()`.
- **Modify:** Detection range uses `GameConfig.BaseDetectionRange` + ability modifiers.

#### `Assets/Scripts/Units/UnitAnimator.cs` (MODIFY)

Support new weapon and armor styles:

- **Add:** `void SetAnimationProfile(UnitCategory category, WeaponStyle weapon)` — Configures animation timings and styles per unit.
- **Modify:** `AnimateAttack()` — Uses WeaponStyle to select attack choreography instead of UnitType.
- **Add:** `void AnimateSwordSwing(float t)` — Generic sword/mace swing animation.
- **Add:** `void AnimateAxeSwing(float t)` — Generic axe swing (single or dual).
- **Add:** `void AnimateSpearThrust(float t)` — Generic spear/pike thrust.
- **Add:** `void AnimateBowShot(float t)` — Generic bow/crossbow draw and release.
- **Add:** `void AnimateClubSwing(float t)` — Club/mace overhead swing.
- **Add:** `void AnimateJavelinThrow(float t)` — Throwing animation.
- **Keep:** Idle, walk, death animations — these are mostly universal.
- **Keep:** Cape physics, rage/shield effects — driven by ability state now.

#### `Assets/Scripts/Units/HealthBar.cs` (MODIFY)

- **Modify:** `GetShortName()` — Read from `unit.typeDefinition.displayName` instead of switch on UnitType.
- **Modify:** `GetUnitTypeColor()` — Color based on UnitCategory instead of hardcoded UnitType.

#### `Assets/Scripts/Units/UnitMovement.cs` (MODIFY)

- **Add:** Terrain speed modifier integration: `agent.speed = unit.moveSpeed * TerrainEffects.GetMovementMultiplier(transform.position, unit.typeDefinition.category)`.
- **Keep:** All existing movement logic.

### Tests

**UnitFactory tests:**
- [ ] `Test_UnitFactoryCreatesUnitWithCorrectStats` — Created unit's HP, attackDamage, armor, moveSpeed all match the UnitTypeDefinition values
- [ ] `Test_UnitFactoryCreatesUnitWithModel` — Created unit has > 0 child GameObjects with MeshRenderer components
- [ ] `Test_UnitFactoryCreatesNavMeshAgent` — Created unit has a NavMeshAgent component attached

**UnitModelBuilder tests:**
- [ ] `Test_UnitModelBuilderCreatesSkeletonWith14Pivots` — BuildSkeleton creates exactly 14 named pivot transforms (spine, head, shoulders, elbows, hands, hips, knees, feet)
- [ ] `Test_LegacyVikingModelPreserved` — North Sea Empire Huscarl model has same pivot count, child mesh count, and weapon type as existing Viking Huscarl
- [ ] `Test_AllArmorStylesBuildWithoutError` — Each of 6 ArmorStyle enum values produces geometry without exceptions
- [ ] `Test_AllWeaponStylesHaveAnimation` — Each of 14 WeaponStyle enum values has a corresponding animation method in UnitAnimator

**AbilitySystem tests (generic triggers):**
- [ ] `Test_AbilityActivatesOnLowHP` — OnLowHP ability activates when unit HP drops below triggerThreshold
- [ ] `Test_AbilityAppliesDamageModifier` — Active berserker_rage multiplies attack output by damageModifier (1.6)
- [ ] `Test_AbilityDeactivatesAfterDuration` — Timed ability reverts stats after duration seconds elapsed
- [ ] `Test_AbilityCooldownPreventsReactivation` — Ability cannot refire while cooldown timer > 0

**AbilitySystem tests (conditional abilities):**
- [ ] `Test_AbilityActivatesOnlyVsTargetCategory` — pike_brace activates when target is HeavyCavalry or LightCavalry
- [ ] `Test_AbilityDoesNotActivateVsWrongCategory` — pike_brace does NOT activate when target is HeavyInfantry
- [ ] `Test_AbilityActivatesOnlyOnMatchingTerrain` — naval_boarding activates when unit is on Coast terrain
- [ ] `Test_AbilityDoesNotActivateOnWrongTerrain` — naval_boarding does NOT activate when unit is on Plains terrain
- [ ] `Test_GetModifiedDamageRespectsTargetCategory` — GetModifiedDamage applies pike_brace bonus ONLY vs cavalry targets

**Combat and movement tests:**
- [ ] `Test_GenericMeleeAttackDealsDamage` — Melee attack reduces target HP by (attackDamage - target.armor), minimum 1
- [ ] `Test_GenericRangedAttackSpawnsProjectile` — Ranged attack instantiates a projectile GameObject traveling toward target
- [ ] `Test_TerrainSpeedModifierApplied` — Unit on Forest terrain has effective speed < base moveSpeed
- [ ] `Test_UnitDiesAtZeroHP` — Unit with HP reduced to 0 triggers Die() and is no longer alive

### Checklist

- [ ] `UnitModelBuilder.cs` extracted from Unit.cs with BuildModel, BuildSkeleton (14 pivots), BuildBody, BuildArmor (6 styles), BuildHelmet (10 styles), BuildWeapon (14 styles), BuildSecondaryWeapon, BuildAccessories, BuildLegacyViking
- [ ] `UnitFactory.cs` created with CreateUnit (from UnitTypeDefinition + FactionDefinition), AttachComponents, WireAnimator, ApplyStats
- [ ] `Unit.cs` refactored: removed UnitType enum, BuildBlockModel, ApplyStats switch; added typeDefinition, factionDefinition, activeEffects dict, HasEffect method
- [ ] `AbilitySystem.cs` created with ProcessAbilities, ShouldActivate (handles all trigger types + targetCategoryCondition + terrainCondition + specialCondition), ActivateAbility, DeactivateAbility, GetModifiedDamage, GetModifiedSpeed, GetModifiedArmor
- [ ] `UnitCombat.cs` refactored: removed per-type attack methods; added generic AttackMelee, AttackRanged; delegates to AbilitySystem for modifier calculations
- [ ] `UnitAnimator.cs` updated with SetAnimationProfile, plus per-weapon animations (AnimateSwordSwing, AnimateAxeSwing, AnimateSpearThrust, AnimateBowShot, AnimateClubSwing, AnimateJavelinThrow)
- [ ] `HealthBar.cs` updated: GetShortName reads from typeDefinition.displayName, GetUnitTypeColor based on UnitCategory
- [ ] `UnitMovement.cs` updated: speed = unit.moveSpeed × TerrainEffects.GetMovementMultiplier(position, category)
- [ ] Legacy Viking models preserved: North Sea Empire Huscarl/Hunter/Berserker/Shieldbearer produce same pivot layout, mesh count, and weapon types as current game
- [ ] All Phase 4 tests written and passing (20 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 4: Unit system refactor — data-driven UnitFactory, UnitModelBuilder, AbilitySystem with conditions"`

---

## Phase 5: AI Simulation Engine (~70 min)

**Goal:** Create a symmetric AI system that controls both battle sides with identical logic, making the outcome purely dependent on army composition, unit positioning, and terrain.

**Prerequisites:** Phase 1, Phase 2, Phase 3, Phase 4.

### Overview

The SimulationAI replaces the old asymmetric AIController. Both the attacker and defender armies use the EXACT same AI class instance with the EXACT same logic. The AI makes decisions based on: unit type, nearby enemies, terrain advantages, army strength ratio, and formation integrity. Decisions run on a fixed tick interval. Key principle: **determinism** — given the same starting positions and random seed, the same outcome should occur.

### Files

#### `Assets/Scripts/AI/SimulationAI.cs` (NEW)

Master AI controller. One instance per side, both configured identically:

- `Faction side` — Which side this AI controls (Attacker or Defender).
- `List<Unit> ownUnits` — Units this AI controls.
- `List<Unit> enemyUnits` — Opposing units.
- `float decisionInterval` — How often decisions are made (GameConfig.SimulationTickRate).
- `void Initialize(Faction side, List<Unit> own, List<Unit> enemy)` — Set up the AI with its units.
- `void Update()` — Called every frame; throttled to decisionInterval.
- `void MakeDecisions()` — Main decision loop: iterates all living units and assigns orders.
- `void AssignOrder(Unit unit)` — Determines the best action for a single unit based on its category.
- `void HandleHeavyInfantry(Unit unit)` — Hold formation, engage nearest, use fortify.
- `void HandleLightInfantry(Unit unit)` — Aggressive engagement, flank, skirmish.
- `void HandleHeavyCavalry(Unit unit)` — Charge weak points, cycle charge.
- `void HandleLightCavalry(Unit unit)` — Harass flanks, kite ranged targets.
- `void HandleRanged(Unit unit)` — Maintain distance, focus marked/low-HP, retreat if threatened.
- `void HandleSiege(Unit unit)` — Advance slowly, target clusters.
- `void HandleElephant(Unit unit)` — Charge line, cause fear, push through.
- `void HandleNaval(Unit unit)` — Prioritize coast terrain, boarding actions.
- `void HandleSpecial(Unit unit)` — Commander/support unit logic (inspire, buff).
- `float EvaluateArmyStrength(List<Unit> units)` — Calculates army power from HP, damage, count.
- `bool ShouldRetreat(Unit unit)` — Whether unit should fall back based on HP and surroundings.
- `bool ShouldAdvance()` — Whether the army should push forward based on strength ratio.

#### `Assets/Scripts/AI/TacticalDecisionMaker.cs` (NEW)

High-level tactical decisions for the army as a whole:

- `enum TacticalStance { Aggressive, Balanced, Defensive, Flanking, Retreating }` — Army-wide stance.
- `static TacticalStance EvaluateStance(List<Unit> own, List<Unit> enemy)` — Determines stance from strength ratio and unit composition.
- `static Vector3 GetAdvanceTarget(List<Unit> own, List<Unit> enemy, Faction side)` — Where the army should push toward.
- `static Vector3 GetFlankPosition(Unit unit, List<Unit> enemy)` — Flanking position for a unit.
- `static Vector3 GetRetreatPosition(Unit unit, Faction side, int mapSize)` — Safe fallback position.
- `static bool ShouldFocusFire(List<Unit> own, Unit target)` — Whether multiple units should focus the same enemy.
- `static float CalculateStrengthRatio(List<Unit> own, List<Unit> enemy)` — Ratio of own strength to enemy strength (1.0 = equal).

#### `Assets/Scripts/AI/TargetSelector.cs` (NEW)

Optimal target selection per unit type:

- `static Unit SelectTarget(Unit attacker, List<Unit> enemies)` — Returns the best target for this attacker.
- `static Unit FindNearestEnemy(Unit unit, List<Unit> enemies)` — Closest living enemy.
- `static Unit FindWeakestEnemy(Unit unit, List<Unit> enemies, float maxRange)` — Lowest HP enemy in range.
- `static Unit FindHighestThreat(Unit unit, List<Unit> enemies)` — Enemy dealing most potential damage.
- `static Unit FindOptimalRangedTarget(Unit archer, List<Unit> enemies)` — Best target for ranged: marked > low HP > nearest, avoid heavy shields.
- `static Unit FindCavalryChargeTarget(Unit cavalry, List<Unit> enemies)` — Best target for cavalry charge: ranged > isolated > weak.
- `static float ScoreTarget(Unit attacker, Unit target)` — Scoring function: factors in distance, HP, threat, terrain, type matchup.
- `static float GetTypeMatchupBonus(UnitCategory attacker, UnitCategory target)` — Bonus for favorable type matchups (cavalry vs ranged, infantry vs cavalry in forest, etc.).

#### `Assets/Scripts/AI/FormationController.cs` (NEW)

Arranges units in tactically sound formations:

- `static void ArrangeFormation(List<Unit> units, Vector3 center, Vector3 facing, float spacing)` — Positions all units in formation.
- `static void ArrangeByCategory(List<Unit> units, Vector3 center, Vector3 facing)` — Auto-formation: shields front, infantry mid, cavalry flanks, ranged back.
- `static Vector3[] GetFormationPositions(int count, Vector3 center, Vector3 facing, float spacing)` — Calculate positions for N units.
- `static bool IsFormationIntact(List<Unit> units, float maxDeviation)` — Whether units are still in formation.
- `static void ReformFormation(List<Unit> units, Vector3 center)` — Order scattered units to reform.

#### `Assets/Scripts/AI/TerrainAnalyzer.cs` (NEW)

Evaluates terrain for tactical positioning:

- `static Vector3 FindBestDefensivePosition(Vector3 near, float radius, UnitCategory category)` — Finds high-ground or terrain-advantageous position.
- `static Vector3 FindBestRangedPosition(Vector3 near, List<Unit> enemies, float range)` — Position with clear sight lines and elevation.
- `static bool HasTerrainAdvantage(Vector3 position, UnitCategory category)` — Whether terrain favors this unit type.
- `static float EvaluatePositionScore(Vector3 position, UnitCategory category, List<Unit> enemies)` — Composite score for a position considering terrain, distance, elevation.
- `static List<Vector3> FindChokePoints(int mapSize)` — Identifies narrow terrain passages.
- `static Vector3 FindForestCover(Vector3 near, float radius)` — Finds nearby forest terrain for cover.

### Tests

**SimulationAI symmetry tests:**
- [ ] `Test_SimulationAIIdenticalForBothSides` — Two SimulationAI instances with same unit lists and seed make identical decisions (same target, same position)
- [ ] `Test_AIDecisionsDeterministicWithSeed` — Same seed, same unit positions → AI produces identical orders on repeated runs

**Per-category behavior tests:**
- [ ] `Test_AISelectsNearestTarget` — Unit with no special context targets the closest living enemy
- [ ] `Test_AIRangedMaintainsDistance` — Ranged unit issues retreat order when melee enemy is within 3 units
- [ ] `Test_AICavalryChargesRanged` — Cavalry targets ranged unit over equal-distance infantry
- [ ] `Test_AITankHoldsFormation` — Heavy infantry stays within 3 units of formation center
- [ ] `Test_AIRetreatsAtLowHP` — Unit below 20% HP retreats toward own spawn side
- [ ] `Test_AIElephantChargesLineFirst` — Elephant unit targets frontline clusters rather than isolated units

**TacticalDecisionMaker tests:**
- [ ] `Test_AIStrengthRatioAffectsStance` — Army with < 0.6 strength ratio adopts Defensive stance
- [ ] `Test_TacticalAdvanceTargetWithinBounds` — GetAdvanceTarget returns position within map bounds (0 to mapSize)
- [ ] `Test_GetFlankPositionNotOnEnemyFrontline` — GetFlankPosition returns a position offset from enemy center

**FormationController tests:**
- [ ] `Test_FormationPlacesShieldsFront` — ArrangeByCategory puts HeavyInfantry in front 30% of formation
- [ ] `Test_FormationPlacesRangedBack` — ArrangeByCategory puts Ranged units in back 30% of formation
- [ ] `Test_FormationPositionsAreSpaced` — GetFormationPositions returns positions at least 1.5 units apart

**TargetSelector tests:**
- [ ] `Test_TargetScoringFavorsWeak` — ScoreTarget returns higher score for 20% HP target than 100% HP target at same distance
- [ ] `Test_TypeMatchupBonusCorrect` — GetTypeMatchupBonus(HeavyCavalry, Ranged) returns a positive bonus value

**TerrainAnalyzer tests:**
- [ ] `Test_TerrainAnalyzerFindsHighGround` — FindBestDefensivePosition returns a position with higher elevation than the starting position
- [ ] `Test_TerrainAnalyzerFindsCover` — FindForestCover returns position on Forest terrain when forest exists within radius

### Checklist

- [ ] `SimulationAI.cs` created with side-agnostic decision loop, per-category handlers (HandleHeavyInfantry, HandleLightInfantry, HandleHeavyCavalry, HandleLightCavalry, HandleRanged, HandleSiege, HandleElephant, HandleNaval, HandleSpecial), EvaluateArmyStrength, ShouldRetreat, ShouldAdvance
- [ ] `TacticalDecisionMaker.cs` created with EvaluateStance, GetAdvanceTarget, GetFlankPosition, GetRetreatPosition, ShouldFocusFire, CalculateStrengthRatio
- [ ] `TargetSelector.cs` created with SelectTarget, FindNearestEnemy, FindWeakestEnemy, FindHighestThreat, FindOptimalRangedTarget, FindCavalryChargeTarget, ScoreTarget, GetTypeMatchupBonus
- [ ] `FormationController.cs` created with ArrangeFormation, ArrangeByCategory, GetFormationPositions, IsFormationIntact, ReformFormation
- [ ] `TerrainAnalyzer.cs` created with FindBestDefensivePosition, FindBestRangedPosition, HasTerrainAdvantage, EvaluatePositionScore, FindChokePoints, FindForestCover
- [ ] Both attacker and defender use same SimulationAI class, confirmed by code inspection (no Faction-specific branching in decision logic)
- [ ] All Phase 5 tests written and passing (18 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 5: AI simulation engine — symmetric SimulationAI, tactical decisions, formations, target selection"`

---

## Phase 6: World Map System (~60 min)

**Goal:** Create a visual world map showing all 43 factions with their territories, cities, and information panels for faction selection.

**Prerequisites:** Phase 1, Phase 2.

### Overview

The world map is a 2D top-down view generated procedurally from faction data. Each faction's territory is rendered as a colored polygon based on its cities' positions. Cities appear as markers. Clicking a faction shows its information panel. The map uses a simplified Mercator-like projection where city positions are normalized to a rectangle. The visual style matches the existing low-poly aesthetic — flat-shaded colored regions with clean borders.

### Files

#### `Assets/Scripts/WorldMap/WorldMapManager.cs` (NEW)

Scene controller for the world map view:

- `static WorldMapManager Instance` — Singleton.
- `void Initialize()` — Creates the world map, camera, and UI.
- `void Show()` — Enables the world map objects.
- `void Hide()` — Disables the world map objects.
- `void SelectFaction(FactionDefinition faction)` — Highlights a faction and shows info panel.
- `void DeselectFaction()` — Clears selection.
- `void ConfirmAttacker(FactionDefinition faction)` — Sets attacker and prompts for defender selection.
- `void ConfirmDefender(FactionDefinition faction)` — Sets defender and initiates battle setup.
- `FactionDefinition SelectedFaction` — Currently highlighted faction.
- `FactionDefinition ChosenAttacker` — Confirmed attacker faction.

#### `Assets/Scripts/WorldMap/WorldMapGenerator.cs` (NEW)

Generates the visual world map:

- `void Generate(List<FactionDefinition> factions)` — Creates the complete world map mesh with faction territories.
- `void CreateOcean(float width, float height)` — Creates blue ocean base plane.
- `void CreateLandmass(float width, float height)` — Creates land-colored base (continental shapes).
- `void CreateFactionTerritory(FactionDefinition faction)` — Creates colored region for a faction.
- `void CreateContinentOutlines()` — Subtle land/water boundary lines.
- `Mesh GenerateTerritoryMesh(List<Vector2> cityPositions, float padding)` — Creates a convex hull polygon from city positions with padding.
- `Vector3 NormalizedToWorld(Vector2 normalized)` — Converts 0-1 normalized position to world map coordinates.

The map is a flat 3D plane viewed from above. Territories are slightly raised colored planes. Cities are small 3D markers. The ocean is a darker blue plane beneath everything.

#### `Assets/Scripts/WorldMap/ProvinceRenderer.cs` (NEW)

Renders individual faction territories with borders:

- `void Initialize(FactionDefinition faction, Mesh territoryMesh)` — Sets up the province renderer.
- `void SetHighlighted(bool highlighted)` — Highlights the territory (brighter color, raised).
- `void SetSelected(bool selected)` — Shows selection ring/glow.
- `Material GetFactionMaterial(FactionDefinition faction)` — Creates material with faction primary color.
- `void CreateBorder(Mesh territoryMesh, Color borderColor)` — Renders territory border lines.

#### `Assets/Scripts/WorldMap/WorldMapCamera.cs` (NEW)

Camera controls for the world map:

- `static WorldMapCamera Instance` — Singleton.
- `float panSpeed` — Camera pan speed.
- `float zoomSpeed` — Camera zoom speed.
- `float minZoom, maxZoom` — Zoom limits.
- `void Initialize(float mapWidth, float mapHeight)` — Set up camera position and bounds.
- `void FocusOnFaction(FactionDefinition faction)` — Smoothly pan to center on a faction.
- `void FocusOnRegion(Region region)` — Zoom to show a region.
- `void HandleInput()` — Process pan/zoom input.
- `void ClampToBounds()` — Keep camera within map bounds.

#### `Assets/Scripts/WorldMap/WorldMapInput.cs` (NEW)

Input handling for world map interactions:

- `void Update()` — Processes mouse clicks and hovers.
- `FactionDefinition RaycastToFaction(Vector2 screenPos)` — Raycasts to find which faction territory was clicked.
- `CityDefinition RaycastToCity(Vector2 screenPos)` — Raycasts to find which city was clicked.
- `void HandleFactionClick(FactionDefinition faction)` — Processes faction selection.
- `void HandleCityClick(CityDefinition city)` — Processes city selection.
- `void HandleHover(Vector2 screenPos)` — Shows tooltips on hover.

#### `Assets/Scripts/WorldMap/FactionInfoPanel.cs` (NEW)

Displays faction details when selected:

- `void Show(FactionDefinition faction)` — Populates and shows the info panel.
- `void Hide()` — Hides the panel.
- `void PopulateFactionInfo(FactionDefinition faction)` — Sets faction name, ruler, trait, cities, units.
- `void PopulateUnitList(List<UnitTypeDefinition> units)` — Shows unit types with stats.
- `void PopulateCityList(List<CityDefinition> cities)` — Shows city names and garrisons.
- `void CreateSelectButton(string label, UnityAction onClick)` — Button to select this faction for battle.

#### `Assets/Scripts/WorldMap/CityMarker.cs` (NEW)

3D city marker on the world map:

- `void Initialize(CityDefinition city, FactionDefinition faction)` — Creates marker at city position.
- `void SetHighlighted(bool highlighted)` — Highlights on hover.
- `bool isCapital` — Capital cities get a larger marker.
- `CityDefinition City` — Reference to city data.

### Tests

**Map generation tests:**
- [ ] `Test_WorldMapGeneratorCreates43Territories` — Generate(factions) creates exactly 43 territory meshes, one per faction
- [ ] `Test_AllCitiesPlacedOnMap` — All ~155 cities have CityMarker objects placed at correct world positions
- [ ] `Test_NormalizedPositionsMapToWorldCoords` — NormalizedToWorld(0,0) returns map origin, NormalizedToWorld(1,1) returns map far corner

**Interaction tests:**
- [ ] `Test_FactionSelectionHighlightsTerritory` — Calling SelectFaction changes territory renderer material to highlighted state
- [ ] `Test_RaycastReturnsFaction` — RaycastToFaction at a known territory position returns the correct FactionDefinition
- [ ] `Test_RaycastToCity` — RaycastToCity at a known city marker position returns the correct CityDefinition
- [ ] `Test_InfoPanelShowsCorrectData` — FactionInfoPanel.Show displays the faction's displayName and rulerName

**Camera tests:**
- [ ] `Test_CameraFocusOnFactionCentersValid` — FocusOnFaction positions camera at the centroid of the faction's city positions (within map bounds)
- [ ] `Test_CameraClampsToMapBounds` — Panning camera past map edge keeps position within [0, WorldMapWidth] × [0, WorldMapHeight]
- [ ] `Test_CameraZoomWithinLimits` — Zoom value is always between minZoom and maxZoom

### Checklist

- [ ] `WorldMapManager.cs` created with Initialize, Show, Hide, SelectFaction, DeselectFaction, ConfirmAttacker, ConfirmDefender
- [ ] `WorldMapGenerator.cs` created with Generate (43 territories), CreateOcean, CreateLandmass, CreateFactionTerritory, GenerateTerritoryMesh (convex hull), NormalizedToWorld
- [ ] `ProvinceRenderer.cs` created with Initialize, SetHighlighted (brightens color + raises Y), SetSelected (glow ring), GetFactionMaterial, CreateBorder
- [ ] `WorldMapCamera.cs` created with Initialize, FocusOnFaction, FocusOnRegion, HandleInput (pan/zoom), ClampToBounds (prevents exceeding map edges)
- [ ] `WorldMapInput.cs` created with Update, RaycastToFaction, RaycastToCity, HandleFactionClick, HandleCityClick, HandleHover
- [ ] `FactionInfoPanel.cs` created with Show, Hide, PopulateFactionInfo (name, ruler, trait, asset), PopulateUnitList, PopulateCityList, CreateSelectButton
- [ ] `CityMarker.cs` created with Initialize (positions at NormalizedToWorld of city.normalizedPosition), SetHighlighted, capital markers are larger
- [ ] All Phase 6 tests written and passing (10 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 6: World map system — 43 territories, city markers, faction info, camera pan/zoom"`

---

## Phase 7: Battle Flow (~50 min)

**Goal:** Create the complete battle pipeline: setup screen with unit placement, simulation runner, time controls, auto-camera, and results screen.

**Prerequisites:** Phase 3, Phase 4, Phase 5.

### Overview

BattleManager orchestrates the entire battle lifecycle. BattleSetup lets the player place units from both armies on their respective halves of the terrain. BattleSimulator runs the AI-vs-AI simulation. BattleTimeController provides play/pause/speed. BattleCamera auto-follows the action. BattleResultsScreen shows the outcome.

### Files

#### `Assets/Scripts/Battle/BattleManager.cs` (NEW)

Master controller for the battle scene:

- `static BattleManager Instance` — Singleton.
- `BattlePhase CurrentPhase` — Current battle sub-state.
- `BattleConfiguration Config` — Active battle configuration.
- `List<Unit> attackerUnits` — All attacker units.
- `List<Unit> defenderUnits` — All defender units.
- `SimulationAI attackerAI` — AI for attacker side.
- `SimulationAI defenderAI` — AI for defender side.
- `void Initialize(BattleConfiguration config)` — Sets up the battle: generate terrain, create UI, enter Placement phase.
- `void StartPlacement()` — Enter placement phase with both armies.
- `void ConfirmPlacement()` — Lock in positions and start countdown.
- `void StartSimulation()` — Activate both AIs, enter Simulating phase.
- `void PauseSimulation()` — Pause time.
- `void ResumeSimulation()` — Resume time.
- `void SetSimulationSpeed(float speed)` — Adjust Time.timeScale.
- `void CheckBattleEnd()` — Called each frame: checks if one side is eliminated.
- `void EndBattle(Faction winner)` — Calculates result, enters Results phase.
- `void Cleanup()` — Destroys all battle objects.
- `BattleResult CalculateResult(Faction winner)` — Gathers statistics into BattleResult.

#### `Assets/Scripts/Battle/BattleSetup.cs` (NEW)

Pre-battle unit placement system:

- `void Initialize(BattleConfiguration config)` — Creates placement zones and spawns unit palettes.
- `void SpawnPlacementUnits(FactionDefinition faction, Faction side)` — Creates draggable unit representations.
- `void CreatePlacementZone(Faction side, int mapSize)` — Visual indicator of valid placement area.
- `void HandleDragAndDrop()` — Input handling for placing units.
- `bool IsValidPlacement(Vector3 position, Faction side)` — Check if position is within valid zone.
- `void ConfirmAttackerPlacement()` — Lock attacker positions.
- `void ConfirmDefenderPlacement()` — Lock defender positions.
- `void AutoPlaceUnits(FactionDefinition faction, Faction side)` — AI-assisted auto-placement using FormationController.
- `List<Unit> GetPlacedUnits(Faction side)` — Returns all placed units for a side.

Placement rules:
- Map divided into attacker half (left/north) and defender half (right/south).
- Each side can only place units in their zone (0 to 35% of map depth).
- Units can be dragged from a palette and dropped onto the terrain.
- "Auto-Place" button uses FormationController.ArrangeByCategory.
- Both sides must confirm placement before simulation starts.

#### `Assets/Scripts/Battle/BattleSimulator.cs` (NEW)

Core simulation runner:

- `bool isRunning` — Whether simulation is active.
- `float elapsedTime` — Total simulation time.
- `int frameCount` — Simulation frames elapsed.
- `void Start(List<Unit> attackers, List<Unit> defenders, SimulationAI aiA, SimulationAI aiB)` — Begins simulation.
- `void Update()` — Main simulation update: process AI, combat, abilities, check end conditions.
- `void Pause()` — Stops simulation updates.
- `void Resume()` — Resumes simulation updates.
- `bool IsBattleOver()` — Returns true if one side has no living units.
- `Faction GetWinner()` — Returns the winning side.
- `void ProcessFrame()` — Single simulation frame: update AI decisions, unit combat, movement, abilities.

#### `Assets/Scripts/Battle/BattleCamera.cs` (NEW)

Camera that auto-follows the battle action:

- `void Initialize(int mapSize)` — Set up camera position and bounds.
- `void SetAutoFollow(bool enabled)` — Toggle auto-follow mode.
- `void Update()` — In auto mode: smoothly follow the center of combat action.
- `Vector3 GetCombatCenter(List<Unit> allUnits)` — Average position of all living units.
- `void HandleManualInput()` — When auto-follow is off, WASD/scroll manual control.
- `void FocusOnUnit(Unit unit)` — Zoom to and follow a specific unit.
- `float GetOptimalZoom(List<Unit> allUnits)` — Calculates zoom level to keep all units in view.

#### `Assets/Scripts/Battle/BattleTimeController.cs` (NEW)

Play/pause/speed controls:

- `float CurrentSpeed` — Current time scale (1x, 2x, 4x).
- `bool IsPaused` — Whether simulation is paused.
- `void Play()` — Set timeScale to CurrentSpeed.
- `void Pause()` — Set timeScale to 0.
- `void SetSpeed(float multiplier)` — Set speed (clamped to GameConfig.MaxBattleSpeed).
- `void TogglePause()` — Toggle between play and pause.

#### `Assets/Scripts/Battle/BattleResultsScreen.cs` (NEW)

Post-battle statistics and outcome display:

- `void Show(BattleResult result, FactionDefinition attacker, FactionDefinition defender)` — Displays the results screen.
- `void PopulateStats(BattleResult result)` — Shows casualty numbers, time, MVP unit type.
- `void CreateRematchButton(UnityAction onClick)` — Button to replay with same config.
- `void CreateReturnButton(UnityAction onClick)` — Button to return to world map.
- `string GetBattleSummary(BattleResult result)` — Human-readable battle summary text.

### Tests

**BattleManager lifecycle tests:**
- [ ] `Test_BattleManagerInitializesFromConfig` — Initialize(config) generates terrain and sets CurrentPhase to Placement
- [ ] `Test_SimulationStartsAfterBothConfirm` — BattlePhase changes to Simulating only after both attacker and defender confirm
- [ ] `Test_SimulationEndsWhenOneSideEliminated` — Battle transitions to Ended phase when one side has 0 living units

**BattleSetup placement tests:**
- [ ] `Test_PlacementZoneConstrainsUnits` — IsValidPlacement returns false for positions outside the faction's 35% zone
- [ ] `Test_PlacementZoneAcceptsValidPosition` — IsValidPlacement returns true for positions inside the faction's 35% zone
- [ ] `Test_AutoPlaceCreatesValidFormation` — AutoPlaceUnits places all units within the valid zone, count matches unit budget

**BattleResult tests:**
- [ ] `Test_BattleResultHasCorrectWinner` — Winner factionId matches the side with surviving units
- [ ] `Test_BattleResultTracksAllCasualties` — totalCasualties == (attackerStartCount - winnerSurvivors) + loserStartCount for losing side
- [ ] `Test_RematchPreservesConfig` — After rematch, BattleConfiguration has same factions, terrain, and seed as previous battle

**Time control tests:**
- [ ] `Test_TimeControlPausesSimulation` — After Pause(), Time.timeScale == 0 and BattleSimulator.isRunning == false
- [ ] `Test_TimeControlSpeedAffectsTimeScale` — SetSpeed(2f) sets Time.timeScale to 2, SetSpeed(4f) sets it to 4
- [ ] `Test_TimeControlSpeedClampedToMax` — SetSpeed(10f) clamps to GameConfig.MaxBattleSpeed (4.0)

**Camera tests:**
- [ ] `Test_BattleCameraFollowsCombat` — Camera position moves closer to GetCombatCenter after Update when auto-follow is on

**Determinism tests:**
- [ ] `Test_DeterministicFullBattleReplay` — Two full battle runs with identical config, seed, and positions produce identical BattleResult (same winner, same casualties, same duration)

### Checklist

- [ ] `BattleManager.cs` created with Initialize, StartPlacement, ConfirmPlacement, StartSimulation, PauseSimulation, ResumeSimulation, SetSimulationSpeed, CheckBattleEnd, EndBattle, Cleanup, CalculateResult
- [ ] `BattleSetup.cs` created with Initialize, SpawnPlacementUnits, CreatePlacementZone, HandleDragAndDrop, IsValidPlacement, ConfirmAttackerPlacement, ConfirmDefenderPlacement, AutoPlaceUnits, GetPlacedUnits
- [ ] `BattleSimulator.cs` created with Start, Update, Pause, Resume, IsBattleOver, GetWinner, ProcessFrame
- [ ] `BattleCamera.cs` created with Initialize, SetAutoFollow, Update (smooth follow), GetCombatCenter, HandleManualInput, FocusOnUnit, GetOptimalZoom
- [ ] `BattleTimeController.cs` created with Play, Pause, SetSpeed (clamped to MaxBattleSpeed), TogglePause
- [ ] `BattleResultsScreen.cs` created with Show, PopulateStats, CreateRematchButton, CreateReturnButton, GetBattleSummary
- [ ] All Phase 7 tests written and passing (14 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 7: Battle flow — BattleManager, placement setup, simulator, camera, time controls, results screen"`

---

## Phase 8: UI Systems (~50 min)

**Goal:** Create all UI screens for the complete game flow: main menu, faction selection, battle setup HUD, battle HUD, results screen, world map overlay, and tooltips.

**Prerequisites:** Phase 6, Phase 7.

### Overview

All UI is procedurally generated (no prefabs) using Unity's Canvas/UI system, consistent with the existing approach. UIThemeManager provides consistent medieval styling. Each screen is a separate script that creates its own Canvas elements. The game flow transitions between screens via GameManager state changes.

### Files

#### `Assets/Scripts/UI/UIThemeManager.cs` (NEW)

Consistent visual theming across all UI:

- `static Color PanelBackground` — Dark semi-transparent panel color.
- `static Color ButtonNormal, ButtonHover, ButtonPressed` — Button state colors.
- `static Color TextPrimary, TextSecondary, TextAccent` — Text colors.
- `static Color HealthGreen, HealthYellow, HealthRed` — Health bar gradient.
- `static int TitleFontSize, HeaderFontSize, BodyFontSize, SmallFontSize` — Font size constants.
- `static GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size)` — Creates a themed panel.
- `static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size, UnityAction onClick)` — Creates a themed button.
- `static GameObject CreateText(Transform parent, string name, string content, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)` — Creates themed text.
- `static GameObject CreateScrollView(Transform parent, string name, Vector2 position, Vector2 size)` — Creates a scrollable container.
- `static Canvas CreateCanvas(string name, int sortOrder)` — Creates a UI canvas with scaler.

#### `Assets/Scripts/UI/MainMenuUI.cs` (NEW)

Redesigned main menu with options for the new game modes:

- `void Show()` — Creates and shows the main menu.
- `void Hide()` — Destroys menu objects.
- `void CreateTitle()` — "WorldWars: 1016 AD" title with medieval styling.
- `void CreateMenuButtons()` — Quick Battle, World Map, Unit Viewer, Settings buttons.
- `void OnQuickBattle()` — Transitions to FactionSelect.
- `void OnWorldMap()` — Transitions to WorldMap.
- `void OnUnitViewer()` — Opens enhanced UnitViewer.
- `void CreateBackgroundScene()` — Subtle animated background (world map silhouette, panning camera).

#### `Assets/Scripts/UI/FactionSelectUI.cs` (NEW)

Faction picker for Quick Battle mode:

- `void Show(bool selectingAttacker)` — Shows faction list, indicates what's being selected.
- `void Hide()` — Destroys picker objects.
- `void PopulateFactionList(List<FactionDefinition> factions)` — Creates scrollable list of 43 factions.
- `void CreateFactionCard(FactionDefinition faction, Transform parent)` — Card with flag color, name, region, military strength.
- `void FilterByRegion(Region region)` — Filters displayed factions by region.
- `void OnFactionSelected(FactionDefinition faction)` — Confirms selection and proceeds.
- `void CreateRegionTabs()` — Tab buttons for each region (Europe, Asia, Africa, Americas, etc.).
- `void ShowFactionPreview(FactionDefinition faction)` — Preview panel with faction details.

#### `Assets/Scripts/UI/BattleSetupUI.cs` (NEW)

UI for the unit placement phase:

- `void Show(BattleConfiguration config)` — Creates placement UI overlay.
- `void Hide()` — Destroys placement UI.
- `void CreateUnitPalette(FactionDefinition faction, Faction side)` — Draggable unit icons for placement.
- `void CreatePlacementControls()` — Auto-place button, confirm button, clear button.
- `void UpdatePlacementCount(Faction side, int placed, int max)` — Shows "12/20 units placed".
- `void ShowTerrainInfo(TerrainType terrain)` — Displays terrain bonuses for the battlefield.
- `void OnConfirmPlacement(Faction side)` — Confirms placement for one side.
- `void OnAutoPlace(Faction side)` — Auto-places remaining units.

#### `Assets/Scripts/UI/BattleHUD.cs` (NEW)

In-battle heads-up display:

- `void Initialize(BattleConfiguration config)` — Creates battle HUD elements.
- `void Update()` — Refreshes unit counts, time, speed indicator.
- `void CreateFactionCounters(FactionDefinition attacker, FactionDefinition defender)` — Shows "Blue: 15 alive / Red: 12 alive".
- `void CreateTimeControls()` — Play, pause, 1x/2x/4x speed buttons.
- `void CreateBattleTimer()` — Elapsed battle time display.
- `void CreateMinimap()` — Small minimap showing unit positions.
- `void CreateUnitTooltip()` — Tooltip on hover over a unit.
- `void ShowBattleEvent(string message)` — Flash text for notable events (ability activated, etc.).

#### `Assets/Scripts/UI/BattleResultsUI.cs` (NEW)

Post-battle results screen:

- `void Show(BattleResult result, FactionDefinition attacker, FactionDefinition defender)` — Full results display.
- `void CreateVictoryBanner(string winnerName, Color winnerColor)` — Large victory/defeat banner.
- `void CreateCasualtyReport(BattleResult result)` — Detailed unit losses by type.
- `void CreateStatistics(BattleResult result)` — Battle duration, total kills, MVP.
- `void CreateActionButtons()` — Rematch, New Battle, Return to Map buttons.

#### `Assets/Scripts/UI/WorldMapHUD.cs` (NEW)

Overlay UI for the world map view:

- `void Show()` — Creates world map HUD elements.
- `void Hide()` — Destroys HUD elements.
- `void CreateRegionButtons()` — Quick-navigate to regions.
- `void CreateSearchBar()` — Search for factions by name.
- `void CreateSelectedFactionBar()` — Shows selected faction summary at bottom.
- `void CreateBattleButton()` — "BATTLE" button (enabled when two factions selected).
- `void UpdateSelectionState(FactionDefinition attacker, FactionDefinition defender)` — Updates button states.

#### `Assets/Scripts/UI/TooltipSystem.cs` (NEW)

Hover tooltips for units, cities, terrain, and UI elements:

- `static TooltipSystem Instance` — Singleton.
- `void ShowTooltip(string title, string body, Vector2 screenPos)` — Display a tooltip at position.
- `void HideTooltip()` — Hide the tooltip.
- `void ShowUnitTooltip(UnitTypeDefinition unitType, Vector2 pos)` — Unit stats tooltip.
- `void ShowCityTooltip(CityDefinition city, FactionDefinition faction, Vector2 pos)` — City info tooltip.
- `void ShowTerrainTooltip(TerrainType terrain, Vector2 pos)` — Terrain effects tooltip.
- `void Update()` — Follow mouse position.

#### `Assets/Scripts/Rendering/MinimapRenderer.cs` (NEW)

Small minimap for battle view:

- `void Initialize(int mapSize, Color attackerColor, Color defenderColor)` — Set up minimap camera and render texture.
- `void UpdateUnitPositions(List<Unit> attackers, List<Unit> defenders)` — Draw unit dots on minimap.
- `RenderTexture MinimapTexture` — Texture for UI display.

#### `Assets/Scripts/Rendering/FactionColorPalette.cs` (NEW)

Centralized faction color management:

- `static Color GetPrimaryColor(string factionId)` — Returns faction's primary color.
- `static Color GetSecondaryColor(string factionId)` — Returns faction's secondary color.
- `static Color GetUIColor(string factionId)` — Returns UI-safe variant (bright enough for text).
- `static Color Brighten(Color c, float amount)` — Utility to brighten a color.
- `static Color Desaturate(Color c, float amount)` — Utility to desaturate.

### Tests

**UIThemeManager tests:**
- [ ] `Test_UIThemeManagerCreatePanelReturnsNonNull` — CreatePanel returns a non-null GameObject with Image component
- [ ] `Test_UIThemeManagerCreateButtonReturnsNonNull` — CreateButton returns a non-null GameObject with Button component and onClick wired
- [ ] `Test_UIThemeManagerCreateTextReturnsNonNull` — CreateText returns a non-null GameObject with Text/TMP component

**MainMenuUI tests:**
- [ ] `Test_MainMenuHasAllButtons` — Menu has exactly 4 buttons: Quick Battle, World Map, Unit Viewer, Settings

**FactionSelectUI tests:**
- [ ] `Test_FactionSelectShows43Factions` — PopulateFactionList creates exactly 43 faction cards
- [ ] `Test_RegionFilterReducesList` — FilterByRegion(Europe) shows only 11 factions, FilterByRegion(Africa) shows only 4

**BattleSetupUI tests:**
- [ ] `Test_BattleSetupShowsUnitPalette` — CreateUnitPalette generates one draggable icon per unit type in the faction

**BattleHUD tests:**
- [ ] `Test_BattleHUDUpdatesUnitCounts` — After a unit dies, displayed count decrements by 1

**WorldMapHUD tests:**
- [ ] `Test_WorldMapHUDBattleButtonDisabledWithOneSelection` — Battle button is disabled when only one faction is selected
- [ ] `Test_WorldMapHUDBattleButtonEnabledWithTwoSelections` — Battle button is enabled when both attacker and defender are selected

**Tooltip tests:**
- [ ] `Test_TooltipShowsCorrectUnitStats` — ShowUnitTooltip displays HP, ATK, DEF, SPD values matching the UnitTypeDefinition

**Results tests:**
- [ ] `Test_ResultsScreenShowsWinner` — CreateVictoryBanner displays the winning faction's displayName

**Color palette tests:**
- [ ] `Test_FactionColorPaletteReturnsDistinctColors` — GetPrimaryColor for 2 different faction IDs returns different Color values

**Minimap tests:**
- [ ] `Test_MinimapTextureNotNull` — After Initialize, MinimapTexture is a valid non-null RenderTexture

### Checklist

- [ ] `UIThemeManager.cs` created with color constants (PanelBackground, Button states, Text colors, Health colors), font sizes, and factory methods (CreatePanel, CreateButton, CreateText, CreateScrollView, CreateCanvas)
- [ ] `MainMenuUI.cs` created with Show, Hide, CreateTitle ("WorldWars: 1016 AD"), CreateMenuButtons (Quick Battle, World Map, Unit Viewer, Settings), OnQuickBattle, OnWorldMap, OnUnitViewer, CreateBackgroundScene
- [ ] `FactionSelectUI.cs` created with Show, Hide, PopulateFactionList (43 cards), CreateFactionCard (flag color, name, region, military), FilterByRegion, OnFactionSelected, CreateRegionTabs, ShowFactionPreview
- [ ] `BattleSetupUI.cs` created with Show, Hide, CreateUnitPalette, CreatePlacementControls (auto-place, confirm, clear), UpdatePlacementCount, ShowTerrainInfo, OnConfirmPlacement, OnAutoPlace
- [ ] `BattleHUD.cs` created with Initialize, Update (refreshes counts/time), CreateFactionCounters, CreateTimeControls, CreateBattleTimer, CreateMinimap, CreateUnitTooltip, ShowBattleEvent
- [ ] `BattleResultsUI.cs` created with Show, CreateVictoryBanner, CreateCasualtyReport (losses by type), CreateStatistics (duration, kills, MVP), CreateActionButtons (Rematch, New Battle, Return)
- [ ] `WorldMapHUD.cs` created with Show, Hide, CreateRegionButtons, CreateSearchBar, CreateSelectedFactionBar, CreateBattleButton (disabled until 2 factions selected), UpdateSelectionState
- [ ] `TooltipSystem.cs` created with singleton, ShowTooltip, HideTooltip, ShowUnitTooltip (HP/ATK/DEF/SPD), ShowCityTooltip (name, garrison, terrain), ShowTerrainTooltip (modifiers), Update (follow mouse)
- [ ] `MinimapRenderer.cs` created with Initialize (render texture + orthographic camera), UpdateUnitPositions (colored dots)
- [ ] `FactionColorPalette.cs` created with GetPrimaryColor, GetSecondaryColor, GetUIColor, Brighten, Desaturate
- [ ] All Phase 8 tests written and passing (14 tests total)
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 8: UI systems — main menu, faction select, battle HUD, results, world map HUD, tooltips, minimap"`

---

## Phase 9: Comprehensive Testing Suite (~70 min)

**Goal:** Write all remaining tests, run the full suite, and achieve comprehensive coverage of every system.

**Prerequisites:** All previous phases (1-8).

### Overview

This phase focuses on writing integration tests, edge case tests, balance validation tests, and PlayMode tests that require the full Unity runtime. All EditMode tests from previous phases should already exist — this phase adds the integration layer and catches anything missed.

### Files

#### `Assets/Tests/EditMode/DataValidationTests.cs` (NEW — consolidates Phase 2 data tests)

All data integrity tests in one file. Tests below marked with ★ are NEW additions beyond Phase 2:

- [ ] `Test_AllFactionsHaveValidId` — Every faction ID is non-null, non-empty, unique snake_case string
- [ ] `Test_AllFactionsHave4To8UnitTypes` — Unit count per faction within 4-8 range inclusive
- [ ] `Test_AllFactionsHaveCities` — Every faction has at least 3 cities
- [ ] `Test_AllFactionsHaveCapital` — Exactly one city with isCapital=true per faction, matching capitalCityId
- [ ] `Test_AllCitiesHavePositiveGarrison` — No city has garrison <= 0
- [ ] `Test_AllCityPositionsInRange` — All normalizedPosition.x and .y between 0.0 and 1.0
- [ ] `Test_NoDuplicateFactionIds` — No two factions share the same ID
- [ ] `Test_NoDuplicateUnitTypeIds` — No two unit types across all factions share the same ID
- [ ] `Test_AllUnitTypesHavePositiveStats` — HP > 0, ATK > 0, armor >= 0, moveSpeed > 0 for all 216 units
- [ ] `Test_AllUnitTypesHaveValidCategory` — Category is a valid UnitCategory enum value
- [ ] `Test_AllTerrainDistributionsSumNear100` — Each faction's terrain distribution sums to 95-105
- [ ] `Test_FactionColorUniqueness` — No two factions have primaryColor with Euclidean distance < 0.05
- [ ] `Test_AllAbilitiesHaveValidId` — Ability IDs are non-null, non-empty, and unique
- [ ] `Test_AllUnitAbilityReferencesExist` — Every unit's abilityId (when non-null) exists in AbilityDatabase
- [ ] `Test_TerrainDatabaseHas10Types` — Exactly 10 terrain definitions, one per TerrainType enum
- [ ] `Test_FactionDatabaseHas43Factions` — Exactly 43 factions loaded
- [ ] `Test_AllRegionsHaveFactions` — Every Region enum value has >= 1 faction
- [ ] ★ `Test_CapitalCityExistsInCityList` — For every faction, capitalCityId matches a city.id in its cities list
- [ ] ★ `Test_ByzantineEmpireDataCorrect` — Spot-check: Byzantine has Cataphracts, capital Constantinople, region Europe
- [ ] ★ `Test_SongEmpireDataCorrect` — Spot-check: Song has crossbow corps, capital Kaifeng, region EastAsia
- [ ] ★ `Test_NorthSeaEmpirePreservesExistingUnits` — North Sea Empire has Huscarl, Berserker, Hunter, Shieldbearer, Ship Crew

#### `Assets/Tests/EditMode/CombatMathTests.cs` (NEW — consolidates Phase 4 combat tests)

- [ ] `Test_BaseDamageIsAttackMinusArmor` — Damage = ATK - target.armor, minimum 1
- [ ] `Test_MarkedTargetTakes40PercentMore` — Marked target: effective damage = base × 1.4
- [ ] `Test_AbilityDamageModifierApplied` — Active berserker_rage: damage × 1.6
- [ ] `Test_ZeroArmorTakesFullDamage` — Target with 0 armor takes full attackDamage
- [ ] `Test_HighArmorCapsAtMinimum1` — Armor >= attackDamage still results in 1 damage minimum
- [ ] `Test_ElevationBonusDamage` — Attacker 2+ height above defender deals bonus damage (> base)
- [ ] `Test_TerrainDefenseBonusReducesDamage` — Defender on Forest terrain takes less damage than on Plains
- [ ] `Test_PikeBraceBonusOnlyVsCavalry` — pike_brace doubles damage vs HeavyCavalry but NOT vs HeavyInfantry

#### `Assets/Tests/EditMode/TerrainEffectTests.cs` (NEW — consolidates Phase 3 terrain tests)

- [ ] `Test_PlainsFullSpeed` — GetMovementMultiplier on Plains for HeavyInfantry returns 1.0
- [ ] `Test_ForestSlowsMovement` — GetMovementMultiplier on Forest returns < 1.0
- [ ] `Test_MountainsHeavilySlows` — GetMovementMultiplier on Mountains returns <= 0.5
- [ ] `Test_CavalryFastOnSteppe` — GetMovementMultiplier for HeavyCavalry on Steppe returns >= 1.4
- [ ] `Test_CavalrySlowInWetlands` — GetMovementMultiplier for HeavyCavalry on Wetlands returns <= 0.3
- [ ] `Test_InfantryDefenseBonusInForest` — GetDefenseBonus for HeavyInfantry on Forest returns >= 3.0
- [ ] `Test_RangedPenaltyInJungle` — GetRangedAccuracy on Jungle returns < 1.0 (specifically 0.6)
- [ ] `Test_DesertNoDefenseBonus` — GetDefenseBonus on Desert returns <= 0 (actually -1)

#### `Assets/Tests/EditMode/AIDecisionTests.cs` (NEW — consolidates Phase 5 AI tests)

- [ ] `Test_IdenticalAIProducesSameDecisions` — Two SimulationAI instances with same inputs and seed produce same outputs
- [ ] `Test_AIDoesNotCheat` — AI only references units in its own ownUnits and enemyUnits lists (no global access)
- [ ] `Test_RangedUnitsRetreatWhenFlanked` — Ranged unit issues retreat when melee enemy is within 3 units
- [ ] `Test_TanksEngageBeforeRanged` — Heavy infantry assigned attack order before ranged units start attacking
- [ ] `Test_CavalryTargetsRangedFirst` — FindCavalryChargeTarget returns ranged unit over equidistant infantry
- [ ] `Test_ArmyStrengthCalculation` — EvaluateArmyStrength for 10 full-HP units with 100 ATK returns expected value
- [ ] `Test_StanceChangesWithStrengthRatio` — Army with CalculateStrengthRatio < 0.6 adopts Defensive stance
- [ ] ★ `Test_FormationShieldsFrontRangedBack` — ArrangeByCategory places HeavyInfantry in front 30%, Ranged in back 30%
- [ ] ★ `Test_TargetScoringFavorsLowHP` — ScoreTarget returns higher score for 20% HP target vs 100% HP at same distance
- [ ] ★ `Test_TypeMatchupCavalryVsRanged` — GetTypeMatchupBonus(HeavyCavalry, Ranged) returns positive value
- [ ] ★ `Test_TerrainAnalyzerFindsHighGround` — FindBestDefensivePosition returns position higher than start position

#### `Assets/Tests/EditMode/AbilityTests.cs` (NEW — consolidates Phase 4 ability tests)

- [ ] `Test_ParryReducesDamage60Percent` — With parry active: incoming damage × 0.4
- [ ] `Test_BerserkerRageBoostsDamage` — With rage active: attack damage × 1.6
- [ ] `Test_ShieldWallIncreasesArmor` — With shield_wall active: armor + 15
- [ ] `Test_MarkIncreasesIncomingDamage` — With mark active: incoming damage × 1.4
- [ ] `Test_DualStrikeProcChance` — Over 1000 trials with BattleRandom, dual_strike procs ~30% (within 25-35%)
- [ ] `Test_ElephantChargeKnockback` — elephant_charge pushes target position away from elephant
- [ ] `Test_HorseArcherKiteRetreat` — horse_archer_kite triggers retreat when enemy within 5 units
- [ ] `Test_PikeBraceVsCavalry` — pike_brace doubles damage when target.category is HeavyCavalry or LightCavalry

#### `Assets/Tests/EditMode/FactionBalanceTests.cs` (NEW)

Cross-faction balance validation:

- [ ] `Test_NoFactionMilitaryExceeds20xSmallest` — Largest estimatedMilitary < 20× smallest estimatedMilitary (Song 900K vs Tu'i Tonga 12K = 75x, so this tests UNIT BUDGET ratio: max GetBattleUnitBudget / min GetBattleUnitBudget < 4)
- [ ] `Test_AverageUnitHPBetween60And120` — Mean HP across all 216 units is between 60 and 120
- [ ] `Test_AverageUnitATKBetween8And20` — Mean ATK across all 216 units is between 8 and 20
- [ ] `Test_EachCategoryRepresentedByAtLeast3Factions` — At least 3 factions have each UnitCategory (except Elephant, Naval, Special which need >= 1)
- [ ] `Test_RangedUnitsHaveLowerHP` — Average Ranged HP < average HeavyInfantry HP
- [ ] `Test_HeavyInfantryHasHighestArmor` — Average HeavyInfantry armor > average of all other categories
- [ ] `Test_CavalryHasHighestSpeed` — Average HeavyCavalry speed > average HeavyInfantry speed
- [ ] `Test_NoUnitHasZeroDamage` — Every unit has attackDamage > 0

#### `Assets/Tests/EditMode/BattleResultTests.cs` (NEW)

- [ ] `Test_BattleResultHasValidWinner` — winnerFactionId matches one of the two faction IDs in the battle
- [ ] `Test_CasualtiesMatchCounts` — totalCasualties == sum of all attackerUnitLosses + sum of all defenderUnitLosses
- [ ] `Test_SurvivorsLessThanStartCount` — winnerSurvivors <= winnerStartCount
- [ ] `Test_BattleDurationIsPositive` — battleDurationSeconds > 0

#### `Assets/Tests/PlayMode/BattleSimulationTests.cs` (NEW)

Full PlayMode battle tests requiring Unity runtime:

- [ ] `Test_FullBattleRunsToCompletion` — Start a battle between Byzantine and Song, it ends with a non-null BattleResult
- [ ] `Test_EqualArmiesProduceBalancedResults` — North Sea Empire vs itself: over 10 runs, each side wins at least 2 times
- [ ] `Test_LargerArmyWinsMoreOften` — Army with 2× units wins >= 8 out of 10 battles
- [ ] `Test_TerrainAdvantageMatters` — Defender on Mountains wins more than attacker on Plains (>= 6 out of 10)
- [ ] `Test_BattleEndsInReasonableTime` — Battle completes within 120 simulated seconds (Time.time)
- [ ] `Test_NoUnitsStuckOrFrozen` — At 30s into simulation, all living units have moved from their start position
- [ ] `Test_AllUnitTypesCanParticipate` — Spawn one unit of every UnitCategory, battle completes without NullReferenceException

#### `Assets/Tests/PlayMode/UnitSpawnTests.cs` (NEW)

- [ ] `Test_UnitFactorySpawnsAllVikingTypes` — All 5 North Sea Empire unit types spawn without errors
- [ ] `Test_UnitFactorySpawnsAsianUnits` — All 5 Song Dynasty unit types spawn without errors
- [ ] `Test_UnitFactorySpawnsAfricanUnits` — All 5 Ghana Empire unit types spawn without errors
- [ ] `Test_SpawnedUnitHasHealthBar` — Every spawned unit has a HealthBar component attached
- [ ] `Test_SpawnedUnitHasNavMeshAgent` — Every spawned unit has a NavMeshAgent component attached
- [ ] `Test_SpawnedUnitModelHasGeometry` — Spawned unit has > 0 child objects with MeshRenderer

#### `Assets/Tests/PlayMode/TerrainGenerationTests.cs` (NEW)

- [ ] `Test_TerrainGeneratesForAllBiomes` — Each of 10 TerrainType enum values generates terrain without exceptions
- [ ] `Test_NavMeshBakesSuccessfully` — After generation, NavMesh.CalculatePath returns a valid path between two points
- [ ] `Test_UnitsCanMoveOnGeneratedTerrain` — Spawned unit with NavMeshAgent reaches destination within 10 seconds

#### `Assets/Tests/PlayMode/WorldMapTests.cs` (NEW)

- [ ] `Test_WorldMapLoadsWithAll43Territories` — WorldMapGenerator creates 43 territory GameObjects
- [ ] `Test_WorldMapCityMarkersRendered` — All cities have visible CityMarker components in scene
- [ ] `Test_WorldMapFactionSelectionWorks` — Clicking a territory highlights it and populates FactionInfoPanel

#### `Assets/Tests/PlayMode/UINavigationTests.cs` (NEW)

- [ ] `Test_MainMenuToWorldMapTransition` — Clicking World Map button transitions GameFlowState to WorldMap
- [ ] `Test_MainMenuToQuickBattleTransition` — Clicking Quick Battle transitions to FactionSelect
- [ ] `Test_FactionSelectToSetupTransition` — Selecting two factions transitions to BattleSetup

#### `Assets/Tests/PlayMode/IntegrationTests.cs` (NEW)

End-to-end game flow:

- [ ] `Test_MainMenuToQuickBattleFlow` — Navigate Main Menu → Faction Select → select 2 factions → Battle Setup loads
- [ ] `Test_FullGameCycleCompletes` — Main Menu → Select → Battle → Results → Return → Main Menu (all transitions succeed)
- [ ] `Test_43FactionsAllPlayable` — Loop through all 43 factions, select each as attacker vs a fixed defender — no errors
- [ ] `Test_BattleBetweenAnyTwoFactions` — 5 random faction pairs each complete a full battle without crashes

### Checklist

- [ ] `DataValidationTests.cs` complete with 21 tests — all pass
- [ ] `CombatMathTests.cs` complete with 8 tests — all pass
- [ ] `TerrainEffectTests.cs` complete with 8 tests — all pass
- [ ] `AIDecisionTests.cs` complete with 11 tests — all pass
- [ ] `AbilityTests.cs` complete with 8 tests — all pass
- [ ] `FactionBalanceTests.cs` complete with 8 tests — all pass
- [ ] `BattleResultTests.cs` complete with 4 tests — all pass
- [ ] `BattleSimulationTests.cs` (PlayMode) complete with 7 tests — all pass
- [ ] `UnitSpawnTests.cs` (PlayMode) complete with 6 tests — all pass
- [ ] `TerrainGenerationTests.cs` (PlayMode) complete with 3 tests — all pass
- [ ] `WorldMapTests.cs` (PlayMode) complete with 3 tests — all pass
- [ ] `UINavigationTests.cs` (PlayMode) complete with 3 tests — all pass
- [ ] `IntegrationTests.cs` (PlayMode) complete with 4 tests — all pass
- [ ] Full test suite runs with 0 failures across all 13 test files
- [ ] Edge cases verified: empty army (0 units) does not crash, same faction vs itself works, single unit vs single unit works
- [ ] Performance check: battle with 40 units per side (80 total) runs at 30+ FPS on target hardware
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 9: Comprehensive testing suite — 94 tests across 13 files, all passing"`

---

## Phase 10: Integration, Polish & Validation (~30 min)

**Goal:** Wire everything together, update editor tools, do final validation, and ensure the game runs end-to-end.

**Prerequisites:** All previous phases (1-9).

### Overview

Connect GameManager to all systems. Update GameBootstrap to launch GameManager. Update editor tools for the new scene structure. Run the full validator. Do a final play-through of the complete game loop.

### Files

#### `Assets/Scripts/Legacy/GameBootstrap.cs` (MODIFY)

- **Modify:** `Start()` — Instead of building the Viking battle directly, check if GameManager exists. If not, create one and call `GameManager.TransitionTo(GameFlowState.MainMenu)`.
- **Keep:** Legacy Viking battle flow as a fallback mode accessible via debug menu.

#### `Assets/Scripts/Legacy/FactionManager.cs` (MODIFY)

- **Modify:** Support new Faction enum (Attacker/Defender) alongside legacy North/South.
- **Add:** Registration methods for arbitrary faction counts.

#### `Assets/Scripts/Legacy/UnitSpawner.cs` (MODIFY)

- **Modify:** Add method `SpawnFromDefinition(FactionDefinition faction, Faction side, List<Vector3> positions)` — Spawns units using UnitFactory at given positions.
- **Keep:** Legacy spawn method for backward compatibility.

#### `Assets/Scripts/Legacy/SelectionManager.cs` (MODIFY)

- **Modify:** In BattleSetup phase, selection is used for drag-and-drop placement instead of unit control.
- **Add:** `bool isPlacementMode` — Toggle between placement and battle selection.

#### `Assets/Scripts/Legacy/UnitViewer.cs` (MODIFY)

- **Modify:** Support viewing any faction's units, not just Vikings.
- **Add:** `void SetFaction(FactionDefinition faction)` — Cycle through the given faction's units.
- **Add:** Faction selection dropdown/buttons in viewer UI.

#### `Assets/Scripts/Rendering/ShaderHelper.cs` (MODIFY)

Add new material presets for cultural diversity:

- `static Material SilkMaterial(Color color)` — Silk/satin for Asian and Islamic units (metallic 0.1, smoothness 0.5).
- `static Material LacquerMaterial(Color color)` — Lacquered armor for East Asian units (metallic 0.4, smoothness 0.8).
- `static Material BronzeMaterial(Color color)` — Bronze age material (metallic 0.6, smoothness 0.5, warm tint).
- `static Material ObsidianMaterial(Color color)` — Obsidian for American units (metallic 0.7, smoothness 0.9, dark).
- `static Material CottonMaterial(Color color)` — Cotton armor for American units (metallic 0, smoothness 0.1).
- `static Material SandMaterial(Color color)` — Desert terrain coloring.
- `static Material JungleMaterial(Color color)` — Dense vegetation coloring.

#### `Assets/Scripts/VFX/CombatVFX.cs` (NEW)

Expanded combat visual effects for new weapon types:

- `static void SpawnClubImpact(Vector3 position)` — Dust cloud on club hit.
- `static void SpawnElephantCharge(Vector3 position)` — Ground shake + dust.
- `static void SpawnSlingImpact(Vector3 position)` — Small stone impact.
- `static void SpawnJavelinStick(Vector3 position, Vector3 direction)` — Javelin embeds in ground.
- `static void SpawnFireLanceBlast(Vector3 position)` — Gunpowder flash and smoke.
- `static void SpawnAtlatlImpact(Vector3 position)` — Dart impact.

#### `Assets/Scripts/Editor/WorldWarsSetup.cs` (MODIFY)

- **Modify:** `SetupScene()` — Create a GameManager object instead of GameBootstrap.
- **Add:** Menu item to set up a quick test battle between two specific factions.

#### `Assets/Scripts/Editor/WorldWarsValidator.cs` (MODIFY)

- **Add:** Validation for all 43 factions (data integrity).
- **Add:** Validation for all new script types (SimulationAI, BattleManager, etc.).
- **Add:** Validation for test assembly definitions.

#### `Assets/Scripts/Editor/FactionDataValidator.cs` (NEW)

Dedicated editor tool for faction data validation:

- `static void ValidateAllFactions()` — MenuItem that runs all data validation tests from the editor.
- `static void ValidateFaction(FactionDefinition faction)` — Validates a single faction.
- `static void PrintFactionSummary()` — Logs a summary of all 43 factions to console.
- `static void ExportBalanceReport()` — Exports unit balance statistics.

#### `Assets/Scripts/Editor/BattleDebugTools.cs` (NEW)

Debug tools for battle testing:

- `static void QuickBattle(string factionAId, string factionBId)` — MenuItem to instantly start a battle between two factions.
- `static void StressTest()` — Run 100 random battles and report win rates.
- `static void BalanceTest()` — Run every faction vs every faction and report win matrices.

### Final Verification Steps

1. **Compile check:** Build → No errors, no warnings on new code.
2. **Editor validation:** WorldWars > Validate Setup → All green.
3. **Faction validation:** Run FactionDataValidator → 43 factions valid.
4. **Test suite:** Run all EditMode + PlayMode tests → 0 failures.
5. **Manual play-through:**
   - Launch game → Main Menu appears
   - Click Quick Battle → Faction Select appears with 43 factions
   - Select Byzantine Empire (attacker) → Select Song Dynasty (defender)
   - Battle Setup → Place units on terrain → Auto-place works
   - Confirm both sides → Countdown → Simulation begins
   - Both AIs fight equally → One side wins
   - Results screen shows statistics → Click Rematch → Battle re-runs
   - Click Return → Back to faction select
6. **World Map:**
   - Main Menu → World Map → See all 43 factions
   - Click factions → Info panels appear
   - Select two factions → Battle → Full flow works
7. **Performance:** 40 units per side at 30+ FPS.
8. **Legacy:** Existing Viking battle still accessible via debug.

### Checklist

- [ ] `GameBootstrap.cs` modified: Start() checks for GameManager, creates one if missing, transitions to MainMenu
- [ ] `FactionManager.cs` modified: supports Attacker/Defender enum alongside legacy North/South
- [ ] `UnitSpawner.cs` modified: added SpawnFromDefinition(FactionDefinition, Faction, positions) using UnitFactory
- [ ] `SelectionManager.cs` modified: isPlacementMode flag toggles between drag-and-drop placement and battle selection
- [ ] `UnitViewer.cs` modified: SetFaction(FactionDefinition) cycles any faction's units, faction dropdown added
- [ ] `ShaderHelper.cs` modified: 7 new material methods added (SilkMaterial, LacquerMaterial, BronzeMaterial, ObsidianMaterial, CottonMaterial, SandMaterial, JungleMaterial)
- [ ] `CombatVFX.cs` created with 6 new VFX methods (SpawnClubImpact, SpawnElephantCharge, SpawnSlingImpact, SpawnJavelinStick, SpawnFireLanceBlast, SpawnAtlatlImpact)
- [ ] `WorldWarsSetup.cs` modified: SetupScene creates GameManager instead of GameBootstrap; added menu item for quick test battle between specific factions
- [ ] `WorldWarsValidator.cs` modified: validates all 43 factions, all new script types, and test assembly definitions
- [ ] `FactionDataValidator.cs` created with ValidateAllFactions (MenuItem), ValidateFaction, PrintFactionSummary, ExportBalanceReport
- [ ] `BattleDebugTools.cs` created with QuickBattle (MenuItem), StressTest (100 random battles), BalanceTest (every faction vs every faction)
- [ ] Full test suite passes (0 failures across all 13 test files)
- [ ] Manual play-through successful: Main Menu → Quick Battle → Faction Select (43 visible) → Battle Setup → Simulation → Results → Rematch → Return
- [ ] World map flow works end-to-end: Main Menu → World Map → click factions → info panels → select 2 → Battle
- [ ] Performance: 40 units per side (80 total) runs at 30+ FPS
- [ ] Build compiles with 0 errors and 0 warnings on new code
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 10: Integration and validation — wired GameManager, editor tools, full test suite green, manual play-through verified"`

---

## Phase 11: Campaign Conquest Mode (~90 min)

**Goal:** Implement the multi-battle campaign system where the player picks a faction and conquers the world map over multiple turns/battles. AI factions simultaneously expand. Campaigns persist across sessions.

**Prerequisites:** Phases 1-10 (everything must work for single battles first).

### Overview

The campaign system layers a turn-based strategy game on top of the existing battle engine. Each turn, the player and all AI factions choose actions (attack, defend, recruit, reinforce). Player-initiated battles use the full interactive setup/simulation pipeline. AI-vs-AI battles auto-resolve with a simplified combat model. Provinces change hands based on battle outcomes. Victory when the player controls a configurable percentage of provinces or all enemy factions are eliminated.

### Files

#### `Assets/Scripts/Campaign/CampaignManager.cs` (NEW)

Top-level campaign orchestrator. Manages turn flow, dispatches events, coordinates all campaign subsystems.

- `void StartNewCampaign(string playerFactionId)` — Initialize campaign state from faction data, set all province ownership, set turn 1, year 1016.
- `void LoadCampaign(CampaignState state)` — Restore from a saved state.
- `CampaignState GetCurrentState()` — Return serializable snapshot for saving.
- `void BeginPlayerTurn()` — Unlock player interaction on campaign map. Publish `CampaignEvent.PlayerTurnStarted`.
- `void SubmitPlayerAction(CampaignAction action)` — Validate and queue the player's chosen action for this turn.
- `void ExecuteTurn()` — Process all queued actions (player + AI). Resolve battles. Update province ownership. Collect income. Recruit. Advance year. Publish `CampaignEvent.TurnResolved`.
- `void CheckVictoryConditions()` — After turn resolution, check if player (or any AI) has met win conditions. Publish `CampaignEvent.CampaignWon` or `CampaignEvent.CampaignLost`.
- `bool IsPlayerTurn { get; }` — Whether we are waiting for player input.
- `int CurrentTurn { get; }` — Current turn number.
- `int CurrentYear { get; }` — 1016 + turn number.

#### `Assets/Scripts/Campaign/CampaignAI.cs` (NEW)

AI decision-making for non-player factions during campaign turns. All AI factions use this same class (symmetric).

- `CampaignAction DecideAction(FactionCampaignState factionState, CampaignState worldState, BattleRandom rng)` — Given world state, decide whether to attack an adjacent province, recruit, reinforce, or defend. Returns a CampaignAction.
- `float EvaluateAttack(string targetProvinceId, FactionCampaignState attacker, CampaignState world)` — Score how good an attack target is (weaker garrison, high income, strategic position).
- `float EvaluateDefense(FactionCampaignState factionState, CampaignState world)` — Score how urgent defense is (enemies adjacent to capital, low army strength).
- `Dictionary<string, int> AllocateArmy(FactionCampaignState factionState, float commitRatio)` — Decide which unit types to commit and how many.

#### `Assets/Scripts/Campaign/CampaignBattleResolver.cs` (NEW)

Resolves AI-vs-AI battles without running the full simulation. Uses a simplified model based on army composition, terrain, and faction traits.

- `BattleResult ResolveAutomatic(CampaignAction attackAction, FactionCampaignState attacker, FactionCampaignState defender, ProvinceDefinition province, BattleRandom rng)` — Compute winner based on: total army strength, terrain advantage, unit composition matchups. Returns casualties for both sides.
- `float CalculateArmyPower(Dictionary<string, int> units, TerrainType terrain)` — Sum of (unit count × unit power × terrain modifier) for all units in the army.
- `float GetTerrainModifier(string unitTypeId, TerrainType terrain)` — Lookup terrain-specific combat modifier for a unit type.

#### `Assets/Scripts/Campaign/ProvinceManager.cs` (NEW)

Manages province data, adjacency, and ownership queries.

- `void Initialize(List<ProvinceDefinition> provinces)` — Load province graph.
- `List<ProvinceDefinition> GetProvincesForFaction(string factionId)` — All provinces owned by faction.
- `List<ProvinceDefinition> GetAdjacentEnemyProvinces(string factionId)` — All enemy-owned provinces bordering this faction.
- `void TransferProvince(string provinceId, string newOwnerId)` — Change ownership. Publish `CampaignEvent.ProvinceTransferred`.
- `int CalculateIncome(string factionId)` — Sum of base income for all provinces owned by faction.
- `bool HasPathToCapital(string factionId, string provinceId)` — Check if a province is still connected to the faction capital (for supply lines).

#### `Assets/Scripts/Campaign/CampaignEconomyManager.cs` (NEW)

Handles income collection, unit recruitment costs, and army upkeep.

- `void CollectIncome(FactionCampaignState faction, ProvinceManager provinces)` — Add income from all owned provinces.
- `bool CanRecruit(FactionCampaignState faction, string unitTypeId, int count)` — Check if faction has enough gold.
- `void Recruit(FactionCampaignState faction, string unitTypeId, int count)` — Deduct gold and add units to army.
- `void PayUpkeep(FactionCampaignState faction)` — Deduct army maintenance cost each turn.
- `int GetRecruitCost(string unitTypeId)` — Base cost derived from unit stats (HP + ATK × 2 + DEF × 3).
- `int GetUpkeepCost(FactionCampaignState faction)` — Total army × upkeep rate per unit.

#### `Assets/Scripts/Campaign/ProvinceDatabase.cs` (NEW)

Static province data for the world map. Defines all provinces with adjacency.

- `static List<ProvinceDefinition> GetAllProvinces()` — Returns all provinces (one per city + bridge provinces for map connectivity).
- `static List<ProvinceDefinition> GetProvincesForRegion(string region)` — Filter by region.

**NOTE:** Province adjacency is derived from geographic proximity. For each city, its province is adjacent to all other provinces within a faction AND neighboring factions. The adjacency list is hardcoded based on the world map layout.

#### `Assets/Scripts/Data/Models/CampaignState.cs` (NEW)

Serializable data models for campaign persistence: `CampaignState`, `FactionCampaignState`, `CampaignAction`, `ProvinceDefinition` — exactly as defined in `architecture.md`.

#### `Assets/Scripts/Data/Models/PlayerProfile.cs` (NEW)

`PlayerProfile` data class as defined in `architecture.md`.

#### `Assets/Scripts/Core/IPersistenceService.cs` (NEW)

Interface for all save/load operations as defined in `architecture.md`.

#### `Assets/Scripts/Core/LocalPersistenceService.cs` (NEW)

Local implementation of `IPersistenceService` using JSON files in `Application.persistentDataPath`.

- `async Task<bool> SaveCampaignState(CampaignState state)` — Serialize to JSON, write to `{persistentDataPath}/campaigns/{id}.json`.
- `async Task<CampaignState> LoadCampaignState(string campaignId)` — Read JSON, deserialize.
- `async Task<List<CampaignState>> ListCampaigns()` — Scan campaigns directory.
- `async Task<bool> DeleteCampaign(string campaignId)` — Delete JSON file.
- All other IPersistenceService methods implemented with local JSON file storage.

### Campaign UI Files

#### `Assets/Scripts/UI/CampaignSetupUI.cs` (NEW)

- `void Show()` — Display faction selection for campaign mode (full world map with clickable factions).
- `void OnFactionSelected(string factionId)` — Highlight selected faction, show faction info, enable Start button.
- `void OnStartCampaign()` — Create new CampaignState, transition to CampaignMap.

#### `Assets/Scripts/UI/CampaignMapUI.cs` (NEW)

The main campaign screen. Shows the world map colored by current faction ownership, army indicators, and action buttons.

- `void Refresh(CampaignState state)` — Update province colors, army counts, income display, turn counter.
- `void OnProvinceClicked(string provinceId)` — If enemy province adjacent to player territory, show attack options. If own province, show garrison info.
- `void ShowAttackPanel(string targetProvinceId)` — Let player pick which units to commit.
- `void ShowRecruitPanel(string cityId)` — Let player recruit new units at a city.
- `void OnEndTurn()` — Submit player action, trigger turn resolution.
- `void ShowTurnResolution(List<BattleResult> results)` — Animate province flips, show battle summaries.

#### `Assets/Scripts/UI/CampaignVictoryUI.cs` (NEW)

- `void ShowVictory(CampaignState state)` — Display victory screen with stats.
- `void ShowDefeat(CampaignState state)` — Display defeat screen.

#### `Assets/Scripts/UI/CampaignHUD.cs` (NEW)

- `void UpdateHUD(FactionCampaignState playerState, int turn, int year)` — Refresh gold, turn, year, province count.

### Campaign GameFlowState Additions

Add to `Enums.cs`:
```
CampaignSetup, CampaignMap, CampaignBattle, CampaignTurnResolve, CampaignVictory, CampaignDefeat
```

### Tests

#### `Assets/Tests/EditMode/CampaignLogicTests.cs` (18 tests)

**CampaignManager tests:**
- [ ] `Test_CampaignManager_StartNew_InitializesAllFactions` — StartNewCampaign creates state with exactly 43 faction entries, turn=1, year=1016
- [ ] `Test_CampaignManager_ExecuteTurn_AdvancesTurnAndYear` — After ExecuteTurn, CurrentTurn increments by 1, CurrentYear = 1016 + turn

**CampaignAI tests:**
- [ ] `Test_CampaignAI_DecideAction_ChoosesWeakestTarget` — AI with 3 adjacent enemy provinces attacks the one with lowest garrison
- [ ] `Test_CampaignAI_DecideAction_DefendsWhenThreatened` — AI defends when enemy has province adjacent to its capital

**ProvinceManager tests:**
- [ ] `Test_ProvinceManager_TransferProvince_UpdatesOwnership` — After TransferProvince, GetProvincesForFaction reflects new owner
- [ ] `Test_ProvinceManager_GetAdjacentEnemy_ReturnsCorrectProvinces` — Returns only enemy-owned provinces bordering faction territory

**ProvinceDatabase tests:**
- [ ] `Test_ProvinceDatabase_GetAllProvinces_ReturnsNonEmpty` — GetAllProvinces returns list with > 0 provinces
- [ ] `Test_ProvinceDatabase_GetProvincesForRegion_FiltersCorrectly` — GetProvincesForRegion("Europe") returns only European provinces
- [ ] `Test_ProvinceDatabase_AllProvincesHaveAdjacency` — Every province has at least 1 adjacent province

**CampaignEconomy tests:**
- [ ] `Test_CampaignEconomy_CollectIncome_SumsAllProvinces` — CollectIncome adds baseIncome × provinceCount to faction gold
- [ ] `Test_CampaignEconomy_Recruit_DeductsGold` — After Recruit, faction gold decreases by GetRecruitCost × count
- [ ] `Test_CampaignEconomy_CantRecruitWithoutGold` — CanRecruit returns false when gold < cost, Recruit does not deduct

**BattleResolver tests:**
- [ ] `Test_BattleResolver_AutoResolve_StrongerArmyWins` — Army with 2× power wins auto-resolve against equal terrain
- [ ] `Test_BattleResolver_TerrainAdvantage_AffectsOutcome` — Defender on Mountains with equal army beats attacker from Plains

**Victory condition tests:**
- [ ] `Test_VictoryCondition_ControlThreshold_Triggers` — Owning >= 70% of provinces returns victory = true
- [ ] `Test_VictoryCondition_AllEnemiesDefeated_Triggers` — When all other factions have 0 provinces, victory = true

**Persistence tests:**
- [ ] `Test_CampaignState_Serialization_RoundTrips` — Serialize CampaignState to JSON and back, all fields match
- [ ] `Test_LocalPersistence_SaveLoad_RoundTrips` — SaveCampaignState then LoadCampaignState returns identical data

#### `Assets/Tests/PlayMode/CampaignFlowTests.cs` (5 tests)

- [ ] `Test_CampaignSetup_SelectFaction_StartsCampaign` — Select a faction → GameFlowState transitions to CampaignMap, world map visible
- [ ] `Test_CampaignMap_AttackProvince_TransitionsToBattle` — Click enemy province → attack → GameFlowState transitions to CampaignBattle
- [ ] `Test_CampaignMap_EndTurn_ResolvesAllActions` — EndTurn processes all AI actions, province ownership changes reflected on map
- [ ] `Test_CampaignVictory_AllProvinces_ShowsVictoryScreen` — Force faction to own 70%+ provinces → CampaignVictoryUI.ShowVictory is called
- [ ] `Test_SaveLoad_Campaign_PreservesState` — Save campaign → load → CurrentTurn, gold, province ownership all match original

### Checklist

- [ ] `CampaignState.cs` created with CampaignState, FactionCampaignState, CampaignAction classes (all fields per architecture.md)
- [ ] `ProvinceDefinition` class created with id, displayName, region, ownerFactionId, baseIncome, garrison, terrain, adjacentProvinceIds
- [ ] `PlayerProfile.cs` created with playerId, displayName, totalCampaignsWon, favoriteFactionId, campaignHistory
- [ ] `IPersistenceService.cs` interface created with SaveCampaignState, LoadCampaignState, ListCampaigns, DeleteCampaign, SavePlayerProfile, LoadPlayerProfile
- [ ] `LocalPersistenceService.cs` implemented: writes/reads JSON to Application.persistentDataPath/campaigns/
- [ ] `ProvinceDatabase.cs` created with GetAllProvinces (one per city + bridge provinces), GetProvincesForRegion, hardcoded adjacency lists
- [ ] `ProvinceManager.cs` created with Initialize, GetProvincesForFaction, GetAdjacentEnemyProvinces, TransferProvince, CalculateIncome, HasPathToCapital
- [ ] `CampaignEconomyManager.cs` created with CollectIncome, CanRecruit, Recruit, PayUpkeep, GetRecruitCost (HP + ATK×2 + DEF×3), GetUpkeepCost
- [ ] `CampaignAI.cs` created with DecideAction (attack/defend/recruit), EvaluateAttack, EvaluateDefense, AllocateArmy
- [ ] `CampaignBattleResolver.cs` created with ResolveAutomatic, CalculateArmyPower, GetTerrainModifier
- [ ] `CampaignManager.cs` created with StartNewCampaign, LoadCampaign, GetCurrentState, BeginPlayerTurn, SubmitPlayerAction, ExecuteTurn, CheckVictoryConditions
- [ ] 6 new GameFlowState values (CampaignSetup, CampaignMap, CampaignBattle, CampaignTurnResolve, CampaignVictory, CampaignDefeat) added to Enums.cs
- [ ] `CampaignSetupUI.cs` created with Show, OnFactionSelected, OnStartCampaign
- [ ] `CampaignMapUI.cs` created with Refresh, OnProvinceClicked, ShowAttackPanel, ShowRecruitPanel, OnEndTurn, ShowTurnResolution
- [ ] `CampaignHUD.cs` created with UpdateHUD (gold, turn, year, province count)
- [ ] `CampaignVictoryUI.cs` created with ShowVictory, ShowDefeat
- [ ] All 18 EditMode CampaignLogicTests pass
- [ ] All 5 PlayMode CampaignFlowTests pass
- [ ] Campaign plays 10+ turns without crashes or NullReferenceExceptions
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 11: Campaign conquest mode — CampaignManager, AI, economy, provinces, persistence, campaign UI"`

---

## Phase 12: Campaign Polish & Full Integration (~30 min)

**Goal:** Wire campaign mode into the main menu, add save/load UI, final balance pass, and full end-to-end validation.

**Prerequisites:** Phase 11.

### Overview

Integrate campaign into the main menu alongside Quick Battle. Add save slot management. Balance the campaign economy so games last 20-50 turns. Add auto-save per turn. Run full validation.

### Files

#### `Assets/Scripts/UI/MainMenuUI.cs` (MODIFY)

- **Add:** "Campaign" button alongside "Quick Battle" and "View Units".
- **Add:** "Load Campaign" button that shows list of saved campaigns.

#### `Assets/Scripts/UI/SaveLoadUI.cs` (NEW)

- `void ShowSaveSlots(List<CampaignState> campaigns)` — Display saved campaigns with faction, turn, date.
- `void OnLoadSlot(string campaignId)` — Load selected campaign.
- `void OnDeleteSlot(string campaignId)` — Delete with confirmation.
- `void OnNewCampaign()` — Transition to CampaignSetup.

#### `Assets/Scripts/Campaign/CampaignAutoSave.cs` (NEW)

- `void OnTurnResolved()` — Subscribes to `CampaignEvent.TurnResolved`, auto-saves.
- `void SaveAsync(CampaignState state)` — Background save via IPersistenceService.

#### `Assets/Scripts/Campaign/CampaignBalanceConfig.cs` (NEW)

Static balance constants:

- `const int StartingGold = 500`
- `const float ProvinceBaseIncome = 50`
- `const float CityBonusIncome = 100`
- `const float UpkeepPerUnit = 2`
- `const float VictoryThreshold = 0.7f`
- `const int MaxTurns = 100`
- `const int YearPerTurn = 1`
- `const float AIAggressionBase = 0.5f`
- `const float AIAggressionGrowth = 0.02f`

### Final Campaign Verification

1. **Main Menu** → Shows Quick Battle, Campaign, Load Campaign, View Units
2. **New Campaign** → Pick faction → World map with provinces owned
3. **Turn 1** → Recruit units, attack adjacent weak province
4. **Battle** → Full battle setup and simulation for player battles
5. **Turn Resolution** → AI factions attack each other, provinces flip
6. **Auto-save** → Save file created in persistent data path
7. **Quit and Reload** → Load campaign → Exact state restored
8. **Play 10+ turns** → AI expands, player expands, economy works
9. **Victory** → Control 70% provinces → Victory screen

### Checklist

- [ ] `MainMenuUI.cs` modified: added "Campaign" button (transitions to CampaignSetup) and "Load Campaign" button (transitions to SaveLoadUI)
- [ ] `SaveLoadUI.cs` created with ShowSaveSlots (displays faction name, turn number, date for each save), OnLoadSlot, OnDeleteSlot (with confirmation), OnNewCampaign
- [ ] `CampaignAutoSave.cs` created: subscribes to CampaignTurnResolvedEvent, calls SaveAsync via IPersistenceService after every turn
- [ ] `CampaignBalanceConfig.cs` created with all 9 constants: StartingGold=500, ProvinceBaseIncome=50, CityBonusIncome=100, UpkeepPerUnit=2, VictoryThreshold=0.7, MaxTurns=100, YearPerTurn=1, AIAggressionBase=0.5, AIAggressionGrowth=0.02
- [ ] Campaign plays 10+ turns without crashes or NullReferenceExceptions
- [ ] Save/Load round-trips: save at turn 5, load, verify CurrentTurn==5, gold matches, province ownership matches
- [ ] AI factions attack at least once every 3 turns on average (verify by running 10 turns and checking AI action log)
- [ ] Economy produces games of 20-50 turns: starting gold + income sustains recruitment; upkeep prevents infinite army growth
- [ ] Victory condition triggers correctly: owning >= 70% provinces or eliminating all other factions both show CampaignVictoryUI
- [ ] Full E2E manual test: New Campaign → pick faction → 5 turns → save → quit → reload save → 5 more turns → achieve victory
- [ ] **GIT COMMIT:** `git add -A && git commit -m "Phase 12: Campaign polish — save/load UI, auto-save, balance config, main menu integration, E2E verified"`

---

## Appendix A: Faction Quick Reference

| # | Faction | Region | Capital | Military | Units |
|---|---------|--------|---------|----------|-------|
| 1 | North Sea Empire | Europe | London | 48,000 | 5 |
| 2 | Kingdom of Norway | Europe | Trondheim | 22,000 | 5 |
| 3 | Kingdom of Sweden | Europe | Sigtuna | 24,000 | 5 |
| 4 | Kievan Rus' | Europe | Kyiv | 62,000 | 5 |
| 5 | Kingdom of Poland | Europe | Gniezno | 36,000 | 5 |
| 6 | Kingdom of Hungary | Europe | Esztergom | 42,000 | 5 |
| 7 | Holy Roman Empire | Europe | Aachen | 96,000 | 5 |
| 8 | Kingdom of France | Europe | Paris | 58,000 | 5 |
| 9 | Byzantine Empire | Europe | Constantinople | 112,000 | 5 |
| 10 | Christian Iberia | Europe | León | 39,000 | 5 |
| 11 | Córdoba / Muslim Iberia | Europe | Córdoba | 46,000 | 5 |
| 12 | Fatimid Caliphate | MiddleEast | Cairo | 72,000 | 5 |
| 13 | Abbasid Caliphate | MiddleEast | Baghdad | 28,000 | 5 |
| 14 | Buyid Emirates | MiddleEast | Shiraz | 56,000 | 5 |
| 15 | Ghaznavid Empire | MiddleEast | Ghazni | 88,000 | 5 |
| 16 | Kara-Khanid Khanate | MiddleEast | Balasaghun | 74,000 | 5 |
| 17 | Khwarazm | MiddleEast | Gurganj | 19,000 | 5 |
| 18 | Georgia | MiddleEast | Kutaisi | 29,000 | 5 |
| 19 | Armenian Kingdoms | MiddleEast | Ani | 23,000 | 5 |
| 20 | Chola Empire | SouthAsia | Thanjavur | 122,000 | 5 |
| 21 | Western Chalukya | SouthAsia | Manyakheta | 92,000 | 5 |
| 22 | Pala Empire | SouthAsia | Pataliputra | 66,000 | 5 |
| 23 | Rajput States | SouthAsia | Ajmer | 98,000 | 6 |
| 24 | Song Empire | EastAsia | Kaifeng | 900,000 | 5 |
| 25 | Liao Dynasty | EastAsia | Shangjing | 185,000 | 5 |
| 26 | Goryeo | EastAsia | Kaesong | 72,000 | 5 |
| 27 | Heian Japan | EastAsia | Kyoto | 61,000 | 5 |
| 28 | Dali Kingdom | EastAsia | Dali | 31,000 | 5 |
| 29 | Khmer Empire | SoutheastAsia | Angkor | 78,000 | 5 |
| 30 | Srivijaya | SoutheastAsia | Palembang | 47,000 | 5 |
| 31 | Đại Cồ Việt | SoutheastAsia | Hoa Lư | 46,000 | 5 |
| 32 | Champa | SoutheastAsia | Indrapura | 36,000 | 5 |
| 33 | Pagan | SoutheastAsia | Bagan | 56,000 | 5 |
| 34 | Ghana Empire | Africa | Koumbi Saleh | 31,000 | 5 |
| 35 | Makuria | Africa | Dongola | 21,000 | 5 |
| 36 | Ethiopian Highlands | Africa | Aksum | 26,000 | 5 |
| 37 | Kanem | Africa | Njimi | 23,000 | 5 |
| 38 | Toltec Sphere | Americas | Tula | 33,000 | 5 |
| 39 | Maya City-States | Americas | Chichén Itzá | 52,000 | 5 |
| 40 | Oaxaca States | Americas | Monte Albán | 26,000 | 5 |
| 41 | Tiwanaku Sphere | Americas | Tiwanaku | 19,000 | 5 |
| 42 | Wari Successor | Americas | Ayacucho | 24,000 | 5 |
| 43 | Tu'i Tonga Empire | Oceania | Mu'a | 12,000 | 5 |

**Total factions: 43 | Total unit types: 216 | Total cities: ~155**

See `faction-data.md` for COMPLETE per-faction data: every unit has full stat blocks (HP/ATK/DEF/SPD/Range/CD), ability IDs, visual configs (weapon/armor/helmet/shield/materials/cape/skin/bodyScale). Every city has garrison numbers, normalized positions, terrain. Every faction has colors, ruler, trait, asset. Every ability has trigger/cooldown/duration/modifiers/conditions. The executing AI should copy these values directly into C# data classes with ZERO interpretation needed.

---

## Appendix B: Terrain Type Reference

| Code | Terrain | Movement | Cav SPD | Inf DEF | Ranged ACC | Color |
|------|---------|----------|---------|---------|------------|-------|
| PL | Plains | 1.0 | 1.2 | 0 | 1.0 | Green |
| FO | Forest | 0.7 | 0.5 | +3 | 0.7 | Dark Green |
| HI | Hills | 0.8 | 0.8 | +2 | 1.1 | Brown-Green |
| MT | Mountains | 0.5 | 0.3 | +5 | 1.2 | Gray |
| ST | Steppe | 1.0 | 1.4 | 0 | 1.0 | Gold |
| DE | Desert | 0.8 | 1.0 | -1 | 1.0 | Sand |
| RV | River Valley | 0.6 | 0.6 | +1 | 0.9 | Blue-Green |
| CO | Coast | 0.9 | 0.9 | 0 | 1.0 | Sandy-Blue |
| SW | Wetlands | 0.5 | 0.3 | +1 | 0.8 | Murky Green |
| JN | Jungle | 0.6 | 0.4 | +4 | 0.6 | Deep Green |

---

## Appendix C: Phase Dependency Graph

```
Phase 0 (Restructure)
  └── Phase 1 (Core Foundation)
        ├── Phase 2 (Data Layer)
        │     ├── Phase 3 (Terrain)
        │     │     └── Phase 7 (Battle Flow) ─┐
        │     ├── Phase 4 (Unit System)         │
        │     │     └── Phase 5 (AI Engine) ────┤
        │     └── Phase 6 (World Map)           │
        │           └── Phase 8 (UI Systems) ───┤
        │                                       │
        └───────────────────────────────────────┤
                                                └── Phase 9 (Testing)
                                                      └── Phase 10 (Integration)
                                                            └── Phase 11 (Campaign Mode)
                                                                  └── Phase 12 (Campaign Polish)
```

Phases 3, 4, 5, 6 can run in parallel after Phase 2. Phases 7 and 8 need their predecessors. Phase 9 needs all systems. Phase 10 is the first integration pass. Phases 11-12 build campaign mode on top of the complete single-battle system.

---

## Appendix D: Critical Rules for the Executing AI

1. **Test after every phase.** Run the phase's tests before moving to the next phase. Fix failures immediately.
2. **Preserve existing Viking battle.** The North Sea Empire faction must produce identical units to the current game. Run the legacy game after Phase 4 to verify.
3. **Data source of truth.** Use `faction-data.md` for ALL faction data (stat blocks, abilities, cities, colors, visual configs). Use `architecture.md` for all data models, interfaces, and system designs. Use `Docs/Ideas/gpt5.2.md` for additional historical context if needed. Use `Docs/Ideas/grok4.2.md` for gameplay packages (heroes, traits, assets).
4. **Symmetric AI is non-negotiable.** Both battle sides MUST use the exact same SimulationAI class. No special cases for attacker vs defender. Campaign AI uses CampaignAI class — also symmetric for all factions.
5. **Deterministic battles.** Given the same random seed and starting positions, the battle outcome must be identical. Use `BattleRandom` (seeded `System.Random`) per architecture.md. NEVER use `UnityEngine.Random` for gameplay. See architecture.md "Seeded Random Pattern" section.
6. **No prefabs.** Everything is procedural. UI is built with code. Models are built from primitives. Terrain is generated. Elephants use the multi-primitive build from architecture.md "Elephant Unit Model Specification".
7. **Keep it running.** The game should compile and run after every phase. Never leave it in a broken state.
8. **File per responsibility.** One class per file. No mega-files over 800 lines. If a file exceeds that, split it.
9. **Error handling.** Every public method should handle null inputs gracefully. Log warnings, don't crash.
10. **Performance matters.** Target: 30+ FPS with 40 units per side (80 total). Use object pooling for projectiles and damage popups. Avoid per-frame allocations (no `new List<>` or string concatenation in Update loops). If battles drop below 30 FPS with 80 units, profile with Unity Profiler and optimize the hottest path.
11. **Persistence through interfaces.** ALL save/load goes through `IPersistenceService`. Implement `LocalPersistenceService` now. This will be swapped for `SupabasePersistenceService` later. See architecture.md "Supabase Persistence Layer" section.
12. **Campaign mode is required.** Phases 11-12 are not optional. The game must support multi-battle conquest campaigns.
13. **Ability conditions.** When implementing `AbilityDefinition`, include `targetCategoryCondition`, `terrainCondition`, and `specialCondition` fields. These are documented in `faction-data.md` Ability Reference table.
14. **Commit after every phase.** After completing each phase's checklist (all tests passing, project compiles), run the exact `git add -A && git commit -m "..."` command shown at the bottom of that phase's checklist. This creates a clean rollback point per phase. Never skip a commit. Never batch multiple phases into one commit.

---

## Appendix E: Test Summary by Phase

Quick reference for test counts per phase. Each phase lists tests inline — write them as you build each phase. Phase 9 consolidates all EditMode tests into final test files.

| Phase | Tests | Type | Notes |
|-------|-------|------|-------|
| 0 | 3 | EditMode | Project structure validation |
| 1 | 16 | EditMode | EventBus, GameConfig, Enums, BattleRandom, GameManager |
| 2 | 31 | EditMode | Faction/unit/city/ability/terrain data validation |
| 3 | 14 | EditMode | Biome generation, rivers, terrain combat modifiers |
| 4 | 20 | EditMode | UnitFactory, UnitModelBuilder, AbilitySystem conditions |
| 5 | 18 | EditMode | SimulationAI symmetry, per-category AI, formations, targeting |
| 6 | 10 | PlayMode | World map generation, interaction, camera |
| 7 | 14 | PlayMode | Battle lifecycle, placement, time controls, determinism |
| 8 | 14 | PlayMode | All UI screens, tooltips, minimap, color palette |
| 9 | 94 | Both | **Final consolidated suite:** 68 EditMode + 26 PlayMode |
| 10 | 0 | — | No new tests; runs full suite from Phase 9 |
| 11 | 23 | Both | **Campaign:** 18 EditMode + 5 PlayMode |
| 12 | 0 | — | No new tests; manual E2E verification |

**Final test file count: 15** (8 EditMode + 7 PlayMode)
**Final unique test count: 117** (86 EditMode + 31 PlayMode)

Each test name follows the pattern `Test_[SystemUnderTest]_[ExpectedBehavior]` with a 5-10 word summary describing the exact pass/fail condition.
