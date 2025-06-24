namespace TO.Presenters.Views.Planets

open Godot
open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Planets

type ToggleAllMotions = bool -> unit
type UpdateLunarDist = unit -> unit
type UpdateMoonMeshRadius = unit -> unit

[<Interface>]
type ICelestialMotionCommand =
    abstract ToggleAllMotions: ToggleAllMotions
    abstract UpdateLunarDist: UpdateLunarDist
    abstract UpdateMoonMeshRadius: UpdateMoonMeshRadius

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 14:37:22
module CelestialMotionCommand =
    let toggleAllMotions (celestialMotion: ICelestialMotion) : ToggleAllMotions =
        fun (toggle: bool) ->
            celestialMotion.PlanetRevolution <- toggle
            celestialMotion.PlanetRotation <- toggle
            celestialMotion.SatelliteRevolution <- toggle
            celestialMotion.SatelliteRotation <- toggle

    let updateLunarDist (planet: IPlanet) (celestialMotion: ICelestialMotion) : UpdateLunarDist =
        fun () ->
            celestialMotion.LunarDist.Position <-
                Vector3.Back
                * Mathf.Clamp(
                    planet.Radius * celestialMotion.SatelliteDistRatio,
                    planet.Radius * (1f + celestialMotion.SatelliteRadiusRatio),
                    800f
                )

    let updateMoonMeshRadius (planet: IPlanet) (celestialMotion: ICelestialMotion) : UpdateMoonMeshRadius =
        fun () ->
            match celestialMotion.Moon with
            | :? MeshInstance3D as moon ->
                match moon.Mesh with
                | :? SphereMesh as moonMesh ->
                    moonMesh.SetRadius <| planet.Radius * celestialMotion.SatelliteRadiusRatio
                    moonMesh.SetHeight <| planet.Radius * celestialMotion.SatelliteRadiusRatio * 2f
                | _ -> ()
            | _ -> ()
