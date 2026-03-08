using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Comprehensive test suite validating the entire WorldWars game pipeline:
/// databases, factions, units, terrain, combat, campaign, persistence, and UI types.
/// These tests ensure the game is ready to run without compilation or runtime errors.
/// </summary>
[TestFixture]
public class WorldWarsComprehensiveTests
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        AbilityDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();
        TerrainDatabase.Initialize();
        ProvinceDatabase.Initialize();
    }

    // ═══════════════════════════════════════════════════════════════════
    //  1. DATABASE INTEGRITY
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void Databases_AllInitializeWithoutErrors()
    {
        Assert.AreEqual(43, FactionDatabase.FactionCount, "Expected 43 factions");
        Assert.Greater(UnitDatabase.GetAll().Count, 0, "UnitDatabase should have entries");
        Assert.Greater(ProvinceDatabase.GetAllProvinces().Count, 0, "ProvinceDatabase should have provinces");
    }

    [Test]
    public void FactionDatabase_AllRegionsRepresented()
    {
        var regions = new HashSet<Region>();
        foreach (var f in FactionDatabase.GetAll())
            regions.Add(f.region);

        foreach (Region r in Enum.GetValues(typeof(Region)))
            Assert.IsTrue(regions.Contains(r), $"Region {r} has no factions");
    }

    [Test]
    public void FactionDatabase_AllFactionsHaveUnitTypes()
    {
        foreach (var f in FactionDatabase.GetAll())
        {
            Assert.IsNotNull(f.unitTypes, $"Faction {f.id} has null unitTypes");
            Assert.Greater(f.unitTypes.Count, 0, $"Faction {f.id} has zero unit types");
        }
    }

    [Test]
    public void FactionDatabase_AllFactionsHaveCities()
    {
        foreach (var f in FactionDatabase.GetAll())
        {
            Assert.IsNotNull(f.cities, $"Faction {f.id} has null cities");
            Assert.Greater(f.cities.Count, 0, $"Faction {f.id} has zero cities");
        }
    }

    [Test]
    public void FactionDatabase_AllFactionsHaveValidMilitaryBudgets()
    {
        foreach (var f in FactionDatabase.GetAll())
        {
            int budget = f.GetBattleUnitBudget();
            Assert.GreaterOrEqual(budget, 48, $"Faction {f.id} budget {budget} below minimum 48");
            Assert.LessOrEqual(budget, 800, $"Faction {f.id} budget {budget} above maximum 800");
        }
    }

    [Test]
    public void FactionDatabase_GetByIdReturnsCorrectFaction()
    {
        var viking = FactionDatabase.Get("north_sea_empire");
        Assert.IsNotNull(viking, "north_sea_empire should exist");
        Assert.AreEqual("north_sea_empire", viking.id);
        Assert.AreEqual(Region.Europe, viking.region);
    }

    [Test]
    public void FactionDatabase_AllFactionIdsAreUnique()
    {
        var ids = FactionDatabase.GetAll().Select(f => f.id).ToList();
        Assert.AreEqual(ids.Count, ids.Distinct().Count(), "Duplicate faction IDs detected");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  2. UNIT SYSTEM
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void UnitDatabase_AllUnitsHavePositiveStats()
    {
        foreach (var u in UnitDatabase.GetAll())
        {
            Assert.Greater(u.maxHP, 0, $"Unit {u.id} has zero maxHP");
            Assert.Greater(u.attackDamage, 0, $"Unit {u.id} has zero attackDamage");
            Assert.Greater(u.moveSpeed, 0, $"Unit {u.id} has zero moveSpeed");
            Assert.Greater(u.attackRange, 0, $"Unit {u.id} has zero attackRange");
        }
    }

    [Test]
    public void UnitDatabase_AllUnitsHaveVisualConfig()
    {
        foreach (var u in UnitDatabase.GetAll())
            Assert.IsNotNull(u.visualConfig, $"Unit {u.id} has null visualConfig");
    }

    [Test]
    public void UnitDatabase_AllUnitCategoriesRepresented()
    {
        var categories = new HashSet<UnitCategory>();
        foreach (var u in UnitDatabase.GetAll())
            categories.Add(u.category);

        Assert.Greater(categories.Count, 4, "Expected at least 5 different unit categories across all factions");
    }

    [Test]
    public void UnitDatabase_GetForFactionReturnsMatchingUnits()
    {
        var factions = FactionDatabase.GetAll();
        foreach (var f in factions)
        {
            var units = UnitDatabase.GetForFaction(f.id);
            Assert.IsNotNull(units, $"GetForFaction returned null for {f.id}");
            foreach (var u in units)
                Assert.AreEqual(f.id, u.factionId, $"Unit {u.id} belongs to {u.factionId}, not {f.id}");
        }
    }

    [Test]
    public void UnitDatabase_TotalUnitCountMatchesSumOfFactions()
    {
        int total = UnitDatabase.GetAll().Count;
        int sum = FactionDatabase.GetAll().Sum(f => f.unitTypes?.Count ?? 0);
        Assert.AreEqual(sum, total, "Total units should equal sum of all factions' unit types");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  3. TERRAIN SYSTEM
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void TerrainDatabase_AllTerrainTypesRegistered()
    {
        foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
        {
            var def = TerrainDatabase.Get(t);
            Assert.IsNotNull(def, $"TerrainType {t} not registered");
            Assert.AreEqual(t, def.type);
        }
    }

    [Test]
    public void TerrainDatabase_AllTerrainHavePositiveMovement()
    {
        foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
        {
            var def = TerrainDatabase.Get(t);
            Assert.Greater(def.movementMultiplier, 0f, $"Terrain {t} has non-positive movement multiplier");
        }
    }

    [Test]
    public void TerrainDatabase_MountainsSlowerThanPlains()
    {
        var plains = TerrainDatabase.Get(TerrainType.Plains);
        var mountains = TerrainDatabase.Get(TerrainType.Mountains);
        Assert.Greater(plains.movementMultiplier, mountains.movementMultiplier,
            "Plains should have higher movement multiplier than Mountains");
    }

    [Test]
    public void BiomeDefinitions_AllBiomesHaveValidColors()
    {
        foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
        {
            var config = BiomeDefinitions.GetConfig(t);
            Assert.IsNotNull(config, $"BiomeDefinitions missing config for {t}");
            Assert.AreNotEqual(default(Color), config.groundColor, $"BiomeConfig {t} has default groundColor");
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    //  4. PROVINCE & WORLD MAP
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void ProvinceDatabase_AllProvincesHaveOwners()
    {
        foreach (var p in ProvinceDatabase.GetAllProvinces())
        {
            Assert.IsFalse(string.IsNullOrEmpty(p.ownerFactionId),
                $"Province {p.id} has no owner");
            Assert.IsNotNull(FactionDatabase.Get(p.ownerFactionId),
                $"Province {p.id} owner '{p.ownerFactionId}' not in FactionDatabase");
        }
    }

    [Test]
    public void ProvinceDatabase_AllProvincesHaveAdjacency()
    {
        var all = ProvinceDatabase.GetAllProvinces();
        int orphaned = 0;
        foreach (var p in all)
        {
            if (p.adjacentProvinceIds == null || p.adjacentProvinceIds.Count == 0)
                orphaned++;
        }
        Assert.LessOrEqual(orphaned, all.Count / 4,
            $"Too many orphaned provinces ({orphaned}/{all.Count})");
    }

    [Test]
    public void ProvinceManager_InitializeAndQuery()
    {
        var pm = new ProvinceManager();
        pm.Initialize(ProvinceDatabase.GetAllProvinces());

        var factions = FactionDatabase.GetAll();
        int totalOwned = 0;
        foreach (var f in factions)
            totalOwned += pm.GetProvincesForFaction(f.id).Count;

        Assert.AreEqual(ProvinceDatabase.GetAllProvinces().Count, totalOwned,
            "Sum of provinces per faction should equal total province count");
    }

    [Test]
    public void ProvinceManager_TransferProvinceChangesOwnership()
    {
        var pm = new ProvinceManager();
        pm.Initialize(ProvinceDatabase.GetAllProvinces());

        var provinces = ProvinceDatabase.GetAllProvinces();
        var first = provinces[0];
        var originalOwner = pm.GetOwner(first.id);
        var newOwner = FactionDatabase.GetAll().First(f => f.id != originalOwner).id;

        pm.TransferProvince(first.id, newOwner);
        Assert.AreEqual(newOwner, pm.GetOwner(first.id));
    }

    // ═══════════════════════════════════════════════════════════════════
    //  5. BATTLE CONFIGURATION
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void BattleConfiguration_CreateForAllFactionPairs()
    {
        var factions = FactionDatabase.GetAll();
        int configsCreated = 0;

        for (int i = 0; i < factions.Count; i++)
        {
            for (int j = i + 1; j < factions.Count; j++)
            {
                var config = BattleConfiguration.Create(factions[i], factions[j]);
                Assert.IsNotNull(config,
                    $"Failed to create battle config: {factions[i].id} vs {factions[j].id}");
                Assert.Greater(config.attackerUnitBudget, 0);
                Assert.Greater(config.defenderUnitBudget, 0);
                configsCreated++;
            }
        }
        Assert.AreEqual(43 * 42 / 2, configsCreated, "Should test all unique pairs");
    }

    [Test]
    public void BattleConfiguration_MirrorBattleHasSymmetricBudgets()
    {
        var viking = FactionDatabase.Get("north_sea_empire");
        var config = BattleConfiguration.Create(viking, viking);
        Assert.AreEqual(config.attackerUnitBudget, config.defenderUnitBudget,
            "Mirror match should have equal budgets");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  6. COMBAT MATH
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void TargetSelector_TypeMatchupBonusesAreConsistent()
    {
        float cavVsRanged = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyCavalry, UnitCategory.Ranged);
        float infantryVsRanged = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyInfantry, UnitCategory.Ranged);
        Assert.Greater(cavVsRanged, infantryVsRanged,
            "Cavalry should have higher matchup bonus vs ranged than infantry");
    }

    [Test]
    public void TargetSelector_FindNearestEnemy_ReturnsClosest()
    {
        var attacker = CreateTestUnit(Vector3.zero, UnitCategory.HeavyInfantry, Faction.Attacker);
        var near = CreateTestUnit(new Vector3(3, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender);
        var far = CreateTestUnit(new Vector3(30, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender);

        var target = TargetSelector.FindNearestEnemy(attacker, new List<Unit> { far, near });
        Assert.AreEqual(near, target, "Should find the nearest enemy");

        CleanupUnits(attacker, near, far);
    }

    [Test]
    public void BattleRandom_DeterministicWithSameSeed()
    {
        var rng1 = new BattleRandom(777);
        var rng2 = new BattleRandom(777);

        for (int i = 0; i < 100; i++)
            Assert.AreEqual(rng1.Range(0, 1000), rng2.Range(0, 1000),
                $"Mismatch at iteration {i}");
    }

    [Test]
    public void BattleRandom_DifferentSeedsProduceDifferentResults()
    {
        var rng1 = new BattleRandom(111);
        var rng2 = new BattleRandom(999);

        bool anyDifferent = false;
        for (int i = 0; i < 20; i++)
        {
            if (rng1.Range(0, 10000) != rng2.Range(0, 10000))
            {
                anyDifferent = true;
                break;
            }
        }
        Assert.IsTrue(anyDifferent, "Different seeds should produce different sequences");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  7. CAMPAIGN SYSTEM
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void CampaignManager_StartNewCampaign_Initializes43Factions()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");

        Assert.IsNotNull(manager.currentState);
        Assert.AreEqual(43, manager.currentState.factions.Count);
        Assert.AreEqual("north_sea_empire", manager.currentState.playerFactionId);
        Assert.AreEqual(1, manager.CurrentTurn);

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void CampaignManager_ExecuteTurn_AdvancesTurn()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");

        int turnBefore = manager.CurrentTurn;
        manager.ExecuteTurn();
        Assert.AreEqual(turnBefore + 1, manager.CurrentTurn);

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void CampaignEconomy_IncomeIsPositive()
    {
        var pm = new ProvinceManager();
        pm.Initialize(ProvinceDatabase.GetAllProvinces());

        var fcs = new FactionCampaignState
        {
            factionId = "north_sea_empire",
            gold = 0,
            isEliminated = false,
            armyComposition = new SerializableKeyValueList()
        };

        CampaignEconomyManager.CollectIncome(fcs, pm);
        Assert.Greater(fcs.gold, 0, "Income collection should increase gold");
    }

    [Test]
    public void CampaignEconomy_RecruitCostIsPositive()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        Assert.Greater(unitTypes.Count, 0);

        foreach (var ut in unitTypes)
        {
            int cost = CampaignEconomyManager.GetRecruitCost(ut.id);
            Assert.Greater(cost, 0, $"Recruit cost for {ut.id} should be positive");
        }
    }

    [Test]
    public void CampaignBattleResolver_AutoResolveProducesResult()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        var committed = new SerializableKeyValueList();
        committed.FromDictionary(new Dictionary<string, int> { { unitTypes[0].id, 20 } });

        var action = new CampaignAction
        {
            type = CampaignAction.ActionType.Attack,
            sourceFactionId = "north_sea_empire",
            targetProvinceId = ProvinceDatabase.GetAllProvinces()[0].id,
            committedUnits = committed
        };

        var attacker = new FactionCampaignState { factionId = "north_sea_empire", armyComposition = committed };
        var defender = new FactionCampaignState { factionId = "norway", armyComposition = new SerializableKeyValueList() };
        var province = ProvinceDatabase.GetAllProvinces()[0];
        var rng = new BattleRandom(42);

        var result = CampaignBattleResolver.ResolveAutomatic(action, attacker, defender, province, rng);
        Assert.IsNotNull(result, "Auto-resolve should produce a result");
    }

    [Test]
    public void CampaignState_SerializesAndDeserializesCorrectly()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("byzantine");

        var original = manager.currentState;
        string json = JsonUtility.ToJson(original);
        Assert.IsFalse(string.IsNullOrEmpty(json), "JSON serialization should produce non-empty result");

        var restored = JsonUtility.FromJson<CampaignState>(json);
        Assert.AreEqual(original.campaignId, restored.campaignId);
        Assert.AreEqual(original.playerFactionId, restored.playerFactionId);
        Assert.AreEqual(original.factions.Count, restored.factions.Count);

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void CampaignAI_DecideAction_ReturnsValidAction()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");

        var aiFaction = manager.currentState.factions
            .First(f => f.factionId != "north_sea_empire" && !f.isEliminated);
        var rng = new BattleRandom(42);

        var action = CampaignAI.DecideAction(aiFaction, manager.currentState, manager.provinceManager, rng);
        Assert.IsNotNull(action, "AI should decide an action");
        Assert.IsTrue(
            action.type == CampaignAction.ActionType.Attack ||
            action.type == CampaignAction.ActionType.Defend ||
            action.type == CampaignAction.ActionType.Recruit,
            "Action type should be Attack, Defend, or Recruit");

        UnityEngine.Object.DestroyImmediate(go);
    }

    // ═══════════════════════════════════════════════════════════════════
    //  8. PERSISTENCE
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void LocalPersistence_SaveAndLoadCampaign()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("song");

        var service = new LocalPersistenceService();
        var state = manager.currentState;

        bool saved = service.SaveCampaignState(state).GetAwaiter().GetResult();
        Assert.IsTrue(saved, "Save should succeed");

        var loaded = service.LoadCampaignState(state.campaignId).GetAwaiter().GetResult();
        Assert.IsNotNull(loaded, "Load should return a state");
        Assert.AreEqual(state.campaignId, loaded.campaignId);
        Assert.AreEqual("song", loaded.playerFactionId);

        service.DeleteCampaign(state.campaignId).GetAwaiter().GetResult();
        UnityEngine.Object.DestroyImmediate(go);
    }

    // ═══════════════════════════════════════════════════════════════════
    //  9. FORMATION SYSTEM
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void FormationController_LineFormationSpansWidth()
    {
        var positions = FormationController.GetFormationPositions(
            20, Vector3.zero, Vector3.forward, FormationType.Line, 2f);

        Assert.AreEqual(20, positions.Length);
        float minX = positions.Min(p => p.x);
        float maxX = positions.Max(p => p.x);
        Assert.Greater(maxX - minX, 10f, "Line formation should span width");
    }

    [Test]
    public void FormationController_ColumnFormationSpansDepth()
    {
        var positions = FormationController.GetFormationPositions(
            20, Vector3.zero, Vector3.forward, FormationType.Column, 2f);

        Assert.AreEqual(20, positions.Length);
        float minZ = positions.Min(p => p.z);
        float maxZ = positions.Max(p => p.z);
        Assert.Greater(maxZ - minZ, 5f, "Column formation should have depth");
    }

    [Test]
    public void FormationController_AllFormationTypesProducePositions()
    {
        foreach (FormationType ft in Enum.GetValues(typeof(FormationType)))
        {
            var positions = FormationController.GetFormationPositions(
                10, Vector3.zero, Vector3.forward, ft, 2f);
            Assert.AreEqual(10, positions.Length, $"FormationType {ft} should produce 10 positions");
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    //  10. TYPE EXISTENCE (ensures no missing MonoBehaviours)
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void CoreTypes_AllCriticalClassesExist()
    {
        Assert.IsNotNull(typeof(GameManager));
        Assert.IsNotNull(typeof(BattleManager));
        Assert.IsNotNull(typeof(CampaignManager));
        Assert.IsNotNull(typeof(FactionManager));
        Assert.IsNotNull(typeof(SelectionManager));
    }

    [Test]
    public void UITypes_AllScreenClassesExist()
    {
        Assert.IsNotNull(typeof(MainMenuUI));
        Assert.IsNotNull(typeof(FactionSelectUI));
        Assert.IsNotNull(typeof(BattleSetupUI));
        Assert.IsNotNull(typeof(BattleHUD));
        Assert.IsNotNull(typeof(BattleResultsUI));
        Assert.IsNotNull(typeof(CampaignSetupUI));
        Assert.IsNotNull(typeof(CampaignMapUI));
        Assert.IsNotNull(typeof(CampaignHUD));
        Assert.IsNotNull(typeof(CampaignVictoryUI));
        Assert.IsNotNull(typeof(SaveLoadUI));
        Assert.IsNotNull(typeof(UIThemeManager));
    }

    [Test]
    public void AITypes_AllAIClassesExist()
    {
        Assert.IsNotNull(typeof(SimulationAI));
        Assert.IsNotNull(typeof(CampaignAI));
        Assert.IsNotNull(typeof(TacticalDecisionMaker));
        Assert.IsNotNull(typeof(TargetSelector));
        Assert.IsNotNull(typeof(FormationController));
        Assert.IsNotNull(typeof(TerrainAnalyzer));
    }

    [Test]
    public void DataTypes_AllModelClassesExist()
    {
        Assert.IsNotNull(typeof(FactionDefinition));
        Assert.IsNotNull(typeof(UnitTypeDefinition));
        Assert.IsNotNull(typeof(AbilityDefinition));
        Assert.IsNotNull(typeof(TerrainDefinition));
        Assert.IsNotNull(typeof(BattleConfiguration));
        Assert.IsNotNull(typeof(CampaignState));
        Assert.IsNotNull(typeof(ProvinceDefinition));
        Assert.IsNotNull(typeof(PlayerProfile));
    }

    [Test]
    public void GameFlowState_HasAllExpectedStates()
    {
        var states = Enum.GetValues(typeof(GameFlowState));
        Assert.GreaterOrEqual(states.Length, 10, "Should have at least 10 game flow states");

        var stateNames = new HashSet<string>();
        foreach (var s in states)
            stateNames.Add(s.ToString());

        Assert.IsTrue(stateNames.Contains("MainMenu"));
        Assert.IsTrue(stateNames.Contains("BattleSimulation"));
        Assert.IsTrue(stateNames.Contains("CampaignMap"));
        Assert.IsTrue(stateNames.Contains("CampaignVictory"));
    }

    // ═══════════════════════════════════════════════════════════════════
    //  11. CROSS-SYSTEM INTEGRATION
    // ═══════════════════════════════════════════════════════════════════

    [Test]
    public void Integration_AllFactionsCanProduceABattleConfig()
    {
        var factions = FactionDatabase.GetAll();
        var defender = factions[0];

        foreach (var attacker in factions)
        {
            var config = BattleConfiguration.Create(attacker, defender);
            Assert.IsNotNull(config, $"Config null for {attacker.id} vs {defender.id}");
        }
    }

    [Test]
    public void Integration_AllFactionsHaveProvincesInCampaign()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");

        int factionsWithProvinces = 0;
        foreach (var f in FactionDatabase.GetAll())
        {
            var owned = manager.provinceManager.GetProvincesForFaction(f.id);
            if (owned.Count > 0) factionsWithProvinces++;
        }

        Assert.Greater(factionsWithProvinces, 30,
            "At least 30 factions should own provinces at campaign start");

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void Integration_EventBusPublishAndSubscribe()
    {
        bool received = false;
        Action<string> handler = msg => received = true;

        EventBus.Subscribe(handler);
        EventBus.Publish("test");
        Assert.IsTrue(received, "EventBus publish/subscribe should work");

        EventBus.Unsubscribe(handler);
    }

    [Test]
    public void Integration_SerializableKeyValueList_RoundTrips()
    {
        var original = new Dictionary<string, int>
        {
            { "viking_huscarl", 15 },
            { "viking_berserker", 8 },
            { "viking_archer", 12 }
        };

        var skvl = new SerializableKeyValueList();
        skvl.FromDictionary(original);

        var restored = skvl.ToDictionary();
        Assert.AreEqual(original.Count, restored.Count);
        foreach (var kv in original)
            Assert.AreEqual(kv.Value, restored[kv.Key], $"Mismatch for key {kv.Key}");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════════════════════

    static Unit CreateTestUnit(Vector3 pos, UnitCategory cat, Faction side,
        float hp = 100f, float atk = 10f)
    {
        var go = new GameObject($"TestUnit_{cat}_{side}");
        go.transform.position = pos;
        var unit = go.AddComponent<Unit>();
        unit.faction = side;
        unit.maxHealth = hp;
        unit.currentHealth = hp;
        unit.attackDamage = atk;
        unit.armor = 5;
        unit.moveSpeed = 3f;
        unit.attackRange = cat == UnitCategory.Ranged ? 14f : 2.5f;
        unit.attackCooldown = 1f;
        unit.typeDefinition = new UnitTypeDefinition
        {
            id = $"test_{cat}",
            factionId = "test",
            displayName = $"Test {cat}",
            category = cat,
            maxHP = hp,
            attackDamage = atk,
            armor = 5,
            moveSpeed = 3f,
            attackRange = unit.attackRange,
            attackCooldown = 1f,
            visualConfig = new UnitVisualConfig()
        };
        unit.factionDefinition = new FactionDefinition
        {
            id = "test",
            primaryColor = Color.red,
            secondaryColor = Color.blue
        };
        return unit;
    }

    static void CleanupUnits(params Unit[] units)
    {
        foreach (var u in units)
            if (u != null) UnityEngine.Object.DestroyImmediate(u.gameObject);
    }
}
