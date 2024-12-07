using FrontEndToolFS.Tool;
using Godot;

namespace ZeromaXPlayground.game.HexPlane.Map;

[Tool]
public partial class HexCell : HexCellFS
{
    // 忽略 IDE 提示，此处和 partial 不能删掉
    public override void _Ready() => base._Ready();
}