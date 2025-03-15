using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

using System.Collections.Generic;

/// <summary>
/// LRU 缓存
/// 类似 Java 的 LinkedHashMap 原理
/// </summary>
/// <param name="capacity"></param>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
class LruCache<TKey, TValue>(int capacity)
{
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _cacheMap = new(capacity);
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _lruList = [];

    public TValue Get(TKey key)
    {
        if (!_cacheMap.TryGetValue(key, out var node)) return default;
        _lruList.Remove(node); // 这里用 LinkedListNode 直接 Remove 的时间复杂度是 O(1)
        _lruList.AddFirst(node);
        return node.Value.Value;
    }

    public void Put(TKey key, TValue value)
    {
        if (_cacheMap.TryGetValue(key, out var existingNode))
        {
            _lruList.Remove(existingNode);
        }
        else if (_cacheMap.Count >= capacity)
        {
            var lastNode = _lruList.Last;
            _cacheMap.Remove(lastNode!.Value.Key);
            _lruList.RemoveLast();
        }

        var newNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
        _lruList.AddFirst(newNode);
        _cacheMap[key] = newNode;
    }
}

public enum ChunkLod
{
    JustHex, // 每个地块只有六个平均高度点组成的六边形（非平面）
    PlaneHex, // 高度立面，无特征，无河流的六边形
    SimpleHex, // 最简单的 Solid + 斜面六边形 
    TerracesHex, // 增加台阶
    Full, // 增加边细分
}

class LodLruCache
{
    private readonly Dictionary<ChunkLod, LruCache<int, Mesh>> _cache = new();
}

public class LodMeshCacheService: ILodMeshCacheService
{
}