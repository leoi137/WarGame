using System;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Behavioral tests for Phase 1 core systems: EventBus, GameConfig, Enums, BattleRandom.
    /// GameManager requires a MonoBehaviour lifecycle and is tested in PlayMode.
    /// </summary>
    public class Phase1CoreFoundationTests
    {
        [SetUp]
        public void SetUp()
        {
            EventBus.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus.Clear();
        }

        // ── EventBus ──────────────────────────────────────────────

        [Test]
        public void Test_EventBusSubscribeAndPublish()
        {
            GameStateChangedEvent received = default;
            EventBus.Subscribe<GameStateChangedEvent>(e => received = e);

            var published = new GameStateChangedEvent
            {
                oldState = GameFlowState.MainMenu,
                newState = GameFlowState.WorldMap
            };
            EventBus.Publish(published);

            Assert.AreEqual(GameFlowState.MainMenu, received.oldState);
            Assert.AreEqual(GameFlowState.WorldMap, received.newState);
        }

        [Test]
        public void Test_EventBusUnsubscribe()
        {
            int callCount = 0;
            Action<GameStateChangedEvent> handler = _ => callCount++;

            EventBus.Subscribe(handler);
            EventBus.Publish(new GameStateChangedEvent());
            Assert.AreEqual(1, callCount);

            EventBus.Unsubscribe(handler);
            EventBus.Publish(new GameStateChangedEvent());
            Assert.AreEqual(1, callCount, "Handler should not fire after unsubscribe");
        }

        [Test]
        public void Test_EventBusMultipleSubscribers()
        {
            int totalCalls = 0;
            EventBus.Subscribe<GameStateChangedEvent>(_ => totalCalls++);
            EventBus.Subscribe<GameStateChangedEvent>(_ => totalCalls++);
            EventBus.Subscribe<GameStateChangedEvent>(_ => totalCalls++);

            EventBus.Publish(new GameStateChangedEvent());
            Assert.AreEqual(3, totalCalls);
        }

        [Test]
        public void Test_EventBusClear()
        {
            int callCount = 0;
            EventBus.Subscribe<GameStateChangedEvent>(_ => callCount++);
            EventBus.Clear();

            EventBus.Publish(new GameStateChangedEvent());
            Assert.AreEqual(0, callCount, "No handlers should remain after Clear");
        }

        [Test]
        public void Test_EventBusNoSubscribersNoError()
        {
            Assert.DoesNotThrow(() => EventBus.Publish(new GameStateChangedEvent()));
        }

        // ── GameConfig ────────────────────────────────────────────

        [Test]
        public void Test_GameConfigDefaultValues()
        {
            Assert.Greater(GameConfig.DefaultMapSize, 0);
            Assert.Greater(GameConfig.SimulationTickRate, 0f);
            Assert.Greater(GameConfig.MaxUnitsPerSide, 0);
            Assert.Greater(GameConfig.BaseDetectionRange, 0f);
            Assert.Greater(GameConfig.UnitBudgetScaleFactor, 0);
            Assert.Greater(GameConfig.PlacementZoneDepth, 0f);
            Assert.Greater(GameConfig.CountdownDuration, 0f);
            Assert.Greater(GameConfig.TerrainHeightScale, 0f);
            Assert.Greater(GameConfig.WorldMapWidth, 0f);
            Assert.Greater(GameConfig.WorldMapHeight, 0f);
            Assert.Greater(GameConfig.FlockingThreshold, 0);
            Assert.Greater(GameConfig.MaxFullLODUnits, 0);
            Assert.Greater(GameConfig.LODFullDistance, 0f);
            Assert.Greater(GameConfig.LODSimplifiedDistance, 0f);
            Assert.Greater(GameConfig.LODBillboardDistance, 0f);
            Assert.Greater(GameConfig.SpatialGridCellSize, 0f);
        }

        [Test]
        public void Test_GameConfigMaxBattleSpeedAboveDefault()
        {
            Assert.GreaterOrEqual(GameConfig.MaxBattleSpeed, GameConfig.DefaultBattleSpeed);
        }

        // ── Enums ─────────────────────────────────────────────────

        [Test]
        public void Test_EnumsHaveExpectedValues()
        {
            Assert.AreEqual(10, Enum.GetValues(typeof(TerrainType)).Length);
            Assert.AreEqual(9, Enum.GetValues(typeof(UnitCategory)).Length);
        }

        [Test]
        public void Test_GameFlowStateHas12Values()
        {
            Assert.AreEqual(12, Enum.GetValues(typeof(GameFlowState)).Length);
        }

        // ── BattleRandom ──────────────────────────────────────────

        [Test]
        public void Test_BattleRandomDeterministic()
        {
            var a = new BattleRandom(42);
            var b = new BattleRandom(42);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(a.Value, b.Value, $"Mismatch at iteration {i}");
            }
        }

        [Test]
        public void Test_BattleRandomRange()
        {
            var rng = new BattleRandom(123);
            for (int i = 0; i < 1000; i++)
            {
                float v = rng.Range(5f, 10f);
                Assert.GreaterOrEqual(v, 5f);
                Assert.Less(v, 10f);
            }
        }

        [Test]
        public void Test_BattleRandomChance()
        {
            var rng = new BattleRandom(99);

            for (int i = 0; i < 100; i++)
                Assert.IsFalse(rng.Chance(0f));

            for (int i = 0; i < 100; i++)
                Assert.IsTrue(rng.Chance(1f));
        }

        // ── GameManager (structure-only, lifecycle tested in PlayMode) ──

        [Test]
        public void Test_GameManagerSingletonFieldExists()
        {
            var prop = typeof(GameManager).GetProperty("Instance",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Assert.IsNotNull(prop, "GameManager must expose a static Instance property");
        }

        [Test]
        public void Test_GameManagerInitialStateIsMainMenu()
        {
            var go = new GameObject("TestGM");
            var gm = go.AddComponent<GameManager>();

            Assert.AreEqual(GameFlowState.MainMenu, gm.CurrentState);

            UnityEngine.Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_GameManagerTransitionToPublishesEvent()
        {
            var go = new GameObject("TestGM");
            var gm = go.AddComponent<GameManager>();

            GameStateChangedEvent received = default;
            bool fired = false;
            EventBus.Subscribe<GameStateChangedEvent>(e =>
            {
                received = e;
                fired = true;
            });

            gm.TransitionTo(GameFlowState.WorldMap);

            Assert.IsTrue(fired, "GameStateChangedEvent should fire on transition");
            Assert.AreEqual(GameFlowState.MainMenu, received.oldState);
            Assert.AreEqual(GameFlowState.WorldMap, received.newState);

            UnityEngine.Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_GameManagerStartQuickBattle()
        {
            var go = new GameObject("TestGM");
            var gm = go.AddComponent<GameManager>();

            var attacker = new FactionDefinition { id = "test_a", displayName = "A" };
            var defender = new FactionDefinition { id = "test_b", displayName = "B" };

            gm.StartQuickBattle(attacker, defender);

            Assert.AreEqual(GameFlowState.BattleSetup, gm.CurrentState);
            Assert.AreEqual(attacker, gm.SelectedAttacker);
            Assert.AreEqual(defender, gm.SelectedDefender);
            Assert.IsNotNull(gm.CurrentBattle);

            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
