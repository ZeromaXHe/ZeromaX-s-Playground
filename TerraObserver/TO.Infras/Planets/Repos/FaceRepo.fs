namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.Utils
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points
open TO.Infras.Planets.Utils

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 20:05:18
type FaceRepo(store: EntityStore) =
    // 面
    let tagChunk = Tags.Get<FaceTagChunk>()
    let tagTile = Tags.Get<FaceTagTile>()

    member this.ForEachByChunky chunky forEachFace =
        FrifloEcsUtil.forEachEntity
        <| store.Query<FaceComponent>().AllTags(if chunky then &tagChunk else &tagTile)
        <| forEachFace

    member this.Add chunky (vertex1: Vector3) (vertex2: Vector3) (vertex3: Vector3) =
        let center = (vertex1 + vertex2 + vertex3) / 3f

        let face =
            if chunky then
                store.CreateEntity(FaceComponent(center, vertex1, vertex2, vertex3), &tagChunk)
            else
                store.CreateEntity(FaceComponent(center, vertex1, vertex2, vertex3), &tagTile)

        face.Id

    // 因为 pointComponent 要传给闭包，所以不用 inref
    member this.GetOrderedFaces (pointComponent: PointComponent) (pointEntity: Entity) =
        let linkFaceIds = pointEntity.GetRelations<PointToFaceId>()

        if linkFaceIds.Length = 0 then
            []
        else
            let faceEntities: Entity array = Array.zeroCreate linkFaceIds.Length

            let mutable i = 0

            for id in linkFaceIds do
                // for i in 0 .. linkFaceIds.Length - 1 do
                // BUG: 似乎这里 Relations<>.[index] 有 bug…… 想要放弃 ECS 了
                // https://github.com/friflo/Friflo.Engine.ECS/issues/70
                // let id = linkFaceIds[i]
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
                    && face.IsAdjacentTo(firstFace)
                    && Math3dUtil.IsRightVSeq(Vector3.Zero, pointComponent.Position, firstFace.Center, face.Center))

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
