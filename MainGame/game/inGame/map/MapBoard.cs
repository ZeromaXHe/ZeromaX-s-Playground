using FrontEnd4IdleStrategyFS.Display;
using Godot;

namespace ZeromaXPlayground.game.inGame.map;

public partial class MapBoard : MapBoardFS
{
    // Export 的实现需要注意 C# get set 里面用一个 F# 的字段（val mutable）/属性（member val）
    [Export(PropertyHint.Range, "2,8")]
    private int PlayerCount
    {
        get => _playerCount;
        set => _playerCount = value;
    }

    // 必须保留此处和 partial，请忽略 IDE 建议 
    public override void _Ready()
    {
        base._Ready();
    }
}