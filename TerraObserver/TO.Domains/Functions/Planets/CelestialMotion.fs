namespace TO.Domains.Functions.Planets

open Godot
open TO.Domains.Types.Configs
open TO.Domains.Types.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 14:37:22
module CelestialMotionCommand =
    let toggleAllMotions (env: #ICelestialMotionQuery) : ToggleAllMotions =
        fun (toggle: bool) ->
            let celestialMotion = env.CelestialMotion
            celestialMotion.PlanetRevolution <- toggle
            celestialMotion.PlanetRotation <- toggle
            celestialMotion.SatelliteRevolution <- toggle
            celestialMotion.SatelliteRotation <- toggle

    let updateLunarDist (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICelestialMotionQuery) : UpdateLunarDist =
        fun () ->
            let planetConfig = env.PlanetConfig
            let celestialMotion = env.CelestialMotion

            celestialMotion.LunarDist.Position <-
                Vector3.Back
                * Mathf.Clamp(
                    planetConfig.Radius * celestialMotion.SatelliteDistRatio,
                    planetConfig.Radius * (1f + celestialMotion.SatelliteRadiusRatio),
                    800f
                )

    let updateMoonMeshRadius
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICelestialMotionQuery)
        : UpdateMoonMeshRadius =
        fun () ->
            let planet = env.PlanetConfig
            let celestialMotion = env.CelestialMotion

            match celestialMotion.Moon with
            | :? MeshInstance3D as moon ->
                match moon.Mesh with
                | :? SphereMesh as moonMesh ->
                    moonMesh.SetRadius <| planet.Radius * celestialMotion.SatelliteRadiusRatio
                    moonMesh.SetHeight <| planet.Radius * celestialMotion.SatelliteRadiusRatio * 2f
                | _ -> ()
            | _ -> ()
