using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour
{
    Unit unit;
    Canvas canvas;
    Image backgroundImage;
    Image fillImage;
    RectTransform fillRect;

    float barWidth = 80f;
    float barHeight = 8f;
    float yOffset = 2.6f;

    void Start()
    {
        unit = GetComponent<Unit>();
        CreateHealthBarUI();
    }

    void CreateHealthBarUI()
    {
        // World-space canvas
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, yOffset, 0);

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(barWidth, barHeight);
        canvasRect.localScale = Vector3.one * 0.01f;

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);

        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(canvasObj.transform, false);

        fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;

        fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.offsetMin = new Vector2(1, 1);
        fillRect.offsetMax = new Vector2(-1, -1);
    }

    void LateUpdate()
    {
        if (unit == null || unit.isDead)
        {
            if (canvas != null) canvas.gameObject.SetActive(false);
            return;
        }

        // Billboard: face camera
        if (Camera.main != null && canvas != null)
        {
            canvas.transform.rotation = Camera.main.transform.rotation;
        }

        // Update fill
        float ratio = unit.currentHealth / unit.maxHealth;
        if (fillRect != null)
        {
            fillRect.anchorMax = new Vector2(ratio, 1f);
        }

        // Color gradient: green -> yellow -> red
        if (fillImage != null)
        {
            if (ratio > 0.5f)
                fillImage.color = Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2f);
            else
                fillImage.color = Color.Lerp(Color.red, Color.yellow, ratio * 2f);
        }
    }
}
