namespace TO.Domains.Functions.HexSpheres.Components

open System.Collections.Generic
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.Friflos
open TO.Domains.Functions.HexSpheres.Components.Faces
open TO.Domains.Functions.HexSpheres.Components.Points
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Types.Chunks
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexMetrics
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps
open TO.Domains.Types.Shaders
open TO.Domains.Types.Units

module Tile =
    let pointCenterId (tile: Entity) = tile.GetComponent<PointCenterId>()

    let pointNeighborCenterIds (tile: Entity) =
        tile.GetComponent<PointNeighborCenterIds>()

    let countId (tile: Entity) = tile.GetComponent<TileCountId>()
    let chunkId (tile: Entity) = tile.GetComponent<TileChunkId>()
    let unitCentroid (tile: Entity) = tile.GetComponent<TileUnitCentroid>()
    let visibility (tile: Entity) = tile.GetComponent<TileVisibility>()
    let unitCorners (tile: Entity) = tile.GetComponent<TileUnitCorners>()
    let hexFaceIds (tile: Entity) = tile.GetComponent<TileHexFaceIds>()
    let flag (tile: Entity) = tile.GetComponent<TileFlag>()
    let value (tile: Entity) = tile.GetComponent<TileValue>()
    let unitId (tile: Entity) = tile.GetComponent<TileUnitId>()

    let cornerWithRadius (func: CornerWithRadius) (idx: int) (radius: float32) (tile: Entity) =
        tile |> unitCorners |> func (unitCentroid tile).UnitCentroid idx radius

    let cornerWithRadiusAndSize
        (func: CornerWithRadiusAndSize)
        (idx: int)
        (radius: float32)
        (size: float32)
        (tile: Entity)
        =
        tile
        |> unitCorners
        |> func (unitCentroid tile |> _.UnitCentroid) idx radius size

    let getCornerByFaceCenterWithRadiusAndSize (faceCenter: Vector3) (radius: float32) (size: float32) (tile: Entity) =
        tile
        |> unitCentroid
        |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize faceCenter radius size

    let getNeighborTileIdx (tile: Entity) (neighbor: Entity) =
        tile
        |> pointNeighborCenterIds
        |> PointNeighborCenterIds.getNeighborIdx (neighbor |> pointCenterId |> _.CenterId)

    let getMoveCost (fromTile: Entity) (toTile: Entity) =
        let edgeType = fromTile |> value |> TileValue.getEdgeType (toTile |> value)

        if edgeType = HexEdgeType.Cliff then
            -1
        elif fromTile |> flag |> TileFlag.hasRoad (getNeighborTileIdx fromTile toTile) then
            1
        elif fromTile |> flag |> TileFlag.walled <> (toTile |> flag |> TileFlag.walled) then
            -1
        else
            let toTileValue = toTile |> value

            (if edgeType = HexEdgeType.Flat then 5 else 10)
            + TileValue.urbanLevel toTileValue
            + TileValue.farmLevel toTileValue
            + TileValue.plantLevel toTileValue

module TileQuery =
    let getTile (env: #IEntityStoreQuery) : GetTile =
        fun (tileId: TileId) -> env.GetEntityById tileId

    let getTileByCountId (env: #IEntityStoreQuery) : GetTileByCountId =
        fun (tileCountId: int) -> (env.EntityStore.ComponentIndex<TileCountId, int>()[tileCountId]).Item 0 // 假定了结果一定非空

    let getTilesByChunkId (env: #IEntityStoreQuery) : GetTilesByChunkId =
        fun (chunkId: ChunkId) -> env.EntityStore.ComponentIndex<TileChunkId, ChunkId>()[chunkId]

    let getAllTiles (env: #IEntityStoreQuery) : GetAllTiles =
        fun () -> env.Query<TileValue>() |> ArchetypeQueryQuery.toEntitySeq

    let getSphereAxial (env: 'E :> IEntityStoreQuery) : GetSphereAxial =
        fun (tile: Entity) ->
            tile
            |> Tile.pointCenterId
            |> _.CenterId
            |> env.GetEntityById
            |> _.GetComponent<PointComponent>().Coords

    let isNeighborTile (env: #IEntityStoreQuery) : IsNeighborTile =
        fun (tileId: TileId) (neighborId: TileId) ->
            let tile = env.GetEntityById tileId
            let tileNeighborCenters = tile |> Tile.pointNeighborCenterIds
            let preTile = env.GetEntityById neighborId
            let neighborCenter = preTile |> Tile.pointCenterId
            tileNeighborCenters |> PointNeighborCenterIds.isNeighbor neighborCenter.CenterId

    let getNeighborTileByIdx (env: 'E :> IPointQuery) : GetNeighborTileByIdx =
        fun (tile: Entity) (idx: int) ->
            tile
            |> Tile.pointNeighborCenterIds
            |> PointNeighborCenterIds.item idx
            |> env.TryHeadEntityByCenterId
            |> Option.get

    let getNeighborTiles (env: #IEntityStoreQuery) : GetNeighborTiles =
        fun (tile: Entity) ->
            let store = env.EntityStore
            let neighborCenterIds = tile |> Tile.pointNeighborCenterIds

            { 0 .. neighborCenterIds.Length - 1 }
            |> Seq.collect (fun i ->
                let centerId = neighborCenterIds |> PointNeighborCenterIds.item i
                store.ComponentIndex<PointCenterId, PointId>()[centerId])

    let getTilesInDistance (env: #ITileQuery) : GetTilesInDistance =
        fun (tile: Entity) (dist: int) ->
            if dist = 0 then
                seq { tile }
            else
                let resultSet = HashSet<Entity>()
                resultSet.Add tile |> ignore
                let mutable preRing = ResizeArray<Entity>()
                preRing.Add tile
                let mutable afterRing = ResizeArray<Entity>()

                for i in 0 .. dist - 1 do
                    afterRing.AddRange(
                        preRing
                        |> Seq.collect (fun t -> env.GetNeighborTiles t)
                        |> Seq.filter resultSet.Add
                    )

                    let tempRing = preRing
                    preRing <- afterRing
                    afterRing <- tempRing
                    afterRing.Clear()

                resultSet


/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 11:59:30
module TileCommand =
    let add (env: #IEntityStoreQuery) : AddTile =
        fun
            (centerId: PointId)
            (chunkId: ChunkId)
            (hexFaces: FaceComponent array)
            (hexFaceIds: HexFaceIds)
            (neighborCenterIds: NeighborCenterIds) ->
            let store = env.EntityStore
            let unitCentroid = FaceFunction.getUnitCentroid hexFaces
            let unitCorners = FaceFunction.getUnitCorners hexFaces

            store
                .CreateEntity(
                    PointCenterId centerId,
                    PointNeighborCenterIds neighborCenterIds,
                    TileCountId <| store.Query<TileUnitCentroid>().Count + 1,
                    TileChunkId chunkId,
                    TileUnitCentroid unitCentroid,
                    TileUnitCorners unitCorners,
                    TileHexFaceIds hexFaceIds,
                    TileFlag TileFlagEnum.Explorable,
                    TileValue 0,
                    TileVisibility 0
                )
                .Id

    let addTileOtherComponents
        (env: 'E when 'E :> IEntityStoreQuery and 'E :> IEntityStoreCommand)
        : AddTileOtherComponents =
        fun () ->
            env.ExecuteInCommandBuffer(fun cb ->
                env
                    .Query<TileCountId>()
                    .ForEachEntity(fun tileCountId tile ->
                        let tileUnitId = TileUnitId 0
                        cb.AddComponent(tile.Id, &tileUnitId)))

    let private refreshTerrainShader
        (env: 'E when 'E :> ITileShaderDataCommand and 'E :> IMiniMapManagerCommand)
        (tile: Entity)
        =
        tile |> Tile.value |> env.RefreshTileShaderDataTerrain(Tile.countId tile)
        env.RefreshMiniMapTile tile

    let private viewElevationChanged (env: #ITileShaderDataCommand) (tile: Entity) =
        tile |> Tile.value |> env.ViewElevationChanged(Tile.countId tile)

    let private refreshSelfOnly (env: #IChunkLoaderCommand) (tile: Entity) =
        env.RefreshChunk (Tile.chunkId tile).ChunkId

    let private refresh (env: 'E when 'E :> IChunkLoaderCommand and 'E :> ITileQuery) =
        fun (tile: Entity) ->
            refreshSelfOnly env tile

            env.GetNeighborTiles tile
            |> Seq.distinctBy (fun n -> n |> Tile.chunkId |> _.ChunkId)
            |> Seq.filter (fun n -> n |> Tile.chunkId |> _.ChunkId <> (tile |> Tile.chunkId |> _.ChunkId))
            |> Seq.iter (refreshSelfOnly env)

    let private getElevationDifference (env: #ITileQuery) (tile: Entity) (idx: int) =
        (tile |> Tile.value |> TileValue.elevation)
        - (env.GetNeighborTileByIdx tile idx |> Tile.value |> TileValue.elevation)
        |> abs

    let private setRoad (env: #ITileQuery) (tile: Entity) (idx: int) (state: bool) =
        if state <> (tile |> Tile.flag |> TileFlag.hasRoad idx) then
            let flags =
                if state then
                    tile |> Tile.flag |> TileFlag.withRoad idx
                else
                    tile |> Tile.flag |> TileFlag.withoutRoad idx

            let newTileFlag = TileFlag flags
            tile.AddComponent<TileFlag> &newTileFlag |> ignore
            refreshSelfOnly env tile

        let neighbor = env.GetNeighborTileByIdx tile idx
        let neighborIdx = Tile.getNeighborTileIdx neighbor tile

        if neighbor |> Tile.flag |> TileFlag.hasRoad neighborIdx <> state then
            let flags =
                if state then
                    neighbor |> Tile.flag |> TileFlag.withRoad neighborIdx
                else
                    neighbor |> Tile.flag |> TileFlag.withoutRoad neighborIdx

            let newNeighborFlag = TileFlag flags
            neighbor.AddComponent<TileFlag> &newNeighborFlag |> ignore
            refreshSelfOnly env neighbor

    let removeRoads env : RemoveRoads =
        fun (tile: Entity) ->
            for i in 0 .. (Tile.hexFaceIds tile |> _.Length) - 1 do
                if tile |> Tile.flag |> TileFlag.hasRoad i then
                    setRoad env tile i false

    let addRoad env : AddRoad =
        fun (tile: Entity) (neighbor: Entity) ->
            let idx = Tile.getNeighborTileIdx tile neighbor
            let tileFlag = tile |> Tile.flag
            let tileValue = tile |> Tile.value
            let neighborValue = neighbor |> Tile.value

            if
                (tileFlag |> TileFlag.hasRoad idx |> not)
                && (tileFlag |> TileFlag.hasRiver idx |> not)
                && (tileValue |> TileValue.isSpecial |> not)
                && (neighborValue |> TileValue.isSpecial |> not)
                && (tileValue |> TileValue.isUnderwater |> not)
                && (neighborValue |> TileValue.isUnderwater |> not) // 不在水下生成道路
                && getElevationDifference env tile idx <= 1
            then
                setRoad env tile idx true

    let private validateRoadsWater env (tile: Entity) =
        if tile |> Tile.value |> TileValue.isUnderwater then
            removeRoads env tile

        tile |> Tile.value |> TileValue.isUnderwater

    let private isValidRiverDestination (tile: Entity) (neighbor: Entity) =
        let neighborElevation = neighbor |> Tile.value |> TileValue.elevation

        (tile |> Tile.value |> TileValue.elevation) >= neighborElevation
        || (tile |> Tile.value |> TileValue.waterLevel) = neighborElevation

    let removeOutgoingRiver (env: #ITileQuery) (tile: Entity) =
        if tile |> Tile.flag |> TileFlag.hasOutgoingRiver then
            let neighbor =
                tile |> Tile.flag |> TileFlag.riverOutDirection |> env.GetNeighborTileByIdx tile

            let newTileFlag =
                tile |> Tile.flag |> TileFlag.withoutMask TileFlagEnum.RiverOut |> TileFlag

            let newNeighborFlag =
                neighbor |> Tile.flag |> TileFlag.withoutMask TileFlagEnum.RiverIn |> TileFlag

            tile.AddComponent<TileFlag> &newTileFlag |> ignore
            neighbor.AddComponent<TileFlag> &newNeighborFlag |> ignore
            refreshSelfOnly env neighbor
            refreshSelfOnly env tile

    let removeIncomingRiver (env: #ITileQuery) (tile: Entity) =
        if tile |> Tile.flag |> TileFlag.hasIncomingRiver then
            let neighbor =
                tile |> Tile.flag |> TileFlag.riverInDirection |> env.GetNeighborTileByIdx tile

            let newTileFlag =
                tile |> Tile.flag |> TileFlag.withoutMask TileFlagEnum.RiverIn |> TileFlag

            let newNeighborFlag =
                neighbor |> Tile.flag |> TileFlag.withoutMask TileFlagEnum.RiverOut |> TileFlag

            tile.AddComponent<TileFlag> &newTileFlag |> ignore
            neighbor.AddComponent<TileFlag> &newNeighborFlag |> ignore
            refreshSelfOnly env neighbor
            refreshSelfOnly env tile

    let removeRivers env : RemoveRivers =
        fun (tile: Entity) ->
            removeOutgoingRiver env tile
            removeIncomingRiver env tile

    let setOutgoingRiver (env: #ITileQuery) : SetOutgoingRiver =
        fun (tile: Entity) (riverToTile: Entity) ->
            if
                (tile |> Tile.flag |> TileFlag.hasOutgoingRiver)
                && (tile
                    |> Tile.flag
                    |> TileFlag.riverOutDirection
                    |> env.GetNeighborTileByIdx tile
                    |> _.Id = riverToTile.Id)
            then
                ()
            elif not <| isValidRiverDestination tile riverToTile then
                GD.Print $"SetOutgoingRiver tile {tile.Id} to {riverToTile.Id} failed because neighbor higher"
                ()
            else
                // GD.Print($"Setting Outgoing River from {tile.Id} to {riverToTile.Id}");
                removeOutgoingRiver env tile

                if
                    (tile |> Tile.flag |> TileFlag.hasIncomingRiver)
                    && (tile
                        |> Tile.flag
                        |> TileFlag.riverInDirection
                        |> env.GetNeighborTileByIdx tile
                        |> _.Id = riverToTile.Id)
                then
                    removeIncomingRiver env tile

                let newTileFlag =
                    tile
                    |> Tile.flag
                    |> TileFlag.withRiverOut (Tile.getNeighborTileIdx tile riverToTile)
                    |> TileFlag

                let newTileValue = tile |> Tile.value |> TileValue.withSpecialIndex 0
                tile.AddComponent<TileFlag> &newTileFlag |> ignore
                tile.AddComponent<TileValue> &newTileValue |> ignore
                removeIncomingRiver env riverToTile

                let newRiverToFlag =
                    riverToTile
                    |> Tile.flag
                    |> TileFlag.withRiverIn (Tile.getNeighborTileIdx riverToTile tile)
                    |> TileFlag

                let newRiverToValue = riverToTile |> Tile.value |> TileValue.withSpecialIndex 0
                riverToTile.AddComponent<TileFlag> &newRiverToFlag |> ignore
                riverToTile.AddComponent<TileValue> &newRiverToValue |> ignore
                setRoad env tile (Tile.getNeighborTileIdx tile riverToTile) false
                refreshSelfOnly env tile
                refreshSelfOnly env riverToTile

    let private validateRivers (env: #ITileQuery) (tile: Entity) =
        if
            tile |> Tile.flag |> TileFlag.hasOutgoingRiver
            && tile
               |> Tile.flag
               |> TileFlag.riverOutDirection
               |> env.GetNeighborTileByIdx tile
               |> isValidRiverDestination tile
               |> not
        then
            removeOutgoingRiver env tile

        if
            tile |> Tile.flag |> TileFlag.hasIncomingRiver
            && tile
               |> Tile.flag
               |> TileFlag.riverInDirection
               |> env.GetNeighborTileByIdx tile
               |> isValidRiverDestination tile
               |> not
        then
            removeIncomingRiver env tile

    let setElevation (env: #IUnitManagerCommand) : SetElevation =
        fun (tile: Entity) (elevation: int) ->
            let tileValue = tile |> Tile.value

            if tileValue |> TileValue.elevation <> elevation then
                let originalViewElevation = tileValue |> TileValue.viewElevation
                let newTileValue = tileValue |> TileValue.withElevation elevation
                tile.AddComponent<TileValue> &newTileValue |> ignore
                validateRivers env tile

                if not <| validateRoadsWater env tile then
                    for i in 0 .. (Tile.hexFaceIds tile |> _.Length) - 1 do
                        if tile |> Tile.flag |> TileFlag.hasRoad i && getElevationDifference env tile i > 1 then
                            setRoad env tile i false

                refresh env tile
                refreshTerrainShader env tile

                if tile |> Tile.value |> TileValue.viewElevation <> originalViewElevation then
                    viewElevationChanged env tile

                let tileUnitId = tile |> Tile.unitId |> _.UnitId

                if tileUnitId > 0 then
                    env.ValidateUnitLocationById tileUnitId

    let setTerrainTypeIndex env : SetTerrainTypeIndex =
        fun (tile: Entity) (idx: int) ->
            if tile |> Tile.value |> TileValue.terrainTypeIndex <> idx then
                let newTileValue = tile |> Tile.value |> TileValue.withTerrainTypeIndex idx
                tile.AddComponent<TileValue> &newTileValue |> ignore
                refresh env tile
                refreshTerrainShader env tile

    let setWaterLevel env : SetWaterLevel =
        fun (tile: Entity) (waterLevel: int) ->
            if tile |> Tile.value |> TileValue.waterLevel <> waterLevel then
                let originalViewElevation = tile |> Tile.value |> TileValue.viewElevation
                let newTileValue = tile |> Tile.value |> TileValue.withWaterLevel waterLevel
                tile.AddComponent<TileValue> &newTileValue |> ignore
                validateRivers env tile
                validateRoadsWater env tile |> ignore
                refresh env tile
                refreshTerrainShader env tile

                if tile |> Tile.value |> TileValue.viewElevation <> originalViewElevation then
                    viewElevationChanged env tile

    let setUrbanLevel env : SetUrbanLevel =
        fun (tile: Entity) (urbanLevel: int) ->
            if tile |> Tile.value |> TileValue.urbanLevel <> urbanLevel then
                let newTileValue = tile |> Tile.value |> TileValue.withUrbanLevel urbanLevel
                tile.AddComponent<TileValue> &newTileValue |> ignore
                refreshSelfOnly env tile

    let setFarmLevel env : SetFarmLevel =
        fun (tile: Entity) (farmLevel: int) ->
            if tile |> Tile.value |> TileValue.farmLevel <> farmLevel then
                let newTileValue = tile |> Tile.value |> TileValue.withFarmLevel farmLevel
                tile.AddComponent<TileValue> &newTileValue |> ignore
                refreshSelfOnly env tile

    let setPlantLevel env : SetPlantLevel =
        fun (tile: Entity) (plantLevel: int) ->
            if tile |> Tile.value |> TileValue.plantLevel <> plantLevel then
                let newTileValue = tile |> Tile.value |> TileValue.withPlantLevel plantLevel
                tile.AddComponent<TileValue> &newTileValue |> ignore
                refreshSelfOnly env tile

    let setWalled env : SetWalled =
        fun (tile: Entity) (walled: bool) ->
            if tile |> Tile.flag |> TileFlag.walled <> walled then
                let newTileFlag =
                    tile
                    |> Tile.flag
                    |> if walled then
                           TileFlag.withMask TileFlagEnum.Walled
                       else
                           TileFlag.withoutMask TileFlagEnum.Walled
                    |> TileFlag

                tile.AddComponent<TileFlag> &newTileFlag |> ignore
                refresh env tile

    let setSpecialIndex env : SetSpecialIndex =
        fun (tile: Entity) (specialIndex: int) ->
            if
                (tile |> Tile.value |> TileValue.specialIndex <> specialIndex)
                || (tile |> Tile.flag |> TileFlag.hasRivers)
            then
                let newTileValue = tile |> Tile.value |> TileValue.withSpecialIndex specialIndex
                tile.AddComponent<TileValue> &newTileValue |> ignore
                removeRoads env tile
                refreshSelfOnly env tile
