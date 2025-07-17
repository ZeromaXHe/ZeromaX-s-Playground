namespace TO.Domains.Functions.Cameras

open Godot
open TO.Domains.Types.Cameras
open TO.Domains.Types.Configs

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 21:24:24
module OrbitCameraRigQuery =
    let getFocusBasePos (env: #IOrbitCameraRigQuery) : GetFocusBasePos =
        fun () -> env.OrbitCameraRig.FocusBase.GlobalPosition

    let isAutoPiloting (env: #IOrbitCameraRigQuery) : IsAutoPiloting =
        fun () -> env.OrbitCameraRig.DestinationDirection <> Vector3.Zero

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 07:28:21
module OrbitCameraRigCommand =
    let private focusBackZoom = 0.2f

    let onZoomChanged (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IOrbitCameraRigQuery) : OnZoomChanged =
        fun () ->
            let planetConfig = env.PlanetConfig
            let cameraRig = env.OrbitCameraRig

            cameraRig.FocusBackStick.Position <-
                cameraRig.FocusBackStick.Basis
                * Vector3.Back
                * Mathf.Lerp(0f, focusBackZoom * planetConfig.Radius * planetConfig.StandardScale, cameraRig.Zoom)

            let distance =
                Mathf.Lerp(
                    cameraRig.StickMinZoom,
                    cameraRig.StickMaxZoom * planetConfig.StandardScale * 2f,
                    cameraRig.Zoom
                )
                * planetConfig.Radius

            cameraRig.Stick.Position <- Vector3.Back * distance

            let angle =
                Mathf.Lerp(cameraRig.SwivelMinZoom, cameraRig.SwivelMaxZoom, cameraRig.Zoom)

            cameraRig.Swivel.RotationDegrees <- Vector3.Right * angle

    let private boxSizeMultiplier = 0.01f
    let private lightRangeMultiplier = 1f

    let onPlanetParamsChanged
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IOrbitCameraRigQuery)
        : OnPlanetParamsChanged =
        fun () ->
            let planetConfig = env.PlanetConfig
            let cameraRig = env.OrbitCameraRig
            cameraRig.FocusBase.Position <- Vector3.Forward * (planetConfig.Radius + planetConfig.MaxHeight)

            cameraRig.FocusBox.Size <-
                Vector3.One
                * planetConfig.Radius
                * boxSizeMultiplier
                * planetConfig.StandardScale

            cameraRig.BackBox.Size <-
                Vector3.One
                * planetConfig.Radius
                * boxSizeMultiplier
                * planetConfig.StandardScale

            cameraRig.Light.SpotRange <- planetConfig.Radius * lightRangeMultiplier
            cameraRig.Light.Position <- Vector3.Up * planetConfig.Radius * lightRangeMultiplier

    let reset
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IOrbitCameraRigQuery and 'E :> IOrbitCameraRigCommand)
        : Reset =
        fun () ->
            env.OnPlanetParamsChanged()
            env.OrbitCameraRig.Zoom <- 1f

    let setAutoPilot (env: #IOrbitCameraRigQuery) : SetAutoPilot =
        fun (destinationDirection: Vector3) ->
            let camRig = env.OrbitCameraRig
            let fromDirection = env.GetFocusBasePos().Normalized()

            if fromDirection.IsEqualApprox destinationDirection then
                GD.Print "设置的自动跳转位置就是当前位置"
            else
                camRig.FromDirection <- fromDirection
                camRig.DestinationDirection <- destinationDirection
                camRig.AutoPilotProgress <- 0f

    // 取消自动跳转目的地
    let cancelAutoPilot (env: #IOrbitCameraRigQuery) : CancelAutoPilot =
        fun () ->
            let camRig = env.OrbitCameraRig
            camRig.FromDirection <- Vector3.Zero
            camRig.DestinationDirection <- Vector3.Zero
            camRig.AutoPilotProgress <- 0f

    let rotateCamera (env: #IOrbitCameraRigQuery) : RotateCamera =
        fun (rotationDelta: float32) ->
            let camRig = env.OrbitCameraRig
            // 旋转
            if rotationDelta = 0f then
                false
            else
                camRig.FocusBackStick.RotateY
                <| - Mathf.DegToRad(rotationDelta * camRig.RotationSpeed)

                camRig.Zoom <- camRig.Zoom // 更新 FocusBackStick 方向
                true

    let private move
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IOrbitCameraRigQuery and 'E :> IOrbitCameraRigCommand)
        =
        fun (xDelta: float32) (zDelta: float32) (delta: float32) ->
            let planetConfig = env.PlanetConfig
            let cameraRig = env.OrbitCameraRig

            if xDelta = 0f && zDelta = 0f then
                false
            else
                let direction = (Vector3.Right * xDelta + Vector3.Back * zDelta).Normalized()
                let damping = Mathf.Max(Mathf.Abs xDelta, Mathf.Abs zDelta)

                let distance =
                    Mathf.Lerp(cameraRig.MoveSpeedMinZoom, cameraRig.MoveSpeedMaxZoom, cameraRig.Zoom)
                    * planetConfig.Radius
                    * cameraRig.AntiStuckSpeedMultiplier
                    * damping
                    * delta

                let target =
                    env.GetFocusBasePos() - cameraRig.GlobalPosition
                    + cameraRig.FocusBackStick.GlobalBasis * (direction * distance)
                // 现在在速度很慢，半径很大的时候，容易在南北极卡住（游戏开始后，只按 WS 即可走到南北极）
                // 所以检查一下按下移动键后，是否没能真正移动。如果没移动，则每帧放大速度 1.5 倍
                let prePos = env.GetFocusBasePos()
                cameraRig.LookAt(target, cameraRig.FocusBase.GlobalBasis.Z)

                cameraRig.AntiStuckSpeedMultiplier <-
                    if prePos.IsEqualApprox <| env.GetFocusBasePos() then
                        cameraRig.AntiStuckSpeedMultiplier * 1.5f
                    else
                        1f

                cameraRig.EmitMoved <| env.GetFocusBasePos() <| delta
                // 打断自动跳转
                env.CancelAutoPilot()
                true

    let private mouseMoveSensitivity = 0.01f

    let onProcessed
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IOrbitCameraRigQuery and 'E :> IOrbitCameraRigCommand)
        : OnProcessed =
        fun (delta: float32) ->
            let cameraRig = env.OrbitCameraRig
            let mutable transformed = false // 变换是否发生过改变
            // 相机自动跳转
            if env.IsAutoPiloting() then
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
                if not <| lookDir.IsEqualApprox(env.GetFocusBasePos().Normalized()) then
                    cameraRig.LookAt(lookDir, cameraRig.FocusBase.GlobalBasis.Z)
                    cameraRig.EmitMoved <| env.GetFocusBasePos() <| delta
                    transformed <- true
                // 抵达目的地，取消自动跳转
                if arrived then
                    env.CancelAutoPilot()
            // 旋转
            let rotationDelta = delta * Input.GetAxis("cam_rotate_left", "cam_rotate_right")
            transformed <- env.RotateCamera rotationDelta || transformed
            // 移动
            let mutable xDelta = Input.GetAxis("cam_move_left", "cam_move_right")
            let mutable zDelta = Input.GetAxis("cam_move_forward", "cam_move_back")
            transformed <- move env xDelta zDelta delta || transformed

            if Input.IsMouseButtonPressed MouseButton.Middle then
                xDelta <- -Input.GetLastMouseVelocity().X * mouseMoveSensitivity
                zDelta <- -Input.GetLastMouseVelocity().Y * mouseMoveSensitivity
                transformed <- move env xDelta zDelta delta || transformed

            if transformed then
                cameraRig.EmitTransformed cameraRig.CamRig.GlobalTransform delta
            // 根据相对于全局太阳光的位置，控制灯光亮度
            if cameraRig.Sun <> null then
                let lightSunAngle =
                    cameraRig.Light.GlobalPosition.AngleTo cameraRig.Sun.GlobalPosition
                // 从 60 度开始到 120 度之间，灯光亮度逐渐从 0 增加到 1
                cameraRig.Light.LightEnergy <- Mathf.Clamp((lightSunAngle - Mathf.Pi / 3f) / (Mathf.Pi / 3f), 0f, 0.1f)
