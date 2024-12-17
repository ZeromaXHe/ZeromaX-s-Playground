namespace FrontEndToolFS.HexPlane

open System
open System.IO

type HexFlags =
    | Empty = 0
    | RoadNE = 0b000001
    | RoadE = 0b000010
    | RoadSE = 0b000100
    | RoadSW = 0b001000
    | RoadW = 0b010000
    | RoadNW = 0b100000
    | Roads = 0b111111
    | RiverInNE = 0b000001_000000
    | RiverInE = 0b000010_000000
    | RiverInSE = 0b000100_000000
    | RiverInSW = 0b001000_000000
    | RiverInW = 0b010000_000000
    | RiverInNW = 0b100000_000000
    | RiverIn = 0b111111_000000
    | RiverOutNE = 0b000001_000000_000000
    | RiverOutE = 0b000010_000000_000000
    | RiverOutSE = 0b000100_000000_000000
    | RiverOutSW = 0b001000_000000_000000
    | RiverOutW = 0b010000_000000_000000
    | RiverOutNW = 0b100000_000000_000000
    | RiverOut = 0b111111_000000_000000
    | River = 0b111111_111111_000000
    | Walled = 0b1_000000_000000_000000
    | Explored = 0b010_000000_000000_000000
    | Explorable = 0b100_000000_000000_000000

module HexFlags =
    type HexFlags with

        member this.HasAny(mask: HexFlags) = this &&& mask <> HexFlags.Empty
        member this.HasAll(mask: HexFlags) = this &&& mask = mask
        member this.HasNone(mask: HexFlags) = this &&& mask = HexFlags.Empty
        member this.With(mask: HexFlags) = this ||| mask
        member this.Without(mask: HexFlags) = this &&& ~~~mask

        member this.Has(start: HexFlags, direction: HexDirection) =
            int this &&& (int start <<< int direction) <> 0

        member this.With(start: HexFlags, direction: HexDirection) =
            this ||| enum<HexFlags> (int start <<< int direction)

        member this.Without(start: HexFlags, direction: HexDirection) =
            this &&& ~~~(enum<HexFlags> (int start <<< int direction))

        member this.HasRoad(direction: HexDirection) = this.Has(HexFlags.RoadNE, direction)
        member this.WithRoad(direction: HexDirection) = this.With(HexFlags.RoadNE, direction)

        member this.WithoutRoad(direction: HexDirection) =
            this.Without(HexFlags.RoadNE, direction)

        member this.HasRiverIn(direction: HexDirection) = this.Has(HexFlags.RiverInNE, direction)

        member this.WithRiverIn(direction: HexDirection) =
            this.With(HexFlags.RiverInNE, direction)

        member this.WithoutRiverIn(direction: HexDirection) =
            this.Without(HexFlags.RiverInNE, direction)

        member this.HasRiverOut(direction: HexDirection) =
            this.Has(HexFlags.RiverOutNE, direction)

        member this.WithRiverOut(direction: HexDirection) =
            this.With(HexFlags.RiverOutNE, direction)

        member this.WithoutRiverOut(direction: HexDirection) =
            this.Without(HexFlags.RiverOutNE, direction)

        member this.ToDirection shift =
            match (int this >>> shift) &&& 0b111111 with
            | 0b000001 -> Some HexDirection.NE
            | 0b000010 -> Some HexDirection.E
            | 0b000100 -> Some HexDirection.SE
            | 0b001000 -> Some HexDirection.SW
            | 0b010000 -> Some HexDirection.W
            | 0b100000 -> Some HexDirection.NW
            | _ -> None

        member this.RiverInDirection = this.ToDirection 6
        member this.RiverOutDirection = this.ToDirection 12

        // 保存
        member this.Save(writer: BinaryWriter) =
            writer.Write(this.HasAny HexFlags.Walled)
            writer.Write(this.RiverInDirection |> Option.map byte |> Option.defaultValue Byte.MaxValue)
            writer.Write(this.RiverOutDirection |> Option.map byte |> Option.defaultValue Byte.MaxValue)
            writer.Write(byte (this &&& HexFlags.Roads))
            writer.Write(this.HasAll(HexFlags.Explored ||| HexFlags.Explorable))
        // 加载
        member this.Load (reader: BinaryReader) header =
            let mutable flags = this &&& HexFlags.Explorable

            if reader.ReadBoolean() then
                flags <- flags.With HexFlags.Walled

            match reader.ReadByte() with
            | Byte.MaxValue -> ()
            | x -> flags <- int x |> enum<HexDirection> |> flags.WithRiverIn

            match reader.ReadByte() with
            | Byte.MaxValue -> ()
            | x -> flags <- int x |> enum<HexDirection> |> flags.WithRiverOut

            flags <- flags ||| enum<HexFlags> (int <| reader.ReadByte())

            if header >= 3 && reader.ReadBoolean() then
                flags <- flags.With HexFlags.Explored

            flags
