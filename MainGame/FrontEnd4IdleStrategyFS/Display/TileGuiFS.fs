namespace FrontEnd4IdleStrategyFS.Display

open BackEnd4IdleStrategyFS.Game.DomainT
open FrontEnd4IdleStrategyFS.Global.Common
open Godot

type TileGuiFS() as this =
    inherit Control()

    let _population = lazy this.GetNode<Label> "Population"

    [<DefaultValue>]
    val mutable _id: TileId

    [<DefaultValue>]
    val mutable _coord: Vector2I

    static member val idMap = Map.empty<TileId, TileGuiFS> with get, set

    static member ChangePopulationById id population =
        match TileGuiFS.idMap.TryFind id with
        | Some tileGui -> tileGui.ChangePopulation population
        | None -> ()

    member this.ChangePopulation population =
        _population.Value.Text <- population.ToString()

    member this.Init id coord population globalPosition =
        this._id <- id
        this._coord <- BackEndUtil.fromI coord
        this.Position <- globalPosition
        _population.Value.Text <- population.ToString()
        TileGuiFS.idMap <- TileGuiFS.idMap.Add(id, this)
