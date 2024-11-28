namespace FrontEndToolFS.HexPlane

open Godot

type EdgeVertices =
    struct
        val v1: Vector3
        val v2: Vector3
        val v3: Vector3
        val v4: Vector3
        val v5: Vector3

        // new(corner1, corner2) =
        //     { v1 = corner1
        //       v2 = corner1.Lerp(corner2, 0.25f)
        //       v3 = corner1.Lerp(corner2, 0.5f)
        //       v4 = corner1.Lerp(corner2, 0.75f)
        //       v5 = corner2 }

        new(corner1, corner2, ?outerStep: float32) =
            // 可选参数。defaultArg 刚好和 Option.defaultValue 入参顺序相反
            let outerStep = defaultArg outerStep 0.25f

            { v1 = corner1
              v2 = corner1.Lerp(corner2, outerStep)
              v3 = corner1.Lerp(corner2, 0.5f)
              v4 = corner1.Lerp(corner2, 1f - outerStep)
              v5 = corner2 }

        new(v1, v2, v3, v4, v5) =
            { v1 = v1
              v2 = v2
              v3 = v3
              v4 = v4
              v5 = v5 }

        // 不确定这样好不好…… 临时这样开个口子方便生成河道
        member this.ChangeV3(v: Vector3) =
            EdgeVertices(this.v1, this.v2, v, this.v4, this.v5)

        override this.ToString() =
            $"{this.v1} {this.v2} {this.v3} {this.v4} {this.v5}"

        static member TerraceLerp (a: EdgeVertices) (b: EdgeVertices) step =
            EdgeVertices(
                HexMetrics.terraceLerp a.v1 b.v1 step,
                HexMetrics.terraceLerp a.v2 b.v2 step,
                HexMetrics.terraceLerp a.v3 b.v3 step,
                HexMetrics.terraceLerp a.v4 b.v4 step,
                HexMetrics.terraceLerp a.v5 b.v5 step
            )
    end
