namespace TO.FSharp.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.FSharp.Domains.Components.HexSpheres.Faces
open TO.FSharp.Domains.Components.HexSpheres.Points
open TO.FSharp.Domains.Components.HexSpheres.Tiles
open TO.FSharp.Domains.Enums.Tiles
open TO.FSharp.Domains.Functions.HexSpheres
open TO.FSharp.Domains.Alias.HexSpheres.Chunks
open TO.FSharp.Domains.Alias.HexSpheres.Faces
open TO.FSharp.Domains.Alias.HexSpheres.Points
open TO.FSharp.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileCommand =
    let add (env: #IEntityStore) =
        fun
            (centerId: PointId)
            (chunkId: ChunkId)
            (hexFaces: FaceComponent array)
            (hexFaceIds: HexFaceIds)
            (neighborCenterIds: NeighborCenterIds) ->
            let unitCentroid = FaceFunction.getUnitCentroid hexFaces
            let unitCorners = FaceFunction.getUnitCorners hexFaces

            env.EntityStore
                .CreateEntity(
                    PointCenterId centerId,
                    PointNeighborCenterIds neighborCenterIds,
                    TileCountId <| env.EntityStore.Query<TileUnitCentroid>().Count + 1,
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
