using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour
{
    Unit unit;
    Canvas canvas;
    Image backgroundImage;
    Image fillImage;
    Image armorImage;
    RectTransform fillRect;
    RectTransform armorRect;
    Text nameText;

    float barWidth = 90f;
    float barHeight = 8f;
    float yOffset = 2.7f;

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
        canvasRect.sizeDelta = new Vector2(barWidth, barHeight + 14f);
        canvasRect.localScale = Vector3.one * 0.01f;

        // Unit type name
        GameObject nameObj = new GameObject("UnitName");
        nameObj.transform.SetParent(canvasObj.transform, false);
        nameText = nameObj.AddComponent<Text>();
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.fontSize = 10;
        nameText.color = GetUnitTypeColor();
        nameText.text = GetShortName();

        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 1);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(0.5f, 1);
        nameRect.offsetMin = new Vector2(0, -13);
        nameRect.offsetMax = new Vector2(0, 0);

        Outline nameOutline = nameObj.AddComponent<Outline>();
        nameOutline.effectColor = new Color(0, 0, 0, 0.9f);
        nameOutline.effectDistance = new Vector2(0.8f, -0.8f);

        // Health bar container (positioned below name)
        GameObject barContainer = new GameObject("BarContainer");
        barContainer.transform.SetParent(canvasObj.transform, false);
        RectTransform barContainerRect = barContainer.AddComponent<RectTransform>();
        barContainerRect.anchorMin = new Vector2(0, 0);
        barContainerRect.anchorMax = new Vector2(1, 0);
        barContainerRect.pivot = new Vector2(0.5f, 0);
        barContainerRect.offsetMin = Vector2.zero;
        barContainerRect.offsetMax = new Vector2(0, barHeight);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(barContainer.transform, false);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(barContainer.transform, false);
        fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;
        fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.offsetMin = new Vector2(1, 1);
        fillRect.offsetMax = new Vector2(-1, -1);

        // Armor indicator (thin bar below health)
        if (unit.armor > 0 || unit.unitType == UnitType.Shieldbearer)
        {
            GameObject armorObj = new GameObject("Armor");
            armorObj.transform.SetParent(barContainer.transform, false);
            armorImage = armorObj.AddComponent<Image>();
            armorImage.color = new Color(0.6f, 0.6f, 0.7f, 0.6f);
            armorRect = armorObj.GetComponent<RectTransform>();
            armorRect.anchorMin = new Vector2(0, 0);
            armorRect.anchorMax = new Vector2(0, 0);
            armorRect.pivot = new Vector2(0, 0);
            armorRect.offsetMin = new Vector2(1, -3);
            armorRect.offsetMax = new Vector2(barWidth * 0.5f, 0);
        }
    }

    string GetShortName()
    {
        switch (unit.unitType)
        {
            case UnitType.Swordsman: return "Huscarl";
            case UnitType.Archer: return "Hunter";
            case UnitType.Berserker: return "Berserker";
            case UnitType.Shieldbearer: return "Shield";
            default: return unit.unitType.ToString();
        }
    }

    Color GetUnitTypeColor()
    {
        if (unit.faction == Faction.North)
        {
            switch (unit.unitType)
            {
                case UnitType.Swordsman: return new Color(0.4f, 0.6f, 1f);
                case UnitType.Archer: return new Color(0.3f, 0.8f, 0.5f);
                case UnitType.Berserker: return new Color(1f, 0.6f, 0.2f);
                case UnitType.Shieldbearer: return new Color(0.7f, 0.7f, 0.9f);
            }
        }
        else
        {
            switch (unit.unitType)
            {
                case UnitType.Swordsman: return new Color(1f, 0.5f, 0.5f);
                case UnitType.Archer: return new Color(0.9f, 0.6f, 0.4f);
                case UnitType.Berserker: return new Color(1f, 0.3f, 0.3f);
                case UnitType.Shieldbearer: return new Color(0.9f, 0.6f, 0.6f);
            }
        }
        return Color.white;
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

        // Update name color on status effects
        if (nameText != null)
        {
            if (unit.isEnraged)
                nameText.color = new Color(1f, 0.2f, 0f); // Rage red
            else if (unit.isShieldWalling)
                nameText.color = new Color(0.9f, 0.85f, 0.4f); // Gold
            else
                nameText.color = GetUnitTypeColor();
        }

        // Armor bar
        if (armorImage != null)
        {
            float totalArmor = unit.armor + (unit.isShieldWalling ? unit.shieldWallArmor : 0);
            float armorRatio = Mathf.Clamp01(totalArmor / 25f);
            armorRect.offsetMax = new Vector2(1 + (barWidth - 2) * armorRatio, 0);

            armorImage.color = unit.isShieldWalling
                ? new Color(0.85f, 0.75f, 0.3f, 0.7f)
                : new Color(0.6f, 0.6f, 0.7f, 0.6f);
        }
    }
}
