namespace TO.Domains.Types.HexMetrics

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:08:29
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-10 23:22:10
[<Struct>]
type EdgeVertices =
    val mutable V1: Vector3
    val mutable V2: Vector3
    val mutable V3: Vector3
    val mutable V4: Vector3
    val mutable V5: Vector3

    new(corner1, corner2, ?outerStep: float32) =
        let outerStep = defaultArg outerStep 0.25f

        { V1 = corner1
          V2 = corner1.Lerp(corner2, outerStep)
          V3 = corner1.Lerp(corner2, 0.5f)
          V4 = corner1.Lerp(corner2, 1.0f - outerStep)
          V5 = corner2 }
