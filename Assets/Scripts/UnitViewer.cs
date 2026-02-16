using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.AI;

/// <summary>
/// Unit Viewer / Showcase mode. Spawns a single unit on a pedestal with
/// a dedicated camera. Lets you cycle through all 4 unit types, both factions,
/// rotate with mouse drag, zoom with scroll, and trigger animations/abilities.
/// </summary>
public class UnitViewer : MonoBehaviour
{
    public static UnitViewer Instance { get; private set; }

    // Callback when viewer closes (so menu can reappear)
    public System.Action OnViewerClosed;

    // Current showcase state
    UnitType currentType = UnitType.Swordsman;
    Faction currentFaction = Faction.North;
    GameObject currentUnit;
    UnitAnimator currentAnimator;
    Unit currentUnitScript;

    // Viewer camera & environment
    Camera viewerCamera;
    GameObject pedestal;
    GameObject backdrop;
    GameObject viewerLight;
    GameObject viewerRoot;
    Canvas viewerCanvas;

    // Camera orbit
    float orbitAngle = 180f;
    float orbitPitch = 20f;
    float orbitDistance = 4.5f;
    float targetOrbitDistance = 4.5f;
    float orbitDistVelocity;
    Vector3 orbitTarget = new Vector3(0, 1.2f, 0);
    bool isDragging;
    Vector2 lastMousePos;

    // UI elements
    Text unitNameText;
    Text unitStatsText;
    Text unitAbilityText;
    Text controlsHintText;

    // Animation preview state
    bool isPlayingIdle = true;

    void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        if (viewerRoot != null) return; // Already open

        // Pause the game
        Time.timeScale = 0f;

        // Disable main camera
        if (Camera.main != null)
            Camera.main.gameObject.SetActive(false);

        CreateViewerEnvironment();
        CreateViewerUI();
        SpawnCurrentUnit();
    }

    public void Close()
    {
        Time.timeScale = 1f;

        // Re-enable main camera
        Camera cam = FindMainCamera();
        if (cam != null)
            cam.gameObject.SetActive(true);

        // Re-enable main camera audio listener
        if (cam != null)
        {
            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }

        if (viewerRoot != null) Destroy(viewerRoot);
        if (viewerCanvas != null) Destroy(viewerCanvas.gameObject);
        if (currentUnit != null) Destroy(currentUnit);

        viewerRoot = null;
        viewerCanvas = null;
        currentUnit = null;
        Instance = null;

        // Notify caller (main menu) that viewer closed
        OnViewerClosed?.Invoke();

        Destroy(gameObject);
    }

    Camera FindMainCamera()
    {
        foreach (Camera c in FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (c.CompareTag("MainCamera") && c != viewerCamera)
                return c;
        }
        return null;
    }

    void CreateViewerEnvironment()
    {
        viewerRoot = new GameObject("UnitViewerRoot");

        // Viewer camera
        GameObject camObj = new GameObject("ViewerCamera");
        camObj.transform.SetParent(viewerRoot.transform);
        viewerCamera = camObj.AddComponent<Camera>();
        viewerCamera.clearFlags = CameraClearFlags.SolidColor;
        viewerCamera.backgroundColor = new Color(0.45f, 0.6f, 0.8f);
        viewerCamera.nearClipPlane = 0.1f;
        viewerCamera.farClipPlane = 100f;
        viewerCamera.depth = 10; // Render on top
        camObj.AddComponent<AudioListener>();

        // Disable main camera's audio listener to avoid warning
        Camera mainCam = FindMainCamera();
        if (mainCam != null)
        {
            AudioListener mainListener = mainCam.GetComponent<AudioListener>();
            if (mainListener != null) mainListener.enabled = false;
        }

        // Simple ground pedestal (no walls, fully open)
        pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.name = "Pedestal";
        pedestal.transform.SetParent(viewerRoot.transform);
        pedestal.transform.position = new Vector3(0, -0.05f, 0);
        pedestal.transform.localScale = new Vector3(3f, 0.1f, 3f);
        Renderer pedRend = pedestal.GetComponent<Renderer>();
        pedRend.material = ShaderHelper.CreateMaterial(new Color(0.3f, 0.25f, 0.2f));

        // Inner ring accent
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "PedestalRing";
        ring.transform.SetParent(viewerRoot.transform);
        ring.transform.position = new Vector3(0, -0.03f, 0);
        ring.transform.localScale = new Vector3(2.2f, 0.08f, 2.2f);
        Renderer ringRend = ring.GetComponent<Renderer>();
        ringRend.material = ShaderHelper.CreateMaterial(new Color(0.4f, 0.33f, 0.25f));
        Destroy(ring.GetComponent<Collider>());

        // Directional light for the viewer
        viewerLight = new GameObject("ViewerLight");
        viewerLight.transform.SetParent(viewerRoot.transform);
        viewerLight.transform.rotation = Quaternion.Euler(40f, -30f, 0f);
        Light light = viewerLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.95f, 0.85f);

        // Fill light
        GameObject fillLightObj = new GameObject("FillLight");
        fillLightObj.transform.SetParent(viewerRoot.transform);
        fillLightObj.transform.position = new Vector3(-3f, 2f, 2f);
        Light fillLight = fillLightObj.AddComponent<Light>();
        fillLight.type = LightType.Point;
        fillLight.intensity = 0.6f;
        fillLight.range = 10f;
        fillLight.color = new Color(0.7f, 0.8f, 1f);

        // Rim light
        GameObject rimLightObj = new GameObject("RimLight");
        rimLightObj.transform.SetParent(viewerRoot.transform);
        rimLightObj.transform.position = new Vector3(2f, 3f, -2f);
        Light rimLight = rimLightObj.AddComponent<Light>();
        rimLight.type = LightType.Point;
        rimLight.intensity = 0.4f;
        rimLight.range = 8f;
        rimLight.color = new Color(1f, 0.85f, 0.6f);

        UpdateCameraOrbit();
    }

    void CreateViewerUI()
    {
        GameObject canvasObj = new GameObject("ViewerCanvas");
        viewerCanvas = canvasObj.AddComponent<Canvas>();
        viewerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        viewerCanvas.sortingOrder = 200;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Title
        unitNameText = CreateUIText(canvasObj.transform, "UnitName",
            new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(600, 50),
            "", 32, FontStyle.Bold, new Color(1f, 0.85f, 0.4f), TextAnchor.UpperCenter);

        // Stats panel (left side)
        unitStatsText = CreateUIText(canvasObj.transform, "Stats",
            new Vector2(0f, 0.5f), new Vector2(25, 0), new Vector2(400, 300),
            "", 17, FontStyle.Normal, Color.white, TextAnchor.MiddleLeft);

        // Ability text (left side below stats)
        unitAbilityText = CreateUIText(canvasObj.transform, "Ability",
            new Vector2(0f, 0f), new Vector2(25, 120), new Vector2(450, 100),
            "", 15, FontStyle.Normal, new Color(1f, 0.85f, 0.4f), TextAnchor.LowerLeft);

        // Controls hint (bottom center)
        controlsHintText = CreateUIText(canvasObj.transform, "Controls",
            new Vector2(0.5f, 0f), new Vector2(0, 15), new Vector2(800, 35),
            "Drag: Rotate  |  Scroll: Zoom  |  1-4: Animations  |  F: Faction  |  ESC: Close",
            14, FontStyle.Normal, new Color(0.7f, 0.7f, 0.7f), TextAnchor.LowerCenter);

        // Navigation buttons
        float btnY = 0f;
        CreateButton(canvasObj.transform, "PrevBtn", "<", new Vector2(0f, 0.5f), new Vector2(25, btnY),
            new Vector2(50, 50), () => CycleUnit(-1));
        CreateButton(canvasObj.transform, "NextBtn", ">", new Vector2(1f, 0.5f), new Vector2(-25, btnY),
            new Vector2(50, 50), () => CycleUnit(1));

        // Action buttons (right side)
        float rightX = -25f;
        CreateButton(canvasObj.transform, "IdleBtn", "IDLE", new Vector2(1f, 1f), new Vector2(rightX, -25),
            new Vector2(130, 40), () => TriggerAnim("idle"));
        CreateButton(canvasObj.transform, "WalkBtn", "WALK", new Vector2(1f, 1f), new Vector2(rightX, -70),
            new Vector2(130, 40), () => TriggerAnim("walk"));
        CreateButton(canvasObj.transform, "AttackBtn", "ATTACK", new Vector2(1f, 1f), new Vector2(rightX, -115),
            new Vector2(130, 40), () => TriggerAnim("attack"));
        CreateButton(canvasObj.transform, "AbilityBtn", "ABILITY", new Vector2(1f, 1f), new Vector2(rightX, -160),
            new Vector2(130, 40), () => TriggerAbility());
        CreateButton(canvasObj.transform, "FactionBtn", "FACTION", new Vector2(1f, 1f), new Vector2(rightX, -210),
            new Vector2(130, 40), () => ToggleFaction());
        CreateButton(canvasObj.transform, "CloseBtn", "CLOSE [ESC]", new Vector2(1f, 1f), new Vector2(rightX, -265),
            new Vector2(130, 40), () => Close(), new Color(0.6f, 0.15f, 0.1f));

        UpdateUI();
    }

    Text CreateUIText(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size,
        string content, int fontSize, FontStyle style, Color color, TextAnchor alignment)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return text;
    }

    void CreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 pos,
        Vector2 size, UnityEngine.Events.UnityAction onClick, Color? bgColor = null)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Image bg = btnObj.AddComponent<Image>();
        bg.color = bgColor ?? new Color(0.25f, 0.25f, 0.3f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bg;
        btn.onClick.AddListener(onClick);

        var colors = btn.colors;
        Color baseCol = bgColor ?? new Color(0.25f, 0.25f, 0.3f);
        colors.highlightedColor = baseCol * 1.4f;
        colors.pressedColor = baseCol * 0.7f;
        btn.colors = colors;

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
        text.fontSize = 16;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(1f, 0.9f, 0.7f);
        text.alignment = TextAnchor.MiddleCenter;

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(1, -1);
    }

    // ========== UNIT MANAGEMENT ==========

    void SpawnCurrentUnit()
    {
        if (currentUnit != null) Destroy(currentUnit);

        currentUnit = new GameObject($"Viewer_{currentFaction}_{currentType}");
        currentUnit.transform.position = Vector3.zero;
        currentUnit.transform.rotation = Quaternion.Euler(0, 180f, 0);

        // CapsuleCollider (needed by some scripts)
        CapsuleCollider col = currentUnit.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1.1f, 0);
        col.radius = 0.4f;
        col.height = 2.2f;

        // NavMeshAgent -- disable it since there's no NavMesh
        // We won't add NavMeshAgent in the viewer to avoid errors.

        // Unit component
        currentUnitScript = currentUnit.AddComponent<Unit>();
        currentUnitScript.faction = currentFaction;
        currentUnitScript.unitType = currentType;
        currentUnitScript.useUnscaledTime = true;
        currentUnitScript.Initialize();

        // Animator (use unscaled time since game is paused)
        currentAnimator = currentUnit.AddComponent<UnitAnimator>();
        currentAnimator.useUnscaledTime = true;
        currentAnimator.head = currentUnitScript.partHead;
        currentAnimator.body = currentUnitScript.partBody;
        currentAnimator.leftArm = currentUnitScript.partLeftArm;
        currentAnimator.rightArm = currentUnitScript.partRightArm;
        currentAnimator.leftLeg = currentUnitScript.partLeftLeg;
        currentAnimator.rightLeg = currentUnitScript.partRightLeg;
        currentAnimator.weapon = currentUnitScript.partWeapon;
        currentAnimator.weaponLeft = currentUnitScript.partWeaponLeft;
        currentAnimator.InitializeRests();

        UpdateUI();
    }

    void CycleUnit(int direction)
    {
        int typeCount = System.Enum.GetValues(typeof(UnitType)).Length;
        int idx = (int)currentType + direction;
        if (idx < 0) idx = typeCount - 1;
        if (idx >= typeCount) idx = 0;
        currentType = (UnitType)idx;
        SpawnCurrentUnit();
    }

    void ToggleFaction()
    {
        currentFaction = (currentFaction == Faction.North) ? Faction.South : Faction.North;
        SpawnCurrentUnit();
    }

    // ========== ANIMATION TRIGGERS ==========

    void TriggerAnim(string anim)
    {
        if (currentAnimator == null) return;

        switch (anim)
        {
            case "idle":
                currentAnimator.currentState = UnitAnimator.AnimState.Idle;
                break;
            case "walk":
                currentAnimator.currentState = UnitAnimator.AnimState.Walking;
                break;
            case "attack":
                currentAnimator.PlayAttackAnimation();
                break;
        }
    }

    void TriggerAbility()
    {
        if (currentUnitScript == null) return;

        switch (currentType)
        {
            case UnitType.Berserker:
                // Reset rage state first so we can trigger again
                currentUnitScript.isEnraged = false;
                currentUnitScript.ApplyStats();
                currentUnitScript.ActivateRage();
                break;
            case UnitType.Shieldbearer:
                currentUnitScript.ActivateShieldWall();
                break;
            case UnitType.Archer:
                // Show the mark effect on self for demo
                currentUnitScript.isMarked = false;
                currentUnitScript.ApplyMark(5f);
                break;
            case UnitType.Swordsman:
                // Flash parry effect
                TriggerAnim("attack");
                break;
        }
    }

    // ========== UI UPDATE ==========

    void UpdateUI()
    {
        if (unitNameText == null) return;

        string factionLabel = currentFaction == Faction.North ? "[NORSE]" : "[RIVAL]";
        Color factionColor = currentFaction == Faction.North
            ? new Color(0.3f, 0.5f, 1f)
            : new Color(1f, 0.35f, 0.3f);

        string fullName = $"{factionLabel}  {GetUnitTitle(currentType)}";
        unitNameText.text = fullName;
        unitNameText.color = factionColor;

        // Stats
        if (currentUnitScript != null)
        {
            unitStatsText.text =
                $"Health:       {currentUnitScript.maxHealth:F0}\n" +
                $"Attack:       {currentUnitScript.attackDamage:F0}\n" +
                $"Range:        {currentUnitScript.attackRange:F1}\n" +
                $"Cooldown:   {currentUnitScript.attackCooldown:F1}s\n" +
                $"Speed:        {currentUnitScript.moveSpeed:F1}\n" +
                $"Armor:        {currentUnitScript.armor:F0}";
        }

        // Ability
        unitAbilityText.text = GetAbilityDescription(currentType);
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

    string GetAbilityDescription(UnitType type)
    {
        switch (type)
        {
            case UnitType.Swordsman:
                return "PARRY: Every 8s, blocks 60% damage for 1.5s\n" +
                       "Role: Balanced fighter -- reliable in all situations";
            case UnitType.Archer:
                return "MARK: Every 6s, marks a target for +40% damage from all sources (5s)\n" +
                       "Role: Ranged DPS -- fast, precise, fragile";
            case UnitType.Berserker:
                return "RAGE: At 40% HP, +60% DMG, +30% SPD, -40% ARM for 6s\n" +
                       "DUAL AXE: 30% chance for bonus swing (50% extra damage)\n" +
                       "FEAR: Slows enemies below 30% HP\n" +
                       "Role: Glass cannon -- high risk, high reward";
            case UnitType.Shieldbearer:
                return "SHIELD WALL: +15 armor, -70% speed (toggle near enemies)\n" +
                       "SHIELD BASH: Pushes enemies back on each hit\n" +
                       "Role: Tank -- absorbs damage, holds the line";
            default: return "";
        }
    }

    // ========== CAMERA ORBIT ==========

    void Update()
    {
        // Use unscaled time since game is paused
        HandleOrbitInput();
        HandleKeyboardShortcuts();

        // Smooth zoom
        orbitDistance = Mathf.SmoothDamp(orbitDistance, targetOrbitDistance, ref orbitDistVelocity, 0.1f,
            Mathf.Infinity, Time.unscaledDeltaTime);
        UpdateCameraOrbit();

        // Slowly rotate the unit on pedestal when not dragging
        if (!isDragging && currentUnit != null)
        {
            orbitAngle += 15f * Time.unscaledDeltaTime;
        }
    }

    void HandleOrbitInput()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Right mouse or left mouse drag to orbit
        bool mouseDown = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;

        // Only count as drag if not over a UI button (simple: check Y > 100 and X > 200)
        if (mouseDown)
        {
            if (!isDragging)
            {
                isDragging = true;
                lastMousePos = mousePos;
            }

            Vector2 delta = mousePos - lastMousePos;
            orbitAngle += delta.x * 0.4f;
            orbitPitch -= delta.y * 0.3f;
            orbitPitch = Mathf.Clamp(orbitPitch, -10f, 80f);
            lastMousePos = mousePos;
        }
        else
        {
            isDragging = false;
        }

        // Scroll to zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetOrbitDistance -= Mathf.Sign(scroll) * 0.8f;
            targetOrbitDistance = Mathf.Clamp(targetOrbitDistance, 1.5f, 10f);
        }
    }

    void HandleKeyboardShortcuts()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            CycleUnit(-1);
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            CycleUnit(1);
        if (Keyboard.current.fKey.wasPressedThisFrame)
            ToggleFaction();
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TriggerAnim("idle");
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TriggerAnim("walk");
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TriggerAnim("attack");
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            TriggerAbility();
    }

    void UpdateCameraOrbit()
    {
        if (viewerCamera == null) return;

        float rad = orbitAngle * Mathf.Deg2Rad;
        float pitchRad = orbitPitch * Mathf.Deg2Rad;

        float x = Mathf.Sin(rad) * Mathf.Cos(pitchRad) * orbitDistance;
        float y = Mathf.Sin(pitchRad) * orbitDistance + orbitTarget.y;
        float z = Mathf.Cos(rad) * Mathf.Cos(pitchRad) * orbitDistance;

        viewerCamera.transform.position = orbitTarget + new Vector3(x, y - orbitTarget.y + Mathf.Sin(pitchRad) * orbitDistance, z);
        viewerCamera.transform.position = new Vector3(x, Mathf.Max(0.3f, Mathf.Sin(pitchRad) * orbitDistance + orbitTarget.y), z);
        viewerCamera.transform.LookAt(orbitTarget);
    }

    void OnDestroy()
    {
        // Ensure time scale is restored if destroyed unexpectedly
        Time.timeScale = 1f;
    }
}
