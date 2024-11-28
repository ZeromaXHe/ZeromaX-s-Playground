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
        lazy this.GetNode<HexGridFS>("SubViewportContainer/SubViewport/HexGrid")

    let _yellowCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Yellow")
    let _greenCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Green")
    let _blueCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Blue")
    let _whiteCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/White")

    let _noChangeColorCheckBox =
        lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/NoChangeColor")

    let _elevationChangeCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/ElevationChange")

    let _elevationSlider =
        lazy this.GetNode<HSlider>("PanelContainer/CellVBox/ElevationSlider")

    let _brushSlider = lazy this.GetNode<HSlider>("PanelContainer/CellVBox/BrushSlider")

    let _riverModeOptionButton =
        lazy this.GetNode<OptionButton>("PanelContainer/CellVBox/RiverHBox/RiverMode")

    let _showLabelsCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/ShowLabels")

    let _wireframeCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/Wireframe")

    // 默认不变色
    let mutable changeColor = false
    let mutable activeColor = Colors.White

    let setColor index =
        changeColor <- true
        activeColor <- this._colors[index]
    // 默认修改高度
    let mutable changeElevation = true
    let mutable activeElevation = 1
    let mutable brushSize = 0
    let mutable riverMode = OptionalToggle.Ignore
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

    let editCell (cellOpt: HexCellFS option) =
        cellOpt
        |> Option.iter (fun cell ->
            if changeColor then
                cell.Color <- activeColor

            if changeElevation then
                cell.Elevation <- activeElevation

            if riverMode = OptionalToggle.No then
                // GD.Print $"cell removeRiver"
                cell.RemoveRiver()
            elif dragDirection.IsSome && riverMode = OptionalToggle.Yes then
                // dragDirection.IsSome 对应 isDrag
                GD.Print $"cell setRiver {dragDirection.Value}"

                cell.GetNeighbor <| dragDirection.Value.Opposite()
                |> Option.iter (fun otherCell -> otherCell.SetOutgoingRiver dragDirection.Value))

    let editCells (center: HexCellFS) =
        let centerX = center.Coordinates.X
        let centerZ = center.Coordinates.Z
        // F# for ... to 写法（区别 F# for ... in 写法）
        for z = centerZ - brushSize to centerZ do
            let r = z - (centerZ - brushSize)

            for x = centerX - r to centerX + brushSize do
                let coords = HexCoordinates(x, z)
                editCell <| _hexGrid.Value.GetCell coords
        // F# for ... downto 写法
        for z = centerZ + brushSize downto centerZ + 1 do
            let r = centerZ + brushSize - z

            for x = centerX - brushSize to centerX + r do
                let coords = HexCoordinates(x, z)
                editCell <| _hexGrid.Value.GetCell coords

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

    member val _colors: Color array = Array.empty with get, set

    override this._Ready() =
        _elevationSlider.Value.Value <- activeElevation
        _brushSlider.Value.Value <- brushSize
        _noChangeColorCheckBox.Value.ButtonPressed <- not changeColor
        _elevationChangeCheckButton.Value.ButtonPressed <- changeElevation

        _yellowCheckBox.Value.add_Pressed (fun _ -> setColor 0)
        _greenCheckBox.Value.add_Pressed (fun _ -> setColor 1)
        _blueCheckBox.Value.add_Pressed (fun _ -> setColor 2)
        _whiteCheckBox.Value.add_Pressed (fun _ -> setColor 3)
        _noChangeColorCheckBox.Value.add_Pressed (fun _ -> changeColor <- false)
        _elevationChangeCheckButton.Value.add_Toggled (fun toggle -> changeElevation <- toggle)
        _elevationSlider.Value.add_ValueChanged (fun value -> activeElevation <- int value)
        _brushSlider.Value.add_ValueChanged (fun value -> brushSize <- int value)
        _riverModeOptionButton.Value.add_ItemSelected (fun index -> riverMode <- enum<OptionalToggle> <| int index)
        _showLabelsCheckButton.Value.add_Toggled showUI

        _wireframeCheckButton.Value.add_Toggled (fun toggle ->
            if toggle then
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Wireframe)
            else
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Disabled))
        // 默认隐藏 UI
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
