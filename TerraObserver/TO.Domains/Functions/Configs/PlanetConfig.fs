namespace TO.Domains.Functions.Configs

open TO.Domains.Types.Configs

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 13:51:24
module PlanetConfigQuery =
    let getTileLen (env: #IPlanetConfigQuery) : GetTileLen =
        fun () -> env.PlanetConfig.Radius / float32 env.PlanetConfig.Divisions
