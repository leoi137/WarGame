using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class AbilityConsolidatedTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AbilityDatabase.Initialize();
            FactionDatabase.Initialize();
            UnitDatabase.Initialize();
        }

        static Unit MakeUnit(float hp = 100, float atk = 10, float armor = 0, float spd = 3f, string abilityId = null, UnitCategory category = UnitCategory.HeavyInfantry)
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
                category = category,
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

        static void Cleanup(params Unit[] units)
        {
            foreach (var u in units)
                if (u != null) Object.DestroyImmediate(u.gameObject);
        }

        [Test]
        public void Test_ParryReducesDamage()
        {
            var attacker = MakeUnit(100, 10, 0, 3f);
            var target = MakeUnit(100, 10, 0, 3f);
            target.activeEffects["parry"] = 1.5f;

            float damage = AbilitySystem.GetModifiedDamage(attacker, target, 10f);
            Assert.AreEqual(4f, damage, 0.01f, "Parry incomingDamageModifier 0.4 => 10 * 0.4 = 4");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_BerserkerRageBoostsDamage()
        {
            var attacker = MakeUnit(100, 10, 0, 3f, "berserker_rage");
            attacker.activeEffects["berserker_rage"] = 6f;
            var target = MakeUnit(100, 10, 0, 3f);

            float damage = AbilitySystem.GetModifiedDamage(attacker, target, 10f);
            Assert.AreEqual(16f, damage, 0.01f, "Berserker rage damageModifier 1.6 => 10 * 1.6 = 16");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_ShieldWallIncreasesArmor()
        {
            var unit = MakeUnit(100, 10, 5, 3f, "shield_wall");
            unit.activeEffects["shield_wall"] = float.MaxValue;

            float armor = AbilitySystem.GetModifiedArmor(unit);
            Assert.AreEqual(20f, armor, 0.01f, "Shield wall armorModifier +15 => 5 + 15 = 20");

            Cleanup(unit);
        }

        [Test]
        public void Test_MarkIncreasesIncomingDamage()
        {
            var attacker = MakeUnit(100, 10, 0, 3f);
            var target = MakeUnit(100, 10, 0, 3f);
            target.activeEffects["mark"] = 5f;

            float damage = AbilitySystem.GetModifiedDamage(attacker, target, 10f);
            Assert.AreEqual(14f, damage, 0.01f, "Mark incomingDamageModifier 1.4 => 10 * 1.4 = 14");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_DualStrikeProcChance()
        {
            var rng = new BattleRandom(42);
            int procs = 0;
            const int trials = 1000;
            const float procChance = 0.3f;

            for (int i = 0; i < trials; i++)
                if (rng.Chance(procChance)) procs++;

            float rate = (float)procs / trials;
            Assert.GreaterOrEqual(rate, 0.25f, "Dual strike proc rate should be >= 25% over 1000 trials");
            Assert.LessOrEqual(rate, 0.35f, "Dual strike proc rate should be <= 35% over 1000 trials");
        }

        [Test]
        public void Test_PikeBraceVsCavalry()
        {
            var pikeman = MakeUnit(100, 10, 0, 3f, "pike_brace");
            var heavyCav = MakeUnit(100, 10, 0, 3f);
            heavyCav.typeDefinition.category = UnitCategory.HeavyCavalry;
            var lightCav = MakeUnit(100, 10, 0, 3f);
            lightCav.typeDefinition.category = UnitCategory.LightCavalry;

            float vsHeavyCav = AbilitySystem.GetModifiedDamage(pikeman, heavyCav, 10f);
            float vsLightCav = AbilitySystem.GetModifiedDamage(pikeman, lightCav, 10f);

            Assert.AreEqual(20f, vsHeavyCav, 0.01f, "Pike brace doubles damage vs HeavyCavalry");
            Assert.AreEqual(20f, vsLightCav, 0.01f, "Pike brace doubles damage vs LightCavalry");

            Cleanup(pikeman, heavyCav, lightCav);
        }
    }
}
