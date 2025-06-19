namespace TO.FSharp.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Enums.Tiles
open TO.Domains.Functions.HexSpheres
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Faces
open TO.Domains.Alias.HexSpheres.Points
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.Shaders

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
    let private resetVisibility (env: 'E when 'E :> IEntityStore and 'E :> ITileShaderData) =
        let cb = env.EntityStore.GetCommandBuffer()

        env.EntityStore
            .Query<TileCountId, TileFlag, TileVisibility>()
            .ForEachEntity(fun tileCountId tileFlag tileVisibility tileEntity ->
                if tileVisibility.Visibility > 0 then
                    let vis = TileVisibility 0
                    cb.AddComponent(tileEntity.Id, &vis)
                    env.TileShaderData.RefreshVisibility tileEntity.Id tileCountId tileFlag tileVisibility)

        cb.Playback()
        cb.Clear()

    let transitionSpeed = 255f

    let updateShaderData (dataEnv: 'DE when 'DE :> IEntityStore and 'DE :> ITileShaderData) =
        fun (delta: float32) ->
            let data = dataEnv.TileShaderData
            let store = dataEnv.EntityStore

            if data.Enabled then
                if data.NeedsVisibilityReset then
                    data.NeedsVisibilityReset <- false
                    resetVisibility dataEnv

                let deltaSpeed = int <| delta * transitionSpeed
                let deltaSpeed = if deltaSpeed = 0 then 1 else deltaSpeed
                let mutable i = 0

                while i < data.TransitioningTileIndices.Count do
                    let tileId = data.TransitioningTileIndices[i]
                    let tile = store.GetEntityById tileId
                    let tileCountId = tile.GetComponent<TileCountId>()
                    let tileFlag = tile.GetComponent<TileFlag>()
                    let tileVisibility = tile.GetComponent<TileVisibility>()

                    if data.UpdateTileData tileCountId tileFlag tileVisibility deltaSpeed then
                        i <- i + 1
                    else
                        let lastIdx = data.TransitioningTileIndices.Count - 1
                        data.TransitioningTileIndices[i] <- data.TransitioningTileIndices[lastIdx]
                        data.TransitioningTileIndices.RemoveAt lastIdx
                // 更新 Shader global uniform 变量（hex_cell_data）
                // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
                // RenderingServer
                //     .GlobalShaderParameterGet("hex_cell_data")
                //     .As<ImageTexture>()
                //     .Update(cellTexture)
                data.HexTileData.Update data.TileTexture
                data.HexTileCivData.Update data.TileCivTexture
                data.Enabled <- data.TransitioningTileIndices.Count > 0
