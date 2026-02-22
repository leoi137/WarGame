using UnityEngine;

public class WorldMapInput : MonoBehaviour
{
    [SerializeField] Camera raycastCamera;
    [SerializeField] WorldMapManager manager;

    void Awake()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;
        if (manager == null)
            manager = WorldMapManager.Instance ?? FindObjectOfType<WorldMapManager>();
    }

    void Update()
    {
        if (manager == null) return;
        var screenPos = Input.mousePosition;
        HandleHover(screenPos);
        if (Input.GetMouseButtonDown(0))
        {
            var faction = RaycastToFaction(screenPos);
            if (faction != null)
            {
                HandleFactionClick(faction);
                return;
            }
            var city = RaycastToCity(screenPos);
            if (city != null)
                HandleCityClick(city);
        }
    }

    public FactionDefinition RaycastToFaction(Vector2 screenPos)
    {
        if (raycastCamera == null) return null;
        var ray = raycastCamera.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out var hit, 1000f)) return null;
        var province = hit.collider.GetComponent<ProvinceRenderer>();
        return province?.Faction;
    }

    public CityDefinition RaycastToCity(Vector2 screenPos)
    {
        if (raycastCamera == null) return null;
        var ray = raycastCamera.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out var hit, 1000f)) return null;
        var marker = hit.collider.GetComponent<CityMarker>();
        return marker?.City;
    }

    public void HandleFactionClick(FactionDefinition faction)
    {
        manager?.SelectFaction(faction);
    }

    public void HandleCityClick(CityDefinition city)
    {
        if (city == null) return;
        var faction = FactionDatabase.GetAll().Find(f => f.cities?.Exists(c => c?.id == city.id) ?? false);
        if (faction != null)
            manager?.SelectFaction(faction);
    }

    public void HandleHover(Vector2 screenPos)
    {
    }
}
