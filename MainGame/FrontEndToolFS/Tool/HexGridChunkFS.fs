namespace FrontEndToolFS.Tool

open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type HexGridChunkFS() as this =
    inherit Node3D()

    let mutable cells: HexCellFS array = null
    let mutable anyCell = false

    /// 置换 Vector3 的 Y
    let changeY (v: Vector3) y = Vector3(v.X, y, v.Z)
    /// 三角形纯色数组
    let tri1Color c = Array.create 3 c
    /// 四边形纯色数组
    let quad1Color c = Array.create 4 c
    /// 四边形双纯色渐变数组
    let quad2Color c1 c2 = [| c1; c1; c2; c2 |]

    /// 四边形 UV
    let quadUV uMin uMax vMin vMax =
        [| Vector2(uMin, vMin)
           Vector2(uMax, vMin)
           Vector2(uMin, vMax)
           Vector2(uMax, vMax) |]

    /// 三角形扇形
    let triangulateEdgeFan center (edge: EdgeVertices) color =
        let colors = tri1Color color
        this.terrain.AddTriangle [| center; edge.v1; edge.v2 |] colors Array.empty
        this.terrain.AddTriangle [| center; edge.v2; edge.v3 |] colors Array.empty
        this.terrain.AddTriangle [| center; edge.v3; edge.v4 |] colors Array.empty
        this.terrain.AddTriangle [| center; edge.v4; edge.v5 |] colors Array.empty

    /// 四边形折条
    let triangulateEdgeStrip (e1: EdgeVertices) c1 (e2: EdgeVertices) c2 =
        let colors = quad2Color c1 c2

        this.terrain.AddQuad [| e1.v1; e1.v2; e2.v1; e2.v2 |] colors Array.empty
        this.terrain.AddQuad [| e1.v2; e1.v3; e2.v2; e2.v3 |] colors Array.empty
        this.terrain.AddQuad [| e1.v3; e1.v4; e2.v3; e2.v4 |] colors Array.empty
        this.terrain.AddQuad [| e1.v4; e1.v5; e2.v4; e2.v5 |] colors Array.empty

    /// SSF 和双斜坡变体 SFS、FSS
    let triangulateCornerTerraces
        beginV
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        =
        let v3 = HexMetrics.terraceLerp beginV left 1
        let v4 = HexMetrics.terraceLerp beginV right 1
        let c3 = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color 1
        let c4 = HexMetrics.terraceColorLerp beginCell.Color rightCell.Color 1
        // 第一个三角形
        this.terrain.AddTriangle [| beginV; v3; v4 |] [| beginCell.Color; c3; c4 |] Array.empty
        // 中间的四边形
        let v3, v4, c3, c4 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (v1, v2, c1, c2) i ->
                    let v3' = HexMetrics.terraceLerp beginV left i
                    let v4' = HexMetrics.terraceLerp beginV right i
                    let c3' = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color i
                    let c4' = HexMetrics.terraceColorLerp beginCell.Color rightCell.Color i

                    this.terrain.AddQuad [| v1; v2; v3'; v4' |] [| c1; c2; c3'; c4' |] Array.empty

                    v3', v4', c3', c4')
                (v3, v4, c3, c4)
        // 最后一个四边形
        this.terrain.AddQuad [| v3; v4; left; right |] [| c3; c4; leftCell.Color; rightCell.Color |] Array.empty

    /// 三角形阶梯
    let triangulateBoundaryTriangle beginV (beginCell: HexCellFS) left (leftCell: HexCellFS) boundary boundaryColor =
        let v2 = HexMetrics.perturb <| HexMetrics.terraceLerp beginV left 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color 1
        // 第一个三角形
        this.terrain.AddTriangleUnperturbed
            [| HexMetrics.perturb beginV; v2; boundary |]
            [| beginCell.Color; c2; boundaryColor |]
            Array.empty
        // 中间的三角形
        let v2, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (v1, c1) i ->
                    let v2' = HexMetrics.perturb <| HexMetrics.terraceLerp beginV left i
                    let c2' = HexMetrics.terraceColorLerp beginCell.Color leftCell.Color i

                    this.terrain.AddTriangleUnperturbed
                        [| v1; v2'; boundary |]
                        [| c1; c2'; boundaryColor |]
                        Array.empty

                    v2', c2')
                (v2, c2)
        // 最后一个三角形
        this.terrain.AddTriangleUnperturbed
            [| v2; HexMetrics.perturb left; boundary |]
            [| c2; leftCell.Color; boundaryColor |]
            Array.empty

    /// 处理 SCS 和 SCC
    let triangulateCornerTerracesCliff
        (beginV: Vector3)
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        =
        let b = 1f / float32 (rightCell.Elevation - beginCell.Elevation) |> Mathf.Abs
        let boundary = (HexMetrics.perturb beginV).Lerp(HexMetrics.perturb right, b)
        let boundaryColor = beginCell.Color.Lerp(rightCell.Color, b)
        // 处理底部
        triangulateBoundaryTriangle beginV beginCell left leftCell boundary boundaryColor
        // 处理顶部
        if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
            triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor
        else
            this.terrain.AddTriangleUnperturbed
                [| HexMetrics.perturb left; HexMetrics.perturb right; boundary |]
                [| leftCell.Color; rightCell.Color; boundaryColor |]
                Array.empty

    /// 处理 CSS 和 CSC
    let triangulateCornerCliffTerraces
        (beginV: Vector3)
        (beginCell: HexCellFS)
        left
        (leftCell: HexCellFS)
        right
        (rightCell: HexCellFS)
        =
        let b = 1f / float32 (leftCell.Elevation - beginCell.Elevation) |> Mathf.Abs
        let boundary = (HexMetrics.perturb beginV).Lerp(HexMetrics.perturb left, b)
        let boundaryColor = beginCell.Color.Lerp(leftCell.Color, b)
        // 处理底部
        triangulateBoundaryTriangle right rightCell beginV beginCell boundary boundaryColor
        // 处理顶部
        if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
            triangulateBoundaryTriangle left leftCell right rightCell boundary boundaryColor
        else
            this.terrain.AddTriangleUnperturbed
                [| HexMetrics.perturb left; HexMetrics.perturb right; boundary |]
                [| leftCell.Color; rightCell.Color; boundaryColor |]
                Array.empty

    /// 处理角
    let triangulateCorner bottom (bottomCell: HexCellFS) left (leftCell: HexCellFS) right (rightCell: HexCellFS) =
        let leftEdgeType = bottomCell.GetEdgeType leftCell
        let rightEdgeType = bottomCell.GetEdgeType rightCell

        match leftEdgeType, rightEdgeType with
        | HexEdgeType.Slope, HexEdgeType.Slope ->
            triangulateCornerTerraces bottom bottomCell left leftCell right rightCell
        | HexEdgeType.Slope, HexEdgeType.Flat ->
            triangulateCornerTerraces left leftCell right rightCell bottom bottomCell
        | HexEdgeType.Slope, _ -> triangulateCornerTerracesCliff bottom bottomCell left leftCell right rightCell
        | HexEdgeType.Flat, HexEdgeType.Slope ->
            triangulateCornerTerraces right rightCell bottom bottomCell left leftCell
        | _, HexEdgeType.Slope -> triangulateCornerCliffTerraces bottom bottomCell left leftCell right rightCell
        | _, _ when leftCell.GetEdgeType rightCell = HexEdgeType.Slope ->
            // CCS
            if leftCell.Elevation < rightCell.Elevation then
                triangulateCornerCliffTerraces right rightCell bottom bottomCell left leftCell
            else
                triangulateCornerTerracesCliff left leftCell right rightCell bottom bottomCell
        | _, _ ->
            this.terrain.AddTriangle
                [| bottom; left; right |]
                [| bottomCell.Color; leftCell.Color; rightCell.Color |]
                Array.empty

    // 处理阶梯边
    let triangulateEdgeTerraces beginE (beginCell: HexCellFS) endE (endCell: HexCellFS) =
        let e2 = EdgeVertices.TerraceLerp beginE endE 1
        let c2 = HexMetrics.terraceColorLerp beginCell.Color endCell.Color 1
        // 第一个条
        triangulateEdgeStrip beginE beginCell.Color e2 c2
        // 中间条
        let e2, c2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (e1, c1) i ->
                    let e2' = EdgeVertices.TerraceLerp beginE endE i
                    let c2' = HexMetrics.terraceColorLerp beginCell.Color endCell.Color i
                    triangulateEdgeStrip e1 c1 e2' c2'
                    e2', c2')
                (e2, c2)
        // 最后一个条
        triangulateEdgeStrip e2 c2 endE endCell.Color

    /// 河面四边形（斜）
    let triangulateRiverQuadSlope (v1: Vector3) (v2: Vector3) (v3: Vector3) (v4: Vector3) y1 y2 v reversed =
        let v1 = changeY v1 y1
        let v2 = changeY v2 y1
        let v3 = changeY v3 y2
        let v4 = changeY v4 y2

        this.rivers.AddQuad
            [| v1; v2; v3; v4 |]
            Array.empty
            (if reversed then quadUV 1f 0f (0.8f - v) (0.6f - v) else quadUV 0f 1f v (v + 0.2f))

    /// 河面四边形（平）
    let triangulateRiverQuadFlat (v1: Vector3) (v2: Vector3) (v3: Vector3) (v4: Vector3) y v reversed =
        triangulateRiverQuadSlope v1 v2 v3 v4 y y v reversed

    /// 处理连接
    let triangulateConnection (cell: HexCellFS) dir (e1: EdgeVertices) =
        match cell.GetNeighbor dir with
        | None -> ()
        | Some neighbor ->
            let bridge = HexMetrics.getBridge dir
            let bridge = changeY bridge <| float32 (neighbor.Position.Y - cell.Position.Y)
            let mutable e2 = EdgeVertices(e1.v1 + bridge, e1.v5 + bridge)

            if cell.HasRiverThroughEdge dir then
                e2 <- e2.ChangeV3 <| changeY e2.v3 neighbor.StreamBedY

                triangulateRiverQuadSlope
                    e1.v2
                    e1.v4
                    e2.v2
                    e2.v4
                    cell.RiverSurfaceY
                    neighbor.RiverSurfaceY
                    0.8f
                    (cell.IncomingRiver = Some dir)

            // 连接条
            if cell.GetEdgeType dir = Some HexEdgeType.Slope then
                triangulateEdgeTerraces e1 cell e2 neighbor
            else
                triangulateEdgeStrip e1 cell.Color e2 neighbor.Color
            // 避免重复绘制
            if dir <= HexDirection.E then
                match cell.GetNeighbor <| dir.Next() with
                | None -> ()
                | Some nextNeighbor ->
                    let v5 = e1.v5 + (HexMetrics.getBridge <| dir.Next())
                    let v5 = changeY v5 (float32 nextNeighbor.Position.Y)
                    // 连接角
                    if cell.Elevation <= neighbor.Elevation then
                        if cell.Elevation <= nextNeighbor.Elevation then
                            triangulateCorner e1.v5 cell e2.v5 neighbor v5 nextNeighbor
                        else
                            triangulateCorner v5 nextNeighbor e1.v5 cell e2.v5 neighbor
                    elif neighbor.Elevation <= nextNeighbor.Elevation then
                        triangulateCorner e2.v5 neighbor v5 nextNeighbor e1.v5 cell
                    else
                        triangulateCorner v5 nextNeighbor e1.v5 cell e2.v5 neighbor

    /// 处理河流
    let triangulateWithRiver (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        let centerL, centerR =
            if cell.HasRiverThroughEdge <| dir.Opposite() then
                center + 0.25f * HexMetrics.getFirstSolidCorner (dir.Previous()),
                center + 0.25f * HexMetrics.getSecondSolidCorner (dir.Next())
            elif cell.HasRiverThroughEdge <| dir.Next() then
                center, center.Lerp(e.v5, 2f / 3f)
            elif cell.HasRiverThroughEdge <| dir.Previous() then
                center.Lerp(e.v1, 2f / 3f), center
            elif cell.HasRiverThroughEdge <| dir.Next2() then
                center,
                center
                + HexMetrics.getSolidEdgeMiddle (dir.Next()) * (0.5f * HexMetrics.innerToOuter)
            else
                center
                + HexMetrics.getSolidEdgeMiddle (dir.Previous())
                  * (0.5f * HexMetrics.innerToOuter),
                center

        let center = centerL.Lerp(centerR, 0.5f)

        let mutable m =
            EdgeVertices(centerL.Lerp(e.v1, 0.5f), centerR.Lerp(e.v5, 0.5f), 1f / 6f)

        m <- m.ChangeV3 <| changeY m.v3 e.v3.Y
        // Unity 的 Vector3 可以修改 x、y、z，但是 Godot 不行
        let center = changeY center e.v3.Y
        // 边缘条
        triangulateEdgeStrip m cell.Color e cell.Color
        // 两边两个三角形和中间两个四边形
        this.terrain.AddTriangle [| centerL; m.v1; m.v2 |] (tri1Color cell.Color) Array.empty
        this.terrain.AddQuad [| centerL; center; m.v2; m.v3 |] (quad1Color cell.Color) Array.empty
        this.terrain.AddQuad [| center; centerR; m.v3; m.v4 |] (quad1Color cell.Color) Array.empty
        this.terrain.AddTriangle [| centerR; m.v4; m.v5 |] (tri1Color cell.Color) Array.empty
        // 河面绘制
        let reversed = cell.IncomingRiver = Some dir
        triangulateRiverQuadFlat centerL centerR m.v2 m.v4 cell.RiverSurfaceY 0.4f reversed
        triangulateRiverQuadFlat m.v2 m.v4 e.v2 e.v4 cell.RiverSurfaceY 0.6f reversed

    /// 处理河流开始和结束
    let triangulateWithRiverBeginOrEnd (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        let mutable m = EdgeVertices(center.Lerp(e.v1, 0.5f), center.Lerp(e.v5, 0.5f))
        m <- m.ChangeV3 <| changeY m.v3 e.v3.Y
        triangulateEdgeStrip m cell.Color e cell.Color
        triangulateEdgeFan center m cell.Color
        let reversed = cell.IncomingRiver.IsSome
        triangulateRiverQuadFlat m.v2 m.v4 e.v2 e.v4 cell.RiverSurfaceY 0.6f reversed
        let center = changeY center cell.RiverSurfaceY
        m <- EdgeVertices(m.v1, changeY m.v2 cell.RiverSurfaceY, m.v3, changeY m.v4 cell.RiverSurfaceY, m.v5)

        this.rivers.AddTriangle
            [| center; m.v2; m.v4 |]
            Array.empty
            (if reversed then
                 [| Vector2(0.5f, 0.4f); Vector2(1f, 0.2f); Vector2(0f, 0.2f) |]
             else
                 [| Vector2(0.5f, 0.4f); Vector2(0f, 0.6f); Vector2(1f, 0.6f) |])

    /// 处理河流流域
    let triangulateAdjacentToRiver (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        let center =
            if cell.HasRiverThroughEdge <| dir.Next() then
                if cell.HasRiverThroughEdge <| dir.Previous() then
                    center + HexMetrics.getSolidEdgeMiddle dir * (0.5f * HexMetrics.innerToOuter)
                elif cell.HasRiverThroughEdge <| dir.Previous2() then
                    center + HexMetrics.getFirstSolidCorner dir * 0.25f
                else
                    center
            elif
                cell.HasRiverThroughEdge <| dir.Previous()
                && cell.HasRiverThroughEdge <| dir.Next2()
            then
                center + HexMetrics.getSecondSolidCorner dir * 0.25f
            else
                center

        let m = EdgeVertices(center.Lerp(e.v1, 0.5f), center.Lerp(e.v5, 0.5f))
        triangulateEdgeStrip m cell.Color e cell.Color
        triangulateEdgeFan center m cell.Color

    let triangulateDir (cell: HexCellFS) dir =
        let center = cell.Position

        let mutable e =
            EdgeVertices(center + HexMetrics.getFirstSolidCorner dir, center + HexMetrics.getSecondSolidCorner dir)

        if cell.HasRiver then
            if cell.HasRiverThroughEdge dir then
                e <- e.ChangeV3 <| Vector3(e.v3.X, cell.StreamBedY, e.v3.Z)

                if cell.HasRiverBeginOrEnd then
                    triangulateWithRiverBeginOrEnd dir cell center e
                else
                    triangulateWithRiver dir cell center e
            else
                triangulateAdjacentToRiver dir cell center e
        else
            triangulateEdgeFan center e cell.Color

        if dir <= HexDirection.SE then
            triangulateConnection cell dir e

    let triangulate (cell: HexCellFS) =
        allHexDirs () |> List.iter (triangulateDir cell)


    [<DefaultValue>]
    val mutable terrain: HexMeshFS

    [<DefaultValue>]
    val mutable rivers: HexMeshFS

    interface IChunk with
        member this.Refresh() =
            // GD.Print $"{this.Name} Refresh"
            this.SetProcess true

    member this.AddCell index cell =
        anyCell <- true
        cells[index] <- cell
        cell.Chunk <- this
        this.AddChild cell

    override this._Ready() =
        cells <- Array.zeroCreate (HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ)

    override this._Process _ =
        // 防止编辑器内循环报错，卡死
        if anyCell then
            this.terrain.Clear()
            this.rivers.Clear()
            cells |> Array.iter triangulate
            this.terrain.Apply()
            this.rivers.Apply()
        // 这里写法挺有意思，可以控制 _Process 不频繁调用
        this.SetProcess false
