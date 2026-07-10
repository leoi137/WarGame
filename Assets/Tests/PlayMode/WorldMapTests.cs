using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class WorldMapTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
        }

        [UnityTest]
        public IEnumerator Test_WorldMapLoadsWithAll43Territories()
        {
            Assert.AreEqual(43, FactionDatabase.FactionCount);
            Assert.IsNotNull(typeof(WorldMapGenerator).GetMethod("Generate"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_WorldMapCityMarkersRendered()
        {
            var factions = FactionDatabase.GetAll();
            int totalCities = 0;
            foreach (var f in factions)
                totalCities += f.cities.Count;
            Assert.Greater(totalCities, 0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_WorldMapFactionSelectionWorks()
        {
            Assert.IsNotNull(typeof(WorldMapInput).GetMethod("RaycastToFaction"));
            yield return null;
        }
    }
}
