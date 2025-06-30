namespace TO.Domains.Types.DataStructures

open System.Collections.Generic

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 15:59:08
type LruCache<'TKey, 'TValue when 'TKey: equality>(capacity: int) =
    member val internal CacheMap = Dictionary<'TKey, LinkedListNode<KeyValuePair<'TKey, 'TValue>>>(capacity)
    member val internal LruList = LinkedList<KeyValuePair<'TKey, 'TValue>>()
    member internal this.Capacity = capacity
