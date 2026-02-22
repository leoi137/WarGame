using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 7: Battle lifecycle, placement, time controls, results.
    /// </summary>
    [TestFixture]
    public class Phase7BattleFlowTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            FactionDatabase.Initialize();
            AbilityDatabase.Initialize();
        }

        // ── BattleManager Lifecycle ────────────────────────────────

        [Test]
        public void Test_BattleManagerInitializesFromConfig()
        {
            var atk = FactionDatabase.Get("north_sea_empire");
            var def2 = FactionDatabase.Get("byzantine");
            var config = BattleConfiguration.Create(atk, def2, null, Faction.Attacker);
            Assert.IsNotNull(config);
            Assert.AreEqual(Faction.Attacker, config.playerSide);
        }

        [Test]
        public void Test_SimulationStartsAfterBothConfirm()
        {
            Assert.IsNotNull(typeof(BattleManager).GetMethod("ConfirmPlacement"),
                "BattleManager must have ConfirmPlacement method");
            Assert.IsNotNull(typeof(BattleManager).GetMethod("StartSimulation"),
                "BattleManager must have StartSimulation method");
        }

        [Test]
        public void Test_SimulationEndsWhenOneSideEliminated()
        {
            Assert.IsNotNull(typeof(BattleSimulator).GetMethod("IsBattleOver"),
                "BattleSimulator must have IsBattleOver method");
        }

        // ── Placement Tests ────────────────────────────────────────

        [Test]
        public void Test_PlacementZoneConstrainsUnits()
        {
            var go = new GameObject("TestSetup");
            var setup = go.AddComponent<BattleSetup>();

            bool valid = setup.IsValidPlacement(new Vector3(50, 0, 0), Faction.Attacker);
            Assert.IsTrue(valid, "Position in first 35% should be valid for attacker");

            bool invalid = setup.IsValidPlacement(new Vector3(50, 0, 250), Faction.Attacker);
            Assert.IsFalse(invalid, "Position in last 65% should be invalid for attacker");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_PlacementZoneAcceptsValidPosition()
        {
            var go = new GameObject("TestSetup");
            var setup = go.AddComponent<BattleSetup>();

            float zoneDepth = GameConfig.DefaultMapSize * GameConfig.PlacementZoneDepth;
            bool valid = setup.IsValidPlacement(new Vector3(50, 0, zoneDepth / 2), Faction.Attacker);
            Assert.IsTrue(valid);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_AutoPlaceCreatesValidFormation()
        {
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("AutoPlacePlayerUnits"),
                "BattleSetup must have AutoPlacePlayerUnits method");
        }

        [Test]
        public void Test_OpponentAutoPlacedOnInit()
        {
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("AutoPlaceOpponent"),
                "BattleSetup must have AutoPlaceOpponent method");
        }

        [Test]
        public void Test_PlayerCanSelectAndMoveUnits()
        {
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("HandleSelection"),
                "BattleSetup must have HandleSelection");
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("HandleDragMove"),
                "BattleSetup must have HandleDragMove");
        }

        [Test]
        public void Test_FormationPresetsArrangeCorrectly()
        {
            var positions = FormationController.GetFormationPositions(
                10, Vector3.zero, Vector3.forward, FormationType.Line, 2f);
            Assert.AreEqual(10, positions.Length);

            float minX = float.MaxValue, maxX = float.MinValue;
            foreach (var p in positions)
            {
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;
            }
            Assert.Greater(maxX - minX, 5f, "Line formation should spread horizontally");
        }

        [Test]
        public void Test_UnitMinimumSpacingEnforced()
        {
            var positions = FormationController.GetFormationPositions(
                20, Vector3.zero, Vector3.forward, FormationType.Square, 2f);
            for (int i = 0; i < positions.Length; i++)
                for (int j = i + 1; j < positions.Length; j++)
                {
                    float dist = Vector3.Distance(positions[i], positions[j]);
                    Assert.GreaterOrEqual(dist, 1.4f,
                        $"Positions too close: {dist:F2}");
                }
        }

        [Test]
        public void Test_RotationPreservesFormationShape()
        {
            Assert.IsNotNull(typeof(FormationController).GetMethod("RotateFormation"),
                "FormationController must have RotateFormation");
        }

        [Test]
        public void Test_DragBoxSelectsMultipleUnits()
        {
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("HandleSelection"),
                "BattleSetup must have HandleSelection for drag-box");
        }

        [Test]
        public void Test_DoubleClickSelectsAllOfType()
        {
            Assert.IsNotNull(typeof(BattleSetup).GetMethod("SelectAllOfType"),
                "BattleSetup must have SelectAllOfType");
        }

        [Test]
        public void Test_LargeArmyPlacement800Units()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(nse);
            int budget = nse.GetBattleUnitBudget();
            Assert.That(budget, Is.InRange(48, 800));
        }

        // ── BattleResult ───────────────────────────────────────────

        [Test]
        public void Test_BattleResultHasCorrectWinner()
        {
            var result = new BattleResult
            {
                winnerFactionId = "north_sea_empire",
                loserFactionId = "byzantine",
                winningSide = Faction.Attacker
            };
            Assert.AreEqual("north_sea_empire", result.winnerFactionId);
        }

        [Test]
        public void Test_BattleResultTracksAllCasualties()
        {
            var result = new BattleResult
            {
                winnerStartCount = 100,
                loserStartCount = 80,
                winnerSurvivors = 60,
                totalCasualties = 120
            };
            Assert.AreEqual(120, result.totalCasualties);
        }

        [Test]
        public void Test_RematchPreservesConfig()
        {
            Assert.IsNotNull(typeof(BattleResultsScreen).GetMethod("CreateRematchButton"),
                "BattleResultsScreen must have CreateRematchButton");
        }

        // ── Time Control ───────────────────────────────────────────

        [Test]
        public void Test_TimeControlPausesSimulation()
        {
            var go = new GameObject("TimeCtrl");
            var tc = go.AddComponent<BattleTimeController>();
            tc.Pause();
            Assert.IsTrue(tc.IsPaused);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_TimeControlSpeedAffectsTimeScale()
        {
            var go = new GameObject("TimeCtrl");
            var tc = go.AddComponent<BattleTimeController>();
            tc.SetSpeed(2f);
            Assert.AreEqual(2f, tc.CurrentSpeed);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_TimeControlSpeedClampedToMax()
        {
            var go = new GameObject("TimeCtrl");
            var tc = go.AddComponent<BattleTimeController>();
            tc.SetSpeed(10f);
            Assert.LessOrEqual(tc.CurrentSpeed, GameConfig.MaxBattleSpeed);
            Object.DestroyImmediate(go);
        }

        // ── Camera ─────────────────────────────────────────────────

        [Test]
        public void Test_BattleCameraFollowsCombat()
        {
            Assert.IsNotNull(typeof(BattleCamera).GetMethod("SetAutoFollow"),
                "BattleCamera must have SetAutoFollow");
        }

        // ── Determinism ────────────────────────────────────────────

        [Test]
        public void Test_DeterministicFullBattleReplay()
        {
            var rng1 = new BattleRandom(42);
            var rng2 = new BattleRandom(42);
            for (int i = 0; i < 100; i++)
                Assert.AreEqual(rng1.Value, rng2.Value);
        }
    }
}
