namespace TO.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Points
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Functions.HexSpheres
open TO.Domains.Structs.HexSphereGrids
open TO.Domains.Utils.Commons
open TO.Repos.Data.HexSpheres

type AddPoint = Chunky -> Vector3 -> SphereAxial -> PointId
type CreateVpTree = Chunky -> unit

[<Interface>]
type IPointCommand =
    abstract Add: AddPoint
    abstract CreateVpTree: CreateVpTree

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:50:19
module PointCommand =
    let add (store: EntityStore) : AddPoint =
        fun (chunky: Chunky) (pos: Vector3) (coords: SphereAxial) ->
            let tag = ChunkFunction.chunkyTag chunky
            let point = store.CreateEntity(PointComponent(pos, coords), &tag)
            point.Id

    let createVpTree (store: EntityStore) (chunkyVpTrees: ChunkyVpTrees) : CreateVpTree =
        fun chunky ->
            let tag = ChunkFunction.chunkyTag chunky
            let pointQuery = store.Query<PointComponent>().AllTags(&tag)

            let items =
                FrifloEcsUtil.toComponentSeq pointQuery |> Seq.map _.Position |> Seq.toArray

            let tree = chunkyVpTrees.Choose chunky
            tree.Create(items, fun p0 p1 -> p0.DistanceTo p1)
