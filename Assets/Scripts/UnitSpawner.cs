using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance { get; set; }

    [Header("Spawn Settings")]
    public int swordsmenPerFaction = 3;
    public int archersPerFaction = 3;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnAllUnits(int mapSize)
    {
        float northBaseX = mapSize * 0.2f;
        float southBaseX = mapSize * 0.8f;
        float centerZ = mapSize * 0.5f;

        // North faction (player) - left side of map
        SpawnFactionUnits(Faction.North, northBaseX, centerZ);

        // South faction (AI) - right side of map
        SpawnFactionUnits(Faction.South, southBaseX, centerZ);
    }

    void SpawnFactionUnits(Faction faction, float baseX, float centerZ)
    {
        float spacing = 3f;

        // Swordsmen in front line
        for (int i = 0; i < swordsmenPerFaction; i++)
        {
            float offsetZ = (i - (swordsmenPerFaction - 1) * 0.5f) * spacing;
            float frontOffset = faction == Faction.North ? 3f : -3f;
            Vector3 pos = new Vector3(baseX + frontOffset, 0, centerZ + offsetZ);
            pos.y = GetTerrainHeight(pos);

            SpawnUnit(faction, UnitType.Swordsman, pos);
        }

        // Archers in back line
        for (int i = 0; i < archersPerFaction; i++)
        {
            float offsetZ = (i - (archersPerFaction - 1) * 0.5f) * spacing;
            float backOffset = faction == Faction.North ? -3f : 3f;
            Vector3 pos = new Vector3(baseX + backOffset, 0, centerZ + offsetZ);
            pos.y = GetTerrainHeight(pos);

            SpawnUnit(faction, UnitType.Archer, pos);
        }
    }

    void SpawnUnit(Faction faction, UnitType unitType, Vector3 position)
    {
        GameObject unitObj = new GameObject($"{faction}_{unitType}");
        unitObj.transform.position = position;
        unitObj.layer = 0;

        // Add collider for click detection
        CapsuleCollider col = unitObj.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1.1f, 0);
        col.radius = 0.4f;
        col.height = 2.2f;

        // Add NavMeshAgent
        NavMeshAgent agent = unitObj.AddComponent<NavMeshAgent>();
        agent.radius = 0.4f;
        agent.height = 2.0f;
        agent.baseOffset = 0f;

        // Add Unit component and configure
        Unit unit = unitObj.AddComponent<Unit>();
        unit.faction = faction;
        unit.unitType = unitType;
        unit.Initialize(); // Sets stats + builds block model

        // Add other components
        unitObj.AddComponent<UnitMovement>();
        unitObj.AddComponent<UnitCombat>();
        unitObj.AddComponent<HealthBar>();

        // Register with FactionManager
        if (FactionManager.Instance != null)
        {
            FactionManager.Instance.RegisterUnit(unit);
        }
    }

    float GetTerrainHeight(Vector3 pos)
    {
        if (Terrain.activeTerrain != null)
        {
            return Terrain.activeTerrain.SampleHeight(pos);
        }
        return 0f;
    }
}
