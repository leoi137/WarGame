using UnityEngine;

public static class ShaderHelper
{
    static Shader _cachedLitShader;
    static Shader _cachedUnlitShader;

    public static Shader GetLitShader()
    {
        if (_cachedLitShader != null) return _cachedLitShader;

        _cachedLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        _cachedLitShader = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        _cachedLitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        _cachedLitShader = Shader.Find("Sprites/Default");
        if (_cachedLitShader != null)
            return _cachedLitShader;

        _cachedLitShader = Shader.Find("Standard");
        return _cachedLitShader;
    }

    public static Shader GetUnlitShader()
    {
        if (_cachedUnlitShader != null) return _cachedUnlitShader;

        _cachedUnlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (_cachedUnlitShader != null && _cachedUnlitShader.name != "Hidden/InternalErrorShader")
            return _cachedUnlitShader;

        _cachedUnlitShader = Shader.Find("Unlit/Color");
        return _cachedUnlitShader;
    }

    public static Material CreateMaterial(Color color)
    {
        return CreateMaterial(color, 0f, 0.3f, null);
    }

    public static Material CreateMaterial(Color color, float metallic, float smoothness, Color? emission = null)
    {
        Material mat = new Material(GetLitShader());
        mat.color = color;

        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Glossiness"))
            mat.SetFloat("_Glossiness", smoothness);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", emission.Value);
        }

        return mat;
    }

    public static Material CreateUnlitMaterial(Color color)
    {
        Shader s = GetUnlitShader();
        Material mat = s != null ? new Material(s) : new Material(GetLitShader());
        mat.color = color;
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        return mat;
    }

    public static Material CreateAdditiveMaterial(Color color)
    {
        Material mat = CreateUnlitMaterial(color);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_ZWrite", 0);
        mat.renderQueue = 3100;
        return mat;
    }

    // PBR presets for common surface types
    public static Material SkinMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.25f, color * 0.03f);

    public static Material ChainmailMaterial(Color color) =>
        CreateMaterial(color, 0.7f, 0.45f, new Color(0.05f, 0.05f, 0.08f));

    public static Material SteelMaterial(Color color) =>
        CreateMaterial(color, 0.85f, 0.6f);

    public static Material GoldMaterial(Color color) =>
        CreateMaterial(color, 0.9f, 0.7f, color * 0.15f);

    public static Material LeatherMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.15f);

    public static Material FurMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.05f);

    public static Material WoodMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.12f);

    public static Material BoneMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.2f, color * 0.04f);

    public static Material ClothMaterial(Color color) =>
        CreateMaterial(color, 0f, 0.08f);

    public static Texture2D CreateGradientTexture(Color top, Color bottom, int size = 16)
    {
        Texture2D tex = new Texture2D(1, size);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
        {
            float t = (float)y / (size - 1);
            tex.SetPixel(0, y, Color.Lerp(bottom, top, t));
        }
        tex.Apply();
        return tex;
    }

    public static Texture2D CreateRadialGradientTexture(Color center, Color edge, int size = 32)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.wrapMode = TextureWrapMode.Clamp;
        float half = size * 0.5f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = (x - half) / half;
                float dy = (y - half) / half;
                float dist = Mathf.Clamp01(Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, Color.Lerp(center, edge, dist));
            }
        }
        tex.Apply();
        return tex;
    }

    public static void SetEmission(Material mat, Color emission)
    {
        if (mat == null) return;
        mat.EnableKeyword("_EMISSION");
        if (mat.HasProperty("_EmissionColor"))
            mat.SetColor("_EmissionColor", emission);
    }
}
