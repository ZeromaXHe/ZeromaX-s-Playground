namespace TO.Domains.Types.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:29:30
type GetOrderedFaces = PointComponent -> Entity -> Entity list

[<Interface>]
type IFaceQuery =
    abstract GetOrderedFaces: GetOrderedFaces

type AddFace = Chunky -> Vector3 -> Vector3 -> Vector3 -> FaceId

[<Interface>]
type IFaceCommand =
    abstract AddFace: AddFace
