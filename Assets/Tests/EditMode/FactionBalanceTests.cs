using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class FactionBalanceTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
        }

        [Test]
        public void Test_UnitBudgetRatioReflectsMilitaryScale()
        {
            var factions = FactionDatabase.GetAll();
            Assert.Greater(factions.Count, 0);

            int maxBudget = factions.Max(f => f.GetBattleUnitBudget());
            int minBudget = factions.Min(f => f.GetBattleUnitBudget());
            float ratio = (float)maxBudget / minBudget;

            Assert.GreaterOrEqual(ratio, 10f, "Largest/smallest budget ratio should be at least 10x");
            Assert.LessOrEqual(ratio, 20f, "Largest/smallest budget ratio should be at most 20x");
        }

        [Test]
        public void Test_AverageUnitHPBetween60And120()
        {
            var all = UnitDatabase.GetAll();
            Assert.Greater(all.Count, 0);

            float avg = (float)all.Sum(u => u.maxHP) / all.Count;
            Assert.GreaterOrEqual(avg, 60f, "Mean HP should be >= 60");
            Assert.LessOrEqual(avg, 120f, "Mean HP should be <= 120");
        }

        [Test]
        public void Test_AverageUnitATKBetween8And20()
        {
            var all = UnitDatabase.GetAll();
            Assert.Greater(all.Count, 0);

            float avg = (float)all.Sum(u => u.attackDamage) / all.Count;
            Assert.GreaterOrEqual(avg, 8f, "Mean ATK should be >= 8");
            Assert.LessOrEqual(avg, 20f, "Mean ATK should be <= 20");
        }

        [Test]
        public void Test_EachCategoryRepresentedByAtLeast3Factions()
        {
            var factions = FactionDatabase.GetAll();
            var commonCategories = new[] { UnitCategory.HeavyInfantry, UnitCategory.LightInfantry, UnitCategory.HeavyCavalry, UnitCategory.LightCavalry, UnitCategory.Ranged, UnitCategory.Siege };
            var rareCategories = new[] { UnitCategory.Elephant, UnitCategory.Naval, UnitCategory.Special };

            foreach (var cat in commonCategories)
            {
                int count = factions.Count(f => f.unitTypes != null && f.unitTypes.Any(u => u.category == cat));
                Assert.GreaterOrEqual(count, 3, $"Category {cat} should be in at least 3 factions");
            }

            foreach (var cat in rareCategories)
            {
                int count = factions.Count(f => f.unitTypes != null && f.unitTypes.Any(u => u.category == cat));
                Assert.GreaterOrEqual(count, 1, $"Category {cat} should be in at least 1 faction");
            }
        }

        [Test]
        public void Test_RangedUnitsHaveLowerHP()
        {
            var all = UnitDatabase.GetAll();
            var ranged = all.Where(u => u.category == UnitCategory.Ranged).ToList();
            var heavyInf = all.Where(u => u.category == UnitCategory.HeavyInfantry).ToList();

            Assert.Greater(ranged.Count, 0, "Need at least one Ranged unit");
            Assert.Greater(heavyInf.Count, 0, "Need at least one HeavyInfantry unit");

            float avgRanged = (float)ranged.Sum(u => u.maxHP) / ranged.Count;
            float avgHeavy = (float)heavyInf.Sum(u => u.maxHP) / heavyInf.Count;

            Assert.Less(avgRanged, avgHeavy, "Average Ranged HP should be less than average HeavyInfantry HP");
        }

        [Test]
        public void Test_HeavyInfantryHasHighestArmor()
        {
            var all = UnitDatabase.GetAll();
            var heavyInf = all.Where(u => u.category == UnitCategory.HeavyInfantry).ToList();
            var others = all.Where(u => u.category != UnitCategory.HeavyInfantry).ToList();

            Assert.Greater(heavyInf.Count, 0);
            Assert.Greater(others.Count, 0);

            float avgHeavy = (float)heavyInf.Sum(u => u.armor) / heavyInf.Count;
            float avgOthers = (float)others.Sum(u => u.armor) / others.Count;

            Assert.Greater(avgHeavy, avgOthers, "Average HeavyInfantry armor should exceed other categories");
        }

        [Test]
        public void Test_CavalryHasHighestSpeed()
        {
            var all = UnitDatabase.GetAll();
            var heavyCav = all.Where(u => u.category == UnitCategory.HeavyCavalry).ToList();
            var heavyInf = all.Where(u => u.category == UnitCategory.HeavyInfantry).ToList();

            Assert.Greater(heavyCav.Count, 0);
            Assert.Greater(heavyInf.Count, 0);

            float avgCav = (float)heavyCav.Sum(u => u.moveSpeed) / heavyCav.Count;
            float avgInf = (float)heavyInf.Sum(u => u.moveSpeed) / heavyInf.Count;

            Assert.Greater(avgCav, avgInf, "Average HeavyCavalry speed should exceed HeavyInfantry");
        }

        [Test]
        public void Test_NoUnitHasZeroDamage()
        {
            var all = UnitDatabase.GetAll();
            foreach (var u in all)
                Assert.Greater(u.attackDamage, 0f, $"Unit {u.id} has zero attack damage");
        }
    }
}
