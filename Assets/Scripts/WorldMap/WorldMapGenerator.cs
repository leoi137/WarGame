using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMapGenerator : MonoBehaviour
{
    const float TerritoryPadding = 2f;
    const float TerritoryHeight = 0.1f;

    public IReadOnlyDictionary<FactionDefinition, ProvinceRenderer> ProvinceRenderers => _provinceRenderers;
    public IReadOnlyDictionary<FactionDefinition, List<CityMarker>> CityMarkers => _cityMarkers;

    readonly Dictionary<FactionDefinition, ProvinceRenderer> _provinceRenderers = new();
    readonly Dictionary<FactionDefinition, List<CityMarker>> _cityMarkers = new();
    GameObject _oceanRoot;
    GameObject _landmassRoot;
    GameObject _territoriesRoot;
    GameObject _citiesRoot;

    public void Generate(List<FactionDefinition> factions)
    {
        float w = GameConfig.WorldMapWidth;
        float h = GameConfig.WorldMapHeight;

        _oceanRoot = new GameObject("Ocean");
        _oceanRoot.transform.SetParent(transform);
        CreateOcean(w, h);

        _landmassRoot = new GameObject("Landmass");
        _landmassRoot.transform.SetParent(transform);
        CreateLandmass(w, h);

        _territoriesRoot = new GameObject("Territories");
        _territoriesRoot.transform.SetParent(transform);

        foreach (var faction in factions)
        {
            if (faction?.cities == null || faction.cities.Count == 0) continue;
            CreateFactionTerritory(faction);
        }

        CreateContinentOutlines();

        _citiesRoot = new GameObject("Cities");
        _citiesRoot.transform.SetParent(transform);
        foreach (var faction in factions)
        {
            if (faction?.cities == null) continue;
            var markers = new List<CityMarker>();
            foreach (var city in faction.cities)
            {
                var go = new GameObject($"City_{city.id}");
                go.transform.SetParent(_citiesRoot.transform);
                var marker = go.AddComponent<CityMarker>();
                marker.Initialize(city, faction);
                markers.Add(marker);
            }
            _cityMarkers[faction] = markers;
        }
    }

    public void CreateOcean(float width, float height)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = "OceanPlane";
        go.transform.SetParent(_oceanRoot.transform);
        go.transform.localPosition = new Vector3(width * 0.5f, -0.5f, height * 0.5f);
        go.transform.localScale = new Vector3(width * 0.1f, 1f, height * 0.1f);
        var mat = ShaderHelper.CreateMaterial(new Color(0.1f, 0.2f, 0.4f));
        go.GetComponent<Renderer>().material = mat;
        Object.Destroy(go.GetComponent<Collider>());
    }

    public void CreateLandmass(float width, float height)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = "LandmassPlane";
        go.transform.SetParent(_landmassRoot.transform);
        go.transform.localPosition = new Vector3(width * 0.5f, 0f, height * 0.5f);
        go.transform.localScale = new Vector3(width * 0.1f, 1f, height * 0.1f);
        var mat = ShaderHelper.CreateMaterial(new Color(0.35f, 0.4f, 0.3f));
        go.GetComponent<Renderer>().material = mat;
        Object.Destroy(go.GetComponent<Collider>());
    }

    public void CreateFactionTerritory(FactionDefinition faction)
    {
        var positions = faction.cities
            .Where(c => c != null)
            .Select(c => NormalizedToWorld(c.normalizedPosition))
            .Select(v => new Vector2(v.x, v.z))
            .ToList();
        if (positions.Count < 3) return;

        var mesh = GenerateTerritoryMesh(positions, TerritoryPadding);
        var go = new GameObject($"Territory_{faction.id}");
        go.transform.SetParent(_territoriesRoot.transform);

        var mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mr = go.AddComponent<MeshRenderer>();
        var provinceRenderer = go.AddComponent<ProvinceRenderer>();
        provinceRenderer.Initialize(faction, mesh);
        mr.material = provinceRenderer.GetFactionMaterial(faction);
        provinceRenderer.CreateBorder(mesh, Color.black);

        var col = go.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;

        _provinceRenderers[faction] = provinceRenderer;
    }

    public void CreateContinentOutlines()
    {
    }

    public Mesh GenerateTerritoryMesh(List<Vector2> cityPositions, float padding)
    {
        if (cityPositions == null || cityPositions.Count < 3)
            return new Mesh();

        var hull = ConvexHull(cityPositions);
        var expanded = ExpandPolygon(hull, padding);
        return PolygonToMesh(expanded);
    }

    static List<Vector2> ConvexHull(List<Vector2> points)
    {
        if (points.Count < 3) return new List<Vector2>(points);
        var sorted = points.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        var lower = new List<Vector2>();
        foreach (var p in sorted)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }
        var upper = new List<Vector2>();
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            var p = sorted[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }
        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);
        return lower;
    }

    static float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    static List<Vector2> ExpandPolygon(List<Vector2> polygon, float amount)
    {
        var centroid = Vector2.zero;
        foreach (var p in polygon) centroid += p;
        centroid /= polygon.Count;
        var result = new List<Vector2>();
        foreach (var p in polygon)
        {
            var dir = (p - centroid).normalized;
            result.Add(p + dir * amount);
        }
        return result;
    }

    static Mesh PolygonToMesh(List<Vector2> polygon)
    {
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        foreach (var p in polygon)
            verts.Add(new Vector3(p.x, TerritoryHeight, p.y));
        mesh.SetVertices(verts);

        var tris = new List<int>();
        for (int i = 1; i < polygon.Count - 1; i++)
        {
            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);
        }
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public Vector3 NormalizedToWorld(Vector2 normalized)
    {
        return new Vector3(
            normalized.x * GameConfig.WorldMapWidth,
            0f,
            normalized.y * GameConfig.WorldMapHeight);
    }
}
