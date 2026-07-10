using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks living units per side and publishes defeat events.
/// Supports both Attacker/Defender (new) and North/South (legacy) naming.
/// </summary>
public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance { get; set; }

    public List<Unit> attackerUnits = new List<Unit>();
    public List<Unit> defenderUnits = new List<Unit>();

    public System.Action<Faction> OnFactionDefeated;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterUnit(Unit unit)
    {
        if (IsAttackerSide(unit.faction))
            attackerUnits.Add(unit);
        else
            defenderUnits.Add(unit);
    }

    public void OnUnitDied(Unit unit)
    {
        if (IsAttackerSide(unit.faction))
            attackerUnits.Remove(unit);
        else
            defenderUnits.Remove(unit);

        attackerUnits.RemoveAll(u => u == null || u.isDead);
        defenderUnits.RemoveAll(u => u == null || u.isDead);

        if (attackerUnits.Count == 0)
            OnFactionDefeated?.Invoke(unit.faction);
        else if (defenderUnits.Count == 0)
            OnFactionDefeated?.Invoke(unit.faction);
    }

    public List<Unit> GetUnitsForFaction(Faction faction)
    {
        var list = IsAttackerSide(faction) ? attackerUnits : defenderUnits;
        return list.Where(u => u != null && !u.isDead).ToList();
    }

    public int GetLivingCount(Faction faction)
    {
        var list = IsAttackerSide(faction) ? attackerUnits : defenderUnits;
        return list.Count(u => u != null && !u.isDead);
    }

    /// <summary>
    /// Maps both new (Attacker) and legacy (North) identifiers to the attacker side.
    /// </summary>
    static bool IsAttackerSide(Faction f) =>
        f == Faction.Attacker || f == Faction.North;
}
