namespace TO.Domains.Functions.Hashes

open Godot
open TO.Domains.Types.Hashes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:16:29
module HexHash =
    let create () =
        // GD.Randf() 的范围是 [0f, 1f]，会取到 1f
        HexHash(GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f, GD.Randf() * 0.999f)
