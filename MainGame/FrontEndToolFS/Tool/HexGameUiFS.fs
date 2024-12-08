namespace FrontEndToolFS.Tool

open Godot

type HexGameUiFS() as this =
    inherit PanelContainer()

    [<DefaultValue>]
    val mutable grid: HexGridFS

    let mutable editMode = true
    let mutable currentCell: HexCellFS option = None
    let mutable selectedUnit: HexUnitFS option = None

    let updateCurrentCell () =
        let cell = this.grid.GetRayCell()

        if cell <> currentCell then
            currentCell <- cell
            true
        else
            false

    let doSelection () =
        this.grid.ClearPath()
        updateCurrentCell () |> ignore

        if currentCell.IsSome then
            selectedUnit <- currentCell.Value.Unit |> Option.map (fun u -> u :?> HexUnitFS)

    let doPathfinding () =
        if updateCurrentCell () then
            if currentCell.IsSome && selectedUnit.Value.IsValidDestination currentCell.Value then
                this.grid.FindPath selectedUnit.Value.Location.Value currentCell.Value 24
            else
                this.grid.ClearPath()

    let doMove () =
        if this.grid.HasPath then
            selectedUnit.Value.Travel <| this.grid.GetPath()
            this.grid.ClearPath()

    member this.SetEditMode toggle =
        this.grid.ShowUI <| not toggle
        editMode <- toggle
        this.SetProcess <| not toggle
        this.grid.ClearPath()

    override this._UnhandledInput e =
        if not editMode then
            match e with
            | :? InputEventMouseButton as b when b.ButtonIndex = MouseButton.Left && b.Pressed -> doSelection ()
            | _ -> ()

    override this._Process _ =
        if selectedUnit.IsSome then
            if Input.IsMouseButtonPressed MouseButton.Right then
                doMove ()
            else
                doPathfinding ()
