using Commons.Utils.HexSphereGrid;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-10 20:35
public class LongitudeLatitudeCoordsTest
{
    [Fact]
    public void ToStringTest()
    {
        // 安排 Arrange
        // 行动 Act
        // 断言 Assert
        Assert.Equal("E179°00'00\", N88°00'00\"",
            new LongitudeLatitudeCoords(181f, 88f).ToString());
        Assert.Equal("   0°00'00\",   0°00'00\"",
            new LongitudeLatitudeCoords().ToString());
        // 舍入进位测试
        Assert.Equal("E 25°31'00\", S33°44'00\"",
            new LongitudeLatitudeCoords(-25.51666667f, -33.7333333f).ToString());
    }

    [Fact]
    public void SlerpTest()
    {
        // 安排 Arrange
        var from = new LongitudeLatitudeCoords(0f, 0f);
        var to = new LongitudeLatitudeCoords(180f, 90f);
        // 行动 Act
        var result1 = from.Slerp(to, 0.5f);
        var result2 = new LongitudeLatitudeCoords(-178f, 60f).Slerp(new LongitudeLatitudeCoords(179f, -30f), 0.5f);
        // 断言 Assert
        Assert.Equal("   0°00'00\", N45°00'00\"", result1.ToString());
        Assert.Equal("E179°54'07\", N15°00'16\"", result2.ToString());
    }
}