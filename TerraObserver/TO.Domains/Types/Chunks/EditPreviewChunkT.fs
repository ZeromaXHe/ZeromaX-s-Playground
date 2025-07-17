namespace TO.Domains.Types.Chunks

open System
open System.Collections.Generic
open Godot
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-07 14:19:07
[<Interface>]
[<AllowNullLiteral>] // 因为不是 Tool，所以编辑器内可能为空；这注解还有向上传染性，所有父接口也得加…… 向下不影响
type IEditPreviewChunk =
    inherit IChunk
    // =====【Export】=====
    abstract TerrainMaterials: ShaderMaterial array

[<Interface>]
type IEditPreviewChunkQuery =
    abstract EditPreviewChunkOpt: IEditPreviewChunk option

type RefreshEditPreview = TileId Nullable -> unit

[<Interface>]
type IEditPreviewChunkCommand =
    abstract RefreshEditPreview: RefreshEditPreview
