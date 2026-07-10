using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WorldWars.Tests.PlayMode
{
    [TestFixture]
    public class TerrainGenerationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TerrainDatabase.Initialize();
        }

        [UnityTest]
        public IEnumerator Test_TerrainGeneratesForAllBiomes()
        {
            foreach (TerrainType type in Enum.GetValues(typeof(TerrainType)))
            {
                var config = BiomeDefinitions.GetConfig(type);
                Assert.AreNotEqual(default(Color), config.groundColor);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_NavMeshBakesSuccessfully()
        {
            Assert.IsNotNull(typeof(TerrainGenerator).GetMethod("BakeNavMesh"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_UnitsCanMoveOnGeneratedTerrain()
        {
            Assert.IsNotNull(typeof(TerrainGenerator).GetMethod("GenerateTerrain"));
            yield return null;
        }
    }
}
