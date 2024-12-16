namespace FrontEndToolFS.Tool

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open FrontEndToolFS.HexPlane
open Godot

type HexCellPriorityQueue() =
    let list = List<HexCellFS option>()

    let mutable minimum = Int32.MaxValue
    let mutable count = 0
    member this.Count = count

    member this.Enqueue(cell: HexCellFS) =
        count <- count + 1
        let priority = cell.SearchPriority

        if priority < minimum then
            minimum <- priority

        while priority >= list.Count do
            list.Add None

        cell.NextWithSamePriority <- list[priority]
        list[priority] <- Some cell

    member this.Dequeue() =
        count <- count - 1
        let mutable breakLoop = false
        let mutable result = None

        while minimum < list.Count && not breakLoop do
            let cellOpt = list[minimum]

            if cellOpt.IsSome then
                result <- cellOpt
                list[minimum] <- cellOpt.Value.NextWithSamePriority
                breakLoop <- true
            else
                minimum <- minimum + 1

        result

    member this.Change (cell: HexCellFS) oldPriority =
        let mutable current = list[oldPriority]
        let mutable next = current |> Option.bind _.NextWithSamePriority

        if current = Some cell then
            list[oldPriority] <- next
        else
            while next <> Some cell do
                current <- next
                next <- current |> Option.bind _.NextWithSamePriority

            current.Value.NextWithSamePriority <- cell.NextWithSamePriority

        this.Enqueue cell
        count <- count - 1 // 但我们实际上并没有添加新的单元格

    member this.Clear() =
        list.Clear()
        count <- 0
        minimum <- Int32.MaxValue

type HexGridFS() as this =
    inherit Node3D()

    interface IGrid with
        // 感觉 Catlike Coding 这部分的逻辑写的一坨……
        override this.MakeChildOfColumn child columnIndex =
            this.MakeChildOfColumn child columnIndex

        override this.AddUnit prefab cellOpt orientation =
            cellOpt
            |> Option.iter (fun cell ->
                let unit = prefab.Instantiate<HexUnitFS>()
                this.AddUnit unit cell orientation)

        override this.GetCell coordinates = this.GetCell coordinates
        override this.IncreaseVisibility cell range = this.IncreaseVisibility cell range
        override this.DecreaseVisibility cell range = this.DecreaseVisibility cell range

    interface IGridVis with
        override this.ResetVisibility() = this.ResetVisibility()
    interface IGridForCell with
        override this.GetCell coords = this.GetCell coords

    [<DefaultValue>]
    val mutable cellPrefab: PackedScene

    [<DefaultValue>]
    val mutable cellLabelPrefab: PackedScene

    [<DefaultValue>]
    val mutable chunkPrefab: PackedScene

    let mutable chunkCountX = 4
    let mutable chunkCountZ = 3

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
                chunk)

    let addCellToChunk x z cell =
        let chunkX = x / HexMetrics.chunkSizeX
        let chunkZ = z / HexMetrics.chunkSizeZ
        let chunk = _chunks[chunkX + chunkZ * chunkCountX]
        let localX = x - chunkX * HexMetrics.chunkSizeX
        let localZ = z - chunkZ * HexMetrics.chunkSizeZ
        chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX) cell

    let createCells () =
        _cells <- Array.init (this.cellCountX * this.cellCountZ) (fun _ -> this.cellPrefab.Instantiate<HexCellFS>())

        for i in 0 .. _cells.Length - 1 do
            let z = i / this.cellCountX
            let x = i % this.cellCountX
            let cell = _cells[i]
            cell.Name <- $"Cell{x}_{z}"
            cell.Coordinates <- HexCoordinates.FromOffsetCoordinates x z
            cell.Index <- i
            cell.ColumnIndex <- x / HexMetrics.chunkSizeX
            cell.ShaderData <- this.cellShaderData
            cell.Grid <- this

            if this.wrapping then
                cell.Explorable <- z > 0 && z < this.cellCountZ - 1
                // 用于测试时区分每个块列的第一个单元格
                if x % HexMetrics.chunkSizeX = 0 then
                    cell.TerrainTypeIndex <- 1
            else
                cell.Explorable <- x > 0 && z > 0 && x < this.cellCountX - 1 && z < this.cellCountZ - 1

            cell.Position <-
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerDiameter,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )

            let label = this.cellLabelPrefab.Instantiate<HexCellLabelFS>()
            label.Position <- cell.Position + Vector3.Up * 0.01f
            cell.uiRect <- label
            // 触发 setter 应用扰动 y
            cell.Elevation <- 0
            // 得在 Elevation 前面。不然单元格的块还没赋值的话，setter 里面会有 refresh，需要刷新块，这时空引用会报错
            addCellToChunk x z cell

    let searchFrontier = HexCellPriorityQueue()
    let mutable searchFrontierPhase = 0
    let mutable currentPathFrom: HexCellFS option = None
    let mutable currentPathTo: HexCellFS option = None
    let mutable currentPathExists = false
    member this.HasPath = currentPathExists

    let search (fromCell: HexCellFS) (toCell: HexCellFS) (unit: HexUnitFS) =
        let speed = unit.Speed
        searchFrontierPhase <- 2 + searchFrontierPhase
        searchFrontier.Clear()
        fromCell.SearchPhase <- searchFrontierPhase
        fromCell.Distance <- 0
        searchFrontier.Enqueue fromCell

        let mutable breakLoop = false

        while searchFrontier.Count > 0 && not breakLoop do
            let current = searchFrontier.Dequeue().Value
            current.SearchPhase <- 1 + current.SearchPhase

            if current = toCell then
                breakLoop <- true
            else
                let currentTurn = (current.Distance - 1) / speed

                for d in HexDirection.allHexDirs () do
                    match current.GetNeighbor d with
                    | Some neighbor when
                        neighbor.SearchPhase <= searchFrontierPhase && unit.IsValidDestination(neighbor)
                        ->
                        let moveCost = unit.GetMoveCost current neighbor d

                        if moveCost >= 0 then
                            let distance = current.Distance + moveCost
                            let turn = (distance - 1) / speed

                            let distance =
                                if turn > currentTurn then
                                    turn * speed + moveCost
                                else
                                    distance

                            if neighbor.SearchPhase < searchFrontierPhase then
                                neighbor.SearchPhase <- searchFrontierPhase
                                neighbor.Distance <- distance
                                neighbor.PathFrom <- current
                                neighbor.SearchHeuristic <- neighbor.Coordinates.DistanceTo toCell.Coordinates
                                searchFrontier.Enqueue neighbor
                            elif distance < neighbor.Distance then
                                let oldPriority = neighbor.SearchPriority
                                neighbor.Distance <- distance
                                neighbor.PathFrom <- current
                                searchFrontier.Change neighbor oldPriority
                    | _ -> ()

        breakLoop

    let getVisibleCells (fromCell: HexCellFS) range =
        let visibleCells = List<HexCellFS>()
        searchFrontierPhase <- 2 + searchFrontierPhase
        searchFrontier.Clear()
        let range = range + fromCell.ViewElevation
        fromCell.SearchPhase <- searchFrontierPhase
        fromCell.Distance <- 0
        searchFrontier.Enqueue fromCell
        let fromCoordinates = fromCell.Coordinates

        while searchFrontier.Count > 0 do
            let current = searchFrontier.Dequeue().Value
            current.SearchPhase <- 1 + current.SearchPhase

            visibleCells.Add current

            for d in HexDirection.allHexDirs () do
                match current.GetNeighbor d with
                | Some neighbor when neighbor.SearchPhase <= searchFrontierPhase && neighbor.Explorable ->
                    let distance = current.Distance + 1

                    if
                        distance + neighbor.ViewElevation <= range
                        && distance <= fromCoordinates.DistanceTo neighbor.Coordinates
                    then
                        if neighbor.SearchPhase < searchFrontierPhase then
                            neighbor.SearchPhase <- searchFrontierPhase
                            neighbor.Distance <- distance
                            neighbor.SearchHeuristic <- 0
                            searchFrontier.Enqueue neighbor
                        elif distance < neighbor.Distance then
                            let oldPriority = neighbor.SearchPriority
                            neighbor.Distance <- distance
                            searchFrontier.Change neighbor oldPriority
                | _ -> ()

        visibleCells

    let showPath speed =
        if currentPathExists then
            let mutable current = currentPathTo.Value

            while current <> currentPathFrom.Value do
                let turn = (current.Distance - 1) / speed
                current.SetLabel <| string turn
                current.EnableHighlight Colors.White
                current <- current.PathFrom

        currentPathFrom.Value.EnableHighlight Colors.Blue
        currentPathTo.Value.EnableHighlight Colors.Red

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

    member this.GetCell(coordinates: HexCoordinates) =
        let z = coordinates.Z
        let x = coordinates.X + z / 2

        if z < 0 || z >= this.cellCountZ || x < 0 || x >= this.cellCountX then
            None
        else
            Some _cells[x + z * this.cellCountX]

    member this.GetCell(xOffset, zOffset) =
        _cells[xOffset + zOffset * this.cellCountX]

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
        currentPathFrom <- Some fromCell
        currentPathTo <- Some toCell
        currentPathExists <- search fromCell toCell unit
        showPath unit.Speed
        sw.Stop()
        // BUG: 现在一次点击会执行多次
        GD.Print $"FindPath search cost: {sw.ElapsedMilliseconds} ms"

    member this.ClearPath() =
        if currentPathExists then
            let mutable current = currentPathTo.Value

            while current <> currentPathFrom.Value do
                current.SetLabel ""
                current.DisableHighlight()
                current <- current.PathFrom

            current.DisableHighlight()
            currentPathExists <- false
        elif currentPathFrom.IsSome then
            currentPathFrom.Value.DisableHighlight()
            currentPathTo.Value.DisableHighlight()

        currentPathFrom <- None
        currentPathTo <- None

    member this.GetPath() =
        if currentPathExists then
            let path = List<HexCellFS>()
            let mutable c = currentPathTo.Value

            while Some c <> currentPathFrom do
                path.Add c
                c <- c.PathFrom

            path.Add currentPathFrom.Value
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
        unit.Location <- Some location
        unit.Orientation <- orientation

    member this.RemoveUnit(unit: HexUnitFS) =
        units.Remove unit |> ignore
        unit.Die()

    member this.IncreaseVisibility (fromCell: HexCellFS) range =
        getVisibleCells fromCell range |> Seq.iter _.IncreaseVisibility()

    member this.DecreaseVisibility (fromCell: HexCellFS) range =
        getVisibleCells fromCell range |> Seq.iter _.DecreaseVisibility()

    member this.ResetVisibility() =
        _cells |> Array.iter _.ResetVisibility()

        units
        |> Seq.iter (fun u -> this.IncreaseVisibility u.Location.Value u.VisionRange)

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
                cell.IncreaseVisibility()

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
