namespace TO.Presenters.Views.Cameras

open Godot
open TO.Abstractions.Views.Cameras
open TO.Abstractions.Models.Planets
open TO.Presenters.Views.Cameras

type OnZoomChanged = unit -> unit
type OnPlanetParamsChanged = unit -> unit
type Reset = unit -> unit
type SetAutoPilot = Vector3 -> unit
type CancelAutoPilot = unit -> unit
type RotateCamera = float32 -> bool
type OnProcessed = float32 -> unit

[<Interface>]
type IOrbitCameraRigCommand =
    abstract OnZoomChanged: OnZoomChanged
    abstract OnPlanetParamsChanged: OnPlanetParamsChanged
    abstract Reset: Reset
    abstract SetAutoPilot: SetAutoPilot
    abstract CancelAutoPilot: CancelAutoPilot
    abstract RotateCamera: RotateCamera
    abstract OnProcessed: OnProcessed

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 07:28:21
module OrbitCameraRigCommand =
    let private focusBackZoom = 0.2f

    let onZoomChanged (planet: IPlanet) (cameraRig: IOrbitCameraRig) : OnZoomChanged =
        fun () ->
            cameraRig.FocusBackStick.Position <-
                cameraRig.FocusBackStick.Basis
                * Vector3.Back
                * Mathf.Lerp(0f, focusBackZoom * planet.Radius * planet.StandardScale, cameraRig.Zoom)

            let distance =
                Mathf.Lerp(cameraRig.StickMinZoom, cameraRig.StickMaxZoom * planet.StandardScale * 2f, cameraRig.Zoom)
                * planet.Radius

            cameraRig.Stick.Position <- Vector3.Back * distance

            let angle =
                Mathf.Lerp(cameraRig.SwivelMinZoom, cameraRig.SwivelMaxZoom, cameraRig.Zoom)

            cameraRig.Swivel.RotationDegrees <- Vector3.Right * angle

    let private boxSizeMultiplier = 0.01f
    let private lightRangeMultiplier = 1f

    let onPlanetParamsChanged (planet: IPlanet) (cameraRig: IOrbitCameraRig) : OnPlanetParamsChanged =
        fun () ->
            GD.Print("onPlanetParamsChanged")
            cameraRig.FocusBase.Position <- Vector3.Forward * (planet.Radius + planet.MaxHeight)
            cameraRig.FocusBox.Size <- Vector3.One * planet.Radius * boxSizeMultiplier * planet.StandardScale
            cameraRig.BackBox.Size <- Vector3.One * planet.Radius * boxSizeMultiplier * planet.StandardScale
            cameraRig.Light.SpotRange <- planet.Radius * lightRangeMultiplier
            cameraRig.Light.Position <- Vector3.Up * planet.Radius * lightRangeMultiplier

    let reset (planet: IPlanet) (cameraRig: IOrbitCameraRig) : Reset =
        fun () ->
            onPlanetParamsChanged planet cameraRig ()
            cameraRig.Zoom <- 1f

    let setAutoPilot (camRig: IOrbitCameraRig) : SetAutoPilot =
        fun (destinationDirection: Vector3) ->
            let fromDirection = (OrbitCameraRigQuery.getFocusBasePos camRig ()).Normalized()

            if fromDirection.IsEqualApprox destinationDirection then
                GD.Print "设置的自动跳转位置就是当前位置"
            else
                camRig.FromDirection <- fromDirection
                camRig.DestinationDirection <- destinationDirection
                camRig.AutoPilotProgress <- 0f

    // 取消自动跳转目的地
    let cancelAutoPilot (camRig: IOrbitCameraRig) : CancelAutoPilot =
        fun () ->
            camRig.FromDirection <- Vector3.Zero
            camRig.DestinationDirection <- Vector3.Zero
            camRig.AutoPilotProgress <- 0f

    let rotateCamera (camRig: IOrbitCameraRig) : RotateCamera =
        fun (rotationDelta: float32) ->
            // 旋转
            if rotationDelta = 0f then
                false
            else
                camRig.FocusBackStick.RotateY
                <| - Mathf.DegToRad(rotationDelta * camRig.RotationSpeed)

                camRig.Zoom <- camRig.Zoom // 更新 FocusBackStick 方向
                true

    let private move (planet: IPlanet) (cameraRig: IOrbitCameraRig) =
        fun (xDelta: float32) (zDelta: float32) (delta: float32) ->
            if xDelta = 0f && zDelta = 0f then
                false
            else
                let direction = (Vector3.Right * xDelta + Vector3.Back * zDelta).Normalized()
                let damping = Mathf.Max(Mathf.Abs xDelta, Mathf.Abs zDelta)

                let distance =
                    Mathf.Lerp(cameraRig.MoveSpeedMinZoom, cameraRig.MoveSpeedMaxZoom, cameraRig.Zoom)
                    * planet.Radius
                    * cameraRig.AntiStuckSpeedMultiplier
                    * damping
                    * delta

                let target =
                    OrbitCameraRigQuery.getFocusBasePos cameraRig () - cameraRig.GlobalPosition
                    + cameraRig.FocusBackStick.GlobalBasis * (direction * distance)
                // 现在在速度很慢，半径很大的时候，容易在南北极卡住（游戏开始后，只按 WS 即可走到南北极）
                // 所以检查一下按下移动键后，是否没能真正移动。如果没移动，则每帧放大速度 1.5 倍
                let prePos = OrbitCameraRigQuery.getFocusBasePos cameraRig ()
                cameraRig.LookAt(target, cameraRig.FocusBase.GlobalBasis.Z)

                cameraRig.AntiStuckSpeedMultiplier <-
                    if prePos.IsEqualApprox <| OrbitCameraRigQuery.getFocusBasePos cameraRig () then
                        cameraRig.AntiStuckSpeedMultiplier * 1.5f
                    else
                        1f

                cameraRig.EmitMoved(OrbitCameraRigQuery.getFocusBasePos cameraRig (), delta)
                // 打断自动跳转
                cancelAutoPilot cameraRig ()
                true

    let private mouseMoveSensitivity = 0.01f

    let onProcessed (planet: IPlanet) (cameraRig: IOrbitCameraRig) : OnProcessed =
        fun (delta: float32) ->
            let mutable transformed = false // 变换是否发生过改变
            // 相机自动跳转
            if OrbitCameraRigQuery.isAutoPiloting cameraRig () then
                // // 点击左键，打断相机自动跳转（不能这样写，因为点击小地图时左键也是按下的）
                // if Input.IsMouseButtonPressed MouseButton.Left then
                //     cameraRig.CancelAutoPilot()
                cameraRig.AutoPilotProgress <- cameraRig.AutoPilotProgress + cameraRig.AutoPilotSpeed * delta

                let arrived =
                    if cameraRig.AutoPilotProgress >= 1f then
                        cameraRig.AutoPilotProgress <- 1f
                        true
                    else
                        false

                let lookDir =
                    cameraRig.FromDirection.Slerp(cameraRig.DestinationDirection, cameraRig.AutoPilotProgress)
                // 有可能出现一帧内移动距离过短无法 LookAt 的情况
                if
                    not
                    <| lookDir.IsEqualApprox((OrbitCameraRigQuery.getFocusBasePos cameraRig ()).Normalized())
                then
                    cameraRig.LookAt(lookDir, cameraRig.FocusBase.GlobalBasis.Z)
                    cameraRig.EmitMoved(OrbitCameraRigQuery.getFocusBasePos cameraRig (), delta)
                    transformed <- true
                // 抵达目的地，取消自动跳转
                if arrived then
                    cancelAutoPilot cameraRig ()
            // 旋转
            let rotationDelta = delta * Input.GetAxis("cam_rotate_left", "cam_rotate_right")
            transformed <- rotateCamera cameraRig rotationDelta || transformed
            // 移动
            let mutable xDelta = Input.GetAxis("cam_move_left", "cam_move_right")
            let mutable zDelta = Input.GetAxis("cam_move_forward", "cam_move_back")
            transformed <- move planet cameraRig xDelta zDelta delta || transformed

            if Input.IsMouseButtonPressed MouseButton.Middle then
                xDelta <- -Input.GetLastMouseVelocity().X * mouseMoveSensitivity
                zDelta <- -Input.GetLastMouseVelocity().Y * mouseMoveSensitivity
                transformed <- move planet cameraRig xDelta zDelta delta || transformed

            if transformed then
                cameraRig.EmitTransformed(cameraRig.CamRig.GlobalTransform, delta)
            // 根据相对于全局太阳光的位置，控制灯光亮度
            if cameraRig.Sun <> null then
                let lightSunAngle =
                    cameraRig.Light.GlobalPosition.AngleTo cameraRig.Sun.GlobalPosition
                // 从 60 度开始到 120 度之间，灯光亮度逐渐从 0 增加到 1
                cameraRig.Light.LightEnergy <- Mathf.Clamp((lightSunAngle - Mathf.Pi / 3f) / (Mathf.Pi / 3f), 0f, 0.1f)
