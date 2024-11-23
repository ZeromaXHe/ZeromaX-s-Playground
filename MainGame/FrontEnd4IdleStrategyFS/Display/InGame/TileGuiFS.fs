namespace FrontEnd4IdleStrategyFS.Display.InGame

open BackEnd4IdleStrategyFS.Game.DomainT
open FrontEndCommonFS.Util
open Godot

type TileGuiFS() as this =
    inherit Control()

    let _population = lazy this.GetNode<Label> "Population"

    [<DefaultValue>]
    val mutable _id: TileId

    [<DefaultValue>]
    val mutable _coord: Vector2I

    static member val idMap = Map.empty<TileId, TileGuiFS> with get, set

    static member ContainsId id = TileGuiFS.idMap.ContainsKey id

    static member ChangePopulationById id population =
        match TileGuiFS.idMap.TryFind id with
        | Some tileGui -> tileGui.ChangePopulation population
        | None -> ()

    member this.ChangePopulation population =
        _population.Value.Text <- population / 1<Pop> |> string

    member this.Init id coord population globalPosition =
        this._id <- id
        this._coord <- BackEndUtil.fromI coord
        this.Position <- globalPosition
        _population.Value.Text <- population / 1<Pop> |> string
        TileGuiFS.idMap <- TileGuiFS.idMap.Add(id, this)
