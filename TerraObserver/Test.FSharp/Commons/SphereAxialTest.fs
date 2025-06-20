namespace Test.FSharp.Commons

open TO.Domains.Structs.HexSphereGrids
open Xunit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-19 09:42:19
module SphereAxialTest =
    [<Fact>]
    let ``DistanceTo 距离测试`` () =
        // 安排 Arrange
        // 行动 Act
        SphereAxial.Div <- 1
        let northPole1 = SphereAxial(0, -1)
        let southPole1 = SphereAxial(-1, 2)
        // 断言 Assert
        Assert.Equal(3, northPole1.DistanceTo(southPole1))
        Assert.Equal(TypeEnum.PoleVertices, northPole1.Type)
        Assert.Equal(0, northPole1.TypeIdx)
        Assert.Equal(TypeEnum.PoleVertices, southPole1.Type)
        Assert.Equal(1, southPole1.TypeIdx)

        // Heuristic: Tile 890 ((-42, 4), Faces, 18) to Tile 848 ((-40, 0), MidVertices, 8) distance 4
        SphereAxial.Div <- 10
        let mutable sa1 = SphereAxial(-42, 4)
        let mutable sa2 = SphereAxial(-40, 0)
        Assert.Equal(4, sa1.DistanceTo(sa2))
        Assert.Equal(4, sa2.DistanceTo(sa1))
        Assert.Equal(TypeEnum.Faces, sa1.Type)
        Assert.Equal(18, sa1.TypeIdx)
        Assert.Equal(TypeEnum.MidVertices, sa2.Type)
        Assert.Equal(8, sa2.TypeIdx)

        sa1 <- SphereAxial(-41, 7)
        sa2 <- SphereAxial(-42, 6)
        Assert.Equal(2, sa1.DistanceTo(sa2))
        Assert.Equal(2, sa2.DistanceTo(sa1))
        Assert.Equal(TypeEnum.Faces, sa1.Type)
        Assert.Equal(18, sa1.TypeIdx)
        Assert.Equal(TypeEnum.Faces, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)

        sa2 <- SphereAxial(-41, 5)
        Assert.Equal(2, sa1.DistanceTo(sa2))
        Assert.Equal(2, sa2.DistanceTo(sa1))
        Assert.Equal(TypeEnum.Faces, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)

        SphereAxial.Div <- 40
        sa1 <- SphereAxial(-92, -27)
        sa2 <- SphereAxial(-120, -27)
        Assert.Equal(1, sa1.DistanceTo(sa2))
        Assert.Equal(1, sa2.DistanceTo(sa1))
        Assert.Equal(TypeEnum.FacesSpecial, sa1.Type)
        Assert.Equal(8, sa1.TypeIdx)
        Assert.Equal(TypeEnum.EdgesSpecial, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)

    [<Fact>]
    let ``细分 3 下的全面距离测试`` () =
        let div = 3
        SphereAxial.Div <- div
        let northPole = SphereAxial(0, -div)
        let southPole = SphereAxial(-div, 2 * div)
        // 二十面体顶点测试
        let midVertices =
            [| SphereAxial(0, 0)
               SphereAxial(0, div)
               SphereAxial(-div, 0)
               SphereAxial(-div, div)
               SphereAxial(-2 * div, 0)
               SphereAxial(-2 * div, div)
               SphereAxial(-3 * div, 0)
               SphereAxial(-3 * div, div)
               SphereAxial(-4 * div, 0)
               SphereAxial(-4 * div, div) |]

        for i = 0 to midVertices.Length - 1 do
            Assert.Equal(i / 2 * 4 + 1 + i % 2, midVertices[i].Index)
            Assert.Equal(i, midVertices[i].TypeIdx)
            Assert.Equal(TypeEnum.MidVertices, midVertices[i].Type)
            Assert.Equal((if i % 2 = 0 then div else 2 * div), midVertices[i].DistanceTo(northPole))
            Assert.Equal((if i % 2 = 0 then div else 2 * div), northPole.DistanceTo(midVertices[i]))
            Assert.Equal((if i % 2 = 1 then div else 2 * div), midVertices[i].DistanceTo(southPole))
            Assert.Equal((if i % 2 = 1 then div else 2 * div), southPole.DistanceTo(midVertices[i]))

        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[0]))
        Assert.Equal(3 * div, midVertices[4].DistanceTo(midVertices[1]))
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[2]))
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[3]))
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[5]))
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[6]))
        Assert.Equal(div, midVertices[4].DistanceTo(midVertices[7]))
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[8]))
        Assert.Equal(2 * div, midVertices[4].DistanceTo(midVertices[9]))

        Assert.Equal(2 * div, midVertices[0].DistanceTo(midVertices[4]))
        Assert.Equal(3 * div, midVertices[1].DistanceTo(midVertices[4]))
        Assert.Equal(div, midVertices[2].DistanceTo(midVertices[4]))
        Assert.Equal(2 * div, midVertices[3].DistanceTo(midVertices[4]))
        Assert.Equal(div, midVertices[5].DistanceTo(midVertices[4]))
        Assert.Equal(div, midVertices[6].DistanceTo(midVertices[4]))
        Assert.Equal(div, midVertices[7].DistanceTo(midVertices[4]))
        Assert.Equal(2 * div, midVertices[8].DistanceTo(midVertices[4]))
        Assert.Equal(2 * div, midVertices[9].DistanceTo(midVertices[4]))
        // 面中心点测试
        let faceCenters =
            [| SphereAxial(-1, -1)
               SphereAxial(-2, 1)
               SphereAxial(-1, 2)
               SphereAxial(-2, 4)
               SphereAxial(-div - 1, -1)
               SphereAxial(-div - 2, 1)
               SphereAxial(-div - 1, 2)
               SphereAxial(-div - 2, 4)
               SphereAxial(-2 * div - 1, -1)
               SphereAxial(-2 * div - 2, 1)
               SphereAxial(-2 * div - 1, 2)
               SphereAxial(-2 * div - 2, 4)
               SphereAxial(-3 * div - 1, -1)
               SphereAxial(-3 * div - 2, 1)
               SphereAxial(-3 * div - 1, 2)
               SphereAxial(-3 * div - 2, 4)
               SphereAxial(-4 * div - 1, -1)
               SphereAxial(-4 * div - 2, 1)
               SphereAxial(-4 * div - 1, 2)
               SphereAxial(-4 * div - 2, 4) |]

        for i = 0 to faceCenters.Length - 1 do
            Assert.Equal(i, faceCenters[i].Index)
            Assert.Equal(i, faceCenters[i].TypeIdx)

            Assert.Equal(
                (if i % 4 = 1 || i % 4 = 2 then
                     TypeEnum.Faces
                 else
                     TypeEnum.FacesSpecial),
                faceCenters[i].Type
            )

            let northDist =
                if i % 4 = 0 then div - 1
                elif i % 4 = 1 then div + 1
                elif i % 4 = 2 then 2 * div - 1
                else 2 * div + 1

            Assert.Equal(northDist, faceCenters[i].DistanceTo(northPole))
            Assert.Equal(northDist, northPole.DistanceTo(faceCenters[i]))

            let southDist =
                if i % 4 = 3 then div - 1
                elif i % 4 = 2 then div + 1
                elif i % 4 = 1 then 2 * div - 1
                else 2 * div + 1

            Assert.Equal(southDist, faceCenters[i].DistanceTo(southPole))
            Assert.Equal(southDist, southPole.DistanceTo(faceCenters[i]))

        let facesDist = [| 0; 2; 3; 5; 6; 8 |]
        // 针对北极圈面点的测试
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[0]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[1]))
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[5], faceCenters[8].DistanceTo(faceCenters[3]))
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[4]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[6]))
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[7]))
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[9]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[10]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[11]))
        Assert.Equal(facesDist[1], faceCenters[8].DistanceTo(faceCenters[12]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[13]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[14]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[15]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[16]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[17]))
        Assert.Equal(facesDist[3], faceCenters[8].DistanceTo(faceCenters[18]))
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[19]))

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[1].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[5], faceCenters[3].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[1], faceCenters[4].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[6].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[4], faceCenters[7].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[1], faceCenters[9].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[10].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[11].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[1], faceCenters[12].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[13].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[14].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[16].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[17].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[18].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[8]))
        // 针对赤道倒三角面测试
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[0]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[1]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[3]))
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[4]))
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[6]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[7]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[9]))
        Assert.Equal(facesDist[1], faceCenters[5].DistanceTo(faceCenters[10]))
        Assert.Equal(facesDist[2], faceCenters[5].DistanceTo(faceCenters[11]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[12]))
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[13]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[14]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[15]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[16]))
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[17]))
        Assert.Equal(facesDist[5], faceCenters[5].DistanceTo(faceCenters[18]))
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[19]))

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[1].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[3].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[1], faceCenters[4].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[1], faceCenters[6].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[8].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[9].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[1], faceCenters[10].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[11].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[4], faceCenters[13].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[14].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[16].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[4], faceCenters[17].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[5], faceCenters[18].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[5]))
        // 针对赤道正三角面测试
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[0]))
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[1]))
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[3]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[4]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[6]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[7]))
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[5], faceCenters[2].DistanceTo(faceCenters[9]))
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[10]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[11]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[12]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[13]))
        Assert.Equal(facesDist[4], faceCenters[2].DistanceTo(faceCenters[14]))
        Assert.Equal(facesDist[3], faceCenters[2].DistanceTo(faceCenters[15]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[16]))
        Assert.Equal(facesDist[1], faceCenters[2].DistanceTo(faceCenters[17]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[18]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[19]))

        Assert.Equal(facesDist[2], faceCenters[0].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[1], faceCenters[1].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[1], faceCenters[3].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[4].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[5].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[2], faceCenters[6].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[5], faceCenters[9].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[4], faceCenters[10].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[11].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[13].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[4], faceCenters[14].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[3], faceCenters[15].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[2], faceCenters[16].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[1], faceCenters[17].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[2], faceCenters[18].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[2]))
        // 针对南极圈面点的测试
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[0]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[1]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[2]))
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[3]))
        Assert.Equal(facesDist[5], faceCenters[19].DistanceTo(faceCenters[4]))
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[5]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[6]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[7]))
        Assert.Equal(facesDist[4], faceCenters[19].DistanceTo(faceCenters[8]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[9]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[10]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[11]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[12]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[13]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[14]))
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[15]))
        Assert.Equal(facesDist[3], faceCenters[19].DistanceTo(faceCenters[16]))
        Assert.Equal(facesDist[2], faceCenters[19].DistanceTo(faceCenters[17]))
        Assert.Equal(facesDist[1], faceCenters[19].DistanceTo(faceCenters[18]))

        Assert.Equal(facesDist[4], faceCenters[0].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[1].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[2].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[1], faceCenters[3].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[5], faceCenters[4].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[4], faceCenters[5].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[6].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[7].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[4], faceCenters[8].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[9].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[10].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[11].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[12].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[13].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[14].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[1], faceCenters[15].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[3], faceCenters[16].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[2], faceCenters[17].DistanceTo(faceCenters[19]))
        Assert.Equal(facesDist[1], faceCenters[18].DistanceTo(faceCenters[19]))
