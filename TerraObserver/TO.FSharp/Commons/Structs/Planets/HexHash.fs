namespace TO.FSharp.Commons.Structs.Planets

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 17:13:06
[<Struct>]
type HexHash =
    val A: float32
    val B: float32
    val C: float32
    val D: float32
    val E: float32

    new(a: float32, b: float32, c: float32, d: float32, e: float32) = { A = a; B = b; C = c; D = d; E = e }

    static member Create() =
        // GD.Randf() 的范围是 [0f, 1f]，会取到 1f
        HexHash(GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f)
