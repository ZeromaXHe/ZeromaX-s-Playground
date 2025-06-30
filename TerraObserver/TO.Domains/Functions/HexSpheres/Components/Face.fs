namespace TO.Domains.Functions.HexSpheres.Components

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexSpheres.Components.Chunks
open TO.Domains.Functions.HexSpheres.Components.Faces
open TO.Domains.Functions.Maths
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Points
open TO.Domains.Types.HexSpheres.Relations

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:23:19
module FaceQuery =
    // 因为 pointComponent 要传给闭包，所以不用 inref
    let getOrderedFaces (env: #IEntityStoreQuery) : GetOrderedFaces =
        fun (pointComponent: PointComponent) (pointEntity: Entity) ->
            let linkFaceIds = pointEntity.GetRelations<PointToFaceId>()

            if linkFaceIds.Length = 0 then
                []
            else
                let store = env.EntityStore
                let faceEntities: Entity array = Array.zeroCreate linkFaceIds.Length

                let mutable i = 0

                for id in linkFaceIds do
                    let faceId = id.FaceId
                    let e = store.GetEntityById(faceId)
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
                        && face |> FaceComponent.isAdjacentTo firstFace
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
                            && face |> FaceComponent.isAdjacentTo currentFace)

                    currentFaceEntity <- neighbor
                    orderedList <- currentFaceEntity :: orderedList

                orderedList |> List.rev

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:21:19
module FaceCommand =
    let add (env: #IEntityStoreQuery) : AddFace =
        fun (chunky: Chunky) (vertex1: Vector3) (vertex2: Vector3) (vertex3: Vector3) ->
            let center = (vertex1 + vertex2 + vertex3) / 3f
            let tag = ChunkFunction.chunkyTag chunky

            let face =
                env.EntityStore.CreateEntity(FaceComponent(center, vertex1, vertex2, vertex3), &tag)

            face.Id
