namespace TO.Domains.Components.HexSpheres.Tiles

open Friflo.Engine.ECS
open Godot
open TO.Domains.Utils.Commons

/// 地块单位重心（顶点坐标的算术平均）
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:12:06
[<Struct>]
type TileUnitCentroid =
    interface IComponent
    val UnitCentroid: Vector3
    new(unitCentroid: Vector3) = { UnitCentroid = unitCentroid }

    member this.Scaled(radius: float32) = this.UnitCentroid * radius
    member this.GetCornerByFaceCenter(faceCenter: Vector3, ?radius: float32, ?size: float32) =
        let radius = defaultArg radius 1f
        let size = defaultArg size 1f
        Math3dUtil.ProjectToSphere(this.UnitCentroid.Lerp(faceCenter, size), radius)
