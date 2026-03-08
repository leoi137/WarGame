using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMapUI : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    GameObject provinceListContent;
    GameObject actionPanel;
    CampaignState _state;
    ProvinceManager _provinceManager;
    string _selectedProvinceId;
    string _selectedCityId;
    Dictionary<string, int> _committedUnits = new Dictionary<string, int>();

    void Awake()
    {
        FactionDatabase.Initialize();
        ProvinceDatabase.Initialize();
        UnitDatabase.Initialize();
    }

    public void Show(CampaignState state, ProvinceManager provinceManager)
    {
        _state = state;
        _provinceManager = provinceManager;
        if (root != null) Hide();

        canvas = UIThemeManager.CreateCanvas("CampaignMapCanvas", 15);
        root = new GameObject("CampaignMapRoot");
        root.transform.SetParent(canvas.transform, false);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        var scrollView = UIThemeManager.CreateScrollView(root.transform, "ProvinceList", new Vector2(-400, 0), new Vector2(280, 500));
        var scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.5f);
        scrollRect.anchorMax = new Vector2(0, 0.5f);
        scrollRect.pivot = new Vector2(0, 0.5f);
        scrollRect.anchoredPosition = new Vector2(-400, 0);

        provinceListContent = scrollView.transform.Find("Viewport/Content")?.gameObject;
        if (provinceListContent == null) provinceListContent = scrollView;

        actionPanel = UIThemeManager.CreatePanel(root.transform, "ActionPanel", new Vector2(350, 0), new Vector2(320, 400));
        var actionRect = actionPanel.GetComponent<RectTransform>();
        actionRect.anchorMin = new Vector2(1, 0.5f);
        actionRect.anchorMax = new Vector2(1, 0.5f);
        actionRect.pivot = new Vector2(1, 0.5f);
        actionRect.anchoredPosition = new Vector2(-50, 0);
        actionPanel.SetActive(false);

        var endTurnBtn = UIThemeManager.CreateButton(root.transform, "EndTurn", "End Turn", new Vector2(0, -350), new Vector2(180, 45), OnEndTurn);
        var endRect = endTurnBtn.GetComponent<RectTransform>();
        endRect.anchorMin = new Vector2(0.5f, 0f);
        endRect.anchorMax = new Vector2(0.5f, 0f);
        endRect.pivot = new Vector2(0.5f, 0.5f);
        endRect.anchoredPosition = new Vector2(0, -350);

        Refresh(state);
    }

    public void Refresh(CampaignState state)
    {
        _state = state;
        if (_provinceManager == null || provinceListContent == null) return;

        foreach (Transform child in provinceListContent.transform)
            Destroy(child.gameObject);

        var provinces = ProvinceDatabase.GetAllProvinces();
        foreach (var p in provinces)
        {
            var ownerId = _provinceManager.GetOwner(p.id);
            var ownerColor = !string.IsNullOrEmpty(ownerId) ? FactionColorPalette.GetPrimaryColor(ownerId) : new Color(0.4f, 0.4f, 0.4f, 1f);

            var card = UIThemeManager.CreatePanel(provinceListContent.transform, "Province_" + p.id, Vector2.zero, new Vector2(0, 44));
            var layout = card.AddComponent<LayoutElement>();
            layout.preferredHeight = 44;
            layout.minHeight = 44;

            var colorBar = new GameObject("ColorBar");
            colorBar.transform.SetParent(card.transform, false);
            var barRect = colorBar.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(0, 1);
            barRect.sizeDelta = new Vector2(6, 0);
            barRect.offsetMin = new Vector2(0, 4);
            barRect.offsetMax = new Vector2(6, -4);
            colorBar.AddComponent<Image>().color = ownerColor;

            var ownerName = !string.IsNullOrEmpty(ownerId) ? FactionDatabase.Get(ownerId)?.displayName ?? ownerId : "Neutral";
            var nameText = UIThemeManager.CreateText(card.transform, "Name", p.displayName + " (" + ownerName + ")", new Vector2(20, 0), new Vector2(240, 40), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
            var nameRect = nameText.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(0, 0.5f);
            nameRect.pivot = new Vector2(0, 0.5f);
            nameRect.anchoredPosition = new Vector2(20, 0);

            var btn = card.AddComponent<Button>();
            var pid = p.id;
            btn.onClick.AddListener(() => OnProvinceClicked(pid));
        }
    }

    public void OnProvinceClicked(string provinceId)
    {
        _selectedProvinceId = provinceId;
        var ownerId = _provinceManager?.GetOwner(provinceId);
        var isPlayer = ownerId == _state?.playerFactionId;

        if (isPlayer)
            ShowRecruitPanel(provinceId);
        else
            ShowAttackPanel(provinceId);
    }

    public void ShowAttackPanel(string targetProvinceId)
    {
        actionPanel.SetActive(true);
        foreach (Transform c in actionPanel.transform)
            Destroy(c.gameObject);

        UIThemeManager.CreateText(actionPanel.transform, "Title", "Attack: " + (ProvinceDatabase.GetAllProvinces().FirstOrDefault(p => p.id == targetProvinceId)?.displayName ?? targetProvinceId), new Vector2(0, 160), new Vector2(300, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);

        var player = _state?.factions?.FirstOrDefault(f => f.factionId == _state.playerFactionId);
        if (player == null) return;

        var armyDict = player.armyComposition?.ToDictionary() ?? new Dictionary<string, int>();
        float y = 120;
        foreach (var kv in armyDict)
        {
            if (kv.Value <= 0) continue;
            var unit = UnitDatabase.Get(kv.Key);
            var unitName = unit?.displayName ?? kv.Key;
            var row = UIThemeManager.CreatePanel(actionPanel.transform, "UnitRow_" + kv.Key, new Vector2(0, y), new Vector2(280, 32));
            var rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = new Vector2(0, y);
            UIThemeManager.CreateText(row.transform, "Label", unitName + ": " + kv.Value, new Vector2(-80, 0), new Vector2(160, 28), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
            var commitBtn = UIThemeManager.CreateButton(row.transform, "Commit", "Commit All", new Vector2(100, 0), new Vector2(80, 28), () => { });
            var key = kv.Key;
            var count = kv.Value;
            commitBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            commitBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!_committedUnits.ContainsKey(key)) _committedUnits[key] = 0;
                _committedUnits[key] = count;
            });
            y -= 38;
        }

        var confirmBtn = UIThemeManager.CreateButton(actionPanel.transform, "ConfirmAttack", "Confirm Attack", new Vector2(0, -150), new Vector2(200, 40), () =>
        {
            var action = new CampaignAction
            {
                type = CampaignAction.ActionType.Attack,
                sourceFactionId = _state.playerFactionId,
                targetProvinceId = targetProvinceId,
                committedUnits = new SerializableKeyValueList()
            };
            action.committedUnits.FromDictionary(_committedUnits);
            CampaignManager.Instance?.SubmitPlayerAction(action);
            _committedUnits.Clear();
            actionPanel.SetActive(false);
        });
        var confirmRect = confirmBtn.GetComponent<RectTransform>();
        confirmRect.anchorMin = new Vector2(0.5f, 0.5f);
        confirmRect.anchorMax = new Vector2(0.5f, 0.5f);
        confirmRect.pivot = new Vector2(0.5f, 0.5f);
        confirmRect.anchoredPosition = new Vector2(0, -150);
    }

    public void ShowRecruitPanel(string cityId)
    {
        _selectedCityId = cityId;
        actionPanel.SetActive(true);
        foreach (Transform c in actionPanel.transform)
            Destroy(c.gameObject);

        var province = ProvinceDatabase.GetAllProvinces().FirstOrDefault(p => p.id == cityId);
        UIThemeManager.CreateText(actionPanel.transform, "Title", "Recruit at " + (province?.displayName ?? cityId), new Vector2(0, 160), new Vector2(300, 24), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);

        var player = _state?.factions?.FirstOrDefault(f => f.factionId == _state.playerFactionId);
        if (player == null) return;

        var unitTypes = UnitDatabase.GetForFaction(_state.playerFactionId);
        float y = 120;
        foreach (var ut in unitTypes)
        {
            if (ut == null) continue;
            var cost = CampaignEconomyManager.GetRecruitCost(ut.id);
            var canAfford = CampaignEconomyManager.CanRecruit(player, ut.id, 1);
            var row = UIThemeManager.CreatePanel(actionPanel.transform, "RecruitRow_" + ut.id, new Vector2(0, y), new Vector2(280, 36));
            var rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = new Vector2(0, y);
            UIThemeManager.CreateText(row.transform, "Label", ut.displayName + " (" + cost + "g)", new Vector2(-60, 0), new Vector2(180, 28), UIThemeManager.BodyFontSize, TextAnchor.MiddleLeft);
            var recruitBtn = UIThemeManager.CreateButton(row.transform, "Recruit", "Recruit", new Vector2(100, 0), new Vector2(70, 28), () => { });
            recruitBtn.GetComponent<Button>().interactable = canAfford;
            var unitId = ut.id;
            recruitBtn.GetComponent<Button>().onClick.RemoveAllListeners();
            recruitBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (CampaignEconomyManager.CanRecruit(player, unitId, 1))
                {
                    CampaignManager.Instance?.SubmitPlayerAction(new CampaignAction
                    {
                        type = CampaignAction.ActionType.Recruit,
                        sourceFactionId = _state.playerFactionId,
                        recruitUnitTypeId = unitId,
                        recruitCount = 1
                    });
                    actionPanel.SetActive(false);
                }
            });
            y -= 42;
        }

        var closeBtn = UIThemeManager.CreateButton(actionPanel.transform, "Close", "Close", new Vector2(0, -160), new Vector2(120, 36), () => actionPanel.SetActive(false));
        var closeRect = closeBtn.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.5f, 0.5f);
        closeRect.anchorMax = new Vector2(0.5f, 0.5f);
        closeRect.pivot = new Vector2(0.5f, 0.5f);
        closeRect.anchoredPosition = new Vector2(0, -160);
    }

    public void OnEndTurn()
    {
        if (CampaignManager.Instance == null) return;
        CampaignManager.Instance.ExecuteTurn();
        Refresh(CampaignManager.Instance.GetCurrentState());
    }

    public void ShowTurnResolution(List<BattleResult> results)
    {
        if (results == null || results.Count == 0) return;

        var overlay = UIThemeManager.CreatePanel(root.transform, "TurnResolutionOverlay", Vector2.zero, new Vector2(1920, 1080));
        var overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        overlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);

        var panel = UIThemeManager.CreatePanel(overlay.transform, "ResultsPanel", new Vector2(0, 0), new Vector2(500, 400));
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;

        UIThemeManager.CreateText(panel.transform, "Title", "Battle Results", new Vector2(0, 160), new Vector2(460, 30), UIThemeManager.HeaderFontSize, TextAnchor.MiddleCenter);
        float y = 120;
        foreach (var r in results)
        {
            var winner = FactionDatabase.Get(r.winnerFactionId);
            var winnerName = winner?.displayName ?? r.winnerFactionId ?? "Unknown";
            var text = winnerName + " defeated " + (FactionDatabase.Get(r.loserFactionId)?.displayName ?? r.loserFactionId) + " (" + r.totalCasualties + " casualties)";
            UIThemeManager.CreateText(panel.transform, "Result_" + y, text, new Vector2(0, y), new Vector2(460, 22), UIThemeManager.BodyFontSize, TextAnchor.MiddleCenter);
            y -= 28;
        }

        var dismissBtn = UIThemeManager.CreateButton(panel.transform, "Dismiss", "Continue", new Vector2(0, -160), new Vector2(140, 40), () =>
        {
            Destroy(overlay);
            if (CampaignManager.Instance != null)
                Refresh(CampaignManager.Instance.GetCurrentState());
        });
        var dismissRect = dismissBtn.GetComponent<RectTransform>();
        dismissRect.anchorMin = new Vector2(0.5f, 0.5f);
        dismissRect.anchorMax = new Vector2(0.5f, 0.5f);
        dismissRect.pivot = new Vector2(0.5f, 0.5f);
        dismissRect.anchoredPosition = new Vector2(0, -160);
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
        provinceListContent = null;
        actionPanel = null;
        _state = null;
        _provinceManager = null;
        _committedUnits.Clear();
    }
}
