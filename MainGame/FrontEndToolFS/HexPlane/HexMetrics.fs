namespace FrontEndToolFS.HexPlane

open Godot

module HexMetrics =
    let outerRadius = 10f

    let innerRadius = outerRadius * 0.866025404f

    let solidFactor = 0.75f

    let blendFactor = 1f - solidFactor

    let corners =
        [| Vector3(0f, 0f, outerRadius)
           Vector3(innerRadius, 0f, 0.5f * outerRadius)
           Vector3(innerRadius, 0f, -0.5f * outerRadius)
           Vector3(0f, 0f, -outerRadius)
           Vector3(-innerRadius, 0f, -0.5f * outerRadius)
           Vector3(-innerRadius, 0f, 0.5f * outerRadius)
           // 复制第一个，方便简化使用逻辑
           Vector3(0f, 0f, outerRadius) |]

    let getFirstCorner (direction: HexDirection) = corners[int direction]
    let getSecondCorner (direction: HexDirection) = corners[int direction + 1]
    let getFirstSolidCorner (direction: HexDirection) = getFirstCorner direction * solidFactor
    let getSecondSolidCorner (direction: HexDirection) = getSecondCorner direction * solidFactor

    let getBridge (direction: HexDirection) =
        (getFirstCorner direction + getSecondCorner direction) * blendFactor
