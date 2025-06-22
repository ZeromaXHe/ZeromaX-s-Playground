namespace TO.Presenters.Commands.Planets

open Godot
open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Planets

type CelestialMotionUpdateLunarDist = unit -> unit
type CelestialMotionUpdateMoonMeshRadius = unit -> unit

[<Interface>]
type ICelestialMotionCommand =
    abstract UpdateLunarDist: CelestialMotionUpdateLunarDist
    abstract UpdateMoonMeshRadius: CelestialMotionUpdateMoonMeshRadius

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 14:37:22
module CelestialMotionCommand =
    let updateLunarDist (planet: IPlanet) (celestialMotion: ICelestialMotion) : CelestialMotionUpdateLunarDist =
        fun () ->
            celestialMotion.LunarDist.Position <-
                Vector3.Back
                * Mathf.Clamp(
                    planet.Radius * celestialMotion.SatelliteDistRatio,
                    planet.Radius * (1f + celestialMotion.SatelliteRadiusRatio),
                    800f
                )

    let updateMoonMeshRadius
        (planet: IPlanet)
        (celestialMotion: ICelestialMotion)
        : CelestialMotionUpdateMoonMeshRadius =
        fun () ->
            match celestialMotion.Moon with
            | :? MeshInstance3D as moon ->
                match moon.Mesh with
                | :? SphereMesh as moonMesh ->
                    moonMesh.SetRadius <| planet.Radius * celestialMotion.SatelliteRadiusRatio
                    moonMesh.SetHeight <| planet.Radius * celestialMotion.SatelliteRadiusRatio * 2f
                | _ -> ()
            | _ -> ()
