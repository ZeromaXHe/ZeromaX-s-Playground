namespace TO.FSharp.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Functions.HexSpheres
open TO.Domains.Structs.HexSphereGrids
open TO.Domains.Utils.Commons
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:50:19
module PointCommand =
    let add (env: #IEntityStore) =
        fun (chunky: bool) (pos: Vector3) (coords: SphereAxial) ->
            let tag = ChunkFunction.chunkyTag chunky
            let point = env.EntityStore.CreateEntity(PointComponent(pos, coords), &tag)
            point.Id

    let createVpTree (env: 'E when 'E :> IEntityStore and 'E :> IChunkyVpTrees) =
        fun chunky ->
            let tag = ChunkFunction.chunkyTag chunky
            let pointQuery = env.EntityStore.Query<PointComponent>().AllTags(&tag)

            let items =
                FrifloEcsUtil.toComponentSeq pointQuery |> Seq.map _.Position |> Seq.toArray

            let tree = env.ChunkyVpTrees.Choose chunky
            tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)
