namespace FrontEndToolFS.Tool

open Godot

type ISaveLoadMenu =
    interface
        abstract member SelectItem: string -> unit
    end

type SaveLoadItemFS() =
    inherit Button()

    [<DefaultValue>]
    val mutable menu: ISaveLoadMenu

    let mutable mapName = ""

    member this.MapName
        with get () = mapName
        and set value =
            mapName <- value
            this.Text <- value

    member this.Select() = this.menu.SelectItem mapName

    override this._Ready() =
        this.add_Pressed (fun _ -> this.Select())
