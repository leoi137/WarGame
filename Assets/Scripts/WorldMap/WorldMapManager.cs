using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance { get; private set; }

    [SerializeField] WorldMapGenerator generator;
    [SerializeField] WorldMapCamera mapCamera;
    [SerializeField] WorldMapInput mapInput;
    [SerializeField] FactionInfoPanel infoPanel;
    [SerializeField] GameObject worldMapRoot;

    public FactionDefinition SelectedFaction { get; private set; }
    public FactionDefinition ChosenAttacker { get; private set; }

    readonly Dictionary<FactionDefinition, ProvinceRenderer> _provinceRenderers = new();
    readonly Dictionary<FactionDefinition, List<CityMarker>> _cityMarkers = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Initialize()
    {
        if (worldMapRoot == null)
        {
            worldMapRoot = new GameObject("WorldMapRoot");
            worldMapRoot.transform.SetParent(transform);
        }

        if (generator == null)
            generator = GetComponentInChildren<WorldMapGenerator>();
        if (generator == null)
        {
            var go = new GameObject("WorldMapGenerator");
            go.transform.SetParent(worldMapRoot.transform);
            generator = go.AddComponent<WorldMapGenerator>();
        }

        var factions = FactionDatabase.GetAll();
        generator.Generate(factions);

        foreach (var kv in generator.ProvinceRenderers)
        {
            _provinceRenderers[kv.Key] = kv.Value;
        }
        foreach (var kv in generator.CityMarkers)
        {
            _cityMarkers[kv.Key] = kv.Value;
        }

        if (mapCamera == null)
            mapCamera = FindObjectOfType<WorldMapCamera>();
        if (mapCamera != null)
            mapCamera.Initialize(GameConfig.WorldMapWidth, GameConfig.WorldMapHeight);

        if (mapInput == null)
            mapInput = GetComponentInChildren<WorldMapInput>();
        if (mapInput == null)
        {
            var go = new GameObject("WorldMapInput");
            go.transform.SetParent(worldMapRoot.transform);
            mapInput = go.AddComponent<WorldMapInput>();
        }

        if (infoPanel == null)
            infoPanel = GetComponentInChildren<FactionInfoPanel>();
        if (infoPanel == null)
        {
            var go = new GameObject("FactionInfoPanel");
            go.transform.SetParent(transform);
            infoPanel = go.AddComponent<FactionInfoPanel>();
        }

        Hide();
    }

    public void Show()
    {
        if (worldMapRoot != null)
            worldMapRoot.SetActive(true);
    }

    public void Hide()
    {
        if (worldMapRoot != null)
            worldMapRoot.SetActive(false);
        infoPanel?.Hide();
        DeselectFaction();
    }

    public void SelectFaction(FactionDefinition faction)
    {
        DeselectFaction();
        SelectedFaction = faction;
        if (faction != null)
        {
            if (_provinceRenderers.TryGetValue(faction, out var renderer))
                renderer.SetHighlighted(true);
            if (_cityMarkers.TryGetValue(faction, out var markers))
            {
                foreach (var m in markers)
                    m.SetHighlighted(true);
            }
            infoPanel?.Show(faction);
            mapCamera?.FocusOnFaction(faction);
        }
    }

    public void DeselectFaction()
    {
        if (SelectedFaction != null && _provinceRenderers.TryGetValue(SelectedFaction, out var r))
            r.SetHighlighted(false);
        if (SelectedFaction != null && _cityMarkers.TryGetValue(SelectedFaction, out var markers))
        {
            foreach (var m in markers)
                m.SetHighlighted(false);
        }
        SelectedFaction = null;
        infoPanel?.Hide();
    }

    public void ConfirmAttacker(FactionDefinition faction)
    {
        ChosenAttacker = faction;
        if (faction != null && _provinceRenderers.TryGetValue(faction, out var r))
            r.SetSelected(true);
    }

    public void ConfirmDefender(FactionDefinition faction)
    {
        if (ChosenAttacker == null) return;
        if (faction != null && _provinceRenderers.TryGetValue(faction, out var r))
            r.SetSelected(true);
    }

    public void ClearBattleSelection()
    {
        if (ChosenAttacker != null && _provinceRenderers.TryGetValue(ChosenAttacker, out var r))
            r.SetSelected(false);
        ChosenAttacker = null;
    }
}
