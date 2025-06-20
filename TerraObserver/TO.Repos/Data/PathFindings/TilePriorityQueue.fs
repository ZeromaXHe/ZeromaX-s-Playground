namespace TO.Repos.Data.PathFindings

open System
open TO.Domains.Alias.HexSpheres.Tiles
open TO.Domains.Structs.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 21:19:06
type TilePriorityQueue(data: TileSearchData array) =
    let list = ResizeArray<int>()
    let mutable minimum = Int32.MaxValue

    member this.Enqueue(tileId: TileId) =
        let priority = data[tileId].SearchPriority

        if priority < minimum then
            minimum <- priority

        while priority >= list.Count do
            list.Add -1

        data[tileId].NextWithSamePriority <- list[priority]
        list[priority] <- tileId

    member this.TryDequeue(tileId: TileId outref) =
        let mutable result = false

        while minimum < list.Count && not result do
            tileId <- list[minimum]

            if tileId >= 0 then
                list[minimum] <- data[tileId].NextWithSamePriority
                result <- true
            else
                minimum <- minimum + 1

        if not result then
            tileId <- -1

        result

    member this.Change(tileId: TileId, oldPriority: int) =
        let mutable current = list[oldPriority]
        let mutable next = data[current].NextWithSamePriority

        if current = tileId then
            list[oldPriority] <- next
        else
            while next <> tileId do
                current <- next
                next <- data[current].NextWithSamePriority

            data[current].NextWithSamePriority <- data[tileId].NextWithSamePriority

        this.Enqueue tileId

    member this.Clear() =
        list.Clear()
        minimum <- Int32.MaxValue
