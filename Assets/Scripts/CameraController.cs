using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; set; }

    [Header("Movement")]
    public float panSpeed = 20f;
    public float edgeScrollSpeed = 15f;
    public float edgeScrollThreshold = 15f;

    [Header("Zoom")]
    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 60f;

    [Header("Bounds")]
    public float mapMinX = -10f;
    public float mapMaxX = 110f;
    public float mapMinZ = -10f;
    public float mapMaxZ = 110f;

    float currentZoom = 30f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(50f, 0f, 0f);
        UpdateZoom();
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
        ClampPosition();
    }

    void HandlePan()
    {
        Vector3 moveDir = Vector3.zero;

        // Keyboard input (new Input System)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveDir += Vector3.forward;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveDir += Vector3.back;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveDir += Vector3.left;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveDir += Vector3.right;
        }

        // Edge scrolling disabled -- was causing unwanted camera movement
        // when mouse is near edges of the Game window in the editor.
        // Can be re-enabled for fullscreen builds later.

        if (moveDir != Vector3.zero)
        {
            Vector3 move = moveDir.normalized * panSpeed * Time.deltaTime;
            transform.position += new Vector3(move.x, 0, move.z);
        }
    }

    void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            currentZoom -= scroll * zoomSpeed * Time.deltaTime * 2f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            UpdateZoom();
        }
    }

    void UpdateZoom()
    {
        Vector3 pos = transform.position;
        pos.y = currentZoom;
        transform.position = pos;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, mapMinX, mapMaxX);
        pos.z = Mathf.Clamp(pos.z, mapMinZ, mapMaxZ);
        transform.position = pos;
    }

    public void SetMapBounds(float minX, float maxX, float minZ, float maxZ)
    {
        mapMinX = minX;
        mapMaxX = maxX;
        mapMinZ = minZ;
        mapMaxZ = maxZ;
    }

    public void FocusOn(Vector3 position)
    {
        Vector3 pos = transform.position;
        pos.x = position.x;
        pos.z = position.z - currentZoom * 0.5f;
        transform.position = pos;
    }
}
