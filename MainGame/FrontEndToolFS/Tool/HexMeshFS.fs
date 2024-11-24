namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type HexMeshFS() =
    inherit MeshInstance3D()

    let addTriangle (triV: Vector3 array) (triC: Color array) (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.SetColor triC[0]
        surfaceTool.AddVertex triV[0]
        surfaceTool.SetColor triC[1]
        surfaceTool.AddVertex triV[1]
        surfaceTool.SetColor triC[2]
        surfaceTool.AddVertex triV[2]
        // Godot 渲染面方向和 Unity 相反
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 1
        vIndex + 3

    let addQuad (triV: Vector3 array) (triC: Color array) (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.SetColor triC[0]
        surfaceTool.AddVertex triV[0]
        surfaceTool.SetColor triC[1]
        surfaceTool.AddVertex triV[1]
        surfaceTool.SetColor triC[2]
        surfaceTool.AddVertex triV[2]
        surfaceTool.SetColor triC[3]
        surfaceTool.AddVertex triV[3]
        // Godot 渲染面方向和 Unity 相反
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 3
        vIndex + 4

    let triangulateDir (surfaceTool: SurfaceTool) (cell: HexCellFS) dir =
        let center = cell.Position
        let bridge = HexMetrics.getBridge dir
        let v1 = center + HexMetrics.getFirstSolidCorner dir
        let v2 = center + HexMetrics.getSecondSolidCorner dir
        let v3 = v1 + bridge
        let v4 = v2 + bridge
        let prevNeighbor = cell.GetNeighbor <| dir.Previous() |> Option.defaultValue cell
        let neighbor = cell.GetNeighbor dir |> Option.defaultValue cell
        let nextNeighbor = cell.GetNeighbor <| dir.Next() |> Option.defaultValue cell
        let c1 = cell.Color
        let c2 = neighbor.Color
        let bridgeColor = (c1 + c2) * 0.5f
        let c3 = (cell.Color + prevNeighbor.Color + neighbor.Color) / 3f
        let c4 = (cell.Color + neighbor.Color + nextNeighbor.Color) / 3f

        let triangleAdder =
            addTriangle [| center; v1; v2 |] (Array.create 3 cell.Color) surfaceTool

        let quadAdder =
            addQuad [| v1; v2; v3; v4 |] [| c1; c1; bridgeColor; bridgeColor |] surfaceTool

        let gap1TriAdder =
            addTriangle [| v1; center + HexMetrics.getFirstCorner dir; v3 |] [| c1; c3; bridgeColor |] surfaceTool

        let gap2TriAdder =
            addTriangle [| v2; v4; center + HexMetrics.getSecondCorner dir |] [| c1; bridgeColor; c4 |] surfaceTool

        triangleAdder >> quadAdder >> gap1TriAdder >> gap2TriAdder

    let triangulate (surfaceTool: SurfaceTool) (cell: HexCellFS) vIndex =
        allHexDirs () |> List.map (triangulateDir surfaceTool cell) |> List.reduce (>>)
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
        // 仅在游戏中生成碰撞体（不然编辑器里执行会给场景新增 StaticBody 子节点，以及下面的碰撞体）
        if not <| Engine.IsEditorHint() then
            this.CreateTrimeshCollision()
