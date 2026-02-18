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

    // Trail state
    bool trailsActive;

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

        if (isParrying)
        {
            parryActiveTimer -= Time.deltaTime;
            if (parryActiveTimer <= 0f)
                isParrying = false;
        }

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
            FindNearestEnemy();

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

                // Manage weapon trails during combat
                if (!trailsActive)
                    ActivateWeaponTrails();
            }
            else
            {
                movement.MoveToTarget(currentTarget.transform, unit.attackRange * 0.9f);
                if (trailsActive)
                    DeactivateWeaponTrails();
            }
        }
        else
        {
            if (trailsActive)
                DeactivateWeaponTrails();
        }
    }

    void ActivateWeaponTrails()
    {
        trailsActive = true;
        switch (unit.unitType)
        {
            case UnitType.Swordsman:
                TrailEffect.AttachSwordTrail(unit.weaponTip);
                break;
            case UnitType.Berserker:
                TrailEffect.AttachAxeTrail(unit.weaponTip, unit.isEnraged);
                TrailEffect.AttachAxeTrail(unit.weaponLeftTip, unit.isEnraged);
                break;
            case UnitType.Shieldbearer:
                TrailEffect.AttachSpearTrail(unit.weaponTip);
                break;
        }
    }

    void DeactivateWeaponTrails()
    {
        trailsActive = false;
        TrailEffect.RemoveTrail(unit.weaponTip);
        TrailEffect.RemoveTrail(unit.weaponLeftTip);
    }

    void FindNearestEnemy()
    {
        float detectionRange = unit.attackRange * 2f;
        if (detectionRange < 10f) detectionRange = 10f;

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
            currentTarget = target;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    void Attack(Unit target)
    {
        UnitAnimator animator = GetComponent<UnitAnimator>();
        if (animator != null)
            animator.PlayAttackAnimation();

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

        if (markTimer <= 0f && !target.isMarked)
        {
            target.ApplyMark(5f);
            markTimer = markCooldown;
            hasMarkedTarget = true;
        }
    }

    void AttackAsSwordsman(Unit target)
    {
        if (parryTimer <= 0f && !isParrying)
            ActivateParry();

        float damage = unit.attackDamage;
        target.TakeDamage(damage);
        SpawnSwordSlashEffect(target.transform.position);
    }

    void AttackAsBerserker(Unit target)
    {
        float damage = unit.attackDamage;

        target.TakeDamage(damage);
        SpawnAxeHitEffect(target.transform.position);

        // 30% chance second axe swing
        if (Random.value < 0.3f)
        {
            target.TakeDamage(damage * 0.5f);
            SpawnAxeHitEffect(target.transform.position + Vector3.right * 0.3f);
        }

        // Fear: weaker enemies slow
        if (target.currentHealth / target.maxHealth < 0.3f)
        {
            var targetAgent = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (targetAgent != null)
                targetAgent.speed *= 0.7f;
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
            targetAgent.Warp(targetAgent.transform.position + pushDir * 0.5f);
        }

        SpawnShieldBashEffect(target.transform.position);
        CameraShake.Shake(0.06f, 0.15f);
    }

    void ActivateParry()
    {
        isParrying = true;
        parryActiveTimer = parryWindow;
        parryTimer = parryCooldown;

        // Parry glow on sword
        if (unit.partWeapon != null)
        {
            Renderer r = unit.partWeapon.GetComponent<Renderer>();
            if (r != null)
            {
                ShaderHelper.SetEmission(r.material, new Color(0.8f, 0.85f, 1f) * 1.5f);
                StartCoroutine(ClearParryGlow(r, parryWindow));
            }
        }
    }

    System.Collections.IEnumerator ClearParryGlow(Renderer r, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (r != null)
            ShaderHelper.SetEmission(r.material, Color.black);
    }

    public float GetParryDamageReduction()
    {
        if (isParrying && unit.unitType == UnitType.Swordsman)
            return 0.6f;
        return 0f;
    }

    // ========== VISUAL EFFECTS ==========

    void SpawnArrow(Unit target)
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.position = transform.position + Vector3.up * 1.5f;

        // Arrow shaft
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.transform.SetParent(arrowObj.transform);
        shaft.transform.localPosition = Vector3.zero;
        shaft.transform.localScale = new Vector3(0.025f, 0.2f, 0.025f);
        shaft.transform.localRotation = Quaternion.Euler(90, 0, 0);
        Renderer shaftRend = shaft.GetComponent<Renderer>();
        shaftRend.material = ShaderHelper.WoodMaterial(new Color(0.5f, 0.33f, 0.15f));
        Destroy(shaft.GetComponent<Collider>());

        // Arrow head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.transform.SetParent(arrowObj.transform);
        head.transform.localPosition = new Vector3(0, 0, 0.22f);
        head.transform.localScale = new Vector3(0.06f, 0.015f, 0.08f);
        Renderer headRend = head.GetComponent<Renderer>();
        headRend.material = ShaderHelper.SteelMaterial(new Color(0.55f, 0.55f, 0.58f));
        Destroy(head.GetComponent<Collider>());

        // Fletching
        GameObject fletch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fletch.transform.SetParent(arrowObj.transform);
        fletch.transform.localPosition = new Vector3(0, 0, -0.18f);
        fletch.transform.localScale = new Vector3(0.08f, 0.04f, 0.06f);
        Renderer fletchRend = fletch.GetComponent<Renderer>();
        fletchRend.material = ShaderHelper.CreateMaterial(new Color(0.8f, 0.2f, 0.15f), 0, 0.1f);
        Destroy(fletch.GetComponent<Collider>());

        // Trail on arrow
        TrailEffect.AttachArrowTrail(arrowObj);

        Projectile proj = arrowObj.AddComponent<Projectile>();
        proj.damage = unit.attackDamage;
        proj.target = target;
        proj.speed = 18f;
    }

    void SpawnSwordSlashEffect(Vector3 position)
    {
        Vector3 hitPos = position + Vector3.up * 1.1f;

        // Fan arc of 3 slash lines
        for (int i = 0; i < 3; i++)
        {
            GameObject slash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slash.name = "SlashArc";
            float angle = -30f + i * 30f;
            slash.transform.position = hitPos;
            slash.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, Random.Range(-15, 15));
            slash.transform.localScale = new Vector3(0.5f + i * 0.1f, 0.04f, 0.04f);

            Renderer rend = slash.GetComponent<Renderer>();
            rend.material = ShaderHelper.CreateMaterial(
                new Color(0.9f, 0.92f, 1f, 0.8f), 0, 0.9f,
                new Color(0.7f, 0.75f, 1f) * 1.5f);
            rend.material.renderQueue = 3100;
            Destroy(slash.GetComponent<Collider>());
            Destroy(slash, 0.18f);
        }

        // Sparks
        for (int i = 0; i < 5; i++)
        {
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spark.name = "Spark";
            spark.transform.position = hitPos + Random.insideUnitSphere * 0.15f;
            spark.transform.localScale = Vector3.one * Random.Range(0.03f, 0.06f);
            spark.transform.rotation = Random.rotation;

            Renderer sr = spark.GetComponent<Renderer>();
            sr.material = ShaderHelper.CreateMaterial(
                new Color(1f, 0.95f, 0.7f, 0.9f), 0.5f, 0.8f,
                new Color(1f, 0.9f, 0.5f) * 2f);
            sr.material.renderQueue = 3100;
            Destroy(spark.GetComponent<Collider>());

            Rigidbody rb = spark.AddComponent<Rigidbody>();
            rb.mass = 0.01f;
            rb.useGravity = true;
            rb.AddForce(Random.insideUnitSphere * 3f + Vector3.up * 2f, ForceMode.Impulse);
            Destroy(spark, 0.2f);
        }
    }

    void SpawnAxeHitEffect(Vector3 position)
    {
        Vector3 hitPos = position + Vector3.up;

        // Expanding impact ring
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "AxeRing";
        ring.transform.position = hitPos;
        ring.transform.localScale = new Vector3(0.1f, 0.015f, 0.1f);

        Renderer ringRend = ring.GetComponent<Renderer>();
        Color ringColor = unit.isEnraged ? new Color(1f, 0.2f, 0f, 0.7f) : new Color(1f, 0.4f, 0.1f, 0.7f);
        ringRend.material = ShaderHelper.CreateMaterial(
            ringColor, 0, 0.8f, ringColor * 2f);
        ringRend.material.renderQueue = 3100;
        Destroy(ring.GetComponent<Collider>());

        ExpandAndFade expandRing = ring.AddComponent<ExpandAndFade>();
        expandRing.lifetime = 0.25f;
        expandRing.expandRate = 4f;

        // Ember particles
        int emberCount = unit.isEnraged ? 12 : 8;
        for (int i = 0; i < emberCount; i++)
        {
            bool useSphere = Random.value > 0.5f;
            GameObject ember = GameObject.CreatePrimitive(useSphere ? PrimitiveType.Sphere : PrimitiveType.Cube);
            ember.name = "Ember";
            ember.transform.position = hitPos + Random.insideUnitSphere * 0.2f;
            ember.transform.localScale = Vector3.one * Random.Range(0.03f, 0.07f);
            ember.transform.rotation = Random.rotation;

            Renderer er = ember.GetComponent<Renderer>();
            float rr = Random.Range(0.85f, 1f);
            float gg = Random.Range(0.15f, 0.6f);
            Color ec = new Color(rr, gg, 0f, 0.85f);
            er.material = ShaderHelper.CreateMaterial(ec, 0, 0.8f, ec * 2f);
            er.material.renderQueue = 3100;
            Destroy(ember.GetComponent<Collider>());

            Rigidbody rb = ember.AddComponent<Rigidbody>();
            rb.mass = 0.015f;
            rb.useGravity = true;
            rb.AddForce(Random.insideUnitSphere * 2.5f + Vector3.up * 3f, ForceMode.Impulse);
            Destroy(ember, 0.3f);
        }
    }

    void SpawnShieldBashEffect(Vector3 position)
    {
        Vector3 hitPos = position + Vector3.up * 0.5f;

        // Golden shockwave ring
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "BashWave";
        ring.transform.position = hitPos;
        ring.transform.localScale = new Vector3(0.2f, 0.02f, 0.2f);

        Renderer rend = ring.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(
            new Color(0.9f, 0.85f, 0.4f, 0.7f), 0.3f, 0.8f,
            new Color(0.9f, 0.8f, 0.3f) * 2f);
        rend.material.renderQueue = 3100;
        Destroy(ring.GetComponent<Collider>());

        ExpandAndFade expand = ring.AddComponent<ExpandAndFade>();
        expand.lifetime = 0.3f;
        expand.expandRate = 5f;

        // Dust cloud
        for (int i = 0; i < 6; i++)
        {
            GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dust.name = "BashDust";
            dust.transform.position = hitPos + new Vector3(
                Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.1f), Random.Range(-0.3f, 0.3f));
            dust.transform.localScale = Vector3.one * Random.Range(0.08f, 0.15f);

            Renderer dr = dust.GetComponent<Renderer>();
            dr.material = ShaderHelper.CreateMaterial(
                new Color(0.65f, 0.6f, 0.45f, 0.4f), 0, 0.05f);
            dr.material.renderQueue = 3050;
            Destroy(dust.GetComponent<Collider>());

            ExpandAndFade ef = dust.AddComponent<ExpandAndFade>();
            ef.lifetime = 0.4f;
            ef.expandRate = 2f;
        }
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

public class ExpandAndFade : MonoBehaviour
{
    public float lifetime = 0.3f;
    public float expandRate = 3f;
    float timer;
    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        float scale = 1f + expandRate * t;
        transform.localScale = startScale * scale;

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = Mathf.Lerp(c.a, 0f, t);
            r.material.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
