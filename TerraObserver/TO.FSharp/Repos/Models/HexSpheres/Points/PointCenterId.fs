namespace TO.FSharp.Repos.Models.HexSpheres.Points

open Friflo.Engine.ECS

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:11:06
[<Struct>]
type PointCenterId =
    interface CenterId IIndexedComponent with
        override this.GetIndexedValue() = this.CenterId

    val CenterId: CenterId
    new(centerId: CenterId) = { CenterId = centerId }