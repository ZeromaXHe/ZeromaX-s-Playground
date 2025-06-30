namespace TO.Domains.Functions.PlanetHuds

open System
open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.Maths
open TO.Domains.Types.Cameras
open TO.Domains.Types.Configs
open TO.Domains.Types.PlanetHuds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 19:57:22
module PlanetHudCommand =
    let onOrbitCameraRigMoved (env: #IPlanetHudQuery) : OnOrbitCameraRigMoved =
        fun pos _ ->
            env.PlanetHudOpt
            |> Option.iter (fun hud ->
                let lonLat = LonLatCoords.fromVector3 pos
                hud.CamLonLatLabel.Text <- $"相机位置：{lonLat}")

    let onOrbitCameraRigTransformed
        (env: 'E when 'E :> IOrbitCameraRigQuery and 'E :> IPlanetHudQuery)
        : OnOrbitCameraRigTransformed =
        fun transform ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let northPolePoint = Vector3.Up
                let posNormal = transform.Origin.Normalized()
                let dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere posNormal northPolePoint

                let angleToNorth =
                    transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal)

                planetHud.CompassPanel.Rotation <- angleToNorth
                let posLocal = env.GetFocusBasePos()
                let lonLat = LonLatCoords.fromVector3 posLocal

                match planetHud.RectMap.Material with
                | :? ShaderMaterial as rectMapMaterial ->
                    rectMapMaterial.SetShaderParameter("lon", lonLat.Longitude)
                    rectMapMaterial.SetShaderParameter("lat", lonLat.Latitude)
                    // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
                    rectMapMaterial.SetShaderParameter("angle_to_north", angleToNorth)
                | _ -> ()
            // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
            //     angleToNorth}; posNormal: {posNormal};");
            )

    let initElevationAndWaterVSlider
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery)
        : InitElevationAndWaterVSlider =
        fun () ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planet = env.PlanetConfig
                // 按照指定的高程分割数量确定 UI
                planetHud.ElevationVSlider.MaxValue <- planet.ElevationStep
                planetHud.ElevationVSlider.TickCount <- planet.ElevationStep + 1
                planetHud.WaterVSlider.MaxValue <- planet.ElevationStep
                planetHud.WaterVSlider.TickCount <- planet.ElevationStep + 1)

    let updateRadiusLineEdit (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery) : UpdateRadiusLineEdit =
        fun text ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planetConfig = env.PlanetConfig

                match Single.TryParse text with
                | true, radius -> planetConfig.Radius <- radius
                | false, _ -> ()

                planetHud.RadiusLineEdit.Text <- $"{planetConfig.Radius:F2}")

    let updateDivisionLineEdit
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery)
        : UpdateDivisionLineEdit =
        fun chunky text ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planetConfig = env.PlanetConfig

                match Int32.TryParse text with
                | true, division ->
                    if chunky then
                        planetConfig.ChunkDivisions <- division
                    else
                        planetConfig.Divisions <- division
                | false, _ -> ()

                planetHud.DivisionLineEdit.Text <- $"{planetConfig.Divisions}"
                planetHud.ChunkDivisionLineEdit.Text <- $"{planetConfig.ChunkDivisions}")
