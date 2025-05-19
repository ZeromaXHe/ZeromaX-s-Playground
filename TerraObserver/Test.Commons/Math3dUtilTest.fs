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

    [<Fact>]
    let ``IsRightVSeq 测试`` () =
        // 安排 Arrange
        let pointCenter = Vector3(0.8944272f, 0.4472136f, 0f)
        let firstFaceCenter = Vector3(0.39027345f, 0.63147575f, 0.28355026f)

        let firstFaceVertices =
            [| Vector3(0f, 1f, 0f)
               Vector3(0.8944272f, 0.4472136f, 0f)
               Vector3(0.2763932f, 0.4472136f, 0.8506508f) |]

        let faceCenter1 = Vector3(0.63147575f, 0.1490712f, 0.45879397f)

        let faceVertices1 =
            [| Vector3(0.72360677f, -0.4472136f, 0.5257311f)
               Vector3(0.2763932f, 0.4472136f, 0.8506508f)
               Vector3(0.8944272f, 0.4472136f, 0f) |]

        let faceCenter2 = Vector3(-0.14907119f, 0.63147575f, 0.45879397f)

        let faceVertices2 =
            [| Vector3(0f, 1f, 0f)
               Vector3(0.2763932f, 0.4472136f, 0.8506508f)
               Vector3(-0.72360677f, 0.4472136f, 0.5257311f) |]
        // 行动 Act
        let result1 =
            Math3dUtil.IsRightVSeq(Vector3.Zero, pointCenter, firstFaceCenter, faceCenter1)

        let result2 =
            Math3dUtil.IsRightVSeq(Vector3.Zero, pointCenter, firstFaceCenter, faceCenter2)
        // 断言 Assert
        Assert.False(result1)
        Assert.False(result2)
