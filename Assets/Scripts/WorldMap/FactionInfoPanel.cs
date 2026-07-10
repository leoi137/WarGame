using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FactionInfoPanel : MonoBehaviour
{
    GameObject _panelRoot;
    GameObject _contentRoot;
    Canvas _canvas;
    Font _font;

    void Awake()
    {
        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    public void Show(FactionDefinition faction)
    {
        EnsurePanel();
        _panelRoot.SetActive(true);
        PopulateFactionInfo(faction);
    }

    public void Hide()
    {
        if (_panelRoot != null)
            _panelRoot.SetActive(false);
    }

    public void PopulateFactionInfo(FactionDefinition faction)
    {
        if (faction == null || _contentRoot == null) return;
        ClearContent();
        CreateText(_contentRoot.transform, "Name", faction.displayName, 24, FontStyle.Bold);
        CreateText(_contentRoot.transform, "Ruler", faction.rulerName ?? "", 18);
        CreateText(_contentRoot.transform, "Trait", faction.factionTrait ?? "", 14);
        if (faction.cities != null && faction.cities.Count > 0)
            PopulateCityList(faction.cities);
        if (faction.unitTypes != null && faction.unitTypes.Count > 0)
            PopulateUnitList(faction.unitTypes);
    }

    public void PopulateUnitList(List<UnitTypeDefinition> units)
    {
        if (units == null || _contentRoot == null) return;
        var header = CreateText(_contentRoot.transform, "UnitsHeader", "Units", 16, FontStyle.Bold);
        foreach (var u in units)
        {
            var line = $"{u.displayName}: HP {u.maxHP} ATK {u.attackDamage} ARM {u.armor}";
            CreateText(_contentRoot.transform, $"Unit_{u.id}", line, 12);
        }
    }

    public void PopulateCityList(List<CityDefinition> cities)
    {
        if (cities == null || _contentRoot == null) return;
        var header = CreateText(_contentRoot.transform, "CitiesHeader", "Cities", 16, FontStyle.Bold);
        foreach (var c in cities)
        {
            var line = c.isCapital ? $"{c.displayName} (Capital) - Garrison: {c.garrison}" : $"{c.displayName} - Garrison: {c.garrison}";
            CreateText(_contentRoot.transform, $"City_{c.id}", line, 12);
        }
    }

    public void CreateSelectButton(string label, UnityAction onClick)
    {
        if (_contentRoot == null) return;
        var btnObj = new GameObject("SelectButton");
        btnObj.transform.SetParent(_contentRoot.transform, false);
        var rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 32);
        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.3f);
        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var text = textObj.AddComponent<Text>();
        text.font = _font;
        text.text = label;
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }

    void EnsurePanel()
    {
        if (_panelRoot != null) return;
        _panelRoot = new GameObject("FactionInfoPanel");
        _panelRoot.transform.SetParent(transform);

        _canvas = _panelRoot.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _panelRoot.AddComponent<CanvasScaler>();
        _panelRoot.AddComponent<GraphicRaycaster>();

        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(_panelRoot.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(1, 1);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.pivot = new Vector2(1, 1);
        bgRect.anchoredPosition = new Vector2(-20, -20);
        bgRect.sizeDelta = new Vector2(280, 400);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

        _contentRoot = new GameObject("Content");
        _contentRoot.transform.SetParent(bgObj.transform, false);
        var contentRect = _contentRoot.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(12, 12);
        contentRect.offsetMax = new Vector2(-12, -12);
        var vlg = _contentRoot.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
    }

    void ClearContent()
    {
        if (_contentRoot == null) return;
        for (int i = _contentRoot.transform.childCount - 1; i >= 0; i--)
            Object.Destroy(_contentRoot.transform.GetChild(i).gameObject);
    }

    Text CreateText(Transform parent, string name, string content, int fontSize, FontStyle style = FontStyle.Normal)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, fontSize + 4);
        var text = go.AddComponent<Text>();
        text.font = _font;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = new Color(0.9f, 0.9f, 0.85f);
        return text;
    }
}
