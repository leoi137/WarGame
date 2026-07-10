using UnityEngine;

public static class FactionColorPalette
{
    public static Color GetPrimaryColor(string factionId)
    {
        var faction = FactionDatabase.Get(factionId);
        return faction != null ? faction.primaryColor : new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public static Color GetSecondaryColor(string factionId)
    {
        var faction = FactionDatabase.Get(factionId);
        return faction != null ? faction.secondaryColor : new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public static Color GetUIColor(string factionId)
    {
        var c = GetPrimaryColor(factionId);
        return Brighten(c, 0.4f);
    }

    public static Color Brighten(Color c, float amount)
    {
        return new Color(
            Mathf.Clamp01(c.r + amount),
            Mathf.Clamp01(c.g + amount),
            Mathf.Clamp01(c.b + amount),
            c.a);
    }

    public static Color Desaturate(Color c, float amount)
    {
        float gray = (c.r + c.g + c.b) / 3f;
        return Color.Lerp(c, new Color(gray, gray, gray, c.a), amount);
    }
}
