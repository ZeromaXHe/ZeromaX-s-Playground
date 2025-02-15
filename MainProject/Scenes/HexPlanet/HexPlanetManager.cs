using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

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

    private Node3D _tiles;

    public override void _Ready()
    {
        _tiles = GetNode<Node3D>("Tiles");
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

        foreach (var child in _tiles.GetChildren())
        {
            child.QueueFree();
        }
        _hexasphereService = new HexasphereService(_radius, _divisions, _hexSize);
        foreach (var tile in _hexasphereService.GetAllTiles())
        {
            var tileNode = new TileNode();
            tileNode.InitTileNode(_hexasphereService.GetRepo(), tile.Id, _radius, _hexSize);
            _tiles.AddChild(tileNode);
        }
    }
}