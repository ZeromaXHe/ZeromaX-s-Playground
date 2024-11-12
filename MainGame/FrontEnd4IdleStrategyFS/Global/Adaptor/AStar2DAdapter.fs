namespace FrontEnd4IdleStrategyFS.Global.Adaptor

open Godot
open BackEnd4IdleStrategyFS.Godot.IAdapter
open FrontEnd4IdleStrategyFS.Global.Common

type AStar2DAdapter(aStar2D: AStar2D) =
    interface IAStar2D with
        member this.AddPoint id coord =
            aStar2D.AddPoint(id, BackEndUtil.from coord)

        member this.ConnectPoints fromId toId =
            // GD.Print $"AStar2D.ConnectPoints ({fromId}, {toId})"
            aStar2D.ConnectPoints(fromId, toId)

        member this.GetPointConnections id =
            // GD.Print $"AStar2D.GetPointConnections {id}"
            let connectNavIdArr = aStar2D.GetPointConnections id
            connectNavIdArr |> Seq.ofArray |> Seq.map int
