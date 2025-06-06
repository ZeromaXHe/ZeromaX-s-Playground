namespace TO.FSharp.Repos.Functions

open Friflo.Engine.ECS
open TO.FSharp.Commons.Utils
open TO.FSharp.Repos.Models.HexSpheres.Faces
open TO.FSharp.Repos.Models.HexSpheres.Points
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

    let add (store: EntityStore) : AddTile =
        fun centerId chunkId hexFaces hexFaceIds neighborCenterIds ->
            let unitCentroid = initUnitCentroid hexFaces
            let unitCorners = initUnitCorners hexFaces

            store
                .CreateEntity(
                    PointCenterId centerId,
                    TileChunkId chunkId,
                    TileUnitCentroid unitCentroid,
                    TileUnitCorners unitCorners,
                    TileHexFaceIds hexFaceIds,
                    PointNeighborCenterIds neighborCenterIds
                )
                .Id

    let centroidAndCornersSeq (store: EntityStore) : CentroidAndCornersSeq =
        fun () -> FrifloEcsUtil.toComponentSeq2 <| store.Query<TileUnitCentroid, TileUnitCorners>()

    let truncate (store: EntityStore) : TruncateTiles =
        fun () -> FrifloEcsUtil.truncate <| store.Query<TileUnitCentroid>()

    let getDependency store : TileRepoDep =
        { Add = add store
          CentroidAndCornersSeq = centroidAndCornersSeq store
          Truncate = truncate store }
