using Godot;

[Tool]
public partial class HexPlanetManager : Node3D
{
    [Export]
    private bool Regenerate
    {
        get => _regenerate;
        set
        {
            if (!value) return;
            UpdateRenderObjects();
            _regenerate = false;
        }
    }

    private bool _regenerate = false;

    public HexPlanet HexPlanet;
    private HexPlanet _prevHexPlanet;
    private Node3D _hexChunkRenders;

    public override void _Ready()
    {
        HexPlanet = GetNode<HexPlanet>("HexPlanet");
        _hexChunkRenders = GetNode<Node3D>("HexChunkRenders");

        UpdateRenderObjects();
    }

    // Called when the whole sphere must be regenerated
    public void UpdateRenderObjects()
    {
        // 删除所有子节点 Delete all children
        if (_hexChunkRenders == null)
        {
            GD.Print("_hexChunkRenders is null");
            return;
        }
        else
            foreach (var child in _hexChunkRenders.GetChildren())
            {
                child.QueueFree();
            }

        if (HexPlanet == null)
        {
            return;
        }

        HexPlanetHexGenerator.GeneratePlanetTilesAndChunks(HexPlanet);

        for (var i = 0; i < HexPlanet.Chunks.Count; i++)
        {
            var chunkRenderer = new HexChunkRenderer();
            chunkRenderer.Name = "Chunk " + i;
            chunkRenderer.Position = Vector3.Zero;
            chunkRenderer.SetHexChunk(HexPlanet, i);
            chunkRenderer.UpdateMesh();
            _hexChunkRenders.AddChild(chunkRenderer);
        }
    }
}