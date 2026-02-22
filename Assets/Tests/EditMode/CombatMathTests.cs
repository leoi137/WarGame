using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace WorldWars.Tests.EditMode
{
    [TestFixture]
    public class CombatMathTests
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
        public void Test_BaseDamageIsAttackMinusArmor()
        {
            var attacker = MakeUnit(100, 15, 0, 3f);
            var target = MakeUnit(100, 10, 5, 3f);
            float before = target.currentHealth;

            target.TakeDamage(attacker.attackDamage);
            float actual = before - target.currentHealth;
            float expected = Mathf.Max(15f - 5f, 1f);

            Assert.AreEqual(expected, actual, 0.01f, "Damage should be ATK - armor, min 1");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_AbilityDamageModifierApplied()
        {
            var attacker = MakeUnit(100, 10, 0, 3f, "berserker_rage");
            attacker.activeEffects["berserker_rage"] = 6f;
            var target = MakeUnit(100, 10, 0, 3f);

            float modified = AbilitySystem.GetModifiedDamage(attacker, target, 10f);
            Assert.AreEqual(16f, modified, 0.01f, "Berserker rage should multiply damage by 1.6");

            Cleanup(attacker, target);
        }

        [Test]
        public void Test_ZeroArmorTakesFullDamage()
        {
            var target = MakeUnit(100, 10, 0, 3f);
            float before = target.currentHealth;

            target.TakeDamage(10f);
            float actual = before - target.currentHealth;

            Assert.AreEqual(10f, actual, 0.01f, "Zero armor target should take full damage");

            Cleanup(target);
        }

        [Test]
        public void Test_HighArmorCapsAtMinimum1()
        {
            var target = MakeUnit(100, 10, 20, 3f);
            float before = target.currentHealth;

            target.TakeDamage(10f);
            float actual = before - target.currentHealth;

            Assert.AreEqual(1f, actual, 0.01f, "Armor >= damage should still result in 1 damage minimum");

            Cleanup(target);
        }

        [Test]
        public void Test_PikeBraceBonusOnlyVsCavalry()
        {
            var pikeman = MakeUnit(100, 10, 0, 3f, "pike_brace");
            var cavalry = MakeUnit(100, 10, 0, 3f);
            cavalry.typeDefinition.category = UnitCategory.HeavyCavalry;
            var infantry = MakeUnit(100, 10, 0, 3f);
            infantry.typeDefinition.category = UnitCategory.HeavyInfantry;

            float vsCavalry = AbilitySystem.GetModifiedDamage(pikeman, cavalry, 10f);
            float vsInfantry = AbilitySystem.GetModifiedDamage(pikeman, infantry, 10f);

            Assert.Greater(vsCavalry, 10f, "Pike brace should boost damage vs cavalry");
            Assert.AreEqual(10f, vsInfantry, 0.01f, "Pike brace should not affect damage vs infantry");

            Cleanup(pikeman, cavalry, infantry);
        }
    }
}
