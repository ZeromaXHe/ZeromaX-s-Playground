module FrifloEcsTests

open Friflo.Engine.ECS
open Xunit

[<Struct>]
type MyComponent =
    interface IComponent
    val Idx: int
    new(idx) = { Idx = idx }

[<Struct>]
type IdRelation =
    interface int IRelation with
        member this.GetRelationKey() = this.Id

    val Id: int
    new(entity: Entity) = { Id = entity.Id }

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
            let relation = IdRelation(face)
            point.AddRelation<IdRelation>(&relation) |> ignore)

    // Assert
    let mutable pIdx = 0
    let mutable difference = 0

    for point in points do
        let relations = point.GetRelations<IdRelation>()

        for relIdx in 0 .. relations.Length - 1 do // using idx
            let relation = relations[relIdx]
            Assert.NotNull(relation)
            let faceId = faces[(3 * pIdx + relIdx) % 9].Id
            let relationId = relation.Id

            if faceId <> relationId then
                difference <- difference + 1 // will find differences in 3.4.1

        let mutable rIdx = 0

        for relation in relations do // using IEnumerator
            let faceId = faces[(3 * pIdx + rIdx) % 9].Id
            let relationId = relation.Id
            Assert.Equal(faceId, relationId) // no differences
            rIdx <- rIdx + 1

        pIdx <- pIdx + 1

    // Assert.Equal(18, difference) // 18 differences in for-idx-loop 3.4.1
    Assert.Equal(0, difference) // 0 differences in for-idx-loop 3.4.2


[<Fact>]
let ``AddComponent test`` () =
    // Arrange
    let store = EntityStore()
    let entity1 = store.CreateEntity(MyComponent 1)
    let entity2 = store.CreateEntity(MyComponent 1)
    let arr = [| entity1; entity2 |]
    let newComponent = MyComponent 2
    // Act
    let result =
        arr
        |> Array.map (fun entity ->
            let idx1 = entity.GetComponent<MyComponent>().Idx
            let updated = entity.AddComponent<MyComponent>(&newComponent)
            let idx2 = entity.GetComponent<MyComponent>().Idx
            let e1idx = entity1.GetComponent<MyComponent>().Idx
            let e2idx = entity2.GetComponent<MyComponent>().Idx
            idx1, updated, idx2, e1idx, e2idx)
    // Assert
    for i in 0..1 do
        let idx1, updated, idx2, e1idx, e2idx = result[i]
        Assert.Equal(1, idx1)
        Assert.False(updated)
        Assert.Equal(2, idx2)
        Assert.Equal(2, e1idx)
        Assert.Equal((if i = 0 then 1 else 2), e2idx) // 验证循环内 Entity 的修改会即刻生效
