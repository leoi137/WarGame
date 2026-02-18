using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public static void AttachTrail(Transform tip, Color startColor, Color endColor,
        float width = 0.25f, float duration = 0.12f)
    {
        if (tip == null) return;

        TrailRenderer existing = tip.GetComponent<TrailRenderer>();
        if (existing != null) return;

        TrailRenderer trail = tip.gameObject.AddComponent<TrailRenderer>();
        trail.time = duration;
        trail.startWidth = width;
        trail.endWidth = 0f;
        trail.minVertexDistance = 0.02f;
        trail.numCapVertices = 2;
        trail.numCornerVertices = 2;

        // Use additive unlit material
        trail.material = ShaderHelper.CreateAdditiveMaterial(Color.white);

        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = grad;

        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
    }

    public static void RemoveTrail(Transform tip)
    {
        if (tip == null) return;
        TrailRenderer trail = tip.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.time = 0f;
            Object.Destroy(trail, 0.2f);
        }
    }

    public static void AttachSwordTrail(Transform weaponTip)
    {
        AttachTrail(weaponTip,
            new Color(0.9f, 0.95f, 1f), new Color(0.5f, 0.6f, 0.8f),
            0.25f, 0.12f);
    }

    public static void AttachAxeTrail(Transform axeTip, bool isRaging)
    {
        Color start = isRaging ? new Color(1f, 0.4f, 0f) : new Color(0.9f, 0.5f, 0.2f);
        Color end = isRaging ? new Color(1f, 0.1f, 0f) : new Color(0.5f, 0.3f, 0.1f);
        float width = isRaging ? 0.3f : 0.2f;
        AttachTrail(axeTip, start, end, width, 0.15f);
    }

    public static void AttachSpearTrail(Transform spearTip)
    {
        AttachTrail(spearTip,
            new Color(0.95f, 0.95f, 0.95f), new Color(0.6f, 0.6f, 0.7f),
            0.12f, 0.08f);
    }

    public static void AttachArrowTrail(GameObject arrowObj)
    {
        if (arrowObj == null) return;

        TrailRenderer trail = arrowObj.AddComponent<TrailRenderer>();
        trail.time = 0.15f;
        trail.startWidth = 0.04f;
        trail.endWidth = 0f;
        trail.minVertexDistance = 0.05f;

        trail.material = ShaderHelper.CreateAdditiveMaterial(Color.white);

        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.9f, 0.85f, 0.7f), 0f),
                new GradientColorKey(new Color(0.5f, 0.4f, 0.3f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.6f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = grad;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
