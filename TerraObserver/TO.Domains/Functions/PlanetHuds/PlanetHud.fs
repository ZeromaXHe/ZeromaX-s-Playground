namespace TO.Domains.Functions.PlanetHuds

open System
open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexGridCoords
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maps
open TO.Domains.Functions.Maths
open TO.Domains.Types.Cameras
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Chunks
open TO.Domains.Types.HexSpheres.Components.Tiles
open TO.Domains.Types.Maps
open TO.Domains.Types.PlanetHuds
open TO.Domains.Types.Units

module PlanetHudQuery =
    let getEditMode (env: #IPlanetHudQuery) : GetEditMode =
        fun () -> env.PlanetHudOpt |> Option.map _.EditMode |> Option.defaultValue true

    let getLabelMode (env: #IPlanetHudQuery) : GetLabelMode =
        fun () -> env.PlanetHudOpt |> Option.map _.LabelMode |> Option.defaultValue 0

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 19:57:22
module PlanetHudCommand =
    let onOrbitCameraRigMoved (env: #IPlanetHudQuery) : OnOrbitCameraRigMoved =
        fun pos _ ->
            env.PlanetHudOpt
            |> Option.iter (fun hud ->
                let lonLat = LonLatCoords.fromVector3 pos
                hud.CamLonLatLabel.Text <- $"相机位置：{lonLat |> LonLatCoords.toString}")

    let onOrbitCameraRigTransformed
        (env: 'E when 'E :> IOrbitCameraRigQuery and 'E :> IPlanetHudQuery)
        : OnOrbitCameraRigTransformed =
        fun transform ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let northPolePoint = Vector3.Up
                let posNormal = transform.Origin.Normalized()
                let dirNorth = Math3dUtil.DirectionBetweenPointsOnSphere posNormal northPolePoint

                let angleToNorth =
                    transform.Basis.Y.Slide(posNormal).SignedAngleTo(dirNorth, -posNormal)

                planetHud.CompassPanel.Rotation <- angleToNorth
                let posLocal = env.GetFocusBasePos()
                let lonLat = LonLatCoords.fromVector3 posLocal

                match planetHud.RectMap.Material with
                | :? ShaderMaterial as rectMapMaterial ->
                    rectMapMaterial.SetShaderParameter("lon", lonLat.Longitude)
                    rectMapMaterial.SetShaderParameter("lat", lonLat.Latitude)
                    // rectMapMaterial?.SetShaderParameter("pos_normal", posLocal.Normalized()); // 非常奇怪，旋转时会改变……
                    rectMapMaterial.SetShaderParameter("angle_to_north", angleToNorth)
                | _ -> ()
            // GD.Print($"lonLat: {longLat.Longitude}, {longLat.Latitude}; angleToNorth: {
            //     angleToNorth}; posNormal: {posNormal};");
            )

    let initElevationAndWaterVSlider
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery)
        : InitElevationAndWaterVSlider =
        fun () ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planet = env.PlanetConfig
                // 按照指定的高程分割数量确定 UI
                planetHud.ElevationVSlider.MaxValue <- planet.ElevationStep
                planetHud.ElevationVSlider.TickCount <- planet.ElevationStep + 1
                planetHud.WaterVSlider.MaxValue <- planet.ElevationStep
                planetHud.WaterVSlider.TickCount <- planet.ElevationStep + 1)

    let initRectMiniMap (env: #IPlanetHudQuery) : InitRectMiniMap =
        fun () ->
            match env.PlanetHudOpt with
            | None -> ()
            | Some planetHud ->
                let rectMap = RectMiniMapQuery.generateRectMap env
                planetHud.RectMap.Texture <- rectMap

    let updateRadiusLineEdit (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery) : UpdateRadiusLineEdit =
        fun text ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planetConfig = env.PlanetConfig

                match Single.TryParse text with
                | true, radius -> planetConfig.Radius <- radius
                | false, _ -> ()

                planetHud.RadiusLineEdit.Text <- $"{planetConfig.Radius:F2}")

    let updateDivisionLineEdit
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IPlanetHudQuery)
        : UpdateDivisionLineEdit =
        fun chunky text ->
            env.PlanetHudOpt
            |> Option.iter (fun planetHud ->
                let planetConfig = env.PlanetConfig

                match Int32.TryParse text with
                | true, division ->
                    if chunky then
                        planetConfig.ChunkDivisions <- division
                    else
                        planetConfig.Divisions <- division
                | false, _ -> ()

                planetHud.DivisionLineEdit.Text <- $"{planetConfig.Divisions}"
                planetHud.ChunkDivisionLineEdit.Text <- $"{planetConfig.ChunkDivisions}")

    let private validateDrag
        (env: 'E when 'E :> ITileQuery and 'E :> IPlanetHudQuery)
        (tileId: int)
        (this: IPlanetHud)
        =
        this.DragTileId <- Nullable tileId
        this.IsDrag <- env.IsNeighborTile tileId this.PreviousTileId.Value

    let private editTile (env: 'E when 'E :> ITileCommand and 'E :> ITileQuery) (tile: Entity) (this: IPlanetHud) =
        ()

        if this.ApplyTerrain then
            env.SetTerrainTypeIndex tile this.ActiveTerrain

        if this.ApplyElevation then
            env.SetElevation tile this.ActiveElevation

        if this.ApplyWaterLevel then
            env.SetWaterLevel tile this.ActiveWaterLevel

        if this.ApplySpecialIndex then
            env.SetSpecialIndex tile this.ActiveSpecialIndex

        if this.ApplyUrbanLevel then
            env.SetUrbanLevel tile this.ActiveUrbanLevel

        if this.ApplyFarmLevel then
            env.SetFarmLevel tile this.ActiveFarmLevel

        if this.ApplyPlantLevel then
            env.SetPlantLevel tile this.ActivePlantLevel

        if this.RiverMode = OptionalToggle.No then
            env.RemoveRivers tile

        if this.RoadMode = OptionalToggle.No then
            env.RemoveRoads tile

        if this.WalledMode <> OptionalToggle.Ignore then
            env.SetWalled tile (this.WalledMode = OptionalToggle.Yes)

        if this.IsDrag then
            let previousTile = env.GetTile this.PreviousTileId.Value // 需要保证入参非空
            let dragTile = env.GetTile this.DragTileId.Value

            if this.RiverMode = OptionalToggle.Yes then
                env.SetOutgoingRiver previousTile dragTile

            if this.RoadMode = OptionalToggle.Yes then
                env.AddRoad previousTile dragTile

    let private editTiles (env: #ITileQuery) (chosenTileId: int) (this: IPlanetHud) =
        let chosenTile = env.GetTile chosenTileId

        for t in env.GetTilesInDistance chosenTile this.BrushSize do
            editTile env t this

    let updateChosenTileInfo
        (env: 'E when 'E :> ITileQuery and 'E :> IPlanetHudQuery and 'E :> ICatlikeCodingNoiseQuery)
        : UpdateChosenTileInfo =
        fun (this: IPlanetHud) ->
            if this.ChosenTileId.HasValue then
                let chosenTile = env.GetTile this.ChosenTileId.Value
                this.IdLineEdit.Text <- chosenTile.Id.ToString()
                this.ChunkLineEdit.Text <- chosenTile |> Tile.chunkId |> _.ChunkId.ToString()
                let sa = env.GetSphereAxial chosenTile
                this.CoordsLineEdit.Text <- sa |> SphereAxial.toString
                this.CoordsLineEdit.TooltipText <- this.CoordsLineEdit.Text
                this.HeightLineEdit.Text <- $"{env.GetHeight chosenTile:F4}"
                this.ElevationLineEdit.Text <- chosenTile |> Tile.value |> TileValue.elevation |> _.ToString()
                let lonLat = sa |> SphereAxial.toLonLat
                this.LonLineEdit.Text <- lonLat |> LonLatCoords.getLonLatString true
                this.LonLineEdit.TooltipText <- this.LonLineEdit.Text
                this.LatLineEdit.Text <- lonLat |> LonLatCoords.getLonLatString false
                this.LatLineEdit.TooltipText <- this.LatLineEdit.Text
            else
                this.IdLineEdit.Text <- "-"
                this.ChunkLineEdit.Text <- "-"
                this.CoordsLineEdit.Text <- "-"
                this.CoordsLineEdit.TooltipText <- null
                this.HeightLineEdit.Text <- "-"
                this.ElevationLineEdit.Text <- "-"
                this.LonLineEdit.Text <- "-"
                this.LonLineEdit.TooltipText <- "" // 试了一下，null 和 "" 效果一样
                this.LatLineEdit.Text <- "-"
                this.LatLineEdit.TooltipText <- null

    let updateNewPlanetInfo
        (env: 'E when 'E :> IPlanetHudQuery and 'E :> IPlanetConfigQuery and 'E :> IEntityStoreQuery)
        : UpdateNewPlanetInfo =
        fun () ->
            match env.PlanetHudOpt with
            | None -> ()
            | Some this ->
                let planetConfig = env.PlanetConfig
                this.RadiusLineEdit.Text <- $"{planetConfig.Radius:F2}"
                this.DivisionLineEdit.Text <- $"{planetConfig.Divisions}"
                this.ChunkDivisionLineEdit.Text <- $"{planetConfig.ChunkDivisions}"
                this.ChunkCountLabel.Text <- $"分块总数：{env.Query<ChunkPos>().Count}"
                this.TileCountLabel.Text <- $"地块总数：{env.Query<TileValue>().Count}"
                this.ChosenTileId <- Nullable()

    let private handleInput
        (env: 'E when 'E :> ISelectTileViewerQuery and 'E :> IUnitManagerCommand)
        (this: IPlanetHud)
        =
        // 在 SubViewportContainer 上按下鼠标左键时，获取鼠标位置地块并更新
        this.ChosenTileId <- env.GetTileIdUnderCursor()

        if this.ChosenTileId.HasValue then
            if
                this.PreviousTileId.HasValue
                && this.PreviousTileId.Value <> this.ChosenTileId.Value
            then
                validateDrag env this.ChosenTileId.Value this
            else
                this.IsDrag <- false

            if this.EditMode then
                editTiles env this.ChosenTileId.Value this
                this.ChosenTileId <- this.ChosenTileId // 刷新 GUI 地块信息
                // 编辑模式下绘制选择地块框
                match env.SelectTileViewerOpt with
                | None -> ()
                | Some viewer -> viewer.SelectedTileId <- this.ChosenTileId
            elif Input.IsActionJustPressed "choose_unit" then
                env.FindUnitPath this.ChosenTileId

            this.PreviousTileId <- this.ChosenTileId
        else
            if not this.EditMode then
                env.FindUnitPath <| Nullable()
            else
                // 清理选择地块框
                match env.SelectTileViewerOpt with
                | None -> ()
                | Some viewer -> viewer.SelectedTileId <- Nullable()

            this.PreviousTileId <- Nullable()

    let onPlanetHudProcessed
        (env: 'E when 'E :> IPlanetHudQuery and 'E :> IMiniMapManagerCommand and 'E :> IUnitManagerCommand)
        : OnPlanetHudProcessed =
        fun () ->
            match env.PlanetHudOpt with
            | Some planetHud ->
                let mutable directReturn = false

                if planetHud.GetViewport().GuiGetHoveredControl() = (planetHud :?> Control) then
                    if Input.IsMouseButtonPressed MouseButton.Left then
                        handleInput env planetHud
                        directReturn <- true
                    elif Input.IsActionJustPressed("destroy_unit") then
                        let tileId = env.GetTileIdUnderCursor()

                        if tileId.HasValue then
                            let tile = env.GetTile tileId.Value
                            let unitId = tile |> Tile.unitId |> _.UnitId

                            if unitId > 0 then
                                GD.Print $"DestroyUnit {unitId} at tile {tile.Id}"
                                env.RemoveUnit unitId

                        directReturn <- true
                    elif Input.IsActionJustPressed("create_unit") then // 必须是 elif，因为按 U 键和 Shift + U 键是同时成立的……
                        let tileId = env.GetTileIdUnderCursor()

                        if not tileId.HasValue then
                            GD.Print "CreateUnit failed: no tile id under cursor"
                        else
                            let tile = env.GetTile tileId.Value
                            let unitId = tile |> Tile.unitId |> _.UnitId

                            if unitId > 0 then
                                GD.Print $"CreateUnit failed: tile {tile.Id} has unit {unitId}"
                            else
                                GD.Print $"CreateUnit at tile {tile.Id}"
                                env.AddUnit tile.Id <| GD.Randf() * Mathf.Tau

                        directReturn <- true
                elif
                    planetHud.GetViewport().GuiGetHoveredControl() = planetHud.MiniMapContainer
                    && Input.IsMouseButtonPressed MouseButton.Left
                then
                    env.ClickOnMiniMap()

                if not directReturn then
                    planetHud.PreviousTileId <- Nullable()
            | None -> ()
