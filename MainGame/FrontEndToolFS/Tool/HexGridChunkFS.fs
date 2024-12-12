namespace FrontEndToolFS.Tool

open FrontEndCommonFS.Util
open FrontEndToolFS.HexPlane
open FrontEndToolFS.HexPlane.HexDirection
open Godot

type HexGridChunkFS() as this =
    inherit Node3D()

    let _gridCanvas = lazy this.GetNode<Node3D> "HexGridCanvas"

    let mutable cells: HexCellFS array = null
    let mutable anyCell = false
    let weights1 = Colors.Red
    let weights2 = Colors.Green
    let weights3 = Colors.Blue
    /// 三角形纯色数组
    let tri1Arr c = Array.create 3 c
    /// 四边形纯色数组
    let quad1Arr c = Array.create 4 c
    /// 四边形双纯色渐变数组
    let quad2Arr c1 c2 = [| c1; c1; c2; c2 |]

    /// 四边形 UV
    let quadUV uMin uMax vMin vMax =
        [| Vector2(uMin, vMin)
           Vector2(uMax, vMin)
           Vector2(uMin, vMax)
           Vector2(uMax, vMax) |]

    /// 道路段
    let triangulateRoadSegment v1 v2 v3 v4 v5 v6 w1 w2 indices =
        this.roads.AddQuad([| v1; v2; v4; v5 |], quad2Arr w1 w2, quadUV 0f 1f 0f 0f, ci = indices)
        this.roads.AddQuad([| v2; v3; v5; v6 |], quad2Arr w1 w2, quadUV 1f 0f 0f 0f, ci = indices)

    /// 道路边缘
    let triangulateRoadEdge center mL mR (index: int) =
        let indices = Vector3.One * float32 index

        this.roads.AddTriangle(
            cw = tri1Arr weights1,
            vs = [| center; mL; mR |],
            uvs = [| Vector2(1f, 0f); Vector2(0f, 0f); Vector2(0f, 0f) |],
            ci = indices
        )

    /// 道路
    let triangulateRoad center (mL: Vector3) mR (e: EdgeVertices) hasRoadThroughCellEdge (index: int) =
        if hasRoadThroughCellEdge then
            let indices = Vector3.One * float32 index
            let mC = mL.Lerp(mR, 0.5f)
            triangulateRoadSegment mL mC mR e.v2 e.v3 e.v4 weights1 weights1 indices

            this.roads.AddTriangle(
                [| center; mL; mC |],
                tri1Arr weights1,
                uvs = [| Vector2(1f, 0f); Vector2(0f, 0f); Vector2(1f, 0f) |],
                ci = indices
            )

            this.roads.AddTriangle(
                [| center; mC; mR |],
                tri1Arr weights1,
                uvs = [| Vector2(1f, 0f); Vector2(1f, 0f); Vector2(0f, 0f) |],
                ci = indices
            )
        else
            triangulateRoadEdge center mL mR index

    /// 道路插值器
    let getRoadInterpolator dir (cell: HexCellFS) =
        if cell.HasRoadThroughEdge dir then
            Vector2(0.5f, 0.5f)
        else
            let x =
                if cell.HasRoadThroughEdge(dir.Previous()) then
                    0.5f
                else
                    0.25f

            let y = if cell.HasRoadThroughEdge(dir.Next()) then 0.5f else 0.25f
            Vector2(x, y)

    /// 三角形扇形
    let triangulateEdgeFan center (edge: EdgeVertices) index =
        let weights = tri1Arr weights1
        let indices = Vector3.One * float32 index
        this.terrain.AddTriangle([| center; edge.v1; edge.v2 |], weights, ci = indices)
        this.terrain.AddTriangle([| center; edge.v2; edge.v3 |], weights, ci = indices)
        this.terrain.AddTriangle([| center; edge.v3; edge.v4 |], weights, ci = indices)
        this.terrain.AddTriangle([| center; edge.v4; edge.v5 |], weights, ci = indices)

    /// 四边形折条
    let triangulateEdgeStrip (e1: EdgeVertices) w1 index1 (e2: EdgeVertices) w2 index2 (hasRoad: bool) =
        let weights = quad2Arr w1 w2
        let indices = Vector3(float32 index1, float32 index2, float32 index1)
        this.terrain.AddQuad([| e1.v1; e1.v2; e2.v1; e2.v2 |], weights, ci = indices)
        this.terrain.AddQuad([| e1.v2; e1.v3; e2.v2; e2.v3 |], weights, ci = indices)
        this.terrain.AddQuad([| e1.v3; e1.v4; e2.v3; e2.v4 |], weights, ci = indices)
        this.terrain.AddQuad([| e1.v4; e1.v5; e2.v4; e2.v5 |], weights, ci = indices)

        if hasRoad then
            triangulateRoadSegment e1.v2 e1.v3 e1.v4 e2.v2 e2.v3 e2.v4 w1 w2 indices

    let triangulateEdgeStripNoRoad (e1: EdgeVertices) w1 index1 (e2: EdgeVertices) w2 index2 =
        triangulateEdgeStrip e1 w1 index1 e2 w2 index2 false

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
        let w3 = HexMetrics.terraceColorLerp weights1 weights2 1
        let w4 = HexMetrics.terraceColorLerp weights1 weights3 1

        let indices =
            Vector3(float32 beginCell.Index, float32 leftCell.Index, float32 rightCell.Index)
        // 第一个三角形
        this.terrain.AddTriangle([| beginV; v3; v4 |], [| weights1; w3; w4 |], ci = indices)
        // 中间的四边形
        let v3, v4, w3, w4 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (v1, v2, w1, w2) i ->
                    let v3' = HexMetrics.terraceLerp beginV left i
                    let v4' = HexMetrics.terraceLerp beginV right i
                    let w3' = HexMetrics.terraceColorLerp weights1 weights2 i
                    let w4' = HexMetrics.terraceColorLerp weights1 weights3 i

                    this.terrain.AddQuad([| v1; v2; v3'; v4' |], [| w1; w2; w3'; w4' |], ci = indices)

                    v3', v4', w3', w4')
                (v3, v4, w3, w4)
        // 最后一个四边形
        this.terrain.AddQuad([| v3; v4; left; right |], [| w3; w4; weights2; weights3 |], ci = indices)

    /// 三角形阶梯
    let triangulateBoundaryTriangle beginV beginWeights left leftWeights boundary boundaryWeights indices =
        let v2 = HexMetrics.perturb <| HexMetrics.terraceLerp beginV left 1
        let w2 = HexMetrics.terraceColorLerp beginWeights leftWeights 1
        // 第一个三角形
        this.terrain.AddTriangleUnperturbed(
            [| HexMetrics.perturb beginV; v2; boundary |],
            [| beginWeights; w2; boundaryWeights |],
            ci = indices
        )
        // 中间的三角形
        let v2, w2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (v1, w1) i ->
                    let v2' = HexMetrics.perturb <| HexMetrics.terraceLerp beginV left i
                    let w2' = HexMetrics.terraceColorLerp beginWeights leftWeights i

                    this.terrain.AddTriangleUnperturbed(
                        [| v1; v2'; boundary |],
                        [| w1; w2'; boundaryWeights |],
                        ci = indices
                    )

                    v2', w2')
                (v2, w2)
        // 最后一个三角形
        this.terrain.AddTriangleUnperturbed(
            [| v2; HexMetrics.perturb left; boundary |],
            [| w2; leftWeights; boundaryWeights |],
            ci = indices
        )

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
        let boundaryWeights = weights1.Lerp(weights3, b)

        let indices =
            Vector3(float32 beginCell.Index, float32 leftCell.Index, float32 rightCell.Index)
        // 处理底部
        triangulateBoundaryTriangle beginV weights1 left weights2 boundary boundaryWeights indices
        // 处理顶部
        if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
            triangulateBoundaryTriangle left weights2 right weights3 boundary boundaryWeights indices
        else
            this.terrain.AddTriangleUnperturbed(
                [| HexMetrics.perturb left; HexMetrics.perturb right; boundary |],
                [| weights2; weights3; boundaryWeights |],
                ci = indices
            )

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
        let boundaryWeights = weights1.Lerp(weights2, b)

        let indices =
            Vector3(float32 beginCell.Index, float32 leftCell.Index, float32 rightCell.Index)
        // 处理底部
        triangulateBoundaryTriangle right weights3 beginV weights1 boundary boundaryWeights indices
        // 处理顶部
        if leftCell.GetEdgeType rightCell = HexEdgeType.Slope then
            triangulateBoundaryTriangle left weights2 right weights3 boundary boundaryWeights indices
        else
            this.terrain.AddTriangleUnperturbed(
                [| HexMetrics.perturb left; HexMetrics.perturb right; boundary |],
                [| weights2; weights3; boundaryWeights |],
                ci = indices
            )

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
            let indices =
                Vector3(float32 bottomCell.Index, float32 leftCell.Index, float32 rightCell.Index)

            this.terrain.AddTriangle([| bottom; left; right |], [| weights1; weights2; weights3 |], ci = indices)

        this.features.AddWall2 bottom bottomCell left leftCell right rightCell

    // 处理阶梯边
    let triangulateEdgeTerraces beginE (beginCell: HexCellFS) endE (endCell: HexCellFS) hasRoad =
        let e2 = EdgeVertices.TerraceLerp beginE endE 1
        let w2 = HexMetrics.terraceColorLerp weights1 weights2 1
        let i1 = beginCell.Index
        let i2 = endCell.Index
        // 第一个条
        triangulateEdgeStrip beginE weights1 i1 e2 w2 i2 hasRoad
        // 中间条
        let e2, w2 =
            [ 2 .. HexMetrics.terraceSteps - 1 ]
            |> List.fold
                (fun (e1, w1) i ->
                    let e2' = EdgeVertices.TerraceLerp beginE endE i
                    let w2' = HexMetrics.terraceColorLerp weights1 weights2 i
                    triangulateEdgeStrip e1 w1 i1 e2' w2' i2 hasRoad
                    e2', w2')
                (e2, w2)
        // 最后一个条
        triangulateEdgeStrip e2 w2 i1 endE weights2 i2 hasRoad

    /// 河面四边形（斜）
    let triangulateRiverQuadSlope (v1: Vector3) (v2: Vector3) (v3: Vector3) (v4: Vector3) y1 y2 v reversed indices =
        let v1 = Vector3Util.changeY v1 y1
        let v2 = Vector3Util.changeY v2 y1
        let v3 = Vector3Util.changeY v3 y2
        let v4 = Vector3Util.changeY v4 y2

        this.rivers.AddQuad(
            [| v1; v2; v3; v4 |],
            quad2Arr weights1 weights2,
            (if reversed then
                 quadUV 1f 0f (0.8f - v) (0.6f - v)
             else
                 quadUV 0f 1f v (v + 0.2f)),
            ci = indices
        )

    /// 河面四边形（平）
    let triangulateRiverQuadFlat (v1: Vector3) (v2: Vector3) (v3: Vector3) (v4: Vector3) y v reversed indices =
        triangulateRiverQuadSlope v1 v2 v3 v4 y y v reversed indices

    /// 处理流入水中的瀑布
    let triangulateWaterfallInWater v1 v2 v3 v4 y1 y2 waterY indices =
        let v1 = HexMetrics.perturb <| Vector3Util.changeY v1 y1
        let v2 = HexMetrics.perturb <| Vector3Util.changeY v2 y1
        let v3 = HexMetrics.perturb <| Vector3Util.changeY v3 y2
        let v4 = HexMetrics.perturb <| Vector3Util.changeY v4 y2
        let t = (waterY - y2) / (y1 - y2)
        let v3 = v3.Lerp(v1, t)
        let v4 = v4.Lerp(v2, t)

        this.rivers.AddQuadUnperturbed(
            [| v1; v2; v3; v4 |],
            quad2Arr weights1 weights2,
            quadUV 0f 1f 0.8f 1f,
            ci = indices
        )

    /// 处理连接
    let triangulateConnection (cell: HexCellFS) dir (e1: EdgeVertices) =
        match cell.GetNeighbor dir with
        | None -> ()
        | Some neighbor ->
            let bridge = HexMetrics.getBridge dir

            let bridge =
                Vector3Util.changeY bridge <| float32 (neighbor.Position.Y - cell.Position.Y)

            let mutable e2 = EdgeVertices(e1.v1 + bridge, e1.v5 + bridge)

            let hasRiver = cell.HasRiverThroughEdge dir
            let hasRoad = cell.HasRoadThroughEdge dir

            if hasRiver then
                e2 <- e2.ChangeV3 <| Vector3Util.changeY e2.v3 neighbor.StreamBedY

                let indices =
                    Vector3(float32 cell.Index, float32 neighbor.Index, float32 cell.Index)

                if not cell.IsUnderWater then
                    if not neighbor.IsUnderWater then
                        triangulateRiverQuadSlope
                            e1.v2
                            e1.v4
                            e2.v2
                            e2.v4
                            cell.RiverSurfaceY
                            neighbor.RiverSurfaceY
                            0.8f
                            (cell.IncomingRiver = Some dir)
                            indices
                    elif cell.Elevation > neighbor.WaterLevel then
                        triangulateWaterfallInWater
                            e1.v2
                            e1.v4
                            e2.v2
                            e2.v4
                            cell.RiverSurfaceY
                            neighbor.RiverSurfaceY
                            neighbor.WaterSurfaceY
                            indices
                elif not neighbor.IsUnderWater && neighbor.Elevation > cell.WaterLevel then
                    triangulateWaterfallInWater
                        e2.v4
                        e2.v2
                        e1.v4
                        e1.v2
                        neighbor.WaterSurfaceY
                        cell.WaterSurfaceY
                        cell.WaterSurfaceY
                        indices

            // 连接条
            if cell.GetEdgeType dir = Some HexEdgeType.Slope then
                triangulateEdgeTerraces e1 cell e2 neighbor hasRoad
            else
                triangulateEdgeStrip e1 weights1 cell.Index e2 weights2 neighbor.Index hasRoad

            this.features.AddWall e1 cell e2 neighbor hasRiver hasRoad
            // 避免重复绘制
            if dir <= HexDirection.E then
                match cell.GetNeighbor <| dir.Next() with
                | None -> ()
                | Some nextNeighbor ->
                    let v5 = e1.v5 + (HexMetrics.getBridge <| dir.Next())
                    let v5 = Vector3Util.changeY v5 (float32 nextNeighbor.Position.Y)
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

        m <- m.ChangeV3 <| Vector3Util.changeY m.v3 e.v3.Y
        // Unity 的 Vector3 可以修改 x、y、z，但是 Godot 不行
        let center = Vector3Util.changeY center e.v3.Y
        // 边缘条
        triangulateEdgeStripNoRoad m weights1 cell.Index e weights1 cell.Index
        // 两边两个三角形和中间两个四边形
        let indices = Vector3.One * float32 cell.Index
        this.terrain.AddTriangle([| centerL; m.v1; m.v2 |], tri1Arr weights1, ci = indices)
        this.terrain.AddQuad([| centerL; center; m.v2; m.v3 |], quad1Arr weights1, ci = indices)
        this.terrain.AddQuad([| center; centerR; m.v3; m.v4 |], quad1Arr weights1, ci = indices)
        this.terrain.AddTriangle([| centerR; m.v4; m.v5 |], tri1Arr weights1, ci = indices)
        // 河面绘制
        if not cell.IsUnderWater then
            let reversed = cell.IncomingRiver = Some dir
            triangulateRiverQuadFlat centerL centerR m.v2 m.v4 cell.RiverSurfaceY 0.4f reversed indices
            triangulateRiverQuadFlat m.v2 m.v4 e.v2 e.v4 cell.RiverSurfaceY 0.6f reversed indices

    /// 处理河流开始和结束
    let triangulateWithRiverBeginOrEnd (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        let mutable m = EdgeVertices(center.Lerp(e.v1, 0.5f), center.Lerp(e.v5, 0.5f))
        m <- m.ChangeV3 <| Vector3Util.changeY m.v3 e.v3.Y
        triangulateEdgeStripNoRoad m weights1 cell.Index e weights1 cell.Index
        triangulateEdgeFan center m cell.Index
        // 河面绘制
        if not cell.IsUnderWater then
            let reversed = cell.IncomingRiver.IsSome
            let indices = Vector3.One * float32 cell.Index
            triangulateRiverQuadFlat m.v2 m.v4 e.v2 e.v4 cell.RiverSurfaceY 0.6f reversed indices
            let center = Vector3Util.changeY center cell.RiverSurfaceY

            m <-
                EdgeVertices(
                    m.v1,
                    Vector3Util.changeY m.v2 cell.RiverSurfaceY,
                    m.v3,
                    Vector3Util.changeY m.v4 cell.RiverSurfaceY,
                    m.v5
                )

            this.rivers.AddTriangle(
                [| center; m.v2; m.v4 |],
                tri1Arr weights1,
                (if reversed then
                     [| Vector2(0.5f, 0.4f); Vector2(1f, 0.2f); Vector2(0f, 0.2f) |]
                 else
                     [| Vector2(0.5f, 0.4f); Vector2(0f, 0.6f); Vector2(1f, 0.6f) |]),
                ci = indices
            )

    /// 处理有道路的河流流域
    let triangulateRoadAdjacentToRiver (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        let hasRoadThroughEdge = cell.HasRoadThroughEdge dir
        let previousHasRiver = cell.HasRiverThroughEdge <| dir.Previous()
        let nextHasRiver = cell.HasRiverThroughEdge <| dir.Next()
        let interpolator = getRoadInterpolator dir cell

        let roadCenter, center, prune =
            if cell.HasRiverBeginOrEnd then
                center
                + HexMetrics.getSolidEdgeMiddle (cell.RiverBeginOrEndDirection.Opposite())
                  * (1f / 3f),
                center,
                false
            // 前提保证了这里 IncomingRiver 和 OutgoingRiver 一定是 Some
            elif cell.IncomingRiver.Value = cell.OutgoingRiver.Value.Opposite() then
                let corner, prune =
                    if previousHasRiver then
                        let prune = not hasRoadThroughEdge && not << cell.HasRoadThroughEdge <| dir.Next()
                        HexMetrics.getSecondSolidCorner dir, prune
                    else
                        let prune =
                            not hasRoadThroughEdge && not << cell.HasRoadThroughEdge <| dir.Previous()

                        HexMetrics.getFirstSolidCorner dir, prune

                let roadCenter = center + corner * 0.5f

                if
                    not prune
                    && cell.IncomingRiver.Value = dir.Next()
                    && (cell.HasRoadThroughEdge <| dir.Next2()
                        || cell.HasRoadThroughEdge <| dir.Opposite())
                then
                    this.features.AddBridge roadCenter <| center - corner * 0.5f

                roadCenter, center + corner * 0.25f, prune
            elif cell.IncomingRiver.Value = cell.OutgoingRiver.Value.Previous() then
                center - HexMetrics.getSecondSolidCorner cell.IncomingRiver.Value * 0.2f, center, false
            elif cell.IncomingRiver.Value = cell.OutgoingRiver.Value.Next() then
                center - HexMetrics.getFirstSolidCorner cell.IncomingRiver.Value * 0.2f, center, false
            elif previousHasRiver && nextHasRiver then
                let offset = HexMetrics.getSolidEdgeMiddle dir * HexMetrics.innerToOuter
                center + offset * 0.7f, center + offset * 0.5f, not hasRoadThroughEdge
            else
                let middle =
                    if previousHasRiver then dir.Next()
                    elif nextHasRiver then dir.Previous()
                    else dir

                let prune =
                    not <| cell.HasRoadThroughEdge middle
                    && not << cell.HasRoadThroughEdge <| middle.Previous()
                    && not << cell.HasRoadThroughEdge <| middle.Next()

                let offset = HexMetrics.getSolidEdgeMiddle middle
                let roadCenter = if prune then center else center + offset * 0.25f

                if not prune && dir = middle && cell.HasRoadThroughEdge <| dir.Opposite() then
                    this.features.AddBridge roadCenter
                    <| center - offset * HexMetrics.innerToOuter * 0.7f

                roadCenter, center, prune
        // 如果需要修剪跨河对面的道路残渣，后面就不需要执行了
        if not prune then
            let mL = roadCenter.Lerp(e.v1, interpolator.X)
            let mR = roadCenter.Lerp(e.v5, interpolator.Y)
            triangulateRoad roadCenter mL mR e hasRoadThroughEdge cell.Index

            if previousHasRiver then
                triangulateRoadEdge roadCenter center mL cell.Index

            if nextHasRiver then
                triangulateRoadEdge roadCenter mR center cell.Index

    /// 处理河流流域
    let triangulateAdjacentToRiver (dir: HexDirection) (cell: HexCellFS) (center: Vector3) (e: EdgeVertices) =
        if cell.HasRoads then
            triangulateRoadAdjacentToRiver dir cell center e

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
        triangulateEdgeStripNoRoad m weights1 cell.Index e weights1 cell.Index
        triangulateEdgeFan center m cell.Index

        if not cell.IsUnderWater && not <| cell.HasRoadThroughEdge dir then
            this.features.AddFeature cell <| (center + e.v1 + e.v5) * (1f / 3f)

    /// 处理没有河流的单元格
    let triangulateWithoutRiver dir (cell: HexCellFS) center e =
        triangulateEdgeFan center e cell.Index

        if cell.HasRoads then
            let interpolator = getRoadInterpolator dir cell

            triangulateRoad center
            <| center.Lerp(e.v1, interpolator.X)
            <| center.Lerp(e.v5, interpolator.Y)
            <| e
            <| cell.HasRoadThroughEdge dir
            <| cell.Index

    /// 处理河口
    let triangulateEstuary (e1: EdgeVertices) (e2: EdgeVertices) isIncomingRiver indices =
        this.waterShore.AddTriangle(
            [| e2.v1; e1.v2; e1.v1 |],
            [| weights2; weights1; weights1 |],
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(0f, 0f) |],
            ci = indices
        )

        this.waterShore.AddTriangle(
            [| e2.v5; e1.v5; e1.v4 |],
            [| weights2; weights1; weights1 |],
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(0f, 0f) |],
            ci = indices
        )

        this.estuaries.AddQuad(
            [| e2.v1; e1.v2; e2.v2; e1.v3 |],
            quad2Arr weights2 weights1,
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(1f, 1f); Vector2(0f, 0f) |],
            (if isIncomingRiver then
                 [| Vector2(1.5f, 1f)
                    Vector2(0.7f, 1.15f)
                    Vector2(1f, 0.8f)
                    Vector2(0.5f, 1.1f) |]
             else
                 [| Vector2(-0.5f, -0.2f)
                    Vector2(0.3f, -0.35f)
                    Vector2(0f, 0f)
                    Vector2(0.5f, -0.3f) |]),
            ci = indices
        )

        this.estuaries.AddTriangle(
            [| e1.v3; e2.v2; e2.v4 |],
            [| weights1; weights2; weights2 |],
            [| Vector2(0f, 0f); Vector2(1f, 1f); Vector2(1f, 1f) |],
            (if isIncomingRiver then
                 [| Vector2(0.5f, 1.1f); Vector2(1f, 0.8f); Vector2(0f, 0.8f) |]
             else
                 [| Vector2(0.5f, -0.3f); Vector2(0f, 0f); Vector2(1f, 0f) |]),
            ci = indices
        )

        this.estuaries.AddQuad(
            [| e1.v3; e1.v4; e2.v4; e2.v5 |],
            quad2Arr weights1 weights2,
            [| Vector2(0f, 0f); Vector2(0f, 0f); Vector2(1f, 1f); Vector2(1f, 1f) |],
            (if isIncomingRiver then
                 [| Vector2(0.5f, 1.1f)
                    Vector2(0.3f, 1.15f)
                    Vector2(0f, 0.8f)
                    Vector2(-0.5f, 1f) |]
             else
                 [| Vector2(0.5f, -0.3f)
                    Vector2(0.7f, -0.35f)
                    Vector2(1f, 0f)
                    Vector2(-1.5f, -0.2f) |]),
            ci = indices
        )

    /// 处理岸边水域
    let triangulateWaterShore dir (cell: HexCellFS) (neighbor: HexCellFS) center =
        let e1 =
            EdgeVertices(center + HexMetrics.getFirstWaterCorner dir, center + HexMetrics.getSecondWaterCorner dir)

        let indices =
            Vector3(float32 cell.Index, float32 neighbor.Index, float32 cell.Index)

        this.water.AddTriangle([| center; e1.v1; e1.v2 |], tri1Arr weights1, ci = indices)
        this.water.AddTriangle([| center; e1.v2; e1.v3 |], tri1Arr weights1, ci = indices)
        this.water.AddTriangle([| center; e1.v3; e1.v4 |], tri1Arr weights1, ci = indices)
        this.water.AddTriangle([| center; e1.v4; e1.v5 |], tri1Arr weights1, ci = indices)
        let center2 = Vector3Util.changeY neighbor.Position center.Y

        let e2 =
            EdgeVertices(
                center2 + (HexMetrics.getSecondSolidCorner <| dir.Opposite()),
                center2 + (HexMetrics.getFirstSolidCorner <| dir.Opposite())
            )

        if cell.HasRiverThroughEdge dir then
            triangulateEstuary e1 e2 (cell.IncomingRiver = Some dir) indices
        else
            this.waterShore.AddQuad(
                [| e1.v1; e1.v2; e2.v1; e2.v2 |],
                quad2Arr weights1 weights2,
                quadUV 0f 0f 0f 1f,
                ci = indices
            )

            this.waterShore.AddQuad(
                [| e1.v2; e1.v3; e2.v2; e2.v3 |],
                quad2Arr weights1 weights2,
                quadUV 0f 0f 0f 1f,
                ci = indices
            )

            this.waterShore.AddQuad(
                [| e1.v3; e1.v4; e2.v3; e2.v4 |],
                quad2Arr weights1 weights2,
                quadUV 0f 0f 0f 1f,
                ci = indices
            )

            this.waterShore.AddQuad(
                [| e1.v4; e1.v5; e2.v4; e2.v5 |],
                quad2Arr weights1 weights2,
                quadUV 0f 0f 0f 1f,
                ci = indices
            )

        match cell.GetNeighbor <| dir.Next() with
        | Some nextNeighbor ->
            let v3 =
                nextNeighbor.Position
                + if nextNeighbor.IsUnderWater then
                      HexMetrics.getFirstWaterCorner <| dir.Previous()
                  else
                      HexMetrics.getFirstSolidCorner <| dir.Previous()

            let v3 = Vector3Util.changeY v3 center.Y
            let indices = Vector3(indices.X, indices.Y, float32 nextNeighbor.Index)

            this.waterShore.AddTriangle(
                [| e1.v5; e2.v5; v3 |],
                [| weights1; weights2; weights3 |],
                [| Vector2(0f, 0f)
                   Vector2(0f, 1f)
                   Vector2(0f, (if nextNeighbor.IsUnderWater then 0f else 1f)) |],
                ci = indices
            )
        | _ -> ()


    /// 处理开放水域
    let triangulateOpenWater dir (cell: HexCellFS) (neighborOpt: HexCellFS option) center =
        let c1 = center + HexMetrics.getFirstWaterCorner dir
        let c2 = center + HexMetrics.getSecondWaterCorner dir
        let indices = Vector3.One * float32 cell.Index
        this.water.AddTriangle([| center; c1; c2 |], tri1Arr weights1, ci = indices)

        if dir <= HexDirection.SE && neighborOpt.IsSome then
            let bridge = HexMetrics.getWaterBridge dir
            let e1 = c1 + bridge
            let e2 = c2 + bridge
            let indices = Vector3Util.changeY indices <| float32 neighborOpt.Value.Index
            this.water.AddQuad([| c1; c2; e1; e2 |], quad2Arr weights1 weights2, ci = indices)

            if dir <= HexDirection.E then
                match cell.GetNeighbor <| dir.Next() with
                | Some nextNeighbor when nextNeighbor.IsUnderWater ->
                    let indices = Vector3(indices.X, indices.Y, float32 nextNeighbor.Index)

                    this.water.AddTriangle(
                        [| c2; e2; c2 + (HexMetrics.getWaterBridge <| dir.Next()) |],
                        [| weights1; weights2; weights3 |],
                        ci = indices
                    )
                | _ -> ()

    /// 处理水
    let triangulateWater dir (cell: HexCellFS) center =
        let center = Vector3Util.changeY center cell.WaterSurfaceY

        match cell.GetNeighbor dir with
        | Some neighbor when not neighbor.IsUnderWater -> triangulateWaterShore dir cell neighbor center
        | neighborOpt -> triangulateOpenWater dir cell neighborOpt center

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
            triangulateWithoutRiver dir cell center e

            if not cell.IsUnderWater && not <| cell.HasRoadThroughEdge dir then
                this.features.AddFeature cell <| (center + e.v1 + e.v5) * (1f / 3f)

        if dir <= HexDirection.SE then
            triangulateConnection cell dir e

        if cell.IsUnderWater then
            triangulateWater dir cell center

    let triangulate (cell: HexCellFS) =
        allHexDirs () |> List.iter (triangulateDir cell)

        if not cell.IsUnderWater then
            if not cell.HasRiver && not cell.HasRoads then
                this.features.AddFeature cell cell.Position

            if cell.IsSpecial then
                this.features.AddSpecialFeature cell cell.Position

    /// 地形
    [<DefaultValue>]
    val mutable terrain: HexMeshFS

    /// 河流
    [<DefaultValue>]
    val mutable rivers: HexMeshFS

    /// 道路
    [<DefaultValue>]
    val mutable roads: HexMeshFS

    /// 水面
    [<DefaultValue>]
    val mutable water: HexMeshFS

    /// 近岸水面
    [<DefaultValue>]
    val mutable waterShore: HexMeshFS

    /// 河口
    [<DefaultValue>]
    val mutable estuaries: HexMeshFS

    /// 特征
    [<DefaultValue>]
    val mutable features: HexFeatureManagerFS

    interface IChunk with
        override this.Refresh() = this.Refresh()

    // F# 接口方法只能通过接口调用，通过这样把普通对象的 Refresh() 暴露直接使用
    member this.Refresh() =
        // GD.Print $"{this.Name} Refresh"
        this.SetProcess true

    member this.AddCell index cell =
        anyCell <- true
        cells[index] <- cell
        cell.Chunk <- this
        this.AddChild cell
        _gridCanvas.Value.AddChild cell.uiRect

    member this.ShowGrid(visible: bool) =
        (this.terrain.MaterialOverride :?> ShaderMaterial)
            .SetShaderParameter("grid_on", visible)

    member this.ShowUI visible = _gridCanvas.Value.Visible <- visible

    override this._Ready() =
        cells <- Array.zeroCreate <| HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ

    override this._Process _ =
        // 防止编辑器内循环报错，卡死
        if anyCell then
            this.terrain.Clear()
            this.rivers.Clear()
            this.roads.Clear()
            this.water.Clear()
            this.waterShore.Clear()
            this.estuaries.Clear()
            this.features.Clear()
            cells |> Array.iter triangulate
            this.terrain.Apply()
            this.rivers.Apply()
            this.roads.Apply()
            this.water.Apply()
            this.waterShore.Apply()
            this.estuaries.Apply()
            this.features.Apply()
        // 这里写法挺有意思，可以控制 _Process 不频繁调用
        this.SetProcess false
