using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; set; }

    [Header("Movement")]
    public float panSpeed = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 8f;
    public float minZoom = 5f;   // Close enough to see unit details
    public float maxZoom = 60f;
    public float minAngle = 25f; // More top-down when zoomed out
    public float maxAngle = 55f; // Steeper when zoomed in

    [Header("Bounds")]
    public float mapMinX = -10f;
    public float mapMaxX = 110f;
    public float mapMinZ = -10f;
    public float mapMaxZ = 110f;

    float currentZoom = 30f;
    float targetZoom = 30f;
    float zoomVelocity;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        targetZoom = currentZoom;
        UpdateCameraFromZoom();
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

        if (moveDir != Vector3.zero)
        {
            // Pan speed scales with zoom level (faster when zoomed out, slower when close)
            float speedMult = Mathf.Lerp(0.4f, 1.5f, (currentZoom - minZoom) / (maxZoom - minZoom));
            Vector3 move = moveDir.normalized * panSpeed * speedMult * Time.deltaTime;
            transform.position += new Vector3(move.x, 0, move.z);
        }
    }

    void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Scroll is an instant value, not a held key -- don't multiply by deltaTime
            float zoomDelta = -Mathf.Sign(scroll) * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom + zoomDelta, minZoom, maxZoom);
        }

        // Smooth zoom interpolation
        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, 0.12f);
        UpdateCameraFromZoom();
    }

    void UpdateCameraFromZoom()
    {
        // Camera angle changes with zoom: more top-down when far, steeper when close
        float zoomT = (currentZoom - minZoom) / (maxZoom - minZoom);
        float angle = Mathf.Lerp(maxAngle, minAngle, zoomT);

        transform.rotation = Quaternion.Euler(angle, 0f, 0f);

        // Position: Y is the zoom height
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
