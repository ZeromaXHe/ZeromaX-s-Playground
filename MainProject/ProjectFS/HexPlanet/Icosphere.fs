namespace ProjectFS.HexPlanet

open System
open System.Collections.Generic
open Godot

// 基于正二十面体生成球面
module Icosphere =
    type PointId = PointId of string
    type Point = { Id: PointId; Position: Vector3 }
    type FaceId = FaceId of string
    type Face = { Id: FaceId }

    type Tile =
        { Center: Point
          Radius: float32
          Size: float32
          Points: Point List
          Faces: Face List
          NeighborCenters: Point List
          Neighbors: Tile List }

    type Hexasphere =
        { Radius: float32
          Subdivision: int
          HexSize: float32 }

    type MeshDetails =
        { Vertices: Vector3 List
          Triangles: int List }

    type DataBase =
        { Sphere: Hexasphere
          Points: Point List
          Tiles: Tile List
          MeshDetails: MeshDetails
          IcosahedronFaces: Face List
          FacePoints: Dictionary<FaceId, Point list>
          PointFaces: Dictionary<PointId, Face List> }

    let isFaceAdjacentToFace database (thisFace: Face) (otherFace: Face) =
        let thisFaceIds = database.FacePoints[thisFace.Id] |> Seq.map _.Id |> List
        let otherFaceIds = database.FacePoints[otherFace.Id] |> Seq.map _.Id |> List
        let intersect = Set thisFaceIds |> Set.intersect (Set otherFaceIds)
        intersect.Count = 2

    let getPointOrderedFaces database (point: Point) =
        let faces = database.PointFaces[point.Id]

        if faces.Count = 0 then
            faces
        else
            let orderedList = List<Face>()
            orderedList.Add faces[0]
            let mutable currentFace = orderedList[0]

            while orderedList.Count < faces.Count do
                let existingIds = orderedList |> Seq.map _.Id |> List

                let neighbor =
                    faces
                    |> Seq.find (fun face ->
                        not (existingIds.Contains face.Id)
                        && isFaceAdjacentToFace database face currentFace)

                currentFace <- neighbor
                orderedList.Add currentFace

            orderedList

    let initPointWithIdAndFaces database position id faces =
        database.PointFaces[id] <- faces
        { Id = id; Position = position }

    let initPoint database position =
        let id = PointId <| Guid.NewGuid().ToString()
        database.PointFaces.Add(id, List<Face>())
        { Id = id; Position = position }

    let private getNormal point1 point2 point3 =
        let side1 = point2.Position - point1.Position
        let side2 = point3.Position - point1.Position
        let cross = side1.Cross side2
        cross.Normalized()

    let private isNormalPointingAwayFromOrigin surface normal =
        Vector3.Zero.DistanceTo surface < Vector3.Zero.DistanceTo(surface + normal)

    let initFace database point1 point2 point3 trackFaceInPoints =
        let id = FaceId <| Guid.NewGuid().ToString()
        let center = (point1.Position + point2.Position + point3.Position) / 3f
        // 决定正确缠绕方向
        let normal = getNormal point1 point2 point3

        let points =
            if isNormalPointingAwayFromOrigin center normal then
                [ point1; point3; point2 ]
            else
                [ point1; point2; point3 ]

        database.FacePoints.Add(id, points)
        let face = { Id = id }

        if trackFaceInPoints then
            for p in points do
                database.PointFaces[p.Id].Add face

        face

    let isPointPartOfFace database (face: Face) (point: Point) =
        database.FacePoints[face.Id] |> Seq.exists (fun p -> p.Id = point.Id)

    let getFaceOtherPoints database face point =
        if not <| isPointPartOfFace database face point then
            failwith "Given point must be one of the points on the face!"

        database.FacePoints[face.Id] |> Seq.filter (fun p -> p.Id <> point.Id) |> List

    let storeNeighborCenters database tile icosahedronFaces =
        for face in icosahedronFaces do
            let otherPoints = getFaceOtherPoints database face tile.Center

            for point in otherPoints do
                if (tile.NeighborCenters |> Seq.tryFind (fun p -> p.Id = point.Id)).IsNone then
                    tile.NeighborCenters.Add point

    let getFaceCenter database face =
        let points = database.FacePoints[face.Id]
        initPoint database ((points[0].Position + points[1].Position + points[2].Position) / 3f)

    let projectToSphere database point radius t =
        let projectionPoint = radius / point.Position.Length()
        initPointWithIdAndFaces database (point.Position * projectionPoint * t) point.Id database.PointFaces[point.Id]

    let private buildFaces database tile icosahedronFaces =
        let polygonPoints =
            icosahedronFaces
            |> Seq.map (fun face -> tile.Center.Position.Lerp((getFaceCenter database face).Position, tile.Size))
            |> List

        for pos in polygonPoints do
            tile.Points.Add
            <| projectToSphere database (initPoint database pos) tile.Radius 0.5f

        tile.Faces.Add
        <| initFace database tile.Points[0] tile.Points[1] tile.Points[2] true

        tile.Faces.Add
        <| initFace database tile.Points[0] tile.Points[2] tile.Points[3] true

        tile.Faces.Add
        <| initFace database tile.Points[0] tile.Points[3] tile.Points[4] true

        if tile.Points.Count > 5 then
            tile.Faces.Add
            <| initFace database tile.Points[0] tile.Points[4] tile.Points[5] true

    let initTile database center radius size =
        let tile =
            { Center = center
              Radius = radius
              Size = Mathf.Clamp(size, 0.01f, 1f)
              Points = List<Point>()
              Faces = List<Face>()
              NeighborCenters = List<Point>()
              Neighbors = List<Tile>() }

        let icosahedronFaces = getPointOrderedFaces database center
        storeNeighborCenters database tile icosahedronFaces
        buildFaces database tile icosahedronFaces
        tile

    let subdividePoint database from target count findDuplicatePointIfExists =
        let segments = List<Point>()
        segments.Add from

        for i in 1..count do
            let vec = from.Position.Lerp(target.Position, float32 i / float32 count)
            let newPoint = findDuplicatePointIfExists (initPoint database vec)
            segments.Add newPoint
        // segments.Add target // 感觉重复了？且不会被用到
        segments

    let pointComparisonAccuracy = 0.0001f

    let isPointOverlapping (a: Point) (b: Point) =
        Mathf.Abs(a.Position.X - b.Position.X) <= pointComparisonAccuracy
        && Mathf.Abs(a.Position.Y - b.Position.Y) <= pointComparisonAccuracy
        && Mathf.Abs(a.Position.Z - b.Position.Z) <= pointComparisonAccuracy

    let cachePoint (database: DataBase) point =
        // TODO：这样检查点是否重复效率很低，考虑引入八叉树之类的？
        let existingPoint = database.Points |> Seq.tryFind (isPointOverlapping point)

        if existingPoint.IsSome then
            database.PointFaces.Remove point.Id |> ignore
            existingPoint.Value
        else
            database.Points.Add point
            point

    let private constructIcosahedron database =
        let icosahedronCorners = Icosahedron.vertices |> Array.map (initPoint database)

        icosahedronCorners
        |> Array.iter (fun point -> cachePoint database point |> ignore)

        for i in 0..3 .. Icosahedron.indices.Length - 1 do
            database.IcosahedronFaces.Add(
                initFace
                    database
                    icosahedronCorners[Icosahedron.indices[i]]
                    icosahedronCorners[Icosahedron.indices[i + 1]]
                    icosahedronCorners[Icosahedron.indices[i + 2]]
                    false
            )

    // 将正二十面体的三角面细分为指定次数 _divisions（对应三条边会被切分为 _divisions + 1 段）
    let private subdivideIcosahedron database =
        for icoFace in database.IcosahedronFaces do
            let facePoints = database.FacePoints[icoFace.Id]
            let mutable bottomSide = List<Point>()
            bottomSide.Add facePoints[0]

            let leftSide =
                subdividePoint database facePoints[0] facePoints[1] database.Sphere.Subdivision (cachePoint database)

            let rightSide =
                subdividePoint database facePoints[0] facePoints[2] database.Sphere.Subdivision (cachePoint database)

            for i in 1 .. database.Sphere.Subdivision do
                let previousPoints = bottomSide

                bottomSide <- subdividePoint database leftSide[i] rightSide[i] i (cachePoint database)

                for j in 0 .. i - 1 do
                    // 不需要存储面，它们的点会持有他们的引用
                    initFace database previousPoints[j] bottomSide[j] bottomSide[j + 1] true
                    |> ignore

                    if j <> 0 then
                        initFace database previousPoints[j - 1] previousPoints[j] bottomSide[j] true
                        |> ignore

    let resolveNeighborTiles tile allTiles =
        let neighborIds = tile.NeighborCenters |> Seq.map _.Id |> List
        tile.Neighbors.Clear()
        // TODO: 效率低，待优化
        tile.Neighbors.AddRange(allTiles |> Seq.filter (fun tile -> neighborIds.Contains tile.Center.Id))

    let private constructTiles database =
        for point in database.Points do
            database.Tiles.Add
            <| initTile database point database.Sphere.Radius database.Sphere.HexSize

        for tile in database.Tiles do
            resolveNeighborTiles tile database.Tiles

    let storeMeshDetails database (surfaceTool: SurfaceTool) =
        let points = List<Point>()

        for tile in database.Tiles do
            surfaceTool.SetColor <| Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf())
            let heightMultiplier = float32 <| GD.RandRange(1, 1.05)
            for point in tile.Points do
                points.Add point
                surfaceTool.AddVertex (point.Position * heightMultiplier)

            for face in tile.Faces do
                for p in database.FacePoints[face.Id] do
                    let vertexIndex = points |> Seq.findIndex (fun vertex -> vertex.Id = p.Id)
                    surfaceTool.AddIndex vertexIndex

    let initHexasphere radius divisions hexSize =
        let sphere =
            { Radius = radius
              Subdivision = divisions
              HexSize = hexSize }

        let database =
            { Sphere = sphere
              Points = List<Point>()
              Tiles = List<Tile>()
              MeshDetails =
                { Vertices = List<Vector3>()
                  Triangles = List<int>() }
              IcosahedronFaces = List<Face>()
              FacePoints = Dictionary<FaceId, Point list>()
              PointFaces = Dictionary<PointId, Face List>() }

        constructIcosahedron database
        subdivideIcosahedron database
        constructTiles database
        database
