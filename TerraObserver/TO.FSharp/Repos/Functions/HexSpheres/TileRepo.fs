namespace TO.FSharp.Repos.Functions.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Repos.Types.HexSpheres.TileRepoT

module private TileInitializer =
    let initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center


/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileRepo =
    let add (store: EntityStore) : AddTile =
        fun centerId chunkId hexFaces hexFaceIds neighborCenterIds ->
            let unitCentroid = TileInitializer.initUnitCentroid hexFaces
            let unitCorners = TileInitializer.initUnitCorners hexFaces

            store
                .CreateEntity(
                    PointCenterId centerId,
                    PointNeighborCenterIds neighborCenterIds,
                    TileChunkId chunkId,
                    TileUnitCentroid unitCentroid,
                    TileUnitCorners unitCorners,
                    TileHexFaceIds hexFaceIds,
                    TileFlag TileFlagEnum.Explorable,
                    (TileValue 0)
                        .WithElevation(GD.RandRange(0, 10))
                        .WithWaterLevel(5)
                        .WithTerrainTypeIndex(GD.RandRange(0, 5)) // TODO: 临时测试用
                )
                .Id

    let count (store: EntityStore) : CountTile =
        fun () -> store.Query<TileUnitCentroid>().Count

    let centroidAndCornersSeq (store: EntityStore) : CentroidAndCornersSeq =
        fun () ->
            FrifloEcsUtil.toComponentSeq2
            <| store.Query<TileUnitCentroid, TileUnitCorners>()

    let forEachByChunkId (store: EntityStore) : ForEachTileByChunkId =
        fun (chunkId: ChunkId) (forEachEntity: TileChunkId ForEachEntity) ->
            store.Query<TileChunkId>().HasValue<TileChunkId, ChunkId>(chunkId).ForEachEntity forEachEntity

    let getChunkIdById (store: EntityStore) : GetTileChunkIdById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileChunkId>()

    let getUnitCentroidById (store: EntityStore) : GetTileUnitCentroidById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileUnitCentroid>()

    let getUnitCornersById (store: EntityStore) : GetTileUnitCornersById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileUnitCorners>()

    let getHexFaceIdsById (store: EntityStore) : GetTileHexFaceIdsById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileHexFaceIds>()

    let getValueById (store: EntityStore) : GetTileValueById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileValue>()

    let getFlagById (store: EntityStore) : GetTileFlagById =
        fun (tileId: TileId) -> store.GetEntityById(tileId).GetComponent<TileFlag>()

    let truncate (store: EntityStore) : TruncateTiles =
        fun () -> FrifloEcsUtil.truncate <| store.Query<TileUnitCentroid>()

    let getDependency store : TileRepoDep =
        { Add = add store
          Count = count store
          CentroidAndCornersSeq = centroidAndCornersSeq store
          ForEachByChunkId = forEachByChunkId store
          GetChunkIdById = getChunkIdById store
          GetUnitCentroidById = getUnitCentroidById store
          GetUnitCornersById = getUnitCornersById store
          GetHexFaceIdsById = getHexFaceIdsById store
          GetValueById = getValueById store
          GetFlagById = getFlagById store
          Truncate = truncate store }
