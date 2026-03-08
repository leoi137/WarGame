using UnityEngine;
using UnityEngine.UI;

public class CampaignVictoryUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;

    void Awake()
    {
        FactionDatabase.Initialize();
    }

    public void ShowVictory(CampaignState state)
    {
        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("CampaignVictoryCanvas", 50);
        root = new GameObject("CampaignVictoryRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var faction = FactionDatabase.Get(state?.winnerId ?? state?.playerFactionId);
        var factionName = faction?.displayName ?? state?.winnerId ?? "Victory";
        var factionColor = faction != null ? FactionColorPalette.GetUIColor(faction.id) : UIThemeManager.TextAccent;

        var banner = UIThemeManager.CreatePanel(root.transform, "VictoryBanner", new Vector2(0, 180), new Vector2(700, 100));
        var bannerRect = banner.GetComponent<RectTransform>();
        bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
        bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
        bannerRect.pivot = new Vector2(0.5f, 0.5f);
        bannerRect.anchoredPosition = new Vector2(0, 180);
        var bannerText = UIThemeManager.CreateText(banner.transform, "BannerText", factionName + " VICTORY!", new Vector2(0, 0), new Vector2(680, 80), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        bannerText.GetComponent<Text>().color = factionColor;

        int provincesConquered = 0;
        if (state != null && CampaignManager.Instance?.provinceManager != null)
            provincesConquered = CampaignManager.Instance.provinceManager.GetProvincesForFaction(state.winnerId ?? state.playerFactionId).Count;

        var statsPanel = UIThemeManager.CreatePanel(root.transform, "StatsPanel", new Vector2(0, 40), new Vector2(400, 120));
        var statsRect = statsPanel.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.5f, 0.5f);
        statsRect.anchorMax = new Vector2(0.5f, 0.5f);
        statsRect.pivot = new Vector2(0.5f, 0.5f);
        statsRect.anchoredPosition = new Vector2(0, 40);
        UIThemeManager.CreateText(statsPanel.transform, "Provinces", "Provinces Conquered: " + provincesConquered, new Vector2(0, 35), new Vector2(380, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(statsPanel.transform, "Turns", "Turns Taken: " + (state?.currentTurn ?? 0), new Vector2(0, 0), new Vector2(380, 24), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
        UIThemeManager.CreateText(statsPanel.transform, "Faction", "Faction: " + factionName, new Vector2(0, -35), new Vector2(380, 24), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);

        var returnBtn = UIThemeManager.CreateButton(root.transform, "ReturnToMenu", "Return to Menu", new Vector2(0, -180), new Vector2(200, 50), () =>
        {
            GameManager.Instance?.TransitionTo(GameFlowState.MainMenu);
            Hide();
        });
        var returnRect = returnBtn.GetComponent<RectTransform>();
        returnRect.anchorMin = new Vector2(0.5f, 0.5f);
        returnRect.anchorMax = new Vector2(0.5f, 0.5f);
        returnRect.pivot = new Vector2(0.5f, 0.5f);
        returnRect.anchoredPosition = new Vector2(0, -180);
    }

    public void ShowDefeat(CampaignState state)
    {
        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("CampaignDefeatCanvas", 50);
        root = new GameObject("CampaignDefeatRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var banner = UIThemeManager.CreatePanel(root.transform, "DefeatBanner", new Vector2(0, 180), new Vector2(600, 90));
        var bannerRect = banner.GetComponent<RectTransform>();
        bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
        bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
        bannerRect.pivot = new Vector2(0.5f, 0.5f);
        bannerRect.anchoredPosition = new Vector2(0, 180);
        var bannerText = UIThemeManager.CreateText(banner.transform, "BannerText", "DEFEAT", new Vector2(0, 0), new Vector2(580, 70), UIThemeManager.TitleFontSize, TextAnchor.MiddleCenter);
        bannerText.GetComponent<Text>().color = UIThemeManager.HealthRed;

        var winnerFaction = FactionDatabase.Get(state?.winnerId);
        var winnerName = winnerFaction?.displayName ?? state?.winnerId ?? "Enemy";
        var descPanel = UIThemeManager.CreatePanel(root.transform, "DescPanel", new Vector2(0, 50), new Vector2(450, 80));
        var descRect = descPanel.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.5f);
        descRect.anchorMax = new Vector2(0.5f, 0.5f);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.anchoredPosition = new Vector2(0, 50);
        UIThemeManager.CreateText(descPanel.transform, "Desc", winnerName + " has conquered the known world.", new Vector2(0, 0), new Vector2(430, 60), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);

        var tryAgainBtn = UIThemeManager.CreateButton(root.transform, "TryAgain", "Try Again", new Vector2(0, -180), new Vector2(180, 50), () =>
        {
            GameManager.Instance?.TransitionTo(GameFlowState.MainMenu);
            Hide();
        });
        var tryAgainRect = tryAgainBtn.GetComponent<RectTransform>();
        tryAgainRect.anchorMin = new Vector2(0.5f, 0.5f);
        tryAgainRect.anchorMax = new Vector2(0.5f, 0.5f);
        tryAgainRect.pivot = new Vector2(0.5f, 0.5f);
        tryAgainRect.anchoredPosition = new Vector2(0, -180);
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
}
