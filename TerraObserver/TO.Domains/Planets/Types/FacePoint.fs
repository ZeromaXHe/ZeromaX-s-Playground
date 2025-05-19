namespace TO.Domains.Planets.Types

open Godot
open TO.Commons.Structs.HexSphereGrid

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 13:40:18
module FacePoint =
    type PointAdder =
        { Position: Vector3
          Coords: SphereAxial }

    type FaceAdder = { TriVertices: Vector3 array }

    type FacePointAdder =
        | PointAdder of PointAdder
        | FaceAdder of FaceAdder
