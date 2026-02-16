using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Unit target;
    public float damage = 10f;
    public float speed = 15f;
    public float arcHeight = 2f;

    Vector3 startPos;
    Vector3 targetPos;
    float journeyLength;
    float distanceTraveled;
    bool hasHit;

    void Start()
    {
        startPos = transform.position;

        if (target != null && !target.isDead)
        {
            targetPos = target.transform.position + Vector3.up;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        journeyLength = Vector3.Distance(startPos, targetPos);
        if (journeyLength < 0.1f)
        {
            HitTarget();
        }
    }

    void Update()
    {
        if (hasHit) return;

        if (target == null || target.isDead)
        {
            Destroy(gameObject);
            return;
        }

        targetPos = target.transform.position + Vector3.up;

        distanceTraveled += speed * Time.deltaTime;
        float fraction = distanceTraveled / journeyLength;

        if (fraction >= 1f)
        {
            HitTarget();
            return;
        }

        // Lerp with arc
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fraction);
        float arc = arcHeight * Mathf.Sin(fraction * Mathf.PI);
        currentPos.y += arc;

        // Face direction of travel
        Vector3 dir = (currentPos - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        transform.position = currentPos;
    }

    void HitTarget()
    {
        hasHit = true;

        if (target != null && !target.isDead)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
