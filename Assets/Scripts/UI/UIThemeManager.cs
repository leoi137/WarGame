using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class UIThemeManager
{
    public static Color PanelBackground => new Color(0.08f, 0.06f, 0.04f, 0.92f);
    public static Color ButtonNormal => new Color(0.25f, 0.18f, 0.12f, 1f);
    public static Color ButtonHover => new Color(0.35f, 0.25f, 0.15f, 1f);
    public static Color ButtonPressed => new Color(0.15f, 0.1f, 0.06f, 1f);
    public static Color TextPrimary => new Color(0.95f, 0.9f, 0.82f, 1f);
    public static Color TextSecondary => new Color(0.75f, 0.7f, 0.6f, 1f);
    public static Color TextAccent => new Color(0.9f, 0.75f, 0.35f, 1f);
    public static Color HealthGreen => new Color(0.2f, 0.75f, 0.25f, 1f);
    public static Color HealthYellow => new Color(0.9f, 0.75f, 0.15f, 1f);
    public static Color HealthRed => new Color(0.85f, 0.2f, 0.15f, 1f);

    public static int TitleFontSize = 42;
    public static int HeaderFontSize = 24;
    public static int BodyFontSize = 16;
    public static int SmallFontSize = 12;

    public static GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size)
    {
        var panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        var rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = panel.AddComponent<Image>();
        image.color = PanelBackground;

        return panel;
    }

    public static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size, UnityAction onClick)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        var rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = buttonObj.AddComponent<Image>();
        image.color = ButtonNormal;

        var button = buttonObj.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = ButtonNormal;
        colors.highlightedColor = ButtonHover;
        colors.pressedColor = ButtonPressed;
        button.colors = colors;

        if (onClick != null)
            button.onClick.AddListener(onClick);

        var labelObj = CreateText(buttonObj.transform, "Label", label, Vector2.zero, size, BodyFontSize, TextAnchor.MiddleCenter);
        labelObj.GetComponent<Text>().raycastTarget = false;

        return buttonObj;
    }

    public static GameObject CreateText(Transform parent, string name, string content, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        var rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = TextPrimary;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return textObj;
    }

    public static GameObject CreateScrollView(Transform parent, string name, Vector2 position, Vector2 size)
    {
        var scrollObj = new GameObject(name);
        scrollObj.transform.SetParent(parent, false);

        var rect = scrollObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = scrollObj.AddComponent<Image>();
        image.color = PanelBackground;

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = Color.clear;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 1f);
        contentRect.anchorMax = new Vector2(0.5f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(size.x - 20f, size.y);

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;

        return scrollObj;
    }

    public static Canvas CreateCanvas(string name, int sortOrder)
    {
        var canvasObj = new GameObject(name);
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasObj.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        EnsureEventSystem();

        return canvas;
    }

    /// <summary>
    /// Guarantees an EventSystem exists in the scene so UI interactions work.
    /// </summary>
    static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }
}
