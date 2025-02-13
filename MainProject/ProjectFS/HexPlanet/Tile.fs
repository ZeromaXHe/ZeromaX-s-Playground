namespace ProjectFS.HexPlanet

open Godot
open System.Collections.Generic

type Tile(center: Point, radius: float32, sizeIn: float32) =
    let points = List<Point>()
    let faces = List<Face>()
    let neighborCenters = List<Point>()
    let neighbors = List<Tile>()
    let size = Mathf.Clamp(sizeIn, 0.01f, 1f)

    let storeNeighborCenters (icosahedronFaces: Face List) =
        for face in icosahedronFaces do
            let otherPoints = face.GetOtherPoints center

            for point in otherPoints do
                if (neighborCenters |> Seq.tryFind (fun p -> p.Id = point.Id)).IsNone then
                    neighborCenters.Add point

    let buildFaces (icosahedronFaces: Face List) =
        let polygonPoints =
            icosahedronFaces
            |> Seq.map (fun face -> center.Position.Lerp(face.GetCenter().Position, size))
            |> List

        for pos in polygonPoints do
            points.Add <| Point(pos).ProjectToSphere radius 0.5f

        faces.Add <| Face(points[0], points[1], points[2])
        faces.Add <| Face(points[0], points[2], points[3])
        faces.Add <| Face(points[0], points[3], points[4])

        if points.Count > 5 then
            faces.Add <| Face(points[0], points[4], points[5])

    let icosahedronFaces = center.GetOrderedFaces()
    do storeNeighborCenters icosahedronFaces
    do buildFaces icosahedronFaces

    member this.Center = center
    member this.Points = points
    member this.Faces = faces
    
    member this.ResolveNeighborTiles (allTiles: Tile List) =
        let neighborIds = neighborCenters |> Seq.map _.Id |> List
        neighbors.Clear()
        // TODO: 效率低，待优化
        neighbors.AddRange(allTiles |> Seq.filter (fun tile -> neighborIds.Contains tile.Center.Id))
