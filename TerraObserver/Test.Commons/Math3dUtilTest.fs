namespace Test.Commons

open Godot
open TO.Commons.Utils
open Xunit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 09:39:19
module Math3dUtilTest =
    [<Fact>]
    let ``GetNormal 获取法线测试`` () =
        // 安排 Arrange
        let v0 = Vector3.Zero
        let v1 = Vector3.Right
        let v2 = Vector3.Back
        // 行动 Act
        let normal = Math3dUtil.GetNormal(v0, v1, v2)
        // 断言 Assert
        Assert.Equal(Vector3.Up, normal)
