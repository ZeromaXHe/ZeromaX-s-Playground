// C# version of Vantage-point tree aka VP tree
// Original C++ source code is made by 
// Steve Hanov and you can find it from
// http://stevehanov.ca/blog/index.php?id=130
//
// This is free and unencumbered software released into the public domain.
// 来源地址：https://github.com/mcraiha/CSharp-vptree/blob/master/src/VpTree.cs
// 我（ZeromaXHe）做了一些简单的格式修改和翻译。

namespace Commons.Utils;

/// <summary>
/// 计算两物体间的距离
/// Calculate distance between two items
/// </summary>
/// <param name="item1">第一个物体 First item</param>
/// <param name="item2">第二个物体 Second item</param>
/// <typeparam name="T"></typeparam>
/// <returns>double 类型的距离。Distance as double</returns>
public delegate double CalculateDistance<in T>(T item1, T item2);

/// <summary>
/// VP 树类
/// Class for VP Tree
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class VpTree<T>
{
    /// <summary>
    /// 默认与唯一的构造函数
    /// Default and only constructor
    /// </summary>
    public VpTree() => _rand = new Random(); // 在 BuildFromPoints 中使用。 Used in BuildFromPoints

    /// <summary>
    /// 创建树
    /// Create tree
    /// </summary>
    /// <param name="newItems">新的物体数组 New items</param>
    /// <param name="distanceCalculator">距离计算方法 Distance calculator method</param>
    public void Create(T[] newItems, CalculateDistance<T> distanceCalculator)
    {
        _items = newItems;
        _calculateDistance = distanceCalculator;
        _root = BuildFromPoints(0, newItems.Length);
    }

    /// <summary>
    /// 搜索结果
    /// Search for results
    /// </summary>
    /// <param name="target">目标 Target</param>
    /// <param name="numberOfResults">想要的结果数量 Number of results wanted</param>
    /// <param name="results">结果（最近的一个是第一个物体） Results (nearest one is the first item)</param>
    /// <param name="distances">距离 Distances</param>
    public void Search(T target, int numberOfResults, out T[] results, out double[] distances)
    {
        var closestHits = new List<HeapItem>();
        // 重置 tau 为最长的可能距离
        // Reset tau to longest possible distance
        _tau = double.MaxValue;
        // 开始搜索
        // Start search
        Search(_root, target, numberOfResults, closestHits);
        // 返回值的临时数组
        // Temp arrays for return values
        var returnResults = new List<T>();
        var returnDistance = new List<double>();
        // 我们必须反转顺序，因为我们想要最近的物体是第一个结果
        // We have to reverse the order since we want the nearest object to be first in the array
        for (var i = numberOfResults - 1; i > -1; i--)
        {
            returnResults.Add(_items[closestHits[i].Index]);
            returnDistance.Add(closestHits[i].Dist);
        }

        results = returnResults.ToArray();
        distances = returnDistance.ToArray();
    }

    private T[] _items = [];
    private double _tau;
    private Node? _root;
    private readonly Random _rand; // 在 BuildFromPoints 中使用。Used in BuildFromPoints

    private CalculateDistance<T>? _calculateDistance;

    // 这不能是 struct，因为 Node 引用 Node 会导致 error CS0523
    // This cannot be struct because Node referring to Node causes error CS0523
    private sealed class Node
    {
        public int Index;
        public double Threshold;
        public Node? Left;
        public Node? Right;
    }

    private sealed class HeapItem(int index, double dist)
    {
        public readonly int Index = index;
        public readonly double Dist = dist;
        public static bool operator <(HeapItem h1, HeapItem h2) => h1.Dist < h2.Dist;
        public static bool operator >(HeapItem h1, HeapItem h2) => h1.Dist > h2.Dist;
    }

    private Node? BuildFromPoints(int lowerIndex, int upperIndex)
    {
        if (upperIndex == lowerIndex)
            return null;
        var node = new Node
        {
            Index = lowerIndex
        };

        if (upperIndex - lowerIndex > 1)
        {
            Swap(_items, lowerIndex, _rand.Next(lowerIndex + 1, upperIndex));
            var medianIndex = (upperIndex + lowerIndex) / 2;
            NthElement(_items, lowerIndex + 1, medianIndex, upperIndex - 1,
                (i1, i2) => Comparer<double>.Default.Compare(_calculateDistance!(_items[lowerIndex], i1),
                    _calculateDistance(_items[lowerIndex], i2)));
            node.Threshold = _calculateDistance!(_items[lowerIndex], _items[medianIndex]);
            node.Left = BuildFromPoints(lowerIndex + 1, medianIndex);
            node.Right = BuildFromPoints(medianIndex, upperIndex);
        }

        return node;
    }

    private void Search(Node? node, T target, int numberOfResults, List<HeapItem> closestHits)
    {
        if (node == null) return;
        var dist = _calculateDistance!(_items[node.Index], target);

        // 我们找到更短距离的项
        // We found entry with shorter distance
        if (dist < _tau)
        {
            if (closestHits.Count == numberOfResults)
            {
                // 太多结果，删除第一个（是最远距离的那个）
                // Too many results, remove the first one which has the longest distance
                closestHits.RemoveAt(0);
            }

            // 添加新的命中
            // Add new hit
            closestHits.Add(new HeapItem(node.Index, dist));

            // 如果我们有 numberOfResults 则重新排序，并设置新的 tau
            // Reorder if we have numberOfResults, and set new tau
            if (closestHits.Count == numberOfResults)
            {
                closestHits.Sort((a, b) => Comparer<double>.Default.Compare(b.Dist, a.Dist));
                _tau = closestHits[0].Dist;
            }
        }

        if (node.Left == null && node.Right == null) return;

        if (dist < node.Threshold)
        {
            if (dist - _tau <= node.Threshold)
                Search(node.Left, target, numberOfResults, closestHits);
            if (dist + _tau >= node.Threshold)
                Search(node.Right, target, numberOfResults, closestHits);
        }
        else
        {
            if (dist + _tau >= node.Threshold)
                Search(node.Right, target, numberOfResults, closestHits);
            if (dist - _tau <= node.Threshold)
                Search(node.Left, target, numberOfResults, closestHits);
        }
    }

    private static void Swap(T[] arr, int index1, int index2) =>
        (arr[index1], arr[index2]) = (arr[index2], arr[index1]);

    private static void NthElement<TE>(TE[] array, int startIndex, int nthToSeek, int endIndex,
        Comparison<TE> comparison)
    {
        var from = startIndex;
        var to = endIndex;

        // 如果 from == to 我们找到了 kth 元素
        // if from == to we reached the kth element
        while (from < to)
        {
            int r = from, w = to;
            var mid = array[(r + w) / 2];

            // 如果 reader 和 writer 相遇，则停止
            // stop if the reader and writer meets
            while (r < w)
            {
                if (comparison(array[r], mid) > -1)
                {
                    // 把大的值放在最后
                    // put the large values at the end
                    (array[w], array[r]) = (array[r], array[w]);
                    w--;
                }
                else
                {
                    // 如果值比中轴值小，则跳过
                    // the value is smaller than the pivot, skip
                    r++;
                }
            }

            // 如果我们步进了（r++），则需要回退一个位置
            // if we stepped up (r++) we need to step one down
            if (comparison(array[r], mid) > 0)
                r--;

            // r 指针在前 k 个元素之后
            // the r pointer is on the end of the first k elements
            if (nthToSeek <= r)
                to = r;
            else
                from = r + 1;
        }
    }
}