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
        var northPole1 = new SphereAxial(0, -1, SphereAxial.TypeEnum.PoleVertices, 0);
        var southPole1 = new SphereAxial(-1, 2, SphereAxial.TypeEnum.PoleVertices, 1);
        // 断言 Assert
        Assert.Equal(3, northPole1.DistanceTo(southPole1));
        
        // Heuristic: Tile 890 ((-42, 4), Faces, 18) to Tile 848 ((-40, 0), MidVertices, 8) distance 4
        SphereAxial.Div = 10;
        var sa1 = new SphereAxial(-42, 4, SphereAxial.TypeEnum.Faces, 18);
        var sa2 = new SphereAxial(-40, 0, SphereAxial.TypeEnum.MidVertices, 8);
        Assert.Equal(4, sa1.DistanceTo(sa2));
        Assert.Equal(4, sa2.DistanceTo(sa1));
        
        sa1 = new SphereAxial(-41, 7, SphereAxial.TypeEnum.Faces, 18);
        sa2 = new SphereAxial(-42, 6, SphereAxial.TypeEnum.Faces, 18);
        Assert.Equal(2, sa1.DistanceTo(sa2));
        Assert.Equal(2, sa2.DistanceTo(sa1));
        
        sa2 = new SphereAxial(-41, 5, SphereAxial.TypeEnum.Faces, 18);
        Assert.Equal(2, sa1.DistanceTo(sa2));
        Assert.Equal(2, sa2.DistanceTo(sa1));
    }
}