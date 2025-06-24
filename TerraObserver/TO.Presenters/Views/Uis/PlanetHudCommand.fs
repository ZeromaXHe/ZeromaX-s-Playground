namespace TO.Presenters.Views.Uis

open System
open Godot
open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Cameras
open TO.Abstractions.Views.Uis
open TO.Domains.Structs.HexSphereGrids
open TO.Domains.Utils.Commons
open TO.Presenters.Views.Cameras

type OnOrbitCameraRigMoved = Vector3 -> float32 -> unit
type OnOrbitCameraRigTransformed = Transform3D -> unit
type InitElevationAndWaterVSlider = unit -> unit
type UpdateRadiusLineEdit = string -> unit
type UpdateDivisionLineEdit = bool -> string -> unit

[<Interface>]
type IPlanetHudCommand =
    abstract OnOrbitCameraRigMoved: OnOrbitCameraRigMoved
    abstract OnOrbitCameraRigTransformed: OnOrbitCameraRigTransformed
    abstract InitElevationAndWaterVSlider: InitElevationAndWaterVSlider
    abstract UpdateRadiusLineEdit: UpdateRadiusLineEdit
    abstract UpdateDivisionLineEdit: UpdateDivisionLineEdit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 19:57:22
module PlanetHudCommand =
    let onOrbitCameraRigMoved (planetHud: IPlanetHud) : OnOrbitCameraRigMoved =
        fun pos _ ->
            let lonLat = LonLatCoords.From pos
            planetHud.CamLonLatLabel.Text <- $"相机位置：{lonLat}"

    let onOrbitCameraRigTransformed
        (orbitCameraRig: IOrbitCameraRig)
        (planetHud: IPlanetHud)
        : OnOrbitCameraRigTransformed =
        fun transform ->
            if planetHud <> null then
                let northPolePoint = Vector3.Up
                let posNormal = transform.Origin.Normalized()
                let dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere posNormal northPolePoint

                let angleToNorth =
                    transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal)

                planetHud.CompassPanel.Rotation <- angleToNorth
                let posLocal = OrbitCameraRigQuery.getFocusBasePos orbitCameraRig ()
                let lonLat = LonLatCoords.From posLocal

                match planetHud.RectMap.Material with
                | :? ShaderMaterial as rectMapMaterial ->
                    rectMapMaterial.SetShaderParameter("lon", lonLat.Longitude)
                    rectMapMaterial.SetShaderParameter("lat", lonLat.Latitude)
                    // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
                    rectMapMaterial.SetShaderParameter("angle_to_north", angleToNorth)
                | _ -> ()
    // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
    //     angleToNorth}; posNormal: {posNormal};");
    let initElevationAndWaterVSlider (planet: IPlanet) (planetHud: IPlanetHud) : InitElevationAndWaterVSlider =
        fun () ->
            // 按照指定的高程分割数量确定 UI
            planetHud.ElevationVSlider.MaxValue <- planet.ElevationStep
            planetHud.ElevationVSlider.TickCount <- planet.ElevationStep + 1
            planetHud.WaterVSlider.MaxValue <- planet.ElevationStep
            planetHud.WaterVSlider.TickCount <- planet.ElevationStep + 1

    let updateRadiusLineEdit (planet: IPlanet) (planetHud: IPlanetHud) : UpdateRadiusLineEdit =
        fun text ->
            match Single.TryParse text with
            | true, radius -> planet.Radius <- radius
            | false, _ -> ()

            planetHud.RadiusLineEdit.Text <- $"{planet.Radius:F2}"

    let updateDivisionLineEdit (planet: IPlanet) (planetHud: IPlanetHud) : UpdateDivisionLineEdit =
        fun chunky text ->
            match Int32.TryParse text with
            | true, division ->
                if chunky then
                    planet.ChunkDivisions <- division
                else
                    planet.Divisions <- division
            | false, _ -> ()

            planetHud.DivisionLineEdit.Text <- $"{planet.Divisions}"
            planetHud.ChunkDivisionLineEdit.Text <- $"{planet.ChunkDivisions}"
