namespace FrontEndToolFS.Tool

open Godot

type HexGameUiFS() as this =
    inherit PanelContainer()

    let showPathCheckButton = lazy this.GetNode<CheckButton> "ScrollC/VBox/ShowPath"

    [<DefaultValue>]
    val mutable grid: HexGridFS

    let mutable editMode = true
    let mutable currentCellIndex = -1
    let mutable selectedUnit: HexUnitFS option = None

    let updateCurrentCell () =
        let cell = this.grid.GetRayCell()
        let index = cell |> Option.map _.Index |> Option.defaultValue -1

        if index <> currentCellIndex then
            currentCellIndex <- index
            true
        else
            false

    let doSelection () =
        this.grid.ClearPath()
        updateCurrentCell () |> ignore

        if currentCellIndex >= 0 then
            selectedUnit <-
                (this.grid.GetCell currentCellIndex).Unit
                |> Option.map (fun u -> u :?> HexUnitFS)

    let doPathfinding () =
        if updateCurrentCell () then
            if
                currentCellIndex >= 0
                && selectedUnit.Value.IsValidDestination <| this.grid.GetCell currentCellIndex
            then
                this.grid.FindPath selectedUnit.Value.Location (this.grid.GetCell currentCellIndex) selectedUnit.Value
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

        if not toggle then
            this.grid.PathShowerOn <- showPathCheckButton.Value.ButtonPressed

    override this._Ready() =
        showPathCheckButton.Value.ButtonPressed <- false
        showPathCheckButton.Value.add_Toggled (fun toggle -> this.grid.PathShowerOn <- toggle)

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
