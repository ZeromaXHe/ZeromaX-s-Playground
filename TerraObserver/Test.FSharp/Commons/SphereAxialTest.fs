namespace Test.FSharp.Commons

open TO.Domains.Functions.HexGridCoords
open TO.Domains.Types.HexGridCoords
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
        let mutable lonLat1 = northPole1 |> SphereAxial.toLonLat
        let mutable lonLat2 = southPole1 |> SphereAxial.toLonLat
        // 断言 Assert
        Assert.Equal(3, northPole1 |> SphereAxial.distanceTo southPole1)
        Assert.Equal(SphereAxialTypeEnum.PoleVertices, northPole1.Type)
        Assert.Equal(0, northPole1.TypeIdx)
        Assert.Equal(SphereAxialTypeEnum.PoleVertices, southPole1.Type)
        Assert.Equal(1, southPole1.TypeIdx)
        Assert.Equal("   0°00'00\", N90°00'00\"", lonLat1 |> LonLatCoords.toString)
        Assert.Equal("   0°00'00\", S90°00'00\"", lonLat2 |> LonLatCoords.toString)

        // Heuristic: Tile 890 ((-42, 4), Faces, 18) to Tile 848 ((-40, 0), MidVertices, 8) distance 4
        SphereAxial.Div <- 10
        let mutable sa1 = SphereAxial(-42, 4)
        let mutable sa2 = SphereAxial(-40, 0)
        lonLat1 <- sa1 |> SphereAxial.toLonLat
        lonLat2 <- sa2 |> SphereAxial.toLonLat
        Assert.Equal(4, sa1 |> SphereAxial.distanceTo sa2)
        Assert.Equal(4, sa2 |> SphereAxial.distanceTo sa1)
        Assert.Equal(SphereAxialTypeEnum.Faces, sa1.Type)
        Assert.Equal(18, sa1.TypeIdx)
        Assert.Equal(SphereAxialTypeEnum.MidVertices, sa2.Type)
        Assert.Equal(8, sa2.TypeIdx)
        Assert.Equal("E 72°00'00\", N 6°06'50\"", lonLat1 |> LonLatCoords.toString)
        Assert.Equal("E 72°00'00\", N29°08'29\"", lonLat2 |> LonLatCoords.toString)

        sa1 <- SphereAxial(-41, 7)
        sa2 <- SphereAxial(-42, 6)
        lonLat1 <- sa1 |> SphereAxial.toLonLat
        lonLat2 <- sa2 |> SphereAxial.toLonLat
        Assert.Equal(2, sa1 |> SphereAxial.distanceTo sa2)
        Assert.Equal(2, sa2 |> SphereAxial.distanceTo sa1)
        Assert.Equal(SphereAxialTypeEnum.Faces, sa1.Type)
        Assert.Equal(18, sa1.TypeIdx)
        Assert.Equal(SphereAxialTypeEnum.Faces, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)
        Assert.Equal("E 89°38'10\", S12°22'00\"", lonLat1 |> LonLatCoords.toString);
        Assert.Equal("E 79°06'03\", S 6°17'48\"", lonLat2 |> LonLatCoords.toString);

        sa2 <- SphereAxial(-41, 5)
        lonLat2 <- sa2 |> SphereAxial.toLonLat
        Assert.Equal(2, sa1 |> SphereAxial.distanceTo sa2)
        Assert.Equal(2, sa2 |> SphereAxial.distanceTo sa1)
        Assert.Equal(SphereAxialTypeEnum.Faces, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)
        Assert.Equal("E 82°48'00\",   0°00'00\"", lonLat2 |> LonLatCoords.toString)
        
        SphereAxial.Div <- 40
        sa1 <- SphereAxial(-92, -27)
        sa2 <- SphereAxial(-120, -27)
        lonLat1 <- sa1 |> SphereAxial.toLonLat
        lonLat2 <- sa2 |> SphereAxial.toLonLat
        Assert.Equal(1, sa1 |> SphereAxial.distanceTo sa2)
        Assert.Equal(1, sa2 |> SphereAxial.distanceTo sa1)
        Assert.Equal(SphereAxialTypeEnum.FacesSpecial, sa1.Type)
        Assert.Equal(8, sa1.TypeIdx)
        Assert.Equal(SphereAxialTypeEnum.EdgesSpecial, sa2.Type)
        Assert.Equal(18, sa2.TypeIdx)
        Assert.Equal("E148°31'00\", N71°09'46\"", lonLat1 |> LonLatCoords.toString)
        Assert.Equal("E144°00'00\", N70°13'15\"", lonLat2 |> LonLatCoords.toString)

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
            Assert.Equal(i / 2 * 4 + 1 + i % 2, midVertices[i] |> SphereAxial.index)
            Assert.Equal(i, midVertices[i].TypeIdx)
            Assert.Equal(SphereAxialTypeEnum.MidVertices, midVertices[i].Type)
            Assert.Equal((if i % 2 = 0 then div else 2 * div), midVertices[i] |> SphereAxial.distanceTo northPole)
            Assert.Equal((if i % 2 = 0 then div else 2 * div), northPole |> SphereAxial.distanceTo midVertices[i])
            Assert.Equal((if i % 2 = 1 then div else 2 * div), midVertices[i] |> SphereAxial.distanceTo southPole)
            Assert.Equal((if i % 2 = 1 then div else 2 * div), southPole |> SphereAxial.distanceTo midVertices[i])

        Assert.Equal(2 * div, midVertices[4] |> SphereAxial.distanceTo midVertices[0])
        Assert.Equal(3 * div, midVertices[4] |> SphereAxial.distanceTo midVertices[1])
        Assert.Equal(div, midVertices[4] |> SphereAxial.distanceTo midVertices[2])
        Assert.Equal(2 * div, midVertices[4] |> SphereAxial.distanceTo midVertices[3])
        Assert.Equal(div, midVertices[4] |> SphereAxial.distanceTo midVertices[5])
        Assert.Equal(div, midVertices[4] |> SphereAxial.distanceTo midVertices[6])
        Assert.Equal(div, midVertices[4] |> SphereAxial.distanceTo midVertices[7])
        Assert.Equal(2 * div, midVertices[4] |> SphereAxial.distanceTo midVertices[8])
        Assert.Equal(2 * div, midVertices[4] |> SphereAxial.distanceTo midVertices[9])

        Assert.Equal(2 * div, midVertices[0] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(3 * div, midVertices[1] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(div, midVertices[2] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(2 * div, midVertices[3] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(div, midVertices[5] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(div, midVertices[6] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(div, midVertices[7] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(2 * div, midVertices[8] |> SphereAxial.distanceTo midVertices[4])
        Assert.Equal(2 * div, midVertices[9] |> SphereAxial.distanceTo midVertices[4])
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
            Assert.Equal(i, faceCenters[i] |> SphereAxial.index)
            Assert.Equal(i, faceCenters[i].TypeIdx)

            Assert.Equal(
                (if i % 4 = 1 || i % 4 = 2 then
                     SphereAxialTypeEnum.Faces
                 else
                     SphereAxialTypeEnum.FacesSpecial),
                faceCenters[i].Type
            )

            let northDist =
                if i % 4 = 0 then div - 1
                elif i % 4 = 1 then div + 1
                elif i % 4 = 2 then 2 * div - 1
                else 2 * div + 1

            Assert.Equal(northDist, faceCenters[i] |> SphereAxial.distanceTo northPole)
            Assert.Equal(northDist, northPole |> SphereAxial.distanceTo faceCenters[i])

            let southDist =
                if i % 4 = 3 then div - 1
                elif i % 4 = 2 then div + 1
                elif i % 4 = 1 then 2 * div - 1
                else 2 * div + 1

            Assert.Equal(southDist, faceCenters[i] |> SphereAxial.distanceTo southPole)
            Assert.Equal(southDist, southPole |> SphereAxial.distanceTo faceCenters[i])

        let facesDist = [| 0; 2; 3; 5; 6; 8 |]
        // 针对北极圈面点的测试
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[0])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[1])
        Assert.Equal(facesDist[4], faceCenters[8] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[5], faceCenters[8] |> SphereAxial.distanceTo faceCenters[3])
        Assert.Equal(facesDist[1], faceCenters[8] |> SphereAxial.distanceTo faceCenters[4])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[6])
        Assert.Equal(facesDist[4], faceCenters[8] |> SphereAxial.distanceTo faceCenters[7])
        Assert.Equal(facesDist[1], faceCenters[8] |> SphereAxial.distanceTo faceCenters[9])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[10])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[11])
        Assert.Equal(facesDist[1], faceCenters[8] |> SphereAxial.distanceTo faceCenters[12])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[13])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[14])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[15])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[16])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[17])
        Assert.Equal(facesDist[3], faceCenters[8] |> SphereAxial.distanceTo faceCenters[18])
        Assert.Equal(facesDist[4], faceCenters[8] |> SphereAxial.distanceTo faceCenters[19])

        Assert.Equal(facesDist[2], faceCenters[0] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[1] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[4], faceCenters[2] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[5], faceCenters[3] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[1], faceCenters[4] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[6] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[4], faceCenters[7] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[1], faceCenters[9] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[10] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[11] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[1], faceCenters[12] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[13] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[14] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[15] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[16] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[17] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[18] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[4], faceCenters[19] |> SphereAxial.distanceTo faceCenters[8])
        // 针对赤道倒三角面测试
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[0])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[1])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[3])
        Assert.Equal(facesDist[1], faceCenters[5] |> SphereAxial.distanceTo faceCenters[4])
        Assert.Equal(facesDist[1], faceCenters[5] |> SphereAxial.distanceTo faceCenters[6])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[7])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[9])
        Assert.Equal(facesDist[1], faceCenters[5] |> SphereAxial.distanceTo faceCenters[10])
        Assert.Equal(facesDist[2], faceCenters[5] |> SphereAxial.distanceTo faceCenters[11])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[12])
        Assert.Equal(facesDist[4], faceCenters[5] |> SphereAxial.distanceTo faceCenters[13])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[14])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[15])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[16])
        Assert.Equal(facesDist[4], faceCenters[5] |> SphereAxial.distanceTo faceCenters[17])
        Assert.Equal(facesDist[5], faceCenters[5] |> SphereAxial.distanceTo faceCenters[18])
        Assert.Equal(facesDist[4], faceCenters[5] |> SphereAxial.distanceTo faceCenters[19])

        Assert.Equal(facesDist[2], faceCenters[0] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[1] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[3] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[1], faceCenters[4] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[1], faceCenters[6] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[7] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[8] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[9] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[1], faceCenters[10] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[11] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[12] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[4], faceCenters[13] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[14] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[15] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[16] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[4], faceCenters[17] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[5], faceCenters[18] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[4], faceCenters[19] |> SphereAxial.distanceTo faceCenters[5])
        // 针对赤道正三角面测试
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[0])
        Assert.Equal(facesDist[1], faceCenters[2] |> SphereAxial.distanceTo faceCenters[1])
        Assert.Equal(facesDist[1], faceCenters[2] |> SphereAxial.distanceTo faceCenters[3])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[4])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[6])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[7])
        Assert.Equal(facesDist[4], faceCenters[2] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[5], faceCenters[2] |> SphereAxial.distanceTo faceCenters[9])
        Assert.Equal(facesDist[4], faceCenters[2] |> SphereAxial.distanceTo faceCenters[10])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[11])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[12])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[13])
        Assert.Equal(facesDist[4], faceCenters[2] |> SphereAxial.distanceTo faceCenters[14])
        Assert.Equal(facesDist[3], faceCenters[2] |> SphereAxial.distanceTo faceCenters[15])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[16])
        Assert.Equal(facesDist[1], faceCenters[2] |> SphereAxial.distanceTo faceCenters[17])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[18])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[19])

        Assert.Equal(facesDist[2], faceCenters[0] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[1], faceCenters[1] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[1], faceCenters[3] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[4] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[5] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[2], faceCenters[6] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[2], faceCenters[7] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[4], faceCenters[8] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[5], faceCenters[9] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[4], faceCenters[10] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[11] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[12] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[13] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[4], faceCenters[14] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[3], faceCenters[15] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[2], faceCenters[16] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[1], faceCenters[17] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[2], faceCenters[18] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[2])
        // 针对南极圈面点的测试
        Assert.Equal(facesDist[4], faceCenters[19] |> SphereAxial.distanceTo faceCenters[0])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[1])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[2])
        Assert.Equal(facesDist[1], faceCenters[19] |> SphereAxial.distanceTo faceCenters[3])
        Assert.Equal(facesDist[5], faceCenters[19] |> SphereAxial.distanceTo faceCenters[4])
        Assert.Equal(facesDist[4], faceCenters[19] |> SphereAxial.distanceTo faceCenters[5])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[6])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[7])
        Assert.Equal(facesDist[4], faceCenters[19] |> SphereAxial.distanceTo faceCenters[8])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[9])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[10])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[11])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[12])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[13])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[14])
        Assert.Equal(facesDist[1], faceCenters[19] |> SphereAxial.distanceTo faceCenters[15])
        Assert.Equal(facesDist[3], faceCenters[19] |> SphereAxial.distanceTo faceCenters[16])
        Assert.Equal(facesDist[2], faceCenters[19] |> SphereAxial.distanceTo faceCenters[17])
        Assert.Equal(facesDist[1], faceCenters[19] |> SphereAxial.distanceTo faceCenters[18])

        Assert.Equal(facesDist[4], faceCenters[0] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[1] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[2] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[1], faceCenters[3] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[5], faceCenters[4] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[4], faceCenters[5] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[6] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[7] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[4], faceCenters[8] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[9] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[10] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[11] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[12] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[13] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[14] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[1], faceCenters[15] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[3], faceCenters[16] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[2], faceCenters[17] |> SphereAxial.distanceTo faceCenters[19])
        Assert.Equal(facesDist[1], faceCenters[18] |> SphereAxial.distanceTo faceCenters[19])
