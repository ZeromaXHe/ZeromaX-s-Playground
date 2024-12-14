namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type NewMapMenuFS() as this =
    inherit PanelContainer()

    let _generateCheckBox =
        lazy this.GetNode<CheckBox> "CenterC/MenuPanel/MarginC/VBox/GenerateCheckBox"

    let _wrappingCheckBox =
        lazy this.GetNode<CheckBox> "CenterC/MenuPanel/MarginC/VBox/WrappingCheckBox"

    let _smallButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/SmallButton"

    let _midButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/MidButton"

    let _largeButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/LargeButton"

    let _cancelButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/CancelButton"

    let mutable gridVisible = false
    let mutable generateMaps = true
    let mutable wrapping = true

    [<DefaultValue>]
    val mutable hexGrid: HexGridFS

    [<DefaultValue>]
    val mutable mapGenerator: HexMapGeneratorFS

    let createMap x z =
        if generateMaps then
            this.mapGenerator.GenerateMap x z wrapping
        else
            this.hexGrid.CreateMap x z wrapping |> ignore

        this.hexGrid.ShowGrid gridVisible // 刷新显示网格与否
        HexMapCameraFS.ValidatePosition()
        this.Close()

    member this.Open gridOn =
        this.Show()
        HexMapCameraFS.Locked <- true
        gridVisible <- gridOn

    member this.Close() =
        this.Hide()
        HexMapCameraFS.Locked <- false

    override this._Ready() =
        _generateCheckBox.Value.ButtonPressed <- generateMaps
        _wrappingCheckBox.Value.ButtonPressed <- wrapping

        _generateCheckBox.Value.add_Toggled (fun toggle -> generateMaps <- toggle)
        _wrappingCheckBox.Value.add_Toggled (fun toggle -> wrapping <- toggle)
        _smallButton.Value.add_Pressed (fun () -> createMap 20 15)
        _midButton.Value.add_Pressed (fun () -> createMap 40 30)
        _largeButton.Value.add_Pressed (fun () -> createMap 80 60)
        _cancelButton.Value.add_Pressed (fun () -> this.Close())
