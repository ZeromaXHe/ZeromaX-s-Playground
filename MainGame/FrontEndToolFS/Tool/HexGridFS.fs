namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open Godot

type HexGridFS() =
    inherit Node3D()

    let _hexCellScene = GD.Load("res://game/HexPlane/Map/HexCell.tscn") :?> PackedScene

    let mutable _cells = Array.empty

    member val _width: int = 6 with get, set
    member val _height: int = 6 with get, set

    override this._Ready() =
        GD.Print "HexGridFS _Ready"
        _cells <- Array.init (this._width * this._height) (fun _ -> _hexCellScene.Instantiate<HexCellFS>())

        for i in 0 .. _cells.Length - 1 do
            let z = i / this._width
            let x = i % this._width

            let cell = _cells[i]
            cell.Coordinates <- HexCoordinates.FromOffsetCoordinates x z

            cell.Position <-
                Vector3(
                    (float32 x + float32 z * 0.5f - float32 (z / 2)) * HexMetrics.innerRadius * 2f,
                    0.0f,
                    float32 z * HexMetrics.outerRadius * 1.5f
                )

            let label = cell.GetNode<Label3D>("Label")
            label.Text <- cell.Coordinates.ToStringOnSeparateLines()
            this.AddChild cell
