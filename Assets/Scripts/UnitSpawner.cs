using UnityEngine;
using UnityEngine.AI;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance { get; set; }

    [Header("Spawn Counts Per Faction")]
    public int swordsmenCount = 3;
    public int archersCount = 2;
    public int berserkersCount = 2;
    public int shieldbearersCount = 2;

    [Header("Formation")]
    public float distanceFromCenter = 25f;
    public float rowSpacing = 4f;
    public float unitSpacing = 3.5f;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnAllUnits(int mapSize)
    {
        float centerX = mapSize * 0.5f;
        float centerZ = mapSize * 0.5f;

        float northBaseX = centerX - distanceFromCenter;
        float southBaseX = centerX + distanceFromCenter;

        SpawnFactionUnits(Faction.North, northBaseX, centerZ, 1f);
        SpawnFactionUnits(Faction.South, southBaseX, centerZ, -1f);
    }

    void SpawnFactionUnits(Faction faction, float baseX, float centerZ, float facing)
    {
        float row1X = baseX + rowSpacing * 2f * facing;
        float row2X = baseX + rowSpacing * 0.5f * facing;
        float row3X = baseX - rowSpacing * 1.5f * facing;

        Quaternion faceRotation = Quaternion.LookRotation(new Vector3(facing, 0, 0));

        // Row 1: Shieldbearers (front line)
        SpawnRow(faction, UnitType.Shieldbearer, shieldbearersCount, row1X, centerZ, unitSpacing, faceRotation);

        // Row 2: Swordsmen and Berserkers (middle line)
        int totalMelee = swordsmenCount + berserkersCount;
        float meleeStartZ = centerZ - (totalMelee - 1) * 0.5f * unitSpacing;

        for (int i = 0; i < swordsmenCount; i++)
        {
            Vector3 pos = new Vector3(row2X, 0, meleeStartZ + i * unitSpacing);
            pos.y = GetTerrainHeight(pos);
            SpawnUnit(faction, UnitType.Swordsman, pos, faceRotation);
        }
        for (int i = 0; i < berserkersCount; i++)
        {
            Vector3 pos = new Vector3(row2X, 0, meleeStartZ + (swordsmenCount + i) * unitSpacing);
            pos.y = GetTerrainHeight(pos);
            SpawnUnit(faction, UnitType.Berserker, pos, faceRotation);
        }

        // Row 3: Archers (back line)
        SpawnRow(faction, UnitType.Archer, archersCount, row3X, centerZ, unitSpacing, faceRotation);
    }

    void SpawnRow(Faction faction, UnitType unitType, int count, float xPos, float centerZ, float spacing, Quaternion rotation)
    {
        for (int i = 0; i < count; i++)
        {
            float offsetZ = (i - (count - 1) * 0.5f) * spacing;
            Vector3 pos = new Vector3(xPos, 0, centerZ + offsetZ);
            pos.y = GetTerrainHeight(pos);
            SpawnUnit(faction, unitType, pos, rotation);
        }
    }

    void SpawnUnit(Faction faction, UnitType unitType, Vector3 position, Quaternion rotation)
    {
        string typeName = unitType.ToString();
        GameObject unitObj = new GameObject($"{faction}_{typeName}");
        unitObj.transform.position = position;
        unitObj.transform.rotation = rotation;
        unitObj.layer = 0;

        CapsuleCollider col = unitObj.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1.1f, 0);

        switch (unitType)
        {
            case UnitType.Berserker:
                col.radius = 0.5f;
                col.height = 2.4f;
                break;
            case UnitType.Shieldbearer:
                col.radius = 0.5f;
                col.height = 2.3f;
                break;
            default:
                col.radius = 0.4f;
                col.height = 2.2f;
                break;
        }

        NavMeshAgent agent = unitObj.AddComponent<NavMeshAgent>();
        agent.radius = col.radius;
        agent.height = 2.0f;
        agent.baseOffset = 0f;

        Unit unit = unitObj.AddComponent<Unit>();
        unit.faction = faction;
        unit.unitType = unitType;
        unit.Initialize();

        agent.speed = unit.moveSpeed;

        unitObj.AddComponent<UnitMovement>();
        unitObj.AddComponent<UnitCombat>();
        unitObj.AddComponent<HealthBar>();

        // Animator: wire pivot references from Unit's skeleton
        UnitAnimator anim = unitObj.AddComponent<UnitAnimator>();
        WireAnimatorPivots(anim, unit);
        anim.InitializeRests();

        if (FactionManager.Instance != null)
            FactionManager.Instance.RegisterUnit(unit);
    }

    void WireAnimatorPivots(UnitAnimator anim, Unit unit)
    {
        anim.pivotHips = unit.pivotHips;
        anim.pivotWaist = unit.pivotWaist;
        anim.pivotNeck = unit.pivotNeck;
        anim.pivotLeftShoulder = unit.pivotLeftShoulder;
        anim.pivotRightShoulder = unit.pivotRightShoulder;
        anim.pivotLeftElbow = unit.pivotLeftElbow;
        anim.pivotRightElbow = unit.pivotRightElbow;
        anim.pivotLeftHand = unit.pivotLeftHand;
        anim.pivotRightHand = unit.pivotRightHand;
        anim.pivotLeftHip = unit.pivotLeftHip;
        anim.pivotRightHip = unit.pivotRightHip;
        anim.pivotLeftKnee = unit.pivotLeftKnee;
        anim.pivotRightKnee = unit.pivotRightKnee;
        anim.pivotCape = unit.pivotCape;

        // Legacy references (for backward compat)
        anim.head = unit.partHead;
        anim.body = unit.partBody;
        anim.leftArm = unit.partLeftArm;
        anim.rightArm = unit.partRightArm;
        anim.leftLeg = unit.partLeftLeg;
        anim.rightLeg = unit.partRightLeg;
        anim.weapon = unit.partWeapon;
        anim.weaponLeft = unit.partWeaponLeft;
    }

    float GetTerrainHeight(Vector3 pos)
    {
        if (Terrain.activeTerrain != null)
            return Terrain.activeTerrain.SampleHeight(pos);
        return 0f;
    }
}
