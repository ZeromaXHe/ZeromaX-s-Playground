namespace TO.FSharp.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 21:14:19
module HexSphereQuery =
    let searchNearest env =
        fun (pos: Vector3) chunky ->
            let center = PointQuery.searchNearestCenterPos env pos chunky

            PointQuery.tryHeadByPosition env chunky center
            |> Option.bind (fun pointEntity -> PointQuery.tryHeadEntityByCenterId env pointEntity.Id)

    let getHexFacesAndNeighborCenterIds env =
        fun chunky (pComp: PointComponent inref) (pEntity: Entity inref) ->
            let hexFaceEntities = FaceQuery.getOrderedFaces env pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                PointQuery.getNeighborCenterPointIds env chunky hexFaces &pComp |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)
