namespace TO.Domains.Types.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:52:30
type HexSphereSearchNearest = Vector3 -> Chunky -> Entity option

type GetHexFacesAndNeighborCenterIds =
    Chunky -> PointComponent -> Entity -> FaceComponent array * FaceId array * PointId array

[<Interface>]
type IHexSphereQuery =
    abstract SearchNearest: HexSphereSearchNearest
    abstract GetHexFacesAndNeighborCenterIds: GetHexFacesAndNeighborCenterIds

type InitChunks = int -> float32 -> unit
type InitTiles = int -> unit
type ClearHexSphereOldData = unit -> unit
type InitHexSphere = unit -> unit

[<Interface>]
type IHexSphereInitCommand =
    abstract InitChunks: InitChunks
    abstract InitTiles: InitTiles
    abstract ClearHexSphereOldData: ClearHexSphereOldData
    abstract InitHexSphere: InitHexSphere
