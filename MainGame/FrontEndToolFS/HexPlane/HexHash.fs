namespace FrontEndToolFS.HexPlane

open Godot

type HexHash =
    struct
        val a: float32
        val b: float32
        val c: float32
        val d: float32
        val e: float32

        new(a, b, c, d, e) = { a = a; b = b; c = c; d = d; e = e }

        static member Create() =
            // GD.Randf 的范围是 [0f, 1f]，会取到 1f
            HexHash(
                GD.Randf() * 0.999f,
                GD.Randf() * 0.999f,
                GD.Randf() * 0.999f,
                GD.Randf() * 0.999f,
                GD.Randf() * 0.999f
            )
    end
