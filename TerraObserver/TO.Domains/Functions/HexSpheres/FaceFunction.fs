namespace TO.Domains.Functions.HexSpheres

open TO.Domains.Components.HexSpheres.Faces

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:25:19
module FaceFunction =
    let getUnitCentroid (hexFaces: FaceComponent array) =
        (hexFaces |> Array.map _.Center.Normalized() |> Array.sum)
        / (float32 hexFaces.Length)

    let getUnitCorners (hexFaces: FaceComponent array) = hexFaces |> Array.map _.Center
