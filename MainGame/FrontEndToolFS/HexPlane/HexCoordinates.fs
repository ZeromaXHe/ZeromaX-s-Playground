namespace FrontEndToolFS.HexPlane

open Godot

type HexCoordinates =
    struct
        val X: int
        val Z: int
        new(x, z) = { X = x; Z = z }

        member this.Y = -this.X - this.Z

        static member FromOffsetCoordinates x z = HexCoordinates(x - z / 2, z)

        static member FromPosition(position: Vector3) =
            let x = position.X / (HexMetrics.innerRadius * 2f)
            let y = -x
            let offset = position.Z / (HexMetrics.outerRadius * 3f)
            let x = x - offset
            let y = y - offset // 之前把这里 offset 直接提前去简化错了。那种情况下， y = -x - 2f * offset？
            let iX = Mathf.RoundToInt x
            let iY = Mathf.RoundToInt y
            let iZ = Mathf.RoundToInt(-x - y)

            if iX + iY + iZ = 0 then
                HexCoordinates(iX, iZ)
            else
                // GD.PrintErr $"rounding error! Pos: {position}"
                let dX = Mathf.Abs(x - float32 iX)
                let dY = Mathf.Abs(y - float32 iY)
                let dZ = Mathf.Abs(-x - y - float32 iZ)

                if dX > dY && dX > dZ then HexCoordinates(-iY - iZ, iZ)
                elif dZ > dY then HexCoordinates(iX, -iX - iY)
                else HexCoordinates(iX, iZ)


        override this.ToString() = $"({this.X}, {this.Y}, {this.Z})"

        member this.ToStringOnSeparateLines() = $"{this.X}\n{this.Y}\n{this.Z}"
    end
