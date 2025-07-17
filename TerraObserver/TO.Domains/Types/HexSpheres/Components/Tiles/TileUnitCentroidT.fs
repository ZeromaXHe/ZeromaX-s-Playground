namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS
open Godot

/// 地块单位重心（顶点坐标的算术平均）
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:12:06
[<Struct>]
type TileUnitCentroid =
    interface IComponent
    val UnitCentroid: Vector3
    new(unitCentroid: Vector3) = { UnitCentroid = unitCentroid }
