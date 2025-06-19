namespace TO.Domains.Utils.Commons

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 19:30:19
module MeshUtil =
    let triArr<'T> (c: 'T) = [| c; c; c |]
    let quadArr<'T> (c: 'T) = [| c; c; c; c |]
    let quad2Arr<'T> (c1: 'T) (c2: 'T) = [| c1; c1; c2; c2 |]

    let quadUv (uMin: float32) (uMax: float32) (vMin: float32) (vMax: float32) =
        [| Vector2(uMin, vMin)
           Vector2(uMax, vMin)
           Vector2(uMin, vMax)
           Vector2(uMax, vMax) |]
