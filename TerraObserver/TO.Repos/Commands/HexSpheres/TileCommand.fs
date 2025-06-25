namespace TO.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Tiles
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Enums.Tiles
open TO.Domains.Functions.HexSpheres
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Faces
open TO.Domains.Alias.HexSpheres.Points

type AddTile = PointId -> ChunkId -> FaceComponent array -> HexFaceIds -> NeighborCenterIds -> TileId

[<Interface>]
type ITileCommand =
    abstract AddTile: AddTile

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileCommand =
    let add (store: EntityStore) : AddTile =
        fun
            (centerId: PointId)
            (chunkId: ChunkId)
            (hexFaces: FaceComponent array)
            (hexFaceIds: HexFaceIds)
            (neighborCenterIds: NeighborCenterIds) ->
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
                        .WithElevation(GD.RandRange(3, 7))
                        .WithWaterLevel(5)
                        .WithTerrainTypeIndex(GD.RandRange(0, 5)), // TODO: 临时测试用
                    TileVisibility 0
                )
                .Id
