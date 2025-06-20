namespace TO.Controllers.Apps.Planets

open Friflo.Engine.ECS
open TO.Repos.Commands.HexSpheres
open TO.Repos.Data.HexSpheres
open TO.Repos.Queries.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
type PlanetRepoEnv(store: EntityStore, chunkyVpTrees: ChunkyVpTrees) =
    interface IPointQuery with
        member this.GetNeighborByIdAndIdx = PointQuery.getNeighborByIdAndIdx store
        member this.GetNeighborCenterPointIds = PointQuery.getNeighborCenterPointIds store
        member this.GetNeighborIdsById = PointQuery.getNeighborIdsById store
        member this.GetNeighborIdx = PointQuery.getNeighborIdx store
        member this.SearchNearestCenterPos = PointQuery.searchNearestCenterPos chunkyVpTrees
        member this.TryHeadByPosition = PointQuery.tryHeadByPosition store
        member this.TryHeadEntityByCenterId = PointQuery.tryHeadEntityByCenterId store
    
    interface IChunkQuery with
        member this.IsHandlingLodGaps = ChunkQuery.isHandlingLodGaps store
    
    interface IHexSphereInitCommand with
        member this.ClearOldData = HexSphereInitCommand.clearOldData store
        member this.InitChunks = HexSphereInitCommand.initChunks store chunkyVpTrees
        member this.InitTiles = HexSphereInitCommand.initTiles store chunkyVpTrees
