namespace TO.FSharp.Commons.Utils

open Godot
open TO.FSharp.Commons.Enums.Tiles
open TO.FSharp.Commons.Structs.Planets

/// 六边形测量数据
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
module HexMetrics =
    let private terracesPerSlope = 2
    let terraceSteps = terracesPerSlope * 2 + 1
    let private horizontalTerracesStepSize = 1f / float32 terraceSteps
    let verticalTerraceStepSize = 1f / float32 (terracesPerSlope + 1)

    // 适用于球面的阶地 Lerp
    let terraceLerp (a: Vector3) b step =
        let bWithAHeight = Math3dUtil.ProjectToSphere(b, a.Length())
        let h = float32 step * horizontalTerracesStepSize
        let horizontal = a.Slerp(bWithAHeight, h)
        let v = float32 ((step + 1) / 2) * verticalTerraceStepSize
        let vertical = Mathf.Lerp(a.Length(), b.Length(), v)
        Math3dUtil.ProjectToSphere(horizontal, vertical)

    let terraceLerpEdgeV (a: EdgeVertices) (b: EdgeVertices) (step: int) =
        EdgeVertices(
            V1 = terraceLerp a.V1 b.V1 step,
            V2 = terraceLerp a.V2 b.V2 step,
            V3 = terraceLerp a.V3 b.V3 step,
            V4 = terraceLerp a.V4 b.V4 step,
            V5 = terraceLerp a.V5 b.V5 step
        )

    let terraceLerpColor (a: Color) (b: Color) (step: int) =
        let h = float32 step * horizontalTerracesStepSize
        a.Lerp(b, h)

    let getEdgeType (elevation1: int) (elevation2: int) =
        if elevation1 = elevation2 then
            HexEdgeType.Flat
        elif Mathf.Abs(elevation1 - elevation2) = 1 then
            HexEdgeType.Slope
        else
            HexEdgeType.Cliff

    // ========== [河流与水面] ==========
    let streamBedElevationOffset = -1.75f
    let waterElevationOffset = -0.5f
    let waterFactor = 0.6f
    let waterBlendFactor = 1f - waterFactor

    // ========== [特征] ==========
    let wallTowerThreshold = 0.6f
    let bridgeDesignLength = 7f

/// 六边形测量数据
/// 作为对其他 .NET 公开的 F# 库，需要遵循《F# 组件设计准则 - 命名空间和类型设计 - 使用命名空间、类型和成员作为组件的主要组织结构》
/// https://learn.microsoft.com/zh-cn/dotnet/fsharp/style-guide/component-design-guidelines#use-namespaces-types-and-members-as-the-primary-organizational-structure-for-your-components
[<AbstractClass; Sealed>]
type HexMetrics =
    static member StandardRadius = 150f // 150f 半径时才是标准大小，其他时候需要按比例缩放
    static member StandardDivisions = 10f // 10 细分时才是标准大小，其他时候需要按比例缩放

    static member OuterToInner = 0.8660254037f // √3/2 = 0.8660254037f
    static member InnerToOuter = 1f / HexMetrics.OuterToInner
    static member SolidFactor = 0.8f
    static member BlendFactor = 1f - HexMetrics.SolidFactor
