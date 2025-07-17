namespace TO.Domains.Functions.HexSpheres.Components.Faces

open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:29:29
module FaceComponent =
    let item i (this: FaceComponent) =
        match i with
        | 0 -> this.Vertex1
        | 1 -> this.Vertex2
        | 2 -> this.Vertex3
        | _ -> failwith "Invalid index"

    let isAdjacentTo (face: FaceComponent) (this: FaceComponent) =
        // F# 没有 Enumerable.Intersect，因为我们只有三个顶点，所以直接数吧
        let mutable count = 0

        for v1 in [| this.Vertex1; this.Vertex2; this.Vertex3 |] do
            for v2 in [| face.Vertex1; face.Vertex2; face.Vertex3 |] do
                if v1 = v2 then
                    count <- count + 1

        count = 2

    let getPointIdx (point: PointComponent) (this: FaceComponent) =
        if this.Vertex1 = point.Position then 0
        elif this.Vertex2 = point.Position then 1
        elif this.Vertex3 = point.Position then 2
        else -1
