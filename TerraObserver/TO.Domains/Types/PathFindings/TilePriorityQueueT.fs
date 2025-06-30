namespace TO.Domains.Types.PathFindings

open System

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 21:19:06
type TilePriorityQueue(data: TileSearchData array) =
    member val List = ResizeArray<int>()
    member val Minimum = Int32.MaxValue with get, set
    member this.Data = data
