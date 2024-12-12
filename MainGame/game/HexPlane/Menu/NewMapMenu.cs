using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Menu;

public partial class NewMapMenu : NewMapMenuFS
{
    [Export]
    public HexGridFS HexGrid
    {
        get => hexGrid;
        set => hexGrid = value;
    }

    [Export]
    public HexMapGeneratorFS MapGenerator
    {
        get => mapGenerator;
        set => mapGenerator = value;
    }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override void _Ready() => base._Ready();
}