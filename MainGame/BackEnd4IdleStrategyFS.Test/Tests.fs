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

[<Fact>]
let ``pipe associativity test`` () =
    // 安排 Arrange
    let diff1 a b = abs (a - b)
    let diff2 a b = abs <| a - b
    let diff3 a b = a - b |> abs
    // 行动 Act
    let res1 = diff1 -3 5
    let res2 = diff2 -3 5
    let res3 = diff3 -3 5
    let res4 = pown 2 <| abs -2 > 0
    let res5 = pown 2 <| abs -2 = 4
    // 断言 Assert
    Assert.Equal(res1, 8)
    Assert.Equal(res2, 8) // 管道是特殊函数，结合优先级低于 +、- 中缀操作符
    Assert.Equal(res3, 8) // 一样道理
    Assert.True(res4) // 有点搞不懂为啥，管道优先级低于这些 <、>、= 之类的运算符，但这里结果是这样的
    Assert.True(res5) // 同上

[<Fact>]
let ``function overload test`` () =
    // 安排 Arrange
    let product a = a * 2 // 并不可以重载
    let product a b = a * b
    // 行动 Act
    let res1 = product 5
    let res2 = product 5 2
    // 断言 Assert
    Assert.Equal(res1 2, 2 |> (fun i -> 5 * i)) // 只有后面的函数会生效
    Assert.Equal(res2, 10)
