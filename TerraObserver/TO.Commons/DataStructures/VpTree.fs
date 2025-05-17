namespace TO.Commons.DataStructures

open System
open System.Collections.Generic

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
type CalculateDistance<'T> = delegate of 'T * 'T -> double

type private Node =
    { mutable Index: int
      mutable Threshold: float
      mutable Left: Node option
      mutable Right: Node option }

[<Sealed>]
type private HeapItem(index: int, dist: float) =
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
    let rand = Random() // 在 BuildFromPoints 中使用。 Used in BuildFromPoints

    let mutable items: 'T array = null
    let mutable tau: float = 0.
    let mutable root: Node option = None
    let mutable calculateDistance: CalculateDistance<'T> = null

    let swap (arr: 'TE array) index1 index2 =
        let temp = arr[index1]
        arr[index1] <- arr[index2]
        arr[index2] <- temp

    let nthElement (array: 'TE array) startIndex nthToSeek endIndex (comparison: 'TE -> 'TE -> int) =
        let mutable fromIdx = startIndex
        let mutable toIdx = endIndex
        // 如果 from == to 我们找到了 kth 元素
        // if from == to we reached the kth element
        while fromIdx < toIdx do
            let mutable r = fromIdx
            let mutable w = toIdx
            let mid = array[(r + w) / 2]
            // 如果 reader 和 writer 相遇，则停止
            // stop if the reader and writer meets
            while r < w do
                if comparison array[r] mid > -1 then
                    // 把大的值放在最后
                    // put the large values at the end
                    swap array r w
                    w <- w - 1
                else
                    // 如果值比中轴值小，则跳过
                    // the value is smaller than the pivot, skip
                    r <- r + 1
            // 如果我们步进了（r++），则需要回退一个位置
            // if we stepped up (r++) we need to step one down
            if comparison array[r] mid > 0 then
                r <- r - 1
            // r 指针在前 k 个元素之后
            // the r pointer is on the end of the first k elements
            if nthToSeek <= r then toIdx <- r else fromIdx <- r + 1

    let rec buildFromPoints (lowerIndex: int) (upperIndex: int) =
        if upperIndex = lowerIndex then
            None
        else
            let node =
                { Index = lowerIndex
                  Threshold = 0.
                  Left = None
                  Right = None }

            if upperIndex - lowerIndex > 1 then
                swap items lowerIndex <| rand.Next(lowerIndex + 1, upperIndex)
                let medianIndex = (upperIndex + lowerIndex) / 2

                nthElement items
                <| lowerIndex + 1
                <| medianIndex
                <| upperIndex - 1
                <| (fun i1 i2 ->
                    compare
                    <| calculateDistance.Invoke(items[lowerIndex], i1)
                    <| calculateDistance.Invoke(items[lowerIndex], i2))

                node.Threshold <- calculateDistance.Invoke(items[lowerIndex], items[medianIndex])
                node.Left <- buildFromPoints <| lowerIndex + 1 <| medianIndex
                node.Right <- buildFromPoints medianIndex upperIndex

            Some node

    let rec search (nodeOpt: Node option, target: 'T, numberOfResults: int, closestHits: List<HeapItem>) =
        match nodeOpt with
        | None -> ()
        | Some node ->
            let dist = calculateDistance.Invoke(items[node.Index], target)
            // 我们找到更短距离的项
            // We found entry with shorter distance
            if dist < tau then
                if closestHits.Count = numberOfResults then
                    // 太多结果，删除第一个（是最远距离的那个）
                    // Too many results, remove the first one which has the longest distance
                    closestHits.RemoveAt 0
                // 添加新的命中
                // Add new hit
                closestHits.Add(HeapItem(node.Index, dist))
                // 如果我们有 numberOfResults 则重新排序，并设置新的 tau
                // Reorder if we have numberOfResults, and set new tau
                if closestHits.Count = numberOfResults then
                    closestHits.Sort(Comparison<HeapItem>(fun a b -> compare b.Dist a.Dist))
                    tau <- closestHits[0].Dist

            if node.Left = None && node.Right = None then
                ()
            elif dist < node.Threshold then
                if dist - tau <= node.Threshold then
                    search (node.Left, target, numberOfResults, closestHits)

                if dist + tau >= node.Threshold then
                    search (node.Right, target, numberOfResults, closestHits)
            else
                if dist + tau >= node.Threshold then
                    search (node.Right, target, numberOfResults, closestHits)

                if dist - tau <= node.Threshold then
                    search (node.Left, target, numberOfResults, closestHits)

    /// <summary>
    /// 创建树
    /// Create tree
    /// </summary>
    /// <param name="newItems">新的物体数组 New items</param>
    /// <param name="distanceCalculator">距离计算方法 Distance calculator method</param>
    member this.Create(newItems: 'T array, distanceCalculator: CalculateDistance<'T>) =
        items <- newItems
        calculateDistance <- distanceCalculator
        root <- buildFromPoints 0 newItems.Length

    /// <summary>
    /// 搜索结果
    /// Search for results
    /// </summary>
    /// <param name="target">目标 Target</param>
    /// <param name="numberOfResults">想要的结果数量 Number of results wanted</param>
    /// <param name="results">结果（最近的一个是第一个物体） Results (nearest one is the first item)</param>
    /// <param name="distances">距离 Distances</param>
    member this.Search(target: 'T, numberOfResults, results: byref<'T array>, distances: byref<float array>) =
        let closestHits = List<HeapItem>()
        // 重置 tau 为最长的可能距离
        // Reset tau to longest possible distance
        tau <- Double.MaxValue
        // 开始搜索
        // Start search
        search (root, target, numberOfResults, closestHits)
        // 返回值的临时数组
        // Temp arrays for return values
        let returnResults = List<'T>()
        let returnDistance = List<float>()
        // 我们必须反转顺序，因为我们想要最近的物体是第一个结果
        // We have to reverse the order since we want the nearest object to be first in the array
        for i in numberOfResults - 1 .. -1 .. 0 do
            returnResults.Add items[closestHits[i].Index]
            returnDistance.Add closestHits[i].Dist

        results <- returnResults.ToArray()
        distances <- returnDistance.ToArray()
