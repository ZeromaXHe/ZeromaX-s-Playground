namespace TO.Presenters.Models.Planets

open Godot
open TO.Domains.Shaders
open TO.Domains.Utils.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 17:41:21
[<AbstractClass>]
type PlanetFS() =
    inherit Resource()
    let maxHeightRadiusRatio = 0.2f
    // 事件
    abstract EmitParamsChanged: unit -> unit
    // Export
    abstract Radius: float32 with get, set
    abstract Divisions: int with get, set
    abstract ChunkDivisions: int with get, set
    // 外部属性
    member val UnitHeight = 1.5f with get, set // 单位高度
    member val MaxHeight = 15f with get, set
    member val MaxHeightRatio = 0.1f with get, set
    // [Export(PropertyHint.Range, "10, 15")]
    member val ElevationStep = 10 with get, set // 这里对应含义是 Elevation 分为几级

    member this.StandardScale =
        this.Radius / HexMetrics.StandardRadius * HexMetrics.StandardDivisions
        / float32 this.Divisions
    // 默认水面高度 [Export(PropertyHint.Range, "1, 5")]
    member val DefaultWaterLevel = 5 with get, set

    member this.CalcUnitHeight() =
        this.MaxHeightRatio <- this.StandardScale * maxHeightRadiusRatio
        this.MaxHeight <- this.Radius * this.MaxHeightRatio
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.MaxHeight, this.MaxHeight)
        this.UnitHeight <- this.MaxHeight / float32 this.ElevationStep

    member this.OnParamsChanged() =
        this.CalcUnitHeight()
        this.EmitParamsChanged()
