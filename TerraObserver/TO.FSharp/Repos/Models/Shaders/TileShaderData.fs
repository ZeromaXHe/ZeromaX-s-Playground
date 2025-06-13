namespace TO.FSharp.Repos.Models.Shaders

open Godot
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.Constants.Shaders

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
    let mutable enabled = false
    let transitioningTileIndices = ResizeArray<int>()
    let transitionSpeed = 255f
    let mutable needsVisibilityReset = false
    let mutable visibilityTransitions: bool array = null

    member val ImmediateMode = false with get, set

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
                tileTextureData.[i] <- Color(0f, 0f, 0f, 0f)
                tileCivTextureData.[i] <- Colors.Black
                visibilityTransitions.[i] <- false

        transitioningTileIndices.Clear()
        enabled <- true