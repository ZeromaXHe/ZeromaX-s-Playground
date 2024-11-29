using FrontEndToolFS.Tool;
using Godot;

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
    public bool UseColor
    {
        get => useColor;
        set => useColor = value;
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