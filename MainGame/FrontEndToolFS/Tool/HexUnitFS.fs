namespace FrontEndToolFS.Tool

open System.IO
open FrontEndToolFS.HexPlane
open Godot

type IGrid =
    interface
        abstract member AddUnit: PackedScene -> HexCellFS option -> float32 -> unit
        abstract member GetCell: HexCoordinates -> HexCellFS option
    end

type HexUnitFS() as this =
    inherit CsgBox3D()

    let mutable location: HexCellFS option = None

    let mutable orientation = 0f

    member this.Orientation
        with get () = orientation
        and set value =
            orientation <- value
            this.RotationDegrees <- Vector3(0f, value, 0f)

    member this.Location
        with get () = location
        and set value =
            if location.IsSome then
                location.Value.Unit <- None

            location <- value
            value.Value.Unit <- Some this
            this.Position <- value.Value.Position + Vector3(0f, 5f, 0f)


    interface IUnit with
        override this.ValidateLocation() = this.ValidateLocation()
        override this.Die() = this.Die()

    member this.ValidateLocation() =
        this.Position <- location.Value.Position + Vector3(0f, 5f, 0f)

    member this.Die() =
        location.Value.Unit <- None
        this.QueueFree()

    member this.Save(writer: BinaryWriter) =
        location.Value.Coordinates.Save writer
        writer.Write orientation

    static member val unitPrefab: PackedScene = null with get, set

    static member Load (reader: BinaryReader) (grid: IGrid) =
        let coordinates = HexCoordinates.Load reader
        let orientation = reader.ReadSingle()
        grid.AddUnit <| HexUnitFS.unitPrefab <| grid.GetCell coordinates <| orientation

    member this.IsValidDestination(cell: HexCellFS) =
        not cell.IsUnderWater && cell.Unit.IsNone
