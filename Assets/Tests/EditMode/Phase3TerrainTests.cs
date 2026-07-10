using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 3: Terrain generation, biome configs, elevation, rivers, and combat modifiers.
    /// Tests that can run without a scene (EditMode) focus on data integrity and determinism.
    /// </summary>
    [TestFixture]
    public class Phase3TerrainTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TerrainDatabase.Initialize();
        }

        // ── Biome Visual Tests ─────────────────────────────────────

        [Test]
        public void Test_PlainsTerrainIsFlat()
        {
            var config = BiomeDefinitions.GetConfig(TerrainType.Plains);
            float[,] heights = ElevationGenerator.Generate(64, config.noiseScale, config.noiseOctaves, config.persistence, config.lacunarity, 42);
            heights = ElevationGenerator.ApplyBiomeProfile(heights, config);

            float sum = 0f, sumSq = 0f;
            int count = 64 * 64;
            for (int x = 0; x < 64; x++)
                for (int z = 0; z < 64; z++)
                {
                    sum += heights[x, z];
                    sumSq += heights[x, z] * heights[x, z];
                }
            float mean = sum / count;
            float stddev = Mathf.Sqrt(sumSq / count - mean * mean);
            Assert.Less(stddev, 0.1f, $"Plains stddev={stddev:F4}, expected < 0.1");
        }

        [Test]
        public void Test_MountainsTerrainHasPeaks()
        {
            var config = BiomeDefinitions.GetConfig(TerrainType.Mountains);
            float[,] heights = ElevationGenerator.Generate(64, config.noiseScale, config.noiseOctaves, config.persistence, config.lacunarity, 42);
            heights = ElevationGenerator.ApplyBiomeProfile(heights, config);

            float maxH = 0f;
            for (int x = 0; x < 64; x++)
                for (int z = 0; z < 64; z++)
                    if (heights[x, z] > maxH) maxH = heights[x, z];

            float threshold = 0.8f * config.maxElevation / GameConfig.TerrainHeightScale;
            Assert.Greater(maxH, threshold,
                $"Mountains peak={maxH:F3} below threshold={threshold:F3}");
        }

        [Test]
        public void Test_ForestHasHighTreeDensity()
        {
            var config = BiomeDefinitions.GetConfig(TerrainType.Forest);
            Assert.GreaterOrEqual(config.vegetationDensity, 0.6f);
        }

        [Test]
        public void Test_DesertHasNoTrees()
        {
            var config = BiomeDefinitions.GetConfig(TerrainType.Desert);
            Assert.AreEqual(0f, config.vegetationDensity);
        }

        [Test]
        public void Test_EachBiomeProducesDistinctTerrain()
        {
            var stats = new Dictionary<TerrainType, (float mean, float stddev)>();

            foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
            {
                var config = BiomeDefinitions.GetConfig(t);
                float[,] heights = ElevationGenerator.Generate(32, config.noiseScale, config.noiseOctaves,
                    config.persistence, config.lacunarity, 42);
                heights = ElevationGenerator.ApplyBiomeProfile(heights, config);

                float sum = 0f, sumSq = 0f;
                int count = 32 * 32;
                for (int x = 0; x < 32; x++)
                    for (int z = 0; z < 32; z++)
                    {
                        sum += heights[x, z];
                        sumSq += heights[x, z] * heights[x, z];
                    }
                float mean = sum / count;
                float stddev = Mathf.Sqrt(Mathf.Max(0, sumSq / count - mean * mean));
                stats[t] = (mean, stddev);
            }

            int distinctPairs = 0;
            var types = new List<TerrainType>(stats.Keys);
            for (int i = 0; i < types.Count; i++)
                for (int j = i + 1; j < types.Count; j++)
                {
                    var a = stats[types[i]];
                    var b = stats[types[j]];
                    float diff = Mathf.Abs(a.mean - b.mean) + Mathf.Abs(a.stddev - b.stddev);
                    if (diff > 0.01f) distinctPairs++;
                }

            int totalPairs = types.Count * (types.Count - 1) / 2;
            Assert.Greater(distinctPairs, totalPairs * 0.6f,
                "At least 60% of biome pairs should produce measurably different heightmaps");
        }

        // ── River Tests ────────────────────────────────────────────

        [Test]
        public void Test_RiverPathCrossesMap()
        {
            var path = RiverGenerator.GenerateRiverPath(300, 42);
            Assert.IsNotNull(path);
            Assert.Greater(path.Length, 2);

            float minX = float.MaxValue, maxX = float.MinValue;
            foreach (var p in path)
            {
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;
            }
            float span = maxX - minX;
            Assert.GreaterOrEqual(span, 0.7f,
                $"River spans {span:F2} of map width, expected >= 0.70");
        }

        [Test]
        public void Test_RiverPathIsContinuous()
        {
            var path = RiverGenerator.GenerateRiverPath(300, 42);
            float maxStep = 2f / 300f;
            for (int i = 1; i < path.Length; i++)
            {
                float dist = Vector2.Distance(path[i - 1], path[i]);
                Assert.Less(dist, maxStep * 10f,
                    $"River gap at segment {i}: dist={dist:F4}");
            }
        }

        // ── Elevation Tests ────────────────────────────────────────

        [Test]
        public void Test_ElevationGeneratorReturnsValidHeightmap()
        {
            float[,] heights = ElevationGenerator.Generate(64, 1f, 4, 0.5f, 2f, 42);
            Assert.AreEqual(64, heights.GetLength(0));
            Assert.AreEqual(64, heights.GetLength(1));

            for (int x = 0; x < 64; x++)
                for (int z = 0; z < 64; z++)
                    Assert.That(heights[x, z], Is.InRange(0f, 1f),
                        $"Height at [{x},{z}] = {heights[x, z]:F4} out of [0,1]");
        }

        // ── Terrain Combat Modifier Tests ──────────────────────────

        [Test]
        public void Test_AllMovementMultipliersInRange()
        {
            foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
            {
                var def = TerrainDatabase.Get(t);
                Assert.IsNotNull(def, $"Missing TerrainDefinition for {t}");
                Assert.That(def.movementMultiplier, Is.InRange(0.4f, 1.4f),
                    $"{t} movementMultiplier={def.movementMultiplier} out of range");
            }
        }

        [Test]
        public void Test_CavalrySlowInMountains()
        {
            var mountains = TerrainDatabase.Get(TerrainType.Mountains);
            Assert.LessOrEqual(mountains.cavalrySpeedMultiplier, 0.3f,
                $"Cavalry speed in mountains = {mountains.cavalrySpeedMultiplier}");
        }

        [Test]
        public void Test_CavalryFastOnSteppe()
        {
            var steppe = TerrainDatabase.Get(TerrainType.Steppe);
            Assert.GreaterOrEqual(steppe.cavalrySpeedMultiplier, 1.4f,
                $"Cavalry speed on steppe = {steppe.cavalrySpeedMultiplier}");
        }

        [Test]
        public void Test_InfantryDefenseBonusInForest()
        {
            var forest = TerrainDatabase.Get(TerrainType.Forest);
            Assert.GreaterOrEqual(forest.infantryDefenseBonus, 3f,
                $"Infantry defense in forest = {forest.infantryDefenseBonus}");
        }

        [Test]
        public void Test_HighGroundDamageBonus()
        {
            float advantage = TerrainEffects.GetElevationAdvantage(
                new Vector3(0, 10, 0), new Vector3(0, 0, 0));
            Assert.Greater(advantage, 0f,
                "Attacker 10 units higher should get positive elevation advantage");
        }

        // ── Determinism Tests ──────────────────────────────────────

        [Test]
        public void Test_TerrainGenerationDeterministic()
        {
            var config = BiomeDefinitions.GetConfig(TerrainType.Forest);
            float[,] a = ElevationGenerator.Generate(32, config.noiseScale, config.noiseOctaves, config.persistence, config.lacunarity, 777);
            float[,] b = ElevationGenerator.Generate(32, config.noiseScale, config.noiseOctaves, config.persistence, config.lacunarity, 777);

            for (int x = 0; x < 32; x++)
                for (int z = 0; z < 32; z++)
                    Assert.AreEqual(a[x, z], b[x, z],
                        $"Non-deterministic at [{x},{z}]: {a[x, z]} != {b[x, z]}");
        }
    }
}
