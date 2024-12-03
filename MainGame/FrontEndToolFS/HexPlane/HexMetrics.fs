namespace FrontEndToolFS.HexPlane

open Godot

module HexMetrics =
    let mutable noiseSource: Image = null
    // 内外半径
    let outerToInner = 0.866025404f
    let innerToOuter = 1f / outerToInner
    let outerRadius = 10f
    let innerRadius = outerRadius * outerToInner
    // 混合颜色
    let solidFactor = 0.8f
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
    // 噪声采样
    let noiseScale = 0.003f
    // 模拟 Unity Texture2D.GetPixelBilinear API
    // （入参是 0 ~ 1 的 float，超过范围则取小数部分，即“包裹模式进行重复”）
    // 参考：https://docs.unity3d.com/cn/2021.3/ScriptReference/Texture2D.GetPixelBilinear.html
    let mockUnityGetPixelBilinear (img: Image) (u: float32) (v: float32) =
        // * 512f 是因为我们使用的噪音图片来自 Catlike Coding，图片大小是 512x512。
        let x = int <| Mathf.PosMod(u * 512f, 512f)
        let y = int <| Mathf.PosMod(v * 512f, 512f)
        let color = img.GetPixel(x, y)
        Vector4(color.R, color.G, color.B, color.A)

    let sampleNoise (position: Vector3) =
        mockUnityGetPixelBilinear noiseSource (position.X * noiseScale) (position.Z * noiseScale)

    // 扰动强度
    let cellPerturbStrength = 4f
    let elevationPerturbStrength = 1.5f

    /// 扰动
    let perturb (position: Vector3) =
        let sample = sampleNoise position
        let x = position.X + (sample.X * 2f - 1f) * cellPerturbStrength
        let z = position.Z + (sample.Z * 2f - 1f) * cellPerturbStrength
        Vector3(x, position.Y, z)
    // 分块
    let chunkSizeX = 5
    let chunkSizeZ = 5
    /// 河床高度偏移
    let streamBedElevationOffset = -1.75f

    let getSolidEdgeMiddle (direction: HexDirection) =
        (getFirstCorner direction + getSecondCorner direction) * (0.5f * solidFactor)

    /// 水面高度偏移
    let waterElevationOffset = -0.5f
    /// 水因子
    let waterFactor = 0.6f
    let waterBlendFactor = 1f - waterFactor
    let getFirstWaterCorner (direction: HexDirection) = getFirstCorner direction * waterFactor
    let getSecondWaterCorner (direction: HexDirection) = getSecondCorner direction * waterFactor

    let getWaterBridge (direction: HexDirection) =
        (getFirstCorner direction + getSecondCorner direction) * waterBlendFactor
    // 随机哈希网格
    let hashGridSize = 256

    let hashGrid: HexHash array = Array.zeroCreate <| hashGridSize * hashGridSize

    let random = new RandomNumberGenerator()

    let initializeHashGrid seed =
        random.Seed <- seed
        let initState = random.State

        for i in 0 .. hashGrid.Length - 1 do
            hashGrid[i] <- HexHash.Create()

        random.State <- initState

    let hashGridScale = 0.25f

    let sampleHashGrid (position: Vector3) =
        let x = int (position.X * hashGridScale) % hashGridSize
        let x = if x < 0 then x + hashGridSize else x
        let z = int (position.Z * hashGridScale) % hashGridSize
        let z = if z < 0 then z + hashGridSize else z
        //GD.Print $"hash x:{x} z:{z} a:{hashGrid[x + z * hashGridSize].a} b:{hashGrid[x + z * hashGridSize].b}"
        hashGrid[x + z * hashGridSize]

    let featureThresholds =
        [| [| 0f; 0f; 0.4f |]; [| 0f; 0.4f; 0.6f |]; [| 0.4f; 0.6f; 0.8f |] |]

    let getFeatureThresholds level = featureThresholds[level]

    // 城墙
    let wallHeight = 3f
    let wallThickness = 0.75f

    let wallThicknessOffset (near: Vector3) (far: Vector3) =
        Vector3(far.X - near.X, 0f, far.Z - near.Z).Normalized() * wallThickness * 0.5f

    let wallElevationOffset = verticalTerraceStepSize

    let wallLerp (near: Vector3) (far: Vector3) =
        let x = near.X + (far.X - near.X) * 0.5f
        let z = near.Z + (far.Z - near.Z) * 0.5f

        let v =
            if near.Y < far.Y then
                wallElevationOffset
            else
                1f - wallElevationOffset

        let y = near.Y + (far.Y - near.Y) * v
        Vector3(x, y, z)
