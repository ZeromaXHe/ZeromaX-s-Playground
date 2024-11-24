namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot
open Microsoft.FSharp.Core

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

    let triangulateConerTerraces
        beginV
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        (surfaceTool: SurfaceTool)
        =
        let v3 = HexMetrics.terraceLerp beginV left 1
        let v4 = HexMetrics.terraceLerp beginV right 1
        let c3 = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color 1
        let c4 = HexMetrics.terraceColorLerp beginCell.Color rightCell.Color 1

        let triAdder =
            addTriangle [| beginV; v3; v4 |] [| beginCell.Color; c3; c4 |] surfaceTool

        let quadsMid, v3, v4, c3, c4 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (quadsAdder, v1, v2, c1, c2) i ->
                    let v3' = HexMetrics.terraceLerp beginV left i
                    let v4' = HexMetrics.terraceLerp beginV right i
                    let c3' = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color i
                    let c4' = HexMetrics.terraceColorLerp beginCell.Color rightCell.Color i

                    let quadsAdder' =
                        quadsAdder >> addQuad [| v1; v2; v3'; v4' |] [| c1; c2; c3'; c4' |] surfaceTool

                    quadsAdder', v3', v4', c3', c4')
                (id, v3, v4, c3, c4)

        let quadLast =
            addQuad [| v3; v4; left; right |] [| c3; c4; leftCell.Color; rightCell.Color |] surfaceTool

        triAdder >> quadsMid >> quadLast

    let triangulateBoundaryTriangle
        beginV
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        boundary
        boundaryColor
        (surfaceTool: SurfaceTool)
        =
        let v2 = HexMetrics.terraceLerp beginV left 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color 1

        let tri1 =
            addTriangle [| beginV; v2; boundary |] [| beginCell.Color; c2; boundaryColor |] surfaceTool

        let trisMid, v2, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (trisAdder, v1, c1) i ->
                    let v2' = HexMetrics.terraceLerp beginV left i
                    let c2' = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color i

                    let trisAdder' =
                        trisAdder
                        >> addTriangle [| v1; v2'; boundary |] [| c1; c2'; boundaryColor |] surfaceTool

                    trisAdder', v2', c2')
                (id, v2, c2)

        let triLast =
            addTriangle [| v2; left; boundary |] [| c2; leftCell.Color; boundaryColor |] surfaceTool

        tri1 >> trisMid >> triLast

    /// 处理 SCS 和 SCC
    let triangulateCornerTerracesCliff
        (beginV: Vector3)
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        (surfaceTool: SurfaceTool)
        =
        let b = 1f / float32 (rightCell.Elevation - beginCell.Elevation) |> Mathf.Abs
        let boundary = beginV.Lerp(right, b)
        let boundaryColor = beginCell.Color.Lerp(rightCell.Color, b)

        let bottomAdder =
            triangulateBoundaryTriangle beginV beginCell left leftCell boundary boundaryColor surfaceTool

        let topAdder =
            if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
                triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor surfaceTool
            else
                addTriangle [| left; right; boundary |] [| leftCell.Color; rightCell.Color; boundaryColor |] surfaceTool

        bottomAdder >> topAdder

    /// 处理 CSS 和 CSC
    let triangulateCornerCliffTerraces
        (beginV: Vector3)
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        (surfaceTool: SurfaceTool)
        =
        let b = 1f / float32 (leftCell.Elevation - beginCell.Elevation) |> Mathf.Abs
        let boundary = beginV.Lerp(left, b)
        let boundaryColor = beginCell.Color.Lerp(leftCell.Color, b)

        let bottomAdder =
            triangulateBoundaryTriangle right rightCell beginV beginCell boundary boundaryColor surfaceTool

        let topAdder =
            if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
                triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor surfaceTool
            else
                addTriangle [| left; right; boundary |] [| leftCell.Color; rightCell.Color; boundaryColor |] surfaceTool

        bottomAdder >> topAdder

    /// 处理角
    let triangulateCorner
        bottom
        (bottomCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        (surfaceTool: SurfaceTool)
        =
        let leftEdgeType = bottomCell.GetEdgeType leftCell
        let rightEdgeType = bottomCell.GetEdgeType rightCell

        match leftEdgeType, rightEdgeType with
        | HexEdgeType.Slope, HexEdgeType.Slope ->
            triangulateConerTerraces bottom bottomCell left leftCell right rightCell surfaceTool
        | HexEdgeType.Slope, HexEdgeType.Flat ->
            triangulateConerTerraces left leftCell right rightCell bottom bottomCell surfaceTool
        | HexEdgeType.Slope, _ ->
            triangulateCornerTerracesCliff bottom bottomCell left leftCell right rightCell surfaceTool
        | HexEdgeType.Flat, HexEdgeType.Slope ->
            triangulateConerTerraces right rightCell bottom bottomCell left leftCell surfaceTool
        | _, HexEdgeType.Slope ->
            triangulateCornerCliffTerraces bottom bottomCell left leftCell right rightCell surfaceTool
        | _, _ when leftCell.GetEdgeType rightCell = HexEdgeType.Slope ->
            // CCS
            if leftCell.Elevation < rightCell.Elevation then
                triangulateCornerCliffTerraces right rightCell bottom bottomCell left leftCell surfaceTool
            else
                triangulateCornerTerracesCliff left leftCell right rightCell bottom bottomCell surfaceTool
        | _, _ ->
            addTriangle [| bottom; left; right |] [| bottomCell.Color; leftCell.Color; rightCell.Color |] surfaceTool

    // 处理阶梯边
    let triangulateEdgeTerraces
        beginLeft
        beginRight
        (beginCell: HexCellFS)
        endLeft
        endRight
        (endCell: HexCellFS)
        (surfaceTool: SurfaceTool)
        =
        let v3 = HexMetrics.terraceLerp beginLeft endLeft 1
        let v4 = HexMetrics.terraceLerp beginRight endRight 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color endCell.Color 1

        let quad1 =
            addQuad [| beginLeft; beginRight; v3; v4 |] [| beginCell.Color; beginCell.Color; c2; c2 |] surfaceTool

        let quadsMid, v3, v4, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (quadsAdder, v1, v2, c) i ->
                    let v1' = HexMetrics.terraceLerp beginLeft endLeft i
                    let v2' = HexMetrics.terraceLerp beginRight endRight i
                    let c' = HexMetrics.terraceColorLerp beginCell.Color endCell.Color i

                    let quadsAdder' =
                        quadsAdder >> addQuad [| v1; v2; v1'; v2' |] [| c; c; c'; c' |] surfaceTool

                    quadsAdder', v1', v2', c')
                (id, v3, v4, c2)

        let quadLast =
            addQuad [| v3; v4; endLeft; endRight |] [| c2; c2; endCell.Color; endCell.Color |] surfaceTool

        quad1 >> quadsMid >> quadLast

    let triangulateConnection (surfaceTool: SurfaceTool) (cell: HexCellFS) dir v1 v2 =
        match cell.GetNeighbor dir with
        | None -> id
        | Some neighbor ->
            let bridge = HexMetrics.getBridge dir
            let y = float32 neighbor.Elevation * HexMetrics.elevationStep
            let v3 = v1 + bridge
            let v3 = Vector3(v3.X, y, v3.Z)
            let v4 = v2 + bridge
            let v4 = Vector3(v4.X, y, v4.Z)

            let quadAdder =
                if cell.GetEdgeType dir = Some HexEdgeType.Slope then
                    triangulateEdgeTerraces v1 v2 cell v3 v4 neighbor surfaceTool
                else
                    addQuad
                        [| v1; v2; v3; v4 |]
                        [| cell.Color; cell.Color; neighbor.Color; neighbor.Color |]
                        surfaceTool
            // 避免重复绘制
            if dir > HexDirection.E then
                quadAdder
            else
                match cell.GetNeighbor <| dir.Next() with
                | None -> quadAdder
                | Some nextNeighbor ->
                    let v5 = v2 + (HexMetrics.getBridge <| dir.Next())

                    let v5 =
                        Vector3(v5.X, float32 nextNeighbor.Elevation * HexMetrics.elevationStep, v5.Z)

                    let triAdder =
                        if cell.Elevation <= neighbor.Elevation then
                            if cell.Elevation <= nextNeighbor.Elevation then
                                triangulateCorner v2 cell v4 neighbor v5 nextNeighbor surfaceTool
                            else
                                triangulateCorner v5 nextNeighbor v2 cell v4 neighbor surfaceTool
                        elif neighbor.Elevation <= nextNeighbor.Elevation then
                            triangulateCorner v4 neighbor v5 nextNeighbor v2 cell surfaceTool
                        else
                            triangulateCorner v5 nextNeighbor v2 cell v4 neighbor surfaceTool

                    quadAdder >> triAdder

    let triangulateDir (surfaceTool: SurfaceTool) (cell: HexCellFS) dir =
        let center = cell.Position
        let v1 = center + HexMetrics.getFirstSolidCorner dir
        let v2 = center + HexMetrics.getSecondSolidCorner dir

        let triangleAdder =
            addTriangle [| center; v1; v2 |] (Array.create 3 cell.Color) surfaceTool

        if dir <= HexDirection.SE then
            triangleAdder >> triangulateConnection surfaceTool cell dir v1 v2
        else
            triangleAdder

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
            this.GetChildren() |> Seq.iter _.QueueFree() // 清理之前生成的碰撞体
            this.CreateTrimeshCollision()
