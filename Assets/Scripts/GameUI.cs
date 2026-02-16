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

        // Create an EventSystem if one doesn't exist (required for button clicks)
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        CreateFactionCounters(canvasObj.transform);
        CreateSelectedInfoPanel(canvasObj.transform);
        CreateGameOverPanel(canvasObj.transform);
        CreateTitleText(canvasObj.transform);
    }

    void CreateFactionCounters(Transform parent)
    {
        GameObject northObj = CreateTextObject(parent, "NorthCount",
            new Vector2(20, -20), new Vector2(300, 40),
            TextAnchor.UpperLeft);
        northCountText = northObj.GetComponent<Text>();
        northCountText.color = new Color(0.3f, 0.5f, 1f);
        northCountText.fontSize = 22;

        GameObject southObj = CreateTextObject(parent, "SouthCount",
            new Vector2(20, -60), new Vector2(300, 40),
            TextAnchor.UpperLeft);
        southCountText = southObj.GetComponent<Text>();
        southCountText.color = new Color(1f, 0.3f, 0.3f);
        southCountText.fontSize = 22;
    }

    void CreateSelectedInfoPanel(Transform parent)
    {
        GameObject obj = CreateTextObject(parent, "SelectedInfo",
            new Vector2(20, 20), new Vector2(400, 80),
            TextAnchor.LowerLeft);
        selectedInfoText = obj.GetComponent<Text>();
        selectedInfoText.color = Color.white;
        selectedInfoText.fontSize = 18;

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
        panelBg.color = new Color(0, 0, 0, 0.7f);

        // Game over text
        GameObject textObj = CreateTextObject(gameOverPanel.transform, "GameOverText",
            new Vector2(0, 40), new Vector2(600, 100),
            TextAnchor.MiddleCenter);
        gameOverText = textObj.GetComponent<Text>();
        gameOverText.fontSize = 48;
        gameOverText.fontStyle = FontStyle.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0, 40);

        // Restart button
        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(gameOverPanel.transform, false);

        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(250, 60);
        btnRect.anchoredPosition = new Vector2(0, -50);

        Image btnBg = btnObj.AddComponent<Image>();
        btnBg.color = new Color(0.2f, 0.6f, 0.2f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnBg;
        btn.onClick.AddListener(RestartGame);

        // Button hover color
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.8f, 0.3f);
        colors.pressedColor = new Color(0.15f, 0.4f, 0.15f);
        btn.colors = colors;

        GameObject btnTextObj = CreateTextObject(btnObj.transform, "BtnText",
            Vector2.zero, new Vector2(250, 60),
            TextAnchor.MiddleCenter);
        Text btnText = btnTextObj.GetComponent<Text>();
        btnText.text = "RESTART BATTLE";
        btnText.fontSize = 26;
        btnText.fontStyle = FontStyle.Bold;
        btnText.color = Color.white;

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
            new Vector2(0, -10), new Vector2(500, 50),
            TextAnchor.UpperCenter);
        Text titleText = obj.GetComponent<Text>();
        titleText.text = "WORLD WARS - Medieval Conquest";
        titleText.fontSize = 26;
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

        // Add outline for readability
        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(1, -1);

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

        int northCount = FactionManager.Instance.GetLivingCount(Faction.North);
        int southCount = FactionManager.Instance.GetLivingCount(Faction.South);

        if (northCountText != null)
            northCountText.text = $"[YOU] Kingdom of the North: {northCount} units";
        if (southCountText != null)
            southCountText.text = $"[ENEMY] Southern Empire: {southCount} units";
    }

    void UpdateSelectedInfo()
    {
        if (selectedInfoText == null) return;

        if (SelectionManager.Instance == null || SelectionManager.Instance.selectedUnits.Count == 0)
        {
            selectedInfoText.text = "Left-click: Select unit | Drag: Select multiple | Right-click: Move/Attack";
            return;
        }

        var selected = SelectionManager.Instance.selectedUnits;
        if (selected.Count == 1)
        {
            Unit u = selected[0];
            if (u != null && !u.isDead)
            {
                selectedInfoText.text = $"{u.unitType} | HP: {u.currentHealth:F0}/{u.maxHealth:F0} | ATK: {u.attackDamage:F0} | Right-click to command";
            }
        }
        else
        {
            int swords = 0, archers = 0;
            foreach (Unit u in selected)
            {
                if (u == null || u.isDead) continue;
                if (u.unitType == UnitType.Swordsman) swords++;
                else archers++;
            }
            selectedInfoText.text = $"Selected: {selected.Count} units ({swords} Swordsmen, {archers} Archers) | Right-click to command";
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
                gameOverText.text = "VICTORY!";
                gameOverText.color = new Color(1f, 0.85f, 0.2f);
            }
            else
            {
                gameOverText.text = "DEFEAT";
                gameOverText.color = new Color(1f, 0.2f, 0.2f);
            }
        }
    }

    void RestartGame()
    {
        Debug.Log("WorldWars: Restarting game...");

        // Reset all singleton statics before reload
        Instance = null;
        FactionManager.Instance = null;
        SelectionManager.Instance = null;
        CommandManager.Instance = null;
        AIController.Instance = null;
        CameraController.Instance = null;
        MapGenerator.Instance = null;
        UnitSpawner.Instance = null;

        // Reload the scene cleanly
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
