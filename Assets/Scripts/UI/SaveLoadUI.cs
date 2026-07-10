using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    GameObject scrollContent;
    IPersistenceService persistence;

    public void Show()
    {
        if (root != null) Hide();

        persistence = new LocalPersistenceService();
        canvas = UIThemeManager.CreateCanvas("SaveLoadCanvas", 20);
        root = new GameObject("SaveLoadRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var title = UIThemeManager.CreateText(root.transform, "Title", "Campaigns", new Vector2(0, 280), new Vector2(600, 50), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, 280);

        var scrollView = UIThemeManager.CreateScrollView(root.transform, "CampaignScroll", new Vector2(0, -80), new Vector2(600, 450));
        var scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.pivot = new Vector2(0.5f, 0.5f);
        scrollRect.anchoredPosition = new Vector2(0, -50);

        scrollContent = scrollView.transform.Find("Viewport/Content")?.gameObject;
        if (scrollContent == null) scrollContent = scrollView;

        var newBtn = UIThemeManager.CreateButton(root.transform, "NewCampaign", "New Campaign", new Vector2(0, -280), new Vector2(220, 50), OnNewCampaign);
        var newRect = newBtn.GetComponent<RectTransform>();
        newRect.anchorMin = new Vector2(0.5f, 0f);
        newRect.anchorMax = new Vector2(0.5f, 0f);
        newRect.pivot = new Vector2(0.5f, 0.5f);
        newRect.anchoredPosition = new Vector2(0, -280);

        RefreshListAsync();
    }

    async void RefreshListAsync()
    {
        try
        {
            var campaigns = await persistence.ListCampaigns();
            PopulateList(campaigns ?? new List<CampaignState>());
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void PopulateList(List<CampaignState> campaigns)
    {
        if (scrollContent == null) return;

        foreach (Transform child in scrollContent.transform)
            Destroy(child.gameObject);

        foreach (var state in campaigns)
            CreateCampaignCard(state);
    }

    GameObject CreateCampaignCard(CampaignState state)
    {
        if (state == null || string.IsNullOrEmpty(state.campaignId)) return null;

        var faction = FactionDatabase.Get(state.playerFactionId);
        var factionName = faction?.displayName ?? state.playerFactionId ?? "Unknown";
        var info = $"{factionName} — Turn {state.currentTurn}, Year {state.currentYear}";

        var card = UIThemeManager.CreatePanel(scrollContent.transform, "Card_" + state.campaignId, Vector2.zero, new Vector2(0, 64));
        var layout = card.AddComponent<LayoutElement>();
        layout.preferredHeight = 64;
        layout.minHeight = 64;

        UIThemeManager.CreateText(card.transform, "Info", info, new Vector2(-180, 0), new Vector2(400, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);

        var loadBtn = UIThemeManager.CreateButton(card.transform, "Load", "Load", new Vector2(180, 0), new Vector2(80, 36), () => OnLoadSlot(state.campaignId));
        var loadRect = loadBtn.GetComponent<RectTransform>();
        loadRect.anchoredPosition = new Vector2(180, 0);

        var deleteBtn = UIThemeManager.CreateButton(card.transform, "Delete", "Delete", new Vector2(270, 0), new Vector2(80, 36), () => OnDeleteSlot(state.campaignId));
        var deleteRect = deleteBtn.GetComponent<RectTransform>();
        deleteRect.anchoredPosition = new Vector2(270, 0);

        return card;
    }

    public void Hide()
    {
        if (root != null)
        {
            Destroy(root);
            root = null;
        }
        if (canvas != null)
        {
            Destroy(canvas.gameObject);
            canvas = null;
        }
        scrollContent = null;
    }

    void OnLoadSlot(string campaignId)
    {
        if (string.IsNullOrEmpty(campaignId)) return;
        LoadAndTransitionAsync(campaignId);
    }

    async void LoadAndTransitionAsync(string campaignId)
    {
        try
        {
            var state = await persistence.LoadCampaignState(campaignId);
            if (state == null) return;

            if (CampaignManager.Instance == null)
            {
                var cmObj = new GameObject("CampaignManager");
                cmObj.AddComponent<CampaignManager>();
            }

            CampaignManager.Instance.LoadCampaign(state);
            GameManager.Instance?.TransitionTo(GameFlowState.CampaignMap);
            Hide();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void OnDeleteSlot(string campaignId)
    {
        if (string.IsNullOrEmpty(campaignId)) return;
        DeleteAndRefreshAsync(campaignId);
    }

    async void DeleteAndRefreshAsync(string campaignId)
    {
        try
        {
            await persistence.DeleteCampaign(campaignId);
            RefreshListAsync();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void OnNewCampaign()
    {
        GameManager.Instance?.TransitionTo(GameFlowState.CampaignSetup);
        Hide();
    }
}
