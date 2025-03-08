using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

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
}