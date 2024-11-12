namespace FrontEnd4IdleStrategyFS.Global

open BackEnd4IdleStrategyFS.Game
open FrontEnd4IdleStrategyFS.Global.Adaptor
open Godot

type GlobalNodeFS() =
    inherit Node()
    
    member val IdleStrategyEntry: Entry option = None with get, set

    member this.ChangeToIdleStrategyScene () =
        this.GetTree().ChangeSceneToFile "res://game/inGame/menu/InGameMenu.tscn"
        |> ignore

    member this.InitIdleStrategyGame (baseTerrain: TileMapLayer) playerCount =
        this.IdleStrategyEntry <- Entry(
            AStar2DAdapter(new AStar2D()),
            TileMapLayerAdapter(baseTerrain),
            playerCount,
            GD.Print) |> Some