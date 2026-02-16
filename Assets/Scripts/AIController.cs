using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; set; }

    public Faction aiFaction = Faction.South;

    [Header("AI Behavior")]
    float decisionInterval = 1.5f;
    float decisionTimer;
    float aggroRange = 25f;
    bool isActive;

    // Tactical state
    float battleStartTime;
    bool hasEngaged;

    void Awake()
    {
        Instance = this;
    }

    public void Activate()
    {
        isActive = true;
        decisionTimer = 1f;
        battleStartTime = Time.time;
    }

    void Update()
    {
        if (!isActive) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            MakeDecisions();
            decisionTimer = decisionInterval;
        }
    }

    void MakeDecisions()
    {
        if (FactionManager.Instance == null) return;

        List<Unit> aiUnits = FactionManager.Instance.GetUnitsForFaction(aiFaction);
        Faction enemyFaction = aiFaction == Faction.North ? Faction.South : Faction.North;
        List<Unit> enemyUnits = FactionManager.Instance.GetUnitsForFaction(enemyFaction);

        if (aiUnits.Count == 0 || enemyUnits.Count == 0) return;

        // Calculate battle state
        float aiStrength = CalculateStrength(aiUnits);
        float enemyStrength = CalculateStrength(enemyUnits);
        bool isWinning = aiStrength > enemyStrength * 1.2f;
        bool isLosing = aiStrength < enemyStrength * 0.7f;

        foreach (Unit aiUnit in aiUnits)
        {
            if (aiUnit == null || aiUnit.isDead) continue;

            UnitCombat combat = aiUnit.GetComponent<UnitCombat>();
            UnitMovement movement = aiUnit.GetComponent<UnitMovement>();

            if (combat == null || movement == null) continue;

            // If already fighting, only override for special tactical decisions
            if (combat.HasTarget())
            {
                HandleInCombatBehavior(aiUnit, combat, movement, enemyUnits, isLosing);
                continue;
            }

            // Find nearest enemy
            Unit nearestEnemy = FindNearestEnemy(aiUnit, enemyUnits);
            if (nearestEnemy == null) continue;

            float dist = Vector3.Distance(aiUnit.transform.position, nearestEnemy.transform.position);

            switch (aiUnit.unitType)
            {
                case UnitType.Shieldbearer:
                    HandleShieldbearerAI(aiUnit, combat, movement, nearestEnemy, dist, enemyUnits);
                    break;
                case UnitType.Berserker:
                    HandleBerserkerAI(aiUnit, combat, movement, nearestEnemy, dist, isWinning);
                    break;
                case UnitType.Swordsman:
                    HandleSwordsmanAI(aiUnit, combat, movement, nearestEnemy, dist);
                    break;
                case UnitType.Archer:
                    HandleArcherAI(aiUnit, combat, movement, nearestEnemy, dist, enemyUnits);
                    break;
            }
        }
    }

    void HandleShieldbearerAI(Unit unit, UnitCombat combat, UnitMovement movement,
        Unit nearestEnemy, float dist, List<Unit> enemies)
    {
        if (dist <= aggroRange)
        {
            // Shieldbearers advance to front and shield wall when near enemies
            if (dist <= unit.attackRange * 2f && !unit.isShieldWalling)
            {
                unit.ActivateShieldWall();
            }

            if (dist <= unit.attackRange)
            {
                combat.SetTarget(nearestEnemy);
            }
            else
            {
                // Move to nearest enemy, positioning in front of friendly ranged
                movement.MoveTo(nearestEnemy.transform.position);
            }
        }
        else
        {
            movement.MoveTo(nearestEnemy.transform.position);
        }
    }

    void HandleBerserkerAI(Unit unit, UnitCombat combat, UnitMovement movement,
        Unit nearestEnemy, float dist, bool isWinning)
    {
        if (dist <= aggroRange)
        {
            // Berserkers always rage when engaging if not already
            if (!unit.isEnraged && dist <= 12f)
            {
                unit.ActivateRage();
            }

            // Prioritize weak enemies for the kill
            Unit weakTarget = FindWeakestEnemy(unit);
            Unit target = weakTarget != null ? weakTarget : nearestEnemy;

            if (dist <= unit.attackRange)
            {
                combat.SetTarget(target);
            }
            else
            {
                // Charge directly
                movement.MoveTo(target.transform.position);
            }
        }
        else
        {
            movement.MoveTo(nearestEnemy.transform.position);
        }
    }

    void HandleSwordsmanAI(Unit unit, UnitCombat combat, UnitMovement movement,
        Unit nearestEnemy, float dist)
    {
        if (dist <= aggroRange)
        {
            combat.SetTarget(nearestEnemy);
        }
        else
        {
            movement.MoveTo(nearestEnemy.transform.position);
        }
    }

    void HandleArcherAI(Unit unit, UnitCombat combat, UnitMovement movement,
        Unit nearestEnemy, float dist, List<Unit> enemies)
    {
        if (dist <= aggroRange)
        {
            if (dist < unit.attackRange * 0.5f)
            {
                // Too close -- retreat behind friendlies
                Vector3 retreatDir = (unit.transform.position - nearestEnemy.transform.position).normalized;
                Vector3 retreatPos = unit.transform.position + retreatDir * 8f;
                movement.MoveTo(retreatPos);
            }
            else if (dist <= unit.attackRange)
            {
                // Prioritize marked targets or low-HP enemies
                Unit bestTarget = FindBestArcherTarget(unit, enemies);
                combat.SetTarget(bestTarget != null ? bestTarget : nearestEnemy);
            }
            else
            {
                // Close distance but stay at range
                Vector3 dirToEnemy = (nearestEnemy.transform.position - unit.transform.position).normalized;
                Vector3 idealPos = nearestEnemy.transform.position - dirToEnemy * (unit.attackRange * 0.8f);
                movement.MoveTo(idealPos);
            }
        }
        else
        {
            Vector3 advanceTarget = nearestEnemy.transform.position;
            Vector3 dirToEnemy = (advanceTarget - unit.transform.position).normalized;
            advanceTarget -= dirToEnemy * 8f;
            movement.MoveTo(advanceTarget);
        }
    }

    void HandleInCombatBehavior(Unit unit, UnitCombat combat, UnitMovement movement,
        List<Unit> enemies, bool isLosing)
    {
        // Berserker: rage when taking damage
        if (unit.unitType == UnitType.Berserker && !unit.isEnraged
            && unit.currentHealth < unit.maxHealth * 0.6f)
        {
            unit.ActivateRage();
        }

        // Shieldbearer: shield wall if surrounded
        if (unit.unitType == UnitType.Shieldbearer && !unit.isShieldWalling)
        {
            int nearbyEnemies = CountNearbyEnemies(unit, enemies, 5f);
            if (nearbyEnemies >= 2)
            {
                unit.ActivateShieldWall();
            }
        }
    }

    // --- Target selection helpers ---

    Unit FindWeakestEnemy(Unit fromUnit)
    {
        Faction enemyFaction = fromUnit.faction == Faction.North ? Faction.South : Faction.North;
        List<Unit> enemies = FactionManager.Instance.GetUnitsForFaction(enemyFaction);

        Unit weakest = null;
        float lowestHP = float.MaxValue;

        foreach (Unit enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;
            float dist = Vector3.Distance(fromUnit.transform.position, enemy.transform.position);
            if (dist > aggroRange) continue;

            if (enemy.currentHealth < lowestHP && enemy.currentHealth < enemy.maxHealth * 0.5f)
            {
                lowestHP = enemy.currentHealth;
                weakest = enemy;
            }
        }
        return weakest;
    }

    Unit FindBestArcherTarget(Unit archer, List<Unit> enemies)
    {
        Unit bestTarget = null;
        float bestScore = float.MinValue;

        foreach (Unit enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;
            float dist = Vector3.Distance(archer.transform.position, enemy.transform.position);
            if (dist > archer.attackRange) continue;

            float score = 0;
            // Prefer marked targets
            if (enemy.isMarked) score += 50f;
            // Prefer low HP targets
            score += (1f - enemy.currentHealth / enemy.maxHealth) * 30f;
            // Prefer archers and berserkers (squishier, high-value)
            if (enemy.unitType == UnitType.Archer) score += 20f;
            if (enemy.unitType == UnitType.Berserker && enemy.isEnraged) score += 25f;
            // Deprioritize shieldbearers in shield wall
            if (enemy.unitType == UnitType.Shieldbearer && enemy.isShieldWalling) score -= 30f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = enemy;
            }
        }
        return bestTarget;
    }

    int CountNearbyEnemies(Unit unit, List<Unit> enemies, float range)
    {
        int count = 0;
        foreach (Unit enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;
            if (Vector3.Distance(unit.transform.position, enemy.transform.position) <= range)
                count++;
        }
        return count;
    }

    float CalculateStrength(List<Unit> units)
    {
        float strength = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            strength += u.currentHealth + u.attackDamage * 3f + u.armor * 2f;
            if (u.isEnraged) strength += 30f;
            if (u.isShieldWalling) strength += 20f;
        }
        return strength;
    }

    Unit FindNearestEnemy(Unit fromUnit, List<Unit> enemies)
    {
        Unit nearest = null;
        float closestDist = float.MaxValue;

        foreach (Unit enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;

            float dist = Vector3.Distance(fromUnit.transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
