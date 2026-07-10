using UnityEngine;

/// <summary>
/// Top-level runtime orchestrator for major game states.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameFlowState CurrentState { get; private set; } = GameFlowState.MainMenu;

    public FactionDefinition SelectedAttacker;
    public FactionDefinition SelectedDefender;
    public BattleConfiguration CurrentBattle;
    public BattleResult LastBattleResult;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Databases must be initialized before any state transition creates UI.
        // GameBootstrap.Start() handles this, but if GameManager is standalone,
        // ensure databases are ready.
        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        if (CurrentState == GameFlowState.MainMenu)
            InitializeState(GameFlowState.MainMenu);
    }

    public void TransitionTo(GameFlowState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        var oldState = CurrentState;
        CleanupState(oldState);

        CurrentState = newState;

        InitializeState(newState);
        EventBus.Publish(new GameStateChangedEvent { oldState = oldState, newState = newState });
    }

    public void StartQuickBattle(FactionDefinition attacker, FactionDefinition defender)
    {
        SelectedAttacker = attacker;
        SelectedDefender = defender;

        CurrentBattle = BattleConfiguration.Create(attacker, defender);
        TransitionTo(GameFlowState.BattleSetup);
    }

    public void StartBattleSimulation()
    {
        TransitionTo(GameFlowState.BattleSimulation);
    }

    public void EndBattle(BattleResult result)
    {
        LastBattleResult = result;
        TransitionTo(GameFlowState.BattleResults);
    }

    public void ReturnToWorldMap()
    {
        TransitionTo(GameFlowState.WorldMap);
    }

    public void ReturnToMainMenu()
    {
        TransitionTo(GameFlowState.MainMenu);
    }

    private void CleanupState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.MainMenu:
                DestroyUI<MainMenuUI>(ui => ui.Hide());
                break;
            case GameFlowState.FactionSelect:
                DestroyUI<FactionSelectUI>(ui => ui.Hide());
                break;
            case GameFlowState.WorldMap:
                DestroyUI<WorldMapHUD>(ui => ui.Hide());
                break;
            case GameFlowState.BattleSetup:
                DestroyUI<BattleSetupUI>(ui => ui.Hide());
                break;
            case GameFlowState.BattleSimulation:
                DestroyUI<BattleHUD>();
                DestroyUI<BattleManager>();
                break;
            case GameFlowState.BattleResults:
                DestroyUI<BattleResultsUI>(ui => ui.Hide());
                EventBus.Clear();
                break;
            case GameFlowState.CampaignSetup:
                DestroyUI<CampaignSetupUI>(ui => ui.Hide());
                break;
            case GameFlowState.CampaignMap:
                DestroyUI<CampaignMapUI>(ui => ui.Hide());
                DestroyUI<CampaignHUD>(ui => ui.Hide());
                break;
            case GameFlowState.CampaignBattle:
                DestroyUI<BattleHUD>();
                DestroyUI<BattleManager>();
                break;
            case GameFlowState.CampaignTurnResolve:
                break;
            case GameFlowState.CampaignVictory:
            case GameFlowState.CampaignDefeat:
                DestroyUI<CampaignVictoryUI>(ui => ui.Hide());
                EventBus.Clear();
                break;
        }
    }

    private void DestroyUI<T>(System.Action<T> cleanup = null) where T : MonoBehaviour
    {
        var ui = FindFirstObjectByType<T>();
        if (ui == null) return;
        cleanup?.Invoke(ui);
        Destroy(ui.gameObject);
    }

    private void InitializeState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.MainMenu:
                SpawnUI<MainMenuUI>().Show();
                break;

            case GameFlowState.WorldMap:
                SpawnUI<WorldMapManager>();
                SpawnUI<WorldMapHUD>().Show();
                break;

            case GameFlowState.FactionSelect:
                SpawnUI<FactionSelectUI>().Show(true, attacker =>
                {
                    SelectedAttacker = attacker;
                    FindFirstObjectByType<FactionSelectUI>()?.Show(false, defender =>
                    {
                        SelectedDefender = defender;
                        StartQuickBattle(attacker, defender);
                    });
                });
                break;

            case GameFlowState.BattleSetup:
                if (CurrentBattle != null)
                {
                    var setupUI = SpawnUI<BattleSetupUI>();
                    setupUI.Show(CurrentBattle, CurrentBattle.playerSide ?? Faction.Attacker);
                }
                break;

            case GameFlowState.BattleSimulation:
                var bmObj = new GameObject("BattleManager");
                var bm = bmObj.AddComponent<BattleManager>();
                if (CurrentBattle != null)
                {
                    bm.Initialize(CurrentBattle, CurrentBattle.playerSide);
                    var hud = SpawnUI<BattleHUD>();
                    hud.Initialize(CurrentBattle);
                }
                break;

            case GameFlowState.BattleResults:
                SpawnUI<BattleResultsUI>().Show(LastBattleResult, SelectedAttacker, SelectedDefender);
                break;

            case GameFlowState.CampaignSetup:
                SpawnUI<CampaignSetupUI>().Show();
                break;

            case GameFlowState.CampaignMap:
                if (CampaignManager.Instance != null)
                {
                    var mapUI = SpawnUI<CampaignMapUI>();
                    mapUI.Show(CampaignManager.Instance.currentState, CampaignManager.Instance.provinceManager);
                    SpawnUI<CampaignHUD>().Show();
                }
                break;

            case GameFlowState.CampaignBattle:
                var cbmObj = new GameObject("BattleManager");
                var cbm = cbmObj.AddComponent<BattleManager>();
                if (CurrentBattle != null)
                {
                    cbm.Initialize(CurrentBattle, CurrentBattle.playerSide);
                    SpawnUI<BattleHUD>().Initialize(CurrentBattle);
                }
                break;

            case GameFlowState.CampaignTurnResolve:
                break;

            case GameFlowState.CampaignVictory:
                if (CampaignManager.Instance != null)
                    SpawnUI<CampaignVictoryUI>().ShowVictory(CampaignManager.Instance.currentState);
                break;

            case GameFlowState.CampaignDefeat:
                if (CampaignManager.Instance != null)
                    SpawnUI<CampaignVictoryUI>().ShowDefeat(CampaignManager.Instance.currentState);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Creates a temporary GameObject with the given MonoBehaviour component.
    /// Used for procedural UI screens that manage their own lifecycle.
    /// </summary>
    private T SpawnUI<T>() where T : MonoBehaviour
    {
        var existing = FindFirstObjectByType<T>();
        if (existing != null) return existing;

        var go = new GameObject(typeof(T).Name);
        return go.AddComponent<T>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
