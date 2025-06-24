namespace TO.Presenters.Models.Planets

open TO.Abstractions.Models.Planets

type GetPlanetTileLen = unit -> float32

[<Interface>]
type IPlanetQuery =
    abstract GetTileLen: GetPlanetTileLen

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 13:51:24
module PlanetQuery =
    let getTileLen (planet: IPlanet) =
        fun () -> planet.Radius / float32 planet.Divisions
