namespace TO.Infras.Planets

open Friflo.Engine.ECS
open Godot
open TO.Commons.DataStructures
open TO.Infras.Abstractions.Planets.Commands
open TO.Infras.Abstractions.Planets.Queries
open TO.Infras.Planets.Models
open TO.Domains.Models.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-15 14:33:15
type PlanetRepo() =
    let store = EntityStore()
    // 单位球上的分块点 VP 树
    let chunkPointVpTree = VpTree<Vector3>()
    // 点
    let pointType = ComponentTypes.Get<PointComponent>()
    let pointTagChunk = Tags.Get<PointTagChunk>()
    let pointTagTile = Tags.Get<PointTagTile>()
    let pointChunkQuery = store.Query<PointComponent>().AllTags(&pointTagChunk)
    let pointTileQuery = store.Query<PointComponent>().AllTags(&pointTagTile)
    let pointChunkArchetype = store.GetArchetype(&pointType, &pointTagChunk)
    let pointTileArchetype = store.GetArchetype(&pointType, &pointTagTile)
    let pointIndexPosition = store.ComponentIndex<PointComponent, Vector3>()

    interface IPointQuery with
        member this.GetAllByChunky chunky =
            let query = if chunky then pointChunkQuery else pointTileQuery
            query.Chunks
            |> Seq.collect (fun chunk ->
                chunk.Entities
                |> Seq.map (fun entity ->
                    let point = entity.GetComponent<PointComponent>()

                    let p =
                        { PointId = entity.Id
                          Chunky = chunky
                          Position = point.Position
                          Coords = point.Coords
                          FaceIds = [] }

                    p))

    interface IPointCommand with
        member this.Add chunky position coords =
            let point =
                if chunky then
                    pointChunkArchetype.CreateEntity()
                else
                    pointTileArchetype.CreateEntity()

            let pointComponent = PointComponent(position, coords)
            point.AddComponent<PointComponent>(&pointComponent) |> ignore
            point.Id

    member this.ClearOldData() =
        store.Entities |> Seq.iter _.DeleteEntity()

    member this.SearchNearestChunk(pos: Vector3) =
        let mutable results: Vector3 array = null
        let mutable distances: float array = null
        chunkPointVpTree.Search(pos.Normalized(), 1, &results, &distances)

    member this.InitChunks() =
        let time = Time.GetTicksMsec()
        ()
