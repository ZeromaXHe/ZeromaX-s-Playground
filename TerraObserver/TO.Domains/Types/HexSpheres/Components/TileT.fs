namespace TO.Domains.Types.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Types.HexGridCoords
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Tiles

type CornerWithRadius = Vector3 -> int -> float32 -> TileUnitCorners -> Vector3
type CornerWithRadiusAndSize = Vector3 -> int -> float32 -> float32 -> TileUnitCorners -> Vector3

type GetTile = TileId -> Entity
type GetTileByCountId = int -> Entity
type GetTilesByChunkId = ChunkId -> Entities
type GetAllTiles = unit -> Entity seq
type GetSphereAxial = Entity -> SphereAxial
type IsNeighborTile = TileId -> TileId -> bool
type GetNeighborTileByIdx = Entity -> int -> Entity
type GetNeighborTiles = Entity -> Entity seq
type GetTilesInDistance = Entity -> int -> Entity seq

[<Interface>]
type ITileQuery =
    abstract GetTile: GetTile
    abstract GetTileByCountId: GetTileByCountId
    abstract GetTilesByChunkId: GetTilesByChunkId
    abstract GetAllTiles: GetAllTiles
    abstract GetSphereAxial: GetSphereAxial
    abstract IsNeighborTile: IsNeighborTile
    abstract GetNeighborTileByIdx: GetNeighborTileByIdx
    abstract GetNeighborTiles: GetNeighborTiles
    abstract GetTilesInDistance: GetTilesInDistance

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 05:36:30
type AddTile = PointId -> ChunkId -> FaceComponent array -> HexFaceIds -> NeighborCenterIds -> TileId
type AddTileOtherComponents = unit -> unit
type RemoveRoads = Entity -> unit
type AddRoad = Entity -> Entity -> unit
type RemoveRivers = Entity -> unit
type SetOutgoingRiver = Entity -> Entity -> unit
type SetElevation = Entity -> int -> unit
type SetTerrainTypeIndex = Entity -> int -> unit
type SetWaterLevel = Entity -> int -> unit
type SetUrbanLevel = Entity -> int -> unit
type SetFarmLevel = Entity -> int -> unit
type SetPlantLevel = Entity -> int -> unit
type SetWalled = Entity -> bool -> unit
type SetSpecialIndex = Entity -> int -> unit


[<Interface>]
type ITileCommand =
    abstract AddTile: AddTile
    abstract AddTileOtherComponents: AddTileOtherComponents
    abstract RemoveRoads: RemoveRoads
    abstract AddRoad: AddRoad
    abstract RemoveRivers: RemoveRivers
    abstract SetOutgoingRiver: SetOutgoingRiver
    abstract SetElevation: SetElevation
    abstract SetTerrainTypeIndex: SetTerrainTypeIndex
    abstract SetWaterLevel: SetWaterLevel
    abstract SetUrbanLevel: SetUrbanLevel
    abstract SetFarmLevel: SetFarmLevel
    abstract SetPlantLevel: SetPlantLevel
    abstract SetWalled: SetWalled
    abstract SetSpecialIndex: SetSpecialIndex
