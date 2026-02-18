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
            HitTarget();
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

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fraction);
        float arc = arcHeight * Mathf.Sin(fraction * Mathf.PI);
        currentPos.y += arc;

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
            StickInTarget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void StickInTarget()
    {
        // Stick arrow into target for a moment before fading
        transform.SetParent(target.transform);

        // Disable trail
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
            trail.enabled = false;

        // Spawn splinter burst
        SpawnSplinters();

        // Fade and destroy
        StartCoroutine(FadeAndDestroy());
    }

    void SpawnSplinters()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject splinter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            splinter.name = "Splinter";
            splinter.transform.position = transform.position + Random.insideUnitSphere * 0.1f;
            splinter.transform.localScale = new Vector3(
                Random.Range(0.02f, 0.04f),
                Random.Range(0.01f, 0.02f),
                Random.Range(0.04f, 0.08f));
            splinter.transform.rotation = Random.rotation;

            Renderer rend = splinter.GetComponent<Renderer>();
            rend.material = ShaderHelper.WoodMaterial(
                new Color(0.5f + Random.Range(0f, 0.15f), 0.33f, 0.15f));
            Destroy(splinter.GetComponent<Collider>());

            Rigidbody rb = splinter.AddComponent<Rigidbody>();
            rb.mass = 0.005f;
            rb.AddForce(Random.insideUnitSphere * 1.5f + Vector3.up * 1f, ForceMode.Impulse);
            Destroy(splinter, 0.4f);
        }
    }

    System.Collections.IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(0.8f);

        float fadeDuration = 0.4f;
        float timer = 0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
                Color c = r.material.color;
                c.a = alpha;
                r.material.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
