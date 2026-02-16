using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameUI.Instance != null && GameUI.Instance.IsGameOver) return;
        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleRightClick();
        }
    }

    void HandleRightClick()
    {
        List<Unit> selected = SelectionManager.Instance?.selectedUnits;
        if (selected == null || selected.Count == 0) return;

        if (Camera.main == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            Unit targetUnit = hit.collider.GetComponentInParent<Unit>();

            if (targetUnit != null && targetUnit.faction != Faction.North && !targetUnit.isDead)
            {
                // Attack command
                foreach (Unit u in selected)
                {
                    if (u == null || u.isDead) continue;
                    UnitCombat combat = u.GetComponent<UnitCombat>();
                    if (combat != null)
                    {
                        combat.SetTarget(targetUnit);
                    }
                }
                SpawnCommandIndicator(targetUnit.transform.position, Color.red);
            }
            else
            {
                // Move command
                Vector3 targetPos = hit.point;
                MoveUnitsInFormation(selected, targetPos);
                SpawnCommandIndicator(targetPos, Color.green);
            }
        }
    }

    void MoveUnitsInFormation(List<Unit> units, Vector3 center)
    {
        int count = units.Count;
        if (count == 0) return;

        if (count == 1)
        {
            UnitMovement mov = units[0].GetComponent<UnitMovement>();
            UnitCombat combat = units[0].GetComponent<UnitCombat>();
            if (combat != null) combat.ClearTarget();
            if (mov != null) mov.MoveTo(center);
            return;
        }

        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        float spacing = 2.0f;

        for (int i = 0; i < count; i++)
        {
            if (units[i] == null || units[i].isDead) continue;

            int row = i / cols;
            int col = i % cols;

            float offsetX = (col - (cols - 1) * 0.5f) * spacing;
            float offsetZ = (row - ((count / cols) - 1) * 0.5f) * spacing;

            Vector3 pos = center + new Vector3(offsetX, 0, offsetZ);

            UnitMovement mov = units[i].GetComponent<UnitMovement>();
            UnitCombat combat = units[i].GetComponent<UnitCombat>();
            if (combat != null) combat.ClearTarget();
            if (mov != null) mov.MoveTo(pos);
        }
    }

    void SpawnCommandIndicator(Vector3 position, Color color)
    {
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        indicator.transform.position = position + Vector3.up * 0.1f;
        indicator.transform.localScale = new Vector3(0.8f, 0.05f, 0.8f);

        Renderer rend = indicator.GetComponent<Renderer>();
        color.a = 0.6f;
        Material mat = ShaderHelper.CreateMaterial(color);
        rend.material = mat;

        Destroy(indicator.GetComponent<Collider>());
        Destroy(indicator, 0.5f);
    }
}
