namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open Godot
open TO.Domains.Functions.Maths
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:39:29
module TileUnitCentroid =
    let scaled (radius: float32) (this: TileUnitCentroid) = this.UnitCentroid * radius

    let getCornerByFaceCenterWithRadiusAndSize
        (faceCenter: Vector3)
        (radius: float32)
        (size: float32)
        (this: TileUnitCentroid)
        =
        Math3dUtil.ProjectToSphere(this.UnitCentroid.Lerp(faceCenter, size), radius)

    let getCornerByFaceCenterWithRadius (faceCenter: Vector3) (radius: float32) (this: TileUnitCentroid) =
        getCornerByFaceCenterWithRadiusAndSize faceCenter radius 1f this

    let getCornerByFaceCenter (faceCenter: Vector3) (this: TileUnitCentroid) =
        getCornerByFaceCenterWithRadiusAndSize faceCenter 1f 1f this

