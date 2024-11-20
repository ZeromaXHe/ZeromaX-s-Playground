module Tests

open System
open Xunit

[<Fact>]
let ``random seed test`` () =
    // 安排 Arrange
    let seed = Random().Next Int32.MaxValue
    let random1 = Random(seed)
    let random2 = Random(seed)
    let times = 3
    // 行动 Act
    let seq1 = { 1..times } |> Seq.map (fun _ -> random1.Next())
    let seq2 = { 1..times } |> Seq.map (fun _ -> random2.Next())
    // 断言 Assert
    Assert.True(Seq.forall2 (=) seq1 seq2)

[<Fact>]
let ``List append test`` () =
    // 安排 Arrange
    let list1 = [ 1; 2; 3 ]
    let list2 = [ 4; 5; 6 ]
    // 行动 Act
    let append1 = list1 @ list2
    let append2 = List.append list1 list2
    let append3 = list1 |> List.append list2
    let append4 = list1 |> List.append <| list2
    // 断言 Assert
    Assert.Equal<int list>(append1, [ 1; 2; 3; 4; 5; 6 ])
    Assert.Equal<int list>(append2, [ 1; 2; 3; 4; 5; 6 ])
    Assert.Equal<int list>(append3, [ 4; 5; 6; 1; 2; 3 ]) // 特别注意这里的顺序！
    Assert.Equal<int list>(append4, [ 1; 2; 3; 4; 5; 6 ])
