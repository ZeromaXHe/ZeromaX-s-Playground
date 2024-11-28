using FrontEndToolFS.Tool;
using Godot;

[Tool]
public partial class HexGridChunk : HexGridChunkFS
{
    [Export]
    public HexMeshFS Terrain
    {
        get => terrain;
        set => terrain = value;
    }

    [Export]
    public HexMeshFS Rivers
    {
        get => rivers;
        set => rivers = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
}