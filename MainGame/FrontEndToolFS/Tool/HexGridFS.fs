namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexGridFS() as this =
    inherit Node3D()

    let _hexCellScene = lazy (GD.Load("res://game/HexPlane/Map/HexCell.tscn") :?> PackedScene)

    let _hexMesh = lazy this.GetNode<HexMeshFS>("HexMesh")

    let mutable _cells: HexCellFS array = Array.empty

    member val _width: int = 6 with get, set
    member val _height: int = 6 with get, set
    member val _defaultColor: Color = Colors.White with get, set
    member val _touchedColor: Color = Colors.Magenta with get, set

    member this.CameraRayCastToMouse() =
        let spaceState = this.GetWorld3D().DirectSpaceState
        let camera = this.GetViewport().GetCamera3D()
        let mousePos = this.GetViewport().GetMousePosition()

        let origin = camera.ProjectRayOrigin mousePos
        let endPoint = origin + camera.ProjectRayNormal mousePos * 1000.0f
        let query = PhysicsRayQueryParameters3D.Create(origin, endPoint)
        query.CollideWithAreas <- true

        spaceState.IntersectRay query

    member this.ColorCell (pos: Vector3) color =
        let coordinates = HexCoordinates.FromPosition pos
        let index = coordinates.X + coordinates.Z * this._width + coordinates.Z / 2
        let cell = _cells[index]
        cell.Color <- color
        _hexMesh.Value.Triangulate _cells
        GD.Print $"rayCast position: {pos.ToString()}, coordinates: {coordinates.ToString()}"

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
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

            let label = cell.GetNode<Label3D>("Label")
            label.Text <- cell.Coordinates.ToStringOnSeparateLines()
            this.AddChild cell

        _hexMesh.Value.Triangulate _cells
