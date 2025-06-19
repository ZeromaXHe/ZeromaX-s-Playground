namespace TO.FSharp.Repos.Data.Shaders

open Godot
open TO.Abstractions.Planets
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Alias.HexSpheres.Tiles
open TO.Domains.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 20:40:06
type TileShaderData() =
    let mutable x = 0
    let mutable z = 0
    let mutable tileTexture: Image = null
    let mutable tileCivTexture: Image = null
    let mutable tileTextureData: Color array = null
    let mutable tileCivTextureData: Color array = null
    let mutable hexTileData: ImageTexture = null
    let mutable hexTileCivData: ImageTexture = null
    let transitioningTileIndices = ResizeArray<int>()
    let mutable visibilityTransitions: bool array = null

    let changePixel (img: Image) tileId data =
        img.SetPixel(tileId % img.GetWidth(), tileId / img.GetWidth(), data)

    member val ImmediateMode = false with get, set
    member val Enabled = false with get, set
    member val NeedsVisibilityReset = false with get, set
    member this.TransitioningTileIndices = transitioningTileIndices
    member this.HexTileData = hexTileData
    member this.HexTileCivData = hexTileCivData
    member this.TileTexture = tileTexture
    member this.TileCivTexture = tileCivTexture

    member this.Initialize(planet: IPlanet) =
        // 地块数等于 20 * div * div / 2 + 2 = 10 * div ^ 2 + 2
        x <- planet.Divisions * 5
        z <- planet.Divisions * 2 + 1 // 十二个五边形会导致余数
        tileTexture <- Image.CreateEmpty(x, z, false, Image.Format.Rgba8)
        tileCivTexture <- Image.CreateEmpty(x, z, false, Image.Format.Rgba8)
        hexTileData <- ImageTexture.CreateFromImage(tileTexture)
        hexTileCivData <- ImageTexture.CreateFromImage(tileCivTexture)
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexTileData, Variant.CreateFrom(hexTileData))
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexTileCivData, Variant.CreateFrom(hexTileCivData))

        RenderingServer.GlobalShaderParameterSet(
            GlobalShaderParam.HexTileDataTexelSize,
            Vector4(1f / float32 x, 1f / float32 z, float32 x, float32 z)
        )

        if tileTextureData = null || tileTextureData.Length <> x * z then
            tileTextureData <- Array.zeroCreate <| x * z
            tileCivTextureData <- Array.zeroCreate <| x * z
            visibilityTransitions <- Array.zeroCreate <| x * z
        else
            for i in 0 .. tileTextureData.Length - 1 do
                tileTextureData[i] <- Color(0f, 0f, 0f, 0f)
                tileCivTextureData[i] <- Colors.Black
                visibilityTransitions[i] <- false

        transitioningTileIndices.Clear()
        this.Enabled <- true

    member this.RefreshCiv (tileCountId: TileCountId) (civColor: Color) =
        let countId = tileCountId.CountId
        tileCivTextureData[countId] <- civColor
        changePixel tileCivTexture countId tileCivTextureData[countId]
        this.Enabled <- true

    member this.RefreshTerrain (planet: IPlanet) (tileCountId: TileCountId) (tileValue: TileValue) =
        let countId = tileCountId.CountId
        let mutable data = tileTextureData[countId]

        data.B8 <-
            if tileValue.IsUnderwater then
                int <| tileValue.WaterSurfaceY planet.UnitHeight * 255f / planet.MaxHeight
            else
                0

        data.A8 <- tileValue.TerrainTypeIndex
        tileTextureData[countId] <- data
        changePixel tileTexture countId data
        this.Enabled <- true

    member this.RefreshVisibility
        (tileId: TileId)
        (tileCountId: TileCountId)
        (tileFlag: TileFlag)
        (tileVisibility: TileVisibility)
        =
        let countId = tileCountId.CountId

        if this.ImmediateMode then
            tileTextureData[countId].R8 <- if tileVisibility.IsVisible tileFlag then 255 else 0
            tileTextureData[countId].G8 <- if tileFlag.IsExplored then 255 else 0
            changePixel tileTexture countId tileTextureData[countId]
        elif not visibilityTransitions[countId] then
            visibilityTransitions[countId] <- true
            transitioningTileIndices.Add tileId

        this.Enabled <- true

    member this.UpdateTileData
        (tileCountId: TileCountId)
        (tileFlag: TileFlag)
        (tileVisibility: TileVisibility)
        (deltaSpeed: int)
        =
        let countId = tileCountId.CountId
        let mutable data = tileTextureData[countId]
        let mutable stillUpdating = false

        if tileFlag.IsExplored && data.G8 < 255 then
            stillUpdating <- true
            let t = data.G8 + deltaSpeed
            data.G8 <- if t >= 255 then 255 else t

        if tileVisibility.IsVisible tileFlag then
            if data.R8 < 255 then
                stillUpdating <- true
                let t = data.R8 + deltaSpeed
                data.R8 <- if t >= 255 then 255 else t
        elif data.R8 > 0 then
            stillUpdating <- true
            let t = data.R8 - deltaSpeed
            data.R8 <- if t <= 0 then 0 else t

        if not stillUpdating then
            visibilityTransitions[countId] <- false

        tileTextureData[countId] <- data
        changePixel tileTexture countId data
        stillUpdating

    member this.ViewElevationChanged (planet: IPlanet) (tileCountId: TileCountId) (tileValue: TileValue) =
        let countId = tileCountId.CountId

        tileTextureData[countId].B8 <-
            if tileValue.IsUnderwater then
                int <| tileValue.WaterSurfaceY planet.UnitHeight * 255f / planet.MaxHeight
            else
                0

        changePixel tileTexture countId tileTextureData[countId]
        this.NeedsVisibilityReset <- true
        this.Enabled <- true

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:32:19
[<Interface>]
type ITileShaderData =
    abstract TileShaderData: TileShaderData
