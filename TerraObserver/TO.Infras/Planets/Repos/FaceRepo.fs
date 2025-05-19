namespace TO.Infras.Planets.Repos

open Friflo.Engine.ECS
open Godot
open TO.Commons.Utils
open TO.Infras.Planets.Models.Faces
open TO.Infras.Planets.Models.Points

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-18 20:05:18
type FaceRepo(store: EntityStore) =
    // 面
    let typeFace = ComponentTypes.Get<FaceComponent>()
    let tagChunkFace = Tags.Get<FaceTagChunk>()
    let tagTileFace = Tags.Get<FaceTagTile>()
    let archetypeChunkFace = store.GetArchetype(&typeFace, &tagChunkFace)
    let archetypeTileFace = store.GetArchetype(&typeFace, &tagTileFace)

    member this.QueryFaces chunky =
        store
            .Query<FaceComponent>()
            .AllTags(if chunky then &tagChunkFace else &tagTileFace)

    member this.AddFace chunky (triVertices: Vector3 array) =
        let face =
            if chunky then
                archetypeChunkFace.CreateEntity()
            else
                archetypeTileFace.CreateEntity()

        let center = (triVertices[0] + triVertices[1] + triVertices[2]) / 3f
        let faceComponent = FaceComponent(center, triVertices)
        face.AddComponent<FaceComponent>(&faceComponent) |> ignore
        face.Id

    // 因为 pointComponent 要传给闭包，所以不用 inref
    member this.GetOrderedFaces (pointComponent: PointComponent) (pointEntity: Entity) =
        let linkFaces = pointEntity.GetRelations<PointLinkFace>()

        if linkFaces.Length = 0 then
            []
        else
            // 将第一个面设置为最接近北方顺时针方向第一个的面
            let mutable first = linkFaces[0].Face
            let mutable minAngle = Mathf.Tau

            for i in 0 .. linkFaces.Length - 1 do
                let faceEntity = linkFaces[i].Face
                let face = faceEntity.GetComponent<FaceComponent>()
                let angle = pointComponent.Position.DirectionTo(face.Center).AngleTo(Vector3.Up)

                if angle < minAngle then
                    minAngle <- angle
                    first <- faceEntity
            // 第二个面必须保证和第一个面形成顺时针方向，从而保证所有都是顺时针
            let second =
                { 0 .. linkFaces.Length - 1 }
                |> Seq.map (fun i -> linkFaces[i].Face)
                |> Seq.find (fun faceEntity ->
                    let firstFace = first.GetComponent<FaceComponent>()
                    let face = faceEntity.GetComponent<FaceComponent>()

                    faceEntity <> first
                    && face.IsAdjacentTo(firstFace)
                    && Math3dUtil.IsRightVSeq(Vector3.Zero, pointComponent.Position, firstFace.Center, face.Center))

            let mutable orderedList = [ second; first ]
            let mutable currentFaceEntity = orderedList[1]

            while orderedList.Length < linkFaces.Length do
                let neighbor =
                    { 0 .. linkFaces.Length - 1 }
                    |> Seq.find (fun faceId ->
                        let faceEntity = linkFaces[faceId].Face
                        let face = faceEntity.GetComponent<FaceComponent>()
                        let currentFace = currentFaceEntity.GetComponent<FaceComponent>()

                        orderedList
                        |> Seq.exists (fun orderedFaceEntity -> orderedFaceEntity <> faceEntity)
                        && face.IsAdjacentTo currentFace)

                currentFaceEntity <- linkFaces[neighbor].Face
                orderedList <- currentFaceEntity :: orderedList

            orderedList |> List.rev