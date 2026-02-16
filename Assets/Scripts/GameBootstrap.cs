using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class GameBootstrap : MonoBehaviour
{
    Canvas menuCanvas;
    bool gameStarted;

    void Start()
    {
        Debug.Log("WorldWars: GameBootstrap.Start() called.");
        SetupCamera(100);
        ShowMainMenu();
    }

    // ========== MAIN MENU ==========

    void ShowMainMenu()
    {
        // Create menu canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        menuCanvas = canvasObj.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 200;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Dark backdrop
        GameObject bgObj = new GameObject("MenuBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.06f, 0.06f, 0.1f, 0.92f);

        // Title
        CreateMenuText(canvasObj.transform, "Title", "WORLD WARS",
            new Vector2(0, 100), new Vector2(700, 80), 58, FontStyle.Bold,
            new Color(1f, 0.85f, 0.35f));

        // Subtitle
        CreateMenuText(canvasObj.transform, "Subtitle", "Viking Conquest",
            new Vector2(0, 45), new Vector2(500, 45), 28, FontStyle.Normal,
            new Color(0.8f, 0.7f, 0.5f));

        // Decorative line
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(canvasObj.transform, false);
        RectTransform lineRect = lineObj.AddComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);
        lineRect.sizeDelta = new Vector2(350, 2);
        lineRect.anchoredPosition = new Vector2(0, 15);
        Image lineImg = lineObj.AddComponent<Image>();
        lineImg.color = new Color(0.7f, 0.6f, 0.3f, 0.5f);

        // START BATTLE button
        CreateMenuButton(canvasObj.transform, "StartBattle", "START BATTLE",
            new Vector2(0, -40), new Vector2(320, 65),
            new Color(0.5f, 0.12f, 0.08f), new Color(1f, 0.85f, 0.4f), 28,
            () => StartBattle());

        // VIEW UNITS button
        CreateMenuButton(canvasObj.transform, "ViewUnits", "VIEW UNITS",
            new Vector2(0, -120), new Vector2(320, 65),
            new Color(0.2f, 0.2f, 0.28f), new Color(0.9f, 0.85f, 0.7f), 28,
            () => OpenUnitViewer());

        // Footer
        CreateMenuText(canvasObj.transform, "Footer",
            "Select your Norse warriors and lead them to glory",
            new Vector2(0, -200), new Vector2(600, 30), 15, FontStyle.Italic,
            new Color(0.5f, 0.5f, 0.5f));

        // Version
        CreateMenuText(canvasObj.transform, "Version", "v0.2",
            new Vector2(0, -230), new Vector2(100, 25), 13, FontStyle.Normal,
            new Color(0.35f, 0.35f, 0.35f));
    }

    void CreateMenuText(Transform parent, string name, string content,
        Vector2 pos, Vector2 size, int fontSize, FontStyle style, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;

        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);
    }

    void CreateMenuButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size, Color bgColor, Color textColor, int fontSize,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Image bg = btnObj.AddComponent<Image>();
        bg.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bg;
        btn.onClick.AddListener(onClick);

        var colors = btn.colors;
        colors.highlightedColor = bgColor * 1.5f;
        colors.pressedColor = bgColor * 0.6f;
        btn.colors = colors;

        // Border
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(btnObj.transform, false);
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;
        Outline border = borderObj.AddComponent<Outline>();
        border.effectColor = new Color(0.7f, 0.6f, 0.3f, 0.4f);
        border.effectDistance = new Vector2(2, -2);
        // Need an Image for Outline to work on
        Image borderImg = borderObj.AddComponent<Image>();
        borderImg.color = new Color(0, 0, 0, 0); // Transparent
        borderImg.raycastTarget = false;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = textColor;
        text.alignment = TextAnchor.MiddleCenter;

        Outline textOutline = textObj.AddComponent<Outline>();
        textOutline.effectColor = new Color(0, 0, 0, 0.85f);
        textOutline.effectDistance = new Vector2(1.5f, -1.5f);
    }

    // ========== ACTIONS ==========

    void StartBattle()
    {
        if (gameStarted) return;
        gameStarted = true;

        // Destroy menu
        if (menuCanvas != null) Destroy(menuCanvas.gameObject);

        // Start the game
        InitializeGame();
    }

    void OpenUnitViewer()
    {
        if (UnitViewer.Instance != null) return;

        // Hide menu while viewer is open
        if (menuCanvas != null) menuCanvas.gameObject.SetActive(false);

        GameObject viewerObj = new GameObject("UnitViewer");
        UnitViewer viewer = viewerObj.AddComponent<UnitViewer>();
        viewer.OnViewerClosed = OnViewerClosed;
        viewer.Open();
    }

    void OnViewerClosed()
    {
        // Show menu again
        if (menuCanvas != null) menuCanvas.gameObject.SetActive(true);
    }

    // ========== GAME INITIALIZATION ==========

    void InitializeGame()
    {
        Debug.Log("WorldWars: Initializing game...");

        // Step 1: FactionManager
        FactionManager factionMgr = gameObject.GetComponent<FactionManager>();
        if (factionMgr == null)
            factionMgr = gameObject.AddComponent<FactionManager>();

        // Step 2: Generate map
        MapGenerator mapGen = gameObject.GetComponent<MapGenerator>();
        if (mapGen == null)
            mapGen = gameObject.AddComponent<MapGenerator>();

        try
        {
            mapGen.Generate();
            Debug.Log("WorldWars: Map generated.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WorldWars: Map generation failed: {e.Message}\n{e.StackTrace}");
            CreateFallbackGround();
        }

        // Step 3: Apply sky
        MapGenerator.ApplySkyToCamera(Camera.main);

        // Step 4: Player input
        GameObject playerInput = new GameObject("PlayerInput");
        playerInput.AddComponent<SelectionManager>();
        playerInput.AddComponent<CommandManager>();

        // Step 5: Spawn units
        UnitSpawner spawner = gameObject.GetComponent<UnitSpawner>();
        if (spawner == null)
            spawner = gameObject.AddComponent<UnitSpawner>();

        try
        {
            spawner.SpawnAllUnits(mapGen.mapSize);
            Debug.Log("WorldWars: Units spawned.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WorldWars: Unit spawning failed: {e.Message}\n{e.StackTrace}");
        }

        // Step 6: Game UI
        GameUI gameUI = gameObject.GetComponent<GameUI>();
        if (gameUI == null)
            gameUI = gameObject.AddComponent<GameUI>();
        gameUI.Initialize();

        // Step 7: AI
        GameObject aiObj = new GameObject("EnemyAI");
        AIController ai = aiObj.AddComponent<AIController>();
        ai.Activate();

        Debug.Log("WorldWars: Battle started!");
    }

    // ========== CAMERA ==========

    void SetupCamera(int mapSize)
    {
        Camera cam = Camera.main;
        if (cam == null)
            cam = FindAnyObjectByType<Camera>();
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
        }

        if (!cam.CompareTag("MainCamera"))
            cam.gameObject.tag = "MainCamera";

        if (cam.GetComponent<AudioListener>() == null)
            cam.gameObject.AddComponent<AudioListener>();

        var urpCamData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (urpCamData == null)
            urpCamData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        urpCamData.renderType = CameraRenderType.Base;

        float centerX = mapSize * 0.35f;
        float centerZ = mapSize * 0.3f;
        cam.transform.position = new Vector3(centerX, 30f, centerZ);
        cam.transform.rotation = Quaternion.Euler(50f, 0f, 0f);

        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 500f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.5f, 0.7f, 0.9f);

        CameraController camCtrl = cam.gameObject.GetComponent<CameraController>();
        if (camCtrl == null)
            camCtrl = cam.gameObject.AddComponent<CameraController>();
        camCtrl.SetMapBounds(-10f, mapSize + 10f, -10f, mapSize + 10f);
    }

    void CreateFallbackGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "FallbackGround";
        ground.transform.position = new Vector3(50, 0, 50);
        ground.transform.localScale = new Vector3(10, 1, 10);

        Renderer rend = ground.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(0.3f, 0.55f, 0.2f));
    }
}
