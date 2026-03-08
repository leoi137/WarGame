using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Top-level campaign orchestrator.
/// </summary>
public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance { get; set; }

    public CampaignState currentState;
    public ProvinceManager provinceManager;

    private CampaignAction _pendingPlayerAction;
    private bool _isPlayerTurn;
    private BattleRandom _rng;

    public bool IsPlayerTurn => _isPlayerTurn;
    public int CurrentTurn => currentState?.currentTurn ?? 0;
    public int CurrentYear => currentState?.currentYear ?? 0;

    void Awake()
    {
        Instance = this;
        provinceManager = new ProvinceManager();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartNewCampaign(string playerFactionId)
    {
        ProvinceDatabase.Initialize();
        UnitDatabase.Initialize();

        if (provinceManager == null)
            provinceManager = new ProvinceManager();

        var provinces = ProvinceDatabase.GetAllProvinces();
        provinceManager.Initialize(provinces);

        _rng = new BattleRandom(System.Environment.TickCount);
        currentState = new CampaignState
        {
            campaignId = System.Guid.NewGuid().ToString(),
            playerFactionId = playerFactionId,
            currentTurn = 1,
            currentYear = 1,
            factions = new List<FactionCampaignState>(),
            provinces = new List<ProvinceState>(),
            isComplete = false
        };

        foreach (var faction in FactionDatabase.GetAll())
        {
            var fcs = new FactionCampaignState
            {
                factionId = faction.id,
                gold = 500,
                isEliminated = false,
                armyComposition = new SerializableKeyValueList()
            };

            int totalUnits = Mathf.Max(1, faction.estimatedMilitary / GameConfig.UnitBudgetScaleFactor);
            var unitTypes = faction.unitTypes;
            if (unitTypes != null && unitTypes.Count > 0)
            {
                int perType = Mathf.Max(1, totalUnits / unitTypes.Count);
                var dict = new Dictionary<string, int>();
                foreach (var ut in unitTypes)
                {
                    if (ut != null && !string.IsNullOrEmpty(ut.id))
                        dict[ut.id] = perType;
                }
                fcs.armyComposition.FromDictionary(dict);
            }

            currentState.factions.Add(fcs);
        }

        foreach (var p in provinces)
        {
            currentState.provinces.Add(new ProvinceState
            {
                provinceId = p.id,
                ownerFactionId = p.ownerFactionId,
                garrison = p.garrison
            });
        }

        SyncProvinceStateToManager();
        _isPlayerTurn = true;
        _pendingPlayerAction = null;

        if (CampaignAutoSave.Instance == null)
        {
            var autoSaveObj = new GameObject("CampaignAutoSave");
            autoSaveObj.AddComponent<CampaignAutoSave>();
        }
        EventBus.Publish(new CampaignPlayerTurnStartedEvent { turn = CurrentTurn, year = CurrentYear });
    }

    public void LoadCampaign(CampaignState state)
    {
        if (state == null) return;

        ProvinceDatabase.Initialize();
        UnitDatabase.Initialize();

        var provinces = ProvinceDatabase.GetAllProvinces();
        provinceManager.Initialize(provinces);

        currentState = state;
        _rng = new BattleRandom(System.Environment.TickCount);

        foreach (var ps in currentState.provinces)
        {
            provinceManager.TransferProvince(ps.provinceId, ps.ownerFactionId);
        }

        _isPlayerTurn = true;
        _pendingPlayerAction = null;
        EventBus.Publish(new CampaignLoadedEvent { campaignId = currentState.campaignId });
    }

    public CampaignState GetCurrentState()
    {
        return currentState;
    }

    public void BeginPlayerTurn()
    {
        _isPlayerTurn = true;
        _pendingPlayerAction = null;
        EventBus.Publish(new CampaignPlayerTurnStartedEvent { turn = CurrentTurn, year = CurrentYear });
    }

    public void SubmitPlayerAction(CampaignAction action)
    {
        if (!_isPlayerTurn || action == null) return;
        if (action.sourceFactionId != currentState.playerFactionId) return;
        _pendingPlayerAction = action;
    }

    public void ExecuteTurn()
    {
        if (currentState == null || currentState.isComplete) return;

        var results = new List<BattleResult>();

        if (_pendingPlayerAction != null && _pendingPlayerAction.type == CampaignAction.ActionType.Attack)
        {
            ProcessAttack(_pendingPlayerAction, results);
        }
        else if (_pendingPlayerAction != null && _pendingPlayerAction.type == CampaignAction.ActionType.Recruit)
        {
            var player = GetFactionState(currentState.playerFactionId);
            if (player != null)
                CampaignEconomyManager.Recruit(player, _pendingPlayerAction.recruitUnitTypeId, _pendingPlayerAction.recruitCount);
        }

        var aiFactions = currentState.factions
            .Where(f => !f.isEliminated && f.factionId != currentState.playerFactionId)
            .ToList();

        foreach (var ai in aiFactions)
        {
            var action = CampaignAI.DecideAction(ai, currentState, provinceManager, _rng);
            if (action == null) continue;

            if (action.type == CampaignAction.ActionType.Attack)
            {
                ProcessAttack(action, results);
            }
            else if (action.type == CampaignAction.ActionType.Recruit)
            {
                CampaignEconomyManager.Recruit(ai, action.recruitUnitTypeId, action.recruitCount);
            }
        }

        foreach (var r in results)
        {
            EventBus.Publish(new CampaignBattleAutoResolvedEvent { result = r });
        }

        SyncProvinceStateToManager();

        foreach (var f in currentState.factions.Where(f => !f.isEliminated))
        {
            CampaignEconomyManager.CollectIncome(f, provinceManager);
        }
        foreach (var f in currentState.factions.Where(f => !f.isEliminated))
        {
            CampaignEconomyManager.PayUpkeep(f);
        }

        currentState.currentTurn++;
        currentState.currentYear = 1 + (currentState.currentTurn - 1) / 4;

        _pendingPlayerAction = null;
        _isPlayerTurn = true;

        EventBus.Publish(new CampaignTurnResolvedEvent { turn = currentState.currentTurn, results = results });

        if (CheckVictoryConditions())
        {
            currentState.isComplete = true;
            currentState.winnerId = GetLeadingFaction();
            EventBus.Publish(new CampaignWonEvent { factionId = currentState.winnerId, finalTurn = currentState.currentTurn });
        }
        else
        {
            BeginPlayerTurn();
        }

        CampaignAutoSave.Instance?.OnTurnResolved();
    }

    public bool CheckVictoryConditions()
    {
        int total = ProvinceDatabase.GetAllProvinces().Count;
        if (total == 0) return false;

        var alive = currentState.factions.Count(f => !f.isEliminated);
        if (alive <= 1) return true;

        foreach (var f in currentState.factions.Where(f => !f.isEliminated))
        {
            int owned = provinceManager.GetProvincesForFaction(f.factionId).Count;
            if (owned >= total * 0.7f)
                return true;
        }
        return false;
    }

    private void ProcessAttack(CampaignAction action, List<BattleResult> results)
    {
        var attacker = GetFactionState(action.sourceFactionId);
        var province = provinceManager.GetProvince(action.targetProvinceId);
        var defenderId = provinceManager.GetOwner(action.targetProvinceId);
        var defender = GetFactionState(defenderId);

        if (attacker == null || province == null || string.IsNullOrEmpty(defenderId)) return;

        var result = CampaignBattleResolver.ResolveAutomatic(action, attacker, defender ?? new FactionCampaignState { factionId = defenderId }, province, _rng);

        results.Add(result);

        if (result.winnerFactionId == attacker.factionId)
        {
            DeductCommittedUnits(attacker, action.committedUnits, result.winnerSurvivors, result.winnerStartCount);
            provinceManager.TransferProvince(action.targetProvinceId, attacker.factionId);
            EventBus.Publish(new CampaignProvinceTransferredEvent { provinceId = action.targetProvinceId, oldOwner = defenderId, newOwner = attacker.factionId });

            if (defender != null)
            {
                var defUnitsDict = CampaignAI.AllocateArmy(defender, 0.5f);
                var defUnits = new SerializableKeyValueList();
                defUnits.FromDictionary(defUnitsDict);
                DeductCommittedUnits(defender, defUnits, 0, CountUnits(defUnitsDict));
                if (GetTotalUnits(defender) <= 0 && provinceManager.GetProvincesForFaction(defenderId).Count == 0)
                {
                    defender.isEliminated = true;
                    EventBus.Publish(new CampaignFactionEliminatedEvent { factionId = defenderId, eliminatedBy = attacker.factionId });
                }
            }
        }
        else
        {
            DeductCommittedUnits(attacker, action.committedUnits, 0, CountUnits(action.committedUnits?.ToDictionary()));
            if (GetTotalUnits(attacker) <= 0 && provinceManager.GetProvincesForFaction(attacker.factionId).Count == 0)
            {
                attacker.isEliminated = true;
                EventBus.Publish(new CampaignFactionEliminatedEvent { factionId = attacker.factionId, eliminatedBy = defenderId });
            }
        }
    }

    private void DeductCommittedUnits(FactionCampaignState faction, SerializableKeyValueList committed, int survivors, int startCount)
    {
        var dict = faction.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        var committedDict = committed?.ToDictionary() ?? new Dictionary<string, int>();

        foreach (var kv in committedDict)
        {
            if (!dict.TryGetValue(kv.Key, out var have)) continue;
            int lost = startCount > 0 ? Mathf.RoundToInt(kv.Value * (1f - (float)survivors / startCount)) : kv.Value;
            dict[kv.Key] = Mathf.Max(0, have - lost);
        }
        faction.armyComposition.FromDictionary(dict);
    }

    private int CountUnits(Dictionary<string, int> units)
    {
        if (units == null) return 0;
        return units.Values.Sum();
    }

    private int GetTotalUnits(FactionCampaignState faction)
    {
        var dict = faction?.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        return dict.Values.Sum();
    }

    private void SyncProvinceStateToManager()
    {
        if (currentState?.provinces == null) return;
        foreach (var ps in currentState.provinces)
        {
            ps.ownerFactionId = provinceManager.GetOwner(ps.provinceId);
        }
    }

    private FactionCampaignState GetFactionState(string factionId)
    {
        return currentState?.factions?.FirstOrDefault(f => f.factionId == factionId);
    }

    private string GetLeadingFaction()
    {
        string best = null;
        int bestCount = 0;
        foreach (var f in currentState.factions.Where(f => !f.isEliminated))
        {
            int c = provinceManager.GetProvincesForFaction(f.factionId).Count;
            if (c > bestCount) { bestCount = c; best = f.factionId; }
        }
        return best ?? currentState.playerFactionId;
    }
}
