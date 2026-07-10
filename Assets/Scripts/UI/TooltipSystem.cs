using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    Canvas canvas;
    GameObject tooltipRoot;
    Text titleText;
    Text bodyText;
    RectTransform tooltipRect;
    bool visible;
    Vector2 offset = new Vector2(15, -15);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (visible && tooltipRect != null)
        {
            tooltipRect.anchoredPosition = (Vector2)Input.mousePosition / canvas.scaleFactor + offset;
        }
    }

    public void ShowTooltip(string title, string body, Vector2 screenPos)
    {
        EnsureCanvas();
        if (tooltipRoot == null) BuildTooltip();

        titleText.text = title;
        bodyText.text = body;
        tooltipRoot.SetActive(true);
        tooltipRect.anchoredPosition = screenPos / canvas.scaleFactor + offset;
        visible = true;
    }

    public void HideTooltip()
    {
        visible = false;
        if (tooltipRoot != null) tooltipRoot.SetActive(false);
    }

    public void ShowUnitTooltip(UnitTypeDefinition unitType, Vector2 pos)
    {
        if (unitType == null) return;
        string title = unitType.displayName ?? unitType.id;
        string body = string.Format("HP: {0} | ATK: {1} | DEF: {2} | SPD: {3}",
            unitType.maxHP, unitType.attackDamage, unitType.armor, unitType.moveSpeed);
        ShowTooltip(title, body, pos);
    }

    public void ShowCityTooltip(CityDefinition city, FactionDefinition faction, Vector2 pos)
    {
        if (city == null) return;
        string title = city.displayName ?? city.id;
        string body = "Garrison: " + city.garrison + " | Terrain: " + city.primaryTerrain;
        if (faction != null) body += " | " + faction.displayName;
        ShowTooltip(title, body, pos);
    }

    public void ShowTerrainTooltip(TerrainType terrain, Vector2 pos)
    {
        string title = terrain.ToString();
        string body = "Terrain modifiers apply to unit stats.";
        ShowTooltip(title, body, pos);
    }

    void EnsureCanvas()
    {
        if (canvas == null)
        {
            var canvasObj = UIThemeManager.CreateCanvas("TooltipCanvas", 100);
            canvas = canvasObj;
        }
    }

    void BuildTooltip()
    {
        tooltipRoot = UIThemeManager.CreatePanel(canvas.transform, "Tooltip", Vector2.zero, new Vector2(200, 80));
        tooltipRect = tooltipRoot.GetComponent<RectTransform>();
        tooltipRect.pivot = new Vector2(0, 1);

        var bg = tooltipRoot.GetComponent<Image>();
        bg.color = new Color(0.1f, 0.08f, 0.06f, 0.98f);

        var titleObj = UIThemeManager.CreateText(tooltipRoot.transform, "Title", "", new Vector2(10, -15), new Vector2(180, 22), UIThemeManager.HeaderFontSize, TextAnchor.MiddleLeft);
        titleText = titleObj.GetComponent<Text>();
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 1);
        titleRect.anchoredPosition = new Vector2(10, -10);

        var bodyObj = UIThemeManager.CreateText(tooltipRoot.transform, "Body", "", new Vector2(10, -45), new Vector2(180, 60), UIThemeManager.SmallFontSize, TextAnchor.UpperLeft);
        bodyText = bodyObj.GetComponent<Text>();
        bodyText.color = UIThemeManager.TextSecondary;
        var bodyRect = bodyObj.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0, 1);
        bodyRect.anchorMax = new Vector2(1, 0);
        bodyRect.pivot = new Vector2(0, 1);
        bodyRect.anchoredPosition = new Vector2(10, -35);
        bodyRect.offsetMax = new Vector2(-10, -10);

        tooltipRoot.AddComponent<CanvasGroup>().blocksRaycasts = false;
        tooltipRoot.SetActive(false);
    }
}
