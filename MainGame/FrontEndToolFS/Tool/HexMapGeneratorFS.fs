namespace FrontEndToolFS.Tool

open System
open FrontEndToolFS.HexPlane
open Godot

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
    member val useFixedSeed = false with get, set
    member val seed = 0 with get, set
    let mutable cellCount = 0
    let searchFrontier = HexCellPriorityQueue()
    let mutable searchFrontierPhase = 0
    let random = new RandomNumberGenerator()

    let getRandomCell () =
        this.grid.GetCell(GD.RandRange(0, cellCount))

    let raiseTerrain chunkSize budget =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCell = getRandomCell ()
        firstCell.SearchPhase <- searchFrontierPhase
        firstCell.Distance <- 0
        firstCell.SearchHeuristic <- 0
        searchFrontier.Enqueue firstCell
        let center = firstCell.Coordinates
        let rise = if GD.Randf() < this.highRiseProbability then 2 else 1
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

                        for d in HexDirection.allHexDirs () do
                            match current.GetNeighbor d with
                            | Some neighbor when neighbor.SearchPhase < searchFrontierPhase ->
                                neighbor.SearchPhase <- searchFrontierPhase
                                neighbor.Distance <- neighbor.Coordinates.DistanceTo center
                                neighbor.SearchHeuristic <- if GD.Randf() < this.jitterProbability then 1 else 0
                                searchFrontier.Enqueue neighbor
                            | _ -> ())

        searchFrontier.Clear()
        budget


    let sinkTerrain chunkSize budget =
        searchFrontierPhase <- 1 + searchFrontierPhase
        let firstCell = getRandomCell ()
        firstCell.SearchPhase <- searchFrontierPhase
        firstCell.Distance <- 0
        firstCell.SearchHeuristic <- 0
        searchFrontier.Enqueue firstCell
        let center = firstCell.Coordinates
        let sink = if GD.Randf() < this.highRiseProbability then 2 else 1
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

                    for d in HexDirection.allHexDirs () do
                        match current.GetNeighbor d with
                        | Some neighbor when neighbor.SearchPhase < searchFrontierPhase ->
                            neighbor.SearchPhase <- searchFrontierPhase
                            neighbor.Distance <- neighbor.Coordinates.DistanceTo center
                            neighbor.SearchHeuristic <- if GD.Randf() < this.jitterProbability then 1 else 0
                            searchFrontier.Enqueue neighbor
                        | _ -> ())

        searchFrontier.Clear()
        budget

    let createLand () =
        let mutable landBudget =
            Mathf.RoundToInt(float32 (cellCount * this.landPercentage) * 0.01f)

        while landBudget > 0 do
            let chunkSize = GD.RandRange(this.chunkSizeMin, this.chunkSizeMax + 1)

            if GD.Randf() < this.sinkProbability then
                landBudget <- sinkTerrain chunkSize landBudget
            else
                landBudget <- raiseTerrain chunkSize landBudget

    let setTerrainType () =
        for i in 0 .. cellCount - 1 do
            let cell = this.grid.GetCell(i)

            if not cell.IsUnderWater then
                cell.TerrainTypeIndex <- cell.Elevation - cell.WaterLevel

    member this.GenerateMap x z =
        let initState = random.State

        if not this.useFixedSeed then
            this.seed <-
                random.RandiRange(0, Int32.MaxValue)
                ^^^ int DateTime.Now.Ticks
                ^^^ int (Time.GetTicksUsec())
                &&& Int32.MaxValue

        random.State <- uint64 this.seed
        cellCount <- x * z
        this.grid.CreateMap x z |> ignore

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).WaterLevel <- this.waterLevel

        createLand ()
        setTerrainType ()

        for i in 0 .. cellCount - 1 do
            this.grid.GetCell(i).SearchPhase <- 0

        random.State <- initState
