using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show()
    {
        if (root != null) return;

        canvas = UIThemeManager.CreateCanvas("MainMenuCanvas", 0);
        root = new GameObject("MainMenuRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CreateBackgroundScene();
        CreateTitle();
        CreateMenuButtons();
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

    public void CreateTitle()
    {
        var titleObj = UIThemeManager.CreateText(root.transform, "Title", "WorldWars: 1016 AD", new Vector2(0, 180), new Vector2(800, 80), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        var text = titleObj.GetComponent<Text>();
        text.color = UIThemeManager.TextAccent;
    }

    public void CreateMenuButtons()
    {
        float y = 40;
        float spacing = 70;

        UIThemeManager.CreateButton(root.transform, "QuickBattle", "Quick Battle", new Vector2(0, y), new Vector2(280, 50), OnQuickBattle);
        y -= spacing;
        UIThemeManager.CreateButton(root.transform, "WorldMap", "World Map", new Vector2(0, y), new Vector2(280, 50), OnWorldMap);
        y -= spacing;
        UIThemeManager.CreateButton(root.transform, "UnitViewer", "Unit Viewer", new Vector2(0, y), new Vector2(280, 50), OnUnitViewer);
        y -= spacing;
        UIThemeManager.CreateButton(root.transform, "Settings", "Settings", new Vector2(0, y), new Vector2(280, 50), OnSettings);
    }

    public void CreateBackgroundScene()
    {
        var bg = UIThemeManager.CreatePanel(root.transform, "Background", Vector2.zero, new Vector2(2000, 1200));
        bg.transform.SetAsFirstSibling();
        var img = bg.GetComponent<Image>();
        img.color = new Color(0.05f, 0.04f, 0.03f, 0.95f);
    }

    void OnQuickBattle()
    {
        GameManager.Instance?.TransitionTo(GameFlowState.FactionSelect);
    }

    void OnWorldMap()
    {
        GameManager.Instance?.TransitionTo(GameFlowState.WorldMap);
    }

    void OnUnitViewer()
    {
    }

    void OnSettings()
    {
    }
}
