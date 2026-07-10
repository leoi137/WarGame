using System.Collections.Generic;
using UnityEngine;

public static class AbilitySystem
{
    public static void ProcessAbilities(Unit unit, List<Unit> nearbyEnemies, float deltaTime)
    {
        if (unit.typeDefinition == null || string.IsNullOrEmpty(unit.typeDefinition.abilityId))
            return;

        AbilityDefinition ability = AbilityDatabase.Get(unit.typeDefinition.abilityId);
        if (ability == null) return;

        TickCooldowns(unit, deltaTime);
        TickActiveDurations(unit, deltaTime);

        if (ability.trigger == AbilityTrigger.Passive)
        {
            if (!unit.activeEffects.ContainsKey(ability.id))
                ActivateAbility(unit, ability);
            return;
        }

        if (ability.trigger == AbilityTrigger.OnAttack || ability.trigger == AbilityTrigger.OnKill)
            return;

        if (ability.trigger == AbilityTrigger.OnNearEnemy && ability.duration <= 0f)
        {
            if (ShouldActivate(ability, unit, nearbyEnemies))
            {
                if (!unit.activeEffects.ContainsKey(ability.id))
                    ActivateAbility(unit, ability);
            }
            else if (unit.activeEffects.ContainsKey(ability.id))
            {
                DeactivateAbility(unit, ability);
            }
            return;
        }

        if (ShouldActivate(ability, unit, nearbyEnemies) && !unit.activeEffects.ContainsKey(ability.id))
        {
            if (ability.cooldown <= 0f || GetCooldownRemaining(unit, ability.id) <= 0f)
            {
                ActivateAbility(unit, ability);
                if (ability.cooldown > 0f)
                    SetCooldown(unit, ability.id, ability.cooldown);
            }
        }
    }

    public static bool ShouldActivate(AbilityDefinition ability, Unit unit, List<Unit> nearbyEnemies)
    {
        if (ability == null) return false;
        if (!PassesTerrainCondition(ability, unit)) return false;
        if (!PassesSpecialCondition(ability, unit, nearbyEnemies)) return false;

        switch (ability.trigger)
        {
            case AbilityTrigger.OnLowHP:
                if (unit.maxHealth <= 0f) return false;
                return (unit.currentHealth / unit.maxHealth) <= ability.triggerThreshold;
            case AbilityTrigger.OnNearEnemy:
                if (!IsAnyEnemyWithinRange(unit, nearbyEnemies, ability.triggerThreshold)) return false;
                return ability.targetCategoryCondition == null || ability.targetCategoryCondition.Length == 0
                    || IsAnyEnemyMatchesCategory(nearbyEnemies, unit, ability.triggerThreshold, ability.targetCategoryCondition);
            case AbilityTrigger.OnCooldown:
                return GetCooldownRemaining(unit, ability.id) <= 0f;
            case AbilityTrigger.Passive:
                return true;
            case AbilityTrigger.OnAttack:
            case AbilityTrigger.OnKill:
                return false;
            default:
                return false;
        }
    }

    public static void ActivateAbility(Unit unit, AbilityDefinition ability)
    {
        if (ability == null) return;
        unit.activeEffects[ability.id] = ability.duration > 0f ? ability.duration : float.MaxValue;

        var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.speed = GetModifiedSpeed(unit);
    }

    public static void DeactivateAbility(Unit unit, AbilityDefinition ability)
    {
        if (ability == null) return;
        unit.activeEffects.Remove(ability.id);

        var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.speed = GetModifiedSpeed(unit);
    }

    public static float GetModifiedDamage(Unit attacker, Unit target, float baseDamage)
    {
        float damageMod = 1f;
        if (attacker != null && attacker.activeEffects != null)
        {
            foreach (string id in attacker.activeEffects.Keys)
            {
                var def = AbilityDatabase.Get(id);
                if (def != null && PassesTargetCategory(def, target, attacker))
                    damageMod *= def.damageModifier;
            }
        }

        float incomingMod = 1f;
        if (target != null && target.activeEffects != null)
        {
            foreach (string id in target.activeEffects.Keys)
            {
                var def = AbilityDatabase.Get(id);
                if (def != null)
                    incomingMod *= def.incomingDamageModifier;
            }
        }

        return baseDamage * damageMod * incomingMod;
    }

    public static float GetModifiedSpeed(Unit unit)
    {
        if (unit == null) return 0f;
        float mod = 1f;
        if (unit.activeEffects != null)
        {
            foreach (string id in unit.activeEffects.Keys)
            {
                var def = AbilityDatabase.Get(id);
                if (def != null) mod *= def.speedModifier;
            }
        }
        return Mathf.Max(0f, unit.moveSpeed * mod);
    }

    public static float GetModifiedArmor(Unit unit)
    {
        if (unit == null) return 0f;
        float mod = 0f;
        if (unit.activeEffects != null)
        {
            foreach (string id in unit.activeEffects.Keys)
            {
                var def = AbilityDatabase.Get(id);
                if (def != null) mod += def.armorModifier;
            }
        }
        return unit.armor + mod;
    }

    public static void NotifyAttack(Unit unit, List<Unit> nearbyEnemies)
    {
        if (unit.typeDefinition == null || string.IsNullOrEmpty(unit.typeDefinition.abilityId))
            return;

        var ability = AbilityDatabase.Get(unit.typeDefinition.abilityId);
        if (ability == null || ability.trigger != AbilityTrigger.OnAttack) return;
        if (ability.cooldown > 0f && GetCooldownRemaining(unit, ability.id) > 0f) return;
        if (ability.triggerThreshold > 0f && Random.value > ability.triggerThreshold) return;

        if (!unit.activeEffects.ContainsKey(ability.id))
        {
            ActivateAbility(unit, ability);
            if (ability.cooldown > 0f) SetCooldown(unit, ability.id, ability.cooldown);
        }
    }

    public static void NotifyKill(Unit unit, Unit killed)
    {
        if (unit.typeDefinition == null || string.IsNullOrEmpty(unit.typeDefinition.abilityId))
            return;

        var ability = AbilityDatabase.Get(unit.typeDefinition.abilityId);
        if (ability == null || ability.trigger != AbilityTrigger.OnKill) return;
        if (ability.cooldown > 0f && GetCooldownRemaining(unit, ability.id) > 0f) return;

        if (!unit.activeEffects.ContainsKey(ability.id))
        {
            ActivateAbility(unit, ability);
            if (ability.cooldown > 0f) SetCooldown(unit, ability.id, ability.cooldown);
        }
    }

    static void TickCooldowns(Unit unit, float deltaTime)
    {
        if (unit.cooldowns == null) return;
        var keys = new List<string>(unit.cooldowns.Keys);
        foreach (string k in keys)
        {
            unit.cooldowns[k] -= deltaTime;
            if (unit.cooldowns[k] <= 0f) unit.cooldowns.Remove(k);
        }
    }

    static void TickActiveDurations(Unit unit, float deltaTime)
    {
        if (unit.activeEffects == null) return;
        var keys = new List<string>(unit.activeEffects.Keys);
        foreach (string k in keys)
        {
            float rem = unit.activeEffects[k];
            if (rem >= float.MaxValue) continue;

            rem -= deltaTime;
            if (rem <= 0f)
            {
                unit.activeEffects.Remove(k);
                var def = AbilityDatabase.Get(k);
                if (def != null) DeactivateAbility(unit, def);
            }
            else
            {
                unit.activeEffects[k] = rem;
            }
        }
    }

    static float GetCooldownRemaining(Unit unit, string abilityId)
    {
        if (unit.cooldowns == null || !unit.cooldowns.TryGetValue(abilityId, out float rem))
            return 0f;
        return Mathf.Max(0f, rem);
    }

    static void SetCooldown(Unit unit, string abilityId, float duration)
    {
        if (unit.cooldowns == null) unit.cooldowns = new Dictionary<string, float>();
        unit.cooldowns[abilityId] = duration;
    }

    static bool IsAnyEnemyWithinRange(Unit unit, List<Unit> enemies, float range)
    {
        if (enemies == null || range <= 0f) return false;
        Vector3 pos = unit.transform.position;
        foreach (var e in enemies)
        {
            if (e == null || e.isDead) continue;
            if (Vector3.Distance(pos, e.transform.position) <= range)
                return true;
        }
        return false;
    }

    static bool IsAnyEnemyMatchesCategory(List<Unit> enemies, Unit unit, float range, UnitCategory[] categories)
    {
        if (enemies == null || categories == null) return false;
        Vector3 pos = unit.transform.position;
        foreach (var e in enemies)
        {
            if (e == null || e.isDead || e.typeDefinition == null) continue;
            if (Vector3.Distance(pos, e.transform.position) > range) continue;
            foreach (var cat in categories)
            {
                if (e.typeDefinition.category == cat) return true;
            }
        }
        return false;
    }

    static bool PassesTargetCategory(AbilityDefinition ability, Unit target, Unit attacker)
    {
        if (ability.targetCategoryCondition == null || ability.targetCategoryCondition.Length == 0)
            return true;
        if (target?.typeDefinition == null) return false;
        foreach (var cat in ability.targetCategoryCondition)
        {
            if (target.typeDefinition.category == cat) return true;
        }
        return false;
    }

    static bool PassesTerrainCondition(AbilityDefinition ability, Unit unit)
    {
        if (ability.terrainCondition == null || ability.terrainCondition.Length == 0)
            return true;
        TerrainType at = TerrainEffects.SampleTerrainAt(unit.transform.position);
        foreach (var t in ability.terrainCondition)
        {
            if (at == t) return true;
        }
        return false;
    }

    static bool PassesSpecialCondition(AbilityDefinition ability, Unit unit, List<Unit> nearbyEnemies)
    {
        if (string.IsNullOrEmpty(ability.specialCondition)) return true;
        if (ability.specialCondition == "AllyWithin5m")
        {
            if (FactionManager.Instance == null) return false;
            var allies = FactionManager.Instance.GetUnitsForFaction(unit.faction);
            if (allies == null) return false;
            Vector3 pos = unit.transform.position;
            foreach (var a in allies)
            {
                if (a == null || a == unit || a.isDead) continue;
                if (Vector3.Distance(pos, a.transform.position) <= 5f) return true;
            }
            return false;
        }
        return true;
    }
}
