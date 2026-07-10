using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Validates all Phase 2 data: 43 factions, ~216 units, 26 abilities, 10 terrains.
    /// Tests are structural/data-integrity checks that run without scene context.
    /// </summary>
    [TestFixture]
    public class Phase2DataModelTests
    {
        private List<FactionDefinition> _factions;
        private List<UnitTypeDefinition> _units;
        private List<AbilityDefinition> _abilities;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            TerrainDatabase.Initialize();

            _factions = FactionDatabase.GetAll();
            _units = UnitDatabase.GetAll();
            _abilities = AbilityDatabase.GetAll();
        }

        // ── Faction Validation ─────────────────────────────────────

        [Test]
        public void Test_AllFactionsHaveValidId()
        {
            foreach (var f in _factions)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(f.id),
                    $"Faction has null/empty ID: {f.displayName}");
                Assert.IsFalse(f.id.Contains(" "),
                    $"Faction ID contains spaces: '{f.id}'");
            }
        }

        [Test]
        public void Test_AllFactionsHave4To8UnitTypes()
        {
            foreach (var f in _factions)
            {
                Assert.IsNotNull(f.unitTypes, $"Faction '{f.id}' has null unitTypes");
                Assert.That(f.unitTypes.Count, Is.InRange(4, 8),
                    $"Faction '{f.id}' has {f.unitTypes.Count} unit types (expected 4-8)");
            }
        }

        [Test]
        public void Test_AllFactionsHaveCities()
        {
            foreach (var f in _factions)
            {
                Assert.IsNotNull(f.cities, $"Faction '{f.id}' has null cities");
                Assert.GreaterOrEqual(f.cities.Count, 3,
                    $"Faction '{f.id}' has only {f.cities.Count} cities (min 3)");
            }
        }

        [Test]
        public void Test_AllFactionsHaveCapital()
        {
            foreach (var f in _factions)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(f.capitalCityId),
                    $"Faction '{f.id}' has no capitalCityId set");
                var capital = f.GetCapital();
                Assert.IsNotNull(capital,
                    $"Faction '{f.id}' capitalCityId '{f.capitalCityId}' not found in cities list");
                Assert.IsTrue(capital.isCapital,
                    $"Capital city '{capital.id}' of '{f.id}' has isCapital=false");
            }
        }

        [Test]
        public void Test_AllCitiesHavePositiveGarrison()
        {
            foreach (var f in _factions)
                foreach (var c in f.cities)
                    Assert.Greater(c.garrison, 0,
                        $"City '{c.id}' in faction '{f.id}' has garrison {c.garrison}");
        }

        [Test]
        public void Test_AllCityPositionsInRange()
        {
            foreach (var f in _factions)
                foreach (var c in f.cities)
                {
                    Assert.That(c.normalizedPosition.x, Is.InRange(0f, 1f),
                        $"City '{c.id}' X position {c.normalizedPosition.x} out of [0,1]");
                    Assert.That(c.normalizedPosition.y, Is.InRange(0f, 1f),
                        $"City '{c.id}' Y position {c.normalizedPosition.y} out of [0,1]");
                }
        }

        [Test]
        public void Test_NoDuplicateFactionIds()
        {
            var ids = _factions.Select(f => f.id).ToList();
            var dupes = ids.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(dupes, $"Duplicate faction IDs: {string.Join(", ", dupes)}");
        }

        [Test]
        public void Test_NoDuplicateUnitTypeIds()
        {
            var ids = _units.Select(u => u.id).ToList();
            var dupes = ids.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(dupes, $"Duplicate unit type IDs: {string.Join(", ", dupes)}");
        }

        [Test]
        public void Test_FactionDatabaseHas43Factions()
        {
            Assert.AreEqual(43, FactionDatabase.FactionCount);
        }

        [Test]
        public void Test_AllRegionsHaveFactions()
        {
            foreach (Region r in Enum.GetValues(typeof(Region)))
            {
                var regionFactions = FactionDatabase.GetByRegion(r);
                Assert.IsNotEmpty(regionFactions,
                    $"Region '{r}' has zero factions");
            }
        }

        // ── Unit Type Validation ───────────────────────────────────

        [Test]
        public void Test_AllUnitTypesHavePositiveStats()
        {
            foreach (var u in _units)
            {
                Assert.Greater(u.maxHP, 0f, $"Unit '{u.id}' has HP <= 0");
                Assert.Greater(u.attackDamage, 0f, $"Unit '{u.id}' has ATK <= 0");
                Assert.GreaterOrEqual(u.armor, 0f, $"Unit '{u.id}' has negative armor");
                Assert.Greater(u.moveSpeed, 0f, $"Unit '{u.id}' has moveSpeed <= 0");
            }
        }

        [Test]
        public void Test_AllUnitTypesHaveValidCategory()
        {
            foreach (var u in _units)
                Assert.IsTrue(Enum.IsDefined(typeof(UnitCategory), u.category),
                    $"Unit '{u.id}' has invalid category {u.category}");
        }

        [Test]
        public void Test_AllUnitTypesHaveVisualConfig()
        {
            foreach (var u in _units)
            {
                Assert.IsNotNull(u.visualConfig, $"Unit '{u.id}' has null visualConfig");
                Assert.IsTrue(Enum.IsDefined(typeof(WeaponStyle), u.visualConfig.primaryWeapon),
                    $"Unit '{u.id}' has invalid primaryWeapon");
            }
        }

        // ── Terrain Validation ─────────────────────────────────────

        [Test]
        public void Test_AllTerrainDistributionsSumNear100()
        {
            foreach (var f in _factions)
            {
                if (f.terrainDistribution == null || f.terrainDistribution.Count == 0) continue;
                float sum = f.terrainDistribution.Values.Sum();
                Assert.That(sum, Is.InRange(0.95f, 1.05f),
                    $"Faction '{f.id}' terrain distribution sums to {sum:F2}");
            }
        }

        [Test]
        public void Test_TerrainDatabaseHas10Types()
        {
            var terrains = TerrainDatabase.GetAll();
            Assert.AreEqual(10, terrains.Count);

            foreach (TerrainType t in Enum.GetValues(typeof(TerrainType)))
                Assert.IsNotNull(TerrainDatabase.Get(t), $"Missing terrain definition for {t}");
        }

        [Test]
        public void Test_FactionColorUniqueness()
        {
            for (int i = 0; i < _factions.Count; i++)
                for (int j = i + 1; j < _factions.Count; j++)
                {
                    var a = _factions[i].primaryColor;
                    var b = _factions[j].primaryColor;
                    float dist = Mathf.Sqrt(
                        (a.r - b.r) * (a.r - b.r) +
                        (a.g - b.g) * (a.g - b.g) +
                        (a.b - b.b) * (a.b - b.b));
                    Assert.Greater(dist, 0.05f,
                        $"Factions '{_factions[i].id}' and '{_factions[j].id}' have near-identical colors (dist={dist:F3})");
                }
        }

        // ── Ability Validation ─────────────────────────────────────

        [Test]
        public void Test_AllAbilitiesHaveValidId()
        {
            var ids = _abilities.Select(a => a.id).ToList();
            foreach (var id in ids)
                Assert.IsFalse(string.IsNullOrWhiteSpace(id), "Ability has null/empty ID");

            var dupes = ids.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            Assert.IsEmpty(dupes, $"Duplicate ability IDs: {string.Join(", ", dupes)}");
        }

        [Test]
        public void Test_AllUnitAbilityReferencesExist()
        {
            foreach (var u in _units)
            {
                if (string.IsNullOrEmpty(u.abilityId)) continue;
                Assert.IsNotNull(AbilityDatabase.Get(u.abilityId),
                    $"Unit '{u.id}' references non-existent ability '{u.abilityId}'");
            }
        }

        [Test]
        public void Test_AbilityTargetCategoryConditionValid()
        {
            foreach (var a in _abilities)
            {
                if (a.targetCategoryCondition == null) continue;
                foreach (var cat in a.targetCategoryCondition)
                    Assert.IsTrue(Enum.IsDefined(typeof(UnitCategory), cat),
                        $"Ability '{a.id}' has invalid target category {cat}");
            }
        }

        [Test]
        public void Test_AbilityTerrainConditionValid()
        {
            foreach (var a in _abilities)
            {
                if (a.terrainCondition == null) continue;
                foreach (var t in a.terrainCondition)
                    Assert.IsTrue(Enum.IsDefined(typeof(TerrainType), t),
                        $"Ability '{a.id}' has invalid terrain condition {t}");
            }
        }

        [Test]
        public void Test_PikeBraceHasCavalryTargetCondition()
        {
            var pike = AbilityDatabase.Get("pike_brace");
            Assert.IsNotNull(pike, "pike_brace ability not found");
            Assert.IsNotNull(pike.targetCategoryCondition);
            Assert.Contains(UnitCategory.HeavyCavalry, pike.targetCategoryCondition);
            Assert.Contains(UnitCategory.LightCavalry, pike.targetCategoryCondition);
        }

        [Test]
        public void Test_NavalBoardingHasCoastTerrainCondition()
        {
            var naval = AbilityDatabase.Get("naval_boarding");
            Assert.IsNotNull(naval, "naval_boarding ability not found");
            Assert.IsNotNull(naval.terrainCondition);
            Assert.Contains(TerrainType.Coast, naval.terrainCondition);
        }

        [Test]
        public void Test_AmbushHasForestTerrainCondition()
        {
            var ambush = AbilityDatabase.Get("ambush");
            Assert.IsNotNull(ambush, "ambush ability not found");
            Assert.IsNotNull(ambush.terrainCondition);
            Assert.Contains(TerrainType.Forest, ambush.terrainCondition);
            Assert.Contains(TerrainType.Jungle, ambush.terrainCondition);
        }

        // ── Data Model Tests ───────────────────────────────────────

        [Test]
        public void Test_GetBattleUnitBudgetClampedTo48And800()
        {
            var tiny = new FactionDefinition { estimatedMilitary = 1000 };
            Assert.AreEqual(48, tiny.GetBattleUnitBudget());

            var huge = new FactionDefinition { estimatedMilitary = 900000 };
            Assert.AreEqual(800, huge.GetBattleUnitBudget());

            var mid = new FactionDefinition { estimatedMilitary = 48000 };
            int budget = mid.GetBattleUnitBudget();
            Assert.That(budget, Is.InRange(48, 800));
        }

        [Test]
        public void Test_GetCapitalReturnsMatchingCity()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(nse);
            var cap = nse.GetCapital();
            Assert.IsNotNull(cap);
            Assert.AreEqual(nse.capitalCityId, cap.id);
        }

        [Test]
        public void Test_GetDominantTerrainReturnsHighest()
        {
            var f = new FactionDefinition();
            f.terrainDistribution[TerrainType.Mountains] = 0.1f;
            f.terrainDistribution[TerrainType.Desert] = 0.6f;
            f.terrainDistribution[TerrainType.Plains] = 0.3f;
            Assert.AreEqual(TerrainType.Desert, f.GetDominantTerrain());
        }

        [Test]
        public void Test_BattleConfigurationStoresFactionData()
        {
            var atk = new FactionDefinition { id = "a", displayName = "A", estimatedMilitary = 50000 };
            var def2 = new FactionDefinition { id = "d", displayName = "D", estimatedMilitary = 30000 };
            var config = BattleConfiguration.Create(atk, def2);
            Assert.AreEqual("a", config.attackerFaction.id);
            Assert.AreEqual("d", config.defenderFaction.id);
        }

        [Test]
        public void Test_BattleResultStoresWinnerAndCasualties()
        {
            var result = new BattleResult
            {
                winnerFactionId = "song",
                loserFactionId = "liao",
                winningSide = Faction.Attacker,
                winnerSurvivors = 100,
                totalCasualties = 250
            };
            Assert.AreEqual("song", result.winnerFactionId);
            Assert.AreEqual(250, result.totalCasualties);
        }

        // ── Spot-Check Tests ───────────────────────────────────────

        [Test]
        public void Test_ByzantineEmpireDataCorrect()
        {
            var byz = FactionDatabase.Get("byzantine");
            Assert.IsNotNull(byz, "Byzantine faction not found");
            Assert.AreEqual(Region.Europe, byz.region);
            Assert.IsTrue(byz.unitTypes.Any(u => u.displayName.Contains("Cataphract")),
                "Byzantine should have Cataphracts");
        }

        [Test]
        public void Test_SongEmpireDataCorrect()
        {
            var song = FactionDatabase.Get("song");
            Assert.IsNotNull(song, "Song faction not found");
            Assert.AreEqual(Region.EastAsia, song.region);
            Assert.IsTrue(song.unitTypes.Any(u =>
                u.displayName.Contains("Crossbow") || u.category == UnitCategory.Ranged),
                "Song should have crossbow/ranged units");
        }

        [Test]
        public void Test_NorthSeaEmpirePreservesExistingUnits()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(nse);

            string[] expected = { "nse_huscarl", "nse_berserker", "nse_hunter", "nse_shieldbearer", "nse_shipcrew" };
            foreach (string uid in expected)
                Assert.IsNotNull(nse.unitTypes.Find(u => u.id == uid),
                    $"North Sea Empire missing unit '{uid}'");
        }
    }
}
