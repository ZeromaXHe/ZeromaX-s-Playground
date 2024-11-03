using Godot;
using BackEnd4IdleStrategyFS.Game;
using ZeromaXPlayground.game.Global.Adapter;

public partial class GlobalNode : Node
{
    public Entry.Container EntryContainer = null;

    public void ChangeToIdleStrategyScene() =>
        GetTree().ChangeSceneToFile("res://game/inGame/menu/InGameMenu.tscn");

    public void InitIdleStrategyGame(TileMapLayer baseTerrain, int playerCount)
    {
        EntryContainer = new Entry.Container(
            new AStar2DAdapter(new AStar2D()), 
            new TileMapLayerAdapter(baseTerrain),
            playerCount);
    }
}