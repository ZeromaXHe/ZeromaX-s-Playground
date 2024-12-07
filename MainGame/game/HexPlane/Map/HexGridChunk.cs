using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

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

    [Export]
    public HexMeshFS Roads
    {
        get => roads;
        set => roads = value;
    }

    [Export]
    public HexMeshFS Water
    {
        get => water;
        set => water = value;
    }

    [Export]
    public HexMeshFS WaterShore
    {
        get => waterShore;
        set => waterShore = value;
    }

    [Export]
    public HexMeshFS Estuaries
    {
        get => estuaries;
        set => estuaries = value;
    }

    [Export]
    public HexFeatureManagerFS Features
    {
        get => features;
        set => features = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
    public override void _Process(double delta) => base._Process(delta);
}