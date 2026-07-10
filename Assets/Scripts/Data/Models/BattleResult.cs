using System;
using System.Collections.Generic;

[Serializable]
public class BattleResult
{
    public string winnerFactionId;
    public string loserFactionId;
    public Faction winningSide;
    public int winnerSurvivors;
    public int winnerStartCount;
    public int loserStartCount;
    public float battleDurationSeconds;
    public int totalCasualties;
    public Dictionary<string, int> attackerUnitLosses = new();
    public Dictionary<string, int> defenderUnitLosses = new();
    public Dictionary<string, int> attackerUnitKills = new();
    public Dictionary<string, int> defenderUnitKills = new();
}
