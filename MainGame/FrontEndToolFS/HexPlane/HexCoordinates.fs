namespace FrontEndToolFS.HexPlane

type HexCoordinates =
    struct
        val X: int
        val Z: int
        new(x, z) = { X = x; Z = z }

        member this.Y = -this.X - this.Z

        static member FromOffsetCoordinates x z = HexCoordinates(x - z / 2, z)

        override this.ToString() = $"({this.X}, {this.Y}, {this.Z})"

        member this.ToStringOnSeparateLines() = $"{this.X}\n{this.Y}\n{this.Z}"
    end
