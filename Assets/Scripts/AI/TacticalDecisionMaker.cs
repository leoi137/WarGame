using System.Collections.Generic;
using UnityEngine;

public static class TacticalDecisionMaker
{
    public enum TacticalStance
    {
        Aggressive,
        Balanced,
        Defensive,
        Flanking,
        Retreating
    }

    const float AggressiveThreshold = 1.4f;
    const float DefensiveThreshold = 0.6f;
    const float RetreatThreshold = 0.4f;
    const float FocusFireAllyRadius = 8f;

    public static TacticalStance EvaluateStance(List<Unit> own, List<Unit> enemy)
    {
        float ratio = CalculateStrengthRatio(own, enemy);
        if (ratio >= AggressiveThreshold) return TacticalStance.Aggressive;
        if (ratio <= RetreatThreshold) return TacticalStance.Retreating;
        if (ratio <= DefensiveThreshold) return TacticalStance.Defensive;
        if (ratio > 1f) return TacticalStance.Balanced;
        return TacticalStance.Flanking;
    }

    public static Vector3 GetAdvanceTarget(List<Unit> own, List<Unit> enemy, Faction side)
    {
        Vector3 ownCenter = GetArmyCenter(own);
        Vector3 enemyCenter = GetArmyCenter(enemy);
        Vector3 dir = (enemyCenter - ownCenter).normalized;
        dir.y = 0;
        if (dir == Vector3.zero) dir = side == Faction.Attacker ? Vector3.forward : -Vector3.forward;
        return ownCenter + dir * 20f;
    }

    public static Vector3 GetFlankPosition(Unit unit, List<Unit> enemy)
    {
        Vector3 enemyCenter = GetArmyCenter(enemy);
        Vector3 toUnit = (unit.transform.position - enemyCenter).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, toUnit);
        return enemyCenter + right * 15f;
    }

    public static Vector3 GetRetreatPosition(Unit unit, Faction side, int mapSize)
    {
        float margin = mapSize * 0.15f;
        float z = side == Faction.Attacker ? margin : mapSize - margin;
        Vector3 retreat = new Vector3(unit.transform.position.x, 0, z);
        retreat.x = Mathf.Clamp(retreat.x, margin, mapSize - margin);
        return retreat;
    }

    public static bool ShouldFocusFire(List<Unit> own, Unit target)
    {
        if (target == null || target.isDead) return false;
        int alliesTargeting = 0;
        foreach (Unit u in own)
        {
            if (u == null || u.isDead) continue;
            var combat = u.GetComponent<UnitCombat>();
            if (combat == null) continue;
            if (combat.GetCurrentTarget() == target) alliesTargeting++;
            else if (Vector3.Distance(u.transform.position, target.transform.position) <= FocusFireAllyRadius)
                alliesTargeting++;
        }
        return alliesTargeting >= 2;
    }

    public static float CalculateStrengthRatio(List<Unit> own, List<Unit> enemy)
    {
        float ownStr = EvaluateArmyStrength(own);
        float enemyStr = EvaluateArmyStrength(enemy);
        if (enemyStr <= 0f) return 2f;
        return ownStr / enemyStr;
    }

    static Vector3 GetArmyCenter(List<Unit> units)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.position;
            count++;
        }
        return count > 0 ? sum / count : Vector3.zero;
    }

    static float EvaluateArmyStrength(List<Unit> units)
    {
        float strength = 0f;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            float hp = u.currentHealth / Mathf.Max(u.maxHealth, 1f);
            float dmg = u.attackDamage;
            float arm = AbilitySystem.GetModifiedArmor(u);
            strength += hp * (dmg + arm * 0.5f);
        }
        return strength;
    }
}
