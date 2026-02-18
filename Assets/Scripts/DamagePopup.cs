using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    float lifetime = 0.9f;
    float timer;
    float riseSpeed = 1.8f;
    float driftX;
    Vector3 startScale;
    Text text;
    Canvas canvas;

    public static void Spawn(Vector3 worldPos, float damage, bool isMarked, bool armorReduced)
    {
        GameObject obj = new GameObject("DmgPopup");
        obj.transform.position = worldPos + new Vector3(
            Random.Range(-0.3f, 0.3f), Random.Range(0f, 0.2f), 0);

        DamagePopup popup = obj.AddComponent<DamagePopup>();
        popup.Setup(damage, isMarked, armorReduced);
    }

    void Setup(float damage, bool isMarked, bool armorReduced)
    {
        driftX = Random.Range(-0.4f, 0.4f);

        // World-space canvas
        GameObject canvasObj = new GameObject("PopupCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 50;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100, 40);
        canvasRect.localScale = Vector3.one * 0.012f;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(canvasObj.transform, false);

        text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        int displayDamage = Mathf.RoundToInt(damage);
        text.text = displayDamage.ToString();

        // Scale font size with damage amount
        int baseFontSize = Mathf.Clamp(14 + Mathf.RoundToInt(damage * 0.4f), 14, 30);
        text.fontSize = baseFontSize;
        text.fontStyle = damage > 20f ? FontStyle.Bold : FontStyle.Normal;

        // Color by type
        if (isMarked)
            text.color = new Color(1f, 0.5f, 0.1f);
        else if (armorReduced)
            text.color = new Color(0.9f, 0.85f, 0.4f);
        else if (damage > 25f)
            text.color = new Color(1f, 0.25f, 0.2f);
        else
            text.color = Color.white;

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.9f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        startScale = canvasObj.transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        // Rise upward
        transform.position += new Vector3(driftX * Time.deltaTime, riseSpeed * Time.deltaTime, 0);
        riseSpeed *= 0.97f;

        // Billboard
        if (Camera.main != null && canvas != null)
            canvas.transform.rotation = Camera.main.transform.rotation;

        // Scale: pop up quickly, then slowly shrink
        if (canvas != null)
        {
            float scaleMult;
            if (t < 0.15f)
                scaleMult = Mathf.Lerp(0.3f, 1.15f, t / 0.15f);
            else if (t < 0.3f)
                scaleMult = Mathf.Lerp(1.15f, 1f, (t - 0.15f) / 0.15f);
            else
                scaleMult = Mathf.Lerp(1f, 0.5f, (t - 0.3f) / 0.7f);

            canvas.transform.localScale = startScale * scaleMult;
        }

        // Fade
        if (text != null && t > 0.6f)
        {
            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
            text.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
