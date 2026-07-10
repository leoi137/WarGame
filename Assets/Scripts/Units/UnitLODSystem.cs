using System.Collections.Generic;
using UnityEngine;

public static class UnitLODSystem
{
    public enum LODLevel { Full, Simplified, Billboard, Culled }

    static readonly Dictionary<Unit, LODCache> Cache = new Dictionary<Unit, LODCache>();
    static readonly List<(Unit unit, float sqDist)> SortBuffer = new List<(Unit, float)>(1024);

    struct LODCache
    {
        public List<Renderer> AllRenderers;
        public List<Renderer> SimplifiedRenderers;
        public GameObject Billboard;
        public LODLevel CurrentLevel;
    }

    public static void Initialize()
    {
        Cache.Clear();
        SortBuffer.Clear();
    }

    public static void UpdateLOD(List<Unit> allUnits, Vector3 cameraPos)
    {
        if (allUnits == null || allUnits.Count == 0) return;

        SortBuffer.Clear();
        for (int i = 0; i < allUnits.Count; i++)
        {
            Unit u = allUnits[i];
            if (u == null || u.isDead) continue;
            float sqDist = (u.transform.position - cameraPos).sqrMagnitude;
            SortBuffer.Add((u, sqDist));
        }

        SortBuffer.Sort((a, b) => a.sqDist.CompareTo(b.sqDist));

        int fullCount = 0;
        float fullSq = GameConfig.LODFullDistance * GameConfig.LODFullDistance;
        float simplifiedSq = GameConfig.LODSimplifiedDistance * GameConfig.LODSimplifiedDistance;
        float billboardSq = GameConfig.LODBillboardDistance * GameConfig.LODBillboardDistance;

        for (int i = 0; i < SortBuffer.Count; i++)
        {
            var (unit, sqDist) = SortBuffer[i];
            LODLevel level;

            if (sqDist <= billboardSq)
            {
                if (sqDist <= simplifiedSq)
                {
                    if (sqDist <= fullSq && fullCount < GameConfig.MaxFullLODUnits)
                    {
                        level = LODLevel.Full;
                        fullCount++;
                    }
                    else
                        level = LODLevel.Simplified;
                }
                else
                    level = LODLevel.Billboard;
            }
            else
                level = LODLevel.Culled;

            SetLODLevelInternal(unit, level, cameraPos);
        }
    }

    public static void SetLODLevel(Unit unit, LODLevel level)
    {
        SetLODLevelInternal(unit, level, null);
    }

    static void SetLODLevelInternal(Unit unit, LODLevel level, Vector3? cameraPos)
    {
        if (unit == null || unit.isDead) return;

        if (!Cache.TryGetValue(unit, out LODCache cache))
        {
            cache = BuildCache(unit);
            Cache[unit] = cache;
        }

        if (cache.CurrentLevel == level)
        {
            if (level == LODLevel.Billboard && cameraPos.HasValue && cache.Billboard != null)
            {
                cache.Billboard.SetActive(true);
                cache.Billboard.transform.rotation = Quaternion.LookRotation(cameraPos.Value - cache.Billboard.transform.position);
            }
            return;
        }

        cache.CurrentLevel = level;
        Cache[unit] = cache;

        switch (level)
        {
            case LODLevel.Full:
                SetRenderersEnabled(cache.AllRenderers, true);
                if (cache.Billboard != null) cache.Billboard.SetActive(false);
                SetHealthBarVisible(unit, true);
                break;

            case LODLevel.Simplified:
                SetRenderersEnabled(cache.AllRenderers, false);
                SetRenderersEnabled(cache.SimplifiedRenderers, true);
                if (cache.Billboard != null) cache.Billboard.SetActive(false);
                SetHealthBarVisible(unit, false);
                DeactivateTrails(unit);
                break;

            case LODLevel.Billboard:
                SetRenderersEnabled(cache.AllRenderers, false);
                SetRenderersEnabled(cache.SimplifiedRenderers, false);
                EnsureBillboard(unit, ref cache);
                cache.Billboard.SetActive(true);
                if (cameraPos.HasValue)
                    cache.Billboard.transform.rotation = Quaternion.LookRotation(cameraPos.Value - cache.Billboard.transform.position);
                Cache[unit] = cache;
                SetHealthBarVisible(unit, false);
                DeactivateTrails(unit);
                break;

            case LODLevel.Culled:
                SetRenderersEnabled(cache.AllRenderers, false);
                SetRenderersEnabled(cache.SimplifiedRenderers, false);
                if (cache.Billboard != null) cache.Billboard.SetActive(false);
                SetHealthBarVisible(unit, false);
                DeactivateTrails(unit);
                break;
        }
    }

    static LODCache BuildCache(Unit unit)
    {
        var all = new List<Renderer>();
        var simplified = new List<Renderer>();

        foreach (Renderer r in unit.GetComponentsInChildren<Renderer>(true))
        {
            if (r == null) continue;
            if (r.gameObject.name == "LODBillboard") continue;
            all.Add(r);

            Transform t = r.transform;
            if (unit.partBody != null && (t == unit.partBody || t.IsChildOf(unit.partBody)))
                simplified.Add(r);
            else if (unit.partWeapon != null && (t == unit.partWeapon || t.IsChildOf(unit.partWeapon)))
                simplified.Add(r);
            else if (unit.partWeaponLeft != null && (t == unit.partWeaponLeft || t.IsChildOf(unit.partWeaponLeft)))
                simplified.Add(r);
            else if (unit.partHead != null && (t == unit.partHead || t.IsChildOf(unit.partHead)))
                simplified.Add(r);
        }

        return new LODCache
        {
            AllRenderers = all,
            SimplifiedRenderers = simplified,
            Billboard = null,
            CurrentLevel = (LODLevel)(-1)
        };
    }

    static void SetRenderersEnabled(List<Renderer> list, bool enabled)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
                list[i].enabled = enabled;
        }
    }

    static void EnsureBillboard(Unit unit, ref LODCache cache)
    {
        if (cache.Billboard != null) return;

        Transform existing = unit.transform.Find("LODBillboard");
        if (existing != null)
        {
            cache.Billboard = existing.gameObject;
            Cache[unit] = cache;
            return;
        }

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "LODBillboard";
        quad.transform.SetParent(unit.transform);
        quad.transform.localPosition = new Vector3(0, 1f, 0);
        quad.transform.localScale = new Vector3(0.8f, 1.6f, 1f);

        Renderer r = quad.GetComponent<Renderer>();
        Color c = unit.faction == Faction.North
            ? new Color(0.15f, 0.35f, 0.7f)
            : new Color(0.7f, 0.15f, 0.12f);
        r.material = ShaderHelper.CreateMaterial(c);

        Collider col = quad.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        cache.Billboard = quad;
        Cache[unit] = cache;
    }

    static void SetHealthBarVisible(Unit unit, bool visible)
    {
        Transform canvas = unit.transform.Find("HealthBarCanvas");
        if (canvas != null)
            canvas.gameObject.SetActive(visible);
    }

    static void DeactivateTrails(Unit unit)
    {
        TrailEffect.RemoveTrail(unit.weaponTip);
        TrailEffect.RemoveTrail(unit.weaponLeftTip);
    }
}
