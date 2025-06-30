namespace TO.Domains.Functions.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexSpheres.Components.Faces
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileCommand =
    let add (env: #IEntityStoreQuery) : AddTile =
        fun
            (centerId: PointId)
            (chunkId: ChunkId)
            (hexFaces: FaceComponent array)
            (hexFaceIds: HexFaceIds)
            (neighborCenterIds: NeighborCenterIds) ->
            let store = env.EntityStore
            let unitCentroid = FaceFunction.getUnitCentroid hexFaces
            let unitCorners = FaceFunction.getUnitCorners hexFaces

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
                    |> TileValue.withElevation (GD.RandRange(3, 7))
                    |> TileValue.withWaterLevel 5
                    |> TileValue.withTerrainTypeIndex (GD.RandRange(0, 5)), // TODO: 临时测试用
                    TileVisibility 0
                )
                .Id
