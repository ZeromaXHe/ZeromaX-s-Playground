namespace TO.FSharp.Services.Actions

open Friflo.Engine.ECS
open TO.FSharp.Domains.Components.HexSpheres.Tiles
open TO.FSharp.Repos.Data.Commons
open TO.FSharp.Repos.Data.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-17 13:56:17
module TileShaderService =
    let private resetVisibility<'E when 'E :> IEntityStore and 'E :> ITileShaderData> (env: 'E) =
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

    let updateData<'E when 'E :> IEntityStore and 'E :> ITileShaderData> (env: 'E) =
        fun (delta: float32) ->
            let data = env.TileShaderData
            let store = env.EntityStore
            if data.Enabled then
                if data.NeedsVisibilityReset then
                    data.NeedsVisibilityReset <- false
                    resetVisibility env

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
