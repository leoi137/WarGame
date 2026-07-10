using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleResultsScreen : MonoBehaviour
{
    Canvas canvas;
    GameObject panel;
    Text summaryText;
    Text statsText;
    GameObject buttonContainer;

    public void Show(BattleResult result, FactionDefinition attacker, FactionDefinition defender)
    {
        if (canvas == null)
            CreateCanvas();

        panel.SetActive(true);
        PopulateStats(result);
        summaryText.text = GetBattleSummary(result);
    }

    public void PopulateStats(BattleResult result)
    {
        if (statsText == null) return;

        string stats = $"Winner: {result.winnerFactionId}\n";
        stats += $"Survivors: {result.winnerSurvivors} / {result.winnerStartCount}\n";
        stats += $"Total Casualties: {result.totalCasualties}\n";
        stats += $"Duration: {result.battleDurationSeconds:F1}s\n";

        string mvp = GetMVPUnitType(result);
        if (!string.IsNullOrEmpty(mvp))
            stats += $"MVP: {mvp}";

        statsText.text = stats;
    }

    public void CreateRematchButton(UnityAction onClick)
    {
        if (canvas == null) CreateCanvas();
        if (buttonContainer != null)
            CreateButton(buttonContainer.transform, "Rematch", onClick);
    }

    public void CreateReturnButton(UnityAction onClick)
    {
        if (canvas == null) CreateCanvas();
        if (buttonContainer != null)
            CreateButton(buttonContainer.transform, "Return to Map", onClick);
    }

    public string GetBattleSummary(BattleResult result)
    {
        string winner = result.winnerFactionId ?? "Unknown";
        string loser = result.loserFactionId ?? "Unknown";
        return $"{winner} defeats {loser}. " +
               $"{result.winnerSurvivors} survivors. " +
               $"{result.totalCasualties} casualties. " +
               $"{result.battleDurationSeconds:F0}s.";
    }

    void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("BattleResultsCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject panelObj = new GameObject("ResultsPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        var rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.3f);
        rect.anchorMax = new Vector2(0.8f, 0.8f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = panelObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        summaryText = CreateText(panelObj.transform, "Summary", 24);
        var summaryRect = summaryText.rectTransform;
        summaryRect.anchorMin = new Vector2(0.05f, 0.75f);
        summaryRect.anchorMax = new Vector2(0.95f, 0.95f);
        summaryRect.offsetMin = Vector2.zero;
        summaryRect.offsetMax = Vector2.zero;

        statsText = CreateText(panelObj.transform, "Stats", 18);
        var statsRect = statsText.rectTransform;
        statsRect.anchorMin = new Vector2(0.05f, 0.35f);
        statsRect.anchorMax = new Vector2(0.95f, 0.7f);
        statsRect.offsetMin = Vector2.zero;
        statsRect.offsetMax = Vector2.zero;

        buttonContainer = new GameObject("Buttons");
        buttonContainer.transform.SetParent(panelObj.transform, false);
        var btnRect = buttonContainer.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.2f, 0.05f);
        btnRect.anchorMax = new Vector2(0.8f, 0.25f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var hlg = buttonContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 20f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        panel = panelObj;
        panel.SetActive(false);
    }

    Text CreateText(Transform parent, string name, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }

    void CreateButton(Transform parent, string label, UnityAction onClick)
    {
        GameObject btnObj = new GameObject($"Button_{label}");
        btnObj.transform.SetParent(parent, false);

        var image = btnObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.6f);

        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(120, 40);
    }

    string GetMVPUnitType(BattleResult result)
    {
        int maxKills = 0;
        string mvp = "";
        var kills = result.winningSide == Faction.Attacker ? result.attackerUnitKills : result.defenderUnitKills;
        foreach (var kv in kills ?? new Dictionary<string, int>())
        {
            if (kv.Value > maxKills)
            {
                maxKills = kv.Value;
                mvp = kv.Key;
            }
        }
        return mvp;
    }
}
