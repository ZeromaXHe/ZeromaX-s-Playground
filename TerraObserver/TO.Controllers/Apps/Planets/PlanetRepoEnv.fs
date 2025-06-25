namespace TO.Controllers.Apps.Planets

open TO.Repos.Commands.HexSpheres
open TO.Repos.Commands.Meshes
open TO.Repos.Commands.PathFindings
open TO.Repos.Commands.Shaders
open TO.Repos.Queries.Friflos
open TO.Repos.Queries.HexSpheres
open TO.Repos.Queries.Meshes

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:33:19
type PlanetRepoEnv(store, chunkyVpTrees, lodMeshCache, tileSearcher, tileShaderData) =
    interface IEntityStoreQuery with
        member this.GetEntityById = EntityStoreQuery.getEntityById store
        member this.GetEntityStore = EntityStoreQuery.getEntityStore store

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
        member this.GetLod = ChunkQuery.getLod store

    interface ILodMeshCacheQuery with
        member this.GetLodMeshes = LodMeshCacheQuery.getLodMeshes lodMeshCache

    interface IChunkCommand with
        member this.Add = ChunkCommand.add store
        member this.UpdateInsightAndLod = ChunkCommand.updateInsightAndLod store

    interface IHexSphereInitCommand with
        member this.ClearOldData = HexSphereInitCommand.clearOldData store
        member this.InitChunks = HexSphereInitCommand.initChunks store chunkyVpTrees
        member this.InitTiles = HexSphereInitCommand.initTiles store chunkyVpTrees

    interface ILodMeshCacheCommand with
        member this.AddLodMeshes = LodMeshCacheCommand.addLodMeshes lodMeshCache

    interface ITileSearcherCommand with
        member this.InitSearchData = TileSearcherCommand.initSearchData store tileSearcher

    interface ITileShaderDataCommand with
        member this.InitShaderData = TileShaderDataCommand.initShaderData tileShaderData
