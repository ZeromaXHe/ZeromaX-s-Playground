namespace TO.Commons.Utils

open Godot
open TO.Commons.Enums

/// 六边形测量数据
/// 作为对其他 .NET 公开的 F# 库，需要遵循《F# 组件设计准则 - 命名空间和类型设计 - 使用命名空间、类型和成员作为组件的主要组织结构》
/// https://learn.microsoft.com/zh-cn/dotnet/fsharp/style-guide/component-design-guidelines#use-namespaces-types-and-members-as-the-primary-organizational-structure-for-your-components
///
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
[<AbstractClass; Sealed>]
type HexMetrics =
    static member StandardRadius = 150f // 150f 半径时才是标准大小，其他时候需要按比例缩放
    static member StandardDivisions = 10f // 10 细分时才是标准大小，其他时候需要按比例缩放

    static member OuterToInner = 0.8660254037f // √3/2 = 0.8660254037f
    static member InnerToOuter = 1f / HexMetrics.OuterToInner
    static member SolidFactor = 0.8f
    static member BlendFactor = 1f - HexMetrics.SolidFactor

    static member private TerracesPerSlope = 2
    static member TerraceSteps = 1 + HexMetrics.TerracesPerSlope * 2
    static member private HorizontalTerracesStepSize = 1f / float32 HexMetrics.TerraceSteps
    static member VerticalTerraceStepSize = 1f / (float32 HexMetrics.TerracesPerSlope + 1f)

    // 适用于球面的阶地 Lerp
    static member TerraceLerp(a: Vector3, b, step) =
        let bWithAHeight = Math3dUtil.ProjectToUnitSphere(b, a.Length())
        let h = float32 step * HexMetrics.HorizontalTerracesStepSize
        let horizontal = a.Slerp(bWithAHeight, h)
        let v = HexMetrics.VerticalTerraceStepSize * (float32 <| (step + 1) / 2)
        let vertical = Mathf.Lerp(a.Length(), b.Length(), v)
        Math3dUtil.ProjectToUnitSphere(horizontal, vertical)

    static member TerraceLerpColor(a: Color, b: Color, step: int) =
        let h = float32 step * HexMetrics.HorizontalTerracesStepSize
        a.Lerp(b, h)

    static member GetEdgeType(elevation1: int, elevation2: int) =
        if elevation1 = elevation2 then
            HexEdgeType.Flat
        elif Mathf.Abs(elevation1 - elevation2) = 1 then
            HexEdgeType.Slope
        else
            HexEdgeType.Cliff

    // ========== [河流与水面] ==========
    static member StreamBedElevationOffset = -1.75f
    static member WaterElevationOffset = -0.5f
    static member WaterFactor = 0.6f
    static member WaterBlendFactor = 1f - HexMetrics.WaterFactor

    // ========== [特征] ==========
    static member WallTowerThreshold = 0.6f
    static member BridgeDesignLength = 7f
