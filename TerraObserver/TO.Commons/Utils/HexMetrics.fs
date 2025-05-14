namespace TO.Commons.Utils

open Godot
open TO.Commons.Enums

module HexMetrics =
    let standardRadius = 150f // 150f 半径时才是标准大小，其他时候需要按比例缩放
    let standardDivisions = 10f // 10 细分时才是标准大小，其他时候需要按比例缩放

    let outerToInner = 0.8660254037f // √3/2 = 0.8660254037f
    let innerToOuter = 1f / outerToInner
    let solidFactor = 0.8f
    let blendFactor = 1f - solidFactor

    let private terracesPerSlope = 2
    let terraceSteps = 1 + terracesPerSlope * 2
    let private horizontalTerracesStepSize = 1f / float32 terraceSteps
    let verticalTerraceStepSize = 1f / (float32 terracesPerSlope + 1f)

    // 适用于球面的阶地 Lerp
    let terraceLerp (a: Vector3, b, step) =
        let bWithAHeight = Math3dUtil.projectToUnitSphere (b, a.Length())
        let h = float32 step * horizontalTerracesStepSize
        let horizontal = a.Slerp(bWithAHeight, h)
        let v = verticalTerraceStepSize * (float32 <| (step + 1) / 2)
        let vertical = Mathf.Lerp(a.Length(), b.Length(), v)
        Math3dUtil.projectToUnitSphere (horizontal, vertical)

    let terraceLerpColor (a: Color, b: Color, step: int) =
        let h = float32 step * horizontalTerracesStepSize
        a.Lerp(b, h)

    let getEdgeType (elevation1: int, elevation2: int) =
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
