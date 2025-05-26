namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 13:34:19
type TileRepo(store: EntityStore) =

    let initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center

    member this.QueryByCenterId(centerId: int) =
        let tileChunks =
            store.Query<TileComponent>().HasValue<TileComponent, int>(centerId).Chunks

        if tileChunks.Count = 0 then
            None
        else
            tileChunks
            |> Seq.tryHead
            |> Option.map (fun chunk ->
                let _, tileEntities = chunk.Deconstruct()
                tileEntities.EntityAt(0)) // 我们默认只会存在一个点

    member this.Add
        (centerId: int, chunkId: int, hexFaces: FaceComponent array, hexFaceIds: int array, neighborCenterIds: int array) =
        let unitCentroid = initUnitCentroid hexFaces
        let unitCorners = initUnitCorners hexFaces

        store
            .CreateEntity(TileComponent(centerId, chunkId, unitCentroid, unitCorners, hexFaceIds, neighborCenterIds))
            .Id
