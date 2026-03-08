using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public BattlePhase CurrentPhase { get; private set; }
    public BattleConfiguration Config { get; private set; }
    public List<Unit> attackerUnits = new();
    public List<Unit> defenderUnits = new();
    public SimulationAI attackerAI;
    public SimulationAI defenderAI;
    public Faction? PlayerSide { get; private set; }

    [SerializeField] BattleSetup battleSetup;
    [SerializeField] BattleSimulator battleSimulator;
    [SerializeField] BattleCamera battleCamera;
    [SerializeField] BattleTimeController timeController;
    [SerializeField] BattleResultsScreen resultsScreen;
    [SerializeField] TerrainGenerator terrainGenerator;

    SpatialGrid spatialGrid;
    float simulationStartTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        CheckBattleEnd();
    }

    public void Initialize(BattleConfiguration config, Faction? playerSide = null)
    {
        Config = config;
        PlayerSide = playerSide;
        CurrentPhase = BattlePhase.Loading;

        int mapSize = config.mapSize > 0 ? config.mapSize : GameConfig.DefaultMapSize;

        if (terrainGenerator == null)
            terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
        if (terrainGenerator != null)
            terrainGenerator.Generate(config);

        int gridCells = Mathf.CeilToInt(mapSize / GameConfig.SpatialGridCellSize);
        spatialGrid = new SpatialGrid(GameConfig.SpatialGridCellSize, gridCells, gridCells);

        if (battleCamera == null)
            battleCamera = FindFirstObjectByType<BattleCamera>();
        battleCamera?.Initialize(mapSize);

        if (timeController == null)
            timeController = FindFirstObjectByType<BattleTimeController>();

        if (resultsScreen == null)
            resultsScreen = FindFirstObjectByType<BattleResultsScreen>();

        attackerUnits.Clear();
        defenderUnits.Clear();

        StartPlacement();
    }

    public void StartPlacement()
    {
        CurrentPhase = BattlePhase.Placement;

        if (battleSetup == null)
            battleSetup = FindFirstObjectByType<BattleSetup>();
        if (battleSetup != null)
            battleSetup.Initialize(Config, PlayerSide);
    }

    public void ConfirmPlacement()
    {
        if (battleSetup != null)
            battleSetup.ConfirmPlacement();
        CurrentPhase = BattlePhase.Countdown;
    }

    public void StartSimulation()
    {
        CurrentPhase = BattlePhase.Simulating;
        simulationStartTime = Time.time;

        if (battleSimulator == null)
            battleSimulator = FindFirstObjectByType<BattleSimulator>();

        spatialGrid.Clear();
        foreach (Unit u in attackerUnits)
            if (u != null && !u.isDead) spatialGrid.Insert(u);
        foreach (Unit u in defenderUnits)
            if (u != null && !u.isDead) spatialGrid.Insert(u);

        Faction enemyA = Faction.Defender;
        Faction enemyB = Faction.Attacker;
        if (attackerAI == null)
        {
            GameObject goA = new GameObject("AttackerAI");
            attackerAI = goA.AddComponent<SimulationAI>();
        }
        if (defenderAI == null)
        {
            GameObject goB = new GameObject("DefenderAI");
            defenderAI = goB.AddComponent<SimulationAI>();
        }
        attackerAI.Initialize(Faction.Attacker, attackerUnits, defenderUnits, spatialGrid, Config.mapSize);
        defenderAI.Initialize(Faction.Defender, defenderUnits, attackerUnits, spatialGrid, Config.mapSize);

        battleSimulator?.BeginSimulation(attackerUnits, defenderUnits, attackerAI, defenderAI);
        timeController?.Play();
        battleCamera?.SetAutoFollow(true);
    }

    public void PauseSimulation()
    {
        timeController?.Pause();
    }

    public void ResumeSimulation()
    {
        timeController?.Play();
    }

    public void SetSimulationSpeed(float speed)
    {
        timeController?.SetSpeed(speed);
    }

    public void CheckBattleEnd()
    {
        if (CurrentPhase != BattlePhase.Simulating) return;
        if (battleSimulator == null) return;
        if (!battleSimulator.IsBattleOver()) return;

        Faction winner = battleSimulator.GetWinner();
        EndBattle(winner);
    }

    public void EndBattle(Faction winner)
    {
        CurrentPhase = BattlePhase.Ended;
        BattleResult result = CalculateResult(winner);
        CurrentPhase = BattlePhase.Results;

        if (resultsScreen != null)
            resultsScreen.Show(result, Config.attackerFaction, Config.defenderFaction);
    }

    public void Cleanup()
    {
        foreach (Unit u in attackerUnits)
            if (u != null) Destroy(u.gameObject);
        foreach (Unit u in defenderUnits)
            if (u != null) Destroy(u.gameObject);
        attackerUnits.Clear();
        defenderUnits.Clear();

        if (attackerAI != null) Destroy(attackerAI.gameObject);
        if (defenderAI != null) Destroy(defenderAI.gameObject);
        attackerAI = null;
        defenderAI = null;

        battleSetup = null;
        battleSimulator = null;
    }

    public BattleResult CalculateResult(Faction winner)
    {
        var result = new BattleResult();
        result.winningSide = winner;
        result.winnerFactionId = winner == Faction.Attacker ? Config?.attackerFaction?.id : Config?.defenderFaction?.id;
        result.loserFactionId = winner == Faction.Attacker ? Config?.defenderFaction?.id : Config?.attackerFaction?.id;
        result.winnerStartCount = winner == Faction.Attacker ? attackerUnits.Count : defenderUnits.Count;
        result.loserStartCount = winner == Faction.Attacker ? defenderUnits.Count : attackerUnits.Count;

        int winnerSurvivors = 0;
        foreach (Unit u in winner == Faction.Attacker ? attackerUnits : defenderUnits)
            if (u != null && !u.isDead) winnerSurvivors++;
        result.winnerSurvivors = winnerSurvivors;
        result.totalCasualties = result.winnerStartCount - winnerSurvivors + result.loserStartCount;
        result.battleDurationSeconds = Time.time - simulationStartTime;

        return result;
    }

    public void RegisterAttackerUnit(Unit unit)
    {
        if (unit != null && !attackerUnits.Contains(unit))
            attackerUnits.Add(unit);
    }

    public void RegisterDefenderUnit(Unit unit)
    {
        if (unit != null && !defenderUnits.Contains(unit))
            defenderUnits.Add(unit);
    }
}
