namespace TO.FSharp.Repos.Functions

open Friflo.Engine.ECS
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Repos.Types.TileRepoT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileRepo =
    let private initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let private initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center

    let tryHeadByCenterId (store: EntityStore) : TryHeadTileByCenterId =
        fun centerId ->
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<TileComponent>().HasValue<TileComponent, int>(centerId)

    let add (store: EntityStore) : AddTile =
        fun centerId chunkId hexFaces hexFaceIds neighborCenterIds ->
            let unitCentroid = initUnitCentroid hexFaces
            let unitCorners = initUnitCorners hexFaces

            store
                .CreateEntity(
                    TileComponent(centerId, chunkId, unitCentroid, unitCorners, hexFaceIds, neighborCenterIds)
                )
                .Id

    let allSeq (store: EntityStore) : AllTilesSeq =
        fun () -> FrifloEcsUtil.toComponentSeq <| store.Query<TileComponent>()

    let truncate (store: EntityStore) : TruncateTiles =
        fun () -> FrifloEcsUtil.truncate <| store.Query<TileComponent>()

    let getDependency store : TileRepoDep =
        { TryHeadByCenterId = tryHeadByCenterId store
          Add = add store
          AllSeq = allSeq store
          Truncate = truncate store }
