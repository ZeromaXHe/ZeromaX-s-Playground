namespace TO.Presenters.Models.Planets

open TO.Abstractions.Models.Planets

type GetRadius = unit -> float32
type GetDivisions = unit -> int
type GetChunkDivisions = unit -> int
type GetUnitHeight = unit -> float32
type GetMaxHeight = unit -> float32
type GetStandardScale = unit -> float32
type GetTileLen = unit -> float32

[<Interface>]
type IPlanetQuery =
    abstract GetRadius: GetRadius
    abstract GetDivisions: GetDivisions
    abstract GetChunkDivisions: GetChunkDivisions
    abstract GetUnitHeight: GetUnitHeight
    abstract GetMaxHeight: GetMaxHeight
    abstract GetStandardScale: GetStandardScale
    abstract GetTileLen: GetTileLen

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 13:51:24
module PlanetQuery =
    let getRadius (planet: IPlanet) : GetRadius = fun () -> planet.Radius
    let getDivisions (planet: IPlanet) : GetDivisions = fun () -> planet.Divisions
    let getChunkDivisions (planet: IPlanet) : GetChunkDivisions = fun () -> planet.ChunkDivisions
    let getUnitHeight (planet: IPlanet) : GetUnitHeight = fun () -> planet.UnitHeight
    let getMaxHeight (planet: IPlanet) : GetMaxHeight = fun () -> planet.MaxHeight
    let getStandardScale (planet: IPlanet) : GetStandardScale = fun () -> planet.StandardScale

    let getTileLen (planet: IPlanet) : GetTileLen =
        fun () -> planet.Radius / float32 planet.Divisions
