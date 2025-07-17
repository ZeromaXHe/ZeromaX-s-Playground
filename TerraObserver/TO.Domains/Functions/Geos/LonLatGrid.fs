namespace TO.Domains.Functions.Geos

open Godot
open TO.Domains.Types.Configs
open TO.Domains.Types.Geos

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 22:28:24
module LonLatGridCommand =
    let private draw90Degrees
        (env: #ILonLatGridQuery)
        (surfaceTool: SurfaceTool)
        (color: Color)
        (fromV: Vector3)
        (toV: Vector3)
        (origin: Vector3)
        (dash: bool)
        =
        let lonLatGrid = env.LonLatGrid
        let mutable preDirection = fromV

        for i in 1 .. lonLatGrid.Segments do
            let currentDirection = fromV.Slerp(toV, float32 i / float32 lonLatGrid.Segments)

            if not dash || i % 2 = 0 then
                surfaceTool.SetColor color
                // 【切记】：Mesh.PrimitiveType.Lines 绘制方式时，必须自己指定法线！！！否则没颜色
                surfaceTool.SetNormal <| origin + preDirection
                surfaceTool.AddVertex <| (origin + preDirection) * lonLatGrid.Radius
                surfaceTool.SetColor color
                surfaceTool.SetNormal <| origin + currentDirection
                surfaceTool.AddVertex <| (origin + currentDirection) * lonLatGrid.Radius

            preDirection <- currentDirection

    /// <summary>
    /// 绘制指定纬线
    /// </summary>
    /// <param name="env"></param>
    /// <param name="surfaceTool"></param>
    /// <param name="latitudeRadian">维度转为弧度制后的表示，+ 表示北纬，- 表示南纬（上方取正）</param>
    /// <param name="color"></param>
    /// <param name="dash">是否按虚线绘制</param>
    let private drawLatitude env (surfaceTool: SurfaceTool) (latitudeRadian: float32) (color: Color) (dash: bool) =
        let cos = Mathf.Cos latitudeRadian // 对应相比赤道应该缩小的比例
        let sin = Mathf.Sin latitudeRadian // 对应固定的高度
        // 本初子午线
        let primeMeridianDirection = Vector3(cos, 0f, 0f)
        // 西经 90 度
        let west90Direction = Vector3(0f, 0f, cos)

        draw90Degrees env surfaceTool color primeMeridianDirection west90Direction
        <| Vector3.Up * sin
        <| dash
        // 对向子午线
        let antiMeridianDirection = Vector3(-cos, 0f, 0f)

        draw90Degrees env surfaceTool color west90Direction antiMeridianDirection
        <| Vector3.Up * sin
        <| dash
        // 东经 90 度
        let east90Direction = Vector3(0f, 0f, -cos)

        draw90Degrees env surfaceTool color antiMeridianDirection east90Direction
        <| Vector3.Up * sin
        <| dash

        draw90Degrees env surfaceTool color east90Direction primeMeridianDirection
        <| Vector3.Up * sin
        <| dash

    /// <summary>
    /// 绘制指定经线
    /// </summary>
    /// <param name="env"></param>
    /// <param name="surfaceTool"></param>
    /// <param name="longitudeRadian">经度转为弧度制后的表示，+ 代表西经，- 代表东经（顺时针方向）</param>
    /// <param name="color"></param>
    let private drawLongitude env (surfaceTool: SurfaceTool) (longitudeRadian: float32) (color: Color) =
        let equatorDirection =
            Vector3(Mathf.Cos longitudeRadian, 0f, Mathf.Sin longitudeRadian)
        // 北面
        draw90Degrees env surfaceTool color equatorDirection Vector3.Up Vector3.Zero false
        // 南面
        draw90Degrees env surfaceTool color equatorDirection Vector3.Down Vector3.Zero false

    let doDraw (env: #ILonLatGridQuery) : DoDraw =
        fun () ->
            let lonLatGrid = env.LonLatGrid
            let surfaceTool = new SurfaceTool()
            surfaceTool.Begin Mesh.PrimitiveType.Lines

            for i in -180 + lonLatGrid.LongitudeInterval .. lonLatGrid.LongitudeInterval .. 180 do
                let longitudeRadian = Mathf.DegToRad(float32 i)

                let color =
                    if i = 0 || i = 180 then
                        lonLatGrid.MeridianColor
                    elif i = -90 || i = 90 then
                        lonLatGrid.Degree90LongitudeColor
                    elif i % (lonLatGrid.LongitudeInterval * lonLatGrid.DeeperLineInterval) = 0 then
                        lonLatGrid.DeeperLineColor
                    else
                        lonLatGrid.NormalLineColor

                drawLongitude env surfaceTool longitudeRadian color

            for i in -90 + lonLatGrid.LatitudeInterval .. lonLatGrid.LatitudeInterval .. 90 do
                let latitudeRadian = Mathf.DegToRad(float32 i)

                let color =
                    if i = 0 then
                        lonLatGrid.EquatorColor
                    elif i % (lonLatGrid.LatitudeInterval * lonLatGrid.DeeperLineInterval) = 0 then
                        lonLatGrid.CircleColor
                    else
                        lonLatGrid.NormalLineColor

                drawLatitude env surfaceTool latitudeRadian color false
            // 北极圈：北纬 66°34′
            if lonLatGrid.DrawArcticCircle then
                drawLatitude env surfaceTool
                <| Mathf.DegToRad 66.567f
                <| lonLatGrid.CircleColor
                <| true
            // 北回归线：北纬 23°26′
            if lonLatGrid.DrawTropicOfCancer then
                drawLatitude env surfaceTool
                <| Mathf.DegToRad 23.433f
                <| lonLatGrid.TropicColor
                <| true
            // 南回归线：南纬 23°26′
            if lonLatGrid.DrawTropicOfCapricorn then
                drawLatitude env surfaceTool
                <| Mathf.DegToRad -23.433f
                <| lonLatGrid.TropicColor
                <| true
            // 南极圈：南纬 66°34′
            if lonLatGrid.DrawAntarcticCircle then
                drawLatitude env surfaceTool
                <| Mathf.DegToRad -66.567f
                <| lonLatGrid.CircleColor
                <| true

            surfaceTool.SetMaterial lonLatGrid.LineMaterial
            lonLatGrid.MeshIns.Mesh <- surfaceTool.Commit()

    let drawOnPlanet (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ILonLatGridQuery) : DrawOnPlanet =
        fun () ->
            let planetConfig = env.PlanetConfig
            env.LonLatGrid.Radius <- planetConfig.Radius + planetConfig.MaxHeight
            doDraw env ()

    let toggleFixFullVisibility (env: #ILonLatGridQuery) : ToggleFixFullVisibility =
        fun toggle -> env.LonLatGrid.FixFullVisibility <- toggle

    let onCameraMoved (env: #ILonLatGridQuery) : OnCameraMoved =
        fun _ delta ->
            let lonLatGrid = env.LonLatGrid

            if lonLatGrid.FixFullVisibility then
                () // 锁定显示时不处理事件
            else
                lonLatGrid.Show()
                lonLatGrid.Visibility <- lonLatGrid.Visibility + delta / lonLatGrid.FullVisibilityTime
                lonLatGrid.FadeVisibility <- false
                lonLatGrid.SetProcess true
