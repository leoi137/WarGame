using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; set; }

    public Faction aiFaction = Faction.South;

    [Header("AI Behavior")]
    float decisionInterval = 2.0f;
    float decisionTimer;
    float aggroRange = 20f;
    bool isActive;

    void Awake()
    {
        Instance = this;
    }

    public void Activate()
    {
        isActive = true;
        decisionTimer = 1f; // First decision after 1 second
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
        List<Unit> enemyUnits = FactionManager.Instance.GetUnitsForFaction(
            aiFaction == Faction.North ? Faction.South : Faction.North);

        if (aiUnits.Count == 0 || enemyUnits.Count == 0) return;

        foreach (Unit aiUnit in aiUnits)
        {
            if (aiUnit == null || aiUnit.isDead) continue;

            UnitCombat combat = aiUnit.GetComponent<UnitCombat>();
            UnitMovement movement = aiUnit.GetComponent<UnitMovement>();

            if (combat == null || movement == null) continue;

            // If already fighting, let combat system handle it
            if (combat.HasTarget()) continue;

            // Find nearest enemy
            Unit nearestEnemy = FindNearestEnemy(aiUnit, enemyUnits);
            if (nearestEnemy == null) continue;

            float dist = Vector3.Distance(aiUnit.transform.position, nearestEnemy.transform.position);

            if (dist <= aggroRange)
            {
                // Engage
                if (aiUnit.unitType == UnitType.Swordsman)
                {
                    // Swordsmen charge directly
                    combat.SetTarget(nearestEnemy);
                }
                else
                {
                    // Archers try to keep distance
                    if (dist < aiUnit.attackRange * 0.6f)
                    {
                        // Too close, back up
                        Vector3 retreatDir = (aiUnit.transform.position - nearestEnemy.transform.position).normalized;
                        Vector3 retreatPos = aiUnit.transform.position + retreatDir * 5f;
                        movement.MoveTo(retreatPos);
                    }
                    else
                    {
                        // In range or approaching -- attack
                        combat.SetTarget(nearestEnemy);
                    }
                }
            }
            else
            {
                // Advance toward enemy territory
                Vector3 advanceTarget = nearestEnemy.transform.position;

                if (aiUnit.unitType == UnitType.Archer)
                {
                    // Archers stay behind swordsmen
                    Vector3 dirToEnemy = (advanceTarget - aiUnit.transform.position).normalized;
                    advanceTarget -= dirToEnemy * 6f;
                }

                movement.MoveTo(advanceTarget);
            }
        }
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
