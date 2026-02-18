using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static CameraShake instance;

    float shakeIntensity;
    float shakeDuration;
    float shakeTimer;
    Vector3 originalLocalPos;
    bool isShaking;
    float seed;

    void Awake()
    {
        instance = this;
    }

    public static void Shake(float intensity, float duration)
    {
        if (instance == null)
        {
            Camera cam = Camera.main;
            if (cam == null) return;
            instance = cam.GetComponent<CameraShake>();
            if (instance == null)
                instance = cam.gameObject.AddComponent<CameraShake>();
        }

        // If already shaking, only override if new shake is stronger
        if (instance.isShaking && intensity < instance.shakeIntensity)
            return;

        instance.shakeIntensity = intensity;
        instance.shakeDuration = duration;
        instance.shakeTimer = 0f;
        instance.seed = Random.Range(0f, 100f);

        if (!instance.isShaking)
        {
            instance.originalLocalPos = instance.transform.localPosition;
            instance.isShaking = true;
        }
    }

    void LateUpdate()
    {
        if (!isShaking) return;

        shakeTimer += Time.deltaTime;
        float t = shakeTimer / shakeDuration;

        if (t >= 1f)
        {
            isShaking = false;
            return;
        }

        // Perlin noise for smooth random shake, with decay envelope
        float decay = 1f - t * t;
        float offsetX = (Mathf.PerlinNoise(seed, shakeTimer * 25f) - 0.5f) * 2f * shakeIntensity * decay;
        float offsetY = (Mathf.PerlinNoise(seed + 100f, shakeTimer * 25f) - 0.5f) * 2f * shakeIntensity * decay;

        transform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0);
    }

    void OnDisable()
    {
        if (isShaking)
        {
            isShaking = false;
        }
    }
}
