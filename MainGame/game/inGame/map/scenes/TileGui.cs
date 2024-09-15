using System.Collections.Generic;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.domain;

namespace ZeromaXPlayground.game.inGame.map.scenes;

public partial class TileGui : Control
{
    private static readonly Dictionary<int, TileGui> IdMap = new();

    #region on-ready nodes

    private Label _population;

    #endregion

    private int _id;
    private Vector2I _coord;
    
    public void Init(TileInfo tileInfo, Vector2 globalPosition)
    {
        _id = tileInfo.Id;
        IdMap[_id] = this;
        _coord = tileInfo.Coord;
        _population.Text = tileInfo.Population.ToString();

        Position = globalPosition;
    }

    public void ChangePopulation(int population)
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
    
    #region 查询接口

    public static TileGui GetById(int id)
    {
        return IdMap.TryGetValue(id, out var result) ? result : null;
    }
    
    #endregion
}