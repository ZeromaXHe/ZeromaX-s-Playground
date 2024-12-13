namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type OptionalToggle =
    | Ignore = 0
    | Yes = 1
    | No = 2

type HexMapEditorFS() as this =
    inherit Control()

    let _hexGrid =
        lazy this.GetNode<HexGridFS> "SubViewportContainer/SubViewport/HexGrid"

    let _tabContainer = lazy this.GetNode<TabContainer> "TabC"

    let _gameUi = lazy this.GetNode<HexGameUiFS> "TabC/GameUi"

    let _terrainModeOptionButton =
        lazy this.GetNode<OptionButton> "TabC/Editor/ScrollC/CellVBox/TerrainHBox/TerrainMode"

    let _elevationChangeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/ElevationChange"

    let _elevationSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/ElevationSlider"

    let _waterChangeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/WaterChange"

    let _waterSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/WaterSlider"

    let _urbanChangeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/UrbanChange"

    let _urbanSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/UrbanSlider"

    let _farmChangeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/FarmChange"

    let _farmSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/FarmSlider"

    let _plantChangeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/PlantChange"

    let _plantSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/PlantSlider"

    let _brushSlider =
        lazy this.GetNode<HSlider> "TabC/Editor/ScrollC/CellVBox/BrushSlider"

    let _riverModeOptionButton =
        lazy this.GetNode<OptionButton> "TabC/Editor/ScrollC/CellVBox/RiverHBox/RiverMode"

    let _roadModeOptionButton =
        lazy this.GetNode<OptionButton> "TabC/Editor/ScrollC/CellVBox/RoadHBox/RoadMode"

    let _walledModeOptionButton =
        lazy this.GetNode<OptionButton> "TabC/Editor/ScrollC/CellVBox/WalledHBox/WalledMode"

    let _specialModeOptionButton =
        lazy this.GetNode<OptionButton> "TabC/Editor/ScrollC/CellVBox/SpecialHBox/SpecialMode"

    let _showGridCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/ShowGrid"

    let _showMapDataCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/ShowMapData"

    let _wireframeCheckButton =
        lazy this.GetNode<CheckButton> "TabC/Editor/ScrollC/CellVBox/Wireframe"

    let _saveButton =
        lazy this.GetNode<Button> "TabC/Editor/ScrollC/CellVBox/SaveLoadHBox/SaveButton"

    let _loadButton =
        lazy this.GetNode<Button> "TabC/Editor/ScrollC/CellVBox/SaveLoadHBox/LoadButton"

    let _newMapButton =
        lazy this.GetNode<Button> "TabC/Editor/ScrollC/CellVBox/NewMapButton"

    let _newMapMenu = lazy this.GetNode<NewMapMenuFS> "NewMapMenu"

    let _saveLoadMenu = lazy this.GetNode<SaveLoadMenuFS> "SaveLoadMenu"

    let mutable activeTerrainTypeIndex = 0
    // 默认修改高度
    let mutable changeElevation = true
    let mutable activeElevation = 1
    let mutable changeWaterLevel = false
    let mutable activeWaterLevel = 0
    let mutable changeUrbanLevel = false
    let mutable activeUrbanLevel = 0
    let mutable changeFarmLevel = false
    let mutable activeFarmLevel = 0
    let mutable changePlantLevel = false
    let mutable activePlantLevel = 0
    let mutable changeSpecial = false
    let mutable activeSpecial = 0
    let mutable brushSize = 0
    let mutable riverMode = OptionalToggle.Ignore
    let mutable roadMode = OptionalToggle.Ignore
    let mutable walledMode = OptionalToggle.Ignore
    let mutable inDragProcess = false
    // dragDirection 的有无对应教程中的 isDrag（即 isDrag 都替换为 dragDirection.IsSome）
    let mutable dragDirection: HexDirection option = None
    let mutable previousCell: HexCellFS option = None

    let validateDrag (cell: HexCellFS) =
        if previousCell.IsNone || previousCell.Value = cell then
            None
        else
            allHexDirs ()
            |> List.tryFind (fun dir -> previousCell.IsSome && previousCell.Value.GetNeighbor dir = Some cell)

    let editCell (cell: HexCellFS) =
        if activeTerrainTypeIndex > 0 then
            cell.TerrainTypeIndex <- activeTerrainTypeIndex - 1

        if changeElevation then
            cell.Elevation <- activeElevation

        if changeWaterLevel then
            cell.WaterLevel <- activeWaterLevel

        if changeSpecial then
            cell.SpecialIndex <- activeSpecial

        if changeUrbanLevel then
            cell.UrbanLevel <- activeUrbanLevel

        if changeFarmLevel then
            cell.FarmLevel <- activeFarmLevel

        if changePlantLevel then
            cell.PlantLevel <- activePlantLevel

        if riverMode = OptionalToggle.No then
            // GD.Print $"cell removeRiver"
            cell.RemoveRiver()

        if roadMode = OptionalToggle.No then
            cell.RemoveRoads()

        if walledMode <> OptionalToggle.Ignore then
            cell.Walled <- (walledMode = OptionalToggle.Yes)

        if dragDirection.IsSome then
            // dragDirection.IsSome 对应 isDrag
            cell.GetNeighbor <| dragDirection.Value.Opposite()
            |> Option.iter (fun otherCell ->
                if riverMode = OptionalToggle.Yes then
                    // GD.Print $"otherCell setRiver {dragDirection.Value}"
                    otherCell.SetOutgoingRiver <| dragDirection.Value

                if roadMode = OptionalToggle.Yes then
                    otherCell.AddRoad dragDirection.Value)

    let editCells (center: HexCellFS) =
        let centerX = center.Coordinates.X
        let centerZ = center.Coordinates.Z
        // F# for ... to 写法（区别 F# for ... in 写法）
        for z = centerZ - brushSize to centerZ do
            let r = z - (centerZ - brushSize)

            for x = centerX - r to centerX + brushSize do
                let coords = HexCoordinates(x, z)
                _hexGrid.Value.GetCell coords |> Option.iter editCell
        // F# for ... downto 写法
        for z = centerZ + brushSize downto centerZ + 1 do
            let r = centerZ + brushSize - z

            for x = centerX - brushSize to centerX + r do
                let coords = HexCoordinates(x, z)
                _hexGrid.Value.GetCell coords |> Option.iter editCell

    /// 显示/隐藏 UI
    let showUI visible = _hexGrid.Value.ShowUI visible
    // 显示/隐藏网格材质
    let showGrid visible = _hexGrid.Value.ShowGrid visible

    let getCellUnderCursor () = _hexGrid.Value.GetRayCell()

    let handleCurrentCell cell =
        dragDirection <- validateDrag cell
        editCells cell
        previousCell <- Some cell

    let createUnit () =
        getCellUnderCursor ()
        |> Option.filter _.Unit.IsNone
        |> Option.iter (fun cell ->
            let unit = HexUnitFS.unitPrefab.Instantiate<HexUnitFS>()
            let orientation = GD.Randf() * 360f
            _hexGrid.Value.AddUnit unit cell orientation)

    let destroyUnit () =
        getCellUnderCursor ()
        |> Option.filter _.Unit.IsNone
        |> Option.bind _.Unit
        |> Option.iter (fun u -> _hexGrid.Value.RemoveUnit(u :?> HexUnitFS))

    member this.SetEditMode toggle =
        this.SetProcess toggle
        _gameUi.Value.SetEditMode toggle
        RenderingServer.GlobalShaderParameterSet("hex_map_edit_mode", toggle)

    override this._Ready() =
        _tabContainer.Value.add_TabChanged (fun tab -> this.SetEditMode(int tab = 0))
        _tabContainer.Value.CurrentTab <- 0
        this.SetEditMode true // 修改 CurrentTab 不会触发 TabChanged 事件，所以需要手动设置
        // 编辑界面
        _terrainModeOptionButton.Value.Selected <- activeTerrainTypeIndex
        _elevationSlider.Value.Value <- activeElevation
        _waterSlider.Value.Value <- activeWaterLevel
        _urbanSlider.Value.Value <- activeUrbanLevel
        _farmSlider.Value.Value <- activeFarmLevel
        _plantSlider.Value.Value <- activePlantLevel
        _brushSlider.Value.Value <- brushSize
        _elevationChangeCheckButton.Value.ButtonPressed <- changeElevation
        _waterChangeCheckButton.Value.ButtonPressed <- changeWaterLevel
        _urbanChangeCheckButton.Value.ButtonPressed <- changeUrbanLevel
        _farmChangeCheckButton.Value.ButtonPressed <- changeFarmLevel
        _plantChangeCheckButton.Value.ButtonPressed <- changePlantLevel
        _riverModeOptionButton.Value.Selected <- 0
        _roadModeOptionButton.Value.Selected <- 0
        _walledModeOptionButton.Value.Selected <- 0
        _specialModeOptionButton.Value.Selected <- 0
        _newMapMenu.Value.Hide()
        _saveLoadMenu.Value.Hide()

        _terrainModeOptionButton.Value.add_ItemSelected (fun index -> activeTerrainTypeIndex <- int index)
        _elevationChangeCheckButton.Value.add_Toggled (fun toggle -> changeElevation <- toggle)
        _elevationSlider.Value.add_ValueChanged (fun value -> activeElevation <- int value)
        _waterChangeCheckButton.Value.add_Toggled (fun toggle -> changeWaterLevel <- toggle)
        _waterSlider.Value.add_ValueChanged (fun value -> activeWaterLevel <- int value)
        _urbanChangeCheckButton.Value.add_Toggled (fun toggle -> changeUrbanLevel <- toggle)
        _urbanSlider.Value.add_ValueChanged (fun value -> activeUrbanLevel <- int value)
        _farmChangeCheckButton.Value.add_Toggled (fun toggle -> changeFarmLevel <- toggle)
        _farmSlider.Value.add_ValueChanged (fun value -> activeFarmLevel <- int value)
        _plantChangeCheckButton.Value.add_Toggled (fun toggle -> changePlantLevel <- toggle)
        _plantSlider.Value.add_ValueChanged (fun value -> activePlantLevel <- int value)
        _brushSlider.Value.add_ValueChanged (fun value -> brushSize <- int value)
        _riverModeOptionButton.Value.add_ItemSelected (fun index -> riverMode <- enum<OptionalToggle> <| int index)
        _roadModeOptionButton.Value.add_ItemSelected (fun index -> roadMode <- enum<OptionalToggle> <| int index)
        _walledModeOptionButton.Value.add_ItemSelected (fun index -> walledMode <- enum<OptionalToggle> <| int index)

        _specialModeOptionButton.Value.add_ItemSelected (fun index ->
            activeSpecial <- int index - 1
            changeSpecial <- int index <> 0)

        _showGridCheckButton.Value.add_Toggled showGrid
        _showMapDataCheckButton.Value.add_Toggled (fun toggle ->
            RenderingServer.GlobalShaderParameterSet("show_map_data", toggle))

        _wireframeCheckButton.Value.add_Toggled (fun toggle ->
            if toggle then
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Wireframe)
            else
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Disabled))
        // 我们需要这样刷新一下新地图的 HexCell 标签的显示和 HexChunk 的网格材质显示，因为我们的标签、网格材质 Shader 实现和教程不同
        _saveButton.Value.add_Pressed (fun _ -> _saveLoadMenu.Value.Open true _showGridCheckButton.Value.ButtonPressed)

        _loadButton.Value.add_Pressed (fun _ -> _saveLoadMenu.Value.Open false _showGridCheckButton.Value.ButtonPressed)

        _newMapButton.Value.add_Pressed (fun _ -> _newMapMenu.Value.Open _showGridCheckButton.Value.ButtonPressed)
        // 默认显示网格，放在最后是为了触发事件调用 showGrid
        _showGridCheckButton.Value.ButtonPressed <- true
        _showMapDataCheckButton.Value.ButtonPressed <- false

    override this._Process _ =
        if inDragProcess && previousCell.IsSome then
            match getCellUnderCursor () with
            | Some cell -> handleCurrentCell cell
            | None -> previousCell <- None

    override this._UnhandledInput e =
        // 编辑模式
        if _tabContainer.Value.CurrentTab = 0 then
            match e with
            | :? InputEventMouseButton as b when b.ButtonIndex = MouseButton.Left ->
                match getCellUnderCursor () with
                | Some cell ->
                    // 仅当左键按下，且是在单元格上的时候，开启拖拽过程
                    inDragProcess <- b.Pressed
                    previousCell <- None

                    if inDragProcess then
                        handleCurrentCell cell
                | None -> inDragProcess <- false
            | :? InputEventKey as k when k.Keycode = Key.U && k.Pressed && k.ShiftPressed -> destroyUnit ()
            | :? InputEventKey as k when k.Keycode = Key.U && k.Pressed -> createUnit ()
            | _ -> ()
