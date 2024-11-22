namespace FrontEnd4IdleStrategyFS.Global

open System.Threading
open BackEnd4IdleStrategyFS.Game
open FrontEnd4IdleStrategyFS.Global.Adaptor
open Godot

type GlobalNodeFS() =
    inherit Node()

    member val IdleStrategyEntry: Entry option = None with get, set

    member this.ChangeToIdleStrategyScene() =
        this.GetTree().ChangeSceneToFile "res://game/inGame/menu/InGameMenu.tscn"
        |> ignore

    member this.ChangeToHexGlobalScene() =
        this.GetTree().ChangeSceneToFile "res://game/HexGlobal/Menu/HexGlobalMenu.tscn"
        |> ignore

    member this.InitIdleStrategyGame (baseTerrain: TileMapLayer) playerCount =
        this.IdleStrategyEntry <-
            Entry(
                AStar2DAdapter(new AStar2D()),
                TileMapLayerAdapter(baseTerrain),
                playerCount,
                (fun str -> SynchronizationContext.Current.Post((fun _ -> GD.Print str), null))
            )
            |> Some

    override this._Process(delta) =
        // 把游戏循环帧间隔时间传递给后端
        if this.IdleStrategyEntry.IsSome then
            this.IdleStrategyEntry.Value.GameProcess.OnNext delta
