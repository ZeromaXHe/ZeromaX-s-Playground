namespace TO.Domains.Types.Hashes

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
