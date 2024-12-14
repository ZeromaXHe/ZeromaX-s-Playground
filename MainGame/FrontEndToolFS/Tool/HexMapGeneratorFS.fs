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

type HemisphereMode =
    | Both = 0
    | North = 1
    | South = 2

type Biome =
    struct
        val mutable terrain: int
        val mutable plant: int
        public new(terrain, plant) = { terrain = terrain; plant = plant }
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
    member val riverPercentage = 10f with get, set
    member val extraLakeProbability = 0.25f with get, set
    member val lowTemperature = 0f with get, set
    member val highTemperature = 1f with get, set
    member val hemisphere: HemisphereMode = HemisphereMode.Both with get, set
    member val temperatureJitter = 0.1f with get, set
    member val useFixedSeed = false with get, set
    member val seed = 0 with get, set
    let mutable cellCount = 0
    let mutable landCells = 0
    let searchFrontier = HexCellPriorityQueue()
    let mutable searchFrontierPhase = 0
    let random = new RandomNumberGenerator()
    let regions = List<MapRegion>()
    let mutable climate = List<ClimateData>()
    let mutable nextClimate = List<ClimateData>()
    let mutable temperatureJitterChannel = 0
    let temperatureBands = [| 0.1f; 0.3f; 0.6f |]
    let moistureBands = [| 0.12f; 0.28f; 0.85f |]

    let biomes =
        [| Biome(0, 0)
           Biome(4, 0)
           Biome(4, 0)
           Biome(4, 0)
           //
           Biome(0, 0)
           Biome(2, 0)
           Biome(2, 1)
           Biome(2, 2)
           //
           Biome(0, 0)
           Biome(1, 0)
           Biome(1, 1)
           Biome(1, 2)
           //
           Biome(0, 0)
           Biome(1, 1)
           Biome(1, 2)
           Biome(1, 3) |]

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

        landCells <- landBudget
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
            landCells <- landCells - landBudget
            GD.PrintErr $"Failed to use up {landBudget} land budget."

    let determineTemperature (cell: HexCellFS) =
        let mutable latitude = float32 cell.Coordinates.Z / float32 this.grid.cellCountZ

        if this.hemisphere = HemisphereMode.Both then
            latitude <- latitude * 2f

            if latitude > 1f then
                latitude <- 2f - latitude
        elif this.hemisphere = HemisphereMode.North then
            latitude <- 1f - latitude

        let mutable temperature =
            Mathf.Lerp(this.lowTemperature, this.highTemperature, latitude)

        temperature <-
            temperature
            * (1f
               - float32 (cell.ViewElevation - this.waterLevel)
                 / (float32 this.elevationMaximum - float32 this.waterLevel + 1f))

        let jitter = HexMetrics.sampleNoise(cell.Position * 0.1f)[temperatureJitterChannel]
        temperature + (jitter * 2f - 1f) * this.temperatureJitter

    let setTerrainType () =
        temperatureJitterChannel <- random.RandiRange(0, 3)

        let rockDesertElevation =
            this.elevationMaximum - (this.elevationMaximum - this.waterLevel) / 2

        for i in 0 .. cellCount - 1 do
            let cell = this.grid.GetCell(i)
            let temperature = determineTemperature cell
            // 显示温度
            // cell.SetMapData temperature
            let moisture = climate[i].moisture
            // 显示湿度
            // cell.SetMapData moisture

            if not cell.IsUnderWater then
                let t =
                    temperatureBands
                    |> Array.tryFindIndex (fun t -> temperature < t)
                    |> Option.defaultValue temperatureBands.Length

                let m =
                    moistureBands
                    |> Array.tryFindIndex (fun m -> moisture < m)
                    |> Option.defaultValue moistureBands.Length

                let mutable cellBiome = biomes[t * 4 + m]

                if cellBiome.terrain = 0 then
                    if cell.Elevation >= rockDesertElevation then
                        // 假设如果一个单元格的高度比水位更接近最高高度，沙子就会变成岩石。这是岩石沙漠高程线
                        cellBiome.terrain <- 3
                elif cell.Elevation = this.elevationMaximum then
                    // 强制处于最高海拔的单元格变成雪盖，无论它们有多暖和，只要它们不太干燥
                    cellBiome.terrain <- 4

                if cellBiome.terrain = 4 then
                    // 确保植物不会出现在雪地上
                    cellBiome.plant <- 0
                elif cellBiome.plant < 3 && cell.HasRiver then
                    // 如果等级还没有达到最高点，让我们也增加河流沿岸的植物等级
                    cellBiome.plant <- cellBiome.plant + 1

                cell.TerrainTypeIndex <- cellBiome.terrain
                cell.PlantLevel <- cellBiome.plant
            else
                let terrain =
                    match cell.Elevation with
                    | e when e = this.waterLevel - 1 ->
                        let cliffs, slopes =
                            allHexDirs ()
                            |> List.fold
                                (fun (c, s) d ->
                                    match cell.GetNeighbor d with
                                    | Some neighbor ->
                                        let delta = neighbor.Elevation - cell.WaterLevel

                                        if delta = 0 then c, s + 1
                                        elif delta > 0 then c + 1, s
                                        else c, s
                                    | _ -> c, s)
                                (0, 0)

                        if cliffs + slopes > 3 then 1
                        elif cliffs > 0 then 3
                        elif slopes > 0 then 0
                        else 1
                    | e when e >= this.waterLevel -> 1 // 用草来建造比水位更高的单元格，这些是由河流形成的湖泊
                    | e when e < 0 -> 3 // 负海拔的单元格位于深处，让我们用岩石来做
                    | _ -> 2

                let terrain =
                    if terrain = 1 && temperature < temperatureBands[0] then
                        // 确保在最冷的温度带内不会出现绿色的水下单元格。用泥代替这些单元格
                        2
                    else
                        terrain

                cell.TerrainTypeIndex <- terrain

            let riverOriginData =
                match
                    moisture * float32 (cell.Elevation - this.waterLevel)
                    / float32 (this.elevationMaximum - this.waterLevel)
                with
                | d when d > 0.75f -> 1f
                | d when d > 0.5f -> 0.5f
                | d when d > 0.25f -> 0.25f
                | _ -> 0f
            // 显示河流源头判定
            cell.SetMapData <| riverOriginData

    let flowDirections = List<HexDirection>()

    let createRiverAt (origin: HexCellFS) =
        let mutable length = 1
        let mutable cell = origin
        let mutable direction = HexDirection.NE
        let mutable directReturn = Int32.MinValue

        while not cell.IsUnderWater && directReturn = Int32.MinValue do
            let mutable minNeighborElevation = Int32.MaxValue
            flowDirections.Clear()

            for d in allHexDirs () do
                if directReturn = Int32.MinValue then
                    match cell.GetNeighbor d with
                    | Some neighbor ->
                        if neighbor.Elevation < minNeighborElevation then
                            minNeighborElevation <- neighbor.Elevation

                        if neighbor = origin || neighbor.IncomingRiver.IsSome then
                            ()
                        else
                            let delta = neighbor.Elevation - cell.Elevation

                            if delta > 0 then
                                ()
                            elif neighbor.OutgoingRiver.IsSome then
                                cell.SetOutgoingRiver d
                                directReturn <- length
                            else
                                if delta < 0 then
                                    flowDirections.Add d
                                    flowDirections.Add d
                                    flowDirections.Add d

                                if length = 1 || (d <> direction.Next2() && d <> direction.Previous2()) then
                                    flowDirections.Add d

                                flowDirections.Add d
                    | _ -> ()

            if directReturn <> Int32.MinValue then
                ()
            elif flowDirections.Count = 0 then
                if length = 1 then
                    directReturn <- 0
                else
                    if minNeighborElevation >= cell.Elevation then
                        cell.WaterLevel <- minNeighborElevation

                        if minNeighborElevation = cell.Elevation then
                            cell.Elevation <- minNeighborElevation - 1

                    directReturn <- length
            else
                direction <- flowDirections[random.RandiRange(0, flowDirections.Count - 1)]
                cell.SetOutgoingRiver direction
                length <- length + 1

                if
                    minNeighborElevation >= cell.Elevation
                    && random.Randf() < this.extraLakeProbability
                then
                    cell.WaterLevel <- cell.Elevation
                    cell.Elevation <- cell.Elevation - 1

                cell <- (cell.GetNeighbor direction).Value

        if directReturn <> Int32.MinValue then
            directReturn
        else
            length

    let createRiver () =
        let riverOrigins = List<HexCellFS>()

        for i in 0 .. cellCount - 1 do
            let cell = this.grid.GetCell i

            if cell.IsUnderWater then
                ()
            else
                let data = climate[i]

                let weight =
                    data.moisture * float32 (cell.Elevation - this.waterLevel)
                    / float32 (this.elevationMaximum - this.waterLevel)

                if weight > 0.75f then
                    riverOrigins.Add cell
                    riverOrigins.Add cell

                if weight > 0.5f then
                    riverOrigins.Add cell

                if weight > 0.25f then
                    riverOrigins.Add cell

        let mutable riverBudget =
            Mathf.RoundToInt(float32 landCells * this.riverPercentage * 0.01f)

        GD.Print $"{riverOrigins.Count} river origins with river budget {riverBudget}"

        while riverBudget > 0 && riverOrigins.Count > 0 do
            let lastIndex = riverOrigins.Count - 1
            let index = random.RandiRange(0, lastIndex)
            let origin = riverOrigins[index]
            riverOrigins[index] <- riverOrigins[lastIndex]
            riverOrigins.RemoveAt lastIndex

            if not origin.HasRiver then
                let isValidOrigin =
                    allHexDirs ()
                    |> List.exists (fun d ->
                        match origin.GetNeighbor d with
                        | Some neighbor when neighbor.HasRiver || neighbor.IsUnderWater -> true
                        | _ -> false)
                    |> not

                if isValidOrigin then
                    riverBudget <- riverBudget - createRiverAt origin

        if riverBudget > 0 then
            GD.PrintErr $"Failed to use up river budget {riverBudget}"

    let createRegions () =
        regions.Clear()

        let mutable borderX =
            if this.grid.wrapping then
                this.regionBorder
            else
                this.mapBorderX

        let mutable region = MapRegion()

        match this.regionCount with
        | 1 ->
            if this.grid.wrapping then
                borderX <- 0

            region.xMin <- borderX
            region.xMax <- this.grid.cellCountX - borderX
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
        | 2 ->
            let rand = random.Randf()

            if rand < 0.5f then
                GD.Print $"Split map vertically {rand}"
                region.xMin <- borderX
                region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
                region.zMin <- this.mapBorderZ
                region.zMax <- this.grid.cellCountZ - this.mapBorderZ
                regions.Add region
                region.xMin <- this.grid.cellCountX / 2 + this.regionBorder
                region.xMax <- this.grid.cellCountX - borderX
                regions.Add region
            else
                GD.Print $"Split map horizontally {rand}"

                if this.grid.wrapping then
                    borderX <- 0

                region.xMin <- borderX
                region.xMax <- this.grid.cellCountX - borderX
                region.zMin <- this.mapBorderZ
                region.zMax <- this.grid.cellCountZ / 2 - this.regionBorder
                regions.Add region
                region.zMin <- this.grid.cellCountZ / 2 + this.regionBorder
                region.zMax <- this.grid.cellCountZ - this.mapBorderZ
                regions.Add region
        | 3 ->
            region.xMin <- borderX
            region.xMax <- this.grid.cellCountX / 3 - this.regionBorder
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
            region.xMin <- this.grid.cellCountX / 3 + this.regionBorder
            region.xMax <- this.grid.cellCountX * 2 / 3 - this.regionBorder
            regions.Add region
            region.xMin <- this.grid.cellCountX * 2 / 3 + this.regionBorder
            region.xMax <- this.grid.cellCountX - borderX
            regions.Add region
        | _ ->
            region.xMin <- borderX
            region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
            region.zMin <- this.mapBorderZ
            region.zMax <- this.grid.cellCountZ / 2 - this.regionBorder
            regions.Add region
            region.xMin <- this.grid.cellCountX / 2 + this.regionBorder
            region.xMax <- this.grid.cellCountX - borderX
            regions.Add region
            region.zMin <- this.grid.cellCountZ / 2 + this.regionBorder
            region.zMax <- this.grid.cellCountZ - this.mapBorderZ
            regions.Add region
            region.xMin <- borderX
            region.xMax <- this.grid.cellCountX / 2 - this.regionBorder
            regions.Add region

    let isErodible (cell: HexCellFS) =
        let erodibleElevation = cell.Elevation - 2

        allHexDirs ()
        |> List.exists (fun d ->
            let neighborOpt = cell.GetNeighbor d
            neighborOpt.IsSome && neighborOpt.Value.Elevation <= erodibleElevation)

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

    member this.GenerateMap x z wrapping =
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
        this.grid.CreateMap x z wrapping |> ignore

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).WaterLevel <- this.waterLevel

        createRegions ()
        createLand ()
        erodeLand ()
        createClimate ()
        createRiver ()
        setTerrainType ()

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).SearchPhase <- 0

        random.State <- initState
