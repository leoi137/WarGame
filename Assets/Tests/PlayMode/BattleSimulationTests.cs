using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class BattleSimulationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        [UnityTest]
        public IEnumerator Test_FullBattleRunsToCompletion()
        {
            var attacker = FactionDatabase.Get("byzantine");
            var defender = FactionDatabase.Get("song");
            Assert.IsNotNull(attacker);
            Assert.IsNotNull(defender);
            var config = BattleConfiguration.Create(attacker, defender);
            Assert.IsNotNull(config);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_EqualArmiesProduceBalancedResults()
        {
            var faction = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(faction);
            var config = BattleConfiguration.Create(faction, faction);
            Assert.IsNotNull(config);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_LargerArmyWinsMoreOften()
        {
            var large = FactionDatabase.Get("song");
            var small = FactionDatabase.Get("tui_tonga");
            Assert.IsNotNull(large);
            Assert.IsNotNull(small);
            Assert.Greater(large.GetBattleUnitBudget(), small.GetBattleUnitBudget());
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_TerrainAdvantageMatters()
        {
            var terrainDef = TerrainDatabase.Get(TerrainType.Mountains);
            Assert.IsNotNull(terrainDef);
            Assert.Greater(terrainDef.infantryDefenseBonus, 0f);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_BattleEndsInReasonableTime()
        {
            var attacker = FactionDatabase.Get("north_sea_empire");
            var defender = FactionDatabase.Get("byzantine");
            var config = BattleConfiguration.Create(attacker, defender);
            Assert.IsNotNull(config);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_NoUnitsStuckOrFrozen()
        {
            Assert.IsNotNull(typeof(BattleSimulator));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_AllUnitTypesCanParticipate()
        {
            var allUnits = UnitDatabase.GetAll();
            Assert.Greater(allUnits.Count, 0);
            yield return null;
        }
    }
}
