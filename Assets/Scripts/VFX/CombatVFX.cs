using UnityEngine;

/// <summary>
/// Procedural combat visual effects for diverse weapon types.
/// All particles are built from primitives — no prefabs.
/// </summary>
public static class CombatVFX
{
    const float DefaultLifetime = 0.8f;

    public static void SpawnClubImpact(Vector3 position)
    {
        SpawnDustBurst(position, new Color(0.6f, 0.5f, 0.35f), 0.8f, 6);
    }

    public static void SpawnElephantCharge(Vector3 position)
    {
        SpawnDustBurst(position, new Color(0.55f, 0.45f, 0.3f), 2.5f, 12);
        SpawnGroundCrack(position);
    }

    public static void SpawnSlingImpact(Vector3 position)
    {
        SpawnDustBurst(position, new Color(0.5f, 0.5f, 0.5f), 0.3f, 3);
    }

    public static void SpawnJavelinStick(Vector3 position, Vector3 direction)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "JavelinStuck";
        go.transform.position = position;
        go.transform.localScale = new Vector3(0.04f, 0.6f, 0.04f);
        go.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

        var col = go.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        var rend = go.GetComponent<Renderer>();
        rend.material = ShaderHelper.WoodMaterial(new Color(0.45f, 0.3f, 0.15f));

        Object.Destroy(go, 4f);
    }

    public static void SpawnFireLanceBlast(Vector3 position)
    {
        var flash = CreateParticle(position, new Color(1f, 0.7f, 0.2f, 0.9f), 0.6f);
        var rend = flash.GetComponent<Renderer>();
        if (rend != null) rend.material = ShaderHelper.CreateAdditiveMaterial(new Color(1f, 0.8f, 0.3f));
        Object.Destroy(flash, 0.4f);

        SpawnDustBurst(position, new Color(0.4f, 0.4f, 0.45f), 1.2f, 5);
    }

    public static void SpawnAtlatlImpact(Vector3 position)
    {
        SpawnDustBurst(position, new Color(0.5f, 0.45f, 0.3f), 0.4f, 3);
    }

    static void SpawnDustBurst(Vector3 origin, Color color, float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = Random.insideUnitSphere * radius;
            offset.y = Mathf.Abs(offset.y) * 0.5f;
            var p = CreateParticle(origin + offset, color, Random.Range(0.15f, 0.35f));
            Object.Destroy(p, Random.Range(0.5f, DefaultLifetime));
        }
    }

    static void SpawnGroundCrack(Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = "GroundCrack";
        go.transform.position = position + Vector3.up * 0.02f;
        go.transform.localScale = new Vector3(0.3f, 1f, 0.3f);

        var col = go.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        var rend = go.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(new Color(0.2f, 0.15f, 0.1f, 0.7f));

        Object.Destroy(go, 3f);
    }

    static GameObject CreateParticle(Vector3 position, Color color, float size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "VFXParticle";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * size;

        var col = go.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        var rend = go.GetComponent<Renderer>();
        rend.material = ShaderHelper.CreateMaterial(color, 0f, 0.05f);

        return go;
    }
}
