namespace TO.Domains.Planets.Types

open Godot
open TO.Commons.Structs.HexSphereGrid

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 13:40:18
module FacePoint =
    type PointPreAdd =
        { Position: Vector3
          Coords: SphereAxial }

    type FacePreAdd =
        { Vertex1: Vector3
          Vertex2: Vector3
          Vertex3: Vector3 }

    type PreAdd =
        | Point of PointPreAdd
        | Face of FacePreAdd
