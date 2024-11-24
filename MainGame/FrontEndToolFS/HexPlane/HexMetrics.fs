namespace FrontEndToolFS.HexPlane

open Godot

module HexMetrics =
    let outerRadius = 10f

    let innerRadius = outerRadius * 0.866025404f

    let corners =
        [| Vector3(0f, 0f, outerRadius)
           Vector3(innerRadius, 0f, 0.5f * outerRadius)
           Vector3(innerRadius, 0f, -0.5f * outerRadius)
           Vector3(0f, 0f, -outerRadius)
           Vector3(-innerRadius, 0f, -0.5f * outerRadius)
           Vector3(-innerRadius, 0f, 0.5f * outerRadius)
           // 复制第一个，方便简化使用逻辑
           Vector3(0f, 0f, outerRadius) |]

    // Godot 渲染面方向和 Unity 相反
    let getFirstCorner (direction: HexDirection) = corners[int direction + 1]
    let getSecondCorner (direction: HexDirection) = corners[int direction]
