namespace FrontEnd4IdleStrategyFS.Global.Adaptor

open Godot
open BackEnd4IdleStrategyFS.Godot.IAdapter
open FrontEndCommonFS.Util

type TileMapLayerAdapter(tileMapLayer: TileMapLayer) =
    interface ITileMapLayer with
        member this.GetSurroundingCells cell =
            // let x, y = cell
            // GD.Print $"TileMapLayer.GetSurroundingCells ({x}, {y}"

            BackEndUtil.fromI cell
            |> tileMapLayer.GetSurroundingCells
            |> Seq.map BackEndUtil.toI

        member this.GetUsedCells() =
            // GD.Print "TileMapLayer.GetUsedCells"
            tileMapLayer.GetUsedCells() |> Seq.map BackEndUtil.toI
