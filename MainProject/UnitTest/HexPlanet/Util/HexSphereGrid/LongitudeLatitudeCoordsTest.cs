using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

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
}