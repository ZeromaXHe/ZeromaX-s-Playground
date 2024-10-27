using System;
using Godot;

namespace ZeromaXPlayground.game.Global.Common;

public static class BackEndUtil
{
    public static Tuple<int, int> To(Vector2I vec) => new(vec.X, vec.Y);

    public static Vector2I FromI(Tuple<int, int> coord) => new(coord.Item1, coord.Item2);

    public static Vector2 From(Tuple<int, int> coord) => new(coord.Item1, coord.Item2);
}