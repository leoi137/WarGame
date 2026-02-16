using UnityEngine;
using UnityEngine.AI;

public enum Faction { North, South }
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
    public float armor; // Flat damage reduction

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

    // Body part references for animator
    [HideInInspector] public Transform partHead;
    [HideInInspector] public Transform partBody;
    [HideInInspector] public Transform partLeftArm;
    [HideInInspector] public Transform partRightArm;
    [HideInInspector] public Transform partLeftLeg;
    [HideInInspector] public Transform partRightLeg;
    [HideInInspector] public Transform partWeapon;       // Primary weapon (sword/bow/axeR/spear)
    [HideInInspector] public Transform partWeaponLeft;   // Offhand (shield/axeL)

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

    // ========== MODEL BUILDING ==========

    void BuildBlockModel()
    {
        Color primary = faction == Faction.North ? NorthPrimary : SouthPrimary;
        Color secondary = faction == Faction.North ? NorthSecondary : SouthSecondary;

        switch (unitType)
        {
            case UnitType.Archer:      BuildArcherModel(primary, secondary); break;
            case UnitType.Swordsman:   BuildSwordsmanModel(primary, secondary); break;
            case UnitType.Berserker:   BuildBerserkerModel(primary, secondary); break;
            case UnitType.Shieldbearer: BuildShieldbearerModel(primary, secondary); break;
        }
    }

    void BuildArcherModel(Color primary, Color secondary)
    {
        partBody = CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.5f, 0.85f, 0.3f), LeatherColor).transform;
        CreateCubePart("Chest", new Vector3(0, 1.35f, 0), new Vector3(0.48f, 0.25f, 0.28f), DarkFur);
        partHead = CreateCubePart("Head", new Vector3(0, 1.88f, 0), new Vector3(0.4f, 0.42f, 0.4f), SkinColor).transform;
        // Eyes
        CreateCubePart("LeftEye", new Vector3(-0.1f, 1.92f, 0.18f), new Vector3(0.07f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("LeftPupil", new Vector3(-0.1f, 1.92f, 0.2f), new Vector3(0.04f, 0.04f, 0.02f), new Color(0.15f, 0.15f, 0.15f));
        CreateCubePart("RightEye", new Vector3(0.1f, 1.92f, 0.18f), new Vector3(0.07f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("RightPupil", new Vector3(0.1f, 1.92f, 0.2f), new Vector3(0.04f, 0.04f, 0.02f), new Color(0.15f, 0.15f, 0.15f));
        // Hood
        CreateCubePart("Hood", new Vector3(0, 2.0f, -0.05f), new Vector3(0.48f, 0.3f, 0.48f), primary * 0.8f);
        CreateCubePart("HoodBrim", new Vector3(0, 1.85f, 0.15f), new Vector3(0.5f, 0.08f, 0.1f), primary * 0.7f);
        // Legs
        partLeftLeg = CreateCubePart("LeftLeg", new Vector3(-0.13f, 0.35f, 0), new Vector3(0.18f, 0.6f, 0.2f), LeatherColor * 0.8f).transform;
        partRightLeg = CreateCubePart("RightLeg", new Vector3(0.13f, 0.35f, 0), new Vector3(0.18f, 0.6f, 0.2f), LeatherColor * 0.8f).transform;
        CreateCubePart("LeftBoot", new Vector3(-0.13f, 0.08f, 0.02f), new Vector3(0.2f, 0.14f, 0.26f), DarkFur);
        CreateCubePart("RightBoot", new Vector3(0.13f, 0.08f, 0.02f), new Vector3(0.2f, 0.14f, 0.26f), DarkFur);
        // Arms
        partLeftArm = CreateCubePart("LeftArm", new Vector3(-0.38f, 1.15f, 0), new Vector3(0.15f, 0.65f, 0.17f), SkinColor).transform;
        CreateCubePart("LeftBracer", new Vector3(-0.38f, 0.95f, 0), new Vector3(0.17f, 0.22f, 0.19f), LeatherColor);
        partRightArm = CreateCubePart("RightArm", new Vector3(0.38f, 1.15f, 0), new Vector3(0.15f, 0.65f, 0.17f), SkinColor).transform;
        CreateCubePart("RightBracer", new Vector3(0.38f, 0.95f, 0), new Vector3(0.17f, 0.22f, 0.19f), LeatherColor);
        // Bow
        partWeaponLeft = CreateCubePart("Bow", new Vector3(-0.5f, 1.2f, 0.1f), new Vector3(0.04f, 0.75f, 0.25f), WoodColor).transform;
        CreateCubePart("BowString", new Vector3(-0.48f, 1.2f, 0.22f), new Vector3(0.02f, 0.7f, 0.02f), BoneWhite);
        // Quiver
        CreateCubePart("Quiver", new Vector3(0.15f, 1.3f, -0.2f), new Vector3(0.12f, 0.5f, 0.12f), LeatherColor * 0.9f);
        CreateCubePart("Arrows", new Vector3(0.15f, 1.6f, -0.2f), new Vector3(0.08f, 0.15f, 0.08f), WoodColor);
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.52f, 0.08f, 0.32f), DarkFur);
        // Cape (back)
        CreateCubePart("Cape", new Vector3(0, 1.1f, -0.18f), new Vector3(0.44f, 0.7f, 0.04f), primary * 0.6f);
    }

    void BuildSwordsmanModel(Color primary, Color secondary)
    {
        partBody = CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.58f, 0.9f, 0.35f), ChainmailColor).transform;
        CreateCubePart("Tunic", new Vector3(0, 1.0f, 0), new Vector3(0.6f, 0.5f, 0.37f), primary);
        partHead = CreateCubePart("Head", new Vector3(0, 1.9f, 0), new Vector3(0.42f, 0.44f, 0.42f), SkinColor).transform;
        // Eyes
        CreateCubePart("LeftEye", new Vector3(-0.1f, 1.94f, 0.19f), new Vector3(0.07f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("LeftPupil", new Vector3(-0.1f, 1.94f, 0.21f), new Vector3(0.04f, 0.04f, 0.02f), new Color(0.2f, 0.3f, 0.5f));
        CreateCubePart("RightEye", new Vector3(0.1f, 1.94f, 0.19f), new Vector3(0.07f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("RightPupil", new Vector3(0.1f, 1.94f, 0.21f), new Vector3(0.04f, 0.04f, 0.02f), new Color(0.2f, 0.3f, 0.5f));
        // Helm
        CreateCubePart("Helmet", new Vector3(0, 2.12f, 0), new Vector3(0.48f, 0.2f, 0.48f), IronColor);
        CreateCubePart("HelmetRim", new Vector3(0, 2.02f, 0), new Vector3(0.52f, 0.06f, 0.52f), IronColor * 0.8f);
        CreateCubePart("NasalGuard", new Vector3(0, 1.95f, 0.2f), new Vector3(0.06f, 0.3f, 0.06f), IronColor);
        CreateCubePart("HelmetBand", new Vector3(0, 2.15f, 0), new Vector3(0.5f, 0.04f, 0.04f), GoldAccent);
        // Legs
        partLeftLeg = CreateCubePart("LeftLeg", new Vector3(-0.15f, 0.35f, 0), new Vector3(0.2f, 0.6f, 0.22f), primary * 0.7f).transform;
        partRightLeg = CreateCubePart("RightLeg", new Vector3(0.15f, 0.35f, 0), new Vector3(0.2f, 0.6f, 0.22f), primary * 0.7f).transform;
        CreateCubePart("LeftBoot", new Vector3(-0.15f, 0.08f, 0.02f), new Vector3(0.22f, 0.14f, 0.28f), LeatherColor);
        CreateCubePart("RightBoot", new Vector3(0.15f, 0.08f, 0.02f), new Vector3(0.22f, 0.14f, 0.28f), LeatherColor);
        // Arms
        partLeftArm = CreateCubePart("LeftArm", new Vector3(-0.42f, 1.15f, 0), new Vector3(0.17f, 0.68f, 0.19f), ChainmailColor).transform;
        CreateCubePart("LeftHand", new Vector3(-0.42f, 0.78f, 0), new Vector3(0.14f, 0.14f, 0.14f), SkinColor);
        partRightArm = CreateCubePart("RightArm", new Vector3(0.42f, 1.15f, 0), new Vector3(0.17f, 0.68f, 0.19f), ChainmailColor).transform;
        CreateCubePart("RightHand", new Vector3(0.42f, 0.78f, 0), new Vector3(0.14f, 0.14f, 0.14f), SkinColor);
        // Sword
        partWeapon = CreateCubePart("SwordBlade", new Vector3(0.55f, 1.35f, 0.15f), new Vector3(0.06f, 0.75f, 0.12f), SteelColor).transform;
        CreateCubePart("SwordGuard", new Vector3(0.55f, 0.95f, 0.15f), new Vector3(0.2f, 0.04f, 0.06f), GoldAccent);
        CreateCubePart("SwordGrip", new Vector3(0.55f, 0.85f, 0.15f), new Vector3(0.05f, 0.15f, 0.05f), LeatherColor);
        CreateCubePart("SwordPommel", new Vector3(0.55f, 0.77f, 0.15f), new Vector3(0.08f, 0.06f, 0.08f), GoldAccent);
        // Shield
        partWeaponLeft = CreateCubePart("Shield", new Vector3(-0.55f, 1.1f, 0.12f), new Vector3(0.06f, 0.55f, 0.55f), WoodColor).transform;
        CreateCubePart("ShieldBoss", new Vector3(-0.58f, 1.1f, 0.12f), new Vector3(0.08f, 0.15f, 0.15f), IronColor);
        CreateCubePart("ShieldRim", new Vector3(-0.55f, 1.1f, 0.12f), new Vector3(0.04f, 0.6f, 0.08f), IronColor);
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.6f, 0.07f, 0.37f), LeatherColor);
        CreateCubePart("BeltBuckle", new Vector3(0, 0.72f, 0.18f), new Vector3(0.08f, 0.06f, 0.03f), GoldAccent);
        // Cape
        CreateCubePart("Cape", new Vector3(0, 1.15f, -0.2f), new Vector3(0.52f, 0.75f, 0.04f), primary * 0.7f);
    }

    void BuildBerserkerModel(Color primary, Color secondary)
    {
        partBody = CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.7f, 0.95f, 0.4f), SkinColor * 0.9f).transform;
        CreateCubePart("FurCloak", new Vector3(0, 1.35f, -0.1f), new Vector3(0.75f, 0.55f, 0.35f), FurColor);
        CreateCubePart("FurCollar", new Vector3(0, 1.6f, 0), new Vector3(0.72f, 0.15f, 0.38f), DarkFur);
        CreateCubePart("WarPaint", new Vector3(0, 1.25f, 0.18f), new Vector3(0.4f, 0.1f, 0.04f), DarkRed);
        CreateCubePart("WarPaintX", new Vector3(0, 1.25f, 0.18f), new Vector3(0.1f, 0.35f, 0.04f), DarkRed);
        partHead = CreateCubePart("Head", new Vector3(0, 1.92f, 0), new Vector3(0.45f, 0.46f, 0.44f), SkinColor).transform;
        // Fierce eyes
        CreateCubePart("LeftEye", new Vector3(-0.11f, 1.96f, 0.2f), new Vector3(0.08f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("LeftPupil", new Vector3(-0.11f, 1.96f, 0.22f), new Vector3(0.05f, 0.05f, 0.02f), DarkRed);
        CreateCubePart("RightEye", new Vector3(0.11f, 1.96f, 0.2f), new Vector3(0.08f, 0.05f, 0.04f), BoneWhite);
        CreateCubePart("RightPupil", new Vector3(0.11f, 1.96f, 0.22f), new Vector3(0.05f, 0.05f, 0.02f), DarkRed);
        // Wild hair
        CreateCubePart("Hair", new Vector3(0, 2.1f, -0.05f), new Vector3(0.5f, 0.2f, 0.5f), HairBlond);
        CreateCubePart("HairBack", new Vector3(0, 1.9f, -0.22f), new Vector3(0.35f, 0.4f, 0.1f), HairBlond);
        CreateCubePart("HairWild1", new Vector3(-0.2f, 2.15f, 0.05f), new Vector3(0.12f, 0.15f, 0.12f), HairBlond * 0.9f);
        CreateCubePart("HairWild2", new Vector3(0.18f, 2.18f, -0.05f), new Vector3(0.1f, 0.12f, 0.1f), HairBlond * 0.95f);
        CreateCubePart("Beard", new Vector3(0, 1.72f, 0.18f), new Vector3(0.25f, 0.22f, 0.1f), HairBlond * 0.85f);
        CreateCubePart("BeardBraid", new Vector3(0, 1.58f, 0.2f), new Vector3(0.08f, 0.14f, 0.06f), HairBlond * 0.8f);
        // Legs
        partLeftLeg = CreateCubePart("LeftLeg", new Vector3(-0.17f, 0.35f, 0), new Vector3(0.24f, 0.6f, 0.26f), FurColor).transform;
        partRightLeg = CreateCubePart("RightLeg", new Vector3(0.17f, 0.35f, 0), new Vector3(0.24f, 0.6f, 0.26f), FurColor).transform;
        CreateCubePart("LeftBoot", new Vector3(-0.17f, 0.08f, 0.02f), new Vector3(0.26f, 0.14f, 0.3f), DarkFur);
        CreateCubePart("RightBoot", new Vector3(0.17f, 0.08f, 0.02f), new Vector3(0.26f, 0.14f, 0.3f), DarkFur);
        // Massive arms
        partLeftArm = CreateCubePart("LeftArm", new Vector3(-0.5f, 1.15f, 0), new Vector3(0.2f, 0.72f, 0.22f), SkinColor * 0.9f).transform;
        CreateCubePart("LeftArmband", new Vector3(-0.5f, 1.35f, 0), new Vector3(0.22f, 0.08f, 0.24f), GoldAccent);
        partRightArm = CreateCubePart("RightArm", new Vector3(0.5f, 1.15f, 0), new Vector3(0.2f, 0.72f, 0.22f), SkinColor * 0.9f).transform;
        CreateCubePart("RightArmband", new Vector3(0.5f, 1.35f, 0), new Vector3(0.22f, 0.08f, 0.24f), GoldAccent);
        // Dual axes
        partWeaponLeft = CreateCubePart("LeftAxeHandle", new Vector3(-0.65f, 1.2f, 0.12f), new Vector3(0.05f, 0.7f, 0.05f), WoodColor).transform;
        CreateCubePart("LeftAxeHead", new Vector3(-0.65f, 1.55f, 0.2f), new Vector3(0.04f, 0.25f, 0.3f), IronColor);
        partWeapon = CreateCubePart("RightAxeHandle", new Vector3(0.65f, 1.2f, 0.12f), new Vector3(0.05f, 0.7f, 0.05f), WoodColor).transform;
        CreateCubePart("RightAxeHead", new Vector3(0.65f, 1.55f, 0.2f), new Vector3(0.04f, 0.25f, 0.3f), IronColor);
        // Belt
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.7f, 0.1f, 0.42f), LeatherColor);
        CreateCubePart("BeltSkull", new Vector3(0, 0.72f, 0.2f), new Vector3(0.12f, 0.12f, 0.08f), BoneWhite);
        CreateCubePart("BeltSkullEyes", new Vector3(0, 0.74f, 0.24f), new Vector3(0.08f, 0.04f, 0.02f), new Color(0.1f, 0.1f, 0.1f));
        // Scars on body
        CreateCubePart("Scar1", new Vector3(-0.15f, 1.3f, 0.18f), new Vector3(0.22f, 0.03f, 0.03f), new Color(0.7f, 0.45f, 0.4f));
        CreateCubePart("Scar2", new Vector3(0.1f, 1.1f, 0.18f), new Vector3(0.03f, 0.18f, 0.03f), new Color(0.7f, 0.45f, 0.4f));
    }

    void BuildShieldbearerModel(Color primary, Color secondary)
    {
        partBody = CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.65f, 0.92f, 0.38f), ChainmailColor).transform;
        CreateCubePart("Surcoat", new Vector3(0, 0.95f, 0), new Vector3(0.67f, 0.55f, 0.4f), primary);
        CreateCubePart("SurcoatCross", new Vector3(0, 1.0f, 0.19f), new Vector3(0.12f, 0.35f, 0.03f), GoldAccent);
        CreateCubePart("SurcoatCrossH", new Vector3(0, 1.05f, 0.19f), new Vector3(0.3f, 0.08f, 0.03f), GoldAccent);
        partHead = CreateCubePart("Head", new Vector3(0, 1.9f, 0), new Vector3(0.42f, 0.44f, 0.42f), SkinColor).transform;
        // Eyes behind faceplate
        CreateCubePart("LeftEye", new Vector3(-0.09f, 1.93f, 0.19f), new Vector3(0.06f, 0.04f, 0.04f), BoneWhite);
        CreateCubePart("RightEye", new Vector3(0.09f, 1.93f, 0.19f), new Vector3(0.06f, 0.04f, 0.04f), BoneWhite);
        // Full helm
        CreateCubePart("Helmet", new Vector3(0, 2.12f, 0), new Vector3(0.5f, 0.24f, 0.5f), IronColor);
        CreateCubePart("HelmetRim", new Vector3(0, 2.0f, 0), new Vector3(0.54f, 0.06f, 0.54f), IronColor * 0.8f);
        CreateCubePart("HelmetCrest", new Vector3(0, 2.25f, 0), new Vector3(0.06f, 0.1f, 0.4f), IronColor);
        CreateCubePart("FacePlate", new Vector3(0, 1.92f, 0.2f), new Vector3(0.35f, 0.12f, 0.05f), IronColor * 0.7f);
        // Legs
        partLeftLeg = CreateCubePart("LeftLeg", new Vector3(-0.17f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.24f), ChainmailColor * 0.85f).transform;
        partRightLeg = CreateCubePart("RightLeg", new Vector3(0.17f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.24f), ChainmailColor * 0.85f).transform;
        CreateCubePart("LeftBoot", new Vector3(-0.17f, 0.08f, 0.02f), new Vector3(0.24f, 0.16f, 0.3f), LeatherColor);
        CreateCubePart("RightBoot", new Vector3(0.17f, 0.08f, 0.02f), new Vector3(0.24f, 0.16f, 0.3f), LeatherColor);
        // Arms
        partLeftArm = CreateCubePart("LeftArm", new Vector3(-0.46f, 1.15f, 0), new Vector3(0.18f, 0.68f, 0.2f), ChainmailColor).transform;
        CreateCubePart("LeftShoulder", new Vector3(-0.46f, 1.48f, 0), new Vector3(0.24f, 0.14f, 0.24f), IronColor);
        partRightArm = CreateCubePart("RightArm", new Vector3(0.46f, 1.15f, 0), new Vector3(0.18f, 0.68f, 0.2f), ChainmailColor).transform;
        CreateCubePart("RightShoulder", new Vector3(0.46f, 1.48f, 0), new Vector3(0.24f, 0.14f, 0.24f), IronColor);
        // Large shield
        partWeaponLeft = CreateCubePart("BigShield", new Vector3(-0.6f, 1.05f, 0.15f), new Vector3(0.07f, 0.8f, 0.8f), WoodColor).transform;
        CreateCubePart("ShieldBoss", new Vector3(-0.65f, 1.05f, 0.15f), new Vector3(0.1f, 0.2f, 0.2f), IronColor);
        CreateCubePart("ShieldRimTop", new Vector3(-0.6f, 1.45f, 0.15f), new Vector3(0.05f, 0.04f, 0.82f), IronColor);
        CreateCubePart("ShieldRimBot", new Vector3(-0.6f, 0.65f, 0.15f), new Vector3(0.05f, 0.04f, 0.82f), IronColor);
        CreateCubePart("ShieldEmb", new Vector3(-0.64f, 1.05f, 0.15f), new Vector3(0.04f, 0.25f, 0.04f), primary);
        CreateCubePart("ShieldEmbH", new Vector3(-0.64f, 1.05f, 0.15f), new Vector3(0.04f, 0.04f, 0.25f), primary);
        // Spear
        partWeapon = CreateCubePart("SpearShaft", new Vector3(0.55f, 1.3f, 0.1f), new Vector3(0.05f, 1.4f, 0.05f), WoodColor).transform;
        CreateCubePart("SpearHead", new Vector3(0.55f, 2.05f, 0.1f), new Vector3(0.04f, 0.2f, 0.1f), SteelColor);
        // Belt
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.67f, 0.08f, 0.4f), LeatherColor);
        CreateCubePart("BeltBuckle", new Vector3(0, 0.72f, 0.2f), new Vector3(0.08f, 0.06f, 0.03f), GoldAccent);
        // Cape
        CreateCubePart("Cape", new Vector3(0, 1.15f, -0.22f), new Vector3(0.58f, 0.8f, 0.04f), primary * 0.6f);
    }

    // ========== PART CREATION ==========

    GameObject CreateCubePart(string partName, Vector3 localPos, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(transform);
        part.transform.localPosition = localPos;
        part.transform.localScale = scale;

        Renderer rend = part.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(color);
        rend.material = mat;

        Destroy(part.GetComponent<Collider>());
        part.layer = gameObject.layer;
        return part;
    }

    void CreateSelectionRing()
    {
        selectionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        selectionRing.name = "SelectionRing";
        selectionRing.transform.SetParent(transform);
        selectionRing.transform.localPosition = new Vector3(0, 0.05f, 0);
        selectionRing.transform.localScale = new Vector3(1.4f, 0.02f, 1.4f);

        Renderer rend = selectionRing.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0f, 1f, 0f, 0.5f));
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
        shadow.transform.localScale = new Vector3(0.9f, 0.01f, 0.9f);

        Renderer rend = shadow.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0f, 0f, 0f, 0.25f));
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

        // Check parry
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

        // Hit flash: briefly tint red
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

        // Flash red
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            Color c = origColors[i];
            renderers[i].material.color = Color.Lerp(c, Color.red, 0.5f);
        }

        yield return new WaitForSeconds(0.08f);

        // Restore
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;
            renderers[i].material.color = origColors[i];
        }
    }

    void SpawnParryEffect()
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        effect.name = "ParryFlash";
        effect.transform.position = transform.position + Vector3.up * 1.2f + transform.forward * 0.3f;
        effect.transform.localScale = new Vector3(0.6f, 0.6f, 0.04f);
        effect.transform.rotation = transform.rotation;

        Renderer rend = effect.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(1f, 1f, 0.6f, 0.7f));
        rend.material.renderQueue = 3100;
        Destroy(effect.GetComponent<Collider>());
        Destroy(effect, 0.15f);
    }

    void Die()
    {
        isDead = true;
        FactionManager.Instance?.OnUnitDied(this);

        // Spawn death particles
        SpawnDeathParticles();

        Destroy(gameObject, 1.2f); // Longer delay for death animation
    }

    void SpawnDeathParticles()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            p.name = "DeathDebris";
            p.transform.position = transform.position + Vector3.up * 1f + Random.insideUnitSphere * 0.5f;
            float size = Random.Range(0.05f, 0.12f);
            p.transform.localScale = Vector3.one * size;
            p.transform.rotation = Random.rotation;

            Renderer rend = p.GetComponent<Renderer>();
            Color debrisColor = (faction == Faction.North) ? NorthPrimary : SouthPrimary;
            debrisColor = Color.Lerp(debrisColor, IronColor, Random.Range(0f, 0.5f));
            rend.material = ShaderHelper.CreateMaterial(debrisColor);

            Destroy(p.GetComponent<Collider>());

            Rigidbody rb = p.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.AddForce(Random.insideUnitSphere * 3f + Vector3.up * 2f, ForceMode.Impulse);

            Destroy(p, 1.5f);
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

        rageEffect = CreateCubePart("RageAura", new Vector3(0, 1.2f, 0), new Vector3(1.0f, 2.0f, 1.0f),
            new Color(1f, 0.15f, 0f, 0.15f));
        rageEffect.GetComponent<Renderer>().material.renderQueue = 3100;
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

            shieldWallEffect = CreateCubePart("ShieldWallAura", new Vector3(0, 0.6f, 0.3f),
                new Vector3(0.1f, 1.2f, 1.2f), new Color(0.8f, 0.75f, 0.3f, 0.2f));
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
        // Diamond marker
        markEffect = CreateCubePart("MarkEffect", new Vector3(0, 2.5f, 0),
            new Vector3(0.2f, 0.2f, 0.2f), new Color(1f, 0.3f, 0f, 0.7f));
        markEffect.transform.localRotation = Quaternion.Euler(0, 45, 45);
        markEffect.GetComponent<Renderer>().material.renderQueue = 3100;
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
