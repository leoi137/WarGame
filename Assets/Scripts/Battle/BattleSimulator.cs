using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour
{
    public bool isRunning { get; private set; }
    public float elapsedTime { get; private set; }
    public int frameCount { get; private set; }

    List<Unit> attackers;
    List<Unit> defenders;
    SimulationAI aiA;
    SimulationAI aiB;
    SpatialGrid spatialGrid;

    void Update()
    {
        if (!isRunning) return;
        ProcessFrame();
    }

    public void BeginSimulation(List<Unit> attackerUnits, List<Unit> defenderUnits, SimulationAI aiA, SimulationAI aiB)
    {
        attackers = attackerUnits ?? new List<Unit>();
        defenders = defenderUnits ?? new List<Unit>();
        this.aiA = aiA;
        this.aiB = aiB;
        elapsedTime = 0f;
        frameCount = 0;
        isRunning = true;

        int mapSize = 300;
        int gridCells = Mathf.CeilToInt(mapSize / GameConfig.SpatialGridCellSize);
        spatialGrid = new SpatialGrid(GameConfig.SpatialGridCellSize, gridCells, gridCells);
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void Resume()
    {
        isRunning = true;
    }

    public bool IsBattleOver()
    {
        bool noAttackers = true;
        foreach (Unit u in attackers)
            if (u != null && !u.isDead) { noAttackers = false; break; }
        if (noAttackers) return true;

        bool noDefenders = true;
        foreach (Unit u in defenders)
            if (u != null && !u.isDead) { noDefenders = false; break; }
        return noDefenders;
    }

    public Faction GetWinner()
    {
        bool hasAttacker = false;
        foreach (Unit u in attackers)
            if (u != null && !u.isDead) { hasAttacker = true; break; }
        bool hasDefender = false;
        foreach (Unit u in defenders)
            if (u != null && !u.isDead) { hasDefender = true; break; }

        if (hasAttacker && !hasDefender) return Faction.Attacker;
        if (hasDefender && !hasAttacker) return Faction.Defender;
        return Faction.Attacker;
    }

    public void ProcessFrame()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        frameCount++;

        if (spatialGrid != null)
        {
            spatialGrid.Clear();
            foreach (Unit u in attackers)
                if (u != null && !u.isDead) spatialGrid.Insert(u);
            foreach (Unit u in defenders)
                if (u != null && !u.isDead) spatialGrid.Insert(u);
        }

        aiA?.MakeDecisions();
        aiB?.MakeDecisions();

        var allUnits = new List<Unit>();
        foreach (Unit u in attackers)
            if (u != null && !u.isDead) allUnits.Add(u);
        foreach (Unit u in defenders)
            if (u != null && !u.isDead) allUnits.Add(u);

        if (Camera.main != null)
            UnitLODSystem.UpdateLOD(allUnits, Camera.main.transform.position);
    }
}
