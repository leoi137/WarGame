using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitMovement))]
public class UnitCombat : MonoBehaviour
{
    Unit unit;
    UnitMovement movement;
    Unit currentTarget;
    float attackTimer;

    // Swordsman parry
    float parryCooldown = 8f;
    float parryTimer;
    float parryWindow = 1.5f;
    float parryActiveTimer;
    public bool isParrying { get; private set; }

    // Berserker auto-rage threshold
    float rageHealthThreshold = 0.4f;
    bool autoRageTriggered;

    // Archer mark cooldown
    float markCooldown = 6f;
    float markTimer;
    bool hasMarkedTarget;

    void Start()
    {
        unit = GetComponent<Unit>();
        movement = GetComponent<UnitMovement>();
        attackTimer = 0f;
        parryTimer = 0f;
        markTimer = 0f;
    }

    void Update()
    {
        if (unit.isDead) return;

        attackTimer -= Time.deltaTime;
        parryTimer -= Time.deltaTime;
        markTimer -= Time.deltaTime;

        // Swordsman parry timer
        if (isParrying)
        {
            parryActiveTimer -= Time.deltaTime;
            if (parryActiveTimer <= 0f)
            {
                isParrying = false;
            }
        }

        // Berserker auto-rage when low HP
        if (unit.unitType == UnitType.Berserker && !autoRageTriggered && !unit.isEnraged)
        {
            if (unit.currentHealth / unit.maxHealth <= rageHealthThreshold)
            {
                autoRageTriggered = true;
                unit.ActivateRage();
            }
        }

        if (currentTarget != null && currentTarget.isDead)
            currentTarget = null;

        if (currentTarget == null)
        {
            FindNearestEnemy();
        }

        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (dist <= unit.attackRange)
            {
                movement.Stop();
                LookAtTarget(currentTarget.transform);

                if (attackTimer <= 0f)
                {
                    Attack(currentTarget);
                    attackTimer = unit.attackCooldown;
                }
            }
            else
            {
                movement.MoveToTarget(currentTarget.transform, unit.attackRange * 0.9f);
            }
        }
    }

    void FindNearestEnemy()
    {
        float detectionRange = unit.attackRange * 2f;
        if (detectionRange < 10f) detectionRange = 10f;

        // Berserker in rage has increased detection
        if (unit.isEnraged) detectionRange *= 1.5f;

        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        float closestDist = float.MaxValue;
        Unit closest = null;

        foreach (Unit other in allUnits)
        {
            if (other.isDead) continue;
            if (other.faction == unit.faction) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < closestDist && dist <= detectionRange)
            {
                closestDist = dist;
                closest = other;
            }
        }

        currentTarget = closest;
    }

    public void SetTarget(Unit target)
    {
        if (target != null && target.faction != unit.faction)
        {
            currentTarget = target;
        }
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    void Attack(Unit target)
    {
        switch (unit.unitType)
        {
            case UnitType.Archer:
                AttackAsArcher(target);
                break;
            case UnitType.Swordsman:
                AttackAsSwordsman(target);
                break;
            case UnitType.Berserker:
                AttackAsBerserker(target);
                break;
            case UnitType.Shieldbearer:
                AttackAsShieldbearer(target);
                break;
        }
    }

    void AttackAsArcher(Unit target)
    {
        SpawnArrow(target);

        // Mark enemy on cooldown (every Nth shot)
        if (markTimer <= 0f && !target.isMarked)
        {
            target.ApplyMark(5f);
            markTimer = markCooldown;
            hasMarkedTarget = true;
        }
    }

    void AttackAsSwordsman(Unit target)
    {
        // Auto-parry: if cooldown ready, activate before attacking
        if (parryTimer <= 0f && !isParrying)
        {
            ActivateParry();
        }

        float damage = unit.attackDamage;
        target.TakeDamage(damage);
        SpawnSwordSlashEffect(target.transform.position);
    }

    void AttackAsBerserker(Unit target)
    {
        float damage = unit.attackDamage;

        // Dual axe: chance for bonus hit
        target.TakeDamage(damage);
        SpawnAxeHitEffect(target.transform.position);

        // 30% chance second axe swing
        if (Random.value < 0.3f)
        {
            target.TakeDamage(damage * 0.5f);
            SpawnAxeHitEffect(target.transform.position + Vector3.right * 0.3f);
        }

        // Fear: weaker enemies (< 30% HP) move slower temporarily
        if (target.currentHealth / target.maxHealth < 0.3f)
        {
            var targetAgent = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (targetAgent != null)
            {
                targetAgent.speed *= 0.7f;
            }
        }
    }

    void AttackAsShieldbearer(Unit target)
    {
        float damage = unit.attackDamage;
        target.TakeDamage(damage);

        // Shield bash: push enemy back
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        var targetAgent = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (targetAgent != null && targetAgent.isOnNavMesh)
        {
            Vector3 pushTarget = target.transform.position + pushDir * 1.5f;
            targetAgent.Warp(targetAgent.transform.position + pushDir * 0.5f);
        }

        SpawnShieldBashEffect(target.transform.position);
    }

    void ActivateParry()
    {
        isParrying = true;
        parryActiveTimer = parryWindow;
        parryTimer = parryCooldown;
    }

    // Called by external damage system to check parry
    public float GetParryDamageReduction()
    {
        if (isParrying && unit.unitType == UnitType.Swordsman)
        {
            return 0.6f; // Blocks 60% damage during parry window
        }
        return 0f;
    }

    // --- Visual Effects ---

    void SpawnArrow(Unit target)
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.position = transform.position + Vector3.up * 1.5f;

        // Arrow shaft
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shaft.transform.SetParent(arrowObj.transform);
        shaft.transform.localPosition = Vector3.zero;
        shaft.transform.localScale = new Vector3(0.04f, 0.04f, 0.45f);
        Renderer shaftRend = shaft.GetComponent<Renderer>();
        shaftRend.material = ShaderHelper.CreateMaterial(new Color(0.5f, 0.33f, 0.15f));
        Destroy(shaft.GetComponent<Collider>());

        // Arrow head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.transform.SetParent(arrowObj.transform);
        head.transform.localPosition = new Vector3(0, 0, 0.25f);
        head.transform.localScale = new Vector3(0.08f, 0.02f, 0.1f);
        Renderer headRend = head.GetComponent<Renderer>();
        headRend.material = ShaderHelper.CreateMaterial(new Color(0.55f, 0.55f, 0.58f));
        Destroy(head.GetComponent<Collider>());

        // Arrow fletching
        GameObject fletch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fletch.transform.SetParent(arrowObj.transform);
        fletch.transform.localPosition = new Vector3(0, 0, -0.2f);
        fletch.transform.localScale = new Vector3(0.1f, 0.06f, 0.08f);
        Renderer fletchRend = fletch.GetComponent<Renderer>();
        fletchRend.material = ShaderHelper.CreateMaterial(new Color(0.8f, 0.2f, 0.15f));
        Destroy(fletch.GetComponent<Collider>());

        Projectile proj = arrowObj.AddComponent<Projectile>();
        proj.damage = unit.attackDamage;
        proj.target = target;
        proj.speed = 18f;
    }

    void SpawnSwordSlashEffect(Vector3 position)
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        effect.name = "SwordSlash";
        effect.transform.position = position + Vector3.up * 1.1f;
        effect.transform.localScale = new Vector3(0.6f, 0.06f, 0.06f);
        effect.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), Random.Range(-20, 20));
        Renderer rend = effect.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(0.9f, 0.9f, 1f, 0.8f));
        Destroy(effect.GetComponent<Collider>());
        Destroy(effect, 0.15f);
    }

    void SpawnAxeHitEffect(Vector3 position)
    {
        // Red-orange burst
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.name = "AxeHit";
        effect.transform.position = position + Vector3.up;
        effect.transform.localScale = Vector3.one * 0.4f;
        Renderer rend = effect.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(1f, 0.3f, 0f, 0.7f));
        Destroy(effect.GetComponent<Collider>());
        Destroy(effect, 0.2f);

        // Sparks (small cubes)
        for (int i = 0; i < 3; i++)
        {
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spark.name = "Spark";
            spark.transform.position = position + Vector3.up + Random.insideUnitSphere * 0.3f;
            spark.transform.localScale = Vector3.one * 0.08f;
            Renderer sr = spark.GetComponent<Renderer>();
            sr.material = ShaderHelper.CreateMaterial(new Color(1f, 0.7f, 0f));
            Destroy(spark.GetComponent<Collider>());
            Destroy(spark, 0.15f);
        }
    }

    void SpawnShieldBashEffect(Vector3 position)
    {
        // White impact ring
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        effect.name = "ShieldBash";
        effect.transform.position = position + Vector3.up * 0.5f;
        effect.transform.localScale = new Vector3(0.8f, 0.03f, 0.8f);
        Renderer rend = effect.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(0.9f, 0.85f, 0.6f, 0.6f));
        Destroy(effect.GetComponent<Collider>());
        Destroy(effect, 0.25f);
    }

    void SpawnHitEffect(Vector3 position)
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.transform.position = position + Vector3.up;
        effect.transform.localScale = Vector3.one * 0.3f;
        Renderer rend = effect.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(Color.yellow);
        Destroy(effect.GetComponent<Collider>());
        Destroy(effect, 0.2f);
    }

    void LookAtTarget(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public bool HasTarget() => currentTarget != null;
    public Unit GetCurrentTarget() => currentTarget;
}
