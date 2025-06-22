namespace TO.Presenters.Views.Planets

open Godot
open TO.Domains.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 13:51:22
[<AbstractClass>]
type CelestialMotionFS() =
    inherit Node3D()
    let mutable ready = false
    // =====【on-ready】=====
    let mutable worldEnvironment: WorldEnvironment = null
    let mutable eclipticPlane: Node3D = null
    let mutable sunRevolution: Node3D = null
    let mutable lunarOrbitPlane: Node3D = null
    let mutable lunarRevolution: Node3D = null
    let mutable lunarDist: Node3D = null
    let mutable lunarObliquity: Node3D = null
    let mutable moonAxis: Node3D = null
    let mutable moonTransform: RemoteTransform3D = null
    let mutable sunTransform: RemoteTransform3D = null
    // =====【事件】=====
    abstract EmitSatelliteRadiusRatioChanged: unit -> unit
    abstract EmitSatelliteDistRatioChanged: unit -> unit
    // =====【Export】=====
    abstract RotationTimeFactor: float32 with get, set
    abstract Sun: Node3D with get, set
    abstract Moon: Node3D with get, set
    abstract PlanetRevolution: bool with get, set
    abstract PlanetRotation: bool with get, set
    abstract SatelliteRevolution: bool with get, set
    abstract SatelliteRotation: bool with get, set
    abstract StarMoveStatus: Callable

    member this.ToggleStarMoveStatus() =
        if this.PlanetRevolution then
            this.PlanetRevolution <- false
            sunRevolution.RotationDegrees <- Vector3.Up * 180f

            RenderingServer.GlobalShaderParameterSet(
                GlobalShaderParam.DirToSun,
                sunTransform.GlobalPosition.Normalized()
            )
        else
            this.PlanetRevolution <- true

    abstract PlanetMoveStatus: Callable

    member this.TogglePlanetMoveStatus() =
        if this.PlanetRotation then
            this.PlanetRotation <- false
            // 将黄道面和天空盒还原
            eclipticPlane.RotationDegrees <- Vector3.Right * eclipticPlane.RotationDegrees.X

            worldEnvironment.Environment.SkyRotation <- Vector3.Right * worldEnvironment.Environment.SkyRotation.X
        else
            this.PlanetRotation <- true

    abstract EclipticInclinationToGalactic: float32 with get, set

    member this.EclipticInclinationToGalacticSetter() =
        if ready then
            this.UpdateGalaxySkyRotation()

    abstract PlanetObliquity: float32 with get, set

    member this.PlanetObliquitySetter() =
        if ready then
            this.UpdateGalaxySkyRotation()
            this.UpdateEclipticPlaneRotation()

    abstract PlanetRevolutionSpeed: float32 with get, set
    abstract PlanetRotationSpeed: float32 with get, set
    abstract SatelliteMoveStatus: Callable

    member this.ToggleSatelliteMoveStatus() =
        if this.SatelliteRevolution || this.SatelliteRotation then
            this.SatelliteRevolution <- false
            this.SatelliteRotation <- false
            lunarRevolution.RotationDegrees <- Vector3.Up * 180f
            moonAxis.Rotation <- Vector3.Zero
        else
            this.SatelliteRevolution <- true
            this.SatelliteRotation <- true

    abstract SatelliteRadiusRatio: float32 with get, set

    member this.SatelliteRadiusRatioSetter() =
        if ready then
            this.EmitSatelliteRadiusRatioChanged()

    abstract SatelliteDistRatio: float32 with get, set

    member this.SatelliteDistRatioSetter() =
        if ready then
            this.EmitSatelliteDistRatioChanged()

    abstract SatelliteObliquity: float32 with get, set

    member this.SatelliteObliquitySetter() =
        if ready then
            this.UpdateLunarObliquityRotation()

    abstract SatelliteOrbitInclination: float32 with get, set

    member this.SatelliteOrbitInclinationSetter() =
        if ready then
            this.UpdateLunarOrbitPlaneRotation()

    abstract SatelliteRevolutionSpeed: float32 with get, set
    abstract SatelliteRotationSpeed: float32 with get, set
    // =====【属性】=====
    member this.LunarDist = lunarDist
    // =====【生命周期】=====
    override this._Ready() : unit =
        worldEnvironment <- this.GetNode<WorldEnvironment>("%WorldEnvironment")
        this.UpdateGalaxySkyRotation()
        // 天体公转自转
        eclipticPlane <- this.GetNode<Node3D>("%EclipticPlane")
        this.UpdateEclipticPlaneRotation()
        sunRevolution <- this.GetNode<Node3D>("%SunRevolution")
        lunarOrbitPlane <- this.GetNode<Node3D>("%LunarOrbitPlane")
        this.UpdateLunarOrbitPlaneRotation()
        lunarRevolution <- this.GetNode<Node3D>("%LunarRevolution")
        lunarDist <- this.GetNode<Node3D>("%LunarDist")
        lunarObliquity <- this.GetNode<Node3D>("%LunarObliquity")
        this.UpdateLunarObliquityRotation()
        moonAxis <- this.GetNode<Node3D>("%MoonAxis")
        moonTransform <- this.GetNode<RemoteTransform3D>("%MoonTransform")
        sunTransform <- this.GetNode<RemoteTransform3D>("%SunTransform")

        if this.Sun <> null then
            sunTransform.RemotePath <- sunTransform.GetPathTo(this.Sun)

        if this.Moon <> null then
            moonTransform.RemotePath <- moonTransform.GetPathTo(this.Moon)

        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.DirToSun, sunTransform.GlobalPosition.Normalized())

        ready <- true

    override this._Process(delta: float) : unit =
        if ready then
            let deltaF = float32 delta

            if this.PlanetRevolution || this.PlanetRotation then
                RenderingServer.GlobalShaderParameterSet(
                    GlobalShaderParam.DirToSun,
                    sunTransform.GlobalPosition.Normalized()
                )
                // 行星公转
                if this.PlanetRevolution then
                    sunRevolution.RotationDegrees <-
                        this.RotationTimeFactor
                        * Vector3.Up
                        * Mathf.Wrap(sunRevolution.RotationDegrees.Y + this.PlanetRevolutionSpeed * deltaF, 0f, 360f)
                // 行星自转
                if this.PlanetRotation then
                    // 用黄道面整体旋转表示所有天体相对运动（所以 deltaF 取负）
                    let eclipticRotDeg = eclipticPlane.RotationDegrees

                    eclipticPlane.RotationDegrees <-
                        Vector3.Right * eclipticRotDeg.X
                        + this.RotationTimeFactor
                          * Vector3.Up
                          * Mathf.Wrap(eclipticRotDeg.Y + this.PlanetRotationSpeed * -deltaF, 0f, 360f)
                    // 用天空盒的旋转表示银河背景的相对运动（所以 deltaF 取负）
                    let skyRotation = worldEnvironment.Environment.SkyRotation

                    worldEnvironment.Environment.SkyRotation <-
                        Vector3.Right * skyRotation.X
                        + this.RotationTimeFactor
                          * Vector3.Up
                          * Mathf.Wrap(
                              skyRotation.Y + Mathf.DegToRad(this.PlanetRotationSpeed) * -deltaF,
                              0f,
                              Mathf.Tau
                          )

            // 卫星公转
            if this.SatelliteRevolution then
                lunarRevolution.RotationDegrees <-
                    this.RotationTimeFactor
                    * Vector3.Up
                    * Mathf.Wrap(lunarRevolution.RotationDegrees.Y + this.SatelliteRevolutionSpeed * deltaF, 0f, 360f)
            // 卫星自转
            if this.SatelliteRotation then
                moonAxis.RotationDegrees <-
                    this.RotationTimeFactor
                    * Vector3.Up
                    * Mathf.Wrap(moonAxis.RotationDegrees.Y + this.SatelliteRotationSpeed * deltaF, 0f, 360f)

    member this.ToggleAllMotions(toggle: bool) =
        this.PlanetRevolution <- toggle
        this.PlanetRotation <- toggle
        this.SatelliteRevolution <- toggle
        this.SatelliteRotation <- toggle

    /// <summary>
    /// 更新银河星系天空盒的旋转角度。
    /// </summary>
    member private this.UpdateGalaxySkyRotation() =
        worldEnvironment.Environment.SkyRotation <-
            Vector3.Right
            * Mathf.DegToRad(this.PlanetObliquity - this.EclipticInclinationToGalactic)

    /// 更新月球轨道平面的旋转角度
    member private this.UpdateEclipticPlaneRotation() =
        eclipticPlane.RotationDegrees <- Vector3.Right * this.PlanetObliquity

    /// <summary>
    /// 更新黄道面的倾角。
    /// </summary>
    member private this.UpdateLunarOrbitPlaneRotation() =
        lunarOrbitPlane.RotationDegrees <- Vector3.Right * this.SatelliteOrbitInclination

    /// <summary>
    /// 更新月球倾斜角。
    /// </summary>
    member private this.UpdateLunarObliquityRotation() =
        lunarObliquity.RotationDegrees <- Vector3.Right * this.SatelliteObliquity
