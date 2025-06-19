namespace TO.FSharp.Domains.Components.HexSpheres.Points

open Friflo.Engine.ECS
open TO.FSharp.Domains.Alias.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:11:06
[<Struct>]
type PointCenterId =
    interface PointId IIndexedComponent with
        override this.GetIndexedValue() = this.CenterId

    val CenterId: PointId
    new(centerId: PointId) = { CenterId = centerId }