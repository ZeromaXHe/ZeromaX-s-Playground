namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type HexMeshFS() =
    inherit MeshInstance3D()

    let addTriangle v1 v2 v3 c1 c2 c3 (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.SetColor c1
        surfaceTool.AddVertex v1
        surfaceTool.SetColor c2
        surfaceTool.AddVertex v2
        surfaceTool.SetColor c3
        surfaceTool.AddVertex v3
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 2
        vIndex + 3

    let triangulate (surfaceTool: SurfaceTool) (cell: HexCellFS) vIndex =
        let center = cell.Position

        HexDirection.all ()
        |> List.map (fun dir ->
            let prevNeighbor = cell.GetNeighbor <| dir.Previous() |> Option.defaultValue cell
            let neighbor = cell.GetNeighbor dir |> Option.defaultValue cell
            let nextNeighbor = cell.GetNeighbor <| dir.Next() |> Option.defaultValue cell
            // Godot 渲染面方向和 Unity 相反
            addTriangle
                center
                (center + HexMetrics.getFirstCorner dir)
                (center + HexMetrics.getSecondCorner dir)
                cell.Color
                ((cell.Color + neighbor.Color + nextNeighbor.Color) / 3f)
                ((cell.Color + prevNeighbor.Color + neighbor.Color) / 3f)
                surfaceTool)
        |> List.reduce (>>)
        <| vIndex

    member this.Triangulate cells =
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles)
        surfaceTool.SetUV Vector2.Zero
        cells |> Array.map (triangulate surfaceTool) |> Array.reduce (>>) <| 0 |> ignore
        surfaceTool.GenerateNormals()
        let material = new StandardMaterial3D()
        material.VertexColorUseAsAlbedo <- true
        surfaceTool.SetMaterial(material)
        this.Mesh <- surfaceTool.Commit()
        this.CreateTrimeshCollision()
