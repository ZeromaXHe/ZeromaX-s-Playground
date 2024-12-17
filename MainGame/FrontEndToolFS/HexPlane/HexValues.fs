namespace FrontEndToolFS.HexPlane

open System.IO

type HexValues =
    struct
        val values: int
        new(values: int) = { values = values }

        member this.Get mask shift =
            int (uint this.values >>> shift) &&& mask

        member this.With value mask shift =
            HexValues((this.values &&& ~~~(mask <<< shift)) ||| ((value &&& mask) <<< shift))

        member this.Elevation = (this.Get 31 0) - 15
        member this.WithElevation value = this.With (value + 15) 31 0
        member this.WaterLevel = this.Get 31 5
        member this.WithWaterLevel value = this.With value 31 5
        member this.UrbanLevel = this.Get 3 10
        member this.WithUrbanLevel value = this.With value 3 10
        member this.FarmLevel = this.Get 3 12
        member this.WithFarmLevel value = this.With value 3 12
        member this.PlantLevel = this.Get 3 14
        member this.WithPlantLevel value = this.With value 3 14
        member this.SpecialIndex = this.Get 255 16
        member this.WithSpecialIndex index = this.With index 255 16
        member this.TerrainTypeIndex = this.Get 255 24
        member this.WithTerrainTypeIndex index = this.With index 255 24

        member this.Save(writer: BinaryWriter) =
            writer.Write(byte this.TerrainTypeIndex)
            writer.Write(byte <| this.Elevation + 127)
            writer.Write(byte <| this.WaterLevel)
            writer.Write(byte <| this.UrbanLevel)
            writer.Write(byte <| this.FarmLevel)
            writer.Write(byte <| this.PlantLevel)
            writer.Write(byte <| this.SpecialIndex)

        static member Load (reader: BinaryReader) header =
            HexValues()
            |> (fun v -> v.WithTerrainTypeIndex << int <| reader.ReadByte())
            |> (fun v ->
                let mutable elevation = int <| reader.ReadByte()

                if header >= 4 then
                    elevation <- elevation - 127

                v.WithElevation elevation)
            |> (fun v -> v.WithWaterLevel << int <| reader.ReadByte())
            |> (fun v -> v.WithUrbanLevel << int <| reader.ReadByte())
            |> (fun v -> v.WithFarmLevel << int <| reader.ReadByte())
            |> (fun v -> v.WithPlantLevel << int <| reader.ReadByte())
            |> (fun v -> v.WithSpecialIndex << int <| reader.ReadByte())
    end
