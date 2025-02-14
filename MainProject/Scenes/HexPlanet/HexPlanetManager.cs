using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

[Tool]
public partial class HexPlanetManager : Node3D
{
    [Export(PropertyHint.Range, "5, 1000")]
    private float _radius = 10f;

    [Export(PropertyHint.Range, "1, 100")] private int _divisions = 4;

    [Export(PropertyHint.Range, "0.1f, 1f")]
    private float _hexSize = 1f;

    private SurfaceTool _surfaceTool;
    private MeshInstance3D _meshIns;
    private HexasphereService _hexasphereService;

    private float _oldRadius;
    private int _oldDivisions;
    private float _oldHexSize;
    private float _lastUpdated;

    public override void _Ready()
    {
        _meshIns = new MeshInstance3D();
        AddChild(_meshIns);
        _surfaceTool = new SurfaceTool();
        DrawHexasphereMesh();
    }

    public override void _Process(double delta)
    {
        _lastUpdated += (float)delta;
        if (_lastUpdated < 1f) return;
        if (Mathf.Abs(_oldRadius - _radius) > 0.001f || _oldDivisions != _divisions ||
            Mathf.Abs(_oldHexSize - _hexSize) > 0.001f)
        {
            DrawHexasphereMesh();
        }
    }

    private void DrawHexasphereMesh()
    {
        _oldRadius = _radius;
        _oldDivisions = _divisions;
        _oldHexSize = _hexSize;
        _lastUpdated = 0f;

        _hexasphereService = new HexasphereService(_radius, _divisions, _hexSize);

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        _hexasphereService.BuildFaces(_surfaceTool);
        _surfaceTool.GenerateNormals();
        var material = new StandardMaterial3D();
        material.VertexColorUseAsAlbedo = true;
        _surfaceTool.SetMaterial(material);
        _meshIns.Mesh = _surfaceTool.Commit();
    }
}