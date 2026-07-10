using System;
using System.Collections.Generic;

/// <summary>
/// Lightweight in-process pub/sub event bus.
/// Designed for low-allocation usage across gameplay systems.
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> HandlersByType = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        if (handler == null) return;

        var eventType = typeof(T);
        if (!HandlersByType.TryGetValue(eventType, out var handlers))
        {
            handlers = new List<Delegate>(4);
            HandlersByType[eventType] = handlers;
        }

        if (!handlers.Contains(handler))
        {
            handlers.Add(handler);
        }
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        if (handler == null) return;

        var eventType = typeof(T);
        if (!HandlersByType.TryGetValue(eventType, out var handlers)) return;

        handlers.Remove(handler);
        if (handlers.Count == 0)
        {
            HandlersByType.Remove(eventType);
        }
    }

    public static void Publish<T>(T eventData)
    {
        var eventType = typeof(T);
        if (!HandlersByType.TryGetValue(eventType, out var handlers)) return;

        var count = handlers.Count;
        if (count == 0) return;

        var snapshot = new Delegate[count];
        handlers.CopyTo(snapshot, 0);

        for (var i = 0; i < snapshot.Length; i++)
        {
            if (snapshot[i] is Action<T> callback)
            {
                callback(eventData);
            }
        }
    }

    public static void Clear()
    {
        HandlersByType.Clear();
    }
}

public struct BattleStartedEvent { public BattleConfiguration config; }
public struct BattleEndedEvent { public BattleResult result; }
public struct UnitDiedEvent { public Unit unit; }
public struct UnitDamagedEvent { public Unit unit; public float damage; }
public struct FactionSelectedEvent { public FactionDefinition faction; }
public struct GameStateChangedEvent { public GameFlowState oldState; public GameFlowState newState; }
public struct PlacementConfirmedEvent { public Faction side; }
public struct AbilityActivatedEvent { public Unit unit; public AbilityDefinition ability; }

public struct CampaignPlayerTurnStartedEvent { public int turn; public int year; }
public struct CampaignTurnResolvedEvent { public int turn; public List<BattleResult> results; }
public struct CampaignProvinceTransferredEvent { public string provinceId; public string oldOwner; public string newOwner; }
public struct CampaignBattleStartedEvent { public string attackerFactionId; public string defenderFactionId; public string provinceId; }
public struct CampaignBattleAutoResolvedEvent { public BattleResult result; }
public struct CampaignWonEvent { public string factionId; public int finalTurn; }
public struct CampaignLostEvent { public string factionId; public int finalTurn; }
public struct CampaignFactionEliminatedEvent { public string factionId; public string eliminatedBy; }
public struct CampaignSavedEvent { public string campaignId; }
public struct CampaignLoadedEvent { public string campaignId; }
