namespace FrontEndToolFS.Tool

open System
open System.Collections.Generic
open FrontEndToolFS.HexPlane.HexDirection
open FrontEndToolFS.HexPlane
open Godot

type MapRegion =
    struct
        val mutable xMin: int
        val mutable xMax: int
        val mutable zMin: int
        val mutable zMax: int
    end

type ClimateData =
    struct
        val mutable clouds: float32
        val mutable moisture: float32
    end

type HexMapGeneratorFS() as this =
    inherit Node3D()

    [<DefaultValue>]
    val mutable grid: HexGridFS

    member val jitterProbability = 0.25f with get, set
    member val chunkSizeMin = 30 with get, set
    member val chunkSizeMax = 100 with get, set
    member val landPercentage = 50 with get, set
    member val waterLevel = 3 with get, set
    member val highRiseProbability = 0.25f with get, set
    member val sinkProbability = 0.2f with get, set
    member val elevationMinimum = -2 with get, set
    member val elevationMaximum = 8 with get, set
    member val mapBorderX = 5 with get, set
    member val mapBorderZ = 5 with get, set
    member val regionBorder = 5 with get, set
    member val regionCount = 1 with get, set
    member val erosionPercentage = 50 with get, set
    member val evaporationFactor = 0.5f with get, set
    member val precipitationFactor = 0.25f with get, set
    member val runoffFactor = 0.25f with get, set
    member val seepageFactor = 0.125f with get, set
    member val windDirection: HexDirection = HexDirection.NW with get, set
    member val windStrength = 4f with get, set
    member val startingMoisture = 0.1f with get, set
    member val useFixedSeed = false with get, set
    member val seed = 0 with get, set
    let mutable cellCount = 0
    let searchFrontier = HexCellPriorityQueue()
    let mutable searchFrontierPhase = 0
    let random = new RandomNumberGenerator()
    let regions = List<MapRegion>()
    let mutable climate = List<ClimateData>()
    let mutable nextClimate = List<ClimateData>()

    let getRandomCell (region: MapRegion) =
        this.grid.GetCell(
            random.RandiRange(region.xMin, region.xMax - 1),
            random.RandiRange(region.zMin, region.zMax - 1)
        )

    let raiseTerrain chunkSize budget region =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCell = getRandomCell region
        firstCell.SearchPhase <- searchFrontierPhase
        firstCell.Distance <- 0
        firstCell.SearchHeuristic <- 0
        searchFrontier.Enqueue firstCell
        let center = firstCell.Coordinates
        let rise = if random.Randf() < this.highRiseProbability then 2 else 1
        let mutable size = 0
        let mutable budget = budget

        while budget > 0 && size < chunkSize && searchFrontier.Count > 0 do
            searchFrontier.Dequeue()
            |> Option.iter (fun current ->
                let originalElevation = current.Elevation
                let newElevation = originalElevation + rise

                if newElevation > this.elevationMaximum then
                    ()
                else
                    current.Elevation <- newElevation

                    let breakLoop =
                        if originalElevation < this.waterLevel && newElevation >= this.waterLevel then
                            budget <- budget - 1
                            budget = 0
                        else
                            false

                    if not breakLoop then
                        size <- size + 1

                        for d in allHexDirs () do
                            match current.GetNeighbor d with
                            | Some neighbor when neighbor.SearchPhase < searchFrontierPhase ->
                                neighbor.SearchPhase <- searchFrontierPhase
                                neighbor.Distance <- neighbor.Coordinates.DistanceTo center
                                neighbor.SearchHeuristic <- if random.Randf() < this.jitterProbability then 1 else 0
                                searchFrontier.Enqueue neighbor
                            | _ -> ())

        searchFrontier.Clear()
        budget


    let sinkTerrain chunkSize budget region =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCell = getRandomCell region
        firstCell.SearchPhase <- searchFrontierPhase
        firstCell.Distance <- 0
        firstCell.SearchHeuristic <- 0
        searchFrontier.Enqueue firstCell
        let center = firstCell.Coordinates
        let sink = if random.Randf() < this.highRiseProbability then 2 else 1
        let mutable size = 0
        let mutable budget = budget

        while size < chunkSize && searchFrontier.Count > 0 do
            searchFrontier.Dequeue()
            |> Option.iter (fun current ->
                let originalElevation = current.Elevation
                let newElevation = current.Elevation - sink

                if newElevation < this.elevationMinimum then
                    ()
                else
                    current.Elevation <- newElevation

                    if originalElevation >= this.waterLevel && newElevation < this.waterLevel then
                        budget <- budget + 1

                    size <- size + 1

                    for d in allHexDirs () do
                        match current.GetNeighbor d with
                        | Some neighbor when neighbor.SearchPhase < searchFrontierPhase ->
                            neighbor.SearchPhase <- searchFrontierPhase
                            neighbor.Distance <- neighbor.Coordinates.DistanceTo center
                            neighbor.SearchHeuristic <- if random.Randf() < this.jitterProbability then 1 else 0
                            searchFrontier.Enqueue neighbor
                        | _ -> ())

        searchFrontier.Clear()
        budget

    let createLand () =
        let mutable landBudget =
            Mathf.RoundToInt(float32 (cellCount * this.landPercentage) * 0.01f)
        // 防止无限循环的守卫值
        let mutable guard = 0

        while landBudget > 0 && guard < 10000 do
            let sink = random.Randf() < this.sinkProbability

            match
                regions
                |> Seq.tryFind (fun region ->
                    let chunkSize = random.RandiRange(this.chunkSizeMin, this.chunkSizeMax)

                    if sink then
                        landBudget <- sinkTerrain chunkSize landBudget region
                    else
                        landBudget <- raiseTerrain chunkSize landBudget region

                    landBudget = 0)
            with
            | Some _ -> () // 已经耗尽预算
            | None -> guard <- guard + 1

        if landBudget > 0 then
            GD.PrintErr $"Failed to use up {landBudget} land budget."

    let setTerrainType () =
        for i in 0 .. cellCount - 1 do
            let cell = this.grid.GetCell(i)
            let moisture = climate[i].moisture

            if not cell.IsUnderWater then
                cell.TerrainTypeIndex <-
                    match moisture with
                    | x when x < 0.05f -> 4
                    | x when x < 0.12f -> 0
                    | x when x < 0.28f -> 3
                    | x when x < 0.85f -> 1
                    | _ -> 2
            else
                cell.TerrainTypeIndex <- 2

            cell.SetMapData <| moisture

    let createRegions () =
        regions.Clear()
        let mutable region = MapRegion()

        match this.regionCount with
        | 1 ->
            region.xMin <- this.mapBorderX
            region.xMax <- this.grid.cellCountX - this.mapBorderX
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
        | 2 ->
            let rand = random.Randf()

            if rand < 0.5f then
                GD.Print $"Split map vertically {rand}"
                region.xMin <- this.mapBorderX
                region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
                region.zMin <- this.mapBorderZ
                region.zMax <- this.grid.cellCountZ - this.mapBorderZ
                regions.Add region
                region.xMin <- this.grid.cellCountX / 2 + this.regionBorder
                region.xMax <- this.grid.cellCountX - this.mapBorderX
                regions.Add region
            else
                GD.Print $"Split map horizontally {rand}"
                region.xMin <- this.mapBorderX
                region.xMax <- this.grid.cellCountX - this.mapBorderX
                region.zMin <- this.mapBorderZ
                region.zMax <- this.grid.cellCountZ / 2 - this.regionBorder
                regions.Add region
                region.zMin <- this.grid.cellCountZ / 2 + this.regionBorder
                region.zMax <- this.grid.cellCountZ - this.mapBorderZ
                regions.Add region
        | 3 ->
            region.xMin <- this.mapBorderX
            region.xMax <- this.grid.cellCountX / 3 - this.regionBorder
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
            region.xMin <- this.grid.cellCountX / 3 + this.regionBorder
            region.xMax <- this.grid.cellCountX * 2 / 3 - this.regionBorder
            regions.Add region
            region.xMin <- this.grid.cellCountX * 2 / 3 + this.regionBorder
            region.xMax <- this.grid.cellCountX - this.mapBorderX
            regions.Add region
        | _ ->
            region.xMin <- this.mapBorderX
            region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ / 2 - this.regionBorder
            regions.Add region
            region.xMin <- this.grid.cellCountX / 2 + this.regionBorder
            region.xMax <- this.grid.cellCountX - this.mapBorderX
            regions.Add region
            region.zMin <- this.grid.cellCountZ / 2 + this.regionBorder
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
            region.xMin <- this.mapBorderX
            region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
            regions.Add region

    let isErodible (cell: HexCellFS) =
        let erodibleElevation = cell.Elevation - 2

        allHexDirs ()
        |> List.tryFind (fun d ->
            let neighborOpt = cell.GetNeighbor d
            neighborOpt.IsSome && neighborOpt.Value.Elevation <= erodibleElevation)
        |> Option.isSome

    let getErosionTarget (cell: HexCellFS) =
        let candidates = List<HexCellFS>()
        let erodibleElevation = cell.Elevation - 2

        for d in allHexDirs () do
            let neighborOpt = cell.GetNeighbor d

            if neighborOpt.IsSome && neighborOpt.Value.Elevation <= erodibleElevation then
                candidates.Add neighborOpt.Value

        candidates[random.RandiRange(0, candidates.Count - 1)]

    let erodeLand () =
        let erodibleCells = List<HexCellFS>()

        for i in 0 .. cellCount - 1 do
            let cell = this.grid.GetCell(i)

            if isErodible cell then
                erodibleCells.Add cell

        GD.Print $"Eroding {erodibleCells.Count} cells."

        let targetErodibleCount =
            int <| float32 (erodibleCells.Count * (100 - this.erosionPercentage)) * 0.01f

        while erodibleCells.Count > targetErodibleCount do
            let index = random.RandiRange(0, erodibleCells.Count - 1)
            let cell = erodibleCells[index]
            let targetCell = getErosionTarget cell
            cell.Elevation <- cell.Elevation - 1
            targetCell.Elevation <- targetCell.Elevation + 1

            if not <| isErodible cell then
                erodibleCells[index] <- erodibleCells[erodibleCells.Count - 1]
                erodibleCells.RemoveAt <| erodibleCells.Count - 1

            for d in allHexDirs () do
                match cell.GetNeighbor d with
                | Some neighbor when
                    neighbor.Elevation = cell.Elevation + 2
                    && not <| erodibleCells.Contains neighbor
                    ->
                    erodibleCells.Add neighbor
                | _ -> ()

            if isErodible targetCell && not <| erodibleCells.Contains targetCell then
                erodibleCells.Add targetCell

            for d in allHexDirs () do
                match targetCell.GetNeighbor d with
                | Some neighbor when
                    neighbor <> cell
                    && neighbor.Elevation = targetCell.Elevation + 1
                    && not <| isErodible neighbor
                    ->
                    erodibleCells.Remove neighbor |> ignore
                | _ -> ()

    let evolveClimate (cellIndex: int) =
        let cell = this.grid.GetCell cellIndex
        let mutable cellClimate = climate[cellIndex]

        if cell.IsUnderWater then
            cellClimate.moisture <- 1f
            cellClimate.clouds <- cellClimate.clouds + this.evaporationFactor
        else
            let evaporation = cellClimate.moisture * this.evaporationFactor
            cellClimate.moisture <- cellClimate.moisture - evaporation
            cellClimate.clouds <- cellClimate.clouds + evaporation

        let precipitation = cellClimate.clouds * this.precipitationFactor
        cellClimate.clouds <- cellClimate.clouds - precipitation
        cellClimate.moisture <- cellClimate.moisture + precipitation

        let cloudMaximum =
            1f - float32 cell.ViewElevation / (float32 this.elevationMaximum + 1f)

        if cellClimate.clouds > cloudMaximum then
            cellClimate.moisture <- cellClimate.moisture + cellClimate.clouds - cloudMaximum
            cellClimate.clouds <- cloudMaximum

        let mainDispersalDirection = this.windDirection.Opposite()
        let cloudDispersal = cellClimate.clouds * (1f / (5f + this.windStrength))
        let runoff = cellClimate.moisture * this.runoffFactor * (1f / 6f)
        let seepage = cellClimate.moisture * this.seepageFactor * (1f / 6f)

        for d in allHexDirs () do
            match cell.GetNeighbor d with
            | Some neighbor ->
                let mutable neighborClimate = nextClimate[neighbor.Index]

                if d = mainDispersalDirection then
                    neighborClimate.clouds <- neighborClimate.clouds + cloudDispersal * this.windStrength
                else
                    neighborClimate.clouds <- neighborClimate.clouds + cloudDispersal

                let elevationDelta = neighbor.ViewElevation - cell.ViewElevation

                if elevationDelta < 0 then
                    cellClimate.moisture <- cellClimate.moisture - runoff
                    neighborClimate.moisture <- neighborClimate.moisture + runoff
                elif elevationDelta = 0 then
                    cellClimate.moisture <- cellClimate.moisture - seepage
                    neighborClimate.moisture <- neighborClimate.moisture + seepage

                nextClimate[neighbor.Index] <- neighborClimate
            | _ -> ()

        let mutable nextCellClimate = nextClimate[cellIndex]
        nextCellClimate.moisture <- nextCellClimate.moisture + cellClimate.moisture

        if nextCellClimate.moisture > 1f then
            nextCellClimate.moisture <- 1f

        nextClimate[cellIndex] <- nextCellClimate
        climate[cellIndex] <- ClimateData()

    let createClimate () =
        climate.Clear()
        nextClimate.Clear()

        let mutable initialData = ClimateData()
        initialData.moisture <- this.startingMoisture
        let clearData = ClimateData()

        for i in 0 .. cellCount - 1 do
            climate.Add <| initialData
            nextClimate.Add <| clearData

        for cycle in 1..40 do
            for i in 0 .. cellCount - 1 do
                evolveClimate i

            let swap = climate
            climate <- nextClimate
            nextClimate <- swap

    member this.GenerateMap x z =
        let initState = random.State

        if not this.useFixedSeed then
            random.Randomize()

            this.seed <-
                random.RandiRange(0, Int32.MaxValue)
                ^^^ int DateTime.Now.Ticks
                ^^^ int (Time.GetTicksUsec())
                &&& Int32.MaxValue

        GD.Print $"Generating map with seed {this.seed}"
        random.Seed <- uint64 this.seed
        cellCount <- x * z
        this.grid.CreateMap x z |> ignore

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).WaterLevel <- this.waterLevel

        createRegions ()
        createLand ()
        erodeLand ()
        createClimate ()
        setTerrainType ()

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).SearchPhase <- 0

        random.State <- initState
