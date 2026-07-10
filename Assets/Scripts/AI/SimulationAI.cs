using System.Collections.Generic;
using UnityEngine;

public class SimulationAI : MonoBehaviour
{
    Faction side;
    List<Unit> ownUnits;
    List<Unit> enemyUnits;
    SpatialGrid grid;
    float decisionTimer;
    int mapSize;

    public void Initialize(Faction side, List<Unit> own, List<Unit> enemy, SpatialGrid grid, int mapSize = 0)
    {
        this.side = side;
        this.ownUnits = own ?? new List<Unit>();
        this.enemyUnits = enemy ?? new List<Unit>();
        this.grid = grid;
        this.mapSize = mapSize > 0 ? mapSize : GameConfig.DefaultMapSize;
        decisionTimer = 0f;
    }

    void Update()
    {
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= GameConfig.SimulationTickRate)
        {
            decisionTimer = 0f;
            MakeDecisions();
        }
    }

    public void MakeDecisions()
    {
        if (ownUnits == null || grid == null) return;
        FilterDeadUnits();
        Faction enemyFaction = side == Faction.Attacker ? Faction.Defender : Faction.Attacker;
        foreach (Unit unit in ownUnits)
        {
            if (unit == null || unit.isDead) continue;
            AssignOrder(unit, enemyFaction);
        }
    }

    void AssignOrder(Unit unit, Faction enemyFaction)
    {
        UnitCategory cat = GetCategory(unit);
        switch (cat)
        {
            case UnitCategory.HeavyInfantry: HandleHeavyInfantry(unit, enemyFaction); break;
            case UnitCategory.LightInfantry: HandleLightInfantry(unit, enemyFaction); break;
            case UnitCategory.HeavyCavalry: HandleHeavyCavalry(unit, enemyFaction); break;
            case UnitCategory.LightCavalry: HandleLightCavalry(unit, enemyFaction); break;
            case UnitCategory.Ranged: HandleRanged(unit, enemyFaction); break;
            case UnitCategory.Siege: HandleSiege(unit, enemyFaction); break;
            case UnitCategory.Elephant: HandleElephant(unit, enemyFaction); break;
            case UnitCategory.Naval: HandleNaval(unit, enemyFaction); break;
            case UnitCategory.Special: HandleSpecial(unit, enemyFaction); break;
            default: HandleHeavyInfantry(unit, enemyFaction); break;
        }
    }

    void HandleHeavyInfantry(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = grid.GetNearest(unit.transform.position, enemyFaction, unit.attackRange * 3f);
        if (target == null) target = TargetSelector.FindNearestEnemy(unit, enemyUnits);
        if (target != null)
        {
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
            return;
        }
        if (ShouldAdvance())
        {
            Vector3 advance = TacticalDecisionMaker.GetAdvanceTarget(ownUnits, enemyUnits, side);
            unit.GetComponent<UnitMovement>()?.MoveTo(advance);
        }
    }

    void HandleLightInfantry(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.SelectTarget(unit, enemyUnits);
        if (target == null) target = grid.GetNearest(unit.transform.position, enemyFaction, unit.attackRange * 4f);
        if (target != null)
        {
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
            return;
        }
        Vector3 flank = TacticalDecisionMaker.GetFlankPosition(unit, enemyUnits);
        unit.GetComponent<UnitMovement>()?.MoveTo(flank);
    }

    void HandleHeavyCavalry(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.FindCavalryChargeTarget(unit, enemyUnits);
        if (target == null) target = grid.GetNearest(unit.transform.position, enemyFaction, unit.attackRange * 5f);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetAdvanceTarget(ownUnits, enemyUnits, side));
    }

    void HandleLightCavalry(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.FindCavalryChargeTarget(unit, enemyUnits);
        if (target == null) target = TargetSelector.FindNearestEnemy(unit, enemyUnits);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetFlankPosition(unit, enemyUnits));
    }

    void HandleRanged(Unit unit, Faction enemyFaction)
    {
        var nearby = grid.GetNearby(unit.transform.position, 3f);
        foreach (Unit u in nearby)
        {
            if (u == null || u.isDead || u.faction != enemyFaction) continue;
            if (GetCategory(u) != UnitCategory.Ranged && GetCategory(u) != UnitCategory.Siege)
            {
                unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
                return;
            }
        }
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.FindOptimalRangedTarget(unit, enemyUnits);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
        {
            Vector3 pos = TerrainAnalyzer.FindBestRangedPosition(unit.transform.position, enemyUnits, unit.attackRange);
            unit.GetComponent<UnitMovement>()?.MoveTo(pos);
        }
    }

    void HandleSiege(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.FindOptimalRangedTarget(unit, enemyUnits);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetAdvanceTarget(ownUnits, enemyUnits, side));
    }

    void HandleElephant(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.FindNearestEnemy(unit, enemyUnits);
        if (target == null) target = grid.GetNearest(unit.transform.position, enemyFaction, unit.attackRange * 4f);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetAdvanceTarget(ownUnits, enemyUnits, side));
    }

    void HandleNaval(Unit unit, Faction enemyFaction)
    {
        if (ShouldRetreat(unit))
        {
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetRetreatPosition(unit, side, mapSize));
            return;
        }
        Unit target = TargetSelector.SelectTarget(unit, enemyUnits);
        if (target != null)
            unit.GetComponent<UnitCombat>()?.SetTarget(target);
        else
            unit.GetComponent<UnitMovement>()?.MoveTo(TacticalDecisionMaker.GetAdvanceTarget(ownUnits, enemyUnits, side));
    }

    void HandleSpecial(Unit unit, Faction enemyFaction)
    {
        Vector3 center = GetFormationCenter(ownUnits);
        if (Vector3.Distance(unit.transform.position, center) > 15f)
            unit.GetComponent<UnitMovement>()?.MoveTo(center);
    }

    public float EvaluateArmyStrength(List<Unit> units)
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

    public bool ShouldRetreat(Unit unit)
    {
        if (unit == null || unit.isDead) return false;
        if (unit.currentHealth / Mathf.Max(unit.maxHealth, 1f) < 0.2f) return true;
        int nearby = grid != null ? grid.CountInRadius(unit.transform.position, 8f, unit.faction) : 0;
        return nearby < 2 && unit.currentHealth / Mathf.Max(unit.maxHealth, 1f) < 0.4f;
    }

    public bool ShouldAdvance()
    {
        float ratio = TacticalDecisionMaker.CalculateStrengthRatio(ownUnits, enemyUnits);
        return ratio >= 1f;
    }

    void FilterDeadUnits()
    {
        ownUnits?.RemoveAll(u => u == null || u.isDead);
        enemyUnits?.RemoveAll(u => u == null || u.isDead);
    }

    static Vector3 GetFormationCenter(List<Unit> units)
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
