namespace TO.FSharp.Services.Functions

open Friflo.Engine.ECS
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Repos.Models.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-17 13:56:17
module TileShaderService =
    let private resetVisibility (store: EntityStore) (data: TileShaderData) =
        let cb = store.GetCommandBuffer()
        store.Query<TileCountId, TileFlag, TileVisibility>().ForEachEntity
            (fun tileCountId tileFlag tileVisibility tileEntity ->
                if tileVisibility.Visibility > 0 then
                    let vis = TileVisibility 0
                    cb.AddComponent(tileEntity.Id, &vis)
                    data.RefreshVisibility tileEntity.Id tileCountId tileFlag tileVisibility)
        cb.Playback()
        cb.Clear()

    let transitionSpeed = 255f

    let updateData (store: EntityStore) (data: TileShaderData) =
        fun (delta: float32) ->
            if data.Enabled then
                if data.NeedsVisibilityReset then
                    data.NeedsVisibilityReset <- false
                    resetVisibility store data

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
