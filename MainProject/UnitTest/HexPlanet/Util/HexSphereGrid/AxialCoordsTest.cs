using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexPlaneGrid;

namespace UnitTest.HexPlanet.Util.HexSphereGrid;

public class AxialCoordsTest
{
    [Fact]
    public void RotationTest()
    {
        var coords = new AxialCoords(-2, 4);
        var rotClockwise = coords.RotateClockwiseAround(new AxialCoords(-3, 3));
        Assert.Equal(new AxialCoords(-4, 5), rotClockwise);
        var rot2Clockwise = rotClockwise.RotateClockwiseAround(new AxialCoords(-6, 3));
        Assert.Equal(new AxialCoords(-8, 7), rot2Clockwise);
        var rotCounter = coords.RotateCounterClockwiseAround(new AxialCoords(0, 3));
        Assert.Equal(new AxialCoords(-1, 5), rotCounter);
        var rot2Counter = rotCounter.RotateCounterClockwiseAround(new AxialCoords(3, 3));
        Assert.Equal(new AxialCoords(1, 7), rot2Counter);
    }
}