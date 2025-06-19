namespace TO.FSharp.Repos.Data.Commons

open System.Collections.Generic

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 15:59:08
type LruCache<'TKey, 'TValue when 'TKey: equality>(capacity: int) =
    let cacheMap =
        Dictionary<'TKey, LinkedListNode<KeyValuePair<'TKey, 'TValue>>>(capacity)

    let lruList = LinkedList<KeyValuePair<'TKey, 'TValue>>()

    member this.Get key =
        match cacheMap.TryGetValue key with
        | true, node ->
            lruList.Remove(node)
            lruList.AddFirst(node)
            Some node.Value.Value
        | _ -> None

    member this.Exist key = cacheMap.ContainsKey key

    member this.Put key value =
        match cacheMap.TryGetValue key with
        | true, node -> lruList.Remove(node)
        | _ when cacheMap.Count >= capacity ->
            let lastNode = lruList.Last
            cacheMap.Remove lastNode.Value.Key |> ignore
            lruList.RemoveLast()
        | _ -> ()

        let node = lruList.AddFirst(KeyValuePair(key, value))
        cacheMap[key] <- node

    member this.Remove key =
        match cacheMap.TryGetValue key with
        | true, node ->
            lruList.Remove node
            cacheMap.Remove key |> ignore
        | _ -> ()

    member this.Clear() =
        cacheMap.Clear()
        lruList.Clear()
