namespace TO.Infras.Abstractions.Planets.Commands

open Godot
open TO.Commons.Structs.HexSphereGrid

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 14:08:17
[<Interface>]
type IPointCommand =
    abstract Add: chunky: bool -> position: Vector3 -> coords: SphereAxial -> int
