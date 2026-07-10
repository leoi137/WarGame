using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    public static WorldMapCamera Instance { get; private set; }

    public float panSpeed = 30f;
    public float zoomSpeed = 5f;
    public float minZoom = 20f;
    public float maxZoom = 120f;

    float _mapWidth;
    float _mapHeight;
    Camera _cam;
    float _currentZoom;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _cam = GetComponent<Camera>();
        if (_cam == null)
            _cam = Camera.main;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        HandleInput();
        ClampToBounds();
    }

    public void Initialize(float mapWidth, float mapHeight)
    {
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _currentZoom = Mathf.Lerp(minZoom, maxZoom, 0.5f);
        if (_cam != null)
        {
            _cam.orthographic = true;
            _cam.orthographicSize = _currentZoom;
            transform.position = new Vector3(mapWidth * 0.5f, 100f, mapHeight * 0.5f);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    public void FocusOnFaction(FactionDefinition faction)
    {
        if (faction?.cities == null || faction.cities.Count == 0) return;
        var positions = faction.cities
            .Where(c => c != null)
            .Select(c => new Vector2(c.normalizedPosition.x * GameConfig.WorldMapWidth, c.normalizedPosition.y * GameConfig.WorldMapHeight))
            .ToList();
        var centroid = Vector2.zero;
        foreach (var p in positions)
            centroid += p;
        centroid /= positions.Count;
        var pos = transform.position;
        pos.x = centroid.x;
        pos.z = centroid.y;
        transform.position = pos;
        ClampToBounds();
    }

    public void FocusOnRegion(Region region)
    {
        var factions = FactionDatabase.GetByRegion(region);
        if (factions == null || factions.Count == 0) return;
        var allPositions = new List<Vector2>();
        foreach (var f in factions)
        {
            if (f?.cities == null) continue;
            foreach (var c in f.cities)
            {
                if (c == null) continue;
                allPositions.Add(new Vector2(c.normalizedPosition.x * GameConfig.WorldMapWidth, c.normalizedPosition.y * GameConfig.WorldMapHeight));
            }
        }
        if (allPositions.Count == 0) return;
        var centroid = Vector2.zero;
        foreach (var p in allPositions)
            centroid += p;
        centroid /= allPositions.Count;
        var pos = transform.position;
        pos.x = centroid.x;
        pos.z = centroid.y;
        transform.position = pos;
        ClampToBounds();
    }

    public void HandleInput()
    {
        if (_cam == null) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
        {
            var delta = new Vector3(h * panSpeed * Time.deltaTime, 0, v * panSpeed * Time.deltaTime);
            transform.position += delta;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _currentZoom -= scroll * zoomSpeed * 50f;
            _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            _cam.orthographicSize = _currentZoom;
        }
    }

    public void ClampToBounds()
    {
        if (_mapWidth <= 0 || _mapHeight <= 0) return;
        float halfW = _mapWidth * 0.5f;
        float halfH = _mapHeight * 0.5f;
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, 0, _mapWidth);
        pos.z = Mathf.Clamp(pos.z, 0, _mapHeight);
        transform.position = pos;
    }
}
