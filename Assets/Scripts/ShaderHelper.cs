using UnityEngine;

/// <summary>
/// Utility to find a working shader for materials.
/// Falls back through multiple options to handle both 2D and 3D render pipelines.
/// </summary>
public static class ShaderHelper
{
    static Shader _cachedLitShader;

    public static Shader GetLitShader()
    {
        if (_cachedLitShader != null) return _cachedLitShader;

        // Try URP Lit first (3D renderer)
        _cachedLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        // Try URP Simple Lit
        _cachedLitShader = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        // Try URP Unlit
        _cachedLitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (_cachedLitShader != null && _cachedLitShader.name != "Hidden/InternalErrorShader")
            return _cachedLitShader;

        // Try Sprites/Default (works everywhere)
        _cachedLitShader = Shader.Find("Sprites/Default");
        if (_cachedLitShader != null)
            return _cachedLitShader;

        // Last resort
        _cachedLitShader = Shader.Find("Standard");
        return _cachedLitShader;
    }

    public static Material CreateMaterial(Color color)
    {
        Material mat = new Material(GetLitShader());
        mat.color = color;
        return mat;
    }
}
