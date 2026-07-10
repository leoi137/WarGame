using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPersistenceService
{
    Task<bool> SaveCampaignState(CampaignState state);
    Task<CampaignState> LoadCampaignState(string campaignId);
    Task<List<CampaignState>> ListCampaigns();
    Task<bool> DeleteCampaign(string campaignId);
    Task<bool> SavePlayerProfile(PlayerProfile profile);
    Task<PlayerProfile> LoadPlayerProfile(string playerId);
}
