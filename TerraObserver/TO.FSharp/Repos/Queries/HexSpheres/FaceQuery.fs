namespace TO.FSharp.Repos.Queries.HexSpheres

open Friflo.Engine.ECS
open Godot
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Points
open TO.Domains.Relations.HexSpheres.Points
open TO.Domains.Utils.Commons
open TO.FSharp.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:23:19
module FaceQuery =
    // 因为 pointComponent 要传给闭包，所以不用 inref
    let getOrderedFaces (env: #IEntityStore) =
        fun (pointComponent: PointComponent) (pointEntity: Entity) ->
            let linkFaceIds = pointEntity.GetRelations<PointToFaceId>()

            if linkFaceIds.Length = 0 then
                []
            else
                let faceEntities: Entity array = Array.zeroCreate linkFaceIds.Length

                let mutable i = 0

                for id in linkFaceIds do
                    let faceId = id.FaceId
                    let e = env.EntityStore.GetEntityById(faceId)
                    faceEntities[i] <- e
                    i <- i + 1
                // 将第一个面设置为最接近北方顺时针方向第一个的面
                let mutable first = faceEntities[0]
                let mutable minAngle = Mathf.Tau

                for faceEntity in faceEntities do
                    let face = faceEntity.GetComponent<FaceComponent>()
                    let angle = pointComponent.Position.DirectionTo(face.Center).AngleTo(Vector3.Up)

                    if angle < minAngle then
                        minAngle <- angle
                        first <- faceEntity

                let firstFace = first.GetComponent<FaceComponent>()
                // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
                let second =
                    faceEntities
                    |> Array.find (fun faceEntity ->
                        let face = faceEntity.GetComponent<FaceComponent>()

                        faceEntity.Id <> first.Id
                        && face.IsAdjacentTo(firstFace)
                        && Math3dUtil.isRightVSeq Vector3.Zero pointComponent.Position firstFace.Center face.Center)

                let mutable orderedList = [ second; first ]
                let mutable currentFaceEntity = second

                while orderedList.Length < faceEntities.Length do
                    let neighbor =
                        faceEntities
                        |> Seq.find (fun faceEntity ->
                            let face = faceEntity.GetComponent<FaceComponent>()
                            let currentFace = currentFaceEntity.GetComponent<FaceComponent>()

                            orderedList
                            |> List.forall (fun orderedFaceEntity -> orderedFaceEntity.Id <> faceEntity.Id)
                            && face.IsAdjacentTo currentFace)

                    currentFaceEntity <- neighbor
                    orderedList <- currentFaceEntity :: orderedList

                orderedList |> List.rev
