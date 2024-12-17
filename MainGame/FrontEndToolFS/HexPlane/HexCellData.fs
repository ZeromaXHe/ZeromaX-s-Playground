namespace FrontEndToolFS.HexPlane

open HexFlags

type HexCellData =
    struct
        val mutable flags: HexFlags
        val mutable values: HexValues
        val mutable coordinates: HexCoordinates

        member this.Elevation = this.values.Elevation
        member this.WaterLevel = this.values.WaterLevel
        member this.TerrainTypeIndex = this.values.TerrainTypeIndex
        member this.UrbanLevel = this.values.UrbanLevel
        member this.FarmLevel = this.values.FarmLevel
        member this.PlantLevel = this.values.PlantLevel
        member this.SpecialIndex = this.values.SpecialIndex
        member this.Walled = this.flags.HasAny HexFlags.Walled
        member this.HasRoads = this.flags.HasAny HexFlags.Roads
        member this.IsExplored = this.flags.HasAll(HexFlags.Explored ||| HexFlags.Explorable)
        member this.IsSpecial = this.values.SpecialIndex > 0
        member this.IsUnderWater = this.values.WaterLevel > this.values.Elevation
        member this.HasIncomingRiver = this.flags.HasAny HexFlags.RiverIn
        member this.HasOutgoingRiver = this.flags.HasAny HexFlags.RiverOut
        member this.HasRiver = this.flags.HasAny HexFlags.River
        member this.HasRiverBeginOrEnd = this.HasIncomingRiver <> this.HasOutgoingRiver
        member this.IncomingRiver = this.flags.RiverInDirection
        member this.OutgoingRiver = this.flags.RiverOutDirection

        member this.StreamBedY =
            (float32 this.values.Elevation + HexMetrics.streamBedElevationOffset)
            * HexMetrics.elevationStep

        member this.RiverSurfaceY =
            (float32 this.values.Elevation + HexMetrics.waterElevationOffset)
            * HexMetrics.elevationStep

        member this.WaterSurfaceY =
            (float32 this.values.WaterLevel + HexMetrics.waterElevationOffset)
            * HexMetrics.elevationStep

        member this.ViewElevation =
            if this.Elevation >= this.WaterLevel then
                this.Elevation
            else
                this.WaterLevel

        member this.GetEdgeType(otherCell: HexCellData) =
            HexMetrics.getEdgeType this.values.Elevation otherCell.values.Elevation

        member this.HasIncomingRiverThroughEdge(direction: HexDirection) = this.flags.HasRiverIn direction

        member this.HasRiverThroughEdge(direction: HexDirection) =
            this.flags.HasRiverIn direction || this.flags.HasRiverOut direction

        member this.HasRoadThroughEdge(direction: HexDirection) = this.flags.HasRoad direction
    end
