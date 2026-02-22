using UnityEngine;

public class ProvinceRenderer : MonoBehaviour
{
    const float HighlightBrightness = 1.3f;
    const float HighlightYOffset = 0.05f;
    const float BorderWidth = 0.15f;
    const float SelectionGlowIntensity = 0.4f;

    public FactionDefinition Faction { get; private set; }

    Material _baseMaterial;
    Material _highlightMaterial;
    Material _selectedMaterial;
    GameObject _borderRoot;
    GameObject _selectionRing;
    Vector3 _basePosition;

    public void Initialize(FactionDefinition faction, Mesh territoryMesh)
    {
        Faction = faction;
        _basePosition = transform.localPosition;
    }

    public void SetHighlighted(bool highlighted)
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr == null) return;
        mr.material = highlighted ? GetOrCreateHighlightMaterial() : GetOrCreateBaseMaterial();
        transform.localPosition = _basePosition + (highlighted ? Vector3.up * HighlightYOffset : Vector3.zero);
    }

    public void SetSelected(bool selected)
    {
        if (_selectionRing != null)
            _selectionRing.SetActive(selected);
        if (_selectionRing == null && selected)
        {
            _selectionRing = CreateSelectionRing();
            _selectionRing.transform.SetParent(transform);
        }
    }

    public Material GetFactionMaterial(FactionDefinition faction)
    {
        _baseMaterial = ShaderHelper.CreateMaterial(faction.primaryColor);
        return _baseMaterial;
    }

    public void CreateBorder(Mesh territoryMesh, Color borderColor)
    {
        _borderRoot = new GameObject("Border");
        _borderRoot.transform.SetParent(transform);
        _borderRoot.transform.localPosition = Vector3.zero;

        if (territoryMesh == null) return;
        var verts = territoryMesh.vertices;
        var borderMat = ShaderHelper.CreateMaterial(borderColor);
        for (int i = 0; i < verts.Length; i++)
        {
            var a = verts[i];
            var b = verts[(i + 1) % verts.Length];
            var line = CreateLine(a, b, BorderWidth, _borderRoot.transform);
            line.GetComponent<Renderer>().material = borderMat;
        }
    }

    Material GetOrCreateBaseMaterial()
    {
        if (_baseMaterial == null && Faction != null)
            _baseMaterial = ShaderHelper.CreateMaterial(Faction.primaryColor);
        return _baseMaterial;
    }

    Material GetOrCreateHighlightMaterial()
    {
        if (_highlightMaterial == null && Faction != null)
        {
            var c = Faction.primaryColor * HighlightBrightness;
            c.a = 1f;
            _highlightMaterial = ShaderHelper.CreateMaterial(c);
        }
        return _highlightMaterial;
    }

    GameObject CreateSelectionRing()
    {
        var go = new GameObject("SelectionRing");
        var mesh = GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) return go;
        var mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mr = go.AddComponent<MeshRenderer>();
        var c = Faction != null ? Faction.primaryColor : Color.yellow;
        var mat = ShaderHelper.CreateMaterial(c, 0f, 0.5f, c * SelectionGlowIntensity);
        mr.material = mat;
        go.transform.localPosition = new Vector3(0, 0.02f, 0);
        go.transform.localScale = Vector3.one * 1.02f;
        return go;
    }

    static GameObject CreateLine(Vector3 a, Vector3 b, float width, Transform parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "BorderSegment";
        go.transform.SetParent(parent);
        var mid = (a + b) * 0.5f;
        go.transform.localPosition = mid;
        var dir = (b - a).normalized;
        go.transform.forward = dir;
        go.transform.localScale = new Vector3(width, (b - a).magnitude, 0.01f);
        Object.Destroy(go.GetComponent<Collider>());
        return go;
    }
}
