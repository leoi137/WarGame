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
    public bool isEnraged;       // Berserker rage active
    public bool isShieldWalling;  // Shieldbearer wall active
    public bool isMarked;         // Marked by archer (takes bonus damage)
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
    }

    void Update()
    {
        // Berserker rage timer
        if (isEnraged)
        {
            rageTimer -= Time.deltaTime;
            if (rageTimer <= 0f)
            {
                EndRage();
            }
        }

        // Archer mark timer
        if (isMarked)
        {
            markTimer -= Time.deltaTime;
            if (markTimer <= 0f)
            {
                RemoveMark();
            }
        }
    }

    void BuildBlockModel()
    {
        Color primary = faction == Faction.North ? NorthPrimary : SouthPrimary;
        Color secondary = faction == Faction.North ? NorthSecondary : SouthSecondary;

        switch (unitType)
        {
            case UnitType.Archer:    BuildArcherModel(primary, secondary); break;
            case UnitType.Swordsman: BuildSwordsmanModel(primary, secondary); break;
            case UnitType.Berserker: BuildBerserkerModel(primary, secondary); break;
            case UnitType.Shieldbearer: BuildShieldbearerModel(primary, secondary); break;
        }
    }

    void BuildArcherModel(Color primary, Color secondary)
    {
        // Light leather armor - slimmer build
        CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.5f, 0.85f, 0.3f), LeatherColor);
        CreateCubePart("Chest", new Vector3(0, 1.35f, 0), new Vector3(0.48f, 0.25f, 0.28f), DarkFur);
        CreateCubePart("Head", new Vector3(0, 1.88f, 0), new Vector3(0.4f, 0.42f, 0.4f), SkinColor);
        // Hood
        CreateCubePart("Hood", new Vector3(0, 2.0f, -0.05f), new Vector3(0.48f, 0.3f, 0.48f), primary * 0.8f);
        CreateCubePart("HoodBrim", new Vector3(0, 1.85f, 0.15f), new Vector3(0.5f, 0.08f, 0.1f), primary * 0.7f);
        // Legs with wrapped leather
        CreateCubePart("LeftLeg", new Vector3(-0.13f, 0.35f, 0), new Vector3(0.18f, 0.6f, 0.2f), LeatherColor * 0.8f);
        CreateCubePart("RightLeg", new Vector3(0.13f, 0.35f, 0), new Vector3(0.18f, 0.6f, 0.2f), LeatherColor * 0.8f);
        CreateCubePart("LeftBoot", new Vector3(-0.13f, 0.08f, 0.02f), new Vector3(0.2f, 0.14f, 0.26f), DarkFur);
        CreateCubePart("RightBoot", new Vector3(0.13f, 0.08f, 0.02f), new Vector3(0.2f, 0.14f, 0.26f), DarkFur);
        // Arms - exposed forearms
        CreateCubePart("LeftArm", new Vector3(-0.38f, 1.15f, 0), new Vector3(0.15f, 0.65f, 0.17f), SkinColor);
        CreateCubePart("LeftBracer", new Vector3(-0.38f, 0.95f, 0), new Vector3(0.17f, 0.22f, 0.19f), LeatherColor);
        CreateCubePart("RightArm", new Vector3(0.38f, 1.15f, 0), new Vector3(0.15f, 0.65f, 0.17f), SkinColor);
        CreateCubePart("RightBracer", new Vector3(0.38f, 0.95f, 0), new Vector3(0.17f, 0.22f, 0.19f), LeatherColor);
        // Bow (held in left hand)
        CreateCubePart("Bow", new Vector3(-0.5f, 1.2f, 0.1f), new Vector3(0.04f, 0.75f, 0.25f), WoodColor);
        CreateCubePart("BowString", new Vector3(-0.48f, 1.2f, 0.22f), new Vector3(0.02f, 0.7f, 0.02f), BoneWhite);
        // Quiver on back
        CreateCubePart("Quiver", new Vector3(0.15f, 1.3f, -0.2f), new Vector3(0.12f, 0.5f, 0.12f), LeatherColor * 0.9f);
        CreateCubePart("Arrows", new Vector3(0.15f, 1.6f, -0.2f), new Vector3(0.08f, 0.15f, 0.08f), WoodColor);
        // Belt
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.52f, 0.08f, 0.32f), DarkFur);
    }

    void BuildSwordsmanModel(Color primary, Color secondary)
    {
        // Chainmail + tunic - medium build
        CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.58f, 0.9f, 0.35f), ChainmailColor);
        CreateCubePart("Tunic", new Vector3(0, 1.0f, 0), new Vector3(0.6f, 0.5f, 0.37f), primary);
        CreateCubePart("Head", new Vector3(0, 1.9f, 0), new Vector3(0.42f, 0.44f, 0.42f), SkinColor);
        // Viking helm with nasal guard
        CreateCubePart("Helmet", new Vector3(0, 2.12f, 0), new Vector3(0.48f, 0.2f, 0.48f), IronColor);
        CreateCubePart("HelmetRim", new Vector3(0, 2.02f, 0), new Vector3(0.52f, 0.06f, 0.52f), IronColor * 0.8f);
        CreateCubePart("NasalGuard", new Vector3(0, 1.95f, 0.2f), new Vector3(0.06f, 0.3f, 0.06f), IronColor);
        CreateCubePart("HelmetBand", new Vector3(0, 2.15f, 0), new Vector3(0.5f, 0.04f, 0.04f), GoldAccent);
        // Legs
        CreateCubePart("LeftLeg", new Vector3(-0.15f, 0.35f, 0), new Vector3(0.2f, 0.6f, 0.22f), primary * 0.7f);
        CreateCubePart("RightLeg", new Vector3(0.15f, 0.35f, 0), new Vector3(0.2f, 0.6f, 0.22f), primary * 0.7f);
        CreateCubePart("LeftBoot", new Vector3(-0.15f, 0.08f, 0.02f), new Vector3(0.22f, 0.14f, 0.28f), LeatherColor);
        CreateCubePart("RightBoot", new Vector3(0.15f, 0.08f, 0.02f), new Vector3(0.22f, 0.14f, 0.28f), LeatherColor);
        // Arms with mail sleeves
        CreateCubePart("LeftArm", new Vector3(-0.42f, 1.15f, 0), new Vector3(0.17f, 0.68f, 0.19f), ChainmailColor);
        CreateCubePart("LeftHand", new Vector3(-0.42f, 0.78f, 0), new Vector3(0.14f, 0.14f, 0.14f), SkinColor);
        CreateCubePart("RightArm", new Vector3(0.42f, 1.15f, 0), new Vector3(0.17f, 0.68f, 0.19f), ChainmailColor);
        CreateCubePart("RightHand", new Vector3(0.42f, 0.78f, 0), new Vector3(0.14f, 0.14f, 0.14f), SkinColor);
        // Sword (right hand)
        CreateCubePart("SwordBlade", new Vector3(0.55f, 1.35f, 0.15f), new Vector3(0.06f, 0.75f, 0.12f), SteelColor);
        CreateCubePart("SwordGuard", new Vector3(0.55f, 0.95f, 0.15f), new Vector3(0.2f, 0.04f, 0.06f), GoldAccent);
        CreateCubePart("SwordGrip", new Vector3(0.55f, 0.85f, 0.15f), new Vector3(0.05f, 0.15f, 0.05f), LeatherColor);
        // Round shield (left hand)
        CreateCubePart("Shield", new Vector3(-0.55f, 1.1f, 0.12f), new Vector3(0.06f, 0.55f, 0.55f), WoodColor);
        CreateCubePart("ShieldBoss", new Vector3(-0.58f, 1.1f, 0.12f), new Vector3(0.08f, 0.15f, 0.15f), IronColor);
        CreateCubePart("ShieldRim", new Vector3(-0.55f, 1.1f, 0.12f), new Vector3(0.04f, 0.6f, 0.08f), IronColor);
        // Belt + scabbard
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.6f, 0.07f, 0.37f), LeatherColor);
    }

    void BuildBerserkerModel(Color primary, Color secondary)
    {
        // Bare-chested with fur, massive build
        CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.7f, 0.95f, 0.4f), SkinColor * 0.9f);
        CreateCubePart("FurCloak", new Vector3(0, 1.35f, -0.1f), new Vector3(0.75f, 0.55f, 0.35f), FurColor);
        CreateCubePart("FurCollar", new Vector3(0, 1.6f, 0), new Vector3(0.72f, 0.15f, 0.38f), DarkFur);
        CreateCubePart("WarPaint", new Vector3(0, 1.25f, 0.18f), new Vector3(0.4f, 0.1f, 0.04f), DarkRed);
        CreateCubePart("Head", new Vector3(0, 1.92f, 0), new Vector3(0.45f, 0.46f, 0.44f), SkinColor);
        // Wild hair + no helmet
        CreateCubePart("Hair", new Vector3(0, 2.1f, -0.05f), new Vector3(0.5f, 0.2f, 0.5f), HairBlond);
        CreateCubePart("HairBack", new Vector3(0, 1.9f, -0.22f), new Vector3(0.35f, 0.4f, 0.1f), HairBlond);
        CreateCubePart("Beard", new Vector3(0, 1.72f, 0.18f), new Vector3(0.25f, 0.22f, 0.1f), HairBlond * 0.85f);
        // Massive legs
        CreateCubePart("LeftLeg", new Vector3(-0.17f, 0.35f, 0), new Vector3(0.24f, 0.6f, 0.26f), FurColor);
        CreateCubePart("RightLeg", new Vector3(0.17f, 0.35f, 0), new Vector3(0.24f, 0.6f, 0.26f), FurColor);
        CreateCubePart("LeftBoot", new Vector3(-0.17f, 0.08f, 0.02f), new Vector3(0.26f, 0.14f, 0.3f), DarkFur);
        CreateCubePart("RightBoot", new Vector3(0.17f, 0.08f, 0.02f), new Vector3(0.26f, 0.14f, 0.3f), DarkFur);
        // Thick bare arms with armbands
        CreateCubePart("LeftArm", new Vector3(-0.5f, 1.15f, 0), new Vector3(0.2f, 0.72f, 0.22f), SkinColor * 0.9f);
        CreateCubePart("LeftArmband", new Vector3(-0.5f, 1.35f, 0), new Vector3(0.22f, 0.08f, 0.24f), GoldAccent);
        CreateCubePart("RightArm", new Vector3(0.5f, 1.15f, 0), new Vector3(0.2f, 0.72f, 0.22f), SkinColor * 0.9f);
        CreateCubePart("RightArmband", new Vector3(0.5f, 1.35f, 0), new Vector3(0.22f, 0.08f, 0.24f), GoldAccent);
        // Dual axes
        CreateCubePart("LeftAxeHandle", new Vector3(-0.65f, 1.2f, 0.12f), new Vector3(0.05f, 0.7f, 0.05f), WoodColor);
        CreateCubePart("LeftAxeHead", new Vector3(-0.65f, 1.55f, 0.2f), new Vector3(0.04f, 0.25f, 0.3f), IronColor);
        CreateCubePart("RightAxeHandle", new Vector3(0.65f, 1.2f, 0.12f), new Vector3(0.05f, 0.7f, 0.05f), WoodColor);
        CreateCubePart("RightAxeHead", new Vector3(0.65f, 1.55f, 0.2f), new Vector3(0.04f, 0.25f, 0.3f), IronColor);
        // Belt with skull
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.7f, 0.1f, 0.42f), LeatherColor);
        CreateCubePart("BeltSkull", new Vector3(0, 0.72f, 0.2f), new Vector3(0.12f, 0.12f, 0.08f), BoneWhite);
    }

    void BuildShieldbearerModel(Color primary, Color secondary)
    {
        // Heavily armored, wide stance
        CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.65f, 0.92f, 0.38f), ChainmailColor);
        CreateCubePart("Surcoat", new Vector3(0, 0.95f, 0), new Vector3(0.67f, 0.55f, 0.4f), primary);
        CreateCubePart("SurcoatCross", new Vector3(0, 1.0f, 0.19f), new Vector3(0.12f, 0.35f, 0.03f), GoldAccent);
        CreateCubePart("SurcoatCrossH", new Vector3(0, 1.05f, 0.19f), new Vector3(0.3f, 0.08f, 0.03f), GoldAccent);
        CreateCubePart("Head", new Vector3(0, 1.9f, 0), new Vector3(0.42f, 0.44f, 0.42f), SkinColor);
        // Full Viking helm (spectacle helm style)
        CreateCubePart("Helmet", new Vector3(0, 2.12f, 0), new Vector3(0.5f, 0.24f, 0.5f), IronColor);
        CreateCubePart("HelmetRim", new Vector3(0, 2.0f, 0), new Vector3(0.54f, 0.06f, 0.54f), IronColor * 0.8f);
        CreateCubePart("HelmetCrest", new Vector3(0, 2.25f, 0), new Vector3(0.06f, 0.1f, 0.4f), IronColor);
        CreateCubePart("FacePlate", new Vector3(0, 1.92f, 0.2f), new Vector3(0.35f, 0.12f, 0.05f), IronColor * 0.7f);
        // Stout legs
        CreateCubePart("LeftLeg", new Vector3(-0.17f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.24f), ChainmailColor * 0.85f);
        CreateCubePart("RightLeg", new Vector3(0.17f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.24f), ChainmailColor * 0.85f);
        CreateCubePart("LeftBoot", new Vector3(-0.17f, 0.08f, 0.02f), new Vector3(0.24f, 0.16f, 0.3f), LeatherColor);
        CreateCubePart("RightBoot", new Vector3(0.17f, 0.08f, 0.02f), new Vector3(0.24f, 0.16f, 0.3f), LeatherColor);
        // Armored arms
        CreateCubePart("LeftArm", new Vector3(-0.46f, 1.15f, 0), new Vector3(0.18f, 0.68f, 0.2f), ChainmailColor);
        CreateCubePart("LeftShoulder", new Vector3(-0.46f, 1.48f, 0), new Vector3(0.24f, 0.14f, 0.24f), IronColor);
        CreateCubePart("RightArm", new Vector3(0.46f, 1.15f, 0), new Vector3(0.18f, 0.68f, 0.2f), ChainmailColor);
        CreateCubePart("RightShoulder", new Vector3(0.46f, 1.48f, 0), new Vector3(0.24f, 0.14f, 0.24f), IronColor);
        // LARGE round shield (left hand) -- the defining feature
        CreateCubePart("BigShield", new Vector3(-0.6f, 1.05f, 0.15f), new Vector3(0.07f, 0.8f, 0.8f), WoodColor);
        CreateCubePart("ShieldBoss", new Vector3(-0.65f, 1.05f, 0.15f), new Vector3(0.1f, 0.2f, 0.2f), IronColor);
        CreateCubePart("ShieldRimTop", new Vector3(-0.6f, 1.45f, 0.15f), new Vector3(0.05f, 0.04f, 0.82f), IronColor);
        CreateCubePart("ShieldRimBot", new Vector3(-0.6f, 0.65f, 0.15f), new Vector3(0.05f, 0.04f, 0.82f), IronColor);
        CreateCubePart("ShieldEmb", new Vector3(-0.64f, 1.05f, 0.15f), new Vector3(0.04f, 0.25f, 0.04f), primary);
        CreateCubePart("ShieldEmbH", new Vector3(-0.64f, 1.05f, 0.15f), new Vector3(0.04f, 0.04f, 0.25f), primary);
        // Spear (right hand)
        CreateCubePart("SpearShaft", new Vector3(0.55f, 1.3f, 0.1f), new Vector3(0.05f, 1.4f, 0.05f), WoodColor);
        CreateCubePart("SpearHead", new Vector3(0.55f, 2.05f, 0.1f), new Vector3(0.04f, 0.2f, 0.1f), SteelColor);
        // Belt
        CreateCubePart("Belt", new Vector3(0, 0.72f, 0), new Vector3(0.67f, 0.08f, 0.4f), LeatherColor);
    }

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

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionRing != null)
            selectionRing.SetActive(selected);
    }

    // --- Damage system with armor and mark ---
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float totalArmor = armor;
        if (isShieldWalling) totalArmor += shieldWallArmor;

        float finalDamage = Mathf.Max(damage - totalArmor, 1f);

        // Marked targets take 40% bonus damage
        if (isMarked) finalDamage *= 1.4f;

        currentHealth -= finalDamage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        FactionManager.Instance?.OnUnitDied(this);
        Destroy(gameObject, 0.5f);
    }

    // --- Berserker Rage ---
    public void ActivateRage()
    {
        if (unitType != UnitType.Berserker || isEnraged) return;

        isEnraged = true;
        rageTimer = rageDuration;

        // Boost damage +60%, speed +30%, but lose 40% armor
        attackDamage *= 1.6f;
        moveSpeed *= 1.3f;
        armor *= 0.6f;

        // Update NavMeshAgent speed
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;

        // Visual: red glow effect
        rageEffect = CreateCubePart("RageAura", new Vector3(0, 1.2f, 0), new Vector3(1.0f, 2.0f, 1.0f),
            new Color(1f, 0.15f, 0f, 0.2f));
        rageEffect.GetComponent<Renderer>().material.renderQueue = 3100;

        Debug.Log($"WorldWars: {gameObject.name} enters BERSERKER RAGE!");
    }

    void EndRage()
    {
        isEnraged = false;
        // Recalculate base stats
        ApplyStats();

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;

        if (rageEffect != null) Destroy(rageEffect);
    }

    // --- Shieldbearer Shield Wall ---
    public void ActivateShieldWall()
    {
        if (unitType != UnitType.Shieldbearer) return;

        isShieldWalling = !isShieldWalling; // Toggle

        if (isShieldWalling)
        {
            moveSpeed *= 0.3f; // Nearly immobile
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            shieldWallEffect = CreateCubePart("ShieldWallAura", new Vector3(0, 0.6f, 0.3f),
                new Vector3(0.1f, 1.2f, 1.2f), new Color(0.8f, 0.75f, 0.3f, 0.25f));
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

    // --- Archer Mark ---
    public void ApplyMark(float duration)
    {
        isMarked = true;
        markTimer = duration;

        if (markEffect != null) Destroy(markEffect);
        markEffect = CreateCubePart("MarkEffect", new Vector3(0, 2.5f, 0),
            new Vector3(0.3f, 0.3f, 0.3f), new Color(1f, 0.3f, 0f, 0.6f));
        markEffect.GetComponent<Renderer>().material.renderQueue = 3100;
    }

    void RemoveMark()
    {
        isMarked = false;
        if (markEffect != null) Destroy(markEffect);
    }

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
