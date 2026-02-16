using UnityEngine;
using UnityEngine.AI;

public enum Faction { North, South }
public enum UnitType { Swordsman, Archer }

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

    [Header("State")]
    public bool isSelected;
    public bool isDead;

    [Header("Runtime References")]
    public GameObject selectionRing;

    // Colors
    static readonly Color NorthColor = new Color(0.2f, 0.4f, 0.8f);    // Blue
    static readonly Color SouthColor = new Color(0.8f, 0.2f, 0.2f);    // Red
    static readonly Color SwordColor = new Color(0.67f, 0.67f, 0.67f); // Silver
    static readonly Color BowColor = new Color(0.545f, 0.271f, 0.075f);// Brown
    static readonly Color SkinColor = new Color(0.87f, 0.72f, 0.53f);  // Skin tone

    bool initialized;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Call after setting faction and unitType to build the visual model.
    /// </summary>
    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        ApplyStats();
        BuildBlockModel();
        CreateSelectionRing();
    }

    void BuildBlockModel()
    {
        Color factionColor = faction == Faction.North ? NorthColor : SouthColor;

        // Body (torso)
        CreateCubePart("Body", new Vector3(0, 1.15f, 0), new Vector3(0.6f, 0.9f, 0.35f), factionColor);

        // Head
        CreateCubePart("Head", new Vector3(0, 1.9f, 0), new Vector3(0.45f, 0.45f, 0.45f), SkinColor);

        // Left Leg
        CreateCubePart("LeftLeg", new Vector3(-0.15f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.25f), factionColor * 0.7f);

        // Right Leg
        CreateCubePart("RightLeg", new Vector3(0.15f, 0.35f, 0), new Vector3(0.22f, 0.6f, 0.25f), factionColor * 0.7f);

        // Left Arm
        CreateCubePart("LeftArm", new Vector3(-0.45f, 1.15f, 0), new Vector3(0.18f, 0.7f, 0.2f), SkinColor);

        // Right Arm
        CreateCubePart("RightArm", new Vector3(0.45f, 1.15f, 0), new Vector3(0.18f, 0.7f, 0.2f), SkinColor);

        // Weapon
        if (unitType == UnitType.Swordsman)
        {
            CreateCubePart("Sword", new Vector3(0.55f, 1.4f, 0.15f), new Vector3(0.08f, 0.8f, 0.15f), SwordColor);
        }
        else
        {
            CreateCubePart("Bow", new Vector3(0.55f, 1.3f, 0.1f), new Vector3(0.04f, 0.65f, 0.22f), BowColor);
        }

        // Helmet (small block on head)
        Color helmetColor = faction == Faction.North ? new Color(0.3f, 0.3f, 0.6f) : new Color(0.6f, 0.15f, 0.15f);
        CreateCubePart("Helmet", new Vector3(0, 2.15f, 0), new Vector3(0.5f, 0.12f, 0.5f), helmetColor);
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

        // Remove collider from body parts (unit has its own collider)
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
        selectionRing.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);

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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
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

    public void ApplyStats()
    {
        if (unitType == UnitType.Swordsman)
        {
            maxHealth = 100f;
            attackDamage = 15f;
            attackRange = 2.5f;
            attackCooldown = 1.0f;
            moveSpeed = 3.5f;
        }
        else
        {
            maxHealth = 60f;
            attackDamage = 10f;
            attackRange = 12f;
            attackCooldown = 1.5f;
            moveSpeed = 4.0f;
        }
        currentHealth = maxHealth;
    }
}
