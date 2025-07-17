namespace TO.Domains.Types.Chunks

open Friflo.Engine.ECS
open TO.Domains.Types.HexMetrics
open TO.Domains.Types.HexSpheres

type IsOverridingTileConnection = IChunk -> TileId -> TileId -> bool
type GetOverrideElevation = IChunk -> Entity -> int
type GetOverrideHeight = IChunk -> Entity -> float32
type GetOverrideEdgeType = IChunk -> Entity -> Entity -> HexEdgeType
type GetOverrideWaterLevel = IChunk -> Entity -> int
type IsOverrideUnderwater = IChunk -> Entity -> bool
type GetOverrideStreamBedY = IChunk -> Entity -> float32
type GetOverrideRiverSurfaceY = IChunk -> Entity -> float32
type GetOverrideWaterSurfaceY = IChunk -> Entity -> float32
type HasOverrideRivers = IChunk -> Entity -> bool
type HasOverrideIncomingRiver = IChunk -> Entity -> bool
type HasOverrideOutgoingRiver = IChunk -> Entity -> bool
type HasOverrideRiverBeginOrEnd = IChunk -> Entity -> bool
type HasOverrideRiverThroughEdge = IChunk -> Entity -> int -> bool
type HasOverrideIncomingRiverThroughEdge = IChunk -> Entity -> int -> bool
type HasOverrideRoads = IChunk -> Entity -> bool
type HasOverrideRoadThroughEdge = IChunk -> Entity -> int -> bool
type GetOverrideWalled = IChunk -> Entity -> bool
type GetOverrideUrbanLevel = IChunk -> Entity -> int
type GetOverrideFarmLevel = IChunk -> Entity -> int
type GetOverridePlantLevel = IChunk -> Entity -> int
type GetOverrideSpecialIndex = IChunk -> Entity -> int
type IsOverrideSpecial = IChunk -> Entity -> bool

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-07 20:37:07
[<Interface>]
type ITileOverriderQuery =
    abstract IsOverridingTileConnection: IsOverridingTileConnection
    abstract GetOverrideElevation: GetOverrideElevation
    abstract GetOverrideHeight: GetOverrideHeight
    abstract GetOverrideEdgeType: GetOverrideEdgeType
    abstract GetOverrideWaterLevel: GetOverrideWaterLevel
    abstract IsOverrideUnderwater: IsOverrideUnderwater
    abstract GetOverrideStreamBedY: GetOverrideStreamBedY
    abstract GetOverrideRiverSurfaceY: GetOverrideRiverSurfaceY
    abstract GetOverrideWaterSurfaceY: GetOverrideWaterSurfaceY
    abstract HasOverrideRivers: HasOverrideRivers
    abstract HasOverrideIncomingRiver: HasOverrideIncomingRiver
    abstract HasOverrideOutgoingRiver: HasOverrideOutgoingRiver
    abstract HasOverrideRiverBeginOrEnd: HasOverrideRiverBeginOrEnd
    abstract HasOverrideRiverThroughEdge: HasOverrideRiverThroughEdge
    abstract HasOverrideIncomingRiverThroughEdge: HasOverrideIncomingRiverThroughEdge
    abstract HasOverrideRoads: HasOverrideRoads
    abstract HasOverrideRoadThroughEdge: HasOverrideRoadThroughEdge
    abstract GetOverrideWalled: GetOverrideWalled
    abstract GetOverrideUrbanLevel: GetOverrideUrbanLevel
    abstract GetOverrideFarmLevel: GetOverrideFarmLevel
    abstract GetOverridePlantLevel: GetOverridePlantLevel
    abstract GetOverrideSpecialIndex: GetOverrideSpecialIndex
    abstract IsOverrideSpecial: IsOverrideSpecial
