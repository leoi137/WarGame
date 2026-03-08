using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Handles click and drag-box unit selection.
/// In placement mode, selection drives pre-battle positioning instead of combat orders.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    public List<Unit> selectedUnits = new List<Unit>();

    /// <summary>
    /// When true, drag-select is used for pre-battle placement instead of combat commands.
    /// </summary>
    public bool isPlacementMode;

    bool isDragging;
    Vector2 dragStartScreen;
    float dragThreshold = 10f;

    GUIStyle dragBoxStyle;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dragBoxStyle = new GUIStyle();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0f, 0.8f, 0f, 0.25f));
        tex.Apply();
        dragBoxStyle.normal.background = tex;
    }

    void Update()
    {
        if (GameUI.Instance != null && GameUI.Instance.IsGameOver) return;
        if (Mouse.current == null) return;

        HandleSelection();
    }

    void HandleSelection()
    {
        Mouse mouse = Mouse.current;
        Vector2 mousePos = mouse.position.ReadValue();

        // Left mouse button pressed -- start drag
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            dragStartScreen = mousePos;
        }

        // Left mouse button released -- complete selection
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (isDragging)
            {
                float dragDist = Vector2.Distance(dragStartScreen, mousePos);

                if (dragDist < dragThreshold)
                {
                    HandleClick(mousePos);
                }
                else
                {
                    HandleBoxSelect(mousePos);
                }

                isDragging = false;
            }
        }
    }

    void HandleClick(Vector2 mousePos)
    {
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
        bool shiftHeld = Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();

            if (clickedUnit != null && clickedUnit.faction == Faction.North && !clickedUnit.isDead)
            {
                if (shiftHeld)
                {
                    if (selectedUnits.Contains(clickedUnit))
                    {
                        clickedUnit.SetSelected(false);
                        selectedUnits.Remove(clickedUnit);
                    }
                    else
                    {
                        clickedUnit.SetSelected(true);
                        selectedUnits.Add(clickedUnit);
                    }
                }
                else
                {
                    DeselectAll();
                    clickedUnit.SetSelected(true);
                    selectedUnits.Add(clickedUnit);
                }
            }
            else if (!shiftHeld)
            {
                DeselectAll();
            }
        }
        else if (!shiftHeld)
        {
            DeselectAll();
        }
    }

    void HandleBoxSelect(Vector2 mousePos)
    {
        bool shiftHeld = Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        if (!shiftHeld)
        {
            DeselectAll();
        }

        if (Camera.main == null) return;
        Rect selectionRect = GetScreenRect(dragStartScreen, mousePos);

        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit u in allUnits)
        {
            if (u.isDead || u.faction != Faction.North) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(u.transform.position);
            if (screenPos.z > 0 && selectionRect.Contains(new Vector2(screenPos.x, screenPos.y)))
            {
                if (!selectedUnits.Contains(u))
                {
                    u.SetSelected(true);
                    selectedUnits.Add(u);
                }
            }
        }
    }

    public void DeselectAll()
    {
        foreach (Unit u in selectedUnits)
        {
            if (u != null) u.SetSelected(false);
        }
        selectedUnits.Clear();
    }

    Rect GetScreenRect(Vector2 a, Vector2 b)
    {
        float x = Mathf.Min(a.x, b.x);
        float y = Mathf.Min(a.y, b.y);
        float w = Mathf.Abs(a.x - b.x);
        float h = Mathf.Abs(a.y - b.y);
        return new Rect(x, y, w, h);
    }

    void OnGUI()
    {
        if (isDragging && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float dragDist = Vector2.Distance(dragStartScreen, mousePos);
            if (dragDist >= dragThreshold)
            {
                Rect rect = GetScreenRect(dragStartScreen, mousePos);
                rect.y = Screen.height - rect.y - rect.height;
                GUI.Box(rect, "", dragBoxStyle);
            }
        }
    }
}
