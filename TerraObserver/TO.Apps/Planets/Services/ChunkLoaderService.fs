namespace TO.Apps.Planets.Services

open TO.Infras.Abstractions.Planets.Repos
open TO.Nodes.Abstractions.Planets.Views.ChunkManagers

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-27 15:35:27
type ChunkLoaderService(self: IChunkLoader, chunkRepo: IChunkRepo) =
    member this.InitChunkNodes() =
        let camera = self.GetViewport().GetCamera3D()
        ()
