using UnityEngine;
using UnityEngine.UI;

public class CampaignHUD : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    GameObject bar;
    Text goldText;
    Text provinceText;
    Text turnText;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void Show()
    {
        if (root != null) return;

        canvas = UIThemeManager.CreateCanvas("CampaignHUDCanvas", 25);
        root = new GameObject("CampaignHUDRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        bar = UIThemeManager.CreatePanel(root.transform, "HudBar", new Vector2(0, 540), new Vector2(1920, 50));
        var barRect = bar.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 1f);
        barRect.anchorMax = new Vector2(1f, 1f);
        barRect.pivot = new Vector2(0.5f, 1f);
        barRect.anchoredPosition = new Vector2(0, 0);
        barRect.offsetMin = new Vector2(0, -50);
        barRect.offsetMax = new Vector2(0, 0);

        var factionColorBar = new GameObject("FactionColor");
        factionColorBar.transform.SetParent(bar.transform, false);
        var colorRect = factionColorBar.AddComponent<RectTransform>();
        colorRect.anchorMin = new Vector2(0, 0);
        colorRect.anchorMax = new Vector2(0, 1);
        colorRect.pivot = new Vector2(0, 0.5f);
        colorRect.anchoredPosition = Vector2.zero;
        colorRect.sizeDelta = new Vector2(8, 0);
        colorRect.offsetMin = new Vector2(0, 4);
        colorRect.offsetMax = new Vector2(8, -4);
        factionColorBar.AddComponent<Image>().color = Color.gray;

        var factionNameObj = UIThemeManager.CreateText(bar.transform, "FactionName", "Faction", new Vector2(-800, 0), new Vector2(200, 40), UIThemeManager.HeaderFontSize, TextAnchor.MiddleLeft);
        var factionRect = factionNameObj.GetComponent<RectTransform>();
        factionRect.anchorMin = new Vector2(0, 0.5f);
        factionRect.anchorMax = new Vector2(0, 0.5f);
        factionRect.pivot = new Vector2(0, 0.5f);
        factionRect.anchoredPosition = new Vector2(20, 0);

        var goldObj = UIThemeManager.CreateText(bar.transform, "Gold", "0", new Vector2(-500, 0), new Vector2(120, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
        var goldRect = goldObj.GetComponent<RectTransform>();
        goldRect.anchorMin = new Vector2(0, 0.5f);
        goldRect.anchorMax = new Vector2(0, 0.5f);
        goldRect.pivot = new Vector2(0, 0.5f);
        goldRect.anchoredPosition = new Vector2(280, 0);
        goldText = goldObj.GetComponent<Text>();

        var provinceObj = UIThemeManager.CreateText(bar.transform, "Provinces", "0", new Vector2(-300, 0), new Vector2(100, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
        var provinceRect = provinceObj.GetComponent<RectTransform>();
        provinceRect.anchorMin = new Vector2(0, 0.5f);
        provinceRect.anchorMax = new Vector2(0, 0.5f);
        provinceRect.pivot = new Vector2(0, 0.5f);
        provinceRect.anchoredPosition = new Vector2(480, 0);
        provinceText = provinceObj.GetComponent<Text>();

        var turnObj = UIThemeManager.CreateText(bar.transform, "Turn", "Turn 1 - Year 1", new Vector2(0, 0), new Vector2(200, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        var turnRect = turnObj.GetComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0.5f, 0.5f);
        turnRect.anchorMax = new Vector2(0.5f, 0.5f);
        turnRect.pivot = new Vector2(0.5f, 0.5f);
        turnRect.anchoredPosition = new Vector2(0, 0);
        turnText = turnObj.GetComponent<Text>();

        _factionColorImage = factionColorBar.GetComponent<Image>();
        _factionNameText = factionNameObj.GetComponent<Text>();
    }

    Image _factionColorImage;
    Text _factionNameText;

    public void UpdateHUD(FactionCampaignState playerState, int turn, int year)
    {
        if (playerState == null) return;

        var faction = FactionDatabase.Get(playerState.factionId);
        if (_factionColorImage != null)
            _factionColorImage.color = faction != null ? faction.primaryColor : Color.gray;
        if (_factionNameText != null)
            _factionNameText.text = faction?.displayName ?? playerState.factionId;

        if (goldText != null)
            goldText.text = playerState.gold + "g";
        if (turnText != null)
            turnText.text = "Turn " + turn + " - Year " + year;

        if (provinceText != null && CampaignManager.Instance != null && CampaignManager.Instance.provinceManager != null)
        {
            var count = CampaignManager.Instance.provinceManager.GetProvincesForFaction(playerState.factionId).Count;
            provinceText.text = count.ToString();
        }
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
        bar = null;
        goldText = null;
        provinceText = null;
        turnText = null;
        _factionColorImage = null;
        _factionNameText = null;
    }
}
