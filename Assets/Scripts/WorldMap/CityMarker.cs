using UnityEngine;

public class CityMarker : MonoBehaviour
{
    const float CapitalScale = 1.5f;
    const float NormalScale = 1f;
    const float HighlightScale = 1.3f;

    public CityDefinition City { get; private set; }
    public bool isCapital => City != null && City.isCapital;

    Material _baseMaterial;
    Material _highlightMaterial;
    float _baseScale;

    public void Initialize(CityDefinition city, FactionDefinition faction)
    {
        City = city;
        if (city == null) return;

        var worldPos = new Vector3(
            city.normalizedPosition.x * GameConfig.WorldMapWidth,
            0.5f,
            city.normalizedPosition.y * GameConfig.WorldMapHeight);
        transform.position = worldPos;

        _baseScale = city.isCapital ? CapitalScale : NormalScale;
        transform.localScale = Vector3.one * _baseScale;

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Marker";
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        var col = gameObject.AddComponent<SphereCollider>();
        col.radius = 1.5f;
        col.center = Vector3.zero;
        Object.Destroy(go.GetComponent<Collider>());

        var color = faction?.primaryColor ?? Color.gray;
        _baseMaterial = ShaderHelper.CreateMaterial(color);
        _highlightMaterial = ShaderHelper.CreateMaterial(color * 1.4f);
        go.GetComponent<Renderer>().material = _baseMaterial;
    }

    public void SetHighlighted(bool highlighted)
    {
        var scale = highlighted ? _baseScale * HighlightScale : _baseScale;
        transform.localScale = Vector3.one * scale;
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material = highlighted ? _highlightMaterial : _baseMaterial;
    }
}
