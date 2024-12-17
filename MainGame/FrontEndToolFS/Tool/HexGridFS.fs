namespace FrontEndToolFS.Tool

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open FrontEndToolFS.HexPlane
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
        override this.ShaderData = this.cellShaderData
        override this.CellData = this.CellData
        override this.CellPositions = this.CellPositions

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
    let mutable cellPositions: Vector3 array = null

    member this.CellData
        with get () = cellData
        and set value = cellData <- value

    member this.CellPositions
        with get () = cellPositions
        and set value = cellPositions <- value

    let mutable _cells: HexCellFS array = null
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

    let addCellToChunk x z (cell: HexCellFS) =
        let chunkX = x / HexMetrics.chunkSizeX
        let chunkZ = z / HexMetrics.chunkSizeZ
        let chunk = _chunks[chunkX + chunkZ * chunkCountX]
        let localX = x - chunkX * HexMetrics.chunkSizeX
        let localZ = z - chunkZ * HexMetrics.chunkSizeZ
        chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX) cell cell.Index cell.uiRect

    let createCells () =
        _cells <- Array.init (this.cellCountX * this.cellCountZ) (fun _ -> HexCellFS())
        cellData <- Array.init _cells.Length (fun _ -> HexCellData())
        cellPositions <- Array.zeroCreate _cells.Length
        searchData <- Array.init _cells.Length (fun _ -> HexCellSearchData())
        cellVisibility <- Array.zeroCreate _cells.Length

        for i in 0 .. _cells.Length - 1 do
            let z = i / this.cellCountX
            let x = i % this.cellCountX

            let position =
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerDiameter,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )

            let cell = _cells[i]
            cell.Grid <- this
            cellPositions[i] <- position
            cellData[i].coordinates <- HexCoordinates.FromOffsetCoordinates x z
            cell.Index <- i
            cell.ColumnIndex <- x / HexMetrics.chunkSizeX

            if this.wrapping then
                cell.Explorable <- z > 0 && z < this.cellCountZ - 1
                // 用于测试时区分每个块列的第一个单元格
                if x % HexMetrics.chunkSizeX = 0 then
                    cell.TerrainTypeIndex <- 1
            else
                cell.Explorable <- x > 0 && z > 0 && x < this.cellCountX - 1 && z < this.cellCountZ - 1

            let label = this.cellLabelPrefab.Instantiate<HexCellLabelFS>()
            label.Position <- cell.Position + Vector3.Up * 0.01f
            cell.uiRect <- label
            // 触发 setter 应用扰动 y
            cell.Elevation <- 0
            addCellToChunk x z cell

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
            let current = _cells[currentIndex]
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

                            if moveCost >= 0 then
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
        let range = range + fromCell.ViewElevation

        searchData[fromCell.Index] <-
            HexCellSearchData(searchPhase = searchFrontierPhase, pathFrom = searchData[fromCell.Index].pathFrom)

        searchFrontier.Enqueue fromCell.Index
        let fromCoordinates = fromCell.Coordinates
        let mutable currentIndex = searchFrontier.Dequeue()

        while currentIndex >= 0 do
            let current = _cells[currentIndex]
            searchData[currentIndex].searchPhase <- 1 + searchData[currentIndex].searchPhase

            visibleCells.Add current

            for d in HexDirection.allHexDirs () do
                match current.GetNeighbor d with
                | Some neighbor ->
                    let neighborData = searchData[neighbor.Index]

                    if neighborData.searchPhase > searchFrontierPhase || not neighbor.Explorable then
                        ()
                    else
                        let distance = searchData[currentIndex].distance + 1

                        if
                            distance + neighbor.ViewElevation > range
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
            let mutable current = _cells[currentPathToIndex]

            while current <> _cells[currentPathFromIndex] do
                let turn = (searchData[current.Index].distance - 1) / speed
                current.SetLabel <| string turn
                current.EnableHighlight Colors.White
                current <- _cells[searchData[current.Index].pathFrom]

        _cells[currentPathFromIndex].EnableHighlight Colors.Blue
        _cells[currentPathToIndex].EnableHighlight Colors.Red

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
            Some _cells[x + z * this.cellCountX]

    member this.GetCell cellIndex = _cells[cellIndex]

    member this.GetCell(pos: Vector3) =
        let coordinates = HexCoordinates.FromPosition pos
        this.GetCell coordinates

    member this.Save(writer: BinaryWriter) =
        writer.Write this.cellCountX
        writer.Write this.cellCountZ
        writer.Write this.wrapping
        _cells |> Array.iter _.Save(writer)
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
            _cells |> Array.iter (fun c -> c.Load reader header)
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
            let mutable current = _cells[currentPathToIndex]

            while current <> _cells[currentPathFromIndex] do
                current.SetLabel ""
                current.DisableHighlight()
                current <- _cells[searchData[current.Index].pathFrom]

            current.DisableHighlight()
            currentPathExists <- false
        elif currentPathFromIndex >= 0 then
            _cells[currentPathFromIndex].DisableHighlight()
            _cells[currentPathToIndex].DisableHighlight()

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
                c.MarkAsExplored()
                this.cellShaderData.RefreshVisibility cellIndex)

    member this.DecreaseVisibility (fromCell: HexCellFS) range =
        getVisibleCells fromCell range
        |> Seq.iter (fun c ->
            let cellIndex = c.Index
            cellVisibility[cellIndex] <- cellVisibility[cellIndex] - 1

            if cellVisibility[cellIndex] = 0 then
                this.cellShaderData.RefreshVisibility cellIndex)

    member this.ResetVisibility() =
        { 0 .. _cells.Length - 1 }
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

            for cell in _cells do
                cell.TerrainTypeIndex <- rand.Next(0, 5)
                cell.Elevation <- rand.Next(0, 7)
                cell.WaterLevel <- 3
                this.IncreaseVisibility cell 1

                if cell.Elevation > 3 then
                    cell.UrbanLevel <- rand.Next(0, 4)
                    cell.FarmLevel <- rand.Next(0, 4)
                    cell.PlantLevel <- rand.Next(0, 4)

                    if cell.UrbanLevel = 3 then
                        cell.Walled <- true

                    cell.SetOutgoingRiver(rand.Next(0, 6) |> enum<HexDirection>)
                    cell.AddRoad << enum<HexDirection> <| rand.Next(0, 6)
                    cell.AddRoad << enum<HexDirection> <| rand.Next(0, 6)

    override this._Process delta =
        // if this.IsNodeReady() then
        // 还真是奇了怪了，必须按下面 dataReady 这样写才行…… 不然 this.cellShaderData 是空引用
        if dataReady then
            this.cellShaderData.UpdateData delta
