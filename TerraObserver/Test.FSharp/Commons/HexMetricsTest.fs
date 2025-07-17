namespace Test.FSharp.Commons

open Godot
open TO.Domains.Functions.HexMetrics
open TO.Domains.Types.HexMetrics
open Xunit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-13 15:46:13
module HexMetricsTest =
    [<Fact>]
    let ``terraceLerp test`` () =
        // 安排 Arrange
        let vFrom = Vector3.Right
        let vTo = Vector3.Back
        // 行动 Act
        let v0 = HexMetrics.terraceLerp vFrom vTo 0
        let v1 = HexMetrics.terraceLerp vFrom vTo 1
        let v2 = HexMetrics.terraceLerp vFrom vTo 2
        let v3 = HexMetrics.terraceLerp vFrom vTo 3
        let v4 = HexMetrics.terraceLerp vFrom vTo 4
        let v5 = HexMetrics.terraceLerp vFrom vTo 5
        let vRev0 = HexMetrics.terraceLerp vTo vFrom 0
        let vRev1 = HexMetrics.terraceLerp vTo vFrom 1
        let vRev2 = HexMetrics.terraceLerp vTo vFrom 2
        let vRev3 = HexMetrics.terraceLerp vTo vFrom 3
        let vRev4 = HexMetrics.terraceLerp vTo vFrom 4
        let vRev5 = HexMetrics.terraceLerp vTo vFrom 5
        // 断言 Assert
        Assert.Equal(Vector3(1f, 0f, 0f), v0)
        Assert.Equal(Vector3(0.9510566f, 0f, 0.30901703f), v1)
        Assert.Equal(Vector3(0.80901706f, 0f, 0.58778524f), v2)
        Assert.Equal(Vector3(0.58778524f, 0f, 0.80901706f), v3)
        Assert.Equal(Vector3(0.30901682f, 0f, 0.9510566f), v4)
        Assert.Equal(Vector3(-8.7422784E-08f, 0f, 1f), v5)
        Assert.Equal(Vector3(0f, 0f, 1f), vRev0)
        Assert.Equal(Vector3(0.30901703f, 0f, 0.9510566f), vRev1)
        Assert.Equal(Vector3(0.58778524f, 0f, 0.80901706f), vRev2)
        Assert.Equal(Vector3(0.80901706f, 0f, 0.58778524f), vRev3)
        Assert.Equal(Vector3(0.9510566f, 0f, 0.30901682f), vRev4)
        Assert.Equal(Vector3(1f, 0f, -8.7422784E-08f), vRev5)

    [<Fact>]
    let ``terraceLerpEdgeV test`` () =
        // 安排 Arrange
        let eFrom = EdgeVertices(Vector3(0f, 1f, 0f), Vector3(1f, 1f, 0f))
        let eTo = EdgeVertices(Vector3(0f, 2f, 1f), Vector3(1f, 2f, 1f))
        // 行动 Act
        let e0 = HexMetrics.terraceLerpEdgeV eFrom eTo 0
        let e1 = HexMetrics.terraceLerpEdgeV eFrom eTo 1
        let e2 = HexMetrics.terraceLerpEdgeV eFrom eTo 2
        let e3 = HexMetrics.terraceLerpEdgeV eFrom eTo 3
        let e4 = HexMetrics.terraceLerpEdgeV eFrom eTo 4
        let e5 = HexMetrics.terraceLerpEdgeV eFrom eTo 5
        let eRev0 = HexMetrics.terraceLerpEdgeV eTo eFrom 0
        let eRev1 = HexMetrics.terraceLerpEdgeV eTo eFrom 1
        let eRev2 = HexMetrics.terraceLerpEdgeV eTo eFrom 2
        let eRev3 = HexMetrics.terraceLerpEdgeV eTo eFrom 3
        let eRev4 = HexMetrics.terraceLerpEdgeV eTo eFrom 4
        let eRev5 = HexMetrics.terraceLerpEdgeV eTo eFrom 5
        // 断言 Assert
        Assert.Equal(Vector3(0f, 1f, 0f), e0.V1)
        Assert.Equal(Vector3(0.25f, 1f, 0f), e0.V2)
        Assert.Equal(Vector3(0.5f, 1f, 0f), e0.V3)
        Assert.Equal(Vector3(0.75f, 1f, 0f), e0.V4)
        Assert.Equal(Vector3(1f, 1f, 0f), e0.V5)

        Assert.Equal(Vector3(0f, 1.4059563f, 0.13074863f), e1.V1)
        Assert.Equal(Vector3(0.31582415f, 1.3957801f, 0.13248347f), e1.V2)
        Assert.Equal(Vector3(0.61670035f, 1.3705177f, 0.137117f), e1.V3)
        Assert.Equal(Vector3(0.8976744f, 1.3402959f, 0.1433969f), e1.V4)
        Assert.Equal(Vector3(1.1621263f, 1.312278f, 0.1501517f), e1.V5)

        Assert.Equal(Vector3(0f, 1.3878089f, 0.2603738f), e2.V1)
        Assert.Equal(Vector3(0.28023183f, 1.3846996f, 0.26377222f), e2.V2)
        Assert.Equal(Vector3(0.5523779f, 1.377628f, 0.27287227f), e2.V3)
        Assert.Equal(Vector3(0.8140524f, 1.370664f, 0.28526074f), e2.V4)
        Assert.Equal(Vector3(1.0675031f, 1.3661613f, 0.29865828f), e2.V5)

        Assert.Equal(Vector3(0f, 1.753919f, 0.50090903f), e3.V1)
        Assert.Equal(Vector3(0.31057715f, 1.7460339f, 0.5037253f), e3.V2)
        Assert.Equal(Vector3(0.6076264f, 1.726363f, 0.5111101f), e3.V3)
        Assert.Equal(Vector3(0.8863941f, 1.702689f, 0.5208301f), e3.V4)
        Assert.Equal(Vector3(1.1497227f, 1.6806861f, 0.5309635f), e3.V5)

        Assert.Equal(Vector3(0f, 1.7000011f, 0.66116405f), e4.V1)
        Assert.Equal(Vector3(0.25887758f, 1.7000567f, 0.6645462f), e4.V2)
        Assert.Equal(Vector3(0.51369303f, 1.7009442f, 0.673558f), e4.V3)
        Assert.Equal(Vector3(0.7635284f, 1.7037851f, 0.68574727f), e4.V4)
        Assert.Equal(Vector3(1.0099523f, 1.7088209f, 0.6988687f), e4.V5)

        Assert.Equal(Vector3(0f, 2f, 1f), e5.V1)
        Assert.Equal(Vector3(0.25f, 1.9999999f, 0.99999994f), e5.V2)
        Assert.Equal(Vector3(0.49999988f, 1.9999999f, 1f), e5.V3)
        Assert.Equal(Vector3(0.7499999f, 1.9999999f, 1f), e5.V4)
        Assert.Equal(Vector3(1f, 2f, 1f), e5.V5)

        Assert.Equal(Vector3(0f, 2f, 1f), eRev0.V1)
        Assert.Equal(Vector3(0.25f, 2f, 1f), eRev0.V2)
        Assert.Equal(Vector3(0.5f, 2f, 1f), eRev0.V3)
        Assert.Equal(Vector3(0.75f, 2f, 1f), eRev0.V4)
        Assert.Equal(Vector3(1f, 2f, 1f), eRev0.V5)

        Assert.Equal(Vector3(0f, 1.700001f, 0.6611639f), eRev1.V1)
        Assert.Equal(Vector3(0.25887766f, 1.7000568f, 0.66454625f), eRev1.V2)
        Assert.Equal(Vector3(0.5136931f, 1.7009442f, 0.673558f), eRev1.V3)
        Assert.Equal(Vector3(0.7635284f, 1.703785f, 0.68574715f), eRev1.V4)
        Assert.Equal(Vector3(1.0099524f, 1.7088209f, 0.6988686f), eRev1.V5)

        Assert.Equal(Vector3(0f, 1.7539189f, 0.5009089f), eRev2.V1)
        Assert.Equal(Vector3(0.31057712f, 1.7460339f, 0.50372523f), eRev2.V2)
        Assert.Equal(Vector3(0.6076263f, 1.726363f, 0.5111101f), eRev2.V3)
        Assert.Equal(Vector3(0.8863942f, 1.7026888f, 0.5208299f), eRev2.V4)
        Assert.Equal(Vector3(1.1497227f, 1.6806861f, 0.53096336f), eRev2.V5)

        Assert.Equal(Vector3(0f, 1.3878089f, 0.2603737f), eRev3.V1)
        Assert.Equal(Vector3(0.28023186f, 1.3846996f, 0.26377222f), eRev3.V2)
        Assert.Equal(Vector3(0.55237776f, 1.3776276f, 0.27287218f), eRev3.V3)
        Assert.Equal(Vector3(0.8140526f, 1.370664f, 0.28526065f), eRev3.V4)
        Assert.Equal(Vector3(1.067503f, 1.3661613f, 0.29865825f), eRev3.V5)

        Assert.Equal(Vector3(0f, 1.4059561f, 0.13074857f), eRev4.V1)
        Assert.Equal(Vector3(0.31582415f, 1.39578f, 0.13248348f), eRev4.V2)
        Assert.Equal(Vector3(0.6167003f, 1.3705175f, 0.13711691f), eRev4.V3)
        Assert.Equal(Vector3(0.8976744f, 1.3402959f, 0.14339675f), eRev4.V4)
        Assert.Equal(Vector3(1.1621261f, 1.3122778f, 0.15015171f), eRev4.V5)

        Assert.Equal(Vector3(0f, 1f, 0f), eRev5.V1)
        Assert.Equal(Vector3(0.25000003f, 1f, -2.7306253E-08f), eRev5.V2)
        Assert.Equal(Vector3(0.49999997f, 0.99999994f, 0f), eRev5.V3)
        Assert.Equal(Vector3(0.75000006f, 1f, -9.4771195E-08f), eRev5.V4)
        Assert.Equal(Vector3(1.0000001f, 1f, -3.441276E-08f), eRev5.V5)

    [<Fact>]
    let ``terraceLerpEdgeV test 2`` () =
        // 安排 Arrange
        let eFrom =
            EdgeVertices(
                V1 = Vector3(-10.66387f, 22.593105f, -102.26439f),
                V2 = Vector3(-11.281568f, 22.987444f, -102.10184f),
                V3 = Vector3(-11.899265f, 23.381783f, -101.939285f),
                V4 = Vector3(-12.516963f, 23.77612f, -101.77673f),
                V5 = Vector3(-13.134661f, 24.170458f, -101.61418f)
            )

        let eTo =
            EdgeVertices(
                V1 = Vector3(-10.0670395f, 23.814316f, -102.74324f),
                V2 = Vector3(-10.688653f, 24.21109f, -102.579254f),
                V3 = Vector3(-11.3102665f, 24.607864f, -102.415276f),
                V4 = Vector3(-11.93188f, 25.004639f, -102.2513f),
                V5 = Vector3(-12.5534935f, 25.401413f, -102.08731f)
            )
        // 行动 Act
        let e1 = HexMetrics.terraceLerpEdgeV eFrom eTo 1
        let e2 = HexMetrics.terraceLerpEdgeV eFrom eTo 2
        let e3 = HexMetrics.terraceLerpEdgeV eFrom eTo 3
        let e4 = HexMetrics.terraceLerpEdgeV eFrom eTo 4
        // 断言 Assert
        Assert.Equal(Vector3(-10.5543f, 22.856f, -102.44886f), e1.V1)
        Assert.Equal(Vector3(-11.173315f, 23.25117f, -102.28589f), e1.V2)
        Assert.Equal(Vector3(-11.792332f, 23.646338f, -102.12292f), e1.V3)
        Assert.Equal(Vector3(-12.411348f, 24.041508f, -101.95994f), e1.V4)
        Assert.Equal(Vector3(-13.030363f, 24.436672f, -101.79695f), e1.V5)

        Assert.Equal(Vector3(-10.421891f, 23.070513f, -102.41434f), e2.V1)
        Assert.Equal(Vector3(-11.040904f, 23.465664f, -102.251274f), e2.V2)
        Assert.Equal(Vector3(-11.659915f, 23.860819f, -102.08823f), e2.V3)
        Assert.Equal(Vector3(-12.278927f, 24.255972f, -101.92518f), e2.V4)
        Assert.Equal(Vector3(-12.897937f, 24.651123f, -101.76211f), e2.V5)

        Assert.Equal(Vector3(-10.311354f, 23.334518f, -102.597404f), e3.V1)
        Assert.Equal(Vector3(-10.931677f, 23.730494f, -102.433914f), e3.V2)
        Assert.Equal(Vector3(-11.551998f, 24.126467f, -102.27043f), e3.V3)
        Assert.Equal(Vector3(-12.172319f, 24.522444f, -102.106964f), e3.V4)
        Assert.Equal(Vector3(-12.792643f, 24.918419f, -101.94347f), e3.V5)

        Assert.Equal(Vector3(-10.178544f, 23.549215f, -102.56159f), e4.V1)
        Assert.Equal(Vector3(-10.798854f, 23.94517f, -102.39803f), e4.V2)
        Assert.Equal(Vector3(-11.419164f, 24.341125f, -102.23449f), e4.V3)
        Assert.Equal(Vector3(-12.0394745f, 24.737082f, -102.07092f), e4.V4)
        Assert.Equal(Vector3(-12.659784f, 25.133038f, -101.907364f), e4.V5)

    [<Fact>]
    let ``terraceLerpColor test`` () =
        // 安排 Arrange
        let a = Colors.Green
        let b = Colors.Red
        // 行动 Act
        let c1 = HexMetrics.terraceLerpColor a b 1
        let c2 = HexMetrics.terraceLerpColor a b 2
        let c3 = HexMetrics.terraceLerpColor a b 3
        let c4 = HexMetrics.terraceLerpColor a b 4
        // 断言 Assert
        Assert.Equal(Color(0.2f, 0.8f, 0f), c1)
        Assert.Equal(Color(0.4f, 0.6f, 0f), c2)
        Assert.Equal(Color(0.6f, 0.39999998f, 0f), c3)
        Assert.Equal(Color(0.8f, 0.19999999f, 0f), c4)