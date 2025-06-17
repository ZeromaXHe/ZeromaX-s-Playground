using Commons.Utils;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-08 16:01
public class Math3dUtilTest
{
    [Fact]
    public void GetNormalTest()
    {
        // 安排 Arrange
        var v0 = Vector3.Zero;
        var v1 = Vector3.Right;
        var v2 = Vector3.Back;
        // 行动 Act
        var normal = Math3dUtil.GetNormal(v0, v1, v2);
        // 断言 Assert
        Assert.Equal(Vector3.Up, normal);
    }

    [Fact]
    public void HexMetricsTerraceLerpTest()
    {
        var vFrom = Vector3.Right;
        var vTo = Vector3.Back;
        var v0 = HexMetrics.TerraceLerp(vFrom, vTo, 0);
        var v1 = HexMetrics.TerraceLerp(vFrom, vTo, 1);
        var v2 = HexMetrics.TerraceLerp(vFrom, vTo, 2);
        var v3 = HexMetrics.TerraceLerp(vFrom, vTo, 3);
        var v4 = HexMetrics.TerraceLerp(vFrom, vTo, 4);
        var v5 = HexMetrics.TerraceLerp(vFrom, vTo, 5);
        var vRev0 = HexMetrics.TerraceLerp(vTo, vFrom, 0);
        var vRev1 = HexMetrics.TerraceLerp(vTo, vFrom, 1);
        var vRev2 = HexMetrics.TerraceLerp(vTo, vFrom, 2);
        var vRev3 = HexMetrics.TerraceLerp(vTo, vFrom, 3);
        var vRev4 = HexMetrics.TerraceLerp(vTo, vFrom, 4);
        var vRev5 = HexMetrics.TerraceLerp(vTo, vFrom, 5);
        Assert.Equal(new Vector3(1, 0, 0), v0);
        Assert.Equal(new Vector3(0.9510566f, 0, 0.30901703f), v1);
        Assert.Equal(new Vector3(0.80901706f, 0, 0.58778524f), v2);
        Assert.Equal(new Vector3(0.58778524f, 0, 0.80901706f), v3);
        Assert.Equal(new Vector3(0.30901682f, 0, 0.9510566f), v4);
        Assert.Equal(new Vector3(-8.7422784E-08f, 0, 1f), v5);
        Assert.Equal(new Vector3(0f, 0f, 1f), vRev0);
        Assert.Equal(new Vector3(0.30901703f, 0f, 0.9510566f), vRev1);
        Assert.Equal(new Vector3(0.58778524f, 0f, 0.80901706f), vRev2);
        Assert.Equal(new Vector3(0.80901706f, 0f, 0.58778524f), vRev3);
        Assert.Equal(new Vector3(0.9510566f, 0f, 0.30901682f), vRev4);
        Assert.Equal(new Vector3(1f, 0f, -8.7422784E-08f), vRev5);
    }

    [Fact]
    public void EdgeVerticesTerraceLerpTest()
    {
        // 安排 Arrange
        var eFrom = new EdgeVertices(new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 0f));
        var eTo = new EdgeVertices(new Vector3(0f, 2f, 1f), new Vector3(1f, 2f, 1f));
        // 行动 Act
        var e0 = EdgeVertices.TerraceLerp(eFrom, eTo, 0);
        var e1 = EdgeVertices.TerraceLerp(eFrom, eTo, 1);
        var e2 = EdgeVertices.TerraceLerp(eFrom, eTo, 2);
        var e3 = EdgeVertices.TerraceLerp(eFrom, eTo, 3);
        var e4 = EdgeVertices.TerraceLerp(eFrom, eTo, 4);
        var e5 = EdgeVertices.TerraceLerp(eFrom, eTo, 5);
        var eRev0 = EdgeVertices.TerraceLerp(eTo, eFrom, 0);
        var eRev1 = EdgeVertices.TerraceLerp(eTo, eFrom, 1);
        var eRev2 = EdgeVertices.TerraceLerp(eTo, eFrom, 2);
        var eRev3 = EdgeVertices.TerraceLerp(eTo, eFrom, 3);
        var eRev4 = EdgeVertices.TerraceLerp(eTo, eFrom, 4);
        var eRev5 = EdgeVertices.TerraceLerp(eTo, eFrom, 5);
        // 断言 Assert
        Assert.Equal(new Vector3(0f, 1f, 0f), e0.V1);
        Assert.Equal(new Vector3(0.25f, 1f, 0f), e0.V2);
        Assert.Equal(new Vector3(0.5f, 1f, 0f), e0.V3);
        Assert.Equal(new Vector3(0.75f, 1f, 0f), e0.V4);
        Assert.Equal(new Vector3(1f, 1f, 0f), e0.V5);

        Assert.Equal(new Vector3(0f, 1.4059563f, 0.13074863f), e1.V1);
        Assert.Equal(new Vector3(0.31582415f, 1.3957801f, 0.13248347f), e1.V2);
        Assert.Equal(new Vector3(0.61670035f, 1.3705177f, 0.137117f), e1.V3);
        Assert.Equal(new Vector3(0.8976744f, 1.3402959f, 0.1433969f), e1.V4);
        Assert.Equal(new Vector3(1.1621263f, 1.312278f, 0.1501517f), e1.V5);

        Assert.Equal(new Vector3(0f, 1.3878089f, 0.2603738f), e2.V1);
        Assert.Equal(new Vector3(0.28023183f, 1.3846996f, 0.26377222f), e2.V2);
        Assert.Equal(new Vector3(0.5523779f, 1.377628f, 0.27287227f), e2.V3);
        Assert.Equal(new Vector3(0.8140524f, 1.370664f, 0.28526074f), e2.V4);
        Assert.Equal(new Vector3(1.0675031f, 1.3661613f, 0.29865828f), e2.V5);

        Assert.Equal(new Vector3(0f, 1.753919f, 0.50090903f), e3.V1);
        Assert.Equal(new Vector3(0.31057715f, 1.7460339f, 0.5037253f), e3.V2);
        Assert.Equal(new Vector3(0.6076264f, 1.726363f, 0.5111101f), e3.V3);
        Assert.Equal(new Vector3(0.8863941f, 1.702689f, 0.5208301f), e3.V4);
        Assert.Equal(new Vector3(1.1497227f, 1.6806861f, 0.5309635f), e3.V5);

        Assert.Equal(new Vector3(0f, 1.7000011f, 0.66116405f), e4.V1);
        Assert.Equal(new Vector3(0.25887758f, 1.7000567f, 0.6645462f), e4.V2);
        Assert.Equal(new Vector3(0.51369303f, 1.7009442f, 0.673558f), e4.V3);
        Assert.Equal(new Vector3(0.7635284f, 1.7037851f, 0.68574727f), e4.V4);
        Assert.Equal(new Vector3(1.0099523f, 1.7088209f, 0.6988687f), e4.V5);

        Assert.Equal(new Vector3(0f, 2f, 1f), e5.V1);
        Assert.Equal(new Vector3(0.25f, 1.9999999f, 0.99999994f), e5.V2);
        Assert.Equal(new Vector3(0.49999988f, 1.9999999f, 1f), e5.V3);
        Assert.Equal(new Vector3(0.7499999f, 1.9999999f, 1f), e5.V4);
        Assert.Equal(new Vector3(1f, 2f, 1f), e5.V5);

        Assert.Equal(new Vector3(0f, 2f, 1f), eRev0.V1);
        Assert.Equal(new Vector3(0.25f, 2f, 1f), eRev0.V2);
        Assert.Equal(new Vector3(0.5f, 2f, 1f), eRev0.V3);
        Assert.Equal(new Vector3(0.75f, 2f, 1f), eRev0.V4);
        Assert.Equal(new Vector3(1f, 2f, 1f), eRev0.V5);

        Assert.Equal(new Vector3(0f, 1.700001f, 0.6611639f), eRev1.V1);
        Assert.Equal(new Vector3(0.25887766f, 1.7000568f, 0.66454625f), eRev1.V2);
        Assert.Equal(new Vector3(0.5136931f, 1.7009442f, 0.673558f), eRev1.V3);
        Assert.Equal(new Vector3(0.7635284f, 1.703785f, 0.68574715f), eRev1.V4);
        Assert.Equal(new Vector3(1.0099524f, 1.7088209f, 0.6988686f), eRev1.V5);

        Assert.Equal(new Vector3(0f, 1.7539189f, 0.5009089f), eRev2.V1);
        Assert.Equal(new Vector3(0.31057712f, 1.7460339f, 0.50372523f), eRev2.V2);
        Assert.Equal(new Vector3(0.6076263f, 1.726363f, 0.5111101f), eRev2.V3);
        Assert.Equal(new Vector3(0.8863942f, 1.7026888f, 0.5208299f), eRev2.V4);
        Assert.Equal(new Vector3(1.1497227f, 1.6806861f, 0.53096336f), eRev2.V5);

        Assert.Equal(new Vector3(0f, 1.3878089f, 0.2603737f), eRev3.V1);
        Assert.Equal(new Vector3(0.28023186f, 1.3846996f, 0.26377222f), eRev3.V2);
        Assert.Equal(new Vector3(0.55237776f, 1.3776276f, 0.27287218f), eRev3.V3);
        Assert.Equal(new Vector3(0.8140526f, 1.370664f, 0.28526065f), eRev3.V4);
        Assert.Equal(new Vector3(1.067503f, 1.3661613f, 0.29865825f), eRev3.V5);

        Assert.Equal(new Vector3(0f, 1.4059561f, 0.13074857f), eRev4.V1);
        Assert.Equal(new Vector3(0.31582415f, 1.39578f, 0.13248348f), eRev4.V2);
        Assert.Equal(new Vector3(0.6167003f, 1.3705175f, 0.13711691f), eRev4.V3);
        Assert.Equal(new Vector3(0.8976744f, 1.3402959f, 0.14339675f), eRev4.V4);
        Assert.Equal(new Vector3(1.1621261f, 1.3122778f, 0.15015171f), eRev4.V5);

        Assert.Equal(new Vector3(0f, 1f, 0f), eRev5.V1);
        Assert.Equal(new Vector3(0.25000003f, 1f, -2.7306253E-08f), eRev5.V2);
        Assert.Equal(new Vector3(0.49999997f, 0.99999994f, 0f), eRev5.V3);
        Assert.Equal(new Vector3(0.75000006f, 1f, -9.4771195E-08f), eRev5.V4);
        Assert.Equal(new Vector3(1.0000001f, 1f, -3.441276E-08f), eRev5.V5);
    }

    [Fact]
    public void EdgeVerticesTerraceLerpRefactorTest()
    {
        // 安排 Arrange
        var eFrom = new EdgeVertices
        {
            V1 = new Vector3(-10.66387f, 22.593105f, -102.26439f),
            V2 = new Vector3(-11.281568f, 22.987444f, -102.10184f),
            V3 = new Vector3(-11.899265f, 23.381783f, -101.939285f),
            V4 = new Vector3(-12.516963f, 23.77612f, -101.77673f),
            V5 = new Vector3(-13.134661f, 24.170458f, -101.61418f)
        };

        var eTo = new EdgeVertices
        {
            V1 = new Vector3(-10.0670395f, 23.814316f, -102.74324f),
            V2 = new Vector3(-10.688653f, 24.21109f, -102.579254f),
            V3 = new Vector3(-11.3102665f, 24.607864f, -102.415276f),
            V4 = new Vector3(-11.93188f, 25.004639f, -102.2513f),
            V5 = new Vector3(-12.5534935f, 25.401413f, -102.08731f)
        };
        // 行动 Act
        var e1 = EdgeVertices.TerraceLerp(eFrom, eTo, 1);
        var e2 = EdgeVertices.TerraceLerp(eFrom, eTo, 2);
        var e3 = EdgeVertices.TerraceLerp(eFrom, eTo, 3);
        var e4 = EdgeVertices.TerraceLerp(eFrom, eTo, 4);
        // 断言 Assert
        Assert.Equal(new Vector3(-10.5543f, 22.856f, -102.44886f), e1.V1);
        Assert.Equal(new Vector3(-11.173315f, 23.25117f, -102.28589f), e1.V2);
        Assert.Equal(new Vector3(-11.792332f, 23.646338f, -102.12292f), e1.V3);
        Assert.Equal(new Vector3(-12.411348f, 24.041508f, -101.95994f), e1.V4);
        Assert.Equal(new Vector3(-13.030363f, 24.436672f, -101.79695f), e1.V5);

        Assert.Equal(new Vector3(-10.421891f, 23.070513f, -102.41434f), e2.V1);
        Assert.Equal(new Vector3(-11.040904f, 23.465664f, -102.251274f), e2.V2);
        Assert.Equal(new Vector3(-11.659915f, 23.860819f, -102.08823f), e2.V3);
        Assert.Equal(new Vector3(-12.278927f, 24.255972f, -101.92518f), e2.V4);
        Assert.Equal(new Vector3(-12.897937f, 24.651123f, -101.76211f), e2.V5);

        Assert.Equal(new Vector3(-10.311354f, 23.334518f, -102.597404f), e3.V1);
        Assert.Equal(new Vector3(-10.931677f, 23.730494f, -102.433914f), e3.V2);
        Assert.Equal(new Vector3(-11.551998f, 24.126467f, -102.27043f), e3.V3);
        Assert.Equal(new Vector3(-12.172319f, 24.522444f, -102.106964f), e3.V4);
        Assert.Equal(new Vector3(-12.792643f, 24.918419f, -101.94347f), e3.V5);

        Assert.Equal(new Vector3(-10.178544f, 23.549215f, -102.56159f), e4.V1);
        Assert.Equal(new Vector3(-10.798854f, 23.94517f, -102.39803f), e4.V2);
        Assert.Equal(new Vector3(-11.419164f, 24.341125f, -102.23449f), e4.V3);
        Assert.Equal(new Vector3(-12.0394745f, 24.737082f, -102.07092f), e4.V4);
        Assert.Equal(new Vector3(-12.659784f, 25.133038f, -101.907364f), e4.V5);
    }
    
    [Fact]
    public void HexMetricsTerraceLerpColorTest()
    {
        // 安排 Arrange
        var a = Colors.Green;
        var b = Colors.Red;
        // 行动 Act
        var c1 = HexMetrics.TerraceLerp(a, b, 1);
        var c2 = HexMetrics.TerraceLerp(a, b, 2);
        var c3 = HexMetrics.TerraceLerp(a, b, 3);
        var c4 = HexMetrics.TerraceLerp(a, b, 4);
        // 断言 Assert
        Assert.Equal(new Color(0.2f, 0.8f, 0f), c1);
        Assert.Equal(new Color(0.4f, 0.6f, 0f), c2);
        Assert.Equal(new Color(0.6f, 0.39999998f, 0f), c3);
        Assert.Equal(new Color(0.8f, 0.19999999f, 0f), c4);
    }
}