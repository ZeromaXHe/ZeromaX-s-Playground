namespace FrontEndToolFS.Tool

open Godot

type HexMapEditorFS() as this =
    inherit Control()

    let _hexGrid =
        lazy this.GetNode<HexGridFS>("SubViewportContainer/SubViewport/HexGrid")

    let _yellowCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Yellow")
    let _greenCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Green")
    let _blueCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/Blue")
    let _whiteCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/CellVBox/White")

    let _elevationVSlider =
        lazy this.GetNode<VSlider>("PanelContainer/CellVBox/Elevation")

    let _wireframeCheckButton =
        lazy this.GetNode<CheckButton>("PanelContainer/CellVBox/Wireframe")

    let mutable activeColor = Colors.White
    let mutable activeElevation = 0

    let setColor index = activeColor <- this._colors[index]

    member val _colors: Color array = Array.empty with get, set

    override this._Ready() =
        _elevationVSlider.Value.Value <- activeElevation

        _yellowCheckBox.Value.add_Pressed (fun _ -> setColor 0)
        _greenCheckBox.Value.add_Pressed (fun _ -> setColor 1)
        _blueCheckBox.Value.add_Pressed (fun _ -> setColor 2)
        _whiteCheckBox.Value.add_Pressed (fun _ -> setColor 3)
        _elevationVSlider.Value.add_ValueChanged (fun value -> activeElevation <- int value)

        _wireframeCheckButton.Value.add_Toggled (fun toggle ->
            if toggle then
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Wireframe)
            else
                _hexGrid.Value.GetViewport().SetDebugDraw(Viewport.DebugDrawEnum.Disabled))

    override this._UnhandledInput e =
        match e with
        | :? InputEventMouseButton as b when b.Pressed && b.ButtonIndex = MouseButton.Left ->
            // GD.Print "Mouse left button pressed"
            let result = _hexGrid.Value.CameraRayCastToMouse()

            if result = null then
                GD.Print "rayCast null result"
            elif result.Count = 0 then
                GD.Print "rayCast empty result"
            else
                let bool, res = result.TryGetValue "position"

                if bool then
                    let pos = res.As<Vector3>()
                    let cell = _hexGrid.Value.GetCell pos
                    cell.Color <- activeColor
                    cell.Elevation <- activeElevation
                else
                    GD.Print "rayCast no position"
        | _ -> ()
