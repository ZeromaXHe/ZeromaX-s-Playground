namespace TO.Presenters.Commands.Uis

open System
open Godot
open TO.Abstractions.Models.Planets
open TO.Abstractions.Views.Cameras
open TO.Abstractions.Views.Uis
open TO.Domains.Structs.HexSphereGrids
open TO.Domains.Utils.Commons

type OnPlanetHudOrbitCameraRigTransformed = Transform3D -> unit
type PlanetHudInitElevationAndWaterVSlider = unit -> unit
type PlanetHudUpdateRadiusLineEdit = string -> unit
type PlanetHudUpdateDivisionLineEdit = string -> bool -> unit

[<Interface>]
type IPlanetHudCommand =
    abstract OnOrbitCameraRigTransformed: OnPlanetHudOrbitCameraRigTransformed
    abstract InitElevationAndWaterVSlider: PlanetHudInitElevationAndWaterVSlider
    abstract UpdateRadiusLineEdit: PlanetHudUpdateRadiusLineEdit
    abstract UpdateDivisionLineEdit: PlanetHudUpdateDivisionLineEdit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 19:57:22
module PlanetHudCommand =
    let onOrbitCameraRigMoved
        (orbitCameraRig: IOrbitCameraRig)
        (planetHud: IPlanetHud)
        : OnPlanetHudOrbitCameraRigTransformed =
        fun (transform: Transform3D) ->
            if planetHud <> null then
                let northPolePoint = Vector3.Up
                let posNormal = transform.Origin.Normalized()
                let dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere posNormal northPolePoint

                let angleToNorth =
                    transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal)

                planetHud.CompassPanel.Rotation <- angleToNorth
                let posLocal = orbitCameraRig.GetFocusBasePos()
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
    let initElevationAndWaterVSlider (planet: IPlanet) (planetHud: IPlanetHud) : PlanetHudInitElevationAndWaterVSlider =
        fun () ->
            if planetHud <> null then // 编辑器内会为空
                // 按照指定的高程分割数量确定 UI
                planetHud.ElevationVSlider.MaxValue <- planet.ElevationStep
                planetHud.ElevationVSlider.TickCount <- planet.ElevationStep + 1
                planetHud.WaterVSlider.MaxValue <- planet.ElevationStep
                planetHud.WaterVSlider.TickCount <- planet.ElevationStep + 1

    let updateRadiusLineEdit (planet: IPlanet) (planetHud: IPlanetHud) : PlanetHudUpdateRadiusLineEdit =
        fun (text: string) ->
            match Single.TryParse text with
            | true, radius -> planet.Radius <- radius
            | false, _ -> ()

            planetHud.UpdateRadiusLineEdit planet.Radius

    let updateDivisionLineEdit (planet: IPlanet) (planetHud: IPlanetHud) : PlanetHudUpdateDivisionLineEdit =
        fun (text: string) (chunky: bool) ->
            match Int32.TryParse text with
            | true, division ->
                if chunky then
                    planet.ChunkDivisions <- division
                else
                    planet.Divisions <- division
            | false, _ -> ()

            planetHud.UpdateDivisionLineEdit(planet.Divisions, planet.ChunkDivisions)
