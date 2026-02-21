using UnityEngine;
using UnityEngine.AI;

public enum UnitType { Swordsman, Archer, Berserker, Shieldbearer }

public class Unit : MonoBehaviour
{
    [Header("Identity")]
    public Faction faction;
    public UnitType unitType;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;
    public float moveSpeed = 3.5f;
    public float armor;

    [Header("Abilities")]
    public bool isEnraged;
    public bool isShieldWalling;
    public bool isMarked;
    public float markTimer;
    public float rageDuration = 6f;
    public float rageTimer;
    public float shieldWallArmor = 15f;

    [Header("State")]
    public bool isSelected;
    public bool isDead;

    [Header("Runtime References")]
    public GameObject selectionRing;
    public GameObject rageEffect;
    public GameObject shieldWallEffect;
    public GameObject markEffect;

    // === PIVOT JOINTS (empty transforms for rotation-based animation) ===
    [HideInInspector] public Transform pivotHips;
    [HideInInspector] public Transform pivotWaist;
    [HideInInspector] public Transform pivotNeck;
    [HideInInspector] public Transform pivotLeftShoulder;
    [HideInInspector] public Transform pivotRightShoulder;
    [HideInInspector] public Transform pivotLeftElbow;
    [HideInInspector] public Transform pivotRightElbow;
    [HideInInspector] public Transform pivotLeftHand;
    [HideInInspector] public Transform pivotRightHand;
    [HideInInspector] public Transform pivotLeftHip;
    [HideInInspector] public Transform pivotRightHip;
    [HideInInspector] public Transform pivotLeftKnee;
    [HideInInspector] public Transform pivotRightKnee;
    [HideInInspector] public Transform pivotCape;

    // Legacy part references (kept for compatibility with HealthBar, etc.)
    [HideInInspector] public Transform partHead;
    [HideInInspector] public Transform partBody;
    [HideInInspector] public Transform partLeftArm;
    [HideInInspector] public Transform partRightArm;
    [HideInInspector] public Transform partLeftLeg;
    [HideInInspector] public Transform partRightLeg;
    [HideInInspector] public Transform partWeapon;
    [HideInInspector] public Transform partWeaponLeft;

    // Weapon tip for trail effects
    [HideInInspector] public Transform weaponTip;
    [HideInInspector] public Transform weaponLeftTip;

    // Palette
    static readonly Color NorthPrimary = new Color(0.15f, 0.35f, 0.7f);
    static readonly Color NorthSecondary = new Color(0.2f, 0.45f, 0.85f);
    static readonly Color SouthPrimary = new Color(0.7f, 0.15f, 0.12f);
    static readonly Color SouthSecondary = new Color(0.85f, 0.2f, 0.15f);
    static readonly Color SkinColor = new Color(0.85f, 0.7f, 0.55f);
    static readonly Color FurColor = new Color(0.45f, 0.35f, 0.25f);
    static readonly Color DarkFur = new Color(0.3f, 0.22f, 0.15f);
    static readonly Color IronColor = new Color(0.55f, 0.55f, 0.58f);
    static readonly Color SteelColor = new Color(0.7f, 0.72f, 0.75f);
    static readonly Color WoodColor = new Color(0.5f, 0.33f, 0.15f);
    static readonly Color GoldAccent = new Color(0.85f, 0.7f, 0.2f);
    static readonly Color LeatherColor = new Color(0.4f, 0.28f, 0.15f);
    static readonly Color BoneWhite = new Color(0.9f, 0.88f, 0.82f);
    static readonly Color DarkRed = new Color(0.5f, 0.08f, 0.05f);
    static readonly Color ChainmailColor = new Color(0.48f, 0.5f, 0.52f);
    static readonly Color HairBlond = new Color(0.82f, 0.7f, 0.4f);

    [HideInInspector] public bool useUnscaledTime;

    bool initialized;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        ApplyStats();
        BuildBlockModel();
        CreateSelectionRing();
        CreateShadow();
    }

    void Update()
    {
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (isEnraged)
        {
            rageTimer -= delta;
            if (rageTimer <= 0f) EndRage();
        }

        if (isMarked)
        {
            markTimer -= delta;
            if (markTimer <= 0f) RemoveMark();
        }
    }

    // ========== JOINT HIERARCHY BUILDER ==========

    Transform CreatePivot(string name, Transform parent, Vector3 localPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj.transform;
    }

    void BuildSkeleton()
    {
        // Hips: root of the skeleton, at pelvis height
        pivotHips = CreatePivot("Pivot_Hips", transform, new Vector3(0, 0.65f, 0));

        // Waist: connects hips to upper body
        pivotWaist = CreatePivot("Pivot_Waist", pivotHips, new Vector3(0, 0.35f, 0));

        // Neck
        pivotNeck = CreatePivot("Pivot_Neck", pivotWaist, new Vector3(0, 0.55f, 0));

        // Shoulders
        pivotLeftShoulder = CreatePivot("Pivot_LShoulder", pivotWaist, new Vector3(-0.28f, 0.45f, 0));
        pivotRightShoulder = CreatePivot("Pivot_RShoulder", pivotWaist, new Vector3(0.28f, 0.45f, 0));

        // Elbows
        pivotLeftElbow = CreatePivot("Pivot_LElbow", pivotLeftShoulder, new Vector3(0, -0.32f, 0));
        pivotRightElbow = CreatePivot("Pivot_RElbow", pivotRightShoulder, new Vector3(0, -0.32f, 0));

        // Hands
        pivotLeftHand = CreatePivot("Pivot_LHand", pivotLeftElbow, new Vector3(0, -0.28f, 0));
        pivotRightHand = CreatePivot("Pivot_RHand", pivotRightElbow, new Vector3(0, -0.28f, 0));

        // Hips -> Legs
        pivotLeftHip = CreatePivot("Pivot_LHip", pivotHips, new Vector3(-0.12f, 0, 0));
        pivotRightHip = CreatePivot("Pivot_RHip", pivotHips, new Vector3(0.12f, 0, 0));

        // Knees
        pivotLeftKnee = CreatePivot("Pivot_LKnee", pivotLeftHip, new Vector3(0, -0.38f, 0));
        pivotRightKnee = CreatePivot("Pivot_RKnee", pivotRightHip, new Vector3(0, -0.38f, 0));

        // Cape pivot at upper back
        pivotCape = CreatePivot("Pivot_Cape", pivotWaist, new Vector3(0, 0.4f, -0.18f));
    }

    // ========== MODEL BUILDING ==========

    void BuildBlockModel()
    {
        Color primary = faction == Faction.North ? NorthPrimary : SouthPrimary;
        Color secondary = faction == Faction.North ? NorthSecondary : SouthSecondary;

        BuildSkeleton();

        switch (unitType)
        {
            case UnitType.Swordsman:   BuildSwordsmanModel(primary, secondary); break;
            case UnitType.Archer:      BuildArcherModel(primary, secondary); break;
            case UnitType.Berserker:   BuildBerserkerModel(primary, secondary); break;
            case UnitType.Shieldbearer: BuildShieldbearerModel(primary, secondary); break;
        }
    }

    // ========== SWORDSMAN ==========
    void BuildSwordsmanModel(Color primary, Color secondary)
    {
        // --- TORSO on waist pivot ---
        partBody = CreatePart(PrimitiveType.Cube, "Chest", pivotWaist,
            new Vector3(0, 0.2f, 0), new Vector3(0.52f, 0.42f, 0.3f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreatePart(PrimitiveType.Cube, "Belly", pivotWaist,
            new Vector3(0, -0.05f, 0), new Vector3(0.48f, 0.3f, 0.28f),
            ShaderHelper.ClothMaterial(primary));
        CreatePart(PrimitiveType.Cube, "Tunic", pivotHips,
            new Vector3(0, 0.15f, 0), new Vector3(0.5f, 0.3f, 0.3f),
            ShaderHelper.ClothMaterial(primary));

        // --- HEAD on neck pivot ---
        partHead = CreateSphere("Head", pivotNeck,
            new Vector3(0, 0.18f, 0), new Vector3(0.38f, 0.4f, 0.38f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        CreatePart(PrimitiveType.Cube, "LeftEye", pivotNeck,
            new Vector3(-0.09f, 0.2f, 0.16f), new Vector3(0.06f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "LeftPupil", pivotNeck,
            new Vector3(-0.09f, 0.2f, 0.175f), new Vector3(0.035f, 0.035f, 0.02f),
            ShaderHelper.CreateMaterial(new Color(0.2f, 0.3f, 0.5f)));
        CreatePart(PrimitiveType.Cube, "RightEye", pivotNeck,
            new Vector3(0.09f, 0.2f, 0.16f), new Vector3(0.06f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "RightPupil", pivotNeck,
            new Vector3(0.09f, 0.2f, 0.175f), new Vector3(0.035f, 0.035f, 0.02f),
            ShaderHelper.CreateMaterial(new Color(0.2f, 0.3f, 0.5f)));

        // Helmet (Viking nasal helm)
        CreateSphere("Helmet", pivotNeck,
            new Vector3(0, 0.32f, 0), new Vector3(0.44f, 0.22f, 0.44f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "HelmetRim", pivotNeck,
            new Vector3(0, 0.22f, 0), new Vector3(0.46f, 0.05f, 0.46f),
            ShaderHelper.SteelMaterial(IronColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "NasalGuard", pivotNeck,
            new Vector3(0, 0.2f, 0.17f), new Vector3(0.05f, 0.25f, 0.05f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "HelmetBand", pivotNeck,
            new Vector3(0, 0.34f, 0), new Vector3(0.46f, 0.035f, 0.035f),
            ShaderHelper.GoldMaterial(GoldAccent));

        // --- LEFT ARM (shield arm) ---
        partLeftArm = CreateCylinder("LUpperArm", pivotLeftShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.14f, 0.16f, 0.14f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreateCylinder("LForearm", pivotLeftElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.12f, 0.14f, 0.12f),
            ShaderHelper.ChainmailMaterial(ChainmailColor));
        CreateSphere("LHand", pivotLeftHand,
            new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Shield on left hand -- faces outward to the left (-X), flat disc
        partWeaponLeft = CreateCylinder("Shield", pivotLeftHand,
            new Vector3(-0.15f, 0.05f, 0.05f), new Vector3(0.5f, 0.025f, 0.5f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
        CreateCylinder("ShieldBoss", pivotLeftHand,
            new Vector3(-0.19f, 0.05f, 0.05f), new Vector3(0.12f, 0.04f, 0.12f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "ShieldCross1", pivotLeftHand,
            new Vector3(-0.17f, 0.05f, 0.05f), new Vector3(0.02f, 0.45f, 0.04f),
            ShaderHelper.SteelMaterial(IronColor * 0.8f));

        // --- RIGHT ARM (sword arm) ---
        partRightArm = CreateCylinder("RUpperArm", pivotRightShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.14f, 0.16f, 0.14f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreateCylinder("RForearm", pivotRightElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.12f, 0.14f, 0.12f),
            ShaderHelper.ChainmailMaterial(ChainmailColor));
        CreateSphere("RHand", pivotRightHand,
            new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Sword on right hand -- blade extends forward (Z) from grip in hand
        CreatePart(PrimitiveType.Cube, "SwordGrip", pivotRightHand,
            new Vector3(0, -0.02f, 0), new Vector3(0.04f, 0.12f, 0.04f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreateSphere("SwordPommel", pivotRightHand,
            new Vector3(0, -0.09f, 0), new Vector3(0.07f, 0.06f, 0.07f),
            ShaderHelper.GoldMaterial(GoldAccent));
        CreatePart(PrimitiveType.Cube, "SwordGuard", pivotRightHand,
            new Vector3(0, 0.04f, 0), new Vector3(0.18f, 0.035f, 0.05f),
            ShaderHelper.GoldMaterial(GoldAccent));
        partWeapon = CreatePart(PrimitiveType.Cube, "SwordBlade", pivotRightHand,
            new Vector3(0, 0.04f, 0.32f), new Vector3(0.05f, 0.1f, 0.55f),
            ShaderHelper.SteelMaterial(SteelColor)).transform;
        weaponTip = CreatePivot("WeaponTip", pivotRightHand, new Vector3(0, 0.04f, 0.6f));

        // --- LEGS ---
        partLeftLeg = CreateCylinder("LUpperLeg", pivotLeftHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.15f, 0.19f, 0.15f),
            ShaderHelper.ClothMaterial(primary * 0.7f)).transform;
        CreateCylinder("LLowerLeg", pivotLeftKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.13f, 0.17f, 0.13f),
            ShaderHelper.ClothMaterial(primary * 0.65f));
        CreatePart(PrimitiveType.Cube, "LBoot", pivotLeftKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.16f, 0.1f, 0.22f),
            ShaderHelper.LeatherMaterial(LeatherColor));

        partRightLeg = CreateCylinder("RUpperLeg", pivotRightHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.15f, 0.19f, 0.15f),
            ShaderHelper.ClothMaterial(primary * 0.7f)).transform;
        CreateCylinder("RLowerLeg", pivotRightKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.13f, 0.17f, 0.13f),
            ShaderHelper.ClothMaterial(primary * 0.65f));
        CreatePart(PrimitiveType.Cube, "RBoot", pivotRightKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.16f, 0.1f, 0.22f),
            ShaderHelper.LeatherMaterial(LeatherColor));

        // --- Belt ---
        CreatePart(PrimitiveType.Cube, "Belt", pivotHips,
            new Vector3(0, 0.3f, 0), new Vector3(0.52f, 0.06f, 0.32f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreatePart(PrimitiveType.Cube, "BeltBuckle", pivotHips,
            new Vector3(0, 0.3f, 0.16f), new Vector3(0.07f, 0.05f, 0.025f),
            ShaderHelper.GoldMaterial(GoldAccent));

        // --- Cape ---
        CreatePart(PrimitiveType.Cube, "Cape", pivotCape,
            new Vector3(0, -0.35f, -0.02f), new Vector3(0.46f, 0.65f, 0.035f),
            ShaderHelper.ClothMaterial(primary * 0.65f));
    }

    // ========== ARCHER ==========
    void BuildArcherModel(Color primary, Color secondary)
    {
        // --- TORSO ---
        partBody = CreatePart(PrimitiveType.Cube, "Chest", pivotWaist,
            new Vector3(0, 0.2f, 0), new Vector3(0.46f, 0.4f, 0.26f),
            ShaderHelper.LeatherMaterial(LeatherColor)).transform;
        CreatePart(PrimitiveType.Cube, "ChestGuard", pivotWaist,
            new Vector3(0, 0.28f, 0), new Vector3(0.44f, 0.2f, 0.24f),
            ShaderHelper.LeatherMaterial(DarkFur));
        CreatePart(PrimitiveType.Cube, "Belly", pivotHips,
            new Vector3(0, 0.15f, 0), new Vector3(0.44f, 0.3f, 0.26f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.9f));

        // --- HEAD ---
        partHead = CreateSphere("Head", pivotNeck,
            new Vector3(0, 0.18f, 0), new Vector3(0.36f, 0.38f, 0.36f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        CreatePart(PrimitiveType.Cube, "LeftEye", pivotNeck,
            new Vector3(-0.08f, 0.2f, 0.15f), new Vector3(0.055f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "LeftPupil", pivotNeck,
            new Vector3(-0.08f, 0.2f, 0.165f), new Vector3(0.03f, 0.03f, 0.02f),
            ShaderHelper.CreateMaterial(new Color(0.15f, 0.4f, 0.15f)));
        CreatePart(PrimitiveType.Cube, "RightEye", pivotNeck,
            new Vector3(0.08f, 0.2f, 0.15f), new Vector3(0.055f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "RightPupil", pivotNeck,
            new Vector3(0.08f, 0.2f, 0.165f), new Vector3(0.03f, 0.03f, 0.02f),
            ShaderHelper.CreateMaterial(new Color(0.15f, 0.4f, 0.15f)));

        // Hood
        CreateSphere("Hood", pivotNeck,
            new Vector3(0, 0.26f, -0.04f), new Vector3(0.44f, 0.32f, 0.44f),
            ShaderHelper.ClothMaterial(primary * 0.75f));
        CreatePart(PrimitiveType.Cube, "HoodBrim", pivotNeck,
            new Vector3(0, 0.13f, 0.13f), new Vector3(0.42f, 0.06f, 0.08f),
            ShaderHelper.ClothMaterial(primary * 0.6f));

        // --- LEFT ARM (bow arm) ---
        partLeftArm = CreateCylinder("LUpperArm", pivotLeftShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.11f, 0.16f, 0.11f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        CreatePart(PrimitiveType.Cube, "LBracer", pivotLeftElbow,
            new Vector3(0, -0.06f, 0), new Vector3(0.13f, 0.16f, 0.13f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreateSphere("LHand", pivotLeftHand,
            new Vector3(0, 0, 0), new Vector3(0.09f, 0.09f, 0.09f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Bow on left hand -- bow staves extend up/down, held forward at arm's length
        partWeaponLeft = CreatePart(PrimitiveType.Cube, "BowUpper", pivotLeftHand,
            new Vector3(0, 0.25f, 0.12f), new Vector3(0.03f, 0.32f, 0.08f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        CreatePart(PrimitiveType.Cube, "BowLower", pivotLeftHand,
            new Vector3(0, -0.22f, 0.12f), new Vector3(0.03f, 0.3f, 0.07f),
            ShaderHelper.WoodMaterial(WoodColor));
        CreatePart(PrimitiveType.Cube, "BowString", pivotLeftHand,
            new Vector3(0, 0f, 0.04f), new Vector3(0.015f, 0.55f, 0.015f),
            ShaderHelper.BoneMaterial(BoneWhite));
        weaponLeftTip = CreatePivot("BowTip", pivotLeftHand, new Vector3(0, 0.4f, 0.12f));

        // --- RIGHT ARM (draw arm) ---
        partRightArm = CreateCylinder("RUpperArm", pivotRightShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.11f, 0.16f, 0.11f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        CreatePart(PrimitiveType.Cube, "RBracer", pivotRightElbow,
            new Vector3(0, -0.06f, 0), new Vector3(0.13f, 0.16f, 0.13f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreateSphere("RHand", pivotRightHand,
            new Vector3(0, 0, 0), new Vector3(0.09f, 0.09f, 0.09f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Quiver on back
        CreatePart(PrimitiveType.Cube, "Quiver", pivotWaist,
            new Vector3(0.13f, 0.2f, -0.16f), new Vector3(0.1f, 0.42f, 0.1f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.85f));
        CreatePart(PrimitiveType.Cube, "Arrows", pivotWaist,
            new Vector3(0.13f, 0.44f, -0.16f), new Vector3(0.06f, 0.08f, 0.06f),
            ShaderHelper.WoodMaterial(WoodColor));

        // --- LEGS ---
        partLeftLeg = CreateCylinder("LUpperLeg", pivotLeftHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.13f, 0.19f, 0.13f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.8f)).transform;
        CreateCylinder("LLowerLeg", pivotLeftKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.11f, 0.17f, 0.11f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.75f));
        CreatePart(PrimitiveType.Cube, "LBoot", pivotLeftKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.14f, 0.1f, 0.2f),
            ShaderHelper.LeatherMaterial(DarkFur));

        partRightLeg = CreateCylinder("RUpperLeg", pivotRightHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.13f, 0.19f, 0.13f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.8f)).transform;
        CreateCylinder("RLowerLeg", pivotRightKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.11f, 0.17f, 0.11f),
            ShaderHelper.LeatherMaterial(LeatherColor * 0.75f));
        CreatePart(PrimitiveType.Cube, "RBoot", pivotRightKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.14f, 0.1f, 0.2f),
            ShaderHelper.LeatherMaterial(DarkFur));

        // Belt
        CreatePart(PrimitiveType.Cube, "Belt", pivotHips,
            new Vector3(0, 0.3f, 0), new Vector3(0.46f, 0.06f, 0.28f),
            ShaderHelper.LeatherMaterial(DarkFur));

        // Cape (short cloak)
        CreatePart(PrimitiveType.Cube, "Cape", pivotCape,
            new Vector3(0, -0.3f, -0.02f), new Vector3(0.4f, 0.55f, 0.03f),
            ShaderHelper.ClothMaterial(primary * 0.55f));
    }

    // ========== BERSERKER ==========
    void BuildBerserkerModel(Color primary, Color secondary)
    {
        // Wider skeleton for berserker
        pivotLeftShoulder.localPosition = new Vector3(-0.33f, 0.45f, 0);
        pivotRightShoulder.localPosition = new Vector3(0.33f, 0.45f, 0);
        pivotLeftHip.localPosition = new Vector3(-0.14f, 0, 0);
        pivotRightHip.localPosition = new Vector3(0.14f, 0, 0);

        // --- TORSO (bare chest + fur) ---
        partBody = CreatePart(PrimitiveType.Cube, "Chest", pivotWaist,
            new Vector3(0, 0.2f, 0), new Vector3(0.6f, 0.44f, 0.34f),
            ShaderHelper.SkinMaterial(SkinColor * 0.9f)).transform;
        CreatePart(PrimitiveType.Cube, "FurCloak", pivotWaist,
            new Vector3(0, 0.28f, -0.08f), new Vector3(0.65f, 0.35f, 0.28f),
            ShaderHelper.FurMaterial(FurColor));
        CreatePart(PrimitiveType.Cube, "FurCollar", pivotWaist,
            new Vector3(0, 0.48f, 0), new Vector3(0.62f, 0.12f, 0.32f),
            ShaderHelper.FurMaterial(DarkFur));
        CreatePart(PrimitiveType.Cube, "Belly", pivotHips,
            new Vector3(0, 0.15f, 0), new Vector3(0.56f, 0.3f, 0.32f),
            ShaderHelper.SkinMaterial(SkinColor * 0.88f));

        // War paint
        CreatePart(PrimitiveType.Cube, "WarPaintH", pivotWaist,
            new Vector3(0, 0.18f, 0.16f), new Vector3(0.32f, 0.06f, 0.03f),
            ShaderHelper.CreateMaterial(DarkRed, 0, 0.1f, DarkRed * 0.3f));
        CreatePart(PrimitiveType.Cube, "WarPaintV", pivotWaist,
            new Vector3(0, 0.18f, 0.16f), new Vector3(0.06f, 0.28f, 0.03f),
            ShaderHelper.CreateMaterial(DarkRed, 0, 0.1f, DarkRed * 0.3f));

        // Scars
        CreatePart(PrimitiveType.Cube, "Scar1", pivotWaist,
            new Vector3(-0.12f, 0.25f, 0.16f), new Vector3(0.18f, 0.025f, 0.025f),
            ShaderHelper.CreateMaterial(new Color(0.7f, 0.45f, 0.4f)));
        CreatePart(PrimitiveType.Cube, "Scar2", pivotWaist,
            new Vector3(0.1f, 0.08f, 0.16f), new Vector3(0.025f, 0.15f, 0.025f),
            ShaderHelper.CreateMaterial(new Color(0.7f, 0.45f, 0.4f)));

        // --- HEAD ---
        partHead = CreateSphere("Head", pivotNeck,
            new Vector3(0, 0.18f, 0), new Vector3(0.4f, 0.42f, 0.4f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        // Fierce red eyes
        CreatePart(PrimitiveType.Cube, "LeftEye", pivotNeck,
            new Vector3(-0.09f, 0.22f, 0.17f), new Vector3(0.065f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "LeftPupil", pivotNeck,
            new Vector3(-0.09f, 0.22f, 0.185f), new Vector3(0.04f, 0.04f, 0.02f),
            ShaderHelper.CreateMaterial(DarkRed, 0, 0.1f, DarkRed * 0.5f));
        CreatePart(PrimitiveType.Cube, "RightEye", pivotNeck,
            new Vector3(0.09f, 0.22f, 0.17f), new Vector3(0.065f, 0.04f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "RightPupil", pivotNeck,
            new Vector3(0.09f, 0.22f, 0.185f), new Vector3(0.04f, 0.04f, 0.02f),
            ShaderHelper.CreateMaterial(DarkRed, 0, 0.1f, DarkRed * 0.5f));

        // Wild hair
        CreateSphere("Hair", pivotNeck,
            new Vector3(0, 0.32f, -0.04f), new Vector3(0.44f, 0.18f, 0.44f),
            ShaderHelper.FurMaterial(HairBlond));
        CreatePart(PrimitiveType.Cube, "HairBack", pivotNeck,
            new Vector3(0, 0.2f, -0.18f), new Vector3(0.3f, 0.35f, 0.08f),
            ShaderHelper.FurMaterial(HairBlond));
        CreatePart(PrimitiveType.Cube, "HairWild1", pivotNeck,
            new Vector3(-0.17f, 0.36f, 0.04f), new Vector3(0.1f, 0.12f, 0.1f),
            ShaderHelper.FurMaterial(HairBlond * 0.9f));
        CreatePart(PrimitiveType.Cube, "HairWild2", pivotNeck,
            new Vector3(0.15f, 0.38f, -0.04f), new Vector3(0.08f, 0.1f, 0.08f),
            ShaderHelper.FurMaterial(HairBlond * 0.95f));
        CreatePart(PrimitiveType.Cube, "Beard", pivotNeck,
            new Vector3(0, 0.02f, 0.15f), new Vector3(0.2f, 0.18f, 0.08f),
            ShaderHelper.FurMaterial(HairBlond * 0.85f));
        CreatePart(PrimitiveType.Cube, "BeardBraid", pivotNeck,
            new Vector3(0, -0.1f, 0.16f), new Vector3(0.06f, 0.12f, 0.05f),
            ShaderHelper.FurMaterial(HairBlond * 0.8f));

        // --- ARMS (massive) ---
        partLeftArm = CreateCylinder("LUpperArm", pivotLeftShoulder,
            new Vector3(0, -0.16f, 0), new Vector3(0.16f, 0.17f, 0.16f),
            ShaderHelper.SkinMaterial(SkinColor * 0.9f)).transform;
        CreatePart(PrimitiveType.Cube, "LArmband", pivotLeftShoulder,
            new Vector3(0, -0.05f, 0), new Vector3(0.18f, 0.06f, 0.18f),
            ShaderHelper.GoldMaterial(GoldAccent));
        CreateCylinder("LForearm", pivotLeftElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.14f, 0.15f, 0.14f),
            ShaderHelper.SkinMaterial(SkinColor * 0.88f));
        CreateSphere("LHand", pivotLeftHand,
            new Vector3(0, 0, 0), new Vector3(0.11f, 0.11f, 0.11f),
            ShaderHelper.SkinMaterial(SkinColor));

        partRightArm = CreateCylinder("RUpperArm", pivotRightShoulder,
            new Vector3(0, -0.16f, 0), new Vector3(0.16f, 0.17f, 0.16f),
            ShaderHelper.SkinMaterial(SkinColor * 0.9f)).transform;
        CreatePart(PrimitiveType.Cube, "RArmband", pivotRightShoulder,
            new Vector3(0, -0.05f, 0), new Vector3(0.18f, 0.06f, 0.18f),
            ShaderHelper.GoldMaterial(GoldAccent));
        CreateCylinder("RForearm", pivotRightElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.14f, 0.15f, 0.14f),
            ShaderHelper.SkinMaterial(SkinColor * 0.88f));
        CreateSphere("RHand", pivotRightHand,
            new Vector3(0, 0, 0), new Vector3(0.11f, 0.11f, 0.11f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Left Axe -- handle extends forward (Z) from grip, axe head at the end
        partWeaponLeft = CreatePart(PrimitiveType.Cube, "LAxeHandle", pivotLeftHand,
            new Vector3(0, 0, 0.28f), new Vector3(0.04f, 0.04f, 0.52f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        CreatePart(PrimitiveType.Cube, "LAxeHead", pivotLeftHand,
            new Vector3(0, 0.08f, 0.5f), new Vector3(0.03f, 0.24f, 0.14f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "LAxeEdge", pivotLeftHand,
            new Vector3(0, 0.2f, 0.5f), new Vector3(0.02f, 0.04f, 0.1f),
            ShaderHelper.SteelMaterial(SteelColor));
        weaponLeftTip = CreatePivot("LAxeTip", pivotLeftHand, new Vector3(0, 0.2f, 0.56f));

        // Right Axe -- handle extends forward (Z) from grip, axe head at the end
        partWeapon = CreatePart(PrimitiveType.Cube, "RAxeHandle", pivotRightHand,
            new Vector3(0, 0, 0.28f), new Vector3(0.04f, 0.04f, 0.52f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        CreatePart(PrimitiveType.Cube, "RAxeHead", pivotRightHand,
            new Vector3(0, 0.08f, 0.5f), new Vector3(0.03f, 0.24f, 0.14f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "RAxeEdge", pivotRightHand,
            new Vector3(0, 0.2f, 0.5f), new Vector3(0.02f, 0.04f, 0.1f),
            ShaderHelper.SteelMaterial(SteelColor));
        weaponTip = CreatePivot("RAxeTip", pivotRightHand, new Vector3(0, 0.2f, 0.56f));

        // --- LEGS ---
        partLeftLeg = CreateCylinder("LUpperLeg", pivotLeftHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.17f, 0.19f, 0.17f),
            ShaderHelper.FurMaterial(FurColor)).transform;
        CreateCylinder("LLowerLeg", pivotLeftKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.15f, 0.17f, 0.15f),
            ShaderHelper.FurMaterial(FurColor * 0.9f));
        CreatePart(PrimitiveType.Cube, "LBoot", pivotLeftKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.18f, 0.12f, 0.24f),
            ShaderHelper.LeatherMaterial(DarkFur));

        partRightLeg = CreateCylinder("RUpperLeg", pivotRightHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.17f, 0.19f, 0.17f),
            ShaderHelper.FurMaterial(FurColor)).transform;
        CreateCylinder("RLowerLeg", pivotRightKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.15f, 0.17f, 0.15f),
            ShaderHelper.FurMaterial(FurColor * 0.9f));
        CreatePart(PrimitiveType.Cube, "RBoot", pivotRightKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.18f, 0.12f, 0.24f),
            ShaderHelper.LeatherMaterial(DarkFur));

        // Belt with skull
        CreatePart(PrimitiveType.Cube, "Belt", pivotHips,
            new Vector3(0, 0.3f, 0), new Vector3(0.58f, 0.08f, 0.34f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreateSphere("BeltSkull", pivotHips,
            new Vector3(0, 0.3f, 0.17f), new Vector3(0.1f, 0.1f, 0.07f),
            ShaderHelper.BoneMaterial(BoneWhite));
    }

    // ========== SHIELDBEARER ==========
    void BuildShieldbearerModel(Color primary, Color secondary)
    {
        // Stocky skeleton
        pivotLeftShoulder.localPosition = new Vector3(-0.3f, 0.43f, 0);
        pivotRightShoulder.localPosition = new Vector3(0.3f, 0.43f, 0);
        pivotLeftHip.localPosition = new Vector3(-0.14f, 0, 0);
        pivotRightHip.localPosition = new Vector3(0.14f, 0, 0);

        // --- TORSO ---
        partBody = CreatePart(PrimitiveType.Cube, "Chest", pivotWaist,
            new Vector3(0, 0.2f, 0), new Vector3(0.56f, 0.44f, 0.32f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreatePart(PrimitiveType.Cube, "Surcoat", pivotHips,
            new Vector3(0, 0.2f, 0), new Vector3(0.58f, 0.38f, 0.34f),
            ShaderHelper.ClothMaterial(primary));
        CreatePart(PrimitiveType.Cube, "SurcoatCross", pivotHips,
            new Vector3(0, 0.25f, 0.17f), new Vector3(0.1f, 0.28f, 0.025f),
            ShaderHelper.GoldMaterial(GoldAccent));
        CreatePart(PrimitiveType.Cube, "SurcoatCrossH", pivotHips,
            new Vector3(0, 0.3f, 0.17f), new Vector3(0.24f, 0.065f, 0.025f),
            ShaderHelper.GoldMaterial(GoldAccent));

        // --- HEAD ---
        partHead = CreateSphere("Head", pivotNeck,
            new Vector3(0, 0.18f, 0), new Vector3(0.38f, 0.4f, 0.38f),
            ShaderHelper.SkinMaterial(SkinColor)).transform;
        CreatePart(PrimitiveType.Cube, "LeftEye", pivotNeck,
            new Vector3(-0.07f, 0.2f, 0.16f), new Vector3(0.05f, 0.035f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));
        CreatePart(PrimitiveType.Cube, "RightEye", pivotNeck,
            new Vector3(0.07f, 0.2f, 0.16f), new Vector3(0.05f, 0.035f, 0.03f),
            ShaderHelper.CreateMaterial(BoneWhite));

        // Full helmet with crest
        CreateSphere("Helmet", pivotNeck,
            new Vector3(0, 0.3f, 0), new Vector3(0.46f, 0.24f, 0.46f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "HelmetRim", pivotNeck,
            new Vector3(0, 0.2f, 0), new Vector3(0.48f, 0.05f, 0.48f),
            ShaderHelper.SteelMaterial(IronColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "HelmetCrest", pivotNeck,
            new Vector3(0, 0.42f, 0), new Vector3(0.05f, 0.08f, 0.36f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "FacePlate", pivotNeck,
            new Vector3(0, 0.18f, 0.17f), new Vector3(0.3f, 0.1f, 0.04f),
            ShaderHelper.SteelMaterial(IronColor * 0.65f));

        // --- LEFT ARM (shield arm) ---
        partLeftArm = CreateCylinder("LUpperArm", pivotLeftShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.15f, 0.16f, 0.15f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreatePart(PrimitiveType.Cube, "LShoulder", pivotLeftShoulder,
            new Vector3(0, -0.02f, 0), new Vector3(0.2f, 0.1f, 0.2f),
            ShaderHelper.SteelMaterial(IronColor));
        CreateCylinder("LForearm", pivotLeftElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.13f, 0.14f, 0.13f),
            ShaderHelper.ChainmailMaterial(ChainmailColor));
        CreateSphere("LHand", pivotLeftHand,
            new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Big shield on left hand -- faces outward to the left (-X)
        partWeaponLeft = CreateCylinder("BigShield", pivotLeftHand,
            new Vector3(-0.18f, 0.08f, 0.06f), new Vector3(0.7f, 0.03f, 0.7f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        partWeaponLeft.localRotation = Quaternion.Euler(0, 0, 90);
        CreateCylinder("ShieldBoss", pivotLeftHand,
            new Vector3(-0.22f, 0.08f, 0.06f), new Vector3(0.16f, 0.05f, 0.16f),
            ShaderHelper.SteelMaterial(IronColor));
        CreatePart(PrimitiveType.Cube, "ShieldRimT", pivotLeftHand,
            new Vector3(-0.18f, 0.43f, 0.06f), new Vector3(0.02f, 0.03f, 0.7f),
            ShaderHelper.SteelMaterial(IronColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "ShieldRimB", pivotLeftHand,
            new Vector3(-0.18f, -0.27f, 0.06f), new Vector3(0.02f, 0.03f, 0.7f),
            ShaderHelper.SteelMaterial(IronColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "ShieldEmb", pivotLeftHand,
            new Vector3(-0.21f, 0.08f, 0.06f), new Vector3(0.02f, 0.2f, 0.035f),
            ShaderHelper.ClothMaterial(primary));
        CreatePart(PrimitiveType.Cube, "ShieldEmbH", pivotLeftHand,
            new Vector3(-0.21f, 0.08f, 0.06f), new Vector3(0.02f, 0.035f, 0.2f),
            ShaderHelper.ClothMaterial(primary));

        // --- RIGHT ARM (spear arm) ---
        partRightArm = CreateCylinder("RUpperArm", pivotRightShoulder,
            new Vector3(0, -0.15f, 0), new Vector3(0.15f, 0.16f, 0.15f),
            ShaderHelper.ChainmailMaterial(ChainmailColor)).transform;
        CreatePart(PrimitiveType.Cube, "RShoulder", pivotRightShoulder,
            new Vector3(0, -0.02f, 0), new Vector3(0.2f, 0.1f, 0.2f),
            ShaderHelper.SteelMaterial(IronColor));
        CreateCylinder("RForearm", pivotRightElbow,
            new Vector3(0, -0.12f, 0), new Vector3(0.13f, 0.14f, 0.13f),
            ShaderHelper.ChainmailMaterial(ChainmailColor));
        CreateSphere("RHand", pivotRightHand,
            new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f),
            ShaderHelper.SkinMaterial(SkinColor));

        // Spear on right hand -- shaft extends forward (Z) from grip
        partWeapon = CreateCylinder("SpearShaft", pivotRightHand,
            new Vector3(0, 0, 0.55f), new Vector3(0.035f, 0.035f, 0.6f),
            ShaderHelper.WoodMaterial(WoodColor)).transform;
        partWeapon.localRotation = Quaternion.Euler(90, 0, 0);
        CreatePart(PrimitiveType.Cube, "SpearHead", pivotRightHand,
            new Vector3(0, 0, 1.05f), new Vector3(0.035f, 0.08f, 0.15f),
            ShaderHelper.SteelMaterial(SteelColor));
        CreatePart(PrimitiveType.Cube, "SpearEdge", pivotRightHand,
            new Vector3(0, 0, 1.14f), new Vector3(0.02f, 0.04f, 0.06f),
            ShaderHelper.SteelMaterial(SteelColor * 1.1f));
        weaponTip = CreatePivot("SpearTip", pivotRightHand, new Vector3(0, 0, 1.18f));

        // --- LEGS ---
        partLeftLeg = CreateCylinder("LUpperLeg", pivotLeftHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.16f, 0.19f, 0.16f),
            ShaderHelper.ChainmailMaterial(ChainmailColor * 0.85f)).transform;
        CreateCylinder("LLowerLeg", pivotLeftKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.14f, 0.17f, 0.14f),
            ShaderHelper.ChainmailMaterial(ChainmailColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "LBoot", pivotLeftKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.17f, 0.12f, 0.24f),
            ShaderHelper.LeatherMaterial(LeatherColor));

        partRightLeg = CreateCylinder("RUpperLeg", pivotRightHip,
            new Vector3(0, -0.18f, 0), new Vector3(0.16f, 0.19f, 0.16f),
            ShaderHelper.ChainmailMaterial(ChainmailColor * 0.85f)).transform;
        CreateCylinder("RLowerLeg", pivotRightKnee,
            new Vector3(0, -0.16f, 0), new Vector3(0.14f, 0.17f, 0.14f),
            ShaderHelper.ChainmailMaterial(ChainmailColor * 0.8f));
        CreatePart(PrimitiveType.Cube, "RBoot", pivotRightKnee,
            new Vector3(0, -0.34f, 0.02f), new Vector3(0.17f, 0.12f, 0.24f),
            ShaderHelper.LeatherMaterial(LeatherColor));

        // Belt
        CreatePart(PrimitiveType.Cube, "Belt", pivotHips,
            new Vector3(0, 0.3f, 0), new Vector3(0.58f, 0.06f, 0.34f),
            ShaderHelper.LeatherMaterial(LeatherColor));
        CreatePart(PrimitiveType.Cube, "BeltBuckle", pivotHips,
            new Vector3(0, 0.3f, 0.17f), new Vector3(0.07f, 0.05f, 0.025f),
            ShaderHelper.GoldMaterial(GoldAccent));

        // Cape
        CreatePart(PrimitiveType.Cube, "Cape", pivotCape,
            new Vector3(0, -0.38f, -0.02f), new Vector3(0.5f, 0.7f, 0.035f),
            ShaderHelper.ClothMaterial(primary * 0.55f));
    }

    // ========== PART CREATION HELPERS ==========

    GameObject CreatePart(PrimitiveType type, string partName, Transform parent,
        Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = partName;
        part.transform.SetParent(parent);
        part.transform.localPosition = localPos;
        part.transform.localScale = scale;
        part.transform.localRotation = Quaternion.identity;

        Renderer rend = part.GetComponent<Renderer>();
        rend.material = mat;

        Collider col = part.GetComponent<Collider>();
        if (col != null) Destroy(col);

        part.layer = gameObject.layer;
        return part;
    }

    GameObject CreateSphere(string partName, Transform parent,
        Vector3 localPos, Vector3 scale, Material mat)
    {
        return CreatePart(PrimitiveType.Sphere, partName, parent, localPos, scale, mat);
    }

    GameObject CreateCylinder(string partName, Transform parent,
        Vector3 localPos, Vector3 scale, Material mat)
    {
        return CreatePart(PrimitiveType.Cylinder, partName, parent, localPos, scale, mat);
    }

    // Legacy helper for backward compat (used by ability effects below)
    GameObject CreateCubePart(string partName, Vector3 localPos, Vector3 scale, Color color)
    {
        return CreatePart(PrimitiveType.Cube, partName, transform, localPos, scale,
            ShaderHelper.CreateMaterial(color));
    }

    void CreateSelectionRing()
    {
        selectionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        selectionRing.name = "SelectionRing";
        selectionRing.transform.SetParent(transform);
        selectionRing.transform.localPosition = new Vector3(0, 0.05f, 0);
        selectionRing.transform.localScale = new Vector3(1.3f, 0.015f, 1.3f);

        Renderer rend = selectionRing.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(
            new Color(0.2f, 1f, 0.3f, 0.6f), 0f, 0.8f,
            new Color(0.1f, 0.8f, 0.2f) * 0.5f);
        mat.renderQueue = 3000;
        rend.material = mat;

        Destroy(selectionRing.GetComponent<Collider>());
        selectionRing.SetActive(false);
    }

    void CreateShadow()
    {
        GameObject shadow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shadow.name = "Shadow";
        shadow.transform.SetParent(transform);
        shadow.transform.localPosition = new Vector3(0, 0.02f, 0);
        shadow.transform.localScale = new Vector3(0.85f, 0.008f, 0.85f);

        Renderer rend = shadow.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0f, 0f, 0f, 0.3f));
        mat.renderQueue = 2999;
        rend.material = mat;

        Destroy(shadow.GetComponent<Collider>());
    }

    // ========== STATE ==========

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionRing != null)
            selectionRing.SetActive(selected);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float totalArmor = armor;
        if (isShieldWalling) totalArmor += shieldWallArmor;

        UnitCombat combat = GetComponent<UnitCombat>();
        if (combat != null)
        {
            float parryReduction = combat.GetParryDamageReduction();
            if (parryReduction > 0f)
            {
                damage *= (1f - parryReduction);
                SpawnParryEffect();
            }
        }

        float finalDamage = Mathf.Max(damage - totalArmor, 1f);
        if (isMarked) finalDamage *= 1.4f;

        currentHealth -= finalDamage;

        // Floating damage number
        DamagePopup.Spawn(transform.position + Vector3.up * 2.2f, finalDamage,
            isMarked, totalArmor > 0 && damage <= totalArmor * 2f);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] origColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                origColors[i] = renderers[i].material.color;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            Color c = origColors[i];
            renderers[i].material.color = Color.Lerp(c, Color.red, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            renderers[i].material.color = origColors[i];
        }
    }

    void SpawnParryEffect()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spark.name = "ParrySpark";
            spark.transform.position = transform.position + Vector3.up * 1.2f +
                transform.forward * 0.3f + Random.insideUnitSphere * 0.2f;
            spark.transform.localScale = Vector3.one * Random.Range(0.04f, 0.08f);
            spark.transform.rotation = Random.rotation;

            Renderer rend = spark.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(
                new Color(1f, 0.95f, 0.5f, 0.9f), 0.8f, 0.9f,
                new Color(1f, 0.9f, 0.3f));
            rend.material.renderQueue = 3100;
            Destroy(spark.GetComponent<Collider>());

            Rigidbody rb = spark.AddComponent<Rigidbody>();
            rb.mass = 0.02f;
            rb.useGravity = false;
            rb.AddForce(Random.insideUnitSphere * 4f + Vector3.up * 2f, ForceMode.Impulse);

            Destroy(spark, 0.2f);
        }

        // Gold flash
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Cube);
        flash.name = "ParryFlash";
        flash.transform.position = transform.position + Vector3.up * 1.2f + transform.forward * 0.3f;
        flash.transform.localScale = new Vector3(0.5f, 0.5f, 0.03f);
        flash.transform.rotation = transform.rotation;
        Renderer fr = flash.GetComponent<Renderer>();
        fr.material = ShaderHelper.CreateMaterial(
            new Color(1f, 0.9f, 0.4f, 0.8f), 0f, 0.8f,
            new Color(1f, 0.85f, 0.3f) * 2f);
        fr.material.renderQueue = 3100;
        Destroy(flash.GetComponent<Collider>());
        Destroy(flash, 0.12f);
    }

    void Die()
    {
        isDead = true;
        FactionManager.Instance?.OnUnitDied(this);
        SpawnDeathParticles();

        CameraShake.Shake(0.08f, 0.1f);

        Destroy(gameObject, 1.8f);
    }

    void SpawnDeathParticles()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            p.name = "DeathDebris";
            p.transform.position = transform.position + Vector3.up * 1f + Random.insideUnitSphere * 0.5f;
            float size = Random.Range(0.04f, 0.1f);
            p.transform.localScale = Vector3.one * size;
            p.transform.rotation = Random.rotation;

            Renderer rend = p.GetComponent<Renderer>();
            Color debrisColor = (faction == Faction.North) ? NorthPrimary : SouthPrimary;
            debrisColor = Color.Lerp(debrisColor, IronColor, Random.Range(0f, 0.5f));
            rend.material = ShaderHelper.CreateMaterial(debrisColor, 0.3f, 0.3f);

            Destroy(p.GetComponent<Collider>());

            Rigidbody rb = p.AddComponent<Rigidbody>();
            rb.mass = 0.08f;
            rb.AddForce(Random.insideUnitSphere * 4f + Vector3.up * 3f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

            Destroy(p, 2f);
        }
    }

    // ========== ABILITIES ==========

    public void ActivateRage()
    {
        if (unitType != UnitType.Berserker || isEnraged) return;

        isEnraged = true;
        rageTimer = rageDuration;

        attackDamage *= 1.6f;
        moveSpeed *= 1.3f;
        armor *= 0.6f;

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;

        rageEffect = CreatePart(PrimitiveType.Cylinder, "RageGround", transform,
            new Vector3(0, 0.03f, 0), new Vector3(1.2f, 0.01f, 1.2f),
            ShaderHelper.CreateMaterial(
                new Color(1f, 0.3f, 0f, 0.3f), 0, 0.8f,
                new Color(1f, 0.2f, 0f) * 1.5f));
        rageEffect.GetComponent<Renderer>().material.renderQueue = 3100;

        CameraShake.Shake(0.2f, 0.3f);
    }

    void EndRage()
    {
        isEnraged = false;
        ApplyStats();

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;

        if (rageEffect != null) Destroy(rageEffect);
    }

    public void ActivateShieldWall()
    {
        if (unitType != UnitType.Shieldbearer) return;

        isShieldWalling = !isShieldWalling;

        if (isShieldWalling)
        {
            moveSpeed *= 0.3f;
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            shieldWallEffect = CreatePart(PrimitiveType.Cylinder, "ShieldWallGround", transform,
                new Vector3(0, 0.03f, 0), new Vector3(1.4f, 0.01f, 1.4f),
                ShaderHelper.CreateMaterial(
                    new Color(0.85f, 0.75f, 0.2f, 0.25f), 0, 0.8f,
                    new Color(0.85f, 0.7f, 0.15f) * 1.2f));
            shieldWallEffect.GetComponent<Renderer>().material.renderQueue = 3100;
        }
        else
        {
            ApplyStats();
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            if (shieldWallEffect != null) Destroy(shieldWallEffect);
        }
    }

    public void ApplyMark(float duration)
    {
        isMarked = true;
        markTimer = duration;

        if (markEffect != null) Destroy(markEffect);

        // Target reticle: three concentric rings
        markEffect = new GameObject("MarkReticle");
        markEffect.transform.SetParent(transform);
        markEffect.transform.localPosition = new Vector3(0, 2.6f, 0);

        for (int i = 0; i < 3; i++)
        {
            float ringSize = 0.12f + i * 0.08f;
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = $"MarkRing{i}";
            ring.transform.SetParent(markEffect.transform);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localScale = new Vector3(ringSize, 0.008f, ringSize);
            ring.transform.localRotation = Quaternion.Euler(0, i * 60f, 0);

            Renderer rend = ring.GetComponent<Renderer>();
            float intensity = 1f - i * 0.2f;
            rend.material = ShaderHelper.CreateMaterial(
                new Color(1f, 0.35f, 0f, 0.7f * intensity), 0, 0.8f,
                new Color(1f, 0.3f, 0f) * (2f * intensity));
            rend.material.renderQueue = 3100;
            Destroy(ring.GetComponent<Collider>());
        }
    }

    void RemoveMark()
    {
        isMarked = false;
        if (markEffect != null) Destroy(markEffect);
    }

    // ========== STATS ==========

    public void ApplyStats()
    {
        switch (unitType)
        {
            case UnitType.Swordsman:
                maxHealth = 110f;
                attackDamage = 14f;
                attackRange = 2.5f;
                attackCooldown = 0.9f;
                moveSpeed = 3.8f;
                armor = 4f;
                break;

            case UnitType.Archer:
                maxHealth = 55f;
                attackDamage = 11f;
                attackRange = 14f;
                attackCooldown = 1.4f;
                moveSpeed = 4.2f;
                armor = 0f;
                break;

            case UnitType.Berserker:
                maxHealth = 85f;
                attackDamage = 22f;
                attackRange = 2.8f;
                attackCooldown = 0.7f;
                moveSpeed = 4.0f;
                armor = 2f;
                rageDuration = 6f;
                break;

            case UnitType.Shieldbearer:
                maxHealth = 140f;
                attackDamage = 8f;
                attackRange = 3.0f;
                attackCooldown = 1.6f;
                moveSpeed = 3.0f;
                armor = 8f;
                shieldWallArmor = 15f;
                break;
        }
        currentHealth = maxHealth;
    }
}
