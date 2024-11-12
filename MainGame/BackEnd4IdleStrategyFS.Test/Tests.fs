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
let ``initPlayerAndSpawnOnTile test`` () =
    // 安排 Arrange
    // 行动 Act
    // 断言 Assert
    Assert.True(true)
