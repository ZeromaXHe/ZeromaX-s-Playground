namespace TO.Domains.Types.Shaders

open Godot
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 20:40:06
type TileShaderData() =
    member val X = 0 with get, set
    member val Z = 0 with get, set
    member val ImmediateMode = false with get, set
    member val Enabled = false with get, set
    member val TileTextureData: Color array = null with get, set
    member val TileCivTextureData: Color array = null with get, set
    member val VisibilityTransitions: bool array = null with get, set
    member val NeedsVisibilityReset = false with get, set
    member val TransitioningTileIndices = ResizeArray<int>()
    member val HexTileData: ImageTexture = null with get, set
    member val HexTileCivData: ImageTexture = null with get, set
    member val TileTexture: Image = null with get, set
    member val TileCivTexture: Image = null with get, set

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 06:22:30
[<Interface>]
type ITileShaderDataQuery =
    abstract TileShaderData: TileShaderData

type InitShaderData = int -> unit
type RefreshCiv = TileCountId -> Color -> unit
type RefreshTerrain = float32 -> float32 -> TileCountId -> TileValue -> unit
type RefreshVisibility = TileId -> TileCountId -> TileFlag -> TileVisibility -> unit
type UpdateTileShaderData = float32 -> unit
type ViewElevationChanged = float32 -> float32 -> TileCountId -> TileValue -> unit

[<Interface>]
type ITileShaderDataCommand =
    abstract InitShaderData: InitShaderData
    abstract RefreshTileShaderDataCiv: RefreshCiv
    abstract RefreshTileShaderDataTerrain: RefreshTerrain
    abstract RefreshTileShaderDataVisibility: RefreshVisibility
    abstract UpdateTileShaderData: UpdateTileShaderData
    abstract ViewElevationChanged: ViewElevationChanged
