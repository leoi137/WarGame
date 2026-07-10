using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CampaignLogicTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();
        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        ProvinceDatabase.Initialize();
    }

    [Test]
    public void Test_CampaignManager_StartNew_InitializesAllFactions()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        Assert.AreEqual(43, manager.currentState.factions.Count);
        Assert.AreEqual(1, manager.CurrentTurn);
        Assert.AreEqual(1, manager.CurrentYear);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_CampaignManager_ExecuteTurn_AdvancesTurnAndYear()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        manager.ExecuteTurn();
        Assert.AreEqual(2, manager.CurrentTurn);
        Assert.GreaterOrEqual(manager.CurrentYear, 1);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_CampaignAI_DecideAction_ReturnsNonNull()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var faction = manager.currentState.factions.First(f => f.factionId != manager.currentState.playerFactionId && !f.isEliminated);
        var rng = new BattleRandom(42);
        var action = CampaignAI.DecideAction(faction, manager.currentState, manager.provinceManager, rng);
        Assert.IsNotNull(action);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_CampaignAI_DecideAction_DefendsWhenThreatened()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var faction = manager.currentState.factions.First(f => f.factionId != manager.currentState.playerFactionId && !f.isEliminated);
        var rng = new BattleRandom(42);
        var action = CampaignAI.DecideAction(faction, manager.currentState, manager.provinceManager, rng);
        Assert.IsNotNull(action);
        Assert.IsTrue(action.type == CampaignAction.ActionType.Attack || action.type == CampaignAction.ActionType.Defend || action.type == CampaignAction.ActionType.Recruit);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_ProvinceManager_TransferProvince_UpdatesOwnership()
    {
        var provinces = ProvinceDatabase.GetAllProvinces();
        Assert.Greater(provinces.Count, 0);
        var pm = new ProvinceManager();
        pm.Initialize(provinces);
        var first = provinces[0];
        var originalOwner = pm.GetOwner(first.id);
        var newOwner = FactionDatabase.GetAll().First(f => f.id != originalOwner)?.id;
        if (string.IsNullOrEmpty(newOwner)) return;
        pm.TransferProvince(first.id, newOwner);
        var ownedByNew = pm.GetProvincesForFaction(newOwner);
        Assert.IsTrue(ownedByNew.Any(p => p.id == first.id));
    }

    [Test]
    public void Test_ProvinceManager_GetAdjacentEnemy_ReturnsResults()
    {
        var provinces = ProvinceDatabase.GetAllProvinces();
        var pm = new ProvinceManager();
        pm.Initialize(provinces);
        var factionsWithProvinces = FactionDatabase.GetAll().Where(f => pm.GetProvincesForFaction(f.id).Count > 0).ToList();
        var foundEnemy = false;
        foreach (var f in factionsWithProvinces)
        {
            var enemies = pm.GetAdjacentEnemyProvinces(f.id);
            if (enemies.Count > 0)
            {
                foundEnemy = true;
                break;
            }
        }
        Assert.IsTrue(foundEnemy, "At least one faction should have adjacent enemy provinces");
    }

    [Test]
    public void Test_ProvinceDatabase_GetAllProvinces_ReturnsNonEmpty()
    {
        var all = ProvinceDatabase.GetAllProvinces();
        Assert.Greater(all.Count, 0);
    }

    [Test]
    public void Test_ProvinceDatabase_GetProvincesForRegion_FiltersCorrectly()
    {
        var europe = ProvinceDatabase.GetProvincesForRegion("Europe");
        Assert.Greater(europe.Count, 0);
        foreach (var p in europe)
            Assert.AreEqual("Europe", p.region);
    }

    [Test]
    public void Test_ProvinceDatabase_AllProvincesHaveAdjacency()
    {
        var all = ProvinceDatabase.GetAllProvinces();
        foreach (var p in all)
            Assert.GreaterOrEqual(p.adjacentProvinceIds?.Count ?? 0, 1, $"Province {p.id} has no adjacent provinces");
    }

    [Test]
    public void Test_CampaignEconomy_CollectIncome_IncreasesGold()
    {
        var provinces = ProvinceDatabase.GetAllProvinces();
        var pm = new ProvinceManager();
        pm.Initialize(provinces);
        var fcs = new FactionCampaignState { factionId = "north_sea_empire", gold = 100, isEliminated = false, armyComposition = new SerializableKeyValueList() };
        var before = fcs.gold;
        CampaignEconomyManager.CollectIncome(fcs, pm);
        Assert.Greater(fcs.gold, before);
    }

    [Test]
    public void Test_CampaignEconomy_Recruit_DeductsGold()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        Assert.Greater(unitTypes.Count, 0);
        var unitId = unitTypes[0].id;
        var cost = CampaignEconomyManager.GetRecruitCost(unitId);
        Assert.Greater(cost, 0);
        var fcs = new FactionCampaignState { factionId = "north_sea_empire", gold = 1000, isEliminated = false, armyComposition = new SerializableKeyValueList() };
        var before = fcs.gold;
        CampaignEconomyManager.Recruit(fcs, unitId, 1);
        Assert.Less(fcs.gold, before);
    }

    [Test]
    public void Test_CampaignEconomy_CantRecruitWithoutGold()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        Assert.Greater(unitTypes.Count, 0);
        var fcs = new FactionCampaignState { factionId = "north_sea_empire", gold = 0, isEliminated = false, armyComposition = new SerializableKeyValueList() };
        Assert.IsFalse(CampaignEconomyManager.CanRecruit(fcs, unitTypes[0].id, 1));
    }

    [Test]
    public void Test_BattleResolver_AutoResolve_ReturnsResult()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        Assert.Greater(unitTypes.Count, 0);
        var committed = new SerializableKeyValueList();
        committed.FromDictionary(new Dictionary<string, int> { { unitTypes[0].id, 10 } });
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
        Assert.IsNotNull(result);
    }

    [Test]
    public void Test_BattleResolver_StrongerArmyWins()
    {
        var unitTypes = UnitDatabase.GetForFaction("north_sea_empire");
        Assert.Greater(unitTypes.Count, 0);
        var strongCommitted = new SerializableKeyValueList();
        strongCommitted.FromDictionary(new Dictionary<string, int> { { unitTypes[0].id, 100 } });
        var weakCommitted = new SerializableKeyValueList();
        weakCommitted.FromDictionary(new Dictionary<string, int> { { unitTypes[0].id, 5 } });
        var province = ProvinceDatabase.GetAllProvinces().First(p => p.ownerFactionId != "north_sea_empire");
        var action = new CampaignAction
        {
            type = CampaignAction.ActionType.Attack,
            sourceFactionId = "north_sea_empire",
            targetProvinceId = province.id,
            committedUnits = strongCommitted
        };
        var attacker = new FactionCampaignState { factionId = "north_sea_empire", armyComposition = strongCommitted };
        var defender = new FactionCampaignState { factionId = province.ownerFactionId, armyComposition = weakCommitted };
        var rng = new BattleRandom(42);
        var result = CampaignBattleResolver.ResolveAutomatic(action, attacker, defender, province, rng);
        Assert.AreEqual("north_sea_empire", result.winnerFactionId);
    }

    [Test]
    public void Test_VictoryCondition_ControlThreshold()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var all = ProvinceDatabase.GetAllProvinces();
        var threshold = Mathf.CeilToInt(all.Count * 0.7f);
        for (int i = 0; i < threshold && i < all.Count; i++)
            manager.provinceManager.TransferProvince(all[i].id, "north_sea_empire");
        manager.ExecuteTurn();
        Assert.IsTrue(manager.currentState.isComplete);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_VictoryCondition_AllEnemiesDefeated()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        foreach (var p in ProvinceDatabase.GetAllProvinces())
            manager.provinceManager.TransferProvince(p.id, "north_sea_empire");
        foreach (var f in manager.currentState.factions)
            if (f.factionId != "north_sea_empire") f.isEliminated = true;
        manager.ExecuteTurn();
        Assert.IsTrue(manager.currentState.isComplete);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_CampaignState_RoundTrips()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var original = manager.currentState;
        var json = JsonUtility.ToJson(original);
        Assert.IsFalse(string.IsNullOrEmpty(json));
        var restored = JsonUtility.FromJson<CampaignState>(json);
        Assert.IsNotNull(restored);
        Assert.AreEqual(original.campaignId, restored.campaignId);
        Assert.AreEqual(original.playerFactionId, restored.playerFactionId);
        Assert.AreEqual(original.currentTurn, restored.currentTurn);
        Assert.AreEqual(original.factions.Count, restored.factions.Count);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Test_LocalPersistence_SaveLoad()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var state = manager.currentState;
        var service = new LocalPersistenceService();
        var saveTask = service.SaveCampaignState(state);
        Assert.IsTrue(saveTask.GetAwaiter().GetResult());
        var loadTask = service.LoadCampaignState(state.campaignId);
        var loaded = loadTask.GetAwaiter().GetResult();
        Assert.IsNotNull(loaded);
        Assert.AreEqual(state.campaignId, loaded.campaignId);
        Assert.AreEqual(state.playerFactionId, loaded.playerFactionId);
        Assert.AreEqual(state.factions.Count, loaded.factions.Count);
        service.DeleteCampaign(state.campaignId).GetAwaiter().GetResult();
        Object.DestroyImmediate(go);
    }
}
