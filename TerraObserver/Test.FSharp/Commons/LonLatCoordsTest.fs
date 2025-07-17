namespace Test.FSharp.Commons

open TO.Domains.Functions.HexGridCoords
open TO.Domains.Types.HexGridCoords
open Xunit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 09:34:19
module LonLatCoordsTest =
    [<Fact>]
    let ``转换字符串测试`` () =
        // 安排 Arrange
        // 行动 Act
        // 断言 Assert
        Assert.Equal("E179°00'00\", N88°00'00\"", LonLatCoords(181f, 88f) |> LonLatCoords.toString)
        Assert.Equal("   0°00'00\",   0°00'00\"", LonLatCoords() |> LonLatCoords.toString)
        // 舍入进位测试
        Assert.Equal("E 25°31'00\", S33°44'00\"", LonLatCoords(-25.51666667f, -33.7333333f) |> LonLatCoords.toString)

    [<Fact>]
    let ``Slerp 测试`` () =
        // 安排 Arrange
        let fromLonLat = LonLatCoords(0f, 0f)
        let toLonLat = LonLatCoords(180f, 90f)
        // 行动 Act
        let result1 = fromLonLat |> LonLatCoords.slerp toLonLat 0.5f

        let result2 =
            LonLatCoords(-178f, 60f) |> LonLatCoords.slerp (LonLatCoords(179f, -30f)) 0.5f
        // 断言 Assert
        Assert.Equal("   0°00'00\", N45°00'00\"", result1 |> LonLatCoords.toString)
        Assert.Equal("E179°54'07\", N15°00'16\"", result2 |> LonLatCoords.toString)
