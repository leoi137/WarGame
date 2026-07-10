using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CampaignSetupUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    GameObject scrollContent;
    GameObject detailsPanel;
    string selectedFactionId;
    Dictionary<string, GameObject> factionCards = new Dictionary<string, GameObject>();

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show()
    {
        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("CampaignSetupCanvas", 20);
        root = new GameObject("CampaignSetupRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var title = UIThemeManager.CreateText(root.transform, "Title", "Campaign - Choose Your Faction", new Vector2(0, 280), new Vector2(800, 50), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, 280);

        var scrollView = UIThemeManager.CreateScrollView(root.transform, "FactionScroll", new Vector2(0, -80), new Vector2(500, 450));
        var scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.pivot = new Vector2(0.5f, 0.5f);
        scrollRect.anchoredPosition = new Vector2(-150, -50);

        scrollContent = scrollView.transform.Find("Viewport/Content")?.gameObject;
        if (scrollContent == null) scrollContent = scrollView;

        PopulateFactionList();

        detailsPanel = UIThemeManager.CreatePanel(root.transform, "DetailsPanel", new Vector2(250, -50), new Vector2(280, 350));
        var detailsRect = detailsPanel.GetComponent<RectTransform>();
        detailsRect.anchorMin = new Vector2(0.5f, 0.5f);
        detailsRect.anchorMax = new Vector2(0.5f, 0.5f);
        detailsRect.pivot = new Vector2(0.5f, 0.5f);
        detailsRect.anchoredPosition = new Vector2(250, -50);
        detailsPanel.SetActive(false);

        var startBtn = UIThemeManager.CreateButton(root.transform, "StartCampaign", "Start Campaign", new Vector2(0, -280), new Vector2(220, 50), null);
        var startRect = startBtn.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0f);
        startRect.anchorMax = new Vector2(0.5f, 0f);
        startRect.pivot = new Vector2(0.5f, 0.5f);
        startRect.anchoredPosition = new Vector2(0, -280);
        startBtn.GetComponent<Button>().interactable = false;
        startBtn.GetComponent<Button>().onClick.AddListener(OnStartCampaign);
        _startButton = startBtn.GetComponent<Button>();
    }

    Button _startButton;

    void PopulateFactionList()
    {
        factionCards.Clear();
        foreach (Transform child in scrollContent.transform)
            Destroy(child.gameObject);

        foreach (var f in FactionDatabase.GetAll())
        {
            var card = CreateFactionCard(f);
            factionCards[f.id] = card;
        }
    }

    GameObject CreateFactionCard(FactionDefinition faction)
    {
        var card = UIThemeManager.CreatePanel(scrollContent.transform, "FactionCard_" + faction.id, Vector2.zero, new Vector2(0, 56));
        var layout = card.AddComponent<LayoutElement>();
        layout.preferredHeight = 56;
        layout.minHeight = 56;

        var flagBar = new GameObject("FlagBar");
        flagBar.transform.SetParent(card.transform, false);
        var flagRect = flagBar.AddComponent<RectTransform>();
        flagRect.anchorMin = new Vector2(0, 0);
        flagRect.anchorMax = new Vector2(0, 1);
        flagRect.pivot = new Vector2(0, 0.5f);
        flagRect.anchoredPosition = Vector2.zero;
        flagRect.sizeDelta = new Vector2(6, 0);
        flagRect.offsetMin = new Vector2(0, 4);
        flagRect.offsetMax = new Vector2(6, -4);
        flagBar.AddComponent<Image>().color = faction.primaryColor;

        var nameText = UIThemeManager.CreateText(card.transform, "Name", faction.displayName, new Vector2(20, 12), new Vector2(200, 22), UIThemeManager.HeaderFontSize, TextAnchor.MiddleLeft);
        var nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(0, 0.5f);
        nameRect.pivot = new Vector2(0, 0.5f);
        nameRect.anchoredPosition = new Vector2(20, 12);

        var regionText = UIThemeManager.CreateText(card.transform, "Region", faction.region.ToString(), new Vector2(20, -12), new Vector2(150, 18), UIThemeManager.SmallFontSize, TextAnchor.MiddleLeft);
        var regionRect = regionText.GetComponent<RectTransform>();
        regionRect.anchorMin = new Vector2(0, 0.5f);
        regionRect.anchorMax = new Vector2(0, 0.5f);
        regionRect.pivot = new Vector2(0, 0.5f);
        regionRect.anchoredPosition = new Vector2(20, -12);
        regionText.GetComponent<Text>().color = UIThemeManager.TextSecondary;

        var btn = card.AddComponent<Button>();
        btn.onClick.AddListener(() => OnFactionSelected(faction.id));

        var trigger = card.AddComponent<EventTrigger>();
        var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => ShowFactionDetails(faction));
        trigger.triggers.Add(enter);
        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => HideFactionDetails());
        trigger.triggers.Add(exit);

        return card;
    }

    void ShowFactionDetails(FactionDefinition faction)
    {
        if (detailsPanel == null) return;
        detailsPanel.SetActive(true);
        foreach (Transform c in detailsPanel.transform)
            Destroy(c.gameObject);

        UIThemeManager.CreateText(detailsPanel.transform, "Name", faction.displayName, new Vector2(0, 120), new Vector2(260, 28), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(detailsPanel.transform, "Region", faction.region.ToString(), new Vector2(0, 85), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(detailsPanel.transform, "Military", "Military: ~" + faction.estimatedMilitary, new Vector2(0, 55), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        if (faction.cities != null)
            UIThemeManager.CreateText(detailsPanel.transform, "Cities", "Provinces: " + faction.cities.Count, new Vector2(0, 25), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
    }

    void HideFactionDetails()
    {
        if (detailsPanel != null && string.IsNullOrEmpty(selectedFactionId))
            detailsPanel.SetActive(false);
    }

    public void OnFactionSelected(string factionId)
    {
        selectedFactionId = factionId;

        foreach (var kv in factionCards)
        {
            var img = kv.Value.GetComponent<Image>();
            if (img != null)
                img.color = kv.Key == factionId ? UIThemeManager.ButtonHover : UIThemeManager.PanelBackground;
        }

        var faction = FactionDatabase.Get(factionId);
        if (faction != null)
        {
            if (detailsPanel != null)
            {
                detailsPanel.SetActive(true);
                foreach (Transform c in detailsPanel.transform)
                    Destroy(c.gameObject);
                UIThemeManager.CreateText(detailsPanel.transform, "Name", faction.displayName, new Vector2(0, 120), new Vector2(260, 28), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
                UIThemeManager.CreateText(detailsPanel.transform, "Region", faction.region.ToString(), new Vector2(0, 85), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
                UIThemeManager.CreateText(detailsPanel.transform, "Military", "Military: ~" + faction.estimatedMilitary, new Vector2(0, 55), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
                if (faction.cities != null)
                    UIThemeManager.CreateText(detailsPanel.transform, "Cities", "Provinces: " + faction.cities.Count, new Vector2(0, 25), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
            }
        }

        if (_startButton != null)
            _startButton.interactable = true;
    }

    public void OnStartCampaign()
    {
        if (string.IsNullOrEmpty(selectedFactionId)) return;

        if (CampaignManager.Instance == null)
        {
            var cmObj = new GameObject("CampaignManager");
            cmObj.AddComponent<CampaignManager>();
        }

        CampaignManager.Instance.StartNewCampaign(selectedFactionId);
        GameManager.Instance?.TransitionTo(GameFlowState.CampaignMap);
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
        detailsPanel = null;
        factionCards.Clear();
        selectedFactionId = null;
    }
}
