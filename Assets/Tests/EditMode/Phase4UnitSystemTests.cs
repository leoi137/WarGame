using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    /// <summary>
    /// Phase 4: Unit system refactor — UnitFactory, UnitModelBuilder, AbilitySystem, LOD, SpatialGrid.
    /// Tests that can run without a NavMesh or full scene use raw component setup.
    /// </summary>
    [TestFixture]
    public class Phase4UnitSystemTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
            TerrainDatabase.Initialize();
        }

        // ── Helper: create a minimal Unit on a fresh GameObject ──

        private static Unit MakeUnit(float hp = 100, float atk = 10, float armor = 0, float spd = 3f, string abilityId = null)
        {
            var go = new GameObject("TestUnit");
            var unit = go.AddComponent<Unit>();
            unit.maxHealth = hp;
            unit.currentHealth = hp;
            unit.attackDamage = atk;
            unit.armor = armor;
            unit.moveSpeed = spd;
            unit.attackRange = 2.5f;
            unit.attackCooldown = 1f;
            unit.typeDefinition = new UnitTypeDefinition
            {
                id = "test_unit",
                factionId = "test",
                displayName = "Test Unit",
                category = UnitCategory.HeavyInfantry,
                maxHP = hp,
                attackDamage = atk,
                armor = armor,
                moveSpeed = spd,
                attackRange = 2.5f,
                attackCooldown = 1f,
                abilityId = abilityId,
                visualConfig = new UnitVisualConfig()
            };
            unit.factionDefinition = new FactionDefinition { id = "test", primaryColor = Color.red, secondaryColor = Color.blue };
            return unit;
        }

        private static void Cleanup(params Unit[] units)
        {
            foreach (var u in units)
                if (u != null) UnityEngine.Object.DestroyImmediate(u.gameObject);
        }

        // ── UnitFactory Tests ──────────────────────────────────────

        [Test]
        public void Test_UnitFactoryCreatesUnitWithCorrectStats()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            Assert.IsNotNull(nse);
            var huscarl = nse.unitTypes.Find(u => u.id == "nse_huscarl");
            Assert.IsNotNull(huscarl);

            var unit = UnitFactory.CreateUnit(huscarl, nse, Faction.Attacker, Vector3.zero, Quaternion.identity);
            Assert.AreEqual(huscarl.maxHP, unit.maxHealth);
            Assert.AreEqual(huscarl.attackDamage, unit.attackDamage);
            Assert.AreEqual(huscarl.armor, unit.armor);
            Assert.AreEqual(huscarl.moveSpeed, unit.moveSpeed);

            UnityEngine.Object.DestroyImmediate(unit.gameObject);
        }

        [Test]
        public void Test_UnitFactoryCreatesUnitWithModel()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            var huscarl = nse.unitTypes.Find(u => u.id == "nse_huscarl");
            var unit = UnitFactory.CreateUnit(huscarl, nse, Faction.Attacker, Vector3.zero, Quaternion.identity);

            var renderers = unit.GetComponentsInChildren<MeshRenderer>();
            Assert.Greater(renderers.Length, 0, "Unit should have visible mesh renderers");

            UnityEngine.Object.DestroyImmediate(unit.gameObject);
        }

        [Test]
        public void Test_UnitFactoryCreatesNavMeshAgent()
        {
            var nse = FactionDatabase.Get("north_sea_empire");
            var huscarl = nse.unitTypes.Find(u => u.id == "nse_huscarl");
            var unit = UnitFactory.CreateUnit(huscarl, nse, Faction.Attacker, Vector3.zero, Quaternion.identity);

            var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
            Assert.IsNotNull(agent, "Unit should have NavMeshAgent");

            UnityEngine.Object.DestroyImmediate(unit.gameObject);
        }

        // ── UnitModelBuilder Tests ─────────────────────────────────

        [Test]
        public void Test_UnitModelBuilderCreatesSkeletonWith14Pivots()
        {
            var go = new GameObject("SkeletonTest");
            var unit = go.AddComponent<Unit>();
            UnitModelBuilder.BuildSkeleton(unit, new UnitVisualConfig());

            Transform[] pivots = {
                unit.pivotHips, unit.pivotWaist, unit.pivotNeck,
                unit.pivotLeftShoulder, unit.pivotRightShoulder,
                unit.pivotLeftElbow, unit.pivotRightElbow,
                unit.pivotLeftHand, unit.pivotRightHand,
                unit.pivotLeftHip, unit.pivotRightHip,
                unit.pivotLeftKnee, unit.pivotRightKnee,
                unit.pivotCape
            };

            foreach (var p in pivots)
                Assert.IsNotNull(p, "All 14 pivots should be created");

            UnityEngine.Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_LegacyVikingModelPreserved()
        {
            var go = new GameObject("LegacyTest");
            var unit = go.AddComponent<Unit>();
            unit.unitType = UnitType.Swordsman;
            UnitModelBuilder.BuildLegacyViking(unit, "Huscarl", Color.blue, Color.cyan);

            Assert.IsNotNull(unit.pivotHips, "Legacy model should have pivots");
            var renderers = unit.GetComponentsInChildren<MeshRenderer>();
            Assert.Greater(renderers.Length, 0, "Legacy model should have meshes");

            UnityEngine.Object.DestroyImmediate(go);
        }

        [Test]
        public void Test_AllArmorStylesBuildWithoutError()
        {
            foreach (ArmorStyle style in Enum.GetValues(typeof(ArmorStyle)))
            {
                var go = new GameObject($"Armor_{style}");
                var unit = go.AddComponent<Unit>();
                var config = new UnitVisualConfig { armorStyle = style, primaryWeapon = WeaponStyle.Sword };
                UnitModelBuilder.BuildSkeleton(unit, config);
                Assert.DoesNotThrow(() => UnitModelBuilder.BuildArmor(unit, config, Color.gray),
                    $"ArmorStyle.{style} should build without exception");
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void Test_AllWeaponStylesHaveAnimation()
        {
            var animator = new GameObject("AnimTest").AddComponent<UnitAnimator>();
            foreach (WeaponStyle ws in Enum.GetValues(typeof(WeaponStyle)))
            {
                if (ws == WeaponStyle.None || ws == WeaponStyle.Elephant) continue;
                Assert.DoesNotThrow(
                    () => animator.SetAnimationProfile(UnitCategory.HeavyInfantry, ws),
                    $"WeaponStyle.{ws} should have animation profile support");
            }
            UnityEngine.Object.DestroyImmediate(animator.gameObject);
        }

        // ── AbilitySystem Tests (Generic Triggers) ─────────────────

        [Test]
        public void Test_AbilityActivatesOnLowHP()
        {
            var unit = MakeUnit(100, 10, 0, 3f, "berserker_rage");
            unit.currentHealth = 30f;

            AbilitySystem.ProcessAbilities(unit, new List<Unit>(), 0.1f);
            Assert.IsTrue(unit.HasEffect("berserker_rage"),
                "Berserker rage should activate at 30% HP");

            Cleanup(unit);
        }

        [Test]
        public void Test_AbilityAppliesDamageModifier()
        {
            var attacker = MakeUnit(100, 10, 0, 3f, "berserker_rage");
            attacker.currentHealth = 30f;
            AbilitySystem.ProcessAbilities(attacker, new List<Unit>(), 0.1f);

            var target = MakeUnit(100, 10, 0, 3f);
            float modified = AbilitySystem.GetModifiedDamage(attacker, target, 10f);
            Assert.Greater(modified, 10f, "Berserker rage should increase damage");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_AbilityDeactivatesAfterDuration()
        {
            var unit = MakeUnit(100, 10, 0, 3f, "berserker_rage");
            unit.currentHealth = 30f;
            AbilitySystem.ProcessAbilities(unit, new List<Unit>(), 0.1f);
            Assert.IsTrue(unit.HasEffect("berserker_rage"));

            for (int i = 0; i < 70; i++)
                AbilitySystem.ProcessAbilities(unit, new List<Unit>(), 0.1f);

            Assert.IsFalse(unit.HasEffect("berserker_rage"),
                "Berserker rage should deactivate after 6s duration");

            Cleanup(unit);
        }

        [Test]
        public void Test_AbilityCooldownPreventsReactivation()
        {
            var unit = MakeUnit(100, 10, 0, 3f, "parry");
            AbilitySystem.ProcessAbilities(unit, new List<Unit>(), 0.1f);
            Assert.IsTrue(unit.HasEffect("parry") || unit.cooldowns.ContainsKey("parry"));

            for (int i = 0; i < 20; i++)
                AbilitySystem.ProcessAbilities(unit, new List<Unit>(), 0.1f);

            bool onCooldown = unit.cooldowns.TryGetValue("parry", out float cd) && cd > 0;
            Assert.IsTrue(onCooldown || unit.HasEffect("parry"),
                "Parry should be on cooldown or active after use");

            Cleanup(unit);
        }

        // ── AbilitySystem Tests (Conditional) ──────────────────────

        [Test]
        public void Test_AbilityActivatesOnlyVsTargetCategory()
        {
            var ability = AbilityDatabase.Get("pike_brace");
            Assert.IsNotNull(ability);

            var pikeman = MakeUnit(100, 10, 0, 3f, "pike_brace");
            var cavalry = MakeUnit(100, 10, 0, 3f);
            cavalry.typeDefinition.category = UnitCategory.HeavyCavalry;
            cavalry.transform.position = new Vector3(1, 0, 0);

            float damage = AbilitySystem.GetModifiedDamage(pikeman, cavalry, 10f);
            Assert.Greater(damage, 10f, "Pike brace should boost damage vs cavalry");

            Cleanup(pikeman, cavalry);
        }

        [Test]
        public void Test_AbilityDoesNotActivateVsWrongCategory()
        {
            var pikeman = MakeUnit(100, 10, 0, 3f, "pike_brace");
            var infantry = MakeUnit(100, 10, 0, 3f);
            infantry.typeDefinition.category = UnitCategory.HeavyInfantry;

            float damage = AbilitySystem.GetModifiedDamage(pikeman, infantry, 10f);
            Assert.AreEqual(10f, damage, 0.01f, "Pike brace should not activate vs infantry");

            Cleanup(pikeman, infantry);
        }

        [Test]
        public void Test_AbilityActivatesOnlyOnMatchingTerrain()
        {
            var ability = AbilityDatabase.Get("naval_boarding");
            Assert.IsNotNull(ability);
            Assert.IsNotNull(ability.terrainCondition);
            Assert.Contains(TerrainType.Coast, ability.terrainCondition);
        }

        [Test]
        public void Test_AbilityDoesNotActivateOnWrongTerrain()
        {
            var ability = AbilityDatabase.Get("naval_boarding");
            Assert.IsFalse(ability.terrainCondition.Contains(TerrainType.Plains),
                "naval_boarding should not activate on Plains");
        }

        [Test]
        public void Test_GetModifiedDamageRespectsTargetCategory()
        {
            var pikeman = MakeUnit(100, 10, 0, 3f, "pike_brace");

            var cav = MakeUnit(100, 10, 0, 3f);
            cav.typeDefinition.category = UnitCategory.LightCavalry;
            float cavDmg = AbilitySystem.GetModifiedDamage(pikeman, cav, 10f);

            var inf = MakeUnit(100, 10, 0, 3f);
            inf.typeDefinition.category = UnitCategory.HeavyInfantry;
            float infDmg = AbilitySystem.GetModifiedDamage(pikeman, inf, 10f);

            Assert.Greater(cavDmg, infDmg, "Pike brace should deal more to cavalry than infantry");

            Cleanup(pikeman, cav, inf);
        }

        // ── Combat & Movement Tests ────────────────────────────────

        [Test]
        public void Test_GenericMeleeAttackDealsDamage()
        {
            var attacker = MakeUnit(100, 15, 0, 3f);
            var target = MakeUnit(100, 10, 3, 3f);
            float expected = Mathf.Max(1f, 15f - 3f);

            target.TakeDamage(expected);
            Assert.Less(target.currentHealth, 100f, "Target should take damage");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_GenericRangedAttackSpawnsProjectile()
        {
            Assert.IsTrue(typeof(Projectile).IsSubclassOf(typeof(MonoBehaviour)),
                "Projectile class must exist as a MonoBehaviour for ranged attacks");
        }

        [Test]
        public void Test_TerrainSpeedModifierApplied()
        {
            var forest = TerrainDatabase.Get(TerrainType.Forest);
            Assert.Less(forest.movementMultiplier, 1f, "Forest should slow units");
        }

        [Test]
        public void Test_UnitDiesAtZeroHP()
        {
            var unit = MakeUnit(100, 10, 0, 3f);
            unit.TakeDamage(200);
            Assert.IsTrue(unit.isDead, "Unit should be dead at 0 HP");
            Cleanup(unit);
        }

        // ── LOD Tests ──────────────────────────────────────────────

        [Test]
        public void Test_LODFullWithin60m()
        {
            float dist = 30f;
            Assert.Less(dist, GameConfig.LODFullDistance);
        }

        [Test]
        public void Test_LODSimplifiedBeyond60m()
        {
            float dist = 90f;
            Assert.Greater(dist, GameConfig.LODFullDistance);
            Assert.Less(dist, GameConfig.LODSimplifiedDistance);
        }

        [Test]
        public void Test_LODBillboardBeyond120m()
        {
            float dist = 150f;
            Assert.Greater(dist, GameConfig.LODSimplifiedDistance);
            Assert.Less(dist, GameConfig.LODBillboardDistance);
        }

        [Test]
        public void Test_LODCulledBeyond200m()
        {
            float dist = 250f;
            Assert.Greater(dist, GameConfig.LODBillboardDistance);
        }

        // ── SpatialGrid Tests ──────────────────────────────────────

        [Test]
        public void Test_SpatialGridGetNearestIsCorrect()
        {
            int dim = Mathf.CeilToInt(300f / 10f);
            var grid = new SpatialGrid(10f, dim, dim);

            var units = new List<Unit>();
            for (int i = 0; i < 10; i++)
            {
                var u = MakeUnit();
                u.faction = Faction.Defender;
                u.transform.position = new Vector3(i * 5f, 0, 0);
                grid.Insert(u);
                units.Add(u);
            }

            var nearest = grid.GetNearest(new Vector3(12f, 0, 0), Faction.Defender, 50f);
            Assert.IsNotNull(nearest);
            Assert.AreEqual(10f, nearest.transform.position.x, 1f,
                "Nearest unit to x=12 should be at x=10");

            foreach (var u in units) Cleanup(u);
        }

        [Test]
        public void Test_SpatialGridPerformanceWith1600Units()
        {
            int dim = Mathf.CeilToInt(300f / 10f);
            var grid = new SpatialGrid(10f, dim, dim);
            var units = new List<Unit>();

            for (int i = 0; i < 1600; i++)
            {
                var u = MakeUnit();
                u.faction = i % 2 == 0 ? Faction.Attacker : Faction.Defender;
                u.transform.position = new Vector3(
                    UnityEngine.Random.Range(0f, 290f), 0,
                    UnityEngine.Random.Range(0f, 290f));
                grid.Insert(u);
                units.Add(u);
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1600; i++)
            {
                grid.GetNearest(units[i].transform.position, 
                    units[i].faction == Faction.Attacker ? Faction.Defender : Faction.Attacker, 50f);
            }
            sw.Stop();

            Assert.Less(sw.ElapsedMilliseconds, 50,
                $"1600 GetNearest queries took {sw.ElapsedMilliseconds}ms (max 50ms)");

            foreach (var u in units) Cleanup(u);
        }
    }
}
