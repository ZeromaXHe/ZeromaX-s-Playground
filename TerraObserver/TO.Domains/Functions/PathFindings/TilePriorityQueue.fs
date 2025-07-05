namespace TO.Domains.Functions.PathFindings

open System
open Friflo.Engine.ECS
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.PathFindings

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 19:52:29
module TilePriorityQueue =
    let enqueue (tileCountId: int) (this: TilePriorityQueue) =
        let priority = this.Data[tileCountId].SearchPriority

        if priority < this.Minimum then
            this.Minimum <- priority

        while priority >= this.List.Count do
            this.List.Add -1

        this.Data[tileCountId].NextWithSamePriority <- this.List[priority]
        this.List[priority] <- tileCountId

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

    let change (tileCountId: int) (oldPriority: int) (this: TilePriorityQueue) =
        let mutable current = this.List[oldPriority]
        let mutable next = this.Data[current].NextWithSamePriority

        if current = tileCountId then
            this.List[oldPriority] <- next
        else
            while next <> tileCountId do
                current <- next
                next <- this.Data[current].NextWithSamePriority

            this.Data[current].NextWithSamePriority <- this.Data[tileCountId].NextWithSamePriority

        enqueue tileCountId this

    let clear (this: TilePriorityQueue) =
        this.List.Clear()
        this.Minimum <- Int32.MaxValue
