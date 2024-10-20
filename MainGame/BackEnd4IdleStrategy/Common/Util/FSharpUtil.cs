using System.Numerics;

namespace BackEnd4IdleStrategy.Common.Util;

public static class FSharpUtil
{
    public static Tuple<int, int> ToTupleIntInt(Vector2 vec) => Tuple.Create((int)vec.X, (int)vec.Y);
    public static Vector2 ToVector2(Tuple<int, int> tuple) => new(tuple.Item1, tuple.Item2);
}