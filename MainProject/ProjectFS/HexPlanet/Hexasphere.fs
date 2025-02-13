namespace ProjectFS.HexPlanet

open Godot
open System.Collections.Generic

type Hexasphere(radius: float32, divisions: int, hexSize: float32) =
    let tiles = List<Tile>()
    let points = List<Point>()

    let icosahedronFaces = List<Face>()

    let cachePoint point =
        // TODO：这样检查点是否重复效率很低，考虑引入八叉树之类的？
        let existingPoint = points |> Seq.tryFind (Point.IsOverlapping point)

        if existingPoint.IsSome then
            existingPoint.Value
        else
            points.Add point
            point

    let constructIcosahedron () =
        let icosahedronCorners = Icosahedron.vertices |> Array.map Point

        icosahedronCorners |> Array.iter (fun point -> cachePoint point |> ignore)

        for i in 0..3 .. Icosahedron.indices.Length - 1 do
            icosahedronFaces.Add
            <| Face(
                icosahedronCorners[Icosahedron.indices[i]],
                icosahedronCorners[Icosahedron.indices[i + 1]],
                icosahedronCorners[Icosahedron.indices[i + 2]],
                false
            )

    // 将正二十面体的三角面细分为指定次数 _divisions（对应三条边会被切分为 _divisions + 1 段）
    let subdivideIcosahedron () =
        for icoFace in icosahedronFaces do
            let facePoints = icoFace.Points
            let mutable bottomSide = List<Point>()
            bottomSide.Add facePoints[0]
            let leftSide = facePoints[0].Subdivide facePoints[1] divisions cachePoint
            let rightSide = facePoints[0].Subdivide facePoints[2] divisions cachePoint

            for i in 1..divisions do
                let previousPoints = bottomSide
                bottomSide <- leftSide[i].Subdivide rightSide[i] i cachePoint

                for j in 0 .. i - 1 do
                    // 不需要存储面，它们的点会持有他们的引用
                    Face(previousPoints[j], bottomSide[j], bottomSide[j + 1]) |> ignore

                    if j <> 0 then
                        Face(previousPoints[j - 1], previousPoints[j], bottomSide[j]) |> ignore

    let constructTiles () =
        for point in points do
            tiles.Add <| Tile(point, radius, hexSize)

        for tile in tiles do
            tile.ResolveNeighborTiles tiles

    do constructIcosahedron ()
    do subdivideIcosahedron ()
    do constructTiles ()

    member this.StoreMeshDetails (surfaceTool: SurfaceTool) =
        let points = List<Point>()

        for tile in tiles do
            surfaceTool.SetColor <| Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf())
            let heightMultiplier = float32 <| GD.RandRange(1, 1.05)

            for point in tile.Points do
                points.Add point
                surfaceTool.AddVertex(point.Position * heightMultiplier)

            for face in tile.Faces do
                for p in face.Points do
                    let vertexIndex = points |> Seq.findIndex (fun vertex -> vertex.Id = p.Id)
                    surfaceTool.AddIndex vertexIndex
