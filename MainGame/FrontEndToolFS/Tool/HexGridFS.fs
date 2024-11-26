namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexGridFS() as this =
    inherit Node3D()

    let _hexCellScene =
        lazy (GD.Load("res://game/HexPlane/Map/HexCell.tscn") :?> PackedScene)

    let _hexMesh = lazy this.GetNode<HexMeshFS>("HexMesh")

    let mutable _cells: HexCellFS array = Array.empty

    member val _width: int = 6 with get, set
    member val _height: int = 6 with get, set
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
        let index = coordinates.X + coordinates.Z * this._width + coordinates.Z / 2
        _cells[index]

    member this.Refresh() = _hexMesh.Value.Triangulate _cells

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
        HexMetrics.noiseSource <- this._noiseSource.GetImage()
        _cells <- Array.init (this._width * this._height) (fun _ -> _hexCellScene.Value.Instantiate<HexCellFS>())

        for i in 0 .. _cells.Length - 1 do
            let z = i / this._width
            let x = i % this._width
            let cell = _cells[i]
            cell.Coordinates <- HexCoordinates.FromOffsetCoordinates x z
            cell.Color <- this._defaultColor

            if x > 0 then
                cell.SetNeighbor HexDirection.W (Some _cells[i - 1])

            if z > 0 then
                if z &&& 1 = 0 then
                    cell.SetNeighbor HexDirection.SE (Some _cells[i - this._width])

                    if x > 0 then
                        cell.SetNeighbor HexDirection.SW (Some _cells[i - this._width - 1])
                else
                    cell.SetNeighbor HexDirection.SW (Some _cells[i - this._width])

                    if x < this._width - 1 then
                        cell.SetNeighbor HexDirection.SE (Some _cells[i - this._width + 1])

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
            this.AddChild cell

        _hexMesh.Value.Triangulate _cells
