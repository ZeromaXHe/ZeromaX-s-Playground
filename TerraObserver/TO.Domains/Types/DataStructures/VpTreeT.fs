namespace TO.Domains.Types.DataStructures

open System

// C# version of Vantage-point tree aka VP tree
// Original C++ source code is made by
// Steve Hanov and you can find it from
// http://stevehanov.ca/blog/index.php?id=130
//
// This is free and unencumbered software released into the public domain.
// 来源地址：https://github.com/mcraiha/CSharp-vptree/blob/master/src/VpTree.cs
// 我（ZeromaXHe）将其改写为 F# 代码，并作了一些简单的格式修改和翻译。

/// <summary>
/// 计算两物体间的距离
/// Calculate distance between two items
/// </summary>
/// <param name="item1">第一个物体 First item</param>
/// <param name="item2">第二个物体 Second item</param>
/// <typeparam name="'T"></typeparam>
/// <returns>double 类型的距离。Distance as double</returns>
type CalculateDistance<'T> = delegate of 'T * 'T -> float32

type internal Node =
    { mutable Index: int
      mutable Threshold: float32
      mutable Left: Node option
      mutable Right: Node option }

[<Sealed>]
type internal HeapItem(index: int, dist: float32) =
    member this.Index = index
    member this.Dist = dist
    // (<)
    static member op_LessThan(h1: HeapItem, h2: HeapItem) = h1.Dist < h2.Dist
    // (>)
    static member op_GreaterThan(h1: HeapItem, h2: HeapItem) = h1.Dist > h2.Dist

/// <summary>
/// VP 树类
/// Class for VP Tree
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 20:02:15
/// </summary>
/// <typeparam name="'T"></typeparam>
type VpTree<'T>() =
    /// <summary>
    /// 默认与唯一的构造函数
    /// Default and only constructor
    /// </summary>
    member val internal Rand = Random() // 在 BuildFromPoints 中使用。 Used in BuildFromPoints

    member val internal Items: 'T array = null with get, set
    member val internal Tau = 0f with get, set
    member val internal Root: Node option = None with get, set
    member val internal CalculateDistance: 'T CalculateDistance = null with get, set
