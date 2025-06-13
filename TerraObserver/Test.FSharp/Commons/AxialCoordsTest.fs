namespace Test.FSharp.Commons

open TO.FSharp.Commons.Structs.HexPlaneGrid
open Xunit

module AxialCoordsTest =
    [<Fact>]
    let ``旋转测试`` () =
        let coords = AxialCoords(-2, 4)
        let rotClockwise = coords.RotateClockwiseAround(AxialCoords(-3, 3))
        Assert.Equal(AxialCoords(-4, 5), rotClockwise)
        let rot2Clockwise = rotClockwise.RotateClockwiseAround(AxialCoords(-6, 3))
        Assert.Equal(AxialCoords(-8, 7), rot2Clockwise)
        let rotCounter = coords.RotateCounterClockwiseAround(AxialCoords(0, 3))
        Assert.Equal(AxialCoords(-1, 5), rotCounter)
        let rot2Counter = rotCounter.RotateCounterClockwiseAround(AxialCoords(3, 3))
        Assert.Equal(AxialCoords(1, 7), rot2Counter)
