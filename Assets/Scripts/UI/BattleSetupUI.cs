using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleSetupUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    BattleConfiguration config;
    Faction playerSide;
    FormationType selectedFormation = FormationType.Line;
    int totalUnits;
    int placedUnits;

    void Awake()
    {
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();
    }

    public void Show(BattleConfiguration config, Faction playerSide)
    {
        this.config = config;
        this.playerSide = playerSide;
        totalUnits = playerSide == Faction.Attacker ? config.attackerUnitBudget : config.defenderUnitBudget;
        placedUnits = totalUnits;

        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("BattleSetupCanvas", 20);
        root = new GameObject("BattleSetupRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var playerFaction = playerSide == Faction.Attacker ? config.attackerFaction : config.defenderFaction;
        var opponentFaction = playerSide == Faction.Attacker ? config.defenderFaction : config.attackerFaction;

        CreateArmyRoster(playerFaction);
        CreateFormationToolbar();
        CreatePlacementControls();
        CreateSelectionInfo();
        ShowOpponentPreview(opponentFaction);
        CreatePlacementMinimap(config.mapSize);
        ShowHotkeyGuide();
        ShowTerrainInfo(config.primaryTerrain);
        UpdateArmyCount(totalUnits, placedUnits);
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

    public void CreateArmyRoster(FactionDefinition faction)
    {
        var roster = UIThemeManager.CreatePanel(root.transform, "ArmyRoster", new Vector2(-450, 0), new Vector2(220, 400));
        var rect = roster.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);

        UIThemeManager.CreateText(roster.transform, "RosterTitle", "Your Army", new Vector2(0, 170), new Vector2(200, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);

        var groups = faction.unitTypes?.GroupBy(u => u.category).ToList() ?? new List<IGrouping<UnitCategory, UnitTypeDefinition>>();
        float y = 130;
        foreach (var grp in groups)
        {
            int count = grp.Sum(u => 1);
            var names = string.Join(", ", grp.Take(3).Select(u => u.displayName));
            if (grp.Count() > 3) names += "...";
            var line = UIThemeManager.CreateText(roster.transform, "Cat_" + grp.Key, grp.Key + ": " + names, new Vector2(0, y), new Vector2(200, 40), UIThemeManager.SmallFontSize, TextAnchor.UpperLeft);
            y -= 35;
        }
    }

    public void CreateFormationToolbar()
    {
        var toolbar = UIThemeManager.CreatePanel(root.transform, "FormationToolbar", new Vector2(0, -250), new Vector2(500, 50));
        var rect = toolbar.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);

        string[] labels = { "Line (F1)", "Column (F2)", "Wedge (F3)", "Square (F4)", "Spread (F5)" };
        float x = -200;
        for (int i = 0; i < 5; i++)
        {
            var formation = (FormationType)i;
            UIThemeManager.CreateButton(toolbar.transform, "Form_" + formation, labels[i], new Vector2(x, 0), new Vector2(90, 36), () => OnFormationSelected(formation));
            x += 100;
        }
    }

    public void CreatePlacementControls()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "PlacementControls", new Vector2(450, -250), new Vector2(200, 80));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-20, -250);

        UIThemeManager.CreateButton(panel.transform, "AutoPlace", "Auto Place", new Vector2(-50, 15), new Vector2(100, 36), OnAutoPlace);
        UIThemeManager.CreateButton(panel.transform, "Confirm", "Confirm", new Vector2(50, 15), new Vector2(100, 36), OnConfirmPlacement);
    }

    public void CreateSelectionInfo()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "SelectionInfo", new Vector2(0, -300), new Vector2(400, 50));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);

        UIThemeManager.CreateText(panel.transform, "SelectionText", "0 selected | Avg HP: - ATK: -", new Vector2(0, 0), new Vector2(380, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
    }

    public void UpdateArmyCount(int total, int placed)
    {
        totalUnits = total;
        placedUnits = placed;
        var info = root?.transform.Find("ArmyCount");
        if (info != null)
        {
            info.GetComponent<Text>().text = placed + " / " + total + " units deployed";
        }
    }

    public void ShowTerrainInfo(TerrainType terrain)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "TerrainInfo", new Vector2(-450, -200), new Vector2(180, 60));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        UIThemeManager.CreateText(panel.transform, "TerrainLabel", "Terrain: " + terrain, new Vector2(0, 0), new Vector2(160, 50), UIThemeManager.SmallFontSize, TextAnchor.MiddleCenter);
    }

    public void ShowOpponentPreview(FactionDefinition opponent)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "OpponentPreview", new Vector2(450, 0), new Vector2(220, 250));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.pivot = new Vector2(1, 0.5f);
        rect.anchoredPosition = new Vector2(-20, 0);

        UIThemeManager.CreateText(panel.transform, "OppTitle", opponent.displayName, new Vector2(0, 100), new Vector2(200, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        var groups = opponent.unitTypes?.GroupBy(u => u.category).ToList() ?? new List<IGrouping<UnitCategory, UnitTypeDefinition>>();
        float y = 60;
        foreach (var grp in groups)
        {
            var names = string.Join(", ", grp.Take(2).Select(u => u.displayName));
            UIThemeManager.CreateText(panel.transform, "Opp_" + grp.Key, grp.Key + ": " + names, new Vector2(0, y), new Vector2(200, 20), UIThemeManager.SmallFontSize, TextAnchor.MiddleCenter);
            y -= 25;
        }
    }

    public void CreatePlacementMinimap(int mapSize)
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "PlacementMinimap", new Vector2(450, 180), new Vector2(120, 120));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-20, -20);
        UIThemeManager.CreateText(panel.transform, "MinimapLabel", "Minimap", new Vector2(0, -50), new Vector2(100, 20), UIThemeManager.SmallFontSize, TextAnchor.MiddleCenter);
    }

    public void ShowHotkeyGuide()
    {
        var panel = UIThemeManager.CreatePanel(root.transform, "HotkeyGuide", new Vector2(-450, 250), new Vector2(200, 100));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        UIThemeManager.CreateText(panel.transform, "Hotkeys", "F1-F5: Formations\nClick: Select units", new Vector2(10, -10), new Vector2(180, 80), UIThemeManager.SmallFontSize, TextAnchor.UpperLeft);
    }

    public void OnConfirmPlacement()
    {
        GameManager.Instance?.StartBattleSimulation();
    }

    public void OnAutoPlace()
    {
    }

    public void OnFormationSelected(FormationType formation)
    {
        selectedFormation = formation;
    }
}
