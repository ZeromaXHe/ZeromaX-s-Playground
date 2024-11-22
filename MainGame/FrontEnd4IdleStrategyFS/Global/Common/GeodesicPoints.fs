namespace FrontEnd4IdleStrategyFS.Global.Common

open System.Collections.Generic
open Godot

module GeodesicPoints =
    let subdivideSphere (vertices: Vector3 list) (indices: int list) =
        let triCount = indices.Length / 3

        let newVertices, newIndices =
            { 0 .. triCount - 1}
            |> Seq.fold
               (fun (vert: Vector3 list, ind: int list) tri ->
                    let oldVertIndex = tri * 3
                    let idxA = indices[oldVertIndex + 0]
                    let idxB = indices[oldVertIndex + 1]
                    let idxC = indices[oldVertIndex + 2]
                    let vA = vert[idxA]
                    let vB = vert[idxB]
                    let vC = vert[idxC]

                    let vAB = vA.Lerp(vB, 0.5f).Normalized()
                    let vBC = vB.Lerp(vC, 0.5f).Normalized()
                    let vAC = vA.Lerp(vC, 0.5f).Normalized()

                    ((vert @ [vAB; vBC; vAC]),
                     (ind @ [
                        vert.Length + 0; vert.Length + 1; vert.Length + 2; // 中间三角形
                        vert.Length + 2; idxA; vert.Length + 0; // A 三角形
                        vert.Length + 0; idxB; vert.Length + 1; // B 三角形
                        vert.Length + 1; idxC; vert.Length + 2; // C 三角形
                      ]))
               )
               (vertices, indices)
        newVertices, newIndices |> List.skip indices.Length

    type Vector3Comparer() =
        interface IEqualityComparer<Vector3> with
            member this.Equals(x, y) = x.IsEqualApprox y
            // 如果 GetHashCode 不相等，就直接不判断 Equals 了！
            member this.GetHashCode _ = 1

    let distinctVector3 list =
        let set = HashSet<Vector3>(Vector3Comparer())
        list |> List.filter set.Add

    let genPoints subdivides (radius: float32) =
        let x = 0.525731112119133606f
        let z = 0.850650808352039932f

        let vertices = [
            Vector3(-x, 0.0f, z); Vector3(x, 0.0f, z); Vector3(-x, 0.0f, -z); Vector3(x, 0.0f, -z);
            Vector3(0.0f, z, x); Vector3(0.0f, z, -x); Vector3(0.0f, -z, x); Vector3(0.0f, -z, -x);
            Vector3(z, x, 0.0f); Vector3(-z, x, 0.0f); Vector3(z, -x, 0.0f); Vector3(-z, -x, 0.0f);
        ]

        let indices = [
            1; 4; 0;
            4; 9; 0;
            4; 5; 9;
            8; 5; 4;
            1; 8; 4;
            1; 10; 8;
            10; 3; 8;
            8; 3; 5;
            3; 2; 5;
            3; 7; 2;
            3; 10; 7;
            10; 6; 7;
            6; 11; 7;
            6; 0; 11;
            6; 1; 0;
            10; 1; 6;
            11; 0; 9;
            2; 11; 9;
            5; 2; 9;
            11; 2; 7;
        ]

        let flatVertices = indices |> List.map (fun i -> vertices[i])

        let flatIndices = [ 0 .. indices.Length - 1 ]

        { 1 .. subdivides }
        |> Seq.fold (fun (v, i) _ -> subdivideSphere v i) (flatVertices, flatIndices)
        |> fst
        |> List.map (fun v -> v * radius) // 缩放
        |> distinctVector3
