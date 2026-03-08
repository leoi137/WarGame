using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JsonUtility cannot serialize Dictionary; this wrapper stores key-value pairs for persistence.
/// </summary>
[Serializable]
public class SerializableKeyValueList
{
    public List<string> keys = new List<string>();
    public List<int> values = new List<int>();

    public Dictionary<string, int> ToDictionary()
    {
        var dict = new Dictionary<string, int>();
        for (int i = 0; i < keys.Count && i < values.Count; i++)
            dict[keys[i]] = values[i];
        return dict;
    }

    public void FromDictionary(Dictionary<string, int> dict)
    {
        keys.Clear();
        values.Clear();
        if (dict == null) return;
        foreach (var kv in dict)
        {
            keys.Add(kv.Key);
            values.Add(kv.Value);
        }
    }
}

[Serializable]
public class CampaignState
{
    public string campaignId;
    public string playerFactionId;
    public int currentTurn;
    public int currentYear;
    public List<FactionCampaignState> factions = new List<FactionCampaignState>();
    public List<ProvinceState> provinces = new List<ProvinceState>();
    public bool isComplete;
    public string winnerId;
}

[Serializable]
public class FactionCampaignState
{
    public string factionId;
    public int gold;
    public bool isEliminated;
    public SerializableKeyValueList armyComposition = new SerializableKeyValueList();
}

[Serializable]
public class ProvinceState
{
    public string provinceId;
    public string ownerFactionId;
    public int garrison;
}

[Serializable]
public class CampaignAction
{
    public enum ActionType { Attack, Defend, Recruit, Reinforce }
    public ActionType type;
    public string sourceFactionId;
    public string targetProvinceId;
    public SerializableKeyValueList committedUnits = new SerializableKeyValueList();
    public string recruitUnitTypeId;
    public int recruitCount;
}
