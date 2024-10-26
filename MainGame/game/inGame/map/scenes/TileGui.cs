using System;
using System.Collections.Generic;
using BackEnd4IdleStrategyFS.Game;
using FrontEnd4IdleStrategyFS.Common;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scenes;

public partial class TileGui : Control
{
    private static readonly Dictionary<int, TileGui> IdMap = new();

    #region on-ready nodes

    private Label _population;

    #endregion

    private int _id;
    private Vector2I _coord;

    public void Init(int id, Tuple<int, int> coord, int population, Vector2 globalPosition)
    {
        _id = id;
        IdMap[_id] = this;
        _coord = BackEndUtil.fromBackEndI(coord.Item1, coord.Item2);
        _population.Text = population.ToString();

        Position = globalPosition;
    }

    public static void ChangePopulation(int id, int population)
    {
        GetById(id).ChangePopulation(population);
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