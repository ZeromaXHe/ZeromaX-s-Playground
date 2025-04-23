using Godot;

namespace Commons.Constants;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 09:34:26
public static class HexMeshConstant
{
    public static readonly Color Weights1 = Colors.Red;
    public static readonly Color Weights2 = Colors.Green;
    public static readonly Color Weights3 = Colors.Blue;

    public static T[] TriArr<T>(T c) => [c, c, c];
    public static T[] QuadArr<T>(T c) => [c, c, c, c];
    public static T[] QuadArr<T>(T c1, T c2) => [c1, c1, c2, c2];
}