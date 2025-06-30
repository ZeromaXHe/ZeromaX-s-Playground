namespace TO.Domains.Functions.DataStructures

open System
open System.Collections.Generic
open TO.Domains.Types.DataStructures

module private VpTree =
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

    let rec buildFromPoints (lowerIndex: int) (upperIndex: int) (this: 'T VpTree) =
        if upperIndex = lowerIndex then
            None
        else
            let node =
                { Index = lowerIndex
                  Threshold = 0f
                  Left = None
                  Right = None }

            if upperIndex - lowerIndex > 1 then
                swap this.Items lowerIndex <| this.Rand.Next(lowerIndex + 1, upperIndex)
                let medianIndex = (upperIndex + lowerIndex) / 2

                nthElement this.Items
                <| lowerIndex + 1
                <| medianIndex
                <| upperIndex - 1
                <| (fun i1 i2 ->
                    compare
                    <| this.CalculateDistance.Invoke(this.Items[lowerIndex], i1)
                    <| this.CalculateDistance.Invoke(this.Items[lowerIndex], i2))

                node.Threshold <- this.CalculateDistance.Invoke(this.Items[lowerIndex], this.Items[medianIndex])
                node.Left <- buildFromPoints <| lowerIndex + 1 <| medianIndex <| this
                node.Right <- buildFromPoints medianIndex upperIndex this

            Some node

    let rec search
        (nodeOpt: Node option)
        (target: 'T)
        (numberOfResults: int)
        (closestHits: List<HeapItem>)
        (this: 'T VpTree)
        =
        match nodeOpt with
        | None -> ()
        | Some node ->
            let dist = this.CalculateDistance.Invoke(this.Items[node.Index], target)
            // 我们找到更短距离的项
            // We found entry with shorter distance
            if dist < this.Tau then
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
                    this.Tau <- closestHits[0].Dist

            if node.Left = None && node.Right = None then
                ()
            elif dist < node.Threshold then
                if dist - this.Tau <= node.Threshold then
                    search node.Left target numberOfResults closestHits this

                if dist + this.Tau >= node.Threshold then
                    search node.Right target numberOfResults closestHits this
            else
                if dist + this.Tau >= node.Threshold then
                    search node.Right target numberOfResults closestHits this

                if dist - this.Tau <= node.Threshold then
                    search node.Left target numberOfResults closestHits this

module VpTreeQuery =
    /// <summary>
    /// 搜索结果
    /// Search for results
    /// </summary>
    /// <param name="target">目标 Target</param>
    /// <param name="numberOfResults">想要的结果数量 Number of results wanted</param>
    /// <param name="this">VP 树</param>
    /// <returns>结果（最近的一个是第一个物体） Results (nearest one is the first item) * 距离 Distances</returns>
    let search (target: 'T) numberOfResults (this: 'T VpTree) =
        let closestHits = List<HeapItem>()
        // 重置 tau 为最长的可能距离
        // Reset tau to longest possible distance
        this.Tau <- Single.MaxValue
        // 开始搜索
        // Start search
        VpTree.search this.Root target numberOfResults closestHits this
        // 返回值的临时数组
        // Temp arrays for return values
        let returnResults = List<'T>()
        let returnDistance = List<float32>()
        // 我们必须反转顺序，因为我们想要最近的物体是第一个结果
        // We have to reverse the order since we want the nearest object to be first in the array
        for i = numberOfResults - 1 downto 0 do
            returnResults.Add this.Items[closestHits[i].Index]
            returnDistance.Add closestHits[i].Dist

        returnResults.ToArray(), returnDistance.ToArray()

module VpTreeCommand =
    /// <summary>
    /// 创建树
    /// Create tree
    /// </summary>
    /// <param name="newItems">新的物体数组 New items</param>
    /// <param name="distanceCalculator">距离计算方法 Distance calculator method</param>
    /// <param name="this">VP 树</param>
    let create (newItems: 'T array) (distanceCalculator: 'T CalculateDistance) (this: 'T VpTree) =
        this.Items <- newItems
        this.CalculateDistance <- distanceCalculator
        this.Root <- VpTree.buildFromPoints 0 newItems.Length this
