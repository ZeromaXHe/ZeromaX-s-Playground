namespace TO.Domains.Types.Configs

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 21:05:29
type GetTileLen = unit -> float32

[<Interface>]
type IPlanetConfigQuery =
    abstract PlanetConfig: IPlanetConfig
    abstract GetTileLen: GetTileLen
