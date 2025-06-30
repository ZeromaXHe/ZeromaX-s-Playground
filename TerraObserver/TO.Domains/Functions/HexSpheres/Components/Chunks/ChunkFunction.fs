namespace TO.Domains.Functions.HexSpheres.Components.Chunks

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres.Tags

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 15:40:19
module ChunkFunction =
    let chunkyTag chunky =
        if chunky then Tags.Get<TagChunk>() else Tags.Get<TagTile>()
