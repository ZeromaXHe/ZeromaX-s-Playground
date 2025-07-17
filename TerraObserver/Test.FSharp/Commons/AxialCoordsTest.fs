namespace Test.FSharp.Commons

open TO.Domains.Types.HexGridCoords
open TO.Domains.Functions.HexGridCoords
open Xunit

module AxialCoordsTest =
    [<Fact>]
    let ``旋转测试`` () =
        let coords = AxialCoords(-2, 4)
        let rotClockwise = coords |> AxialCoords.rotateClockwiseAround (AxialCoords(-3, 3))
        Assert.Equal(AxialCoords(-4, 5), rotClockwise)

        let rot2Clockwise =
            rotClockwise |> AxialCoords.rotateClockwiseAround (AxialCoords(-6, 3))

        Assert.Equal(AxialCoords(-8, 7), rot2Clockwise)

        let rotCounter =
            coords |> AxialCoords.rotateCounterClockwiseAround (AxialCoords(0, 3))

        Assert.Equal(AxialCoords(-1, 5), rotCounter)

        let rot2Counter =
            rotCounter |> AxialCoords.rotateCounterClockwiseAround (AxialCoords(3, 3))

        Assert.Equal(AxialCoords(1, 7), rot2Counter)
