using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.domain;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;

namespace ZeromaXPlayground.game.inGame.map.scenes;

public partial class TileGui : Control
{
    #region on-ready nodes

    private Label _population;

    #endregion

    private Vector2I _coord;
    
    public void Init(int tileInfoId)
    {
        TileInfo tileInfo = TileInfo.GetById(tileInfoId);
        _coord = tileInfo.Coord;
        _population.Text = tileInfo.Population.ToString();
        tileInfo.PopulationChanged += OnTilePopulationChanged;
    }

    private void OnTilePopulationChanged(int population)
    {
        _population.Text = population.ToString();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _population = GetNode<Label>("Population");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}