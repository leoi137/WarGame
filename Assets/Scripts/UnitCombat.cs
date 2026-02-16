using UnityEngine;

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(UnitMovement))]
public class UnitCombat : MonoBehaviour
{
    Unit unit;
    UnitMovement movement;
    Unit currentTarget;
    float attackTimer;

    [Header("Projectile (for Archers)")]
    public static GameObject arrowPrefab;

    void Start()
    {
        unit = GetComponent<Unit>();
        movement = GetComponent<UnitMovement>();
        attackTimer = 0f;
    }

    void Update()
    {
        if (unit.isDead) return;

        attackTimer -= Time.deltaTime;

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
        if (detectionRange < 8f) detectionRange = 8f;

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
        if (unit.unitType == UnitType.Archer)
        {
            SpawnArrow(target);
        }
        else
        {
            target.TakeDamage(unit.attackDamage);
            SpawnHitEffect(target.transform.position);
        }
    }

    void SpawnArrow(Unit target)
    {
        GameObject arrowObj = new GameObject("Arrow");
        arrowObj.transform.position = transform.position + Vector3.up * 1.5f;

        // Visual: small elongated cube
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(arrowObj.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.05f, 0.05f, 0.4f);

        Renderer rend = visual.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(new Color(0.4f, 0.25f, 0.1f));
        rend.material = mat;

        Destroy(visual.GetComponent<Collider>());

        Projectile proj = arrowObj.AddComponent<Projectile>();
        proj.damage = unit.attackDamage;
        proj.target = target;
        proj.speed = 15f;
    }

    void SpawnHitEffect(Vector3 position)
    {
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.transform.position = position + Vector3.up;
        effect.transform.localScale = Vector3.one * 0.3f;

        Renderer rend = effect.GetComponent<Renderer>();
        Material mat = ShaderHelper.CreateMaterial(Color.yellow);
        rend.material = mat;

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
}
