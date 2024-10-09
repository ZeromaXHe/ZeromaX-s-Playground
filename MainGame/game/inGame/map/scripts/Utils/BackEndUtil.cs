using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.Utils;

public static class BackEndUtil
{
    public static System.Numerics.Vector2 To(Vector2I vec) => new(vec.X, vec.Y);

    public static Vector2I From(System.Numerics.Vector2 vec) => new((int)vec.X, (int)vec.Y);
}