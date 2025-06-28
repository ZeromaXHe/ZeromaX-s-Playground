namespace TO.Repos.Data.Shaders

open Godot
open TO.Domains.Components.HexSpheres.Tiles
open TO.Domains.Alias.HexSpheres.Tiles
open TO.Domains.Shaders

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
