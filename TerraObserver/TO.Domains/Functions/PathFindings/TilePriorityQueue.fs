namespace TO.Domains.Functions.PathFindings

open System
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 19:52:29
module TilePriorityQueue =
    let enqueue (tileId: TileId) (this: TilePriorityQueue) =
        let priority = this.Data[tileId].SearchPriority

        if priority < this.Minimum then
            this.Minimum <- priority

        while priority >= this.List.Count do
            this.List.Add -1

        this.Data[tileId].NextWithSamePriority <- this.List[priority]
        this.List[priority] <- tileId

    let tryDequeue (tileId: TileId outref) (this: TilePriorityQueue) =
        let mutable result = false

        while this.Minimum < this.List.Count && not result do
            tileId <- this.List[this.Minimum]

            if tileId >= 0 then
                this.List[this.Minimum] <- this.Data[tileId].NextWithSamePriority
                result <- true
            else
                this.Minimum <- this.Minimum + 1

        if not result then
            tileId <- -1

        result

    let change (tileId: TileId) (oldPriority: int) (this: TilePriorityQueue) =
        let mutable current = this.List[oldPriority]
        let mutable next = this.Data[current].NextWithSamePriority

        if current = tileId then
            this.List[oldPriority] <- next
        else
            while next <> tileId do
                current <- next
                next <- this.Data[current].NextWithSamePriority

            this.Data[current].NextWithSamePriority <- this.Data[tileId].NextWithSamePriority

        enqueue tileId this

    let clear (this: TilePriorityQueue) =
        this.List.Clear()
        this.Minimum <- Int32.MaxValue
