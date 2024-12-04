namespace FrontEndToolFS.Tool

open System.IO
open Godot

type SaveLoadMenuFS() as this =
    inherit PanelContainer()

    let _menuLabel = lazy this.GetNode<Label> "CenterC/MenuPanel/MarginC/VBox/Title"

    let _listContent =
        lazy this.GetNode<VBoxContainer> "CenterC/MenuPanel/MarginC/VBox/ScrollContainer/ItemVBox"

    let _nameInput =
        lazy this.GetNode<LineEdit> "CenterC/MenuPanel/MarginC/VBox/NameInput"

    let _actionButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/ButtonHBox/ActionButton"

    let _deleteButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/ButtonHBox/DeleteButton"

    let _cancelButton =
        lazy this.GetNode<Button> "CenterC/MenuPanel/MarginC/VBox/ButtonHBox/CancelButton"

    [<DefaultValue>]
    val mutable hexGrid: HexGridFS

    [<DefaultValue>]
    val mutable itemPrefab: PackedScene

    let mutable saveMode = false

    let mutable labelVisible = false

    let saveDirPath = ProjectSettings.GlobalizePath "res://save"

    let save path =
        use writer = new BinaryWriter(File.Open(path, FileMode.Create))
        writer.Write 1 // 版本号
        this.hexGrid.Save writer
        GD.Print "Saved!"

    let load path =
        if not <| File.Exists path then
            GD.PrintErr "File does not exist"
        else
            use reader = new BinaryReader(File.OpenRead path)
            let header = reader.ReadInt32()

            if header <= 1 then
                this.hexGrid.Load reader header
                this.hexGrid.ShowUI labelVisible // 我们的实现需要这样刷新一下标签的显示
                HexMapCameraFS.ValidatePosition()
                GD.Print "Loaded!"
            else
                GD.PrintErr $"Unknown map format {header}"

    let getSelectedPath () =
        let mapName = _nameInput.Value.Text

        if mapName.Length = 0 then
            null
        else
            Path.Combine(saveDirPath, mapName + ".map")

    let fillList () =
        _listContent.Value.GetChildren() |> Seq.iter _.QueueFree()

        Directory.GetFiles(saveDirPath, "*.map")
        |> Array.sort
        |> Array.iter (fun path ->
            let item = this.itemPrefab.Instantiate<SaveLoadItemFS>()
            item.menu <- this
            item.MapName <- Path.GetFileNameWithoutExtension path
            _listContent.Value.AddChild item)

    member this.Action() =
        let path = getSelectedPath ()

        if path <> null then
            if saveMode then save path else load path
            this.Close()

    interface ISaveLoadMenu with
        member this.SelectItem name = _nameInput.Value.Text <- name

    member this.SelectItem = (this :> ISaveLoadMenu).SelectItem

    member this.Open saveBool labelVis =
        labelVisible <- labelVis
        saveMode <- saveBool

        if saveMode then
            _menuLabel.Value.Text <- "Save Map"
            _actionButton.Value.Text <- "Save"
        else
            _menuLabel.Value.Text <- "Load Map"
            _actionButton.Value.Text <- "Load"

        fillList ()
        this.Show()
        HexMapCameraFS.Locked <- true

    member this.Close() =
        this.Hide()
        HexMapCameraFS.Locked <- false

    member this.Delete() =
        let path = getSelectedPath ()

        if path <> null then
            if File.Exists path then
                File.Delete path

            _nameInput.Value.Text <- ""
            fillList ()

    override this._Ready() =
        _cancelButton.Value.add_Pressed (fun _ -> this.Close())
        _actionButton.Value.add_Pressed (fun _ -> this.Action())
        _deleteButton.Value.add_Pressed (fun _ -> this.Delete())
