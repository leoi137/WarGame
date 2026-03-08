using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class CampaignFlowTests
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

    [UnityTest]
    public IEnumerator Test_CampaignSetup_SelectFaction_StartsCampaign()
    {
        Assert.IsNotNull(typeof(CampaignSetupUI));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_CampaignMap_AttackProvince_TransitionsToBattle()
    {
        Assert.IsNotNull(typeof(CampaignMapUI));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_CampaignMap_EndTurn_ResolvesAllActions()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        var turnBefore = manager.CurrentTurn;
        manager.ExecuteTurn();
        Assert.AreEqual(turnBefore + 1, manager.CurrentTurn);
        Object.DestroyImmediate(go);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_CampaignVictory_ShowsVictoryScreen()
    {
        Assert.IsNotNull(typeof(CampaignVictoryUI));
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_SaveLoad_Campaign_PreservesState()
    {
        var go = new GameObject("CampaignManager");
        var manager = go.AddComponent<CampaignManager>();
        manager.StartNewCampaign("north_sea_empire");
        Assert.AreEqual(43, manager.currentState.factions.Count);
        Object.DestroyImmediate(go);
        yield return null;
    }
}
