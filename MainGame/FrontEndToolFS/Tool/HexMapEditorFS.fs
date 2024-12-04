namespace FrontEndToolFS.Tool

open System.IO
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

    let _colorModeOptionButton =
        lazy this.GetNode<OptionButton> "PanelContainer/CellVBox/ColorHBox/ColorMode"

    let _elevationChangeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/ElevationChange"

    let _elevationSlider =
        lazy this.GetNode<HSlider> "PanelContainer/CellVBox/ElevationSlider"

    let _waterChangeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/WaterChange"

    let _waterSlider = lazy this.GetNode<HSlider> "PanelContainer/CellVBox/WaterSlider"

    let _urbanChangeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/UrbanChange"

    let _urbanSlider = lazy this.GetNode<HSlider> "PanelContainer/CellVBox/UrbanSlider"

    let _farmChangeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/FarmChange"

    let _farmSlider = lazy this.GetNode<HSlider> "PanelContainer/CellVBox/FarmSlider"

    let _plantChangeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/PlantChange"

    let _plantSlider = lazy this.GetNode<HSlider> "PanelContainer/CellVBox/PlantSlider"

    let _brushSlider = lazy this.GetNode<HSlider> "PanelContainer/CellVBox/BrushSlider"

    let _riverModeOptionButton =
        lazy this.GetNode<OptionButton> "PanelContainer/CellVBox/RiverHBox/RiverMode"

    let _roadModeOptionButton =
        lazy this.GetNode<OptionButton> "PanelContainer/CellVBox/RoadHBox/RoadMode"

    let _walledModeOptionButton =
        lazy this.GetNode<OptionButton> "PanelContainer/CellVBox/WalledHBox/WalledMode"

    let _specialModeOptionButton =
        lazy this.GetNode<OptionButton> "PanelContainer/CellVBox/SpecialHBox/SpecialMode"

    let _showLabelsCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/ShowLabels"

    let _wireframeCheckButton =
        lazy this.GetNode<CheckButton> "PanelContainer/CellVBox/Wireframe"

    let _saveButton =
        lazy this.GetNode<Button> "PanelContainer/CellVBox/SaveLoadHBox/SaveButton"

    let _loadButton =
        lazy this.GetNode<Button> "PanelContainer/CellVBox/SaveLoadHBox/LoadButton"

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

    let save () =
        let path = Path.Combine(ProjectSettings.GlobalizePath "res://save", "test.map")
        use writer = new BinaryWriter(File.Open(path, FileMode.Create))
        writer.Write 0
        _hexGrid.Value.Save writer
        GD.Print "Saved!"

    let load () =
        let path = Path.Combine(ProjectSettings.GlobalizePath "res://save", "test.map")
        use reader = new BinaryReader(File.OpenRead path)
        let header = reader.ReadInt32()

        if header = 0 then
            _hexGrid.Value.Load reader
            GD.Print "Loaded!"
        else
            GD.PrintErr $"Unknown map format {header}"

    /// 显示/隐藏 UI
    let showUI visible = _hexGrid.Value.ShowUI visible

    let getMouseHitPoint () =
        let result = _hexGrid.Value.CameraRayCastToMouse()

        if result = null || result.Count = 0 then
            // GD.Print "rayCast empty result"
            None
        else
            let bool, res = result.TryGetValue "position"

            if bool then
                Some <| res.As<Vector3>()
            else
                // GD.Print "rayCast no position"
                None

    override this._Ready() =
        _colorModeOptionButton.Value.Selected <- activeTerrainTypeIndex
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

        _colorModeOptionButton.Value.add_ItemSelected (fun index -> activeTerrainTypeIndex <- int index)
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

        _showLabelsCheckButton.Value.add_Toggled showUI

        _wireframeCheckButton.Value.add_Toggled (fun toggle ->
            if toggle then
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Wireframe)
            else
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Disabled))

        _saveButton.Value.add_Pressed (fun _ -> save ())
        _loadButton.Value.add_Pressed (fun _ -> load ())
        // 默认隐藏 UI，放在最后是为了触发事件调用 showUI
        _showLabelsCheckButton.Value.ButtonPressed <- false

    override this._Process _ =
        if inDragProcess && previousCell.IsSome then
            match getMouseHitPoint () with
            | Some pos ->
                match _hexGrid.Value.GetCell pos with
                | Some currentCell ->
                    dragDirection <- validateDrag currentCell
                    editCells currentCell
                    previousCell <- Some currentCell
                | None -> previousCell <- None
            | None -> previousCell <- None

    override this._UnhandledInput e =
        match e with
        | :? InputEventMouseButton as b when b.ButtonIndex = MouseButton.Left ->
            match getMouseHitPoint () with
            | Some pos ->
                let cellOpt = _hexGrid.Value.GetCell pos
                // 仅当左键按下，且是在单元格上的时候，开启拖拽过程
                inDragProcess <- cellOpt.IsSome && b.Pressed
                previousCell <- None

                if inDragProcess then
                    let currentCell = cellOpt.Value
                    dragDirection <- validateDrag currentCell
                    editCells currentCell
                    previousCell <- Some currentCell
            | None -> inDragProcess <- false
        | _ -> ()
