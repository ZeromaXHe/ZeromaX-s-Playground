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

        SphereAxial.Div = 40;
        sa1 = new SphereAxial(-92, -27);
        sa2 = new SphereAxial(-120, -27);
        Assert.Equal(1, sa1.DistanceTo(sa2));
        Assert.Equal(1, sa2.DistanceTo(sa1));
        Assert.Equal(SphereAxial.TypeEnum.FacesSpecial, sa1.Type);
        Assert.Equal(8, sa1.TypeIdx);
        Assert.Equal(SphereAxial.TypeEnum.EdgesSpecial, sa2.Type);
        Assert.Equal(18, sa2.TypeIdx);
    }


    // 细分 3 下的全面距离测试
    [Fact]
    public void TestDiv3()
    {
        const int div = 3;
        SphereAxial.Div = div;
        var northPole = new SphereAxial(0, -div);
        var southPole = new SphereAxial(-div, 2 * div);
        // 二十面体顶点测试
        var midVertices = new SphereAxial[]
        {
            new(0, 0), new(0, div), new(-div, 0), new(-div, div), new(-2 * div, 0),
            new(-2 * div, div), new(-3 * div, 0), new(-3 * div, div), new(-4 * div, 0), new(-4 * div, div)
        };
        for (var i = 0; i < midVertices.Length; i++)
        {
            Assert.Equal(i / 2 * 4 + 1 + i % 2, midVertices[i].Index);
            Assert.Equal(i, midVertices[i].TypeIdx);
            Assert.Equal(SphereAxial.TypeEnum.MidVertices, midVertices[i].Type);
            Assert.Equal(i % 2 == 0 ? div : 2 * div, midVertices[i].DistanceTo(northPole));
            Assert.Equal(i % 2 == 0 ? div : 2 * div, northPole.DistanceTo(midVertices[i]));
            Assert.Equal(i % 2 == 1 ? div : 2 * div, midVertices[i].DistanceTo(southPole));
            Assert.Equal(i % 2 == 1 ? div : 2 * div, southPole.DistanceTo(midVertices[i]));
        }

        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[0]));
        Assert.Equal(3 * div, midVertices[4].DistanceTo(midVertices[1]));
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[2]));
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[3]));
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[5]));
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[6]));
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[7]));
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[8]));
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[9]));

        Assert.Equal(2 * div, midVertices[0].DistanceTo(midVertices[4]));
        Assert.Equal(3 * div, midVertices[1].DistanceTo(midVertices[4]));
        Assert.Equal(div, midVertices[2].DistanceTo(midVertices[4]));
        Assert.Equal(2 * div, midVertices[3].DistanceTo(midVertices[4]));
        Assert.Equal(div, midVertices[5].DistanceTo(midVertices[4]));
        Assert.Equal(div, midVertices[6].DistanceTo(midVertices[4]));
        Assert.Equal(div, midVertices[7].DistanceTo(midVertices[4]));
        Assert.Equal(2 * div, midVertices[8].DistanceTo(midVertices[4]));
        Assert.Equal(2 * div, midVertices[9].DistanceTo(midVertices[4]));
        // 面中心点测试
        var faceCenters = new SphereAxial[]
        {
            new(-1, -1), new(-2, 1), new(-1, 2), new(-2, 4),
            new(-div - 1, -1), new(-div - 2, 1), new(-div - 1, 2), new(-div - 2, 4),
            new(-2 * div - 1, -1), new(-2 * div - 2, 1), new(-2 * div - 1, 2), new(-2 * div - 2, 4),
            new(-3 * div - 1, -1), new(-3 * div - 2, 1), new(-3 * div - 1, 2), new(-3 * div - 2, 4),
            new(-4 * div - 1, -1), new(-4 * div - 2, 1), new(-4 * div - 1, 2), new(-4 * div - 2, 4),
        };
        for (var i = 0; i < faceCenters.Length; i++)
        {
            Assert.Equal(i, faceCenters[i].Index);
            Assert.Equal(i, faceCenters[i].TypeIdx);
            Assert.Equal(i % 4 == 1 || i % 4 == 2
                    ? SphereAxial.TypeEnum.Faces
                    : SphereAxial.TypeEnum.FacesSpecial,
                faceCenters[i].Type);
            var northDist = i % 4 == 0 ? div - 1 :
                i % 4 == 1 ? div + 1 :
                i % 4 == 2 ? 2 * div - 1 :
                2 * div + 1;
            Assert.Equal(northDist, faceCenters[i].DistanceTo(northPole));
            Assert.Equal(northDist, northPole.DistanceTo(faceCenters[i]));
            var southDist = i % 4 == 3 ? div - 1 :
                i % 4 == 2 ? div + 1 :
                i % 4 == 1 ? 2 * div - 1 :
                2 * div + 1;
            Assert.Equal(southDist, faceCenters[i].DistanceTo(southPole));
            Assert.Equal(southDist, southPole.DistanceTo(faceCenters[i]));
        }

        var facesDist = new[] { 0, 2, 3, 5, 6, 8 };
        // 针对北极圈面点的测试
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[0]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[1]));
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[5], faceCenters[8].DistanceTo(faceCenters[3]));
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[4]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[6]));
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[7]));
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[9]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[10]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[11]));
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[12]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[13]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[14]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[15]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[16]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[17]));
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[18]));
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[19]));

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[1].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[5], faceCenters[3].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[1], faceCenters[4].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[6].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[4], faceCenters[7].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[1], faceCenters[9].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[10].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[11].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[1], faceCenters[12].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[13].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[14].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[16].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[17].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[18].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[8]));
        // 针对赤道倒三角面测试
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[0]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[1]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[3]));
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[4]));
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[6]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[7]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[9]));
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[10]));
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[11]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[12]));
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[13]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[14]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[15]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[16]));
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[17]));
        Assert.Equal(facesDist[5], faceCenters[5].DistanceTo(faceCenters[18]));
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[19]));

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[1].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[3].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[1], faceCenters[4].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[1], faceCenters[6].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[9].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[1], faceCenters[10].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[11].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[4], faceCenters[13].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[14].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[16].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[4], faceCenters[17].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[5], faceCenters[18].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[5]));
        // 针对赤道正三角面测试
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[0]));
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[1]));
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[3]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[4]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[6]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[7]));
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[5], faceCenters[2].DistanceTo(faceCenters[9]));
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[10]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[11]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[12]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[13]));
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[14]));
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[15]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[16]));
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[17]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[18]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[19]));

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[1], faceCenters[1].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[1], faceCenters[3].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[4].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[2], faceCenters[6].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[5], faceCenters[9].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[4], faceCenters[10].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[11].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[13].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[4], faceCenters[14].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[2], faceCenters[16].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[1], faceCenters[17].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[2], faceCenters[18].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[2]));
        // 针对南极圈面点的测试
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[0]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[1]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[2]));
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[3]));
        Assert.Equal(facesDist[5], faceCenters[19].DistanceTo(faceCenters[4]));
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[5]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[6]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[7]));
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[8]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[9]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[10]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[11]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[12]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[13]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[14]));
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[15]));
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[16]));
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[17]));
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[18]));

        Assert.Equal(facesDist[4], faceCenters[0].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[1].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[1], faceCenters[3].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[5], faceCenters[4].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[6].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[9].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[10].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[11].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[13].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[14].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[1], faceCenters[15].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[3], faceCenters[16].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[2], faceCenters[17].DistanceTo(faceCenters[19]));
        Assert.Equal(facesDist[1], faceCenters[18].DistanceTo(faceCenters[19]));
    }
}