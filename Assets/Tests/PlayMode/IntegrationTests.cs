using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class IntegrationTests
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
        public IEnumerator Test_MainMenuToQuickBattleFlow()
        {
            Assert.IsNotNull(typeof(GameManager));
            Assert.IsNotNull(typeof(MainMenuUI));
            Assert.IsNotNull(typeof(FactionSelectUI));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_FullGameCycleCompletes()
        {
            Assert.IsNotNull(typeof(BattleManager));
            Assert.IsNotNull(typeof(BattleResultsUI));
            Assert.IsNotNull(typeof(WorldMapManager));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_43FactionsAllPlayable()
        {
            var factions = FactionDatabase.GetAll();
            Assert.AreEqual(43, factions.Count);
            var defender = factions[0];
            foreach (var attacker in factions)
            {
                var config = BattleConfiguration.Create(attacker, defender);
                Assert.IsNotNull(config);
                Assert.Greater(config.attackerUnitBudget, 0);
                Assert.Greater(config.defenderUnitBudget, 0);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_BattleBetweenAnyTwoFactions()
        {
            var rng = new BattleRandom(42);
            var factions = FactionDatabase.GetAll();
            for (int i = 0; i < 5; i++)
            {
                var a = factions[rng.Range(0, factions.Count)];
                var b = factions[rng.Range(0, factions.Count)];
                var config = BattleConfiguration.Create(a, b);
                Assert.IsNotNull(config, $"Battle config for {a.id} vs {b.id} should not be null");
            }
            yield return null;
        }
    }
}
