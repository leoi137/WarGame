using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class TerrainEffectTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TerrainDatabase.Initialize();
        }

        [Test]
        public void Test_PlainsFullSpeed()
        {
            var def = TerrainDatabase.Get(TerrainType.Plains);
            Assert.IsNotNull(def);
            Assert.AreEqual(1.0f, def.movementMultiplier, 0.01f);
        }

        [Test]
        public void Test_ForestSlowsMovement()
        {
            var def = TerrainDatabase.Get(TerrainType.Forest);
            Assert.IsNotNull(def);
            Assert.Less(def.movementMultiplier, 1.0f, "Forest should slow movement");
        }

        [Test]
        public void Test_MountainsHeavilySlows()
        {
            var def = TerrainDatabase.Get(TerrainType.Mountains);
            Assert.IsNotNull(def);
            Assert.LessOrEqual(def.movementMultiplier, 0.5f, "Mountains should heavily slow movement");
        }

        [Test]
        public void Test_CavalryFastOnSteppe()
        {
            var def = TerrainDatabase.Get(TerrainType.Steppe);
            Assert.IsNotNull(def);
            Assert.GreaterOrEqual(def.movementMultiplier, 1.0f, "Steppe should be fast terrain");
        }

        [Test]
        public void Test_CavalrySlowInWetlands()
        {
            var def = TerrainDatabase.Get(TerrainType.Wetlands);
            Assert.IsNotNull(def);
            Assert.Less(def.movementMultiplier, 0.8f, "Wetlands should slow movement significantly");
        }

        [Test]
        public void Test_InfantryDefenseBonusInForest()
        {
            var def = TerrainDatabase.Get(TerrainType.Forest);
            Assert.IsNotNull(def);
            Assert.GreaterOrEqual(def.defenseBonus, 3.0f, "Forest should provide defense bonus >= 3");
        }

        [Test]
        public void Test_RangedPenaltyInJungle()
        {
            var def = TerrainDatabase.Get(TerrainType.Jungle);
            Assert.IsNotNull(def);
            Assert.Less(def.rangedAccuracy, 1.0f, "Jungle should penalize ranged accuracy");
        }

        [Test]
        public void Test_DesertNoDefenseBonus()
        {
            var def = TerrainDatabase.Get(TerrainType.Desert);
            Assert.IsNotNull(def);
            Assert.LessOrEqual(def.defenseBonus, 0f, "Desert should provide no defense bonus");
        }
    }
}
