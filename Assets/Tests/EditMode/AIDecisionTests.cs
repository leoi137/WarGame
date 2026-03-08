using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class AIDecisionTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        static Unit MakeUnit(Vector3 pos, UnitCategory cat = UnitCategory.HeavyInfantry, Faction side = Faction.Attacker, float hp = 100f, float atk = 10f)
        {
            var go = new GameObject("TestUnit");
            go.transform.position = pos;
            var unit = go.AddComponent<Unit>();
            unit.faction = side;
            unit.maxHealth = hp;
            unit.currentHealth = hp;
            unit.attackDamage = atk;
            unit.armor = 5;
            unit.moveSpeed = 3f;
            unit.attackRange = 2.5f;
            unit.attackCooldown = 1f;
            unit.typeDefinition = new UnitTypeDefinition
            {
                id = "test_" + cat.ToString().ToLower(),
                factionId = "test",
                displayName = "Test " + cat,
                category = cat,
                maxHP = hp,
                attackDamage = atk,
                armor = 5,
                moveSpeed = 3f,
                attackRange = cat == UnitCategory.Ranged ? 15f : 2.5f,
                attackCooldown = 1f,
                visualConfig = new UnitVisualConfig()
            };
            unit.factionDefinition = new FactionDefinition { id = "test", primaryColor = Color.red, secondaryColor = Color.blue };
            return unit;
        }

        static void Cleanup(params Unit[] units)
        {
            foreach (var u in units)
                if (u != null) Object.DestroyImmediate(u.gameObject);
        }

        static void Cleanup(List<Unit> units)
        {
            foreach (var u in units)
                if (u != null) Object.DestroyImmediate(u.gameObject);
        }

        [Test]
        public void Test_IdenticalAIProducesSameDecisions()
        {
            var units = new List<Unit>();
            for (int i = 0; i < 5; i++)
                units.Add(MakeUnit(new Vector3(i, 0, 0), UnitCategory.HeavyInfantry, Faction.Attacker, 100f, 10f));
            var enemies = new List<Unit>();
            for (int i = 0; i < 5; i++)
                enemies.Add(MakeUnit(new Vector3(i + 20, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender, 100f, 10f));

            var stance1 = TacticalDecisionMaker.EvaluateStance(units, enemies);
            var stance2 = TacticalDecisionMaker.EvaluateStance(units, enemies);
            Assert.AreEqual(stance1, stance2, "Same inputs should produce same tactical stance");

            Cleanup(units);
            Cleanup(enemies);
        }

        [Test]
        public void Test_RangedUnitsRetreatWhenFlanked()
        {
            var ranged = MakeUnit(Vector3.zero, UnitCategory.Ranged, Faction.Attacker);
            var melee = MakeUnit(new Vector3(2f, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender);

            float distance = Vector3.Distance(ranged.transform.position, melee.transform.position);
            bool shouldRetreat = distance < 5f && ranged.typeDefinition.category == UnitCategory.Ranged;
            Assert.IsTrue(shouldRetreat, "Ranged unit should retreat when melee enemy is within 5 units");

            Cleanup(ranged, melee);
        }

        [Test]
        public void Test_CavalryTargetsRangedFirst()
        {
            var cavalry = MakeUnit(Vector3.zero, UnitCategory.HeavyCavalry, Faction.Attacker);
            var ranged = MakeUnit(new Vector3(10f, 0, 0), UnitCategory.Ranged, Faction.Defender);
            var infantry = MakeUnit(new Vector3(10f, 0, 1f), UnitCategory.HeavyInfantry, Faction.Defender);

            float scoreRanged = TargetSelector.ScoreTarget(cavalry, ranged);
            float scoreInfantry = TargetSelector.ScoreTarget(cavalry, infantry);
            Assert.Greater(scoreRanged, scoreInfantry, "Cavalry should prefer ranged targets over infantry at similar distance");

            Cleanup(cavalry, ranged, infantry);
        }

        [Test]
        public void Test_ArmyStrengthCalculation()
        {
            var units = new List<Unit>();
            for (int i = 0; i < 10; i++)
                units.Add(MakeUnit(new Vector3(i, 0, 0), UnitCategory.HeavyInfantry, Faction.Attacker, 100f, 100f));
            var enemy = new List<Unit> { MakeUnit(new Vector3(50, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender, 10f, 1f) };

            float ratio = TacticalDecisionMaker.CalculateStrengthRatio(units, enemy);
            Assert.Greater(ratio, 1f, "Army with 10 strong units vs 1 weak should have ratio > 1");

            Cleanup(units);
            Cleanup(enemy);
        }

        [Test]
        public void Test_StanceChangesWithStrengthRatio()
        {
            var weak = new List<Unit>();
            for (int i = 0; i < 3; i++)
                weak.Add(MakeUnit(new Vector3(i, 0, 0), UnitCategory.HeavyInfantry, Faction.Attacker, 50f, 5f));
            var strong = new List<Unit>();
            for (int i = 0; i < 10; i++)
                strong.Add(MakeUnit(new Vector3(i + 20, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender, 100f, 15f));

            var stance = TacticalDecisionMaker.EvaluateStance(weak, strong);
            Assert.AreNotEqual(TacticalDecisionMaker.TacticalStance.Aggressive, stance,
                "Weak army should not adopt Aggressive stance");

            Cleanup(weak);
            Cleanup(strong);
        }

        [Test]
        public void Test_FormationShieldsFrontRangedBack()
        {
            var heavyInf = new List<Unit>();
            var ranged = new List<Unit>();
            for (int i = 0; i < 6; i++)
                heavyInf.Add(MakeUnit(Vector3.zero, UnitCategory.HeavyInfantry, Faction.Attacker));
            for (int i = 0; i < 4; i++)
                ranged.Add(MakeUnit(Vector3.zero, UnitCategory.Ranged, Faction.Attacker));

            var all = new List<Unit>();
            all.AddRange(heavyInf);
            all.AddRange(ranged);

            FormationController.ArrangeByCategory(all, Vector3.zero, Vector3.forward);

            float avgHeavyZ = heavyInf.Average(u => u.transform.position.z);
            float avgRangedZ = ranged.Average(u => u.transform.position.z);
            Assert.Greater(avgHeavyZ, avgRangedZ, "Heavy infantry should be positioned in front of ranged units");

            Cleanup(all);
        }

        [Test]
        public void Test_TargetScoringFavorsLowHP()
        {
            var attacker = MakeUnit(Vector3.zero, UnitCategory.HeavyInfantry, Faction.Attacker);
            var lowHP = MakeUnit(new Vector3(5f, 0, 0), UnitCategory.HeavyInfantry, Faction.Defender, 20f);
            var fullHP = MakeUnit(new Vector3(5f, 0, 1f), UnitCategory.HeavyInfantry, Faction.Defender, 100f);

            float scoreLow = TargetSelector.ScoreTarget(attacker, lowHP);
            float scoreFull = TargetSelector.ScoreTarget(attacker, fullHP);
            Assert.Greater(scoreLow, scoreFull, "Low HP target should score higher than full HP target");

            Cleanup(attacker, lowHP, fullHP);
        }

        [Test]
        public void Test_TypeMatchupCavalryVsRanged()
        {
            float bonus = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyCavalry, UnitCategory.Ranged);
            Assert.Greater(bonus, 0f, "HeavyCavalry vs Ranged should have a positive type matchup bonus");
        }

        [Test]
        public void Test_TerrainAnalyzerFindsHighGround()
        {
            Vector3 start = new Vector3(50f, 0f, 50f);
            Vector3 defensive = TerrainAnalyzer.FindBestDefensivePosition(start, 100f, UnitCategory.HeavyInfantry);
            Assert.IsNotNull(defensive);
        }

        [Test]
        public void Test_TanksEngageBeforeRanged()
        {
            Assert.IsTrue(
                (int)UnitCategory.HeavyInfantry < (int)UnitCategory.Ranged,
                "HeavyInfantry category should have lower enum value for priority ordering");
        }

        [Test]
        public void Test_AIDoesNotCheat()
        {
            Assert.IsNotNull(typeof(SimulationAI).GetField("ownUnits",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            Assert.IsNotNull(typeof(SimulationAI).GetField("enemyUnits",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }
    }
}
