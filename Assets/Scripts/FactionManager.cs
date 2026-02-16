using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance { get; set; }

    public List<Unit> northUnits = new List<Unit>();
    public List<Unit> southUnits = new List<Unit>();

    public System.Action<Faction> OnFactionDefeated;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit.faction == Faction.North)
            northUnits.Add(unit);
        else
            southUnits.Add(unit);
    }

    public void OnUnitDied(Unit unit)
    {
        if (unit.faction == Faction.North)
            northUnits.Remove(unit);
        else
            southUnits.Remove(unit);

        // Check win condition
        northUnits.RemoveAll(u => u == null || u.isDead);
        southUnits.RemoveAll(u => u == null || u.isDead);

        if (northUnits.Count == 0)
        {
            OnFactionDefeated?.Invoke(Faction.North);
        }
        else if (southUnits.Count == 0)
        {
            OnFactionDefeated?.Invoke(Faction.South);
        }
    }

    public List<Unit> GetUnitsForFaction(Faction faction)
    {
        if (faction == Faction.North)
            return northUnits.Where(u => u != null && !u.isDead).ToList();
        else
            return southUnits.Where(u => u != null && !u.isDead).ToList();
    }

    public int GetLivingCount(Faction faction)
    {
        if (faction == Faction.North)
            return northUnits.Count(u => u != null && !u.isDead);
        else
            return southUnits.Count(u => u != null && !u.isDead);
    }
}
