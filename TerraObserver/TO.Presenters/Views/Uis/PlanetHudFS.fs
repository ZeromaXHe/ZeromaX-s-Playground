namespace TO.Presenters.Views.Uis

open System
open Godot
open TO.Domains.Enums.Meshes
open TO.Domains.Shaders
open TO.Domains.Structs.HexSphereGrids
open TO.Presenters.Models.Uis

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 17:56:22
[<AbstractClass>]
type PlanetHudFS() =
    inherit Control()
    let mutable chosenTileId: int Nullable = Nullable()
    // =====【on-ready】=====
    let mutable wireframeCheckButton: CheckButton = null
    let mutable celestialMotionCheckButton: CheckButton = null
    // 小地图
    let mutable miniMapContainer: SubViewportContainer = null
    let mutable camLonLatLabel: Label = null
    let mutable lonLatFixCheckButton: CheckButton = null
    // 指南针
    let mutable compassPanel: PanelContainer = null
    // 矩形地图测试
    let mutable rectMap: TextureRect = null
    // 星球信息
    let mutable planetTabBar: TabBar = null
    let mutable planetGrid: GridContainer = null
    let mutable radiusLineEdit: LineEdit = null
    let mutable divisionLineEdit: LineEdit = null
    let mutable chunkDivisionLineEdit: LineEdit = null
    // 地块信息
    let mutable tileTabBar: TabBar = null
    let mutable tileVBox: VBoxContainer = null
    let mutable chunkCountLabel: Label = null
    let mutable tileCountLabel: Label = null
    let mutable showLableOptionButton: OptionButton = null
    let mutable tileGrid: GridContainer = null
    let mutable idLineEdit: LineEdit = null
    let mutable chunkLineEdit: LineEdit = null
    let mutable coordsLineEdit: LineEdit = null
    let mutable heightLineEdit: LineEdit = null
    let mutable elevationLineEdit: LineEdit = null
    let mutable lonLineEdit: LineEdit = null
    let mutable latLineEdit: LineEdit = null
    // 编辑功能
    let mutable editCheckButton: CheckButton = null
    let mutable editTabBar: TabBar = null
    let mutable editGrid: GridContainer = null
    let mutable terrainOptionButton: OptionButton = null
    let mutable elevationVSlider: VSlider = null
    let mutable elevationCheckButton: CheckButton = null
    let mutable elevationValueLabel: Label = null
    let mutable waterVSlider: VSlider = null
    let mutable waterCheckButton: CheckButton = null
    let mutable waterValueLabel: Label = null
    let mutable brushLabel: Label = null
    let mutable brushHSlider: HSlider = null
    let mutable riverOptionButton: OptionButton = null
    let mutable roadOptionButton: OptionButton = null
    let mutable urbanCheckButton: CheckButton = null
    let mutable urbanHSlider: HSlider = null
    let mutable farmCheckButton: CheckButton = null
    let mutable farmHSlider: HSlider = null
    let mutable plantCheckButton: CheckButton = null
    let mutable plantHSlider: HSlider = null
    let mutable wallOptionButton: OptionButton = null
    let mutable specialFeatureCheckButton: CheckButton = null
    let mutable specialFeatureOptionButton: OptionButton = null
    member this.CelestialMotionCheckButton = celestialMotionCheckButton
    member this.LonLatFixCheckButton = lonLatFixCheckButton
    member this.CompassPanel = compassPanel
    member this.RectMap = rectMap
    member this.RadiusLineEdit = radiusLineEdit
    member this.DivisionLineEdit = divisionLineEdit
    member this.ChunkDivisionLineEdit = chunkDivisionLineEdit
    member this.ElevationVSlider = elevationVSlider
    member this.WaterVSlider = waterVSlider
    // =====【事件】=====
    abstract EmitChosenTileIdChanged: int Nullable -> unit
    // =====【属性】=====
    member this.ChosenTileId
        with get () = chosenTileId
        and set v =
            chosenTileId <- v
            this.EmitChosenTileIdChanged chosenTileId

    member val IsDrag = false with get, set
    member val DragTileId: int Nullable = Nullable() with get, set
    member val PreviousTileId: int Nullable = Nullable() with get, set
    member val LabelMode = 0 with get, set
    member val TileOverrider = HexTileDataOverrider()
    // =====【生命周期】=====
    override this._Ready() : unit =
        wireframeCheckButton <- this.GetNode<CheckButton>("%WireframeCheckButton")
        celestialMotionCheckButton <- this.GetNode<CheckButton>("%CelestialMotionCheckButton")
        // 小地图
        miniMapContainer <- this.GetNode<SubViewportContainer>("%MiniMapContainer")
        camLonLatLabel <- this.GetNode<Label>("%CamLonLatLabel")
        lonLatFixCheckButton <- this.GetNode<CheckButton>("%LonLatFixCheckButton")
        // 指南针
        compassPanel <- this.GetNode<PanelContainer>("%CompassPanel")
        // 矩形地图测试
        rectMap <- this.GetNode<TextureRect>("%RectMap")
        // 星球信息
        planetTabBar <- this.GetNode<TabBar>("%PlanetTabBar")
        planetGrid <- this.GetNode<GridContainer>("%PlanetGrid")
        radiusLineEdit <- this.GetNode<LineEdit>("%RadiusLineEdit")
        divisionLineEdit <- this.GetNode<LineEdit>("%DivisionLineEdit")
        chunkDivisionLineEdit <- this.GetNode<LineEdit>("%ChunkDivisionLineEdit")
        // 地块信息
        tileTabBar <- this.GetNode<TabBar>("%TileTabBar")
        tileVBox <- this.GetNode<VBoxContainer>("%TileVBox")
        chunkCountLabel <- this.GetNode<Label>("%ChunkCountLabel")
        tileCountLabel <- this.GetNode<Label>("%TileCountLabel")
        showLableOptionButton <- this.GetNode<OptionButton>("%ShowLabelOptionButton")
        tileGrid <- this.GetNode<GridContainer>("%TileGrid")
        idLineEdit <- this.GetNode<LineEdit>("%IdLineEdit")
        chunkLineEdit <- this.GetNode<LineEdit>("%ChunkLineEdit")
        coordsLineEdit <- this.GetNode<LineEdit>("%CoordsLineEdit")
        heightLineEdit <- this.GetNode<LineEdit>("%HeightLineEdit")
        elevationLineEdit <- this.GetNode<LineEdit>("%ElevationLineEdit")
        lonLineEdit <- this.GetNode<LineEdit>("%LonLineEdit")
        latLineEdit <- this.GetNode<LineEdit>("%LatLineEdit")
        // 编辑功能
        editCheckButton <- this.GetNode<CheckButton>("%EditCheckButton")
        editTabBar <- this.GetNode<TabBar>("%EditTabBar")
        editGrid <- this.GetNode<GridContainer>("%EditGrid")
        terrainOptionButton <- this.GetNode<OptionButton>("%TerrainOptionButton")
        elevationVSlider <- this.GetNode<VSlider>("%ElevationVSlider")
        elevationCheckButton <- this.GetNode<CheckButton>("%ElevationCheckButton")
        elevationValueLabel <- this.GetNode<Label>("%ElevationValueLabel")
        waterVSlider <- this.GetNode<VSlider>("%WaterVSlider")
        waterCheckButton <- this.GetNode<CheckButton>("%WaterCheckButton")
        waterValueLabel <- this.GetNode<Label>("%WaterValueLabel")
        brushLabel <- this.GetNode<Label>("%BrushLabel")
        brushHSlider <- this.GetNode<HSlider>("%BrushHSlider")
        riverOptionButton <- this.GetNode<OptionButton>("%RiverOptionButton")
        roadOptionButton <- this.GetNode<OptionButton>("%RoadOptionButton")
        urbanCheckButton <- this.GetNode<CheckButton>("%UrbanCheckButton")
        urbanHSlider <- this.GetNode<HSlider>("%UrbanHSlider")
        farmCheckButton <- this.GetNode<CheckButton>("%FarmCheckButton")
        farmHSlider <- this.GetNode<HSlider>("%FarmHSlider")
        plantCheckButton <- this.GetNode<CheckButton>("%PlantCheckButton")
        plantHSlider <- this.GetNode<HSlider>("%PlantHSlider")
        wallOptionButton <- this.GetNode<OptionButton>("%WallOptionButton")
        specialFeatureCheckButton <- this.GetNode<CheckButton>("%SpecialFeatureCheckButton")
        specialFeatureOptionButton <- this.GetNode<OptionButton>("%SpecialFeatureOptionButton")
        this.SetEditMode editCheckButton.ButtonPressed
        this.SetLabelMode showLableOptionButton.Selected
        this.SetTerrain 0

        wireframeCheckButton.add_Toggled (fun toggle ->
            this
                .GetViewport()
                .SetDebugDraw(
                    if toggle then
                        Viewport.DebugDrawEnum.Wireframe
                    else
                        Viewport.DebugDrawEnum.Disabled
                ))

        planetTabBar.add_TabClicked (fun _ -> planetGrid.Visible <- not planetGrid.Visible)

        tileTabBar.add_TabClicked (fun _ ->
            let vis = not tileGrid.Visible
            tileVBox.Visible <- vis
            tileGrid.Visible <- vis)

        editTabBar.add_TabClicked (fun _ ->
            let vis = not editGrid.Visible
            editGrid.Visible <- vis

            if vis then
                this.SetTerrain terrainOptionButton.Selected
                this.SetElevation elevationVSlider.Value
            else
                this.SetApplyTerrain false
                this.SetApplyElevation false)

        editCheckButton.add_Toggled (fun v -> this.SetEditMode v)
        showLableOptionButton.add_ItemSelected (fun v -> this.SetLabelMode v)
        terrainOptionButton.add_ItemSelected (fun v -> this.SetTerrain v)
        elevationVSlider.add_ValueChanged (fun v -> this.SetElevation v)
        elevationCheckButton.add_Toggled (fun v -> this.SetApplyElevation v)
        waterVSlider.add_ValueChanged (fun v -> this.SetWaterLevel v)
        waterCheckButton.add_Toggled (fun v -> this.SetApplyWaterLevel v)
        brushHSlider.add_ValueChanged (fun v -> this.SetBrushSize v)
        riverOptionButton.add_ItemSelected (fun v -> this.SetRiverMode v)
        roadOptionButton.add_ItemSelected (fun v -> this.SetRoadMode v)
        urbanCheckButton.add_Toggled (fun v -> this.SetApplyUrbanLevel v)
        urbanHSlider.add_ValueChanged (fun v -> this.SetUrbanLevel v)
        farmCheckButton.add_Toggled (fun v -> this.SetApplyFarmLevel v)
        farmHSlider.add_ValueChanged (fun v -> this.SetFarmLevel v)
        plantCheckButton.add_Toggled (fun v -> this.SetApplyPlantLevel v)
        plantHSlider.add_ValueChanged (fun v -> this.SetPlantLevel v)
        wallOptionButton.add_ItemSelected (fun v -> this.SetWalledMode v)
        specialFeatureCheckButton.add_Toggled (fun v -> this.SetApplySpecialIndex v)
        specialFeatureOptionButton.add_ItemSelected (fun v -> this.SetSpecialIndex v)

    member this.OnOrbitCameraRigMoved(pos: Vector3, _) =
        let lonLat = LonLatCoords.From pos
        camLonLatLabel.Text <- $"相机位置：{lonLat}"

    member private this.SetEditMode(toggle: bool) =
        let editMode = this.TileOverrider.EditMode
        this.TileOverrider.EditMode <- toggle
        // if toggle <> editMode then
        //     EmitEditModeChanged()
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.HexMapEditMode, toggle)

    member private this.SetLabelMode(mode: int64) =
        let before = this.LabelMode
        let intMode = int mode
        this.LabelMode <- intMode
    // if before <> intMode then
    //     EmitLabelModeChanged()
    member private this.SetTerrain(index: int64) =
        this.TileOverrider.ApplyTerrain <- index > 0

        if this.TileOverrider.ApplyTerrain then
            this.TileOverrider.ActiveTerrain <- int index - 1

    member private this.SetApplyTerrain(toggle: bool) =
        this.TileOverrider.ApplyTerrain <- toggle

    member private this.SetElevation(elevation: float) =
        this.TileOverrider.ActiveElevation <- int elevation
        elevationValueLabel.Text <- this.TileOverrider.ActiveElevation.ToString()

    member private this.SetApplyElevation(toggle: bool) =
        this.TileOverrider.ApplyElevation <- toggle

    member private this.SetBrushSize(brushSize: float) =
        this.TileOverrider.BrushSize <- int brushSize
        brushLabel.Text <- $"笔刷大小：{this.TileOverrider.BrushSize}"

    member private this.SetRiverMode(mode: int64) =
        this.TileOverrider.RiverMode <- enum<OptionalToggle> <| int mode

    member private this.SetRoadMode(mode: int64) =
        this.TileOverrider.RoadMode <- enum<OptionalToggle> <| int mode

    member private this.SetApplyWaterLevel(toggle: bool) =
        this.TileOverrider.ApplyWaterLevel <- toggle

    member private this.SetWaterLevel(level: float) =
        this.TileOverrider.ActiveWaterLevel <- int level
        waterValueLabel.Text <- this.TileOverrider.ActiveWaterLevel.ToString()

    member private this.SetApplyUrbanLevel(toggle: bool) =
        this.TileOverrider.ApplyUrbanLevel <- toggle

    member private this.SetUrbanLevel(level: float) =
        this.TileOverrider.ActiveUrbanLevel <- int level

    member private this.SetApplyFarmLevel(toggle: bool) =
        this.TileOverrider.ApplyFarmLevel <- toggle

    member private this.SetFarmLevel(level: float) =
        this.TileOverrider.ActiveFarmLevel <- int level

    member private this.SetApplyPlantLevel(toggle: bool) =
        this.TileOverrider.ApplyPlantLevel <- toggle

    member private this.SetPlantLevel(level: float) =
        this.TileOverrider.ActivePlantLevel <- int level

    member private this.SetWalledMode(mode: int64) =
        this.TileOverrider.WalledMode <- enum<OptionalToggle> <| int mode

    member private this.SetApplySpecialIndex(toggle: bool) =
        this.TileOverrider.ApplySpecialIndex <- toggle

    member private this.SetSpecialIndex(index: int64) =
        this.TileOverrider.ActiveSpecialIndex <- int index
