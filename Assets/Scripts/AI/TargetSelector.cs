using System.Collections.Generic;
using UnityEngine;

public static class TargetSelector
{
    const float WeakHPThreshold = 0.35f;
    const float IsolatedRadius = 6f;

    public static Unit SelectTarget(Unit attacker, List<Unit> enemies)
    {
        if (attacker == null || enemies == null) return null;
        UnitCategory cat = GetCategory(attacker);
        switch (cat)
        {
            case UnitCategory.Ranged:
            case UnitCategory.Siege:
                return FindOptimalRangedTarget(attacker, enemies);
            case UnitCategory.HeavyCavalry:
            case UnitCategory.LightCavalry:
                return FindCavalryChargeTarget(attacker, enemies);
            default:
                return FindNearestEnemy(attacker, enemies);
        }
    }

    public static Unit FindNearestEnemy(Unit unit, List<Unit> enemies)
    {
        if (unit == null || enemies == null) return null;
        Unit nearest = null;
        float nearestSq = float.MaxValue;
        Vector3 pos = unit.transform.position;
        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            float sq = (e.transform.position - pos).sqrMagnitude;
            if (sq < nearestSq)
            {
                nearestSq = sq;
                nearest = e;
            }
        }
        return nearest;
    }

    public static Unit FindWeakestEnemy(Unit unit, List<Unit> enemies, float maxRange)
    {
        if (unit == null || enemies == null) return null;
        Unit weakest = null;
        float lowestHP = float.MaxValue;
        float rangeSq = maxRange * maxRange;
        Vector3 pos = unit.transform.position;
        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            if ((e.transform.position - pos).sqrMagnitude > rangeSq) continue;
            if (e.currentHealth < lowestHP)
            {
                lowestHP = e.currentHealth;
                weakest = e;
            }
        }
        return weakest;
    }

    public static Unit FindHighestThreat(Unit unit, List<Unit> enemies)
    {
        if (unit == null || enemies == null) return null;
        Unit highest = null;
        float bestScore = float.MinValue;
        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            float threat = e.attackDamage * (e.currentHealth / Mathf.Max(e.maxHealth, 1f));
            if (threat > bestScore)
            {
                bestScore = threat;
                highest = e;
            }
        }
        return highest;
    }

    public static Unit FindOptimalRangedTarget(Unit archer, List<Unit> enemies)
    {
        if (archer == null || enemies == null) return null;
        float range = archer.attackRange;
        float rangeSq = range * range;
        Vector3 pos = archer.transform.position;

        Unit marked = null;
        Unit weak = null;
        Unit nearest = null;
        float weakHP = float.MaxValue;
        float nearestSq = float.MaxValue;

        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            float sq = (e.transform.position - pos).sqrMagnitude;
            if (sq > rangeSq) continue;

            if (e.isMarked) marked = e;
            float hpRatio = e.currentHealth / Mathf.Max(e.maxHealth, 1f);
            if (hpRatio < weakHP && GetCategory(e) != UnitCategory.HeavyInfantry)
            {
                weakHP = hpRatio;
                weak = e;
            }
            if (sq < nearestSq)
            {
                nearestSq = sq;
                nearest = e;
            }
        }

        if (marked != null) return marked;
        if (weak != null && weakHP < WeakHPThreshold) return weak;
        return nearest;
    }

    public static Unit FindCavalryChargeTarget(Unit cavalry, List<Unit> enemies)
    {
        if (cavalry == null || enemies == null) return null;
        float range = cavalry.attackRange * 3f;
        float rangeSq = range * range;
        Vector3 pos = cavalry.transform.position;

        Unit ranged = null;
        Unit isolated = null;
        Unit weak = null;
        float isolatedDist = float.MaxValue;
        float weakHP = float.MaxValue;

        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            float sq = (e.transform.position - pos).sqrMagnitude;
            if (sq > rangeSq) continue;

            if (GetCategory(e) == UnitCategory.Ranged) ranged = e;
            int nearbyAllies = CountNearby(e, enemies, IsolatedRadius);
            if (nearbyAllies <= 1 && sq < isolatedDist)
            {
                isolatedDist = sq;
                isolated = e;
            }
            float hpRatio = e.currentHealth / Mathf.Max(e.maxHealth, 1f);
            if (hpRatio < weakHP)
            {
                weakHP = hpRatio;
                weak = e;
            }
        }

        if (ranged != null) return ranged;
        if (isolated != null) return isolated;
        return weak;
    }

    public static float ScoreTarget(Unit attacker, Unit target)
    {
        if (attacker == null || target == null) return 0f;
        float dist = Vector3.Distance(attacker.transform.position, target.transform.position);
        float distScore = 1f / (1f + dist * 0.1f);
        float hpScore = 1f - (target.currentHealth / Mathf.Max(target.maxHealth, 1f));
        float matchup = GetTypeMatchupBonus(GetCategory(attacker), GetCategory(target));
        float threat = target.attackDamage * 0.01f;
        return distScore * 2f + hpScore * 3f + matchup + threat;
    }

    public static float GetTypeMatchupBonus(UnitCategory attacker, UnitCategory target)
    {
        if (attacker == UnitCategory.HeavyCavalry || attacker == UnitCategory.LightCavalry)
        {
            if (target == UnitCategory.Ranged) return 0.5f;
            if (target == UnitCategory.LightInfantry) return 0.3f;
        }
        if (attacker == UnitCategory.HeavyInfantry || attacker == UnitCategory.LightInfantry)
        {
            if (target == UnitCategory.HeavyCavalry || target == UnitCategory.LightCavalry) return 0.2f;
        }
        if (attacker == UnitCategory.Ranged)
        {
            if (target == UnitCategory.HeavyInfantry) return -0.2f;
        }
        return 0f;
    }

    static int CountNearby(Unit center, List<Unit> enemies, float radius)
    {
        int count = 0;
        float rSq = radius * radius;
        Vector3 pos = center.transform.position;
        foreach (Unit e in enemies)
        {
            if (e == null || e == center || e.isDead) continue;
            if ((e.transform.position - pos).sqrMagnitude <= rSq) count++;
        }
        return count;
    }

    static UnitCategory GetCategory(Unit unit)
    {
        if (unit?.typeDefinition != null) return unit.typeDefinition.category;
        if (unit == null) return UnitCategory.HeavyInfantry;
        switch (unit.unitType)
        {
            case UnitType.Archer: return UnitCategory.Ranged;
            case UnitType.Berserker: return UnitCategory.LightInfantry;
            default: return UnitCategory.HeavyInfantry;
        }
    }
}
