namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open TO.Infras.Abstractions.Planets.Repos
open TO.Infras.Abstractions.Planets.Models.Faces
open TO.Infras.Abstractions.Planets.Models.Tiles
open TO.Infras.Planets.Utils

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 13:34:19
type TileRepo(store: EntityStore) =
    let initUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let initUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center

    interface ITileRepo with
        override this.TryHeadByCenterId(centerId: int) =
            // 我们默认只会存在最多一个结果
            FrifloEcsUtil.tryHeadEntity
            <| store.Query<TileComponent>().HasValue<TileComponent, int>(centerId)

        override this.Add
            (
                centerId: int,
                chunkId: int,
                hexFaces: FaceComponent array,
                hexFaceIds: int array,
                neighborCenterIds: int array
            ) =
            let unitCentroid = initUnitCentroid hexFaces
            let unitCorners = initUnitCorners hexFaces

            store
                .CreateEntity(
                    TileComponent(centerId, chunkId, unitCentroid, unitCorners, hexFaceIds, neighborCenterIds)
                )
                .Id

        override this.AllSeq() =
            FrifloEcsUtil.toComponentSeq <| store.Query<TileComponent>()

        override this.Truncate() =
            FrifloEcsUtil.truncate <| store.Query<TileComponent>()
