using System.Collections.Generic;
using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts;

public partial class Player : GodotObject
{
    [Signal]
    public delegate void TerritoryConqueredEventHandler(Player player, Vector2I vec);

    [Signal]
    public delegate void TerritoryLostEventHandler(Player player, Vector2I vec);

    public int Id
    {
        get => _id;
    }

    private int _id;
    private readonly List<Vector2I> _territoryVecs = new();

    public Player(int id)
    {
        _id = id;
    }

    public void ConquerTerritory(Vector2I vec)
    {
        GD.Print($"player id: {_id} conquering territory at vec: {vec}");
        _territoryVecs.Add(vec);
        EmitSignal(SignalName.TerritoryConquered, this, vec);
    }

    public void LoseTerritory(Vector2I vec)
    {
        _territoryVecs.Remove(vec);
        EmitSignal(SignalName.TerritoryLost, this, vec);
    }
}