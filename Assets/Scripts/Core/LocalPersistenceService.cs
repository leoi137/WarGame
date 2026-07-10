using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LocalPersistenceService : IPersistenceService
{
    private readonly string _campaignsPath;
    private readonly string _profilesPath;

    public LocalPersistenceService()
    {
        _campaignsPath = Path.Combine(Application.persistentDataPath, "campaigns");
        _profilesPath = Path.Combine(Application.persistentDataPath, "profiles");
    }

    private static string SanitizeFileName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "unnamed";
        var invalid = Path.GetInvalidFileNameChars();
        var chars = id.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
            if (Array.IndexOf(invalid, chars[i]) >= 0)
                chars[i] = '_';
        return new string(chars);
    }

    public Task<bool> SaveCampaignState(CampaignState state)
    {
        return Task.Run(() =>
        {
            try
            {
                if (state == null || string.IsNullOrEmpty(state.campaignId)) return false;
                Directory.CreateDirectory(_campaignsPath);
                var path = Path.Combine(_campaignsPath, SanitizeFileName(state.campaignId) + ".json");
                var json = JsonUtility.ToJson(state, true);
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public Task<CampaignState> LoadCampaignState(string campaignId)
    {
        return Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(campaignId)) return null;
                var path = Path.Combine(_campaignsPath, SanitizeFileName(campaignId) + ".json");
                if (!File.Exists(path)) return null;
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<CampaignState>(json);
            }
            catch
            {
                return null;
            }
        });
    }

    public Task<List<CampaignState>> ListCampaigns()
    {
        return Task.Run(() =>
        {
            var list = new List<CampaignState>();
            try
            {
                if (!Directory.Exists(_campaignsPath)) return list;
                foreach (var file in Directory.GetFiles(_campaignsPath, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var state = JsonUtility.FromJson<CampaignState>(json);
                        if (state != null) list.Add(state);
                    }
                    catch
                    {
                        // Skip corrupted files
                    }
                }
            }
            catch { }
            return list;
        });
    }

    public Task<bool> DeleteCampaign(string campaignId)
    {
        return Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(campaignId)) return false;
                var path = Path.Combine(_campaignsPath, SanitizeFileName(campaignId) + ".json");
                if (!File.Exists(path)) return true;
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public Task<bool> SavePlayerProfile(PlayerProfile profile)
    {
        return Task.Run(() =>
        {
            try
            {
                if (profile == null || string.IsNullOrEmpty(profile.playerId)) return false;
                Directory.CreateDirectory(_profilesPath);
                var path = Path.Combine(_profilesPath, SanitizeFileName(profile.playerId) + ".json");
                var json = JsonUtility.ToJson(profile, true);
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public Task<PlayerProfile> LoadPlayerProfile(string playerId)
    {
        return Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(playerId)) return null;
                var path = Path.Combine(_profilesPath, SanitizeFileName(playerId) + ".json");
                if (!File.Exists(path)) return null;
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<PlayerProfile>(json);
            }
            catch
            {
                return null;
            }
        });
    }
}
