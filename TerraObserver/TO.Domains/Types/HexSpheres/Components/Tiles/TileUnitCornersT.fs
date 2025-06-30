namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS
open Godot

/// 地块单位角落
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:13:06
[<Struct>]
type TileUnitCorners =
    interface IComponent
    val Length: int
    val Corner0: Vector3
    val Corner1: Vector3
    val Corner2: Vector3
    val Corner3: Vector3
    val Corner4: Vector3
    val Corner5: Vector3

    new(corners: Vector3 array) =
        if corners.Length <> 5 && corners.Length <> 6 then
            failwith "TileUnitCorners must init with length 5 or 6"

        { Length = corners.Length
          Corner0 = corners[0]
          Corner1 = corners[1]
          Corner2 = corners[2]
          Corner3 = corners[3]
          Corner4 = corners[4]
          Corner5 = if corners.Length > 5 then corners[5] else corners[0] }
