namespace FrontEndToolFS.Tool

open Godot

type HexMapEditorFS() as this =
    inherit Control()

    let _hexGrid =
        lazy this.GetNode<HexGridFS>("SubViewportContainer/SubViewport/HexGrid")

    let _yellowCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/ColorVBox/Yellow")
    let _greenCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/ColorVBox/Green")
    let _blueCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/ColorVBox/Blue")
    let _whiteCheckBox = lazy this.GetNode<CheckBox>("PanelContainer/ColorVBox/White")
    let mutable activeColor = Colors.White

    let setColor index = activeColor <- this._colors[index]

    member val _colors: Color array = Array.empty with get, set

    override this._Ready() =
        _yellowCheckBox.Value.add_Pressed (fun _ -> setColor 0)
        _greenCheckBox.Value.add_Pressed (fun _ -> setColor 1)
        _blueCheckBox.Value.add_Pressed (fun _ -> setColor 2)
        _whiteCheckBox.Value.add_Pressed (fun _ -> setColor 3)

    override this._Input e =
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
                    _hexGrid.Value.ColorCell pos activeColor
                else
                    GD.Print "rayCast no position"
        | _ -> ()
