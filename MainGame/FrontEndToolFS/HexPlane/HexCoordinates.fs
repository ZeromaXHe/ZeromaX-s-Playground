namespace FrontEndToolFS.HexPlane

open System.IO
open Godot

type HexCoordinates =
    struct
        val mutable X: int
        val mutable Z: int

        new(x, z) =
            let x =
                if HexMetrics.wrapping () then
                    let oX = x + z / 2

                    if oX < 0 then x + HexMetrics.wrapSize
                    elif oX >= HexMetrics.wrapSize then x - HexMetrics.wrapSize
                    else x
                else
                    x

            { X = x; Z = z }

        member this.Y = -this.X - this.Z

        member this.HexX =
            float32 (this.X + this.Z / 2) + if (this.Z &&& 1) = 0 then 0f else 0.5f

        member this.HexZ = float32 this.Z * HexMetrics.outerToInner

        static member FromOffsetCoordinates x z = HexCoordinates(x - z / 2, z)

        static member FromPosition(position: Vector3) =
            let x = position.X / HexMetrics.innerDiameter
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

        member this.DistanceTo(other: HexCoordinates) =
            let xy = (abs <| this.X - other.X) + (abs <| this.Y - other.Y)

            let xy' =
                if HexMetrics.wrapping () then
                    let plusX = other.X + HexMetrics.wrapSize
                    let xyWrappedPlus = (abs <| this.X - plusX) + (abs <| this.Y - other.Y)

                    if xyWrappedPlus < xy then
                        xyWrappedPlus
                    else
                        let minusX = other.X - HexMetrics.wrapSize
                        let xyWrappedMinus = (abs <| this.X - minusX) + (abs <| this.Y - other.Y)
                        if xyWrappedMinus < xy then xyWrappedMinus else xy
                else
                    xy

            (xy' + (abs <| this.Z - other.Z)) / 2

        member this.Step(direction: HexDirection) =
            match direction with
            | HexDirection.NE -> HexCoordinates(this.X, this.Z + 1)
            | HexDirection.E -> HexCoordinates(this.X + 1, this.Z)
            | HexDirection.SE -> HexCoordinates(this.X + 1, this.Z - 1)
            | HexDirection.SW -> HexCoordinates(this.X, this.Z - 1)
            | HexDirection.W -> HexCoordinates(this.X - 1, this.Z)
            | _ -> HexCoordinates(this.X - 1, this.Z + 1)

        member this.Save(writer: BinaryWriter) =
            writer.Write this.X
            writer.Write this.Z

        static member Load(reader: BinaryReader) =
            let x = reader.ReadInt32()
            let z = reader.ReadInt32()
            HexCoordinates(x, z)
    end
