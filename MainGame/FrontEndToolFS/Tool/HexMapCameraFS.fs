namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot
open Microsoft.FSharp.Core

type HexMapCameraFS() as this =
    inherit Node3D()

    // 这个控制摄像头的方式挺有意思的，保证根节点在拍摄的焦点上
    let _swivel = lazy this.GetNode<Node3D>("Swivel")
    let _stick = lazy this.GetNode<Node3D>("Swivel/Stick")
    let _camera = lazy this.GetNode<Camera3D>("Swivel/Stick/Camera3D")

    let mutable zoom = 1.0

    let adjustZoom delta =
        zoom <- Mathf.Clamp(zoom + delta, 0.0, 1.0)
        let distance = Mathf.Lerp(this._stickMinZoom, this._stickMaxZoom, zoom)
        _stick.Value.Position <- Vector3.Back * float32 distance
        let angle = Mathf.Lerp(this._swivelMinZoom, this._swivelMaxZoom, zoom)
        _swivel.Value.RotationDegrees <- Vector3.Right * float32 angle

    let adjustRotation delta =
        let y = delta * this._rotationSpeed + this.RotationDegrees.Y

        let degree =
            if y < 0f then y + 360f
            elif y > 360f then y - 360f
            else y

        this.RotationDegrees <- Vector3(0.0f, degree, 0.0f)

    let clampPosition (pos: Vector3) =
        let xMax = (float32 this._grid.cellCountX - 0.5f) * HexMetrics.innerDiameter
        let zMax = float32 (this._grid.cellCountZ - 1) * 1.5f * HexMetrics.outerRadius
        let x = Mathf.Clamp(pos.X, 0f, xMax)
        let z = Mathf.Clamp(pos.Z, 0f, zMax)
        Vector3(x, pos.Y, z)

    let wrapPosition (pos: Vector3) =
        let mutable pos = pos
        let width = float32 this._grid.cellCountX * HexMetrics.innerDiameter

        while pos.X < 0f do
            pos.X <- pos.X + width

        while pos.X > width do
            pos.X <- pos.X - width

        let zMax = float32 (this._grid.cellCountZ - 1) * 1.5f * HexMetrics.outerRadius
        pos.Z <- Mathf.Clamp(pos.Z, 0f, zMax)
        this._grid.CenterMap pos.X
        pos

    // Unity 的 z 方向和 Godot 相反，所以这里都是正的
    member val _stickMinZoom = 250.0 with get, set
    member val _stickMaxZoom = 45.0 with get, set
    member val _swivelMinZoom = -90.0 with get, set
    member val _swivelMaxZoom = -45.0 with get, set
    member val _moveSpeedMinZoom = 400.0 with get, set
    member val _moveSpeedMaxZoom = 100.0 with get, set
    member val _rotationSpeed = 180.0f with get, set

    [<DefaultValue>]
    val mutable _grid: HexGridFS

    // 因为这些静态变量的存在，不能把摄像机放在 HexGrid 这种 [Tool] 节点下，否则又会程序集卸载失败……
    // 目前摄像机是直接放在 HexMapEditor 里面的 HexGrid 下
    static member val instance: HexMapCameraFS option = None with get, set

    static member Locked
        with set value = HexMapCameraFS.instance.Value.SetProcess <| not value

    static member ValidatePosition() =
        HexMapCameraFS.instance.Value.AdjustPosition 0f 0f 0.0

    member this.AdjustPosition (xDelta: float32) (zDelta: float32) (delta: float) =
        let direction = (this.Basis.X * xDelta + this.Basis.Z * zDelta).Normalized()

        let damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta))

        let distance =
            (float32 <| Mathf.Lerp(this._moveSpeedMinZoom, this._moveSpeedMaxZoom, zoom))
            * damping
            * (float32 delta)
        // GD.Print $"移动摄像头 x:{xDelta}, z:{zDelta} delta:{delta}, distance:{distance}, damping:{damping}, direction:{direction}"
        let position = this.Position + direction * distance

        this.Position <-
            if this._grid.wrapping then
                wrapPosition position
            else
                clampPosition position

    override this._Ready() =
        HexMapCameraFS.instance <- Some this
        // 需要保证 grid 初始化地图完毕后再调用
        this._grid.add_Ready (fun _ -> HexMapCameraFS.ValidatePosition())

    override this._UnhandledInput e =
        match e with
        | :? InputEventMouseButton as b when
            b.ButtonIndex = MouseButton.WheelDown || b.ButtonIndex = MouseButton.WheelUp
            ->
            // 缩放
            let zoomDelta =
                if b.ButtonIndex = MouseButton.WheelUp then
                    0.025f * b.Factor
                else
                    -0.025f * b.Factor
            // GD.Print $"缩放摄像头 z:{zoomDelta} {b.Pressed} {b.ButtonIndex}" // 一次滚动会有一对 Pressed True/False
            adjustZoom <| float zoomDelta
        | _ -> ()

    override this._Process delta =
        // if
        //     Input.IsMouseButtonPressed MouseButton.WheelDown
        //     || Input.IsMouseButtonPressed MouseButton.WheelUp
        // then
        //     GD.Print "滚轮 Pressed" // 并不会打印，无法通过这个方法获取滚轮滚动信息

        // 旋转
        let rotationDelta = float32 delta * Input.GetAxis("rotate_left", "rotate_right")

        if rotationDelta <> 0.0f then
            // GD.Print $"旋转摄像头 r:{rotationDelta}"
            adjustRotation rotationDelta
        // 移动
        let xDelta = Input.GetAxis("move_left", "move_right")
        let zDelta = Input.GetAxis("move_forward", "move_back")

        if xDelta <> 0.0f || zDelta <> 0.0f then
            this.AdjustPosition xDelta zDelta delta
