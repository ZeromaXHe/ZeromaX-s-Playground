namespace FrontEndToolFS.HexPlane

type HexDirection =
    | NE = 0
    | E = 1
    | SE = 2
    | SW = 3
    | W = 4
    | NW = 5

module HexDirection =
    let all () = [ 0..5 ] |> List.map enum<HexDirection>

    // 扩展方法（其他文件中需要 open HexDirection 模块后可用）
    type HexDirection with

        member dir.Opposite() =
            enum<HexDirection> <| if int dir < 3 then int dir + 3 else int dir - 3

        member dir.Previous() =
            if dir = HexDirection.NE then
                HexDirection.NW
            else
                enum<HexDirection> <| int dir - 1

        member dir.Next() =
            if dir = HexDirection.NW then
                HexDirection.NE
            else
                enum<HexDirection> <| int dir + 1
