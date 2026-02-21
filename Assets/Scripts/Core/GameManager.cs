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

        TransitionTo(GameFlowState.MainMenu);
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
        // Intentionally lean for now. State-specific managers should handle local cleanup.
        // We only clear transient global event subscriptions at major state boundaries.
        if (state is GameFlowState.BattleResults or GameFlowState.CampaignVictory or GameFlowState.CampaignDefeat)
        {
            EventBus.Clear();
        }
    }

    private void InitializeState(GameFlowState state)
    {
        // Bootstrapping points for state-specific managers.
        switch (state)
        {
            case GameFlowState.MainMenu:
            case GameFlowState.WorldMap:
            case GameFlowState.FactionSelect:
            case GameFlowState.BattleSetup:
            case GameFlowState.BattleSimulation:
            case GameFlowState.BattleResults:
            case GameFlowState.CampaignSetup:
            case GameFlowState.CampaignMap:
            case GameFlowState.CampaignBattle:
            case GameFlowState.CampaignTurnResolve:
            case GameFlowState.CampaignVictory:
            case GameFlowState.CampaignDefeat:
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
