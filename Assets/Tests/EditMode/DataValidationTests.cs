using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class DataValidationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            AbilityDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        [Test]
        public void Test_AllFactionsHaveValidId()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(f.id), $"Faction has null or empty id: {f.displayName}");
            }
        }

        [Test]
        public void Test_AllFactionsHave4To8UnitTypes()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                Assert.IsNotNull(f.unitTypes, $"Faction {f.id} has null unitTypes");
                Assert.GreaterOrEqual(f.unitTypes.Count, 4, $"Faction {f.id} has fewer than 4 unit types: {f.unitTypes.Count}");
                Assert.LessOrEqual(f.unitTypes.Count, 8, $"Faction {f.id} has more than 8 unit types: {f.unitTypes.Count}");
            }
        }

        [Test]
        public void Test_AllFactionsHaveCities()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                Assert.IsNotNull(f.cities, $"Faction {f.id} has null cities");
                Assert.GreaterOrEqual(f.cities.Count, 3, $"Faction {f.id} has fewer than 3 cities: {f.cities.Count}");
            }
        }

        [Test]
        public void Test_AllFactionsHaveCapital()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                Assert.IsNotNull(f.cities, $"Faction {f.id} has null cities");
                var capitalCount = f.cities.Count(c => c != null && c.isCapital);
                Assert.AreEqual(1, capitalCount, $"Faction {f.id} must have exactly one capital, found {capitalCount}");
            }
        }

        [Test]
        public void Test_AllCitiesHavePositiveGarrison()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                if (f.cities == null) continue;
                foreach (var c in f.cities)
                {
                    if (c == null) continue;
                    Assert.Greater(c.garrison, 0, $"City {c.id} in faction {f.id} has garrison <= 0: {c.garrison}");
                }
            }
        }

        [Test]
        public void Test_AllCityPositionsInRange()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                if (f.cities == null) continue;
                foreach (var c in f.cities)
                {
                    if (c == null) continue;
                    Assert.GreaterOrEqual(c.normalizedPosition.x, 0f, $"City {c.id} x < 0");
                    Assert.LessOrEqual(c.normalizedPosition.x, 1f, $"City {c.id} x > 1");
                    Assert.GreaterOrEqual(c.normalizedPosition.y, 0f, $"City {c.id} y < 0");
                    Assert.LessOrEqual(c.normalizedPosition.y, 1f, $"City {c.id} y > 1");
                }
            }
        }

        [Test]
        public void Test_NoDuplicateFactionIds()
        {
            var factions = FactionDatabase.GetAll();
            var ids = factions.Select(f => f.id).ToList();
            var distinctCount = ids.Distinct().Count();
            Assert.AreEqual(ids.Count, distinctCount, "Duplicate faction IDs found");
        }

        [Test]
        public void Test_NoDuplicateUnitTypeIds()
        {
            var units = UnitDatabase.GetAll();
            var ids = units.Select(u => u.id).ToList();
            var distinctCount = ids.Distinct().Count();
            Assert.AreEqual(ids.Count, distinctCount, "Duplicate unit type IDs found across factions");
        }

        [Test]
        public void Test_AllUnitTypesHavePositiveStats()
        {
            var units = UnitDatabase.GetAll();
            foreach (var u in units)
            {
                Assert.Greater(u.maxHP, 0, $"Unit {u.id} has HP <= 0");
                Assert.Greater(u.attackDamage, 0, $"Unit {u.id} has attack damage <= 0");
                Assert.GreaterOrEqual(u.armor, 0, $"Unit {u.id} has armor < 0");
                Assert.Greater(u.moveSpeed, 0, $"Unit {u.id} has moveSpeed <= 0");
            }
        }

        [Test]
        public void Test_AllUnitTypesHaveValidCategory()
        {
            var units = UnitDatabase.GetAll();
            foreach (var u in units)
            {
                Assert.IsTrue(System.Enum.IsDefined(typeof(UnitCategory), u.category), $"Unit {u.id} has invalid category: {u.category}");
            }
        }

        [Test]
        public void Test_AllTerrainDistributionsSumNear100()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                if (f.terrainDistribution == null || f.terrainDistribution.Count == 0) continue;
                var sum = f.terrainDistribution.Values.Sum();
                var normalized = sum < 10f ? sum * 100f : sum;
                Assert.GreaterOrEqual(normalized, 95f, $"Faction {f.id} terrain distribution sum {sum} < 95%");
                Assert.LessOrEqual(normalized, 105f, $"Faction {f.id} terrain distribution sum {sum} > 105%");
            }
        }

        [Test]
        public void Test_FactionColorUniqueness()
        {
            var factions = FactionDatabase.GetAll();
            for (int i = 0; i < factions.Count; i++)
            {
                for (int j = i + 1; j < factions.Count; j++)
                {
                    var a = factions[i].primaryColor;
                    var b = factions[j].primaryColor;
                    var dx = a.r - b.r;
                    var dy = a.g - b.g;
                    var dz = a.b - b.b;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
                    Assert.GreaterOrEqual(dist, 0.05f, $"Factions {factions[i].id} and {factions[j].id} have primaryColor within 0.05 distance: {dist}");
                }
            }
        }

        [Test]
        public void Test_AllAbilitiesHaveValidId()
        {
            var abilities = AbilityDatabase.GetAll();
            foreach (var a in abilities)
            {
                Assert.IsFalse(string.IsNullOrEmpty(a.id), $"Ability has null or empty id: {a.displayName}");
            }
        }

        [Test]
        public void Test_AllUnitAbilityReferencesExist()
        {
            var units = UnitDatabase.GetAll();
            foreach (var u in units)
            {
                if (string.IsNullOrEmpty(u.abilityId)) continue;
                var ability = AbilityDatabase.Get(u.abilityId);
                Assert.IsNotNull(ability, $"Unit {u.id} references non-existent ability: {u.abilityId}");
            }
        }

        [Test]
        public void Test_TerrainDatabaseHas10Types()
        {
            var terrains = TerrainDatabase.GetAll();
            Assert.AreEqual(10, terrains.Count, $"TerrainDatabase should have exactly 10 types, found {terrains.Count}");
        }

        [Test]
        public void Test_FactionDatabaseHas43Factions()
        {
            Assert.AreEqual(43, FactionDatabase.FactionCount, $"FactionDatabase should have exactly 43 factions, found {FactionDatabase.FactionCount}");
        }

        [Test]
        public void Test_AllRegionsHaveFactions()
        {
            foreach (Region r in System.Enum.GetValues(typeof(Region)))
            {
                var factions = FactionDatabase.GetByRegion(r);
                Assert.GreaterOrEqual(factions.Count, 1, $"Region {r} has no factions");
            }
        }

        [Test]
        public void Test_CapitalCityExistsInCityList()
        {
            var factions = FactionDatabase.GetAll();
            foreach (var f in factions)
            {
                if (string.IsNullOrEmpty(f.capitalCityId) || f.cities == null) continue;
                var found = f.cities.Any(c => c != null && c.id == f.capitalCityId);
                Assert.IsTrue(found, $"Faction {f.id} capitalCityId {f.capitalCityId} not found in cities list");
            }
        }

        [Test]
        public void Test_ByzantineEmpireDataCorrect()
        {
            var f = FactionDatabase.Get("byzantine");
            Assert.IsNotNull(f, "Byzantine Empire not found");
            Assert.IsTrue(f.unitTypes.Any(u => u != null && u.id != null && u.id.Contains("cataphract")), "Byzantine Empire should have cataphract unit");
            Assert.IsTrue(f.capitalCityId != null && f.capitalCityId.ToLowerInvariant().Contains("constantinople"), "Byzantine capital should be constantinople");
            Assert.AreEqual(Region.Europe, f.region);
        }

        [Test]
        public void Test_SongEmpireDataCorrect()
        {
            var f = FactionDatabase.Get("song");
            Assert.IsNotNull(f, "Song Empire not found");
            Assert.IsTrue(f.unitTypes.Any(u => u != null && u.id != null && u.id.Contains("crossbow")), "Song Empire should have crossbow unit");
            Assert.IsTrue(f.capitalCityId != null && f.capitalCityId.ToLowerInvariant().Contains("kaifeng"), "Song capital should be kaifeng");
            Assert.AreEqual(Region.EastAsia, f.region);
        }

        [Test]
        public void Test_NorthSeaEmpirePreservesExistingUnits()
        {
            var f = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(f, "North Sea Empire not found");
            var unitIds = f.unitTypes.Where(u => u != null).Select(u => u.id.ToLowerInvariant()).ToList();
            Assert.IsTrue(unitIds.Any(id => id.Contains("huscarl")), "North Sea Empire should have huscarl");
            Assert.IsTrue(unitIds.Any(id => id.Contains("berserker")), "North Sea Empire should have berserker");
            Assert.IsTrue(unitIds.Any(id => id.Contains("hunter")), "North Sea Empire should have hunter");
            Assert.IsTrue(unitIds.Any(id => id.Contains("shieldbearer")), "North Sea Empire should have shieldbearer");
            Assert.IsTrue(unitIds.Any(id => id.Contains("shipcrew") || id.Contains("ship_crew")), "North Sea Empire should have ship_crew");
        }
    }
}
