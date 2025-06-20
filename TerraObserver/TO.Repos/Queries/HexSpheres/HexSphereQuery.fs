namespace TO.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Faces
open TO.Domains.Alias.HexSpheres.Points
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points

type HexSphereSearchNearest = Vector3 -> Chunky -> Entity option

type GetHexFacesAndNeighborCenterIds =
    Chunky -> PointComponent -> Entity -> FaceComponent array * FaceId array * PointId array

[<Interface>]
type IHexSphereQuery =
    abstract SearchNearest: HexSphereSearchNearest
    abstract GetHexFacesAndNeighborCenterIds: GetHexFacesAndNeighborCenterIds

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 21:14:19
module HexSphereQuery =
    let searchNearest store chunkyVpTrees : HexSphereSearchNearest =
        fun (pos: Vector3) chunky ->
            let center = PointQuery.searchNearestCenterPos chunkyVpTrees pos chunky

            PointQuery.tryHeadByPosition store chunky center
            |> Option.bind (fun pointEntity -> PointQuery.tryHeadEntityByCenterId store pointEntity.Id)

    let getHexFacesAndNeighborCenterIds store : GetHexFacesAndNeighborCenterIds =
        fun chunky (pComp: PointComponent) (pEntity: Entity) ->
            let hexFaceEntities = FaceQuery.getOrderedFaces store pComp pEntity
            let hexFaces = hexFaceEntities |> List.map _.GetComponent<FaceComponent>()

            let neighborCenterIds =
                PointQuery.getNeighborCenterPointIds store chunky hexFaces pComp |> Seq.toArray

            (hexFaces |> List.toArray, hexFaceEntities |> List.map _.Id |> List.toArray, neighborCenterIds)
