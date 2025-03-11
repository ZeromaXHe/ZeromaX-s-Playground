using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

public class SphereAxialTest
{
    [Fact]
    public void DistanceToTest()
    {
        // 安排 Arrange
        // 行动 Act
        SphereAxial.Div = 1;
        var northPole1 = new SphereAxial(0, -1);
        var southPole1 = new SphereAxial(-1, 2);
        // 断言 Assert
        Assert.Equal(3, northPole1.DistanceTo(southPole1));
        Assert.Equal(SphereAxial.TypeEnum.PoleVertices, northPole1.Type);
        Assert.Equal(0, northPole1.TypeIdx);
        Assert.Equal(SphereAxial.TypeEnum.PoleVertices, southPole1.Type);
        Assert.Equal(1, southPole1.TypeIdx);

        // Heuristic: Tile 890 ((-42, 4), Faces, 18) to Tile 848 ((-40, 0), MidVertices, 8) distance 4
        SphereAxial.Div = 10;
        var sa1 = new SphereAxial(-42, 4);
        var sa2 = new SphereAxial(-40, 0);
        Assert.Equal(4, sa1.DistanceTo(sa2));
        Assert.Equal(4, sa2.DistanceTo(sa1));
        Assert.Equal(SphereAxial.TypeEnum.Faces, sa1.Type);
        Assert.Equal(18, sa1.TypeIdx);
        Assert.Equal(SphereAxial.TypeEnum.MidVertices, sa2.Type);
        Assert.Equal(8, sa2.TypeIdx);

        sa1 = new SphereAxial(-41, 7);
        sa2 = new SphereAxial(-42, 6);
        Assert.Equal(2, sa1.DistanceTo(sa2));
        Assert.Equal(2, sa2.DistanceTo(sa1));
        Assert.Equal(SphereAxial.TypeEnum.Faces, sa1.Type);
        Assert.Equal(18, sa1.TypeIdx);
        Assert.Equal(SphereAxial.TypeEnum.Faces, sa2.Type);
        Assert.Equal(18, sa2.TypeIdx);

        sa2 = new SphereAxial(-41, 5);
        Assert.Equal(2, sa1.DistanceTo(sa2));
        Assert.Equal(2, sa2.DistanceTo(sa1));
        Assert.Equal(SphereAxial.TypeEnum.Faces, sa2.Type);
        Assert.Equal(18, sa2.TypeIdx);
    }
}