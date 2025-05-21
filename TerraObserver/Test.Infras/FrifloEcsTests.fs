module FrifloEcsTests

open Friflo.Engine.ECS
open TO.Infras.Planets.Models.Points
open Xunit

[<Fact>]
let ``Relations for-loop test`` () =
    // Arrange
    let store = EntityStore()
    let points = Array.init 9 (fun _ -> store.CreateEntity())
    let faces = Array.init 9 (fun _ -> store.CreateEntity())
    // Act
    for i in 0..8 do
        let point = points[i]

        seq {
            faces[(3 * i) % 9]
            faces[(3 * i + 1) % 9]
            faces[(3 * i + 2) % 9]
        }
        |> Seq.iter (fun face ->
            let relation = PointToFaceId(face)
            point.AddRelation<PointToFaceId>(&relation) |> ignore)

    // Asset
    let mutable pIdx = 0
    let mutable difference = 0

    for point in points do
        let relations = point.GetRelations<PointToFaceId>()

        for relIdx in 0 .. relations.Length - 1 do // using idx
            let relation = relations[relIdx]
            Assert.NotNull(relation)
            let faceId = faces[(3 * pIdx + relIdx) % 9].Id
            let relationId = relation.FaceId

            if faceId <> relationId then
                difference <- difference + 1 // will find differences

        let mutable rIdx = 0
        for relation in relations do // using IEnumerator
            let faceId = faces[(3 * pIdx + rIdx) % 9].Id
            let relationId = relation.FaceId
            Assert.Equal(faceId, relationId) // no differences
            rIdx <- rIdx + 1

        pIdx <- pIdx + 1

    Assert.Equal(18, difference) // 18 differences in for-idx-loop
