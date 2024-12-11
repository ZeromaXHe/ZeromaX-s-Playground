using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

[Tool]
public partial class HexMesh : HexMeshFS
{
    [Export]
    public bool UseCollider
    {
        get => useCollider;
        set => useCollider = value;
    }

    [Export]
    public bool UseCellData
    {
        get => useCellData;
        set => useCellData = value;
    }

    [Export]
    public bool UseUvCoordinates
    {
        get => useUvCoordinates;
        set => useUvCoordinates = value;
    }

    [Export]
    public bool UseUv2Coordinates
    {
        get => useUv2Coordinates;
        set => useUv2Coordinates = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}