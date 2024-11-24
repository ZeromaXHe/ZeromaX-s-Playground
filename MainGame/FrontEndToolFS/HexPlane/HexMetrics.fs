namespace FrontEndToolFS.HexPlane

open System.Numerics
open Godot

module HexMetrics =
    // 内外半径
    let outerRadius = 10f
    let innerRadius = outerRadius * 0.866025404f
    // 混合颜色
    let solidFactor = 0.75f
    let blendFactor = 1f - solidFactor
    // 六角坐标
    let corners =
        [| Vector3(0f, 0f, outerRadius)
           Vector3(innerRadius, 0f, 0.5f * outerRadius)
           Vector3(innerRadius, 0f, -0.5f * outerRadius)
           Vector3(0f, 0f, -outerRadius)
           Vector3(-innerRadius, 0f, -0.5f * outerRadius)
           Vector3(-innerRadius, 0f, 0.5f * outerRadius)
           // 复制第一个，方便简化使用逻辑
           Vector3(0f, 0f, outerRadius) |]
    // 获取特定方向六角坐标
    let getFirstCorner (direction: HexDirection) = corners[int direction]
    let getSecondCorner (direction: HexDirection) = corners[int direction + 1]
    let getFirstSolidCorner direction = getFirstCorner direction * solidFactor
    let getSecondSolidCorner direction = getSecondCorner direction * solidFactor
    // 获取桥接坐标
    let getBridge direction =
        (getFirstCorner direction + getSecondCorner direction) * blendFactor
    // 立面
    let elevationStep = 3f
    // 阶梯
    let terracesPerSlope = 2
    let terraceSteps = terracesPerSlope * 2 + 1
    let horizontalTerraceStepSize = 1f / float32 terraceSteps
    let verticalTerraceStepSize = 1f / float32 (terracesPerSlope + 1)
    // 阶梯坐标插值
    let terraceLerp (a: Vector3) (b: Vector3) (step: int) =
        let h = float32 step * horizontalTerraceStepSize
        let v = float32 ((step + 1) / 2) * verticalTerraceStepSize
        let x = a.X + (b.X - a.X) * h
        let y = a.Y + (b.Y - a.Y) * v
        let z = a.Z + (b.Z - a.Z) * h
        Vector3(x, y, z)
    // 阶梯颜色插值
    let terraceColorLerp (a: Color) b (step: int) =
        let h = float32 step * horizontalTerraceStepSize
        a.Lerp(b, h)
    // 边类型
    let getEdgeType elevation1 elevation2 =
        if elevation1 = elevation2 then
            HexEdgeType.Flat
        else
            let delta = elevation2 - elevation1

            if (delta = 1 || delta = -1) then
                HexEdgeType.Slope
            else
                HexEdgeType.Cliff
