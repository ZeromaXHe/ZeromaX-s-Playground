namespace TO.FSharp.Repos.Functions.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles

module private TileInitializer =
    let initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center


/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileRepo =
    let add (store: EntityStore) =
        fun
            (centerId: PointId)
            (chunkId: ChunkId)
            (hexFaces: FaceComponent array)
            (hexFaceIds: HexFaceIds)
            (neighborCenterIds: NeighborCenterIds) ->
            let unitCentroid = TileInitializer.initUnitCentroid hexFaces
            let unitCorners = TileInitializer.initUnitCorners hexFaces

            store
                .CreateEntity(
                    PointCenterId centerId,
                    PointNeighborCenterIds neighborCenterIds,
                    TileCountId <| store.Query<TileUnitCentroid>().Count + 1,
                    TileChunkId chunkId,
                    TileUnitCentroid unitCentroid,
                    TileUnitCorners unitCorners,
                    TileHexFaceIds hexFaceIds,
                    TileFlag TileFlagEnum.Explorable,
                    (TileValue 0)
                        .WithElevation(GD.RandRange(3, 7))
                        .WithWaterLevel(5)
                        .WithTerrainTypeIndex(GD.RandRange(0, 5)) // TODO: 临时测试用
                )
                .Id
