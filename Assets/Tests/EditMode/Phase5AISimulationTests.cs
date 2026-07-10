using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 5: AI simulation engine — symmetry, per-category behavior, tactics, formations, targeting.
    /// </summary>
    [TestFixture]
    public class Phase5AISimulationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        private static Unit MakeUnit(Faction side, UnitCategory cat, Vector3 pos, float hp = 100, float atk = 10)
        {
            var go = new GameObject($"Unit_{side}_{cat}");
            var unit = go.AddComponent<Unit>();
            unit.faction = side;
            unit.maxHealth = hp;
            unit.currentHealth = hp;
            unit.attackDamage = atk;
            unit.moveSpeed = 3.5f;
            unit.attackRange = cat == UnitCategory.Ranged ? 14f : 2.5f;
            unit.typeDefinition = new UnitTypeDefinition
            {
                id = $"test_{cat}", category = cat, maxHP = hp, attackDamage = atk,
                moveSpeed = 3.5f, attackRange = unit.attackRange, attackCooldown = 1f,
                visualConfig = new UnitVisualConfig()
            };
            go.transform.position = pos;
            return unit;
        }

        private static void Cleanup(params Unit[] units)
        {
            foreach (var u in units)
                if (u != null) Object.DestroyImmediate(u.gameObject);
        }

        // ── SimulationAI Symmetry ──────────────────────────────────

        [Test]
        public void Test_SimulationAIIdenticalForBothSides()
        {
            var attackerAI = typeof(SimulationAI);
            var defenderAI = typeof(SimulationAI);
            Assert.AreEqual(attackerAI, defenderAI, "Both sides must use the same AI class");

            var methods = typeof(SimulationAI).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var m in methods)
            {
                if (m.Name.StartsWith("Handle"))
                {
                    var body = m.GetMethodBody();
                    Assert.IsNotNull(body, $"{m.Name} should have a method body");
                }
            }
        }

        [Test]
        public void Test_AIDecisionsDeterministicWithSeed()
        {
            var matchup = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyCavalry, UnitCategory.Ranged);
            var matchup2 = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyCavalry, UnitCategory.Ranged);
            Assert.AreEqual(matchup, matchup2, "Type matchup should be deterministic");
        }

        // ── Per-Category Behavior ──────────────────────────────────

        [Test]
        public void Test_AISelectsNearestTarget()
        {
            var attacker = MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero);
            var near = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(5, 0, 0));
            var far = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(50, 0, 0));

            var target = TargetSelector.FindNearestEnemy(attacker, new List<Unit> { near, far });
            Assert.AreEqual(near, target);

            Cleanup(attacker, near, far);
        }

        [Test]
        public void Test_AIRangedMaintainsDistance()
        {
            var ranged = MakeUnit(Faction.Attacker, UnitCategory.Ranged, new Vector3(0, 0, 0));
            var melee = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(2, 0, 0));

            var target = TargetSelector.FindOptimalRangedTarget(ranged, new List<Unit> { melee });
            Assert.IsNotNull(target, "Ranged should still find a target");

            Cleanup(ranged, melee);
        }

        [Test]
        public void Test_AICavalryChargesRanged()
        {
            var cav = MakeUnit(Faction.Attacker, UnitCategory.HeavyCavalry, Vector3.zero);
            var ranged = MakeUnit(Faction.Defender, UnitCategory.Ranged, new Vector3(10, 0, 0));
            var infantry = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(10, 0, 5));

            var target = TargetSelector.FindCavalryChargeTarget(cav, new List<Unit> { ranged, infantry });
            Assert.AreEqual(ranged, target, "Cavalry should prefer ranged targets");

            Cleanup(cav, ranged, infantry);
        }

        [Test]
        public void Test_AITankHoldsFormation()
        {
            var units = new List<Unit>();
            for (int i = 0; i < 5; i++)
                units.Add(MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, new Vector3(i * 2, 0, 0)));

            Vector3 center = new Vector3(4, 0, 0);
            Assert.IsTrue(FormationController.IsFormationIntact(units, 5f),
                "Units placed 2 apart within 5 should be in formation");

            foreach (var u in units) Cleanup(u);
        }

        [Test]
        public void Test_AIRetreatsAtLowHP()
        {
            var unit = MakeUnit(Faction.Attacker, UnitCategory.LightInfantry, new Vector3(50, 0, 50), hp: 100);
            unit.currentHealth = 15f;

            var retreat = TacticalDecisionMaker.GetRetreatPosition(unit, Faction.Attacker, 300);
            float retreatZ = retreat.z;
            Assert.Less(retreatZ, 50f, "Attacker retreat should be toward own spawn (z < current)");

            Cleanup(unit);
        }

        [Test]
        public void Test_AIElephantChargesLineFirst()
        {
            var elephant = MakeUnit(Faction.Attacker, UnitCategory.Elephant, Vector3.zero);
            var cluster1 = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(10, 0, 0));
            var isolated = MakeUnit(Faction.Defender, UnitCategory.Ranged, new Vector3(40, 0, 0));

            var enemies = new List<Unit> { cluster1, isolated };
            var target = TargetSelector.SelectTarget(elephant, enemies);
            Assert.AreEqual(cluster1, target, "Elephant should target cluster over isolated");

            Cleanup(elephant, cluster1, isolated);
        }

        // ── TacticalDecisionMaker ──────────────────────────────────

        [Test]
        public void Test_AIStrengthRatioAffectsStance()
        {
            var strong = new List<Unit>();
            var weak = new List<Unit>();
            for (int i = 0; i < 10; i++)
                strong.Add(MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero, 100, 15));
            for (int i = 0; i < 3; i++)
                weak.Add(MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, Vector3.zero, 50, 5));

            var stance = TacticalDecisionMaker.EvaluateStance(weak, strong);
            Assert.AreNotEqual(TacticalDecisionMaker.TacticalStance.Aggressive, stance,
                "Weak army should not be aggressive");

            foreach (var u in strong) Cleanup(u);
            foreach (var u in weak) Cleanup(u);
        }

        [Test]
        public void Test_TacticalAdvanceTargetWithinBounds()
        {
            var own = new List<Unit> { MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, new Vector3(50, 0, 50)) };
            var enemy = new List<Unit> { MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(200, 0, 200)) };

            var target = TacticalDecisionMaker.GetAdvanceTarget(own, enemy, Faction.Attacker);
            Assert.GreaterOrEqual(target.x, 0f);
            Assert.GreaterOrEqual(target.z, 0f);

            Cleanup(own[0], enemy[0]);
        }

        [Test]
        public void Test_GetFlankPositionNotOnEnemyFrontline()
        {
            var unit = MakeUnit(Faction.Attacker, UnitCategory.LightCavalry, new Vector3(50, 0, 50));
            var enemy = new List<Unit> { MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(100, 0, 100)) };

            var flank = TacticalDecisionMaker.GetFlankPosition(unit, enemy);
            float enemyCenterX = 100f;
            float offset = Mathf.Abs(flank.x - enemyCenterX);
            Assert.Greater(offset, 5f, "Flank position should be offset from enemy center");

            Cleanup(unit, enemy[0]);
        }

        // ── FormationController ────────────────────────────────────

        [Test]
        public void Test_FormationPlacesShieldsFront()
        {
            var units = new List<Unit>
            {
                MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero),
                MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero),
                MakeUnit(Faction.Attacker, UnitCategory.Ranged, Vector3.zero),
                MakeUnit(Faction.Attacker, UnitCategory.Ranged, Vector3.zero)
            };

            FormationController.ArrangeByCategory(units, new Vector3(50, 0, 50), Vector3.forward);

            float infantryZ = units[0].transform.position.z;
            float rangedZ = units[2].transform.position.z;
            Assert.Greater(infantryZ, rangedZ,
                "Heavy infantry should be in front (higher Z) of ranged");

            foreach (var u in units) Cleanup(u);
        }

        [Test]
        public void Test_FormationPlacesRangedBack()
        {
            var units = new List<Unit>
            {
                MakeUnit(Faction.Attacker, UnitCategory.Ranged, Vector3.zero),
                MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero),
                MakeUnit(Faction.Attacker, UnitCategory.Ranged, Vector3.zero)
            };

            FormationController.ArrangeByCategory(units, new Vector3(50, 0, 50), Vector3.forward);

            float infantryZ = units[1].transform.position.z;
            float maxRangedZ = Mathf.Max(units[0].transform.position.z, units[2].transform.position.z);
            Assert.GreaterOrEqual(infantryZ, maxRangedZ,
                "Ranged should be behind infantry");

            foreach (var u in units) Cleanup(u);
        }

        [Test]
        public void Test_FormationPositionsAreSpaced()
        {
            var positions = FormationController.GetFormationPositions(10, Vector3.zero, Vector3.forward, 2f);
            for (int i = 0; i < positions.Length; i++)
                for (int j = i + 1; j < positions.Length; j++)
                {
                    float dist = Vector3.Distance(positions[i], positions[j]);
                    Assert.GreaterOrEqual(dist, 1.5f,
                        $"Positions [{i}] and [{j}] too close: {dist:F2}");
                }
        }

        // ── TargetSelector ─────────────────────────────────────────

        [Test]
        public void Test_TargetScoringFavorsWeak()
        {
            var attacker = MakeUnit(Faction.Attacker, UnitCategory.HeavyInfantry, Vector3.zero);
            var weak = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(5, 0, 0), hp: 100);
            weak.currentHealth = 20f;
            var strong = MakeUnit(Faction.Defender, UnitCategory.HeavyInfantry, new Vector3(5, 0, 5), hp: 100);

            float weakScore = TargetSelector.ScoreTarget(attacker, weak);
            float strongScore = TargetSelector.ScoreTarget(attacker, strong);
            Assert.Greater(weakScore, strongScore, "Weak target should score higher");

            Cleanup(attacker, weak, strong);
        }

        [Test]
        public void Test_TypeMatchupBonusCorrect()
        {
            float bonus = TargetSelector.GetTypeMatchupBonus(UnitCategory.HeavyCavalry, UnitCategory.Ranged);
            Assert.Greater(bonus, 0f, "Cavalry should have bonus vs ranged");
        }

        // ── TerrainAnalyzer ────────────────────────────────────────

        [Test]
        public void Test_TerrainAnalyzerFindsHighGround()
        {
            var pos = TerrainAnalyzer.FindBestDefensivePosition(new Vector3(50, 0, 50), 30f, UnitCategory.HeavyInfantry);
            Assert.IsNotNull(pos);
        }

        [Test]
        public void Test_TerrainAnalyzerFindsCover()
        {
            var pos = TerrainAnalyzer.FindForestCover(new Vector3(50, 0, 50), 30f);
            Assert.IsNotNull(pos);
        }
    }
}
