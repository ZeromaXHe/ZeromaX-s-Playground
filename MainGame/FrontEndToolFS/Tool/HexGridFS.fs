namespace FrontEndToolFS.Tool

open System
open System.IO
open FrontEndToolFS.HexPlane
open Godot

type HexGridFS() as this =
    inherit Node3D()

    let _hexCellScene =
        lazy (GD.Load("res://game/HexPlane/Map/HexCell.tscn") :?> PackedScene)

    let _hexChunkScene =
        lazy (GD.Load("res://game/HexPlane/Map/HexGridChunk.tscn") :?> PackedScene)

    let mutable chunkCountX = 4
    let mutable chunkCountZ = 3

    let mutable _cells: HexCellFS array = null
    let mutable _chunks: HexGridChunkFS array = null

    let createChunks () =
        _chunks <- Array.init (chunkCountX * chunkCountZ) (fun _ -> _hexChunkScene.Value.Instantiate<HexGridChunkFS>())

        _chunks
        |> Array.iteri (fun i c ->
            let x = i % chunkCountX
            let z = i / chunkCountX
            c.Name <- $"Chunk{x}_{z}"
            this.AddChild c)

    let addCellToChunk x z cell =
        let chunkX = x / HexMetrics.chunkSizeX
        let chunkZ = z / HexMetrics.chunkSizeZ
        let chunk = _chunks[chunkX + chunkZ * chunkCountX]
        let localX = x - chunkX * HexMetrics.chunkSizeX
        let localZ = z - chunkZ * HexMetrics.chunkSizeZ
        chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX) cell

    let createCells () =
        _cells <- Array.init (this.cellCountX * this.cellCountZ) (fun _ -> _hexCellScene.Value.Instantiate<HexCellFS>())

        for i in 0 .. _cells.Length - 1 do
            let z = i / this.cellCountX
            let x = i % this.cellCountX
            let cell = _cells[i]
            cell.Name <- $"Cell{x}_{z}"
            cell.Coordinates <- HexCoordinates.FromOffsetCoordinates x z

            if x > 0 then
                cell.SetNeighbor HexDirection.W (Some _cells[i - 1])

            if z > 0 then
                if z &&& 1 = 0 then
                    cell.SetNeighbor HexDirection.SE (Some _cells[i - this.cellCountX])

                    if x > 0 then
                        cell.SetNeighbor HexDirection.SW (Some _cells[i - this.cellCountX - 1])
                else
                    cell.SetNeighbor HexDirection.SW (Some _cells[i - this.cellCountX])

                    if x < this.cellCountX - 1 then
                        cell.SetNeighbor HexDirection.SE (Some _cells[i - this.cellCountX + 1])

            cell.Position <-
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerRadius * 2f,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )

            let label = cell.GetNode<Label3D>("Label")
            label.Text <- cell.Coordinates.ToStringOnSeparateLines()
            // 得在 Elevation 前面。不然单元格的块还没赋值的话，setter 里面会有 refresh，需要刷新块，这时空引用会报错
            addCellToChunk x z cell
            // 触发 setter 应用扰动 y
            cell.Elevation <- 0

    member val cellCountX: int = 20 with get, set
    member val cellCountZ: int = 15 with get, set
    member val _noiseSource: Texture2D = null with get, set
    member val seed = 1234 with get, set
    member val colors: Color array = null with get, set

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

        if z < 0 || z >= this.cellCountZ then
            None
        else
            let x = coordinates.X + z / 2

            if x < 0 || x >= this.cellCountX then
                None
            else
                Some _cells[x + z * this.cellCountX]

    member this.GetCell(pos: Vector3) =
        let coordinates = HexCoordinates.FromPosition pos
        this.GetCell coordinates

    member this.Save(writer: BinaryWriter) =
        writer.Write this.cellCountX
        writer.Write this.cellCountZ
        _cells |> Array.iter _.Save(writer)

    member this.Load (reader: BinaryReader) header =
        let x = if header >= 1 then reader.ReadInt32() else 20
        let z = if header >= 1 then reader.ReadInt32() else 15

        if (x = this.cellCountX && z = this.cellCountZ) || this.CreateMap x z then
            _cells |> Array.iter _.Load(reader)
            _chunks |> Array.iter _.Refresh()

    member this.CreateMap x z =
        if
            x <= 0
            || x % HexMetrics.chunkSizeX <> 0
            || z <= 0
            || z % HexMetrics.chunkSizeZ <> 0
        then
            GD.PrintErr "Unsupported map size"
            false
        else
            if _chunks <> null then
                _chunks |> Array.iter _.QueueFree()

            this.cellCountX <- x
            this.cellCountZ <- z
            chunkCountX <- this.cellCountX / HexMetrics.chunkSizeX
            chunkCountZ <- this.cellCountZ / HexMetrics.chunkSizeZ

            createChunks ()
            createCells ()
            true

    member this.ShowUI visible =
        _cells |> Array.iter (fun c -> c.ShowUI visible)

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
        HexMetrics.noiseSource <- this._noiseSource.GetImage()
        HexMetrics.initializeHashGrid <| uint64 this.seed
        HexMetrics.colors <- this.colors
        this.CreateMap this.cellCountX this.cellCountZ |> ignore
        // 编辑器里显示随机颜色和随机高度的单元格
        if Engine.IsEditorHint() then
            let rand = Random()

            for cell in _cells do
                cell.TerrainTypeIndex <- rand.Next(0, 5)
                cell.Elevation <- rand.Next(0, 7)
