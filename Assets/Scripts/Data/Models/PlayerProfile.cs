using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProfile
{
    public string playerId;
    public string displayName;
    public int totalCampaignsWon;
    public string favoriteFactionId;
    public List<string> campaignHistory = new List<string>();
}
