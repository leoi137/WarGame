using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FactionSelectUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    GameObject scrollContent;
    List<FactionDefinition> displayedFactions = new();
    bool selectingAttacker = true;
    System.Action<FactionDefinition> onFactionSelected;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show(bool selectingAttacker, System.Action<FactionDefinition> onSelected = null)
    {
        this.selectingAttacker = selectingAttacker;
        this.onFactionSelected = onSelected;

        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("FactionSelectCanvas", 10);
        root = new GameObject("FactionSelectRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CreateRegionTabs();
        var scrollView = UIThemeManager.CreateScrollView(root.transform, "FactionScroll", new Vector2(0, -80), new Vector2(700, 500));
        scrollContent = scrollView.transform.Find("Viewport/Content")?.gameObject;
        if (scrollContent == null) scrollContent = scrollView;

        PopulateFactionList(FactionDatabase.GetAll());
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
        displayedFactions.Clear();
    }

    public void PopulateFactionList(List<FactionDefinition> factions)
    {
        displayedFactions = new List<FactionDefinition>(factions);
        if (scrollContent == null) return;

        foreach (Transform child in scrollContent.transform)
            Destroy(child.gameObject);

        foreach (var f in displayedFactions)
            CreateFactionCard(f, scrollContent.transform);
    }

    public void CreateFactionCard(FactionDefinition faction, Transform parent)
    {
        var card = UIThemeManager.CreatePanel(parent, "FactionCard_" + faction.id, Vector2.zero, new Vector2(0, 60));
        var layout = card.AddComponent<LayoutElement>();
        layout.preferredHeight = 60;
        layout.minHeight = 60;

        var flagBar = new GameObject("FlagBar");
        flagBar.transform.SetParent(card.transform, false);
        var flagRect = flagBar.AddComponent<RectTransform>();
        flagRect.anchorMin = new Vector2(0, 0);
        flagRect.anchorMax = new Vector2(0, 1);
        flagRect.pivot = new Vector2(0, 0.5f);
        flagRect.anchoredPosition = Vector2.zero;
        flagRect.sizeDelta = new Vector2(8, 0);
        flagRect.offsetMin = new Vector2(0, 4);
        flagRect.offsetMax = new Vector2(8, -4);
        var flagImg = flagBar.AddComponent<Image>();
        flagImg.color = faction.primaryColor;

        var nameText = UIThemeManager.CreateText(card.transform, "Name", faction.displayName, new Vector2(50, 15), new Vector2(250, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleLeft);
        var rect = nameText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchoredPosition = new Vector2(20, 15);

        var regionText = UIThemeManager.CreateText(card.transform, "Region", faction.region.ToString(), new Vector2(50, -15), new Vector2(150, 18), UIThemeManager.SmallFontSize, TextAnchor.MiddleLeft);
        var regionRect = regionText.GetComponent<RectTransform>();
        regionRect.anchorMin = new Vector2(0, 0.5f);
        regionRect.anchorMax = new Vector2(0, 0.5f);
        regionRect.pivot = new Vector2(0, 0.5f);
        regionRect.anchoredPosition = new Vector2(20, -15);
        regionText.GetComponent<Text>().color = UIThemeManager.TextSecondary;

        var milText = UIThemeManager.CreateText(card.transform, "Military", "~" + faction.estimatedMilitary, new Vector2(-20, 0), new Vector2(80, 24), UIThemeManager.BodyFontSize, TextAnchor.MiddleRight);
        var milRect = milText.GetComponent<RectTransform>();
        milRect.anchorMin = new Vector2(1, 0.5f);
        milRect.anchorMax = new Vector2(1, 0.5f);
        milRect.pivot = new Vector2(1, 0.5f);
        milRect.anchoredPosition = new Vector2(-20, 0);

        var btn = card.AddComponent<Button>();
        btn.onClick.AddListener(() => OnFactionSelected(faction));
    }

    public void FilterByRegion(Region region)
    {
        PopulateFactionList(FactionDatabase.GetByRegion(region));
    }

    public void OnFactionSelected(FactionDefinition faction)
    {
        ShowFactionPreview(faction);
        onFactionSelected?.Invoke(faction);
        if (GameManager.Instance != null)
        {
            if (selectingAttacker)
            {
                GameManager.Instance.SelectedAttacker = faction;
                Hide();
            }
            else
            {
                GameManager.Instance.SelectedDefender = faction;
                GameManager.Instance.StartQuickBattle(GameManager.Instance.SelectedAttacker, faction);
                Hide();
            }
        }
    }

    public void CreateRegionTabs()
    {
        var tabPanel = UIThemeManager.CreatePanel(root.transform, "RegionTabs", new Vector2(0, 280), new Vector2(800, 40));
        float x = -350;
        foreach (Region r in System.Enum.GetValues(typeof(Region)))
        {
            var tab = UIThemeManager.CreateButton(tabPanel.transform, "Tab_" + r, r.ToString(), new Vector2(x, 0), new Vector2(80, 32), () => FilterByRegion(r));
            x += 90;
        }
    }

    public void ShowFactionPreview(FactionDefinition faction)
    {
        var existing = root.transform.Find("PreviewPanel");
        if (existing != null) Destroy(existing.gameObject);

        var preview = UIThemeManager.CreatePanel(root.transform, "PreviewPanel", new Vector2(450, 0), new Vector2(280, 200));
        UIThemeManager.CreateText(preview.transform, "PreviewName", faction.displayName, new Vector2(0, 70), new Vector2(260, 30), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(preview.transform, "PreviewRegion", faction.region.ToString(), new Vector2(0, 40), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(preview.transform, "PreviewMilitary", "Military: ~" + faction.estimatedMilitary, new Vector2(0, 10), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
    }
}
