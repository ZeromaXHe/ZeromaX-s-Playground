namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

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

    let _showLabelsCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/ShowLabels")

    let _wireframeCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/Wireframe")

    // 默认不变色
    let mutable changeColor = false
    let mutable activeColor = Colors.White
    // 默认修改高度
    let mutable changeElevation = true
    let mutable activeElevation = 1
    let mutable brushSize = 0

    let setColor index =
        changeColor <- true
        activeColor <- this._colors[index]

    let editCell (cellOpt: HexCellFS option) =
        cellOpt
        |> Option.iter (fun cell ->
            if changeColor then
                cell.Color <- activeColor

            if changeElevation then
                cell.Elevation <- activeElevation)

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
        _showLabelsCheckButton.Value.add_Toggled showUI

        _wireframeCheckButton.Value.add_Toggled (fun toggle ->
            if toggle then
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Wireframe)
            else
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Disabled))
        // 默认隐藏 UI
        _showLabelsCheckButton.Value.ButtonPressed <- false

    override this._UnhandledInput e =

        match e with
        | :? InputEventMouseButton as b when b.Pressed && b.ButtonIndex = MouseButton.Left ->
            // GD.Print "Mouse left button pressed"
            if changeColor || changeElevation then
                let result = _hexGrid.Value.CameraRayCastToMouse()

                if result = null then
                    GD.Print "rayCast null result"
                elif result.Count = 0 then
                    GD.Print "rayCast empty result"
                else
                    let bool, res = result.TryGetValue "position"

                    if bool then
                        let pos = res.As<Vector3>()
                        _hexGrid.Value.GetCell pos |> Option.iter editCells
                    else
                        GD.Print "rayCast no position"
        | _ -> ()
