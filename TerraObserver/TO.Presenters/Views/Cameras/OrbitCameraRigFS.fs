namespace TO.Presenters.Views.Cameras

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 16:11:21
[<AbstractClass>]
type OrbitCameraRigFS() =
    inherit Node3D()
    let mutable ready = false
    let mutable zoom = 1f
    // 事件
    abstract EmitZoomChanged: unit -> unit
    abstract EmitProcessed: float32 -> unit
    abstract EmitTransformed: Transform3D * float32 -> unit
    // Export
    abstract Camera: Camera3D with get, set // 设置摄像机节点
    abstract StickMinZoom: float32 with get, set
    abstract StickMaxZoom: float32 with get, set
    abstract SwivelMinZoom: float32 with get, set
    abstract SwivelMaxZoom: float32 with get, set
    abstract MoveSpeedMinZoom: float32 with get, set
    abstract MoveSpeedMaxZoom: float32 with get, set
    abstract RotationSpeed: float32 with get, set
    abstract Sun: Node3D with get, set
    abstract AutoPilotSpeed: float32 with get, set
    // 外部属性
    member val FocusBase: Node3D = null with get, set
    member val FocusBox: CsgBox3D = null with get, set
    member val FocusBackStick: Node3D = null with get, set
    member val BackBox: CsgBox3D = null with get, set
    member val Light: SpotLight3D = null with get, set
    member val Swivel: Node3D = null with get, set
    member val Stick: Node3D = null with get, set
    member val CamRig: RemoteTransform3D = null with get, set

    member this.Zoom
        with get () = zoom
        and set v =
            zoom <- v

            if ready then
                this.EmitZoomChanged()

    member val AntiStuckSpeedMultiplier = 1f with get, set // 用于防止速度过低的时候相机卡死
    member val AutoPilotProgress = 0f with get, set
    member val FromDirection = Vector3.Zero with get, set
    member val DestinationDirection = Vector3.Zero with get, set

    override this._Ready() : unit =
        this.FocusBase <- this.GetNode<Node3D>("%FocusBase")
        this.FocusBox <- this.GetNode<CsgBox3D>("%FocusBox")
        this.FocusBackStick <- this.GetNode<Node3D>("%FocusBackStick")
        this.BackBox <- this.GetNode<CsgBox3D>("%BackBox")
        this.Light <- this.GetNode<SpotLight3D>("%Light")
        this.Swivel <- this.GetNode<Node3D>("%Swivel")
        this.Stick <- this.GetNode<Node3D>("%Stick")
        this.CamRig <- this.GetNode<RemoteTransform3D>("%CamRig")

        if this.Camera <> null then
            this.CamRig.RemotePath <- this.CamRig.GetPathTo this.Camera

        ready <- true

    override this._Process(delta: float) : unit =
        if Engine.IsEditorHint() then
            this.SetProcess false
        else
            this.EmitProcessed <| float32 delta

    override this._UnhandledInput(event: InputEvent) : unit =
        if Engine.IsEditorHint() then
            ()
        else
            // 缩放
            match event with
            | :? InputEventMouseButton as e when
                e.ButtonIndex = MouseButton.WheelDown || e.ButtonIndex = MouseButton.WheelUp
                ->
                let zoomDelta =
                    0.025f * e.Factor * (if e.ButtonIndex = MouseButton.WheelUp then 1f else -1f)

                this.Zoom <- Mathf.Clamp(this.Zoom + zoomDelta, 0f, 1f)
                this.EmitTransformed(this.CamRig.GlobalTransform, float32 <| this.GetProcessDeltaTime())
            | _ -> ()

    member this.GetFocusBasePos() = this.FocusBase.GlobalPosition

    member this.SetAutoPilot(destinationDirection: Vector3) =
        let fromDirection = this.GetFocusBasePos().Normalized()

        if fromDirection.IsEqualApprox destinationDirection then
            GD.Print "设置的自动跳转位置就是当前位置"
        else
            this.FromDirection <- fromDirection
            this.DestinationDirection <- destinationDirection
            this.AutoPilotProgress <- 0f

    member this.IsAutoPiloting() =
        this.DestinationDirection <> Vector3.Zero
    // 取消自动跳转目的地
    member this.CancelAutoPilot() =
        this.FromDirection <- Vector3.Zero
        this.DestinationDirection <- Vector3.Zero
        this.AutoPilotProgress <- 0f

    member this.RotateCamera(rotationDelta: float32) =
        // 旋转
        if rotationDelta = 0f then
            false
        else
            this.FocusBackStick.RotateY
            <| - Mathf.DegToRad(rotationDelta * this.RotationSpeed)

            this.Zoom <- zoom // 更新 FocusBackStick 方向
            true
