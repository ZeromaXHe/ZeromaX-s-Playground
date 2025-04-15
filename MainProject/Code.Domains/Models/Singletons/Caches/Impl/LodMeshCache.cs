using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace Domains.Models.Singletons.Caches.Impl;

/// <summary>
/// LRU 缓存
/// 类似 Java 的 LinkedHashMap 原理
/// </summary>
/// <param name="capacity"></param>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
class LruCache<TKey, TValue>(int capacity) where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _cacheMap = new(capacity);
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _lruList = [];

    public TValue? Get(TKey key)
    {
        if (!_cacheMap.TryGetValue(key, out var node)) return default;
        _lruList.Remove(node); // 这里用 LinkedListNode 直接 Remove 的时间复杂度是 O(1)
        _lruList.AddFirst(node);
        return node.Value.Value;
    }

    public bool Exist(TKey key) => _cacheMap.ContainsKey(key);

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

    public void Remove(TKey key)
    {
        if (!_cacheMap.TryGetValue(key, out var node)) return;
        _lruList.Remove(node);
        _cacheMap.Remove(key);
    }
    
    public void Clear()
    {
        _cacheMap.Clear();
        _lruList.Clear();
    }
}

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-15 20:53
public class LodMeshCache : ILodMeshCache
{
    private readonly Dictionary<ChunkLod, LruCache<int, Mesh[]>> _cache = new()
    {
        [ChunkLod.JustHex] = new LruCache<int, Mesh[]>(2500),
        [ChunkLod.PlaneHex] = new LruCache<int, Mesh[]>(5000),
        [ChunkLod.SimpleHex] = new LruCache<int, Mesh[]>(10000),
        [ChunkLod.TerracesHex] = new LruCache<int, Mesh[]>(20000),
        [ChunkLod.Full] = new LruCache<int, Mesh[]>(40000),
    };

    public Mesh[]? GetLodMeshes(ChunkLod lod, int id) =>
        _cache.TryGetValue(lod, out var cache) ? cache.Get(id) : null;

    public void AddLodMeshes(ChunkLod lod, int id, Mesh[] mesh)
    {
        if (!_cache.TryGetValue(lod, out var cache)) return;
        if (cache.Exist(id))
            GD.PrintErr($"overwriting Chunk {id} Lod {lod} Mesh!");
        cache.Put(id, mesh);
    }

    public void RemoveAllLodMeshes()
    {
        foreach (var lod in Enum.GetValues<ChunkLod>())
            if (_cache.TryGetValue(lod, out var cache))
                cache.Clear();
    }

    public void RemoveLodMeshes(int id)
    {
        foreach (var lod in Enum.GetValues<ChunkLod>())
            if (_cache.TryGetValue(lod, out var cache))
                cache.Remove(id);
    }
}