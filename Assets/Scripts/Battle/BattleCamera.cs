using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed = 30f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float minHeight = 15f;
    [SerializeField] float maxHeight = 80f;
    [SerializeField] float smoothTime = 0.2f;

    bool autoFollow;
    int mapSize;
    float boundsPadding = 20f;
    Vector3 velocity;

    void Update()
    {
        if (autoFollow)
        {
            var allUnits = GetAllLivingUnits();
            if (allUnits.Count > 0)
            {
                Vector3 center = GetCombatCenter(allUnits);
                float zoom = GetOptimalZoom(allUnits);
                Vector3 targetPos = center + Vector3.back * zoom + Vector3.up * zoom * 0.6f;
                targetPos.x = Mathf.Clamp(targetPos.x, -boundsPadding, mapSize + boundsPadding);
                targetPos.z = Mathf.Clamp(targetPos.z, -boundsPadding, mapSize + boundsPadding);
                targetPos.y = Mathf.Clamp(targetPos.y, minHeight, maxHeight);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                transform.LookAt(center + Vector3.up * 5f);
            }
        }
        else
        {
            HandleManualInput();
        }
    }

    public void Initialize(int mapSize)
    {
        this.mapSize = mapSize > 0 ? mapSize : GameConfig.DefaultMapSize;
        if (Camera.main != null)
        {
            transform.position = new Vector3(this.mapSize * 0.5f, 50f, this.mapSize * 0.5f - 40f);
            transform.LookAt(new Vector3(this.mapSize * 0.5f, 0, this.mapSize * 0.5f));
        }
    }

    public void SetAutoFollow(bool enabled)
    {
        autoFollow = enabled;
    }

    public Vector3 GetCombatCenter(List<Unit> allUnits)
    {
        if (allUnits == null || allUnits.Count == 0)
            return new Vector3(mapSize * 0.5f, 0, mapSize * 0.5f);

        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Unit u in allUnits)
        {
            if (u == null || u.isDead) continue;
            sum += u.transform.position;
            count++;
        }
        return count > 0 ? sum / count : new Vector3(mapSize * 0.5f, 0, mapSize * 0.5f);
    }

    public void HandleManualInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.position += transform.TransformDirection(move);
        transform.position += Vector3.up * (-scroll * zoomSpeed * 20f);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -boundsPadding, mapSize + boundsPadding),
            Mathf.Clamp(transform.position.y, minHeight, maxHeight),
            Mathf.Clamp(transform.position.z, -boundsPadding, mapSize + boundsPadding));
    }

    public void FocusOnUnit(Unit unit)
    {
        if (unit == null || unit.isDead) return;
        autoFollow = false;
        transform.position = unit.transform.position + Vector3.back * 25f + Vector3.up * 20f;
        transform.LookAt(unit.transform.position + Vector3.up * 2f);
    }

    public float GetOptimalZoom(List<Unit> allUnits)
    {
        if (allUnits == null || allUnits.Count == 0) return 40f;

        Vector3 center = GetCombatCenter(allUnits);
        float maxDist = 0f;
        foreach (Unit u in allUnits)
        {
            if (u == null || u.isDead) continue;
            float d = Vector3.Distance(u.transform.position, center);
            if (d > maxDist) maxDist = d;
        }
        return Mathf.Clamp(maxDist * 1.5f + 20f, 25f, 80f);
    }

    List<Unit> GetAllLivingUnits()
    {
        var list = new List<Unit>();
        if (BattleManager.Instance != null)
        {
            foreach (Unit u in BattleManager.Instance.attackerUnits)
                if (u != null && !u.isDead) list.Add(u);
            foreach (Unit u in BattleManager.Instance.defenderUnits)
                if (u != null && !u.isDead) list.Add(u);
        }
        return list;
    }
}
