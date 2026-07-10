using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class UnitSpawnTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
        }

        [UnityTest]
        public IEnumerator Test_UnitFactorySpawnsAllVikingTypes()
        {
            var faction = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(faction);
            Assert.GreaterOrEqual(faction.unitTypes.Count, 5);
            foreach (var ut in faction.unitTypes)
            {
                var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
                Assert.IsNotNull(unit);
                Object.DestroyImmediate(unit.gameObject);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_UnitFactorySpawnsAsianUnits()
        {
            var faction = FactionDatabase.Get("song");
            Assert.IsNotNull(faction);
            foreach (var ut in faction.unitTypes)
            {
                var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
                Assert.IsNotNull(unit);
                Object.DestroyImmediate(unit.gameObject);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_UnitFactorySpawnsAfricanUnits()
        {
            var faction = FactionDatabase.Get("ghana");
            Assert.IsNotNull(faction);
            foreach (var ut in faction.unitTypes)
            {
                var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
                Assert.IsNotNull(unit);
                Object.DestroyImmediate(unit.gameObject);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_SpawnedUnitHasHealthBar()
        {
            var faction = FactionDatabase.Get("north_sea_empire");
            var ut = faction.unitTypes[0];
            var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
            Assert.IsNotNull(unit.GetComponentInChildren<HealthBar>());
            Object.DestroyImmediate(unit.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_SpawnedUnitHasNavMeshAgent()
        {
            var faction = FactionDatabase.Get("north_sea_empire");
            var ut = faction.unitTypes[0];
            var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
            Assert.IsNotNull(unit.GetComponent<UnityEngine.AI.NavMeshAgent>());
            Object.DestroyImmediate(unit.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_SpawnedUnitModelHasGeometry()
        {
            var faction = FactionDatabase.Get("north_sea_empire");
            var ut = faction.unitTypes[0];
            var unit = UnitFactory.CreateUnit(ut, faction, Faction.Attacker, Vector3.zero, Quaternion.identity);
            var renderers = unit.GetComponentsInChildren<MeshRenderer>();
            Assert.Greater(renderers.Length, 0, "Spawned unit should have mesh renderers");
            Object.DestroyImmediate(unit.gameObject);
            yield return null;
        }
    }
}
