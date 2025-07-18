namespace TO.Domains.Functions.Shaders

open Godot
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:15:25
module TileShaderDataCommand =
    let initShaderData (env: #ITileShaderDataQuery) : InitShaderData =
        fun divisions ->
            let this = env.TileShaderData
            // 地块数等于 20 * div * div / 2 + 2 = 10 * div ^ 2 + 2
            this.X <- divisions * 5
            this.Z <- divisions * 2 + 1 // 十二个五边形会导致余数
            this.TileTexture <- Image.CreateEmpty(this.X, this.Z, false, Image.Format.Rgba8)
            this.TileCivTexture <- Image.CreateEmpty(this.X, this.Z, false, Image.Format.Rgba8)
            this.HexTileData <- ImageTexture.CreateFromImage(this.TileTexture)
            this.HexTileCivData <- ImageTexture.CreateFromImage(this.TileCivTexture)

            RenderingServer.GlobalShaderParameterSet(
                GlobalShaderParam.HexTileData,
                Variant.CreateFrom(this.HexTileData)
            )

            RenderingServer.GlobalShaderParameterSet(
                GlobalShaderParam.HexTileCivData,
                Variant.CreateFrom(this.HexTileCivData)
            )

            RenderingServer.GlobalShaderParameterSet(
                GlobalShaderParam.HexTileDataTexelSize,
                Vector4(1f / float32 this.X, 1f / float32 this.Z, float32 this.X, float32 this.Z)
            )

            if this.TileTextureData = null || this.TileTextureData.Length <> this.X * this.Z then
                this.TileTextureData <- Array.zeroCreate <| this.X * this.Z
                this.TileCivTextureData <- Array.zeroCreate <| this.X * this.Z
                this.VisibilityTransitions <- Array.zeroCreate <| this.X * this.Z
            else
                for i in 0 .. this.TileTextureData.Length - 1 do
                    this.TileTextureData[i] <- Color(0f, 0f, 0f, 0f)
                    this.TileCivTextureData[i] <- Colors.Black
                    this.VisibilityTransitions[i] <- false

            this.TransitioningTileIndices.Clear()
            this.Enabled <- true

    let private changePixel (img: Image) tileId data =
        img.SetPixel(tileId % img.GetWidth(), tileId / img.GetWidth(), data)

    let refreshCiv (env: #ITileShaderDataQuery) : RefreshCiv =
        fun (tileCountId: TileCountId) (civColor: Color) ->
            let this = env.TileShaderData
            let countId = tileCountId.CountId
            this.TileCivTextureData[countId] <- civColor
            changePixel this.TileCivTexture countId this.TileCivTextureData[countId]
            this.Enabled <- true

    let refreshTerrain (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ITileShaderDataQuery) : RefreshTerrain =
        fun (tileCountId: TileCountId) (tileValue: TileValue) ->
            let this = env.TileShaderData
            let countId = tileCountId.CountId
            let mutable data = this.TileTextureData[countId]
            let planetConfig = env.PlanetConfig
            let unitHeight = planetConfig.UnitHeight
            let maxHeight = planetConfig.MaxHeight

            data.B8 <-
                if TileValue.isUnderwater tileValue then
                    tileValue |> TileValue.waterSurfaceY (unitHeight * 255f / maxHeight) |> int
                else
                    0

            data.A8 <- TileValue.terrainTypeIndex tileValue
            this.TileTextureData[countId] <- data
            changePixel this.TileTexture countId data
            this.Enabled <- true

    let refreshVisibility (env: #ITileShaderDataQuery) : RefreshVisibility =
        fun (tileId: TileId) (tileCountId: TileCountId) (tileFlag: TileFlag) (tileVisibility: TileVisibility) ->
            let this = env.TileShaderData
            let countId = tileCountId.CountId

            if this.ImmediateMode then
                this.TileTextureData[countId].R8 <-
                    if TileVisibility.isVisible tileFlag tileVisibility then
                        255
                    else
                        0

                this.TileTextureData[countId].G8 <- if TileFlag.isExplored tileFlag then 255 else 0
                changePixel this.TileTexture countId this.TileTextureData[countId]
            elif not this.VisibilityTransitions[countId] then
                this.VisibilityTransitions[countId] <- true
                this.TransitioningTileIndices.Add tileId

            this.Enabled <- true

    let private updateTileData (env: #ITileShaderDataQuery) =
        fun (tileCountId: TileCountId) (tileFlag: TileFlag) (tileVisibility: TileVisibility) (deltaSpeed: int) ->
            let this = env.TileShaderData
            let countId = tileCountId.CountId
            let mutable data = this.TileTextureData[countId]
            let mutable stillUpdating = false

            if TileFlag.isExplored tileFlag && data.G8 < 255 then
                stillUpdating <- true
                let t = data.G8 + deltaSpeed
                data.G8 <- if t >= 255 then 255 else t

            if TileVisibility.isVisible tileFlag tileVisibility then
                if data.R8 < 255 then
                    stillUpdating <- true
                    let t = data.R8 + deltaSpeed
                    data.R8 <- if t >= 255 then 255 else t
            elif data.R8 > 0 then
                stillUpdating <- true
                let t = data.R8 - deltaSpeed
                data.R8 <- if t <= 0 then 0 else t

            if not stillUpdating then
                this.VisibilityTransitions[countId] <- false

            this.TileTextureData[countId] <- data
            changePixel this.TileTexture countId data
            stillUpdating

    let private resetVisibility (env: 'E when 'E :> IEntityStoreQuery and 'E :> IEntityStoreCommand) =
        env.ExecuteInCommandBuffer(fun cb ->
            env.EntityStore
                .Query<TileVisibility, TileCountId, TileFlag>()
                .ForEachEntity(fun tileVisibility tileCountId tileFlag tile ->
                    if tileVisibility.Visibility > 0 then
                        let vis = TileVisibility 0
                        cb.AddComponent<TileVisibility>(tile.Id, &vis)
                        refreshVisibility env tile.Id tileCountId tileFlag vis))

    let transitionSpeed = 255f

    let updateData (env: 'E when 'E :> IEntityStoreQuery and 'E :> ITileShaderDataQuery) : UpdateTileShaderData =
        fun (delta: float32) ->
            let this = env.TileShaderData

            if this.Enabled then
                if this.NeedsVisibilityReset then
                    this.NeedsVisibilityReset <- false
                    resetVisibility env

                let deltaSpeed = int <| delta * transitionSpeed
                let deltaSpeed = if deltaSpeed = 0 then 1 else deltaSpeed
                let store = env.EntityStore
                let mutable i = 0

                while i < this.TransitioningTileIndices.Count do
                    let tileId = this.TransitioningTileIndices[i]
                    let tile = store.GetEntityById tileId
                    let tileCountId = tile.GetComponent<TileCountId>()
                    let tileFlag = tile.GetComponent<TileFlag>()
                    let tileVisibility = tile.GetComponent<TileVisibility>()

                    if updateTileData env tileCountId tileFlag tileVisibility deltaSpeed then
                        i <- i + 1
                    else
                        let lastIdx = this.TransitioningTileIndices.Count - 1
                        this.TransitioningTileIndices[i] <- this.TransitioningTileIndices[lastIdx]
                        this.TransitioningTileIndices.RemoveAt lastIdx
                // 更新 Shader global uniform 变量（hex_cell_data）
                // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
                // RenderingServer
                //     .GlobalShaderParameterGet("hex_cell_data")
                //     .As<ImageTexture>()
                //     .Update(cellTexture)
                this.HexTileData.Update this.TileTexture
                this.HexTileCivData.Update this.TileCivTexture
                this.Enabled <- this.TransitioningTileIndices.Count > 0

    let viewElevationChanged
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ITileShaderDataQuery)
        : ViewElevationChanged =
        fun (tileCountId: TileCountId) (tileValue: TileValue) ->
            let this = env.TileShaderData
            let countId = tileCountId.CountId
            let planetConfig = env.PlanetConfig
            let unitHeight = planetConfig.UnitHeight
            let maxHeight = planetConfig.MaxHeight

            this.TileTextureData[countId].B8 <-
                if TileValue.isUnderwater tileValue then
                    tileValue |> TileValue.waterSurfaceY (unitHeight * 255f / maxHeight) |> int
                else
                    0

            changePixel this.TileTexture countId this.TileTextureData[countId]
            this.NeedsVisibilityReset <- true
            this.Enabled <- true
