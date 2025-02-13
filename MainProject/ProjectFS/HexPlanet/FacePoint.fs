namespace ProjectFS.HexPlanet

open System
open System.Collections.Generic
open Godot

type Point(position: Vector3, id: string, faces: Face List) =
    new(position) = Point(position, Guid.NewGuid().ToString(), List<Face>())
    member this.Position = position
    member this.Id = id
    member this.Faces = faces
    member this.AssignFace(face: Face) = faces.Add face

    member this.Subdivide (target: Point) count findDuplicatePointIfExists =
        let segments = List<Point>()
        segments.Add this

        for i in 1..count do
            let vec = this.Position.Lerp(target.Position, float32 i / float32 count)
            let newPoint = findDuplicatePointIfExists (Point(vec))
            segments.Add newPoint
        // segments.Add target // 感觉重复了？且不会被用到
        segments

    member this.ProjectToSphere radius (t: float32) =
        let projectionPoint = radius / this.Position.Length()
        Point(this.Position * projectionPoint * t, this.Id, this.Faces)

    member this.GetOrderedFaces() =
        if this.Faces.Count = 0 then
            this.Faces
        else
            let orderedList = List<Face>()
            orderedList.Add this.Faces[0]
            let mutable currentFace = orderedList[0]

            while orderedList.Count < this.Faces.Count do
                let existingIds = orderedList |> Seq.map _.Id |> List

                let neighbor =
                    faces
                    |> Seq.find (fun face -> not (existingIds.Contains face.Id) && face.IsAdjacentToFace currentFace)

                currentFace <- neighbor
                orderedList.Add currentFace

            orderedList

    static member private ComparisonAccuracy = 0.0001f // 不知道为啥不能是 let

    static member IsOverlapping (a: Point) (b: Point) =
        Mathf.Abs(a.Position.X - b.Position.X) <= Point.ComparisonAccuracy
        && Mathf.Abs(a.Position.Y - b.Position.Y) <= Point.ComparisonAccuracy
        && Mathf.Abs(a.Position.Z - b.Position.Z) <= Point.ComparisonAccuracy

and Face(point1: Point, point2: Point, point3: Point, trackFaceInPoints: bool) as this =
    let id = Guid.NewGuid().ToString()

    let getNormal (p1: Point) (p2: Point) (p3: Point) =
        let side1: Vector3 = p2.Position - p1.Position
        let side2 = p3.Position - p1.Position
        let cross = side1.Cross side2
        cross.Normalized()

    let isNormalPointingAwayFromOrigin surface normal =
        Vector3.Zero.DistanceTo surface < Vector3.Zero.DistanceTo(surface + normal)

    let center = (point1.Position + point2.Position + point3.Position) / 3.0f
    // 决定正确缠绕方向
    let normal = getNormal point1 point2 point3

    let points =
        if isNormalPointingAwayFromOrigin center normal then
            [ point1; point3; point2 ]
        else
            [ point1; point2; point3 ]

    do
        if trackFaceInPoints then
            for point in points do
                point.AssignFace this

    new(point1, point2, point3) = Face(point1, point2, point3, true)
    member this.Id = id
    member this.Points = points

    member private this.IsPointPartOfFace(point: Point) =
        this.Points |> Seq.exists (fun facePoint -> facePoint.Id = point.Id)

    member this.GetOtherPoints(point: Point) =
        if not <| this.IsPointPartOfFace point then
            failwith "Given point must be one of the points on the face!"

        this.Points |> Seq.filter (fun facePoint -> facePoint.Id <> point.Id) |> List

    member this.IsAdjacentToFace(face: Face) =
        let thisFaceIds = this.Points |> Seq.map _.Id |> List
        let otherFaceIds = face.Points |> Seq.map _.Id |> List
        let intersect = Set thisFaceIds |> Set.intersect (Set otherFaceIds)
        intersect.Count = 2

    member this.GetCenter() =
        Point(
            (this.Points[0].Position + this.Points[1].Position + this.Points[2].Position)
            / 3f
        )
