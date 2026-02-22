using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    BattleConfiguration config;
    Text attackerCountText;
    Text defenderCountText;
    Text timerText;
    Text speedText;
    float elapsedTime;
    int lastAttackerCount = -1;
    int lastDefenderCount = -1;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Initialize(BattleConfiguration config)
    {
        this.config = config;

        if (root != null) return;

        canvas = UIThemeManager.CreateCanvas("BattleHUDCanvas", 30);
        root = new GameObject("BattleHUDRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CreateFactionCounters(config.attackerFaction, config.defenderFaction);
        CreateTimeControls();
        CreateBattleTimer();
        CreateMinimap();
        CreateUnitTooltip();
    }

    void Update()
    {
        if (config == null || root == null) return;

        elapsedTime += Time.deltaTime;

        if (BattleManager.Instance != null)
        {
            int ac = 0, dc = 0;
            foreach (var u in BattleManager.Instance.attackerUnits) if (u != null && !u.isDead) ac++;
            foreach (var u in BattleManager.Instance.defenderUnits) if (u != null && !u.isDead) dc++;

            if (ac != lastAttackerCount && attackerCountText != null)
            {
                lastAttackerCount = ac;
                attackerCountText.text = config.attackerFaction?.displayName + ": " + ac + " alive";
            }
            if (dc != lastDefenderCount && defenderCountText != null)
            {
                lastDefenderCount = dc;
                defenderCountText.text = config.defenderFaction?.displayName + ": " + dc + " alive";
            }
        }

        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
    }

    public void CreateFactionCounters(FactionDefinition attacker, FactionDefinition defender)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "FactionCounters", new Vector2(0, 280), new Vector2(400, 60));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);

        var atkObj = UIThemeManager.CreateText(panel.transform, "AttackerCount", (attacker?.displayName ?? "Blue") + ": 0 alive", new Vector2(-100, 10), new Vector2(180, 24), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        attackerCountText = atkObj.GetComponent<Text>();
        if (attacker != null) attackerCountText.color = FactionColorPalette.GetUIColor(attacker.id);

        var defObj = UIThemeManager.CreateText(panel.transform, "DefenderCount", (defender?.displayName ?? "Red") + ": 0 alive", new Vector2(100, 10), new Vector2(180, 24), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        defenderCountText = defObj.GetComponent<Text>();
        if (defender != null) defenderCountText.color = FactionColorPalette.GetUIColor(defender.id);
    }

    public void CreateTimeControls()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "TimeControls", new Vector2(-400, 250), new Vector2(120, 80));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        UIThemeManager.CreateButton(panel.transform, "Play", "Play", new Vector2(-40, 20), new Vector2(50, 30), null);
        UIThemeManager.CreateButton(panel.transform, "Pause", "Pause", new Vector2(20, 20), new Vector2(50, 30), null);
        UIThemeManager.CreateButton(panel.transform, "1x", "1x", new Vector2(-50, -20), new Vector2(30, 24), null);
        UIThemeManager.CreateButton(panel.transform, "2x", "2x", new Vector2(-15, -20), new Vector2(30, 24), null);
        UIThemeManager.CreateButton(panel.transform, "4x", "4x", new Vector2(20, -20), new Vector2(30, 24), null);
    }

    public void CreateBattleTimer()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "BattleTimer", new Vector2(0, 250), new Vector2(150, 40));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);

        var timerObj = UIThemeManager.CreateText(panel.transform, "Timer", "0:00", new Vector2(0, 0), new Vector2(130, 30), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        timerText = timerObj.GetComponent<Text>();
    }

    public void CreateMinimap()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "Minimap", new Vector2(400, -200), new Vector2(140, 140));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-20, 20);
    }

    public void CreateUnitTooltip()
    {
    }

    public void ShowBattleEvent(string message)
    {
        var evt = UIThemeManager.CreateText(root.transform, "BattleEvent", message, new Vector2(0, 0), new Vector2(400, 40), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        evt.GetComponent<Text>().color = UIThemeManager.TextAccent;
        Destroy(evt, 2f);
    }

    static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return m + ":" + s.ToString("D2");
    }
}
