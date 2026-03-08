using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Entry point for the game. Uses [RuntimeInitializeOnLoadMethod] so it
/// executes automatically when Play starts — no scene binding required.
/// Initializes databases and camera, then hands control to GameManager
/// which drives all UI flow and state transitions.
/// </summary>
public static class GameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        Debug.Log("WorldWars: GameBootstrap.Initialize() called.");

        AbilityDatabase.Initialize();
        TerrainDatabase.Initialize();
        FactionDatabase.Initialize();
        UnitDatabase.Initialize();

        SetupCamera(GameConfig.DefaultMapSize);

        if (GameManager.Instance == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }

        Debug.Log("WorldWars: Ready. GameManager driving UI flow.");
    }

    static void SetupCamera(int mapSize)
    {
        Camera cam = Camera.main;
        if (cam == null)
            cam = Object.FindAnyObjectByType<Camera>();
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
}
