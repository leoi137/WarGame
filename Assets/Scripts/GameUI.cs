using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; set; }

    public bool IsGameOver { get; private set; }

    Canvas mainCanvas;
    Text northCountText;
    Text southCountText;
    Text selectedInfoText;
    Text abilityHintText;
    GameObject gameOverPanel;
    Text gameOverText;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        CreateUI();

        if (FactionManager.Instance != null)
        {
            FactionManager.Instance.OnFactionDefeated += HandleFactionDefeated;
        }
    }

    void CreateUI()
    {
        // Main Canvas
        GameObject canvasObj = new GameObject("GameUICanvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create an EventSystem if one doesn't exist
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        CreateFactionCounters(canvasObj.transform);
        CreateSelectedInfoPanel(canvasObj.transform);
        CreateAbilityHint(canvasObj.transform);
        CreateGameOverPanel(canvasObj.transform);
        CreateTitleText(canvasObj.transform);
    }

    void CreateFactionCounters(Transform parent)
    {
        // North faction breakdown
        GameObject northObj = CreateTextObject(parent, "NorthCount",
            new Vector2(20, -20), new Vector2(500, 50),
            TextAnchor.UpperLeft);
        northCountText = northObj.GetComponent<Text>();
        northCountText.color = new Color(0.3f, 0.5f, 1f);
        northCountText.fontSize = 20;

        // South faction breakdown
        GameObject southObj = CreateTextObject(parent, "SouthCount",
            new Vector2(20, -70), new Vector2(500, 50),
            TextAnchor.UpperLeft);
        southCountText = southObj.GetComponent<Text>();
        southCountText.color = new Color(1f, 0.3f, 0.3f);
        southCountText.fontSize = 20;
    }

    void CreateSelectedInfoPanel(Transform parent)
    {
        GameObject obj = CreateTextObject(parent, "SelectedInfo",
            new Vector2(20, 20), new Vector2(600, 100),
            TextAnchor.LowerLeft);
        selectedInfoText = obj.GetComponent<Text>();
        selectedInfoText.color = Color.white;
        selectedInfoText.fontSize = 17;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(20, 50);
    }

    void CreateAbilityHint(Transform parent)
    {
        GameObject obj = CreateTextObject(parent, "AbilityHint",
            new Vector2(20, 20), new Vector2(600, 30),
            TextAnchor.LowerLeft);
        abilityHintText = obj.GetComponent<Text>();
        abilityHintText.color = new Color(1f, 0.85f, 0.4f);
        abilityHintText.fontSize = 15;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(20, 20);
    }

    void CreateGameOverPanel(Transform parent)
    {
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(parent, false);

        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelBg = gameOverPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.75f);

        // Game over text
        GameObject textObj = CreateTextObject(gameOverPanel.transform, "GameOverText",
            new Vector2(0, 60), new Vector2(700, 120),
            TextAnchor.MiddleCenter);
        gameOverText = textObj.GetComponent<Text>();
        gameOverText.fontSize = 52;
        gameOverText.fontStyle = FontStyle.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0, 60);

        // Sub text
        GameObject subObj = CreateTextObject(gameOverPanel.transform, "SubText",
            new Vector2(0, 10), new Vector2(500, 40),
            TextAnchor.MiddleCenter);
        Text subText = subObj.GetComponent<Text>();
        subText.text = "The battle has ended. The fallen shall feast in Valhalla.";
        subText.fontSize = 18;
        subText.color = new Color(0.8f, 0.8f, 0.8f);

        RectTransform subRect = subObj.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.pivot = new Vector2(0.5f, 0.5f);
        subRect.anchoredPosition = new Vector2(0, 10);

        // Restart button
        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(gameOverPanel.transform, false);

        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(280, 65);
        btnRect.anchoredPosition = new Vector2(0, -50);

        Image btnBg = btnObj.AddComponent<Image>();
        btnBg.color = new Color(0.6f, 0.15f, 0.1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnBg;
        btn.onClick.AddListener(RestartGame);

        var colors = btn.colors;
        colors.highlightedColor = new Color(0.8f, 0.2f, 0.15f);
        colors.pressedColor = new Color(0.4f, 0.1f, 0.08f);
        btn.colors = colors;

        GameObject btnTextObj = CreateTextObject(btnObj.transform, "BtnText",
            Vector2.zero, new Vector2(280, 65),
            TextAnchor.MiddleCenter);
        Text btnText = btnTextObj.GetComponent<Text>();
        btnText.text = "FIGHT AGAIN";
        btnText.fontSize = 28;
        btnText.fontStyle = FontStyle.Bold;
        btnText.color = new Color(1f, 0.85f, 0.4f);

        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        gameOverPanel.SetActive(false);
    }

    void CreateTitleText(Transform parent)
    {
        GameObject obj = CreateTextObject(parent, "Title",
            new Vector2(0, -10), new Vector2(600, 55),
            TextAnchor.UpperCenter);
        Text titleText = obj.GetComponent<Text>();
        titleText.text = "WORLD WARS  -  Viking Conquest";
        titleText.fontSize = 28;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = new Color(1f, 0.85f, 0.4f);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, -10);
    }

    GameObject CreateTextObject(Transform parent, string name, Vector2 position, Vector2 size, TextAnchor anchor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.85f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return obj;
    }

    void Update()
    {
        UpdateFactionCounts();
        UpdateSelectedInfo();
    }

    void UpdateFactionCounts()
    {
        if (FactionManager.Instance == null) return;

        var northUnits = FactionManager.Instance.GetUnitsForFaction(Faction.North);
        var southUnits = FactionManager.Instance.GetUnitsForFaction(Faction.South);

        string northBreakdown = GetFactionBreakdown(northUnits);
        string southBreakdown = GetFactionBreakdown(southUnits);

        int northAlive = CountAlive(northUnits);
        int southAlive = CountAlive(southUnits);

        if (northCountText != null)
            northCountText.text = $"[YOU] Norse Warband: {northAlive} warriors  |  {northBreakdown}";
        if (southCountText != null)
            southCountText.text = $"[ENEMY] Rival Clan: {southAlive} warriors  |  {southBreakdown}";
    }

    string GetFactionBreakdown(System.Collections.Generic.List<Unit> units)
    {
        int swords = 0, archers = 0, berserkers = 0, shields = 0;
        foreach (Unit u in units)
        {
            if (u == null || u.isDead) continue;
            switch (u.unitType)
            {
                case UnitType.Swordsman: swords++; break;
                case UnitType.Archer: archers++; break;
                case UnitType.Berserker: berserkers++; break;
                case UnitType.Shieldbearer: shields++; break;
            }
        }

        string result = "";
        if (shields > 0) result += $"{shields} Shieldbearers  ";
        if (swords > 0) result += $"{swords} Swordsmen  ";
        if (berserkers > 0) result += $"{berserkers} Berserkers  ";
        if (archers > 0) result += $"{archers} Archers";
        return result.Trim();
    }

    int CountAlive(System.Collections.Generic.List<Unit> units)
    {
        int count = 0;
        foreach (Unit u in units)
        {
            if (u != null && !u.isDead) count++;
        }
        return count;
    }

    void UpdateSelectedInfo()
    {
        if (selectedInfoText == null) return;

        if (SelectionManager.Instance == null || SelectionManager.Instance.selectedUnits.Count == 0)
        {
            selectedInfoText.text = "Left-click: Select | Drag: Box select | Right-click: Move/Attack";
            if (abilityHintText != null)
                abilityHintText.text = "";
            return;
        }

        var selected = SelectionManager.Instance.selectedUnits;
        if (selected.Count == 1)
        {
            Unit u = selected[0];
            if (u != null && !u.isDead)
            {
                string status = GetUnitStatusText(u);
                selectedInfoText.text = $"{GetUnitTitle(u.unitType)} | HP: {u.currentHealth:F0}/{u.maxHealth:F0} | ATK: {u.attackDamage:F0} | ARM: {u.armor:F0} | SPD: {u.moveSpeed:F1}{status}";

                if (abilityHintText != null)
                    abilityHintText.text = GetAbilityHint(u.unitType);
            }
        }
        else
        {
            int swords = 0, archers = 0, berserkers = 0, shields = 0;
            foreach (Unit u in selected)
            {
                if (u == null || u.isDead) continue;
                switch (u.unitType)
                {
                    case UnitType.Swordsman: swords++; break;
                    case UnitType.Archer: archers++; break;
                    case UnitType.Berserker: berserkers++; break;
                    case UnitType.Shieldbearer: shields++; break;
                }
            }
            string breakdown = "";
            if (shields > 0) breakdown += $"{shields} Shieldbearers, ";
            if (swords > 0) breakdown += $"{swords} Swordsmen, ";
            if (berserkers > 0) breakdown += $"{berserkers} Berserkers, ";
            if (archers > 0) breakdown += $"{archers} Archers, ";
            breakdown = breakdown.TrimEnd(',', ' ');

            selectedInfoText.text = $"Selected: {selected.Count} Vikings ({breakdown})";
            if (abilityHintText != null)
                abilityHintText.text = "Right-click ground to move | Right-click enemy to attack";
        }
    }

    string GetUnitTitle(UnitType type)
    {
        switch (type)
        {
            case UnitType.Swordsman: return "Huscarl Swordsman";
            case UnitType.Archer: return "Norse Hunter";
            case UnitType.Berserker: return "Berserker";
            case UnitType.Shieldbearer: return "Shieldbearer";
            default: return type.ToString();
        }
    }

    string GetUnitStatusText(Unit u)
    {
        string status = "";
        if (u.isEnraged) status += " | ENRAGED!";
        if (u.isShieldWalling) status += " | SHIELD WALL";
        if (u.isMarked) status += " | MARKED";
        return status;
    }

    string GetAbilityHint(UnitType type)
    {
        switch (type)
        {
            case UnitType.Berserker: return "Passive: Auto-rages at 40% HP (+60% DMG, +30% SPD, -40% ARM for 6s)";
            case UnitType.Shieldbearer: return "Passive: Shield Wall near enemies (+15 ARM, -70% speed)";
            case UnitType.Archer: return "Passive: Marks enemies every 6s (+40% damage from all sources)";
            case UnitType.Swordsman: return "Passive: Parries attacks every 8s (blocks 60% damage for 1.5s)";
            default: return "";
        }
    }

    void HandleFactionDefeated(Faction defeated)
    {
        IsGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (defeated == Faction.South)
            {
                gameOverText.text = "VICTORY! SKAL!";
                gameOverText.color = new Color(1f, 0.85f, 0.2f);
            }
            else
            {
                gameOverText.text = "DEFEAT... TO VALHALLA";
                gameOverText.color = new Color(1f, 0.2f, 0.2f);
            }
        }
    }

    void RestartGame()
    {
        Debug.Log("WorldWars: Restarting battle...");

        Instance = null;
        FactionManager.Instance = null;
        SelectionManager.Instance = null;
        CommandManager.Instance = null;
        AIController.Instance = null;
        CameraController.Instance = null;
        MapGenerator.Instance = null;
        UnitSpawner.Instance = null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
