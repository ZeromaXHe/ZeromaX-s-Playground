namespace TO.Presenters.Views.Geos

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 10:33:22
[<AbstractClass>]
type LonLatGridFS() =
    inherit Node3D()
    let mutable ready = false
    // 对应着色器中的 alpha_factor
    let mutable visibility = 1f
    let mutable fadeVisibility = false
    let mutable radius = 110f
    let mutable meshIns: MeshInstance3D = null
    // =====【事件】=====
    abstract EmitFixFullVisibilityChanged: bool -> unit
    // =====【Export】=====
    abstract Redraw: Callable
    abstract LongitudeInterval: int with get, set
    abstract LatitudeInterval: int with get, set
    abstract Segments: int with get, set
    abstract LineMaterial: Material with get, set
    abstract NormalLineColor: Color with get, set
    abstract DeeperLineColor: Color with get, set
    abstract DeeperLineInterval: int with get, set
    abstract TropicColor: Color with get, set
    abstract CircleColor: Color with get, set
    abstract EquatorColor: Color with get, set
    abstract Degree90LongitudeColor: Color with get, set
    abstract MeridianColor: Color with get, set
    abstract DrawTropicOfCancer: bool with get, set
    abstract DrawTropicOfCapricorn: bool with get, set
    abstract DrawArcticCircle: bool with get, set
    abstract DrawAntarcticCircle: bool with get, set
    abstract FullVisibilityTime: float32 with get, set
    abstract FullVisibility: float32 with get, set
    abstract FixFullVisibility: bool with get, set

    member this.FixFullVisibilitySetter v =
        if v then
            this.Visibility <- this.FullVisibility
            fadeVisibility <- false
            this.Show()
        else
            this.SetProcess true

        if ready then
            this.EmitFixFullVisibilityChanged v

    member this.Visibility
        with get () = visibility
        and set v =
            visibility <- Mathf.Clamp(v, 0f, this.FullVisibility)

            if ready then
                match this.LineMaterial with
                | :? ShaderMaterial as m -> m.SetShaderParameter("alpha_factor", visibility)
                | _ -> ()
    // =====【生命周期】=====
    override this._Ready() : unit =
        meshIns <- new MeshInstance3D()
        this.AddChild meshIns
        ready <- true
        // 在 _ready = true 后面，触发 setter 的着色器参数初始化
        this.Visibility <- this.FullVisibility

    override this._Process(delta: float) : unit =
        if Engine.IsEditorHint() || this.FixFullVisibility then
            this.SetProcess false // 编辑器下以及锁定显示时，无需处理显隐
        else
            if fadeVisibility then
                this.Visibility <- this.Visibility - float32 delta / this.FullVisibilityTime

            fadeVisibility <- true

            if this.Visibility <= 0f then
                this.Visibility <- 0f
                this.Hide()
                this.SetProcess false
    member this.ToggleFixFullVisibility toggle =
        this.FixFullVisibility <- toggle

    member this.OnCameraMoved(pos: Vector3, delta: float32) =
        if this.FixFullVisibility then
            () // 锁定显示时不处理事件
        else
            this.Show()
            this.Visibility <- this.Visibility + delta / this.FullVisibilityTime
            fadeVisibility <- false
            this.SetProcess true

    member this.Draw(r: float32) =
        radius <- r
        this.DoDraw()

    member this.DoDraw() =
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin Mesh.PrimitiveType.Lines

        for i in -180 + this.LongitudeInterval .. this.LongitudeInterval .. 180 do
            let longitudeRadian = Mathf.DegToRad(float32 i)

            let color =
                if i = 0 || i = 180 then
                    this.MeridianColor
                elif i = -90 || i = 90 then
                    this.Degree90LongitudeColor
                elif i % (this.LongitudeInterval * this.DeeperLineInterval) = 0 then
                    this.DeeperLineColor
                else
                    this.NormalLineColor

            this.DrawLongitude surfaceTool longitudeRadian color

        for i in -90 + this.LatitudeInterval .. this.LatitudeInterval .. 90 do
            let latitudeRadian = Mathf.DegToRad(float32 i)

            let color =
                if i = 0 then
                    this.EquatorColor
                elif i % (this.LatitudeInterval * this.DeeperLineInterval) = 0 then
                    this.CircleColor
                else
                    this.NormalLineColor

            this.DrawLatitude(surfaceTool, latitudeRadian, color)
        // 北极圈：北纬 66°34′
        if this.DrawArcticCircle then
            this.DrawLatitude(surfaceTool, Mathf.DegToRad 66.567f, this.CircleColor, true)
        // 北回归线：北纬 23°26′
        if this.DrawTropicOfCancer then
            this.DrawLatitude(surfaceTool, Mathf.DegToRad 23.433f, this.TropicColor, true)
        // 南回归线：南纬 23°26′
        if this.DrawTropicOfCapricorn then
            this.DrawLatitude(surfaceTool, Mathf.DegToRad -23.433f, this.TropicColor, true)
        // 南极圈：南纬 66°34′
        if this.DrawAntarcticCircle then
            this.DrawLatitude(surfaceTool, Mathf.DegToRad -66.567f, this.CircleColor, true)

        surfaceTool.SetMaterial this.LineMaterial
        meshIns.Mesh <- surfaceTool.Commit()

    /// <summary>
    /// 绘制指定经线
    /// </summary>
    /// <param name="surfaceTool"></param>
    /// <param name="longitudeRadian">经度转为弧度制后的表示，+ 代表西经，- 代表东经（顺时针方向）</param>
    /// <param name="color"></param>
    member private this.DrawLongitude (surfaceTool: SurfaceTool) (longitudeRadian: float32) (color: Color) =
        let equatorDirection =
            Vector3(Mathf.Cos longitudeRadian, 0f, Mathf.Sin longitudeRadian)
        // 北面
        this.Draw90Degrees(surfaceTool, color, equatorDirection, Vector3.Up)
        // 南面
        this.Draw90Degrees(surfaceTool, color, equatorDirection, Vector3.Down)

    /// <summary>
    /// 绘制指定纬线
    /// </summary>
    /// <param name="surfaceTool"></param>
    /// <param name="latitudeRadian">维度转为弧度制后的表示，+ 表示北纬，- 表示南纬（上方取正）</param>
    /// <param name="color"></param>
    /// <param name="dash">是否按虚线绘制</param>
    member private this.DrawLatitude(surfaceTool: SurfaceTool, latitudeRadian: float32, color: Color, ?dash: bool) =
        let dash = defaultArg dash false
        let cos = Mathf.Cos latitudeRadian // 对应相比赤道应该缩小的比例
        let sin = Mathf.Sin latitudeRadian // 对应固定的高度
        // 本初子午线
        let primeMeridianDirection = Vector3(cos, 0f, 0f)
        // 西经 90 度
        let west90Direction = Vector3(0f, 0f, cos)
        this.Draw90Degrees(surfaceTool, color, primeMeridianDirection, west90Direction, Vector3.Up * sin, dash)
        // 对向子午线
        let antiMeridianDirection = Vector3(-cos, 0f, 0f)
        this.Draw90Degrees(surfaceTool, color, west90Direction, antiMeridianDirection, Vector3.Up * sin, dash)
        // 东经 90 度
        let east90Direction = Vector3(0f, 0f, -cos)
        this.Draw90Degrees(surfaceTool, color, antiMeridianDirection, east90Direction, Vector3.Up * sin, dash)
        this.Draw90Degrees(surfaceTool, color, east90Direction, primeMeridianDirection, Vector3.Up * sin, dash)

    member private this.Draw90Degrees
        (surfaceTool: SurfaceTool, color: Color, fromV: Vector3, toV: Vector3, ?origin: Vector3, ?dash: bool)
        =
        let origin = defaultArg origin Vector3.Zero
        let dash = defaultArg dash false
        let mutable preDirection = fromV

        for i in 1 .. this.Segments do
            let currentDirection = fromV.Slerp(toV, float32 i / float32 this.Segments)

            if not dash || i % 2 = 0 then
                surfaceTool.SetColor color
                // 【切记】：Mesh.PrimitiveType.Lines 绘制方式时，必须自己指定法线！！！否则没颜色
                surfaceTool.SetNormal <| origin + preDirection
                surfaceTool.AddVertex <| (origin + preDirection) * radius
                surfaceTool.SetColor color
                surfaceTool.SetNormal <| origin + currentDirection
                surfaceTool.AddVertex <| (origin + currentDirection) * radius

            preDirection <- currentDirection
