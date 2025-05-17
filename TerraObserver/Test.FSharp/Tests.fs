module Tests

open Xunit

[<Fact>]
let ``Pipeline test`` () =
    // Arrange
    let multi2 = (*) 2
    // Act
    let right1 = 1 + -2 |> multi2 // 会先计算 1 + -2，再引用管道符
    let right2 = 1 + (-2 |> multi2)
    let left1 = multi2 <| 1 + -2
    let left2 = (multi2 <| 1) + -2
    // Assert
    Assert.Equal(-2, right1)
    Assert.Equal(-3, right2)
    Assert.Equal(-2, left1)
    Assert.Equal(0, left2)
