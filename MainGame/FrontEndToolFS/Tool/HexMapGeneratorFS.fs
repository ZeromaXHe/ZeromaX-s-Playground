namespace FrontEndToolFS.Tool

open System
open System.Collections.Generic
open FrontEndToolFS.HexPlane.HexDirection
open FrontEndToolFS.HexPlane.HexFlags
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
    let searchFrontier = lazy HexCellPriorityQueue(this.grid) // lazy 时 grid 才非空
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

    let getRandomCellIndex (region: MapRegion) =
        this.grid.GetCellIndex(
            random.RandiRange(region.xMin, region.xMax - 1),
            random.RandiRange(region.zMin, region.zMax - 1)
        )

    let raiseTerrain chunkSize budget region =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCellIndex = getRandomCellIndex region
        this.grid.SearchData[firstCellIndex] <- HexCellSearchData(searchPhase = searchFrontierPhase)
        searchFrontier.Value.Enqueue firstCellIndex
        let center = this.grid.CellData[firstCellIndex].coordinates
        let rise = if random.Randf() < this.highRiseProbability then 2 else 1
        let mutable size = 0
        let mutable budget = budget

        let mutable index =
            if budget > 0 && size < chunkSize then
                searchFrontier.Value.Dequeue()
            else
                -1

        while budget > 0 && size < chunkSize && index >= 0 do
            let current = this.grid.CellData[index]
            let originalElevation = current.Elevation
            let newElevation = originalElevation + rise

            if newElevation > this.elevationMaximum then
                ()
            else
                this.grid.CellData[index].values <- current.values.WithElevation newElevation

                let breakLoop =
                    if originalElevation < this.waterLevel && newElevation >= this.waterLevel then
                        budget <- budget - 1
                        budget = 0
                    else
                        false

                if not breakLoop then
                    size <- size + 1

                    for d in allHexDirs () do
                        match this.grid.GetCellIndex <| current.coordinates.Step d with
                        | neighborIndex when
                            neighborIndex >= 0
                            && this.grid.SearchData[neighborIndex].searchPhase < searchFrontierPhase
                            ->
                            this.grid.SearchData[neighborIndex] <-
                                HexCellSearchData(
                                    searchPhase = searchFrontierPhase,
                                    distance = this.grid.CellData[neighborIndex].coordinates.DistanceTo center,
                                    heuristic = if random.Randf() < this.jitterProbability then 1 else 0
                                )

                            searchFrontier.Value.Enqueue neighborIndex
                        | _ -> ()

            index <-
                if budget > 0 && size < chunkSize then
                    searchFrontier.Value.Dequeue()
                else
                    -1

        searchFrontier.Value.Clear()
        budget


    let sinkTerrain chunkSize budget region =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCellIndex = getRandomCellIndex region
        this.grid.SearchData[firstCellIndex] <- HexCellSearchData(searchPhase = searchFrontierPhase)
        searchFrontier.Value.Enqueue firstCellIndex
        let center = this.grid.CellData[firstCellIndex].coordinates
        let sink = if random.Randf() < this.highRiseProbability then 2 else 1
        let mutable size = 0
        let mutable budget = budget
        let mutable index = if size < chunkSize then searchFrontier.Value.Dequeue() else -1

        while size < chunkSize && index >= 0 do
            let current = this.grid.CellData[index]
            let originalElevation = current.Elevation
            let newElevation = current.Elevation - sink

            if newElevation < this.elevationMinimum then
                ()
            else
                this.grid.CellData[index].values <- current.values.WithElevation newElevation

                if originalElevation >= this.waterLevel && newElevation < this.waterLevel then
                    budget <- budget + 1

                size <- size + 1

                for d in allHexDirs () do
                    match this.grid.GetCellIndex <| current.coordinates.Step d with
                    | neighborIndex when
                        neighborIndex >= 0
                        && this.grid.SearchData[neighborIndex].searchPhase < searchFrontierPhase
                        ->
                        this.grid.SearchData[neighborIndex] <-
                            HexCellSearchData(
                                searchPhase = searchFrontierPhase,
                                distance = this.grid.CellData[neighborIndex].coordinates.DistanceTo center,
                                heuristic = if random.Randf() < this.jitterProbability then 1 else 0
                            )

                        searchFrontier.Value.Enqueue neighborIndex
                    | _ -> ()

            index <- if size < chunkSize then searchFrontier.Value.Dequeue() else -1

        searchFrontier.Value.Clear()
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

    let determineTemperature cellIndex (cell: HexCellData) =
        let mutable latitude = float32 cell.coordinates.Z / float32 this.grid.cellCountZ

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

        let jitter =
            HexMetrics.sampleNoise(this.grid.CellPositions[cellIndex] * 0.1f)[temperatureJitterChannel]

        temperature + (jitter * 2f - 1f) * this.temperatureJitter

    let setTerrainType () =
        temperatureJitterChannel <- random.RandiRange(0, 3)

        let rockDesertElevation =
            this.elevationMaximum - (this.elevationMaximum - this.waterLevel) / 2

        for i in 0 .. cellCount - 1 do
            let cell = this.grid.CellData[i]
            let temperature = determineTemperature i cell
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

                this.grid.CellData[i].values <-
                    cell.values
                        .WithTerrainTypeIndex(cellBiome.terrain)
                        .WithPlantLevel(cellBiome.plant)
            else
                let terrain =
                    match cell.Elevation with
                    | e when e = this.waterLevel - 1 ->
                        let cliffs, slopes =
                            allHexDirs ()
                            |> List.fold
                                (fun (c, s) d ->
                                    match this.grid.GetCellIndex <| cell.coordinates.Step d with
                                    | neighborIndex when neighborIndex >= 0 ->
                                        let delta = this.grid.CellData[neighborIndex].Elevation - cell.WaterLevel

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

                this.grid.CellData[i].values <- cell.values.WithTerrainTypeIndex terrain

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
            // cell.SetMapData <| riverOriginData
            ()

    let flowDirections = List<HexDirection>()

    let createRiverAt originIndex =
        let mutable length = 1
        let mutable cellIndex = originIndex
        let mutable cell = this.grid.CellData[cellIndex]
        let mutable direction = HexDirection.NE
        let mutable directReturn = Int32.MinValue

        while not cell.IsUnderWater && directReturn = Int32.MinValue do
            let mutable minNeighborElevation = Int32.MaxValue
            flowDirections.Clear()

            for d in allHexDirs () do
                if directReturn = Int32.MinValue then
                    match this.grid.GetCellIndex <| cell.coordinates.Step d with
                    | neighborIndex when neighborIndex >= 0 ->
                        let neighbor = this.grid.CellData[neighborIndex]

                        if neighbor.Elevation < minNeighborElevation then
                            minNeighborElevation <- neighbor.Elevation

                        if neighborIndex = originIndex || neighbor.IncomingRiver.IsSome then
                            ()
                        else
                            let delta = neighbor.Elevation - cell.Elevation

                            if delta > 0 then
                                ()
                            elif neighbor.OutgoingRiver.IsSome then
                                this.grid.CellData[cellIndex].flags <- cell.flags.WithRiverOut d
                                this.grid.CellData[neighborIndex].flags <- neighbor.flags.WithRiverIn d.Opposite
                                directReturn <- length
                            else
                                if delta < 0 then
                                    flowDirections.Add d
                                    flowDirections.Add d
                                    flowDirections.Add d

                                if length = 1 || (d <> direction.Next2 && d <> direction.Previous2) then
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
                        cell.values <- cell.values.WithWaterLevel minNeighborElevation

                        if minNeighborElevation = cell.Elevation then
                            cell.values <- cell.values.WithElevation <| minNeighborElevation - 1

                    directReturn <- length
            else
                direction <- flowDirections[random.RandiRange(0, flowDirections.Count - 1)]
                cell.flags <- cell.flags.WithRiverOut direction
                let outIndex = this.grid.GetCellIndex <| cell.coordinates.Step direction
                this.grid.CellData[outIndex].flags <- this.grid.CellData[outIndex].flags.WithRiverIn direction.Opposite
                length <- length + 1

                if
                    minNeighborElevation >= cell.Elevation
                    && random.Randf() < this.extraLakeProbability
                then
                    cell.values <- cell.values.WithWaterLevel cell.Elevation
                    cell.values <- cell.values.WithElevation <| cell.Elevation - 1

                this.grid.CellData[cellIndex] <- cell
                cellIndex <- outIndex
                cell <- this.grid.CellData[cellIndex]

        if directReturn <> Int32.MinValue then
            directReturn
        else
            length

    let createRiver () =
        let riverOrigins = List<int>()

        for i in 0 .. cellCount - 1 do
            let cell = this.grid.CellData[i]

            if cell.IsUnderWater then
                ()
            else
                let data = climate[i]

                let weight =
                    data.moisture * float32 (cell.Elevation - this.waterLevel)
                    / float32 (this.elevationMaximum - this.waterLevel)

                if weight > 0.75f then
                    riverOrigins.Add i
                    riverOrigins.Add i

                if weight > 0.5f then
                    riverOrigins.Add i

                if weight > 0.25f then
                    riverOrigins.Add i

        let mutable riverBudget =
            Mathf.RoundToInt(float32 landCells * this.riverPercentage * 0.01f)

        GD.Print $"{riverOrigins.Count} river origins with river budget {riverBudget}"

        while riverBudget > 0 && riverOrigins.Count > 0 do
            let lastIndex = riverOrigins.Count - 1
            let index = random.RandiRange(0, lastIndex)
            let originIndex = riverOrigins[index]
            let origin = this.grid.CellData[originIndex]
            riverOrigins[index] <- riverOrigins[lastIndex]
            riverOrigins.RemoveAt lastIndex

            if not origin.HasRiver then
                let isValidOrigin =
                    allHexDirs ()
                    |> List.exists (fun d ->
                        match this.grid.GetCellIndex <| origin.coordinates.Step d with
                        | neighborIndex when
                            neighborIndex >= 0
                            && (this.grid.CellData[neighborIndex].HasRiver
                                || this.grid.CellData[neighborIndex].IsUnderWater)
                            ->
                            true
                        | _ -> false)
                    |> not

                if isValidOrigin then
                    riverBudget <- riverBudget - createRiverAt originIndex

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

    let isErodible cellIndex cellElevation =
        let erodibleElevation = cellElevation - 2
        let coordinates = this.grid.CellData[cellIndex].coordinates

        allHexDirs ()
        |> List.exists (fun d ->
            let neighborIndex = this.grid.GetCellIndex <| coordinates.Step d

            neighborIndex >= 0
            && this.grid.CellData[neighborIndex].Elevation <= erodibleElevation)

    let getErosionTarget cellIndex cellElevation =
        let candidates = List<int>()
        let erodibleElevation = cellElevation - 2
        let coordinates = this.grid.CellData[cellIndex].coordinates

        for d in allHexDirs () do
            let neighborIndex = this.grid.GetCellIndex <| coordinates.Step d

            if
                neighborIndex >= 0
                && this.grid.CellData[neighborIndex].Elevation <= erodibleElevation
            then
                candidates.Add neighborIndex

        candidates[random.RandiRange(0, candidates.Count - 1)]

    let erodeLand () =
        let erodibleIndices = List<int>()

        for i in 0 .. cellCount - 1 do
            if isErodible i this.grid.CellData[i].Elevation then
                erodibleIndices.Add i

        GD.Print $"Eroding {erodibleIndices.Count} cells."

        let targetErodibleCount =
            int <| float32 (erodibleIndices.Count * (100 - this.erosionPercentage)) * 0.01f

        while erodibleIndices.Count > targetErodibleCount do
            let index = random.RandiRange(0, erodibleIndices.Count - 1)
            let cellIndex = erodibleIndices[index]
            let mutable cell = this.grid.CellData[cellIndex]
            let targetCellIndex = getErosionTarget cellIndex cell.Elevation
            cell.values <- cell.values.WithElevation <| cell.Elevation - 1
            this.grid.CellData[cellIndex].values <- cell.values
            let mutable targetCell = this.grid.CellData[targetCellIndex]
            targetCell.values <- targetCell.values.WithElevation <| targetCell.Elevation + 1
            this.grid.CellData[targetCellIndex].values <- targetCell.values

            if not <| isErodible cellIndex cell.Elevation then
                erodibleIndices[index] <- erodibleIndices[erodibleIndices.Count - 1]
                erodibleIndices.RemoveAt <| erodibleIndices.Count - 1

            for d in allHexDirs () do
                match this.grid.GetCellIndex <| cell.coordinates.Step d with
                | neighborIndex when
                    neighborIndex >= 0
                    && this.grid.CellData[neighborIndex].Elevation = cell.Elevation + 2
                    && not <| erodibleIndices.Contains neighborIndex
                    ->
                    erodibleIndices.Add neighborIndex
                | _ -> ()

            if
                isErodible targetCellIndex targetCell.Elevation
                && not <| erodibleIndices.Contains targetCellIndex
            then
                erodibleIndices.Add targetCellIndex

            for d in allHexDirs () do
                match this.grid.GetCellIndex <| targetCell.coordinates.Step d with
                | neighborIndex when
                    neighborIndex >= 0
                    && neighborIndex <> cellIndex
                    && this.grid.CellData[neighborIndex].Elevation = targetCell.Elevation + 1
                    && not <| isErodible neighborIndex this.grid.CellData[neighborIndex].Elevation
                    ->
                    erodibleIndices.Remove neighborIndex |> ignore
                | _ -> ()

    let evolveClimate (cellIndex: int) =
        let cell = this.grid.CellData[cellIndex]
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

        let mainDispersalDirection = this.windDirection.Opposite
        let cloudDispersal = cellClimate.clouds * (1f / (5f + this.windStrength))
        let runoff = cellClimate.moisture * this.runoffFactor * (1f / 6f)
        let seepage = cellClimate.moisture * this.seepageFactor * (1f / 6f)

        for d in allHexDirs () do
            match this.grid.GetCellIndex <| cell.coordinates.Step d with
            | neighborIndex when neighborIndex >= 0 ->
                let mutable neighborClimate = nextClimate[neighborIndex]

                if d = mainDispersalDirection then
                    neighborClimate.clouds <- neighborClimate.clouds + cloudDispersal * this.windStrength
                else
                    neighborClimate.clouds <- neighborClimate.clouds + cloudDispersal

                let elevationDelta =
                    this.grid.CellData[neighborIndex].ViewElevation - cell.ViewElevation

                if elevationDelta < 0 then
                    cellClimate.moisture <- cellClimate.moisture - runoff
                    neighborClimate.moisture <- neighborClimate.moisture + runoff
                elif elevationDelta = 0 then
                    cellClimate.moisture <- cellClimate.moisture - seepage
                    neighborClimate.moisture <- neighborClimate.moisture + seepage

                nextClimate[neighborIndex] <- neighborClimate
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
            this.grid.CellData[i].values <- this.grid.CellData[i].values.WithWaterLevel this.waterLevel

        createRegions ()
        createLand ()
        erodeLand ()
        createClimate ()
        createRiver ()
        setTerrainType ()

        for i in 0 .. cellCount - 1 do
            this.grid.SearchData[i].searchPhase <- 0
            (this.grid.GetCell i).RefreshAll()

        random.State <- initState
