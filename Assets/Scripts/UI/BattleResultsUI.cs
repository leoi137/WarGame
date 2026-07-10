using UnityEngine;
using UnityEngine.UI;

public class BattleResultsUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show(BattleResult result, FactionDefinition attacker, FactionDefinition defender)
    {
        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("BattleResultsCanvas", 40);
        root = new GameObject("BattleResultsRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var winnerId = result.winnerFactionId;
        var winner = FactionDatabase.Get(winnerId);
        var winnerName = winner?.displayName ?? winnerId ?? "Unknown";
        var winnerColor = winner != null ? FactionColorPalette.GetUIColor(winner.id) : Color.white;

        CreateVictoryBanner(winnerName, winnerColor);
        CreateCasualtyReport(result);
        CreateStatistics(result);
        CreateActionButtons();
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
    }

    public void CreateVictoryBanner(string winnerName, Color winnerColor)
    {
        var banner = UIThemeManager.CreatePanel(root.transform, "VictoryBanner", new Vector2(0, 200), new Vector2(600, 80));
        var textObj = UIThemeManager.CreateText(banner.transform, "BannerText", winnerName + " VICTORY!", new Vector2(0, 0), new Vector2(580, 70), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        textObj.GetComponent<Text>().color = winnerColor;
    }

    public void CreateCasualtyReport(BattleResult result)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "CasualtyReport", new Vector2(-200, 0), new Vector2(350, 300));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);

        UIThemeManager.CreateText(panel.transform, "ReportTitle", "Casualties", new Vector2(0, 130), new Vector2(330, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);

        float y = 90;
        foreach (var kv in result.attackerUnitLosses)
        {
            var unit = UnitDatabase.Get(kv.Key);
            var name = unit?.displayName ?? kv.Key;
            UIThemeManager.CreateText(panel.transform, "Loss_" + kv.Key, "Attacker " + name + ": " + kv.Value, new Vector2(10, y), new Vector2(330, 20), UIThemeManager.SmallFontSize, TextAnchor.MiddleLeft);
            y -= 22;
        }
        foreach (var kv in result.defenderUnitLosses)
        {
            var unit = UnitDatabase.Get(kv.Key);
            var name = unit?.displayName ?? kv.Key;
            UIThemeManager.CreateText(panel.transform, "Loss_" + kv.Key, "Defender " + name + ": " + kv.Value, new Vector2(10, y), new Vector2(330, 20), UIThemeManager.SmallFontSize, TextAnchor.MiddleLeft);
            y -= 22;
        }
    }

    public void CreateStatistics(BattleResult result)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "Statistics", new Vector2(200, 0), new Vector2(280, 180));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.pivot = new Vector2(1, 0.5f);
        rect.anchoredPosition = new Vector2(-20, 0);

        UIThemeManager.CreateText(panel.transform, "StatTitle", "Statistics", new Vector2(0, 70), new Vector2(260, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(panel.transform, "Duration", "Duration: " + result.battleDurationSeconds.ToString("F1") + "s", new Vector2(0, 40), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(panel.transform, "Kills", "Total Kills: " + result.totalCasualties, new Vector2(0, 10), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(panel.transform, "MVP", "MVP: -", new Vector2(0, -20), new Vector2(260, 20), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
    }

    public void CreateActionButtons()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "ActionButtons", new Vector2(0, -220), new Vector2(500, 60));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);

        UIThemeManager.CreateButton(panel.transform, "Rematch", "Rematch", new Vector2(-120, 0), new Vector2(100, 44), OnRematch);
        UIThemeManager.CreateButton(panel.transform, "NewBattle", "New Battle", new Vector2(0, 0), new Vector2(100, 44), OnNewBattle);
        UIThemeManager.CreateButton(panel.transform, "ReturnToMap", "Return to Map", new Vector2(120, 0), new Vector2(120, 44), OnReturnToMap);
    }

    void OnRematch()
    {
        if (GameManager.Instance != null && GameManager.Instance.SelectedAttacker != null && GameManager.Instance.SelectedDefender != null)
        {
            GameManager.Instance.StartQuickBattle(GameManager.Instance.SelectedAttacker, GameManager.Instance.SelectedDefender);
        }
        Hide();
    }

    void OnNewBattle()
    {
        GameManager.Instance?.TransitionTo(GameFlowState.FactionSelect);
        Hide();
    }

    void OnReturnToMap()
    {
        GameManager.Instance?.ReturnToWorldMap();
        Hide();
    }
}
