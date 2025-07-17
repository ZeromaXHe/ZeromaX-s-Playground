namespace TO.Domains.Functions.DataStructures

open System.Collections.Generic
open TO.Domains.Types.DataStructures

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 19:12:29
module LruCache =
    let get key (this: LruCache<'TKey, 'TValue>) =
        match this.CacheMap.TryGetValue key with
        | true, node ->
            this.LruList.Remove(node)
            this.LruList.AddFirst(node)
            Some node.Value.Value
        | _ -> None

    let exist key (this: LruCache<'TKey, 'TValue>) = this.CacheMap.ContainsKey key

    let put key value (this: LruCache<'TKey, 'TValue>) =
        match this.CacheMap.TryGetValue key with
        | true, node -> this.LruList.Remove(node)
        | _ when this.CacheMap.Count >= this.Capacity ->
            let lastNode = this.LruList.Last
            this.CacheMap.Remove lastNode.Value.Key |> ignore
            this.LruList.RemoveLast()
        | _ -> ()

        let node = this.LruList.AddFirst(KeyValuePair(key, value))
        this.CacheMap[key] <- node

    let remove key (this: LruCache<'TKey, 'TValue>) =
        match this.CacheMap.TryGetValue key with
        | true, node ->
            this.LruList.Remove node
            this.CacheMap.Remove key |> ignore
        | _ -> ()

    let clear (this: LruCache<'TKey, 'TValue>) =
        this.CacheMap.Clear()
        this.LruList.Clear()
