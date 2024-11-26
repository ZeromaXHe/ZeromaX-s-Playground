namespace FrontEndToolFS.HexPlane

open Godot

type EdgeVertices =
    struct
        val v1: Vector3
        val v2: Vector3
        val v3: Vector3
        val v4: Vector3

        new(c1, c2) =
            { v1 = c1
              v2 = c1.Lerp(c2, 1f / 3f)
              v3 = c1.Lerp(c2, 2f / 3f)
              v4 = c2 }

        new(v1, v2, v3, v4) = { v1 = v1; v2 = v2; v3 = v3; v4 = v4 }

        static member TerraceLerp (a: EdgeVertices) (b: EdgeVertices) step =
            EdgeVertices(
                HexMetrics.terraceLerp a.v1 b.v1 step,
                HexMetrics.terraceLerp a.v2 b.v2 step,
                HexMetrics.terraceLerp a.v3 b.v3 step,
                HexMetrics.terraceLerp a.v4 b.v4 step
            )
    end
