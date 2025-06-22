namespace TO.Presenters.Commands.Geos

open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Geos

type LonLatGridDrawOnPlanet = unit -> unit

[<Interface>]
type ILonLatGridCommand =
    abstract DrawOnPlanet: LonLatGridDrawOnPlanet

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 13:28:22
module LonLatGridCommand =
    let drawOnPlanet (planet: IPlanet) (lonLatGrid: ILonLatGrid) : LonLatGridDrawOnPlanet =
        fun () -> lonLatGrid.Draw(planet.Radius + planet.MaxHeight)
