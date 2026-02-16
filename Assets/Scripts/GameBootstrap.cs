using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        Debug.Log("WorldWars: GameBootstrap.Start() called.");
        InitializeGame();
    }

    void InitializeGame()
    {
        Debug.Log("WorldWars: Initializing game...");

        // Step 1: Set up FactionManager
        FactionManager factionMgr = gameObject.GetComponent<FactionManager>();
        if (factionMgr == null)
            factionMgr = gameObject.AddComponent<FactionManager>();
        Debug.Log("WorldWars: FactionManager ready.");

        // Step 2: Set up camera FIRST (before anything that might reference Camera.main)
        SetupCamera(100);
        Debug.Log("WorldWars: Camera ready.");

        // Step 3: Generate the map
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

        // Step 4: Apply sky color to camera
        MapGenerator.ApplySkyToCamera(Camera.main);

        // Step 5: Set up player input
        GameObject playerInput = new GameObject("PlayerInput");
        playerInput.AddComponent<SelectionManager>();
        playerInput.AddComponent<CommandManager>();
        Debug.Log("WorldWars: Player input ready.");

        // Step 6: Spawn units
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

        // Step 7: Set up UI
        GameUI gameUI = gameObject.GetComponent<GameUI>();
        if (gameUI == null)
            gameUI = gameObject.AddComponent<GameUI>();
        gameUI.Initialize();
        Debug.Log("WorldWars: UI ready.");

        // Step 8: Activate AI
        GameObject aiObj = new GameObject("EnemyAI");
        AIController ai = aiObj.AddComponent<AIController>();
        ai.Activate();
        Debug.Log("WorldWars: AI activated.");

        Debug.Log("WorldWars: Game initialized! Select your blue units and command them to battle!");
    }

    void SetupCamera(int mapSize)
    {
        // Find or create camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            // Try to find any camera
            cam = FindAnyObjectByType<Camera>();
        }
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
        }

        // Ensure it's tagged as MainCamera
        if (!cam.CompareTag("MainCamera"))
        {
            cam.gameObject.tag = "MainCamera";
        }

        // Ensure AudioListener exists
        if (cam.GetComponent<AudioListener>() == null)
        {
            cam.gameObject.AddComponent<AudioListener>();
        }

        // Ensure Universal Additional Camera Data (required for URP)
        var urpCamData = cam.GetComponent<UniversalAdditionalCameraData>();
        if (urpCamData == null)
        {
            urpCamData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        }
        urpCamData.renderType = CameraRenderType.Base;

        // Position camera
        float centerX = mapSize * 0.35f;
        float centerZ = mapSize * 0.3f;
        cam.transform.position = new Vector3(centerX, 30f, centerZ);
        cam.transform.rotation = Quaternion.Euler(50f, 0f, 0f);

        // Clip planes
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 500f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.5f, 0.7f, 0.9f);

        // Camera controller
        CameraController camCtrl = cam.gameObject.GetComponent<CameraController>();
        if (camCtrl == null)
            camCtrl = cam.gameObject.AddComponent<CameraController>();
        camCtrl.SetMapBounds(-10f, mapSize + 10f, -10f, mapSize + 10f);

        Debug.Log($"WorldWars: Camera set up at position {cam.transform.position}");
    }

    void CreateFallbackGround()
    {
        // Simple flat plane as fallback if terrain generation fails
        Debug.Log("WorldWars: Creating fallback ground plane.");
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "FallbackGround";
        ground.transform.position = new Vector3(50, 0, 50);
        ground.transform.localScale = new Vector3(10, 1, 10);

        Renderer rend = ground.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0.3f, 0.55f, 0.2f));
        rend.material = mat;
    }
}
