namespace FrontEndToolFS.Tool

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexFlags
open Godot

type HexCellPriorityQueue(grid: HexGridFS) =
    let list = List<int>()

    let mutable minimum = Int32.MaxValue

    member this.Enqueue cellIndex =
        let priority = grid.SearchData[cellIndex].SearchPriority

        if priority < minimum then
            minimum <- priority

        while priority >= list.Count do
            list.Add -1

        grid.SearchData[cellIndex].nextWithSamePriority <- list[priority]
        list[priority] <- cellIndex

    member this.Dequeue() =
        let mutable breakLoop = false
        let mutable result = -1

        while minimum < list.Count && not breakLoop do
            let cellIndex = list[minimum]

            if cellIndex >= 0 then
                result <- cellIndex
                list[minimum] <- grid.SearchData[cellIndex].nextWithSamePriority
                breakLoop <- true
            else
                minimum <- minimum + 1

        result

    member this.Change cellIndex oldPriority =
        let mutable current = list[oldPriority]
        let mutable next = grid.SearchData[current].nextWithSamePriority

        if current = cellIndex then
            list[oldPriority] <- next
        else
            while next <> cellIndex do
                current <- next
                next <- grid.SearchData[current].nextWithSamePriority

            grid.SearchData[current].nextWithSamePriority <- grid.SearchData[cellIndex].nextWithSamePriority

        this.Enqueue cellIndex

    member this.Clear() =
        list.Clear()
        minimum <- Int32.MaxValue

and HexGridFS() as this =
    inherit Node3D()

    interface IGridForUnit with
        // 感觉 Catlike Coding 这部分的逻辑写的一坨……
        override this.MakeChildOfColumn child columnIndex =
            this.MakeChildOfColumn child columnIndex

        override this.AddUnit prefab cellOpt orientation =
            cellOpt
            |> Option.iter (fun cell ->
                let unit = prefab.Instantiate<HexUnitFS>()
                this.AddUnit unit cell orientation)

        override this.GetCell(coordinates: HexCoordinates) = this.GetCell coordinates
        override this.GetCell(index: int) = this.GetCell index
        override this.IncreaseVisibility cell range = this.IncreaseVisibility cell range
        override this.DecreaseVisibility cell range = this.DecreaseVisibility cell range
        override this.GetPathShower = this.GetNode<Node3D> "PathShower"
        override this.PathShowerOn = this.PathShowerOn

    interface IGridForShader with
        override this.ResetVisibility() = this.ResetVisibility()
        override this.IsCellVisible cellIndex = this.IsCellVisible cellIndex
        override this.CellData = this.CellData

    interface IGridForCell with
        override this.GetCell coords = this.GetCell coords
        override this.RefreshCell cellIndex = this.RefreshCell cellIndex
        override this.RefreshCellPosition cellIndex = this.RefreshCellPosition cellIndex

        override this.RefreshCellWithDependents cellIndex =
            this.RefreshCellWithDependents cellIndex

        override this.ShaderData = this.cellShaderData
        override this.CellData = this.CellData
        override this.CellPositions = this.CellPositions

        override this.SetCellUnits index unit =
            this.CellUnits[index] <- unit |> Option.map (fun u -> u :?> HexUnitFS)

        override this.GetCellUnits index =
            this.CellUnits[index] |> Option.map (fun u -> u :> IUnit)

    interface IGridForChunk with
        override this.GetCellIndex coords = this.GetCellIndex coords
        override this.CellData = this.CellData
        override this.CellPositions = this.CellPositions

    [<DefaultValue>]
    val mutable cellLabelPrefab: PackedScene

    [<DefaultValue>]
    val mutable chunkPrefab: PackedScene

    let mutable pathShowerOn = false

    member this.PathShowerOn
        with get () = pathShowerOn
        and set value =
            pathShowerOn <- value

            if not value then
                (this.GetNode<Node3D> "PathShower").GetChildren() |> Seq.iter _.QueueFree()

    let mutable searchData: HexCellSearchData array = null
    member this.SearchData: HexCellSearchData array = searchData
    let mutable cellVisibility: int array = null
    member this.IsCellVisible cellIndex = cellVisibility[cellIndex] > 0
    let mutable chunkCountX = 4
    let mutable chunkCountZ = 3

    let mutable cellData: HexCellData array = null
    member this.CellData = cellData
    let mutable cellPositions: Vector3 array = null
    member this.CellPositions = cellPositions
    let mutable cellUnits: HexUnitFS option array = null
    member this.CellUnits = cellUnits
    let mutable cellGridChunks: HexGridChunkFS array = null
    let mutable cellUiRects: HexCellLabelFS array = null
    let mutable _chunks: HexGridChunkFS array = null
    let units = List<HexUnitFS>()
    let mutable columns: Node3D array = null
    let mutable currentCenterColumnIndex = -1

    let createChunks (shaderData: HexCellShaderData) =
        columns <-
            Array.init chunkCountX (fun i ->
                let column = new Node3D()
                column.Name <- $"Column{i}"
                this.AddChild column
                column)

        _chunks <-
            Array.init (chunkCountX * chunkCountZ) (fun i ->
                let chunk = this.chunkPrefab.Instantiate<HexGridChunkFS>()
                let x = i % chunkCountX
                let z = i / chunkCountX
                chunk.Name <- $"Chunk{x}_{z}"
                columns[x].AddChild chunk
                chunk.Grid <- this
                chunk)

    let createCells () =
        cellData <- Array.init (this.cellCountX * this.cellCountZ) (fun _ -> HexCellData())
        cellPositions <- Array.zeroCreate cellData.Length
        cellUiRects <- Array.zeroCreate cellData.Length
        cellGridChunks <- Array.zeroCreate cellData.Length
        cellUnits <- Array.zeroCreate cellData.Length
        searchData <- Array.init cellData.Length (fun _ -> HexCellSearchData())
        cellVisibility <- Array.zeroCreate cellData.Length

        for i in 0 .. cellData.Length - 1 do
            let z = i / this.cellCountX
            let x = i % this.cellCountX

            let position =
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerDiameter,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )

            let mutable cell = HexCellFS(i, this)
            cellPositions[i] <- position
            cellData[i].coordinates <- HexCoordinates.FromOffsetCoordinates x z

            let explorable =
                if this.wrapping then
                    z > 0 && z < this.cellCountZ - 1
                else
                    x > 0 && z > 0 && x < this.cellCountX - 1 && z < this.cellCountZ - 1

            cell.Flags <-
                if explorable then
                    cell.Flags.With HexFlags.Explorable
                else
                    cell.Flags.Without HexFlags.Explorable

            let label = this.cellLabelPrefab.Instantiate<HexCellLabelFS>()
            label.Position <- cell.Position + Vector3.Up * 0.01f
            cellUiRects[i] <- label
            cell.Values <- cell.Values.WithElevation 0
            this.RefreshCellPosition i
            let chunkX = x / HexMetrics.chunkSizeX
            let chunkZ = z / HexMetrics.chunkSizeZ
            let chunk = _chunks[chunkX + chunkZ * chunkCountX]
            let localX = x - chunkX * HexMetrics.chunkSizeX
            let localZ = z - chunkZ * HexMetrics.chunkSizeZ
            cellGridChunks[i] <- chunk
            chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX) i label

    let setLabel cellIndex text = cellUiRects[cellIndex].Text <- text

    let disableHighlight cellIndex =
        (cellUiRects[cellIndex].GetNode<Sprite3D> "Highlight").Visible <- false

    let enableHighlight cellIndex color =
        let highlight = cellUiRects[cellIndex].GetNode<Sprite3D> "Highlight"
        highlight.Modulate <- color
        highlight.Visible <- true

    member this.RefreshCellPosition cellIndex =
        let mutable position = cellPositions[cellIndex]
        position.Y <- float32 cellData[cellIndex].Elevation * HexMetrics.elevationStep

        position.Y <-
            position.Y
            + ((HexMetrics.sampleNoise position).Y * 2f - 1f)
              * HexMetrics.elevationPerturbStrength

        cellPositions[cellIndex] <- position
        let label = cellUiRects[cellIndex]
        label.Position <- position + Vector3.Up * 0.01f

    member this.RefreshCell cellIndex = cellGridChunks[cellIndex].Refresh()

    member this.RefreshCellWithDependents cellIndex =
        let chunk = cellGridChunks[cellIndex]
        chunk.Refresh()
        let coordinates = cellData[cellIndex].coordinates

        for d in HexDirection.allHexDirs () do
            match this.GetCellIndex <| coordinates.Step d with
            | neighborIndex when neighborIndex >= 0 ->
                let neighborChunk = cellGridChunks[neighborIndex]

                if neighborChunk <> chunk then
                    neighborChunk.Refresh()
            | _ -> ()

        let unit = cellUnits[cellIndex]
        unit |> Option.iter _.ValidateLocation()

    member this.RefreshAllCells() =
        for i in 0 .. cellData.Length - 1 do
            searchData[i].searchPhase <- 0
            this.RefreshCellPosition i
            this.cellShaderData.RefreshTerrain i
            this.cellShaderData.RefreshVisibility i

    let searchFrontier = HexCellPriorityQueue this
    let mutable searchFrontierPhase = 0
    let mutable currentPathFromIndex = -1
    let mutable currentPathToIndex = -1
    let mutable currentPathExists = false
    member this.HasPath = currentPathExists

    let search (fromCell: HexCellFS) (toCell: HexCellFS) (unit: HexUnitFS) =
        let speed = unit.Speed
        searchFrontierPhase <- 2 + searchFrontierPhase
        searchFrontier.Clear()
        searchData[fromCell.Index] <- HexCellSearchData(searchPhase = searchFrontierPhase)
        searchFrontier.Enqueue fromCell.Index

        let mutable breakLoop = false
        let mutable currentIndex = searchFrontier.Dequeue()

        while currentIndex >= 0 && not breakLoop do
            let current = HexCellFS(currentIndex, this)
            let currentDistance = searchData[currentIndex].distance
            searchData[currentIndex].searchPhase <- 1 + searchData[currentIndex].searchPhase

            if current = toCell then
                breakLoop <- true
            else
                let currentTurn = (currentDistance - 1) / speed

                for d in HexDirection.allHexDirs () do
                    match current.GetNeighbor d with
                    | Some neighbor ->
                        let neighborData = searchData[neighbor.Index]

                        if
                            neighborData.searchPhase > searchFrontierPhase
                            || not <| unit.IsValidDestination(neighbor)
                        then
                            ()
                        else
                            let moveCost = unit.GetMoveCost current neighbor d

                            if moveCost < 0 then
                                ()
                            else
                                let distance = currentDistance + moveCost
                                let turn = (distance - 1) / speed

                                let distance =
                                    if turn > currentTurn then
                                        turn * speed + moveCost
                                    else
                                        distance

                                if neighborData.searchPhase < searchFrontierPhase then
                                    searchData[neighbor.Index] <-
                                        HexCellSearchData(
                                            searchPhase = searchFrontierPhase,
                                            distance = distance,
                                            pathFrom = currentIndex,
                                            heuristic = neighbor.Coordinates.DistanceTo toCell.Coordinates
                                        )

                                    searchFrontier.Enqueue neighbor.Index
                                elif distance < neighborData.distance then
                                    searchData[neighbor.Index].distance <- distance
                                    searchData[neighbor.Index].pathFrom <- currentIndex
                                    searchFrontier.Change neighbor.Index neighborData.SearchPriority
                    | _ -> ()

            currentIndex <- if breakLoop then -1 else searchFrontier.Dequeue()

        breakLoop

    let getVisibleCells (fromCell: HexCellFS) range =
        let visibleCells = List<HexCellFS>()
        searchFrontierPhase <- 2 + searchFrontierPhase
        searchFrontier.Clear()
        let range = range + fromCell.Values.ViewElevation

        searchData[fromCell.Index] <-
            HexCellSearchData(searchPhase = searchFrontierPhase, pathFrom = searchData[fromCell.Index].pathFrom)

        searchFrontier.Enqueue fromCell.Index
        let fromCoordinates = fromCell.Coordinates
        let mutable currentIndex = searchFrontier.Dequeue()

        while currentIndex >= 0 do
            let current = HexCellFS(currentIndex, this)
            searchData[currentIndex].searchPhase <- 1 + searchData[currentIndex].searchPhase

            visibleCells.Add current

            for d in HexDirection.allHexDirs () do
                match current.GetNeighbor d with
                | Some neighbor ->
                    let neighborData = searchData[neighbor.Index]

                    if
                        neighborData.searchPhase > searchFrontierPhase
                        || neighbor.Flags.HasNone HexFlags.Explorable
                    then
                        ()
                    else
                        let distance = searchData[currentIndex].distance + 1

                        if
                            distance + neighbor.Values.ViewElevation > range
                            || distance > fromCoordinates.DistanceTo neighbor.Coordinates
                        then
                            ()
                        elif neighborData.searchPhase < searchFrontierPhase then
                            searchData[neighbor.Index] <-
                                HexCellSearchData(
                                    searchPhase = searchFrontierPhase,
                                    distance = distance,
                                    pathFrom = neighborData.pathFrom
                                )

                            searchFrontier.Enqueue neighbor.Index
                        elif distance < searchData[neighbor.Index].distance then
                            searchData[neighbor.Index].distance <- distance
                            searchFrontier.Change neighbor.Index neighborData.SearchPriority
                | _ -> ()

            currentIndex <- searchFrontier.Dequeue()

        visibleCells

    let showPath speed =
        if currentPathExists then
            let mutable currentIndex = currentPathToIndex

            while currentIndex <> currentPathFromIndex do
                let turn = (searchData[currentIndex].distance - 1) / speed
                setLabel currentIndex <| string turn
                enableHighlight currentIndex Colors.White
                currentIndex <- searchData[currentIndex].pathFrom

        enableHighlight currentPathFromIndex Colors.Blue
        enableHighlight currentPathToIndex Colors.Red

    let clearUnits () =
        units |> Seq.iter _.Die()
        units.Clear()

    member val cellCountX: int = 20 with get, set
    member val cellCountZ: int = 15 with get, set
    member val wrapping: bool = false with get, set
    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    member val _noiseSource: Texture2D = null with get, set
    member val seed = 1234 with get, set

    [<DefaultValue>]
    val mutable unitPrefab: PackedScene

    [<DefaultValue>]
    val mutable cellShaderData: HexCellShaderData

    let mutable dataReady = false

    member this.CameraRayCastToMouse() =
        let spaceState = this.GetWorld3D().DirectSpaceState
        let camera = this.GetViewport().GetCamera3D()
        let mousePos = this.GetViewport().GetMousePosition()

        let origin = camera.ProjectRayOrigin mousePos
        let endPoint = origin + camera.ProjectRayNormal mousePos * 1000.0f
        let query = PhysicsRayQueryParameters3D.Create(origin, endPoint)
        query.CollideWithAreas <- true

        spaceState.IntersectRay query

    member this.GetCellIndex(coordinates: HexCoordinates) =
        let z = coordinates.Z
        let x = coordinates.X + z / 2

        if z < 0 || z >= this.cellCountZ || x < 0 || x >= this.cellCountX then
            -1
        else
            x + z * this.cellCountX

    member this.GetCellIndex(xOffset, zOffset) = xOffset + zOffset * this.cellCountX

    member this.GetCell(coordinates: HexCoordinates) =
        let z = coordinates.Z
        let x = coordinates.X + z / 2

        if z < 0 || z >= this.cellCountZ || x < 0 || x >= this.cellCountX then
            None
        else
            Some <| HexCellFS(x + z * this.cellCountX, this)

    member this.GetCell cellIndex = HexCellFS(cellIndex, this)

    member this.GetCell(pos: Vector3) =
        let coordinates = HexCoordinates.FromPosition pos
        this.GetCell coordinates

    member this.Save(writer: BinaryWriter) =
        writer.Write this.cellCountX
        writer.Write this.cellCountZ
        writer.Write this.wrapping

        cellData
        |> Array.iter (fun c ->
            c.values.Save(writer)
            c.flags.Save(writer))

        writer.Write units.Count
        units |> Seq.iter _.Save(writer)

    member this.Load (reader: BinaryReader) header =
        this.ClearPath()
        clearUnits ()
        let x = if header >= 1 then reader.ReadInt32() else 20
        let z = if header >= 1 then reader.ReadInt32() else 15
        let wrapping = if header >= 5 then reader.ReadBoolean() else false

        if
            (x = this.cellCountX && z = this.cellCountZ && this.wrapping = wrapping)
            || this.CreateMap x z wrapping
        then
            let originalImmediateMode = this.cellShaderData.ImmediateMode
            this.cellShaderData.ImmediateMode <- true

            cellData
            |> Array.iteri (fun i c ->
                let mutable data = c
                data.values <- HexValues.Load reader header
                data.flags <- c.flags.Load reader header
                cellData[i] <- data
                this.RefreshCellPosition i
                this.cellShaderData.RefreshTerrain i
                this.cellShaderData.RefreshVisibility i)

            _chunks |> Array.iter _.Refresh()

            if header >= 2 then
                let unitCount = reader.ReadInt32()

                for i in 0 .. unitCount - 1 do
                    HexUnitFS.Load reader this

            this.cellShaderData.ImmediateMode <- originalImmediateMode

    member this.CreateMap x z wrapping =
        if
            x <= 0
            || x % HexMetrics.chunkSizeX <> 0
            || z <= 0
            || z % HexMetrics.chunkSizeZ <> 0
        then
            GD.PrintErr "Unsupported map size"
            false
        else
            this.ClearPath()
            clearUnits ()

            if columns <> null then
                columns |> Array.iter _.QueueFree()

            this.cellCountX <- x
            this.cellCountZ <- z
            this.wrapping <- wrapping
            currentCenterColumnIndex <- -1
            HexMetrics.wrapSize <- if this.wrapping then this.cellCountX else 0
            chunkCountX <- this.cellCountX / HexMetrics.chunkSizeX
            chunkCountZ <- this.cellCountZ / HexMetrics.chunkSizeZ
            this.cellShaderData.Initialize this.cellCountX this.cellCountZ
            createChunks this.cellShaderData
            createCells ()
            true

    member this.CenterMap xPosition =
        let centerColumnIndex =
            int (xPosition / (HexMetrics.innerDiameter * float32 HexMetrics.chunkSizeX))

        if centerColumnIndex = currentCenterColumnIndex then
            ()
        else
            currentCenterColumnIndex <- centerColumnIndex
            let minColumnIndex = centerColumnIndex - chunkCountX / 2
            let maxColumnIndex = centerColumnIndex + chunkCountX / 2
            let mutable position = Vector3.Zero

            columns
            |> Array.iteri (fun i c ->
                if i < minColumnIndex then
                    position.X <- float32 chunkCountX * (HexMetrics.innerDiameter * float32 HexMetrics.chunkSizeX)
                elif i > maxColumnIndex then
                    position.X <-
                        float32 chunkCountX
                        * -(HexMetrics.innerDiameter * float32 HexMetrics.chunkSizeX)
                else
                    position.X <- 0f

                c.Position <- position)

    member this.ShowUI visible =
        _chunks |> Array.iter (fun c -> c.ShowUI visible)

    member this.ShowGrid visible =
        // 同一个 Shader 的参数是共有的，改第一个 Chunk 就可以
        _chunks[0].ShowGrid visible

    member this.FindPath (fromCell: HexCellFS) (toCell: HexCellFS) (unit: HexUnitFS) =
        let sw = Stopwatch()
        sw.Start()
        this.ClearPath()
        currentPathFromIndex <- fromCell.Index
        currentPathToIndex <- toCell.Index
        currentPathExists <- search fromCell toCell unit
        showPath unit.Speed
        sw.Stop()
        // BUG: 现在一次点击会执行多次
        GD.Print $"FindPath search cost: {sw.ElapsedMilliseconds} ms"

    member this.ClearPath() =
        if currentPathExists then
            let mutable currentIndex = currentPathToIndex

            while currentIndex <> currentPathFromIndex do
                setLabel currentIndex ""
                disableHighlight currentIndex
                currentIndex <- searchData[currentIndex].pathFrom

            disableHighlight currentIndex
            currentPathExists <- false
        elif currentPathFromIndex >= 0 then
            disableHighlight currentPathFromIndex
            disableHighlight currentPathToIndex

        currentPathFromIndex <- -1
        currentPathToIndex <- -1

    member this.GetPath() =
        if currentPathExists then
            let path = List<int>()
            let mutable i = currentPathToIndex

            while i <> currentPathFromIndex do
                path.Add i
                i <- searchData[i].pathFrom

            path.Add currentPathFromIndex
            path.Reverse()
            path
        else
            null

    member this.MakeChildOfColumn (child: Node) columnIndex =
        let parent = child.GetParentOrNull()

        if parent <> null && parent <> columns[columnIndex] then
            child.Reparent columns[columnIndex]
        elif parent = null then
            columns[columnIndex].AddChild child

    member this.AddUnit (unit: HexUnitFS) (location: HexCellFS) orientation =
        units.Add unit
        unit.Grid <- this
        unit.Location <- location
        unit.Orientation <- orientation

    member this.RemoveUnit(unit: HexUnitFS) =
        units.Remove unit |> ignore
        unit.Die()

    member this.IncreaseVisibility (fromCell: HexCellFS) range =
        getVisibleCells fromCell range
        |> Seq.iter (fun c ->
            let cellIndex = c.Index
            cellVisibility[cellIndex] <- cellVisibility[cellIndex] + 1

            if cellVisibility[cellIndex] = 1 then
                let mutable c = c
                c.Flags <- c.Flags.With HexFlags.Explored
                this.cellShaderData.RefreshVisibility cellIndex)

    member this.DecreaseVisibility (fromCell: HexCellFS) range =
        getVisibleCells fromCell range
        |> Seq.iter (fun c ->
            let cellIndex = c.Index
            cellVisibility[cellIndex] <- cellVisibility[cellIndex] - 1

            if cellVisibility[cellIndex] = 0 then
                this.cellShaderData.RefreshVisibility cellIndex)

    member this.ResetVisibility() =
        { 0 .. cellVisibility.Length - 1 }
        |> Seq.filter (fun i -> cellVisibility[i] > 0)
        |> Seq.iter (fun i ->
            cellVisibility[i] <- 0
            this.cellShaderData.RefreshVisibility i)

        units |> Seq.iter (fun u -> this.IncreaseVisibility u.Location u.VisionRange)

    member this.GetRayCell() =
        let result = this.CameraRayCastToMouse()

        if result = null || result.Count = 0 then
            // GD.Print "rayCast empty result"
            None
        else
            let bool, res = result.TryGetValue "position"

            if bool then
                res.As<Vector3>() |> this.GetCell
            else
                // GD.Print "rayCast no position"
                None

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
        HexMetrics.noiseSource <- this._noiseSource.GetImage()
        HexMetrics.initializeHashGrid <| uint64 this.seed
        HexUnitFS.unitPrefab <- this.unitPrefab
        HexMetrics.wrapSize <- if this.wrapping then this.cellCountX else 0
        this.cellShaderData <- HexCellShaderData()
        this.cellShaderData.Grid <- this
        dataReady <- true
        this.CreateMap this.cellCountX this.cellCountZ this.wrapping |> ignore
        // 编辑器里显示随机颜色和随机高度的单元格
        if Engine.IsEditorHint() then
            let rand = Random()

            for i in 0 .. cellData.Length - 1 do
                let c = this.GetCell i
                c.SetTerrainTypeIndex <| rand.Next(0, 5)
                c.SetElevation <| rand.Next(0, 7)
                c.SetWaterLevel 3
                this.IncreaseVisibility c 1

                if c.Values.Elevation > 3 then
                    c.SetUrbanLevel <| rand.Next(0, 4)
                    c.SetFarmLevel <| rand.Next(0, 4)
                    c.SetPlantLevel <| rand.Next(0, 4)

                    if c.Values.UrbanLevel = 3 then
                        c.SetWalled true

                    c.SetOutgoingRiver(rand.Next(0, 6) |> enum<HexDirection>)
                    c.AddRoad << enum<HexDirection> <| rand.Next(0, 6)
                    c.AddRoad << enum<HexDirection> <| rand.Next(0, 6)

    override this._Process delta =
        // if this.IsNodeReady() then
        // 还真是奇了怪了，必须按下面 dataReady 这样写才行…… 不然 this.cellShaderData 是空引用
        if dataReady then
            this.cellShaderData.UpdateData delta
