module FSharpTests

open Xunit

[<Fact>]
let ``管道符测试`` () =
    // Arrange
    let multi2 = (*) 2
    // Act
    let right1 = 1 + -2 |> multi2 // 会先计算 1 + -2，再引用管道符
    let right2 = 1 + (-2 |> multi2)
    let left1 = multi2 <| 1 + -2
    let left2 = (multi2 <| 1) + -2
    let leftRight = 1 |> (-) <| 2 // = 1 - 2
    let onlyRight = 1 |> (-) 2 // = 2 - 1
    // Assert
    Assert.Equal(-2, right1)
    Assert.Equal(-3, right2)
    Assert.Equal(-2, left1)
    Assert.Equal(0, left2)
    Assert.Equal(-1, leftRight) // 对于符合交换律的，x |> f <| y = x |> f y 才成立
    Assert.Equal(1, onlyRight)

[<Fact>]
let ``not 管道符测试`` () =
    // Arrange
    let b1 = not <| true || true // 会先计算 not <| true = false 再 || true = true
    let b1Anti = not <| (true || true)
    let b2 = not <| false && false // 会先计算 not <| false = true 再 && false = false
    let b2Anti = not <| (false && false)
    // Act
    // Assert
    Assert.True(b1)
    Assert.False(b1Anti)
    Assert.False(b2)
    Assert.True(b2Anti)

[<Fact>]
let ``Seq 延迟执行测试`` () =
    // Arrange
    let mutable count = 0

    let executor (i: int) =
        count <- count + 1
        i

    let testSeq =
        seq {
            executor 1
            executor 2
        }

    let countAfterTestSeq = count

    let testSeq2 =
        seq {
            yield executor 1
            yield executor 2
        }

    let countAfterTestSeq2 = count
    // Act
    let result = testSeq |> Seq.find (fun i -> i = 1)
    let countAfterResult = count
    let result2 = testSeq2 |> Seq.find (fun i -> i = 1)
    // Assert
    Assert.Equal(0, countAfterTestSeq)
    Assert.Equal(0, countAfterTestSeq2) // 由于 seq 延迟执行，所以 count 为 0
    Assert.Equal(1, countAfterResult)
    Assert.Equal(2, count) // 由于 find 只会执行到第一个，每次加 1；所以执行两次，count 为 2
    Assert.Equal(1, result)
    Assert.Equal(1, result2)

[<Fact>]
let ``不同 For 循环写法测试`` () =
    // Arrange
    let mutable sum1 = 0
    let mutable sum2 = 0
    let mutable sum3 = 0
    let mutable sum4 = 0
    // Act
    for i in 1..10 do
        sum1 <- sum1 + i

    for i in 10..-1..1 do
        sum2 <- sum2 + i

    for i = 1 to 10 do
        sum3 <- sum3 + i

    for i = 10 downto 1 do
        sum4 <- sum4 + i
    // Assert
    Assert.Equal(55, sum1)
    Assert.Equal(55, sum2)
    Assert.Equal(55, sum3)
    Assert.Equal(55, sum4)
