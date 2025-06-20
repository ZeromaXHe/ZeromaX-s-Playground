namespace Test.FSharp.Commons

open TO.Domains.Structs.HexSphereGrids
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
        Assert.Equal("E179°00'00\", N88°00'00\"", LonLatCoords(181f, 88f).ToString())
        Assert.Equal("   0°00'00\",   0°00'00\"", LonLatCoords().ToString())
        // 舍入进位测试
        Assert.Equal("E 25°31'00\", S33°44'00\"", LonLatCoords(-25.51666667f, -33.7333333f).ToString())
