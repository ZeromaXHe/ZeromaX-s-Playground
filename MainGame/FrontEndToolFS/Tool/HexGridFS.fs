namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexGridFS() as this =
    inherit Node3D()

    let _hexCellScene =
        lazy (GD.Load("res://game/HexPlane/Map/HexCell.tscn") :?> PackedScene)

    let _hexChunkScene =
        lazy (GD.Load("res://game/HexPlane/Map/HexGridChunk.tscn") :?> PackedScene)

    let mutable cellCountX = 24
    let mutable cellCountZ = 18

    let mutable _cells: HexCellFS array = null
    let mutable _chunks: HexGridChunkFS array = null

    let createChunks () =
        _chunks <-
            Array.init (this._chunkCountX * this._chunkCountZ) (fun _ ->
                _hexChunkScene.Value.Instantiate<HexGridChunkFS>())

        _chunks
        |> Array.iteri (fun i c ->
            let x = i % this._chunkCountX
            let z = i / this._chunkCountX
            c.Name <- $"Chunk{x}_{z}"
            this.AddChild c)

    let addCellToChunk x z cell =
        let chunkX = x / HexMetrics.chunkSizeX
        let chunkZ = z / HexMetrics.chunkSizeZ
        let chunk = _chunks[chunkX + chunkZ * this._chunkCountX]
        let localX = x - chunkX * HexMetrics.chunkSizeX
        let localZ = z - chunkZ * HexMetrics.chunkSizeZ
        chunk.AddCell (localX + localZ * HexMetrics.chunkSizeX) cell

    let createCells () =
        _cells <- Array.init (cellCountX * cellCountZ) (fun _ -> _hexCellScene.Value.Instantiate<HexCellFS>())

        for i in 0 .. _cells.Length - 1 do
            let z = i / cellCountX
            let x = i % cellCountX
            let cell = _cells[i]
            cell.Name <- $"Cell{x}_{z}"
            cell.Coordinates <- HexCoordinates.FromOffsetCoordinates x z
            cell.Color <- this._defaultColor

            if x > 0 then
                cell.SetNeighbor HexDirection.W (Some _cells[i - 1])

            if z > 0 then
                if z &&& 1 = 0 then
                    cell.SetNeighbor HexDirection.SE (Some _cells[i - cellCountX])

                    if x > 0 then
                        cell.SetNeighbor HexDirection.SW (Some _cells[i - cellCountX - 1])
                else
                    cell.SetNeighbor HexDirection.SW (Some _cells[i - cellCountX])

                    if x < cellCountX - 1 then
                        cell.SetNeighbor HexDirection.SE (Some _cells[i - cellCountX + 1])

            cell.Position <-
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerRadius * 2f,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )
            // 触发 setter 应用扰动 y
            cell.Elevation <- 0

            let label = cell.GetNode<Label3D>("Label")
            label.Text <- cell.Coordinates.ToStringOnSeparateLines()

            addCellToChunk x z cell

    member val _chunkCountX: int = 6 with get, set
    member val _chunkCountZ: int = 6 with get, set
    member val _defaultColor: Color = Colors.White with get, set
    member val _touchedColor: Color = Colors.Magenta with get, set
    member val _noiseSource: Texture2D = null with get, set

    member this.CameraRayCastToMouse() =
        let spaceState = this.GetWorld3D().DirectSpaceState
        let camera = this.GetViewport().GetCamera3D()
        let mousePos = this.GetViewport().GetMousePosition()

        let origin = camera.ProjectRayOrigin mousePos
        let endPoint = origin + camera.ProjectRayNormal mousePos * 1000.0f
        let query = PhysicsRayQueryParameters3D.Create(origin, endPoint)
        query.CollideWithAreas <- true

        spaceState.IntersectRay query

    member this.GetCell(pos: Vector3) =
        let coordinates = HexCoordinates.FromPosition pos
        let index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2
        _cells[index]

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
        HexMetrics.noiseSource <- this._noiseSource.GetImage()

        cellCountX <- this._chunkCountX * HexMetrics.chunkSizeX
        cellCountZ <- this._chunkCountZ * HexMetrics.chunkSizeZ

        createChunks ()
        createCells ()
