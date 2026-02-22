using System.Collections.Generic;
using UnityEngine;

public class MinimapRenderer : MonoBehaviour
{
    Camera minimapCamera;
    RenderTexture renderTexture;
    int mapSize;
    Color attackerColor;
    Color defenderColor;
    Transform dotsRoot;
    List<GameObject> dotObjects = new();

    public RenderTexture MinimapTexture => renderTexture;

    public void Initialize(int mapSize, Color attackerColor, Color defenderColor)
    {
        this.mapSize = mapSize;
        this.attackerColor = attackerColor;
        this.defenderColor = defenderColor;

        renderTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        var camObj = new GameObject("MinimapCamera");
        minimapCamera = camObj.AddComponent<Camera>();
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = mapSize * 0.5f;
        minimapCamera.targetTexture = renderTexture;
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = new Color(0.15f, 0.12f, 0.08f, 1f);
        minimapCamera.cullingMask = -1;
        minimapCamera.enabled = true;

        camObj.transform.position = new Vector3(mapSize * 0.5f, 100f, mapSize * 0.5f);
        camObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        dotsRoot = new GameObject("MinimapDots").transform;
        dotsRoot.SetParent(transform);
    }

    public void UpdateUnitPositions(List<Unit> attackers, List<Unit> defenders)
    {
        foreach (var d in dotObjects) if (d != null) Destroy(d);
        dotObjects.Clear();

        if (dotsRoot == null) return;

        foreach (var u in attackers ?? new List<Unit>())
        {
            if (u == null || u.isDead) continue;
            var dot = CreateDot(u.transform.position, attackerColor);
            if (dot != null) dotObjects.Add(dot);
        }
        foreach (var u in defenders ?? new List<Unit>())
        {
            if (u == null || u.isDead) continue;
            var dot = CreateDot(u.transform.position, defenderColor);
            if (dot != null) dotObjects.Add(dot);
        }
    }

    GameObject CreateDot(Vector3 worldPos, Color color)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "MinimapDot";
        quad.transform.SetParent(dotsRoot);
        quad.transform.position = new Vector3(worldPos.x, 5f, worldPos.z);
        quad.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        quad.transform.localScale = Vector3.one * 3f;
        var rend = quad.GetComponent<Renderer>();
        if (rend != null) rend.material.color = color;
        Destroy(quad.GetComponent<Collider>());
        return quad;
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
        if (minimapCamera != null && minimapCamera.gameObject != null)
            Destroy(minimapCamera.gameObject);
    }
}
