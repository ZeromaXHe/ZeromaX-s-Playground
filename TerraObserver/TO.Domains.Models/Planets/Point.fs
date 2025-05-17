namespace TO.Domains.Models.Planets

open Godot
open TO.Commons.Structs.HexSphereGrid

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 14:14:17
type Point =
    { PointId: int
      Chunky: bool
      Position: Vector3
      Coords: SphereAxial
      FaceIds: int list }
