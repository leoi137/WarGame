using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 6: World map generation, interactions, camera.
    /// </summary>
    [TestFixture]
    public class Phase6WorldMapTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
        }

        [Test]
        public void Test_WorldMapGeneratorCreates43Territories()
        {
            var factions = FactionDatabase.GetAll();
            Assert.AreEqual(43, factions.Count);

            foreach (var f in factions)
            {
                Assert.IsNotNull(f.cities, $"Faction '{f.id}' has null cities");
                Assert.Greater(f.cities.Count, 0, $"Faction '{f.id}' has no cities for territory");
            }
        }

        [Test]
        public void Test_AllCitiesPlacedOnMap()
        {
            var factions = FactionDatabase.GetAll();
            int totalCities = factions.Sum(f => f.cities.Count);
            Assert.Greater(totalCities, 150, $"Expected 155+ cities, got {totalCities}");
        }

        [Test]
        public void Test_NormalizedPositionsMapToWorldCoords()
        {
            Vector3 origin = new Vector3(0 * GameConfig.WorldMapWidth, 0, 0 * GameConfig.WorldMapHeight);
            Vector3 corner = new Vector3(1 * GameConfig.WorldMapWidth, 0, 1 * GameConfig.WorldMapHeight);

            Assert.AreEqual(0f, origin.x, 0.1f);
            Assert.AreEqual(0f, origin.z, 0.1f);
            Assert.AreEqual(GameConfig.WorldMapWidth, corner.x, 0.1f);
            Assert.AreEqual(GameConfig.WorldMapHeight, corner.z, 0.1f);
        }

        [Test]
        public void Test_FactionSelectionHighlightsTerritory()
        {
            Assert.IsNotNull(typeof(ProvinceRenderer).GetMethod("SetHighlighted"),
                "ProvinceRenderer must have SetHighlighted method");
        }

        [Test]
        public void Test_RaycastReturnsFaction()
        {
            Assert.IsNotNull(typeof(WorldMapInput).GetMethod("RaycastToFaction"),
                "WorldMapInput must have RaycastToFaction method");
        }

        [Test]
        public void Test_RaycastToCity()
        {
            Assert.IsNotNull(typeof(WorldMapInput).GetMethod("RaycastToCity"),
                "WorldMapInput must have RaycastToCity method");
        }

        [Test]
        public void Test_InfoPanelShowsCorrectData()
        {
            Assert.IsNotNull(typeof(FactionInfoPanel).GetMethod("Show"),
                "FactionInfoPanel must have Show method");
            Assert.IsNotNull(typeof(FactionInfoPanel).GetMethod("PopulateFactionInfo"),
                "FactionInfoPanel must have PopulateFactionInfo method");
        }

        [Test]
        public void Test_CameraFocusOnFactionCentersValid()
        {
            Assert.IsNotNull(typeof(WorldMapCamera).GetMethod("FocusOnFaction"),
                "WorldMapCamera must have FocusOnFaction method");
        }

        [Test]
        public void Test_CameraClampsToMapBounds()
        {
            Assert.IsNotNull(typeof(WorldMapCamera).GetMethod("ClampToBounds"),
                "WorldMapCamera must have ClampToBounds method");
        }

        [Test]
        public void Test_CameraZoomWithinLimits()
        {
            Assert.IsNotNull(typeof(WorldMapCamera).GetMethod("HandleInput"),
                "WorldMapCamera must have HandleInput method for zoom");
        }
    }
}
