using System.Threading.Tasks;
using UnityEngine;

public class CampaignAutoSave : MonoBehaviour
{
    public static CampaignAutoSave Instance { get; set; }

    IPersistenceService persistence;

    void Awake()
    {
        Instance = this;
        persistence = new LocalPersistenceService();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void OnTurnResolved()
    {
        var state = CampaignManager.Instance?.currentState;
        if (state != null)
            SaveAsync(state);
    }

    async void SaveAsync(CampaignState state)
    {
        try
        {
            await persistence.SaveCampaignState(state);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
