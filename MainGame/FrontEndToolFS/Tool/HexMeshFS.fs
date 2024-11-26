namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot
open Microsoft.FSharp.Core

type HexMeshFS() =
    inherit MeshInstance3D()

    /// 未扰动的三角形
    let addTriangleUnperturbed (triV: Vector3 array) (triC: Color array) (surfaceTool: SurfaceTool) vIndex =
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

    /// 扰动
    let perturb position =
        let sample = HexMetrics.sampleNoise position
        let x = position.X + (sample.X * 2f - 1f) * HexMetrics.cellPerturbStrength
        let y = position.Y // + (sample.Y * 2f - 1f) * HexMetrics.cellPerturbStrength
        let z = position.Z + (sample.Z * 2f - 1f) * HexMetrics.cellPerturbStrength
        Vector3(x, y, z)

    /// 三角形
    let addTriangle (triV: Vector3 array) (triC: Color array) (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.SetColor triC[0]
        surfaceTool.AddVertex <| perturb triV[0]
        surfaceTool.SetColor triC[1]
        surfaceTool.AddVertex <| perturb triV[1]
        surfaceTool.SetColor triC[2]
        surfaceTool.AddVertex <| perturb triV[2]
        // Godot 渲染面方向和 Unity 相反
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 1
        vIndex + 3

    /// 四边形
    let addQuad (triV: Vector3 array) (triC: Color array) (surfaceTool: SurfaceTool) vIndex =
        surfaceTool.SetColor triC[0]
        surfaceTool.AddVertex <| perturb triV[0]
        surfaceTool.SetColor triC[1]
        surfaceTool.AddVertex <| perturb triV[1]
        surfaceTool.SetColor triC[2]
        surfaceTool.AddVertex <| perturb triV[2]
        surfaceTool.SetColor triC[3]
        surfaceTool.AddVertex <| perturb triV[3]
        // Godot 渲染面方向和 Unity 相反
        surfaceTool.AddIndex <| vIndex
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 2
        surfaceTool.AddIndex <| vIndex + 1
        surfaceTool.AddIndex <| vIndex + 3
        vIndex + 4

    /// 三角形纯色数组
    let tri1Color c = Array.create 3 c
    /// 四边形双纯色渐变数组
    let quad2Color c1 c2 = [| c1; c1; c2; c2 |]

    /// 三角形扇形
    let triangulateEdgeFan center (edge: EdgeVertices) color (surfaceTool: SurfaceTool) =
        let colors = tri1Color color

        addTriangle [| center; edge.v1; edge.v2 |] colors surfaceTool
        >> addTriangle [| center; edge.v2; edge.v3 |] colors surfaceTool
        >> addTriangle [| center; edge.v3; edge.v4 |] colors surfaceTool

    /// 四边形折条
    let triangulateEdgeStrip (e1: EdgeVertices) c1 (e2: EdgeVertices) c2 (surfaceTool: SurfaceTool) =
        let colors = quad2Color c1 c2

        addQuad [| e1.v1; e1.v2; e2.v1; e2.v2 |] colors surfaceTool
        >> addQuad [| e1.v2; e1.v3; e2.v2; e2.v3 |] colors surfaceTool
        >> addQuad [| e1.v3; e1.v4; e2.v3; e2.v4 |] colors surfaceTool

    /// SSF 和双斜坡变体 SFS、FSS
    let triangulateCornerTerraces
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

    /// 三角形阶梯
    let triangulateBoundaryTriangle
        beginV
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        boundary
        boundaryColor
        (surfaceTool: SurfaceTool)
        =
        let v2 = perturb <| HexMetrics.terraceLerp beginV left 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color 1

        let tri1 =
            addTriangleUnperturbed
                [| perturb beginV; v2; boundary |]
                [| beginCell.Color; c2; boundaryColor |]
                surfaceTool

        let trisMid, v2, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (trisAdder, v1, c1) i ->
                    let v2' = perturb <| HexMetrics.terraceLerp beginV left i
                    let c2' = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color i

                    let trisAdder' =
                        trisAdder
                        >> addTriangleUnperturbed
                            [| v1; v2'; boundary |]
                            [| c1; c2'; boundaryColor |]
                            surfaceTool

                    trisAdder', v2', c2')
                (id, v2, c2)

        let triLast =
            addTriangleUnperturbed
                [| v2; perturb left; boundary |]
                [| c2; leftCell.Color; boundaryColor |]
                surfaceTool

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
        let boundary = (perturb beginV).Lerp(perturb right, b)
        let boundaryColor = beginCell.Color.Lerp(rightCell.Color, b)

        let bottomAdder =
            triangulateBoundaryTriangle beginV beginCell left leftCell boundary boundaryColor surfaceTool

        let topAdder =
            if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
                triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor surfaceTool
            else
                addTriangleUnperturbed [| perturb left; perturb right; boundary |] [| leftCell.Color; rightCell.Color; boundaryColor |] surfaceTool

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
        let boundary = (perturb beginV).Lerp(perturb left, b)
        let boundaryColor = beginCell.Color.Lerp(leftCell.Color, b)

        let bottomAdder =
            triangulateBoundaryTriangle right rightCell beginV beginCell boundary boundaryColor surfaceTool

        let topAdder =
            if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
                triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor surfaceTool
            else
                addTriangleUnperturbed [| perturb left; perturb right; boundary |] [| leftCell.Color; rightCell.Color; boundaryColor |] surfaceTool

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
            triangulateCornerTerraces bottom bottomCell left leftCell right rightCell surfaceTool
        | HexEdgeType.Slope, HexEdgeType.Flat ->
            triangulateCornerTerraces left leftCell right rightCell bottom bottomCell surfaceTool
        | HexEdgeType.Slope, _ ->
            triangulateCornerTerracesCliff bottom bottomCell left leftCell right rightCell surfaceTool
        | HexEdgeType.Flat, HexEdgeType.Slope ->
            triangulateCornerTerraces right rightCell bottom bottomCell left leftCell surfaceTool
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
    let triangulateEdgeTerraces beginE (beginCell: HexCellFS) endE (endCell: HexCellFS) (surfaceTool: SurfaceTool) =
        let e2 = EdgeVertices.TerraceLerp beginE endE 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color endCell.Color 1

        let strip1 = triangulateEdgeStrip beginE beginCell.Color e2 c2 surfaceTool

        let stripsMid, e2, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (stripsAdder, e1, c1) i ->
                    let e2' = EdgeVertices.TerraceLerp beginE endE i
                    let c2' = HexMetrics.terraceColorLerp beginCell.Color endCell.Color i
                    let stripsAdder' = stripsAdder >> triangulateEdgeStrip e1 c1 e2' c2' surfaceTool
                    stripsAdder', e2', c2')
                (id, e2, c2)

        let stripLast = triangulateEdgeStrip e2 c2 endE endCell.Color surfaceTool

        strip1 >> stripsMid >> stripLast

    let triangulateConnection (surfaceTool: SurfaceTool) (cell: HexCellFS) dir (e1: EdgeVertices) =
        match cell.GetNeighbor dir with
        | None -> id
        | Some neighbor ->
            let bridge = HexMetrics.getBridge dir
            let y = float32 (neighbor.Position.Y - cell.Position.Y)
            let bridge = Vector3(bridge.X, y, bridge.Z)
            let e2 = EdgeVertices(e1.v1 + bridge, e1.v4 + bridge)

            let quadAdder =
                if cell.GetEdgeType dir = Some HexEdgeType.Slope then
                    triangulateEdgeTerraces e1 cell e2 neighbor surfaceTool
                else
                    triangulateEdgeStrip e1 cell.Color e2 neighbor.Color surfaceTool
            // 避免重复绘制
            if dir > HexDirection.E then
                quadAdder
            else
                match cell.GetNeighbor <| dir.Next() with
                | None -> quadAdder
                | Some nextNeighbor ->
                    let v5 = e1.v4 + (HexMetrics.getBridge <| dir.Next())

                    let v5 = Vector3(v5.X, float32 nextNeighbor.Position.Y, v5.Z)

                    let triAdder =
                        if cell.Elevation <= neighbor.Elevation then
                            if cell.Elevation <= nextNeighbor.Elevation then
                                triangulateCorner e1.v4 cell e2.v4 neighbor v5 nextNeighbor surfaceTool
                            else
                                triangulateCorner v5 nextNeighbor e1.v4 cell e2.v4 neighbor surfaceTool
                        elif neighbor.Elevation <= nextNeighbor.Elevation then
                            triangulateCorner e2.v4 neighbor v5 nextNeighbor e1.v4 cell surfaceTool
                        else
                            triangulateCorner v5 nextNeighbor e1.v4 cell e2.v4 neighbor surfaceTool

                    quadAdder >> triAdder

    let triangulateDir (surfaceTool: SurfaceTool) (cell: HexCellFS) dir =
        let center = cell.Position

        let e =
            EdgeVertices(center + HexMetrics.getFirstSolidCorner dir, center + HexMetrics.getSecondSolidCorner dir)

        let fanAdder = triangulateEdgeFan center e cell.Color surfaceTool

        if dir <= HexDirection.SE then
            fanAdder >> triangulateConnection surfaceTool cell dir e
        else
            fanAdder

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
        material.Roughness <- 0.5f
        material.Metallic <- 0.0f
        surfaceTool.SetMaterial(material)
        this.Mesh <- surfaceTool.Commit()
        // 仅在游戏中生成碰撞体（不然编辑器里执行会给场景新增 StaticBody 子节点，以及下面的碰撞体）
        if not <| Engine.IsEditorHint() then
            this.GetChildren() |> Seq.iter _.QueueFree() // 清理之前生成的碰撞体
            this.CreateTrimeshCollision()
