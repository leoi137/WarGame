using UnityEngine;
using UnityEngine.UI;

public class WorldMapHUD : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    Button battleButton;
    FactionDefinition selectedAttacker;
    FactionDefinition selectedDefender;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show()
    {
        if (root != null) return;

        canvas = UIThemeManager.CreateCanvas("WorldMapHUDCanvas", 5);
        root = new GameObject("WorldMapHUDRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CreateRegionButtons();
        CreateSearchBar();
        CreateSelectedFactionBar();
        CreateBattleButton();
        UpdateSelectionState(null, null);
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
        battleButton = null;
    }

    public void CreateRegionButtons()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "RegionButtons", new Vector2(-450, 0), new Vector2(100, 300));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);

        float y = 120;
        foreach (Region r in System.Enum.GetValues(typeof(Region)))
        {
            UIThemeManager.CreateButton(panel.transform, "Region_" + r, r.ToString(), new Vector2(0, y), new Vector2(80, 28), null);
            y -= 35;
        }
    }

    public void CreateSearchBar()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "SearchBar", new Vector2(0, 280), new Vector2(300, 40));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);

        UIThemeManager.CreateText(panel.transform, "SearchPlaceholder", "Search factions...", new Vector2(0, 0), new Vector2(280, 30), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
    }

    public void CreateSelectedFactionBar()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "SelectedFactionBar", new Vector2(0, -280), new Vector2(600, 60));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);

        UIThemeManager.CreateText(panel.transform, "FactionSummary", "Select attacker and defender", new Vector2(0, 0), new Vector2(580, 50), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
    }

    public void CreateBattleButton()
    {
        var btnObj = UIThemeManager.CreateButton(root.transform, "BattleButton", "BATTLE", new Vector2(450, -280), new Vector2(140, 50), OnBattleClicked);
        var rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-20, 20);
        battleButton = btnObj.GetComponent<Button>();
    }

    public void UpdateSelectionState(FactionDefinition attacker, FactionDefinition defender)
    {
        selectedAttacker = attacker;
        selectedDefender = defender;

        if (battleButton != null)
            battleButton.interactable = attacker != null && defender != null;

        var summary = root?.transform.Find("SelectedFactionBar/FactionSummary");
        if (summary != null)
        {
            var text = summary.GetComponent<Text>();
            if (attacker != null && defender != null)
                text.text = attacker.displayName + " vs " + defender.displayName;
            else if (attacker != null)
                text.text = "Attacker: " + attacker.displayName + " — Select defender";
            else if (defender != null)
                text.text = "Defender: " + defender.displayName + " — Select attacker";
            else
                text.text = "Select attacker and defender";
        }
    }

    void OnBattleClicked()
    {
        if (selectedAttacker != null && selectedDefender != null)
            GameManager.Instance?.StartQuickBattle(selectedAttacker, selectedDefender);
    }
}
