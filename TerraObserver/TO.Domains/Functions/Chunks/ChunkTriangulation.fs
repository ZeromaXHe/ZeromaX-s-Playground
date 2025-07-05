namespace TO.Domains.Functions.Chunks

open Friflo.Engine.ECS
open Godot
open TO.Domains.Functions.HexMeshes
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexMetrics
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Chunks
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 07:55:30
module ChunkTriangulationCommand =
    /// 仅绘制六边形（无扰动，点平均周围地块高度）
    let private triangulateJustHex
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ICatlikeCodingNoiseQuery and 'E :> ITileQuery)
        (chunk: IChunk)
        (tileId: TileId)
        =
        let planetConfig = env.PlanetConfig
        let unitHeight = planetConfig.UnitHeight
        let radius = planetConfig.Radius
        let tile = env.GetTile tileId
        let ids = Tile.countId tile |> _.CountId |> float32 |> (*) Vector3.One
        let height = env.GetHeight tile
        let waterHeight = tile |> Tile.value |> TileValue.waterSurfaceY unitHeight
        let tileHexFaceIds = Tile.hexFaceIds tile

        let mutable preNeighbor =
            HexIndexUtil.previousIdx 0 tileHexFaceIds.Length
            |> env.GetNeighborTileByIdx tile

        let mutable neighbor = env.GetNeighborTileByIdx tile 0

        let mutable nextNeighbor =
            HexIndexUtil.nextIdx 0 tileHexFaceIds.Length |> env.GetNeighborTileByIdx tile

        let mutable v0 = Vector3.Zero
        let mutable vw0 = Vector3.Zero

        for i in 0 .. tileHexFaceIds.Length - 1 do
            let neighborHeight = env.GetHeight neighbor

            let neighborWaterHeight =
                neighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight

            let preHeight = env.GetHeight preNeighbor
            let preWaterHeight = preNeighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight
            let nextHeight = env.GetHeight nextNeighbor

            let nextWaterHeight =
                nextNeighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight

            let avgHeight1 = (preHeight + neighborHeight + height) / 3f
            let avgHeight2 = (neighborHeight + nextHeight + height) / 3f
            let avgWaterHeight1 = (preWaterHeight + neighborWaterHeight + waterHeight) / 3f
            let avgWaterHeight2 = (neighborWaterHeight + nextWaterHeight + waterHeight) / 3f

            let v1 =
                Tile.cornerWithRadius TileUnitCorners.getFirstCornerWithRadius i (radius + avgHeight1) tile

            if i = 0 then
                v0 <- v1

            let v2 =
                Tile.cornerWithRadius TileUnitCorners.getSecondCornerWithRadius i (radius + avgHeight2) tile

            let vw1 =
                Tile.cornerWithRadius TileUnitCorners.getFirstCornerWithRadius i (radius + avgWaterHeight1) tile

            if i = 0 then
                vw0 <- vw1

            let vw2 =
                Tile.cornerWithRadius TileUnitCorners.getSecondCornerWithRadius i (radius + avgWaterHeight2) tile

            if i > 0 && i < tileHexFaceIds.Length - 1 then
                // 绘制地面
                chunk
                    .GetTerrain()
                    .AddTriangle(
                        [| v0; v1; v2 |],
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = ids
                    )
                // 绘制水面
                if tile |> Tile.value |> TileValue.isUnderwater then
                    chunk
                        .GetWater()
                        .AddTriangle([| vw0; vw1; vw2 |], MeshUtil.triArr HexMeshConstant.weights1, tis = ids)

            preNeighbor <- neighbor
            neighbor <- nextNeighbor
            nextNeighbor <- HexIndexUtil.next2Idx i tileHexFaceIds.Length |> env.GetNeighborTileByIdx tile

    /// 绘制平面六边形（有高度立面、处理接缝、但无特征、无河流）
    let private triangulatePlaneHex
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IFaceQuery
                and 'E :> ITileQuery
                and 'E :> IChunkQuery)
        (chunk: IChunk)
        (tileId: TileId)
        =
        let planetConfig = env.PlanetConfig
        let unitHeight = planetConfig.UnitHeight
        let radius = planetConfig.Radius
        let tile = env.GetTile tileId
        let ids = Tile.countId tile |> _.CountId |> float32 |> (*) Vector3.One
        let height = env.GetHeight tile
        let waterHeight = TileValue.waterSurfaceY unitHeight <| Tile.value tile

        let v0 =
            Tile.cornerWithRadius TileUnitCorners.getFirstCornerWithRadius 0 (radius + height) tile

        let mutable v1 = v0

        let vw0 =
            Tile.cornerWithRadius TileUnitCorners.getFirstCornerWithRadius 0 (radius + waterHeight) tile

        let mutable vw1 = vw0
        let tileHexFaceIds = Tile.hexFaceIds tile

        for i in 0 .. tileHexFaceIds.Length - 1 do
            let v2 =
                Tile.cornerWithRadius TileUnitCorners.getSecondCornerWithRadius i (radius + height) tile

            let vw2 =
                Tile.cornerWithRadius TileUnitCorners.getSecondCornerWithRadius i (radius + waterHeight) tile

            let neighbor = env.GetNeighborTileByIdx tile i

            let nIds =
                Vector3(
                    Tile.countId tile |> _.CountId |> float32,
                    Tile.countId neighbor |> _.CountId |> float32,
                    Tile.countId tile |> _.CountId |> float32
                )

            let neighborHeight = env.GetHeight neighbor

            let neighborWaterHeight =
                neighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight
            // 绘制陆地立面（由高的地块绘制）
            if neighborHeight < height then
                let vn1 = Math3dUtil.ProjectToSphere(v1, radius + neighborHeight)
                let vn2 = Math3dUtil.ProjectToSphere(v2, radius + neighborHeight)

                chunk
                    .GetTerrain()
                    .AddQuad(
                        [| v1; v2; vn1; vn2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = nIds
                    )
            // 绘制水面立面（由高的水面绘制）
            if
                tile |> Tile.value |> TileValue.isUnderwater
                && neighborWaterHeight < waterHeight
            then
                let vnw1 = Math3dUtil.ProjectToSphere(vw1, radius + neighborWaterHeight)
                let vnw2 = Math3dUtil.ProjectToSphere(vw2, radius + neighborWaterHeight)

                chunk
                    .GetWater()
                    .AddQuad(
                        [| vw1; vw2; vnw1; vnw2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = nIds
                    )
            // 处理接缝（目前很粗暴的对所有相邻地块的分块的 LOD 是 SimpleHex 以上时向外绘制到 Solid 边界）
            if
                neighbor |> Tile.chunkId |> _.ChunkId <> (tile |> Tile.chunkId |> _.ChunkId)
                && neighbor |> Tile.chunkId |> _.ChunkId |> env.GetChunkLod
                   >= ChunkLodEnum.SimpleHex
            then
                let n1FaceCenter = env.GetFaceCenter <| TileHexFaceIds.item i tileHexFaceIds

                let vn1 =
                    Tile.getCornerByFaceCenterWithRadiusAndSize
                        n1FaceCenter
                        (radius + neighborHeight)
                        HexMetrics.SolidFactor
                        neighbor

                let n2FaceCenter =
                    env.GetFaceCenter
                    <| TileHexFaceIds.item (HexIndexUtil.next2Idx i tileHexFaceIds.Length) tileHexFaceIds

                let vn2 =
                    Tile.getCornerByFaceCenterWithRadiusAndSize
                        n2FaceCenter
                        (radius + neighborHeight)
                        HexMetrics.SolidFactor
                        neighbor

                chunk
                    .GetTerrain()
                    .AddQuad(
                        [| v1; v2; vn1; vn2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = nIds
                    )

                if tile |> Tile.value |> TileValue.isUnderwater then
                    let vnw1 =
                        Tile.getCornerByFaceCenterWithRadiusAndSize
                            n1FaceCenter
                            (radius + neighborWaterHeight)
                            HexMetrics.waterFactor
                            neighbor

                    let vnw2 =
                        Tile.getCornerByFaceCenterWithRadiusAndSize
                            n2FaceCenter
                            (radius + neighborWaterHeight)
                            HexMetrics.waterFactor
                            neighbor

                    chunk
                        .GetWater()
                        .AddQuad(
                            [| vw1; vw2; vnw1; vnw2 |] |> Array.map env.Perturb,
                            MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                            tis = nIds
                        )

            if i > 0 && i < tileHexFaceIds.Length - 1 then
                // 绘制地面
                chunk
                    .GetTerrain()
                    .AddTriangle(
                        [| v0; v1; v2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = ids
                    )
                // 绘制水面
                if tile |> Tile.value |> TileValue.isUnderwater then
                    chunk
                        .GetWater()
                        .AddTriangle(
                            [| vw0; vw1; vw2 |] |> Array.map env.Perturb,
                            MeshUtil.triArr HexMeshConstant.weights1,
                            tis = ids
                        )

            v1 <- v2
            vw1 <- vw2

    let private triangulateRoadSegment
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (v1: Vector3)
        (v2: Vector3)
        (v3: Vector3)
        (v4: Vector3)
        (v5: Vector3)
        (v6: Vector3)
        (w1: Color)
        (w2: Color)
        (ids: Vector3)
        =
        let roads = chunk.GetRoads()

        roads.AddQuad(
            [| v1; v2; v4; v5 |] |> Array.map env.Perturb,
            MeshUtil.quad2Arr w1 w2,
            MeshUtil.quadUv 0f 1f 0f 0f,
            tis = ids
        )

        roads.AddQuad(
            [| v2; v3; v5; v6 |] |> Array.map env.Perturb,
            MeshUtil.quad2Arr w1 w2,
            MeshUtil.quadUv 1f 0f 0f 0f,
            tis = ids
        )

    let private triangulateEdgeStrip
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (e1: EdgeVertices)
        (w1: Color)
        (id1: TileCountId)
        (e2: EdgeVertices)
        (w2: Color)
        (id2: TileCountId)
        (hasRoad: bool)
        (simple: bool)
        =
        let ids = Vector3(float32 id1.CountId, float32 id2.CountId, float32 id1.CountId)
        let terrain = chunk.GetTerrain()

        let addTerrainQuad verticesArr =
            terrain.AddQuad(verticesArr |> Array.map env.Perturb, MeshUtil.quad2Arr w1 w2, tis = ids)

        if simple then
            addTerrainQuad [| e1.V1; e1.V5; e2.V1; e2.V5 |]
        else
            addTerrainQuad [| e1.V1; e1.V2; e2.V1; e2.V2 |]
            addTerrainQuad [| e1.V2; e1.V3; e2.V2; e2.V3 |]
            addTerrainQuad [| e1.V3; e1.V4; e2.V3; e2.V4 |]
            addTerrainQuad [| e1.V4; e1.V5; e2.V4; e2.V5 |]

        if hasRoad then
            triangulateRoadSegment env chunk e1.V2 e1.V3 e1.V4 e2.V2 e2.V3 e2.V4 w1 w2 ids

    let private triangulateEdgeFan
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (center: Vector3)
        (edge: EdgeVertices)
        (simple: bool)
        (tileCountId: TileCountId)
        =
        let ids = Vector3.One * float32 tileCountId.CountId
        let terrain = chunk.GetTerrain()

        let addTerrainTriangle verticesArr =
            terrain.AddTriangle(
                verticesArr |> Array.map env.Perturb,
                MeshUtil.triArr HexMeshConstant.weights1,
                tis = ids
            )

        if simple then
            addTerrainTriangle [| center; edge.V1; edge.V5 |]
        else
            addTerrainTriangle [| center; edge.V1; edge.V2 |]
            addTerrainTriangle [| center; edge.V2; edge.V3 |]
            addTerrainTriangle [| center; edge.V3; edge.V4 |]
            addTerrainTriangle [| center; edge.V4; edge.V5 |]

    let private triangulateRiverQuad
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (v1: Vector3)
        (v2: Vector3)
        (v3: Vector3)
        (v4: Vector3)
        (height1: float32)
        (height2: float32)
        (v: float32)
        (reversed: bool)
        (ids: Vector3)
        =
        let v1 = Math3dUtil.ProjectToSphere(v1, height1)
        let v2 = Math3dUtil.ProjectToSphere(v2, height1)
        let v3 = Math3dUtil.ProjectToSphere(v3, height2)
        let v4 = Math3dUtil.ProjectToSphere(v4, height2)

        chunk
            .GetRivers()
            .AddQuad(
                [| v1; v2; v3; v4 |] |> Array.map env.Perturb,
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                (if reversed then
                     MeshUtil.quadUv 1f 0f <| 0.8f - v <| 0.6f - v
                 else
                     MeshUtil.quadUv 0f 1f v <| v + 0.2f),
                tis = ids
            )

    let private triangulateWithRiverBeginOrEnd
        (env: #IPlanetConfigQuery)
        (chunk: IChunk)
        (tile: Entity)
        (centroid: Vector3)
        (e: EdgeVertices)
        =
        let mutable m = EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f))
        m.V3 <- Math3dUtil.ProjectToSphere(m.V3, e.V3.Length())

        triangulateEdgeStrip
            env
            chunk
            m
            HexMeshConstant.weights1
            (Tile.countId tile)
            e
            HexMeshConstant.weights1
            (Tile.countId tile)
            false
            false

        tile |> Tile.countId |> triangulateEdgeFan env chunk centroid m false

        if tile |> Tile.value |> TileValue.isUnderwater |> not then
            let reversed = tile |> Tile.flag |> TileFlag.hasIncomingRiver
            let ids = Tile.countId tile |> _.CountId |> float32 |> (*) Vector3.One
            let planetConfig = env.PlanetConfig

            let riverHeight =
                planetConfig.Radius
                + (tile |> Tile.value |> TileValue.riverSurfaceY planetConfig.UnitHeight)

            triangulateRiverQuad env chunk m.V2 m.V4 e.V2 e.V4 riverHeight riverHeight 0.6f reversed ids
            let centroid = Math3dUtil.ProjectToSphere(centroid, riverHeight)
            m.V2 <- Math3dUtil.ProjectToSphere(m.V2, riverHeight)
            m.V4 <- Math3dUtil.ProjectToSphere(m.V4, riverHeight)

            chunk
                .GetRivers()
                .AddTriangle(
                    [| centroid; m.V2; m.V4 |] |> Array.map env.Perturb,
                    MeshUtil.triArr HexMeshConstant.weights1,
                    (if reversed then
                         [| Vector2(0.5f, 0.4f); Vector2(1f, 0.2f); Vector2(0f, 0.2f) |]
                     else
                         [| Vector2(0.5f, 0.4f); Vector2(0f, 0.6f); Vector2(1f, 0.6f) |]),
                    tis = ids
                )

    let private triangulateWithRiver
        (env: #IPlanetConfigQuery)
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (height: float32)
        (centroid: Vector3)
        (e: EdgeVertices)
        =
        let planetConfig = env.PlanetConfig
        let radius = planetConfig.Radius
        let tileUnitCorners = Tile.unitCorners tile

        let centerL, centerR =
            // 注意五边形没有对边的情况
            if
                tileUnitCorners.Length = 6
                && TileFlag.hasRiver
                   <| HexIndexUtil.oppositeIdx idx tileUnitCorners.Length
                   <| Tile.flag tile
            then
                // 直线河流
                (Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                    (HexIndexUtil.previousIdx idx tileUnitCorners.Length)
                    (radius + height)
                    0.25f
                    tile),
                (Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getSecondSolidCornerWithRadiusAndSize
                    (HexIndexUtil.nextIdx idx tileUnitCorners.Length)
                    (radius + height)
                    0.25f
                    tile)
            elif
                TileFlag.hasRiver
                <| HexIndexUtil.nextIdx idx tileUnitCorners.Length
                <| Tile.flag tile
            then
                // 锐角弯
                centroid, centroid.Lerp(e.V5, 2f / 3f)
            elif
                TileFlag.hasRiver
                <| HexIndexUtil.previousIdx idx tileUnitCorners.Length
                <| Tile.flag tile
            then
                // 锐角弯
                centroid.Lerp(e.V1, 2f / 3f), centroid
            elif
                TileFlag.hasRiver
                <| HexIndexUtil.next2Idx idx tileUnitCorners.Length
                <| Tile.flag tile
            then
                // 钝角弯
                centroid,
                (Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                    (HexIndexUtil.nextIdx idx tileUnitCorners.Length)
                    (radius + height)
                    (0.5f * HexMetrics.InnerToOuter)
                    tile)
            elif
                TileFlag.hasRiver
                <| HexIndexUtil.previous2Idx idx tileUnitCorners.Length
                <| Tile.flag tile
            then
                // 钝角弯
                (Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                    (HexIndexUtil.previousIdx idx tileUnitCorners.Length)
                    (radius + height)
                    (0.5f * HexMetrics.InnerToOuter)
                    tile),
                centroid
            else
                centroid, centroid

        let mutable centroid = centerL.Lerp(centerR, 0.5f)

        let mutable m =
            EdgeVertices(centerL.Lerp(e.V1, 0.5f), centerR.Lerp(e.V5, 0.5f), 1f / 6f)

        m.V3 <- Math3dUtil.ProjectToSphere(m.V3, e.V3.Length())
        centroid <- Math3dUtil.ProjectToSphere(centroid, e.V3.Length())

        triangulateEdgeStrip
            env
            chunk
            m
            HexMeshConstant.weights1
            (Tile.countId tile)
            e
            HexMeshConstant.weights1
            (Tile.countId tile)
            false
            false

        let ids = Tile.countId tile |> _.CountId |> float32 |> (*) Vector3.One
        let terrain = chunk.GetTerrain()

        terrain.AddTriangle(
            [| centerL; m.V1; m.V2 |] |> Array.map env.Perturb,
            MeshUtil.triArr HexMeshConstant.weights1,
            tis = ids
        )

        terrain.AddQuad(
            [| centerL; centroid; m.V2; m.V3 |] |> Array.map env.Perturb,
            MeshUtil.quadArr HexMeshConstant.weights1,
            tis = ids
        )

        terrain.AddQuad(
            [| centroid; centerR; m.V3; m.V4 |] |> Array.map env.Perturb,
            MeshUtil.quadArr HexMeshConstant.weights1,
            tis = ids
        )

        terrain.AddTriangle(
            [| centerR; m.V4; m.V5 |] |> Array.map env.Perturb,
            MeshUtil.triArr HexMeshConstant.weights1,
            tis = ids
        )

        if tile |> Tile.value |> TileValue.isUnderwater |> not then
            let reversed = tile |> Tile.flag |> TileFlag.hasRiverIn idx

            let riverHeight =
                tile |> Tile.value |> TileValue.riverSurfaceY planetConfig.UnitHeight

            let riverTotalHeight = radius + riverHeight
            triangulateRiverQuad env chunk centerL centerR m.V2 m.V4 riverTotalHeight riverTotalHeight 0.4f reversed ids
            triangulateRiverQuad env chunk m.V2 m.V4 e.V2 e.V4 riverTotalHeight riverTotalHeight 0.6f reversed ids

    let private triangulateRoadEdge
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (centroid: Vector3)
        (mL: Vector3)
        (mR: Vector3)
        (tileCountId: TileCountId)
        =
        let ids = Vector3.One * float32 tileCountId.CountId

        chunk
            .GetRoads()
            .AddTriangle(
                [| centroid; mL; mR |] |> Array.map env.Perturb,
                MeshUtil.triArr HexMeshConstant.weights1,
                [| Vector2(1f, 0f); Vector2(0f, 0f); Vector2(0f, 0f) |],
                tis = ids
            )

    let private triangulateRoad
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (centroid: Vector3)
        (mL: Vector3)
        (mR: Vector3)
        (e: EdgeVertices)
        (hasRoadThroughCellEdge: bool)
        (tileCountId: TileCountId)
        =
        if hasRoadThroughCellEdge then
            let ids = Vector3.One * float32 tileCountId.CountId
            let mC = mL.Lerp(mR, 0.5f)

            triangulateRoadSegment
                env
                chunk
                mL
                mC
                mR
                e.V2
                e.V3
                e.V4
                HexMeshConstant.weights1
                HexMeshConstant.weights2
                ids

            let roads = chunk.GetRoads()

            roads.AddTriangle(
                [| centroid; mL; mC |] |> Array.map env.Perturb,
                MeshUtil.triArr HexMeshConstant.weights1,
                [| Vector2(1f, 0f); Vector2(0f, 0f); Vector2(1f, 0f) |],
                tis = ids
            )

            roads.AddTriangle(
                [| centroid; mC; mR |] |> Array.map env.Perturb,
                MeshUtil.triArr HexMeshConstant.weights1,
                [| Vector2(1f, 0f); Vector2(1f, 0f); Vector2(0f, 0f) |],
                tis = ids
            )
        else
            triangulateRoadEdge env chunk centroid mL mR tileCountId

    let private triangulateRoadAdjacentToRiver
        (env: #IPlanetConfigQuery)
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (height: float32)
        (centroid: Vector3)
        (e: EdgeVertices)
        =
        let cornerCount = Tile.unitCorners tile |> _.Length
        let hasRoadThroughEdge = tile |> Tile.flag |> TileFlag.hasRoad idx

        let previousHasRiver =
            TileFlag.hasRiver <| HexIndexUtil.previousIdx idx cornerCount <| Tile.flag tile

        let nextHasRiver =
            TileFlag.hasRiver <| HexIndexUtil.nextIdx idx cornerCount <| Tile.flag tile

        let incomingRiverIdx = tile |> Tile.flag |> TileFlag.riverInDirection
        let outgoingRiverIdx = tile |> Tile.flag |> TileFlag.riverOutDirection
        let radius = env.PlanetConfig.Radius
        let mutable roadCenter = centroid
        let mutable centroid = centroid
        let mutable returnNow = false

        if tile |> Tile.flag |> TileFlag.hasRiverBeginOrEnd then
            let riverBeginOrEndIdx =
                if tile |> Tile.flag |> TileFlag.hasIncomingRiver then
                    incomingRiverIdx
                else
                    outgoingRiverIdx

            if cornerCount = 5 then
                roadCenter <-
                    Tile.cornerWithRadiusAndSize
                        TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                        (HexIndexUtil.oppositeIdx riverBeginOrEndIdx cornerCount)
                        (radius + height)
                        (HexMetrics.OuterToInner / 3f)
                        tile
            else
                roadCenter <-
                    Tile.cornerWithRadiusAndSize
                        TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                        (HexIndexUtil.oppositeIdx riverBeginOrEndIdx cornerCount)
                        (radius + height)
                        (1f / 3f)
                        tile
        elif
            cornerCount = 6
            && incomingRiverIdx = HexIndexUtil.oppositeIdx outgoingRiverIdx cornerCount
        then
            // 河流走势是对边（直线）的情况（需要注意五边形没有对边的概念）
            let mutable corner = Vector3.Zero

            if previousHasRiver then
                if
                    not hasRoadThroughEdge
                    && tile
                       |> Tile.flag
                       |> TileFlag.hasRoad (HexIndexUtil.nextIdx idx cornerCount)
                       |> not
                then
                    returnNow <- true
                else
                    corner <-
                        Tile.cornerWithRadius TileUnitCorners.getSecondSolidCornerWithRadius idx (radius + height) tile
            else if
                not hasRoadThroughEdge
                && tile
                   |> Tile.flag
                   |> TileFlag.hasRoad (HexIndexUtil.previousIdx idx cornerCount)
                   |> not
            then
                returnNow <- true
            else
                corner <-
                    Tile.cornerWithRadius TileUnitCorners.getSecondSolidCornerWithRadius idx (radius + height) tile

            if not returnNow then
                roadCenter <- roadCenter + (corner - centroid) * 0.5f
                centroid <- centroid + (corner - centroid) * 0.25f
        elif incomingRiverIdx = HexIndexUtil.previousIdx outgoingRiverIdx cornerCount then
            // 河流走势是逆时针锐角的情况
            roadCenter <-
                roadCenter
                - Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getSecondCornerWithRadiusAndSize
                    incomingRiverIdx
                    (radius + height)
                    0.2f
                    tile
                + centroid
        elif incomingRiverIdx = HexIndexUtil.nextIdx outgoingRiverIdx cornerCount then
            // 河流走势是顺时针锐角的情况
            roadCenter <-
                roadCenter
                - Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getFirstCornerWithRadiusAndSize
                    incomingRiverIdx
                    (radius + height)
                    0.2f
                    tile
                + centroid
        elif previousHasRiver && nextHasRiver then
            // 河流走势是钝角的情况，且当前方向被夹在河流出入角中间
            if not hasRoadThroughEdge then
                returnNow <- true
            else
                let offset =
                    Tile.cornerWithRadiusAndSize
                        TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                        idx
                        (radius + height)
                        HexMetrics.InnerToOuter
                        tile

                roadCenter <- roadCenter + (offset - centroid) * 0.7f
                centroid <- centroid + (offset - centroid) * 0.5f
        elif cornerCount = 5 then
            // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：五边形有两个方向可能）
            let firstIdx =
                if previousHasRiver then
                    idx
                else
                    HexIndexUtil.previousIdx idx cornerCount // 两个可能方向中的顺时针第一个

            if
                tile |> Tile.flag |> TileFlag.hasRoad firstIdx |> not
                && tile
                   |> Tile.flag
                   |> TileFlag.hasRoad (HexIndexUtil.nextIdx firstIdx cornerCount)
                   |> not
            then
                returnNow <- true
            else
                let offset =
                    Tile.cornerWithRadius TileUnitCorners.getSecondSolidCornerWithRadius firstIdx (radius + height) tile

                roadCenter <- roadCenter + (offset - centroid) * 0.25f * HexMetrics.OuterToInner
        else
            // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：六边形有三个方向可能）
            let middleIdx =
                if previousHasRiver then
                    HexIndexUtil.nextIdx idx cornerCount
                elif nextHasRiver then
                    HexIndexUtil.previousIdx idx cornerCount
                else
                    idx

            if
                tile |> Tile.flag |> TileFlag.hasRoad middleIdx |> not
                && tile
                   |> Tile.flag
                   |> TileFlag.hasRoad (HexIndexUtil.previousIdx middleIdx cornerCount)
                   |> not
                && tile
                   |> Tile.flag
                   |> TileFlag.hasRoad (HexIndexUtil.nextIdx middleIdx cornerCount)
                   |> not
            then
                returnNow <- true
            else
                let offset =
                    Tile.cornerWithRadius TileUnitCorners.getSolidEdgeMiddleWithRadius middleIdx (radius + height) tile

                roadCenter <- roadCenter + (offset - centroid) * 0.25f

        if not returnNow then
            let interpolator = tile |> Tile.flag |> TileFlag.getRoadInterpolator cornerCount idx
            let mL = roadCenter.Lerp(e.V1, interpolator.X)
            let mR = roadCenter.Lerp(e.V5, interpolator.Y)

            tile
            |> Tile.countId
            |> triangulateRoad env chunk roadCenter mL mR e hasRoadThroughEdge

            if previousHasRiver then
                tile |> Tile.countId |> triangulateRoadEdge env chunk roadCenter centroid mL

            if nextHasRiver then
                tile |> Tile.countId |> triangulateRoadEdge env chunk roadCenter mR centroid

    let private triangulateAdjacentToRiver
        (env: #IPlanetConfigQuery)
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (height: float32)
        (centroid: Vector3)
        (e: EdgeVertices)
        (simple: bool)
        =
        if tile |> Tile.flag |> TileFlag.hasRoads then
            triangulateRoadAdjacentToRiver env chunk tile idx height centroid e

        let radius = env.PlanetConfig.Radius
        let cornerCount = Tile.unitCorners tile |> _.Length
        let tileFlag = Tile.flag tile

        let centroid =
            if TileFlag.hasRiver <| HexIndexUtil.nextIdx idx cornerCount <| tileFlag then
                if TileFlag.hasRiver <| HexIndexUtil.previousIdx idx cornerCount <| tileFlag then
                    Tile.cornerWithRadiusAndSize
                        TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                        idx
                        (radius + height)
                        (0.5f * HexMetrics.InnerToOuter)
                        tile
                elif
                    cornerCount = 6
                    && TileFlag.hasRiver <| HexIndexUtil.previous2Idx idx cornerCount <| tileFlag
                then
                    // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                    Tile.cornerWithRadiusAndSize
                        TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                        idx
                        (radius + height)
                        0.25f
                        tile
                else
                    centroid
            elif
                cornerCount = 6
                && TileFlag.hasRiver <| HexIndexUtil.previousIdx idx cornerCount <| tileFlag
                && TileFlag.hasRiver <| HexIndexUtil.next2Idx idx cornerCount <| tileFlag
            then
                // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                Tile.cornerWithRadiusAndSize
                    TileUnitCorners.getSecondSolidCornerWithRadiusAndSize
                    idx
                    (radius + height)
                    0.25f
                    tile
            else
                centroid

        let m = EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f))

        triangulateEdgeStrip
            env
            chunk
            m
            HexMeshConstant.weights1
            (Tile.countId tile)
            e
            HexMeshConstant.weights1
            (Tile.countId tile)
            false
            simple

        tile |> Tile.countId |> triangulateEdgeFan env chunk centroid m simple

    let private triangulateWithoutRiver
        env
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (centroid: Vector3)
        (e: EdgeVertices)
        (simple: bool)
        =
        tile |> Tile.countId |> triangulateEdgeFan env chunk centroid e simple

        if tile |> Tile.flag |> TileFlag.hasRoads then
            let interpolator =
                tile
                |> Tile.flag
                |> TileFlag.getRoadInterpolator (Tile.unitCorners tile).Length idx

            let mL = centroid.Lerp(e.V1, interpolator.X)
            let mR = centroid.Lerp(e.V5, interpolator.Y)
            let hasRoadThroughEdge = tile |> Tile.flag |> TileFlag.hasRoad idx

            tile
            |> Tile.countId
            |> triangulateRoad env chunk centroid mL mR e hasRoadThroughEdge

    let private triangulateWaterfallInWater
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (v1: Vector3)
        (v2: Vector3)
        (v3: Vector3)
        (v4: Vector3)
        (height1: float32)
        (height2: float32)
        (waterHeight: float32)
        (ids: Vector3)
        =
        let t = (waterHeight - height2) / (height1 - height2)
        let v1 = env.Perturb <| Math3dUtil.ProjectToSphere(v1, height1)
        let v2 = env.Perturb <| Math3dUtil.ProjectToSphere(v2, height1)
        let v3 = (env.Perturb <| Math3dUtil.ProjectToSphere(v3, height2)).Lerp(v1, t)
        let v4 = (env.Perturb <| Math3dUtil.ProjectToSphere(v4, height2)).Lerp(v2, t)

        chunk
            .GetRivers()
            .AddQuad(
                [| v1; v2; v3; v4 |],
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                MeshUtil.quadUv 0f 1f 0.8f 1f,
                tis = ids
            )

    let private triangulateEdgeTerraces
        env
        (chunk: IChunk)
        (beginE: EdgeVertices)
        (beginTileCountId: TileCountId)
        (endE: EdgeVertices)
        (endTileCountId: TileCountId)
        (hasRoad: bool)
        (simple: bool)
        =
        let mutable e2 = beginE
        let mutable w2 = HexMeshConstant.weights1

        for i in 1 .. HexMetrics.terraceSteps do
            let e1 = e2
            let w1 = w2
            e2 <- HexMetrics.terraceLerpEdgeV beginE endE i
            w2 <- HexMetrics.terraceLerpColor HexMeshConstant.weights1 HexMeshConstant.weights2 i
            triangulateEdgeStrip env chunk e1 w1 beginTileCountId e2 w2 endTileCountId hasRoad simple

    /// 处理高度不同的 beginTile 和两个高度相同的 endTile（即三角形两边是等高阶地，一边是平地）的情况
    let private triangulateCornerTerraces
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (beginV: Vector3)
        (beginTileCountId: TileCountId)
        (leftV: Vector3)
        (leftTileCountId: TileCountId)
        (rightV: Vector3)
        (rightTileCountId: TileCountId)
        =
        let mutable v3 = beginV
        let mutable v4 = beginV
        let mutable w3 = HexMeshConstant.weights1
        let mutable w4 = HexMeshConstant.weights1

        let ids =
            Vector3(float32 beginTileCountId.CountId, float32 leftTileCountId.CountId, float32 rightTileCountId.CountId)

        let terrain = chunk.GetTerrain()

        for i in 1 .. HexMetrics.terraceSteps do
            let v1 = v3
            let v2 = v4
            let w1 = w3
            let w2 = w4
            v3 <- HexMetrics.terraceLerp beginV leftV i
            v4 <- HexMetrics.terraceLerp beginV rightV i
            w3 <- HexMetrics.terraceLerpColor HexMeshConstant.weights1 HexMeshConstant.weights2 i
            w4 <- HexMetrics.terraceLerpColor HexMeshConstant.weights1 HexMeshConstant.weights3 i

            terrain.AddQuad([| v1; v2; v3; v4 |] |> Array.map env.Perturb, [| w1; w2; w3; w4 |], tis = ids)

    /// 阶地和悬崖中间的半三角形
    let private triangulateBoundaryTriangle
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (beginV: Vector3)
        (beginWeight: Color)
        (leftV: Vector3)
        (leftWeight: Color)
        (boundaryV: Vector3)
        (boundaryWeight: Color)
        (ids: Vector3)
        =
        let mutable v2 = env.Perturb beginV
        let mutable w2 = beginWeight
        let terrain = chunk.GetTerrain()

        for i in 1 .. HexMetrics.terraceSteps do
            let v1 = v2
            let w1 = w2
            v2 <- env.Perturb <| HexMetrics.terraceLerp beginV leftV i
            w2 <- HexMetrics.terraceLerpColor beginWeight leftWeight i
            terrain.AddTriangle([| v1; v2; boundaryV |], [| w1; w2; boundaryWeight |], tis = ids)

    /// 三角形靠近 tile 的左边是阶地，右边是悬崖，另一边任意的情况
    let private triangulateCornerTerracesCliff
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (beginV: Vector3)
        (beginTileCountId: TileCountId)
        (beginValue: TileValue)
        (leftV: Vector3)
        (leftTileCountId: TileCountId)
        (leftValue: TileValue)
        (rightV: Vector3)
        (rightTileCountId: TileCountId)
        (rightValue: TileValue)
        =
        let b =
            1f
            / Mathf.Abs(float32 <| TileValue.elevation rightValue - TileValue.elevation beginValue)

        let boundary = env.Perturb(beginV).Lerp(env.Perturb(rightV), b)
        let boundaryWeights = HexMeshConstant.weights1.Lerp(HexMeshConstant.weights3, b)

        let ids =
            Vector3(float32 beginTileCountId.CountId, float32 leftTileCountId.CountId, float32 rightTileCountId.CountId)

        triangulateBoundaryTriangle
            env
            chunk
            beginV
            HexMeshConstant.weights1
            leftV
            HexMeshConstant.weights2
            boundary
            boundaryWeights
            ids

        if
            HexMetrics.getEdgeType
            <| TileValue.elevation leftValue
            <| TileValue.elevation rightValue = HexEdgeType.Slope
        then
            triangulateBoundaryTriangle
                env
                chunk
                leftV
                HexMeshConstant.weights2
                rightV
                HexMeshConstant.weights3
                boundary
                boundaryWeights
                ids
        else
            chunk
                .GetTerrain()
                .AddTriangle(
                    [| env.Perturb leftV; env.Perturb rightV; boundary |],
                    [| HexMeshConstant.weights2; HexMeshConstant.weights3; boundaryWeights |],
                    tis = ids
                )

    /// 三角形靠近 tile 的左边是悬崖，右边是阶地，另一边任意的情况
    let private triangulateCornerCliffTerraces
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (beginV: Vector3)
        (beginTileCountId: TileCountId)
        (beginValue: TileValue)
        (leftV: Vector3)
        (leftTileCountId: TileCountId)
        (leftValue: TileValue)
        (rightV: Vector3)
        (rightTileCountId: TileCountId)
        (rightValue: TileValue)
        =
        let b =
            1f
            / Mathf.Abs(float32 <| TileValue.elevation leftValue - TileValue.elevation beginValue)

        let boundary = env.Perturb(beginV).Lerp(env.Perturb(leftV), b)
        let boundaryWeights = HexMeshConstant.weights1.Lerp(HexMeshConstant.weights2, b)

        let ids =
            Vector3(float32 beginTileCountId.CountId, float32 leftTileCountId.CountId, float32 rightTileCountId.CountId)

        triangulateBoundaryTriangle
            env
            chunk
            rightV
            HexMeshConstant.weights3
            beginV
            HexMeshConstant.weights1
            boundary
            boundaryWeights
            ids

        if
            HexMetrics.getEdgeType
            <| TileValue.elevation leftValue
            <| TileValue.elevation rightValue = HexEdgeType.Slope
        then
            triangulateBoundaryTriangle
                env
                chunk
                leftV
                HexMeshConstant.weights2
                rightV
                HexMeshConstant.weights3
                boundary
                boundaryWeights
                ids
        else
            chunk
                .GetTerrain()
                .AddTriangle(
                    [| env.Perturb leftV; env.Perturb rightV; boundary |],
                    [| HexMeshConstant.weights2; HexMeshConstant.weights3; boundaryWeights |],
                    tis = ids
                )

    /// 需要保证入参 bottom -> left -> right 是顺时针
    let private triangulateCorner
        env
        (chunk: IChunk)
        (chunkLod: ChunkLodEnum)
        (bottom: Vector3)
        (bottomTileCountId: TileCountId)
        (bottomValue: TileValue)
        (left: Vector3)
        (leftTileCountId: TileCountId)
        (leftValue: TileValue)
        (right: Vector3)
        (rightTileCountId: TileCountId)
        (rightValue: TileValue)
        =
        let edgeType1 =
            HexMetrics.getEdgeType
            <| TileValue.elevation bottomValue
            <| TileValue.elevation leftValue

        let edgeType2 =
            HexMetrics.getEdgeType
            <| TileValue.elevation bottomValue
            <| TileValue.elevation rightValue

        if chunkLod > ChunkLodEnum.SimpleHex then
            if edgeType1 = HexEdgeType.Slope then
                if edgeType2 = HexEdgeType.Slope then
                    triangulateCornerTerraces
                        env
                        chunk
                        bottom
                        bottomTileCountId
                        left
                        leftTileCountId
                        right
                        rightTileCountId
                elif edgeType2 = HexEdgeType.Flat then
                    triangulateCornerTerraces
                        env
                        chunk
                        left
                        leftTileCountId
                        right
                        rightTileCountId
                        bottom
                        bottomTileCountId
                else
                    triangulateCornerTerracesCliff
                        env
                        chunk
                        bottom
                        bottomTileCountId
                        bottomValue
                        left
                        leftTileCountId
                        leftValue
                        right
                        rightTileCountId
                        rightValue
            elif edgeType2 = HexEdgeType.Slope then
                if edgeType1 = HexEdgeType.Flat then
                    triangulateCornerTerraces
                        env
                        chunk
                        right
                        rightTileCountId
                        bottom
                        bottomTileCountId
                        left
                        leftTileCountId
                else
                    triangulateCornerCliffTerraces
                        env
                        chunk
                        bottom
                        bottomTileCountId
                        bottomValue
                        left
                        leftTileCountId
                        leftValue
                        right
                        rightTileCountId
                        rightValue
            elif
                HexMetrics.getEdgeType
                <| TileValue.elevation leftValue
                <| TileValue.elevation rightValue = HexEdgeType.Slope
            then
                if TileValue.elevation leftValue < TileValue.elevation rightValue then
                    triangulateCornerCliffTerraces
                        env
                        chunk
                        right
                        rightTileCountId
                        rightValue
                        bottom
                        bottomTileCountId
                        bottomValue
                        left
                        leftTileCountId
                        leftValue
                else
                    triangulateCornerTerracesCliff
                        env
                        chunk
                        left
                        leftTileCountId
                        leftValue
                        right
                        rightTileCountId
                        rightValue
                        bottom
                        bottomTileCountId
                        bottomValue
            else
                chunk
                    .GetTerrain()
                    .AddTriangle(
                        [| bottom; left; right |] |> Array.map env.Perturb,
                        [| HexMeshConstant.weights1
                           HexMeshConstant.weights2
                           HexMeshConstant.weights3 |],
                        tis =
                            Vector3(
                                float32 bottomTileCountId.CountId,
                                float32 leftTileCountId.CountId,
                                float32 rightTileCountId.CountId
                            )
                    )
        else
            chunk
                .GetTerrain()
                .AddTriangle(
                    [| bottom; left; right |] |> Array.map env.Perturb,
                    [| HexMeshConstant.weights1
                       HexMeshConstant.weights2
                       HexMeshConstant.weights3 |],
                    tis =
                        Vector3(
                            float32 bottomTileCountId.CountId,
                            float32 leftTileCountId.CountId,
                            float32 rightTileCountId.CountId
                        )
                )

    let private triangulateConnection
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IFaceQuery
                and 'E :> ITileQuery
                and 'E :> IChunkQuery)
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (e: EdgeVertices)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig
        let tileHeight = env.GetHeight tile
        let neighbor = env.GetNeighborTileByIdx tile idx
        let neighborHeight = env.GetHeight neighbor
        let standardScale = planetConfig.StandardScale
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成，或者是编辑地块与非编辑地块间的连接
        if
            tileHeight > neighborHeight
            || (Mathf.Abs(tileHeight - neighborHeight) < 0.0001f * standardScale
                && tile.Id < neighbor.Id)
        then
            () // 忽略
        else
            let radius = planetConfig.Radius
            let unitHeight = planetConfig.UnitHeight

            let n1FaceCenter =
                tile |> Tile.hexFaceIds |> TileHexFaceIds.item idx |> env.GetFaceCenter

            let vn1 =
                Tile.getCornerByFaceCenterWithRadiusAndSize
                    n1FaceCenter
                    (radius + neighborHeight)
                    HexMetrics.SolidFactor
                    neighbor

            let n2FaceCenter =
                tile
                |> Tile.hexFaceIds
                |> TileHexFaceIds.item (Tile.hexFaceIds tile |> _.Length |> HexIndexUtil.nextIdx idx)
                |> env.GetFaceCenter

            let vn2 =
                Tile.getCornerByFaceCenterWithRadiusAndSize
                    n2FaceCenter
                    (radius + neighborHeight)
                    HexMetrics.SolidFactor
                    neighbor

            let mutable en = EdgeVertices(vn1, vn2)
            let hasRiver = tile |> Tile.flag |> TileFlag.hasRiver idx
            let hasRoad = tile |> Tile.flag |> TileFlag.hasRoad idx

            if hasRiver then
                en.V3 <-
                    Math3dUtil.ProjectToSphere(en.V3, radius + TileValue.streamBedY unitHeight (Tile.value neighbor))

                let ids =
                    Vector3(
                        Tile.countId tile |> _.CountId |> float32,
                        Tile.countId neighbor |> _.CountId |> float32,
                        Tile.countId tile |> _.CountId |> float32
                    )

                let tileRiverHeight =
                    tile |> Tile.value |> TileValue.riverSurfaceY unitHeight |> (+) radius

                let neighborRiverHeight =
                    neighbor |> Tile.value |> TileValue.riverSurfaceY unitHeight |> (+) radius

                if tile |> Tile.value |> TileValue.isUnderwater |> not then
                    if neighbor |> Tile.value |> TileValue.isUnderwater |> not then
                        let reversed =
                            (tile |> Tile.flag |> TileFlag.hasIncomingRiver)
                            && (tile |> Tile.flag |> TileFlag.hasRiverIn idx)

                        triangulateRiverQuad
                            env
                            chunk
                            e.V2
                            e.V4
                            en.V2
                            en.V4
                            tileRiverHeight
                            neighborRiverHeight
                            0.8f
                            reversed
                            ids
                    elif
                        tile |> Tile.value |> TileValue.elevation > (neighbor |> Tile.value |> TileValue.elevation)
                    then
                        let neighborWaterHeight =
                            neighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight |> (+) radius

                        triangulateWaterfallInWater
                            env
                            chunk
                            e.V2
                            e.V4
                            en.V2
                            en.V4
                            tileRiverHeight
                            neighborRiverHeight
                            neighborWaterHeight
                            ids
                elif
                    neighbor |> Tile.value |> TileValue.isUnderwater |> not
                    && neighbor |> Tile.value |> TileValue.elevation > (tile |> Tile.value |> TileValue.elevation)
                then
                    let tileWaterHeight =
                        tile |> Tile.value |> TileValue.waterSurfaceY unitHeight |> (+) radius

                    triangulateWaterfallInWater
                        env
                        chunk
                        en.V4
                        en.V2
                        e.V4
                        e.V2
                        neighborRiverHeight
                        tileRiverHeight
                        tileWaterHeight
                        ids

            let chunkLod = Tile.chunkId tile |> _.ChunkId |> env.GetChunkLod

            if
                chunkLod > ChunkLodEnum.SimpleHex
                && TileValue.getEdgeType <| Tile.value neighbor <| Tile.value tile = HexEdgeType.Slope
            then
                triangulateEdgeTerraces
                    env
                    chunk
                    e
                    (Tile.countId tile)
                    en
                    (Tile.countId neighbor)
                    hasRoad
                    (not hasRiver && simple)
            else
                triangulateEdgeStrip
                    env
                    chunk
                    e
                    HexMeshConstant.weights1
                    (Tile.countId tile)
                    en
                    HexMeshConstant.weights2
                    (Tile.countId neighbor)
                    hasRoad
                    (not hasRiver && simple)

            let preIdx = Tile.hexFaceIds tile |> _.Length |> HexIndexUtil.previousIdx idx
            let preNeighbor = env.GetNeighborTileByIdx tile preIdx
            let preNeighborHeight = env.GetHeight preNeighbor
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成，或者是编辑地块与非编辑地块间的连接三角形
            if
                tileHeight < preNeighborHeight
                || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.0001f * standardScale
                    && tile.Id > preNeighbor.Id)
            then
                let vpn =
                    Tile.getCornerByFaceCenterWithRadiusAndSize
                        n1FaceCenter
                        (radius + preNeighborHeight)
                        HexMetrics.SolidFactor
                        preNeighbor

                triangulateCorner
                    env
                    chunk
                    chunkLod
                    e.V1
                    (Tile.countId tile)
                    (Tile.value tile)
                    vpn
                    (Tile.countId preNeighbor)
                    (Tile.value preNeighbor)
                    vn1
                    (Tile.countId neighbor)
                    (Tile.value neighbor)

    let private triangulateEstuary
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (e1: EdgeVertices)
        (e2: EdgeVertices)
        (ids: Vector3)
        (incomingRiver: bool)
        =
        let waterShore = chunk.GetWaterShore()
        let estuary = chunk.GetEstuary()

        waterShore.AddTriangle(
            [| e2.V1; e1.V2; e1.V1 |] |> Array.map env.Perturb,
            [| HexMeshConstant.weights2
               HexMeshConstant.weights1
               HexMeshConstant.weights1 |],
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(0f, 0f) |],
            tis = ids
        )

        waterShore.AddTriangle(
            [| e2.V5; e1.V5; e1.V4 |] |> Array.map env.Perturb,
            [| HexMeshConstant.weights2
               HexMeshConstant.weights1
               HexMeshConstant.weights1 |],
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(0f, 0f) |],
            tis = ids
        )

        estuary.AddQuad(
            [| e2.V1; e1.V2; e2.V2; e1.V3 |] |> Array.map env.Perturb,
            [| HexMeshConstant.weights2
               HexMeshConstant.weights1
               HexMeshConstant.weights2
               HexMeshConstant.weights1 |],
            [| Vector2(0f, 1f); Vector2(0f, 0f); Vector2(1f, 1f); Vector2(0f, 0f) |],
            (if incomingRiver then
                 [| Vector2(1.5f, 1f)
                    Vector2(0.7f, 1.15f)
                    Vector2(1f, 0.8f)
                    Vector2(0.5f, 1.1f) |]
             else
                 [| Vector2(-0.5f, -0.2f)
                    Vector2(0.3f, -0.35f)
                    Vector2(0f, 0f)
                    Vector2(0.5f, -0.3f) |]),
            ids
        )

        estuary.AddTriangle(
            [| e1.V3; e2.V2; e2.V4 |] |> Array.map env.Perturb,
            [| HexMeshConstant.weights1
               HexMeshConstant.weights2
               HexMeshConstant.weights2 |],
            [| Vector2(0f, 0f); Vector2(1f, 1f); Vector2(1f, 1f) |],
            (if incomingRiver then
                 [| Vector2(0.5f, 1.1f); Vector2(1f, 0.8f); Vector2(0f, 0.8f) |]
             else
                 [| Vector2(0.5f, -0.3f); Vector2(0f, 0f); Vector2(1f, 0f) |]),
            ids
        )

        estuary.AddQuad(
            [| e1.V3; e1.V4; e2.V4; e2.V5 |] |> Array.map env.Perturb,
            MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
            [| Vector2(0f, 0f); Vector2(0f, 0f); Vector2(1f, 1f); Vector2(0f, 1f) |],
            (if incomingRiver then
                 [| Vector2(0.5f, 1.1f)
                    Vector2(0.3f, 1.15f)
                    Vector2(0f, 0.8f)
                    Vector2(-0.5f, 1f) |]
             else
                 [| Vector2(0.5f, -0.3f)
                    Vector2(0.7f, -0.35f)
                    Vector2(1f, 0f)
                    Vector2(1.5f, -0.2f) |]),
            ids
        )

    let private triangulateWaterShore
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IFaceQuery
                and 'E :> ITileQuery)
        (chunk: IChunk)
        (tile: Entity)
        (neighbor: Entity)
        (centroid: Vector3)
        (idx: int)
        (waterHeight: float32)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig
        let radius = planetConfig.Radius
        let unitHeight = planetConfig.UnitHeight

        let e1 =
            EdgeVertices(
                Tile.cornerWithRadius TileUnitCorners.getFirstWaterCornerWithRadius idx (radius + waterHeight) tile,
                Tile.cornerWithRadius TileUnitCorners.getSecondWaterCornerWithRadius idx (radius + waterHeight) tile
            )

        let mutable ids =
            Vector3(
                Tile.countId tile |> _.CountId |> float32,
                Tile.countId neighbor |> _.CountId |> float32,
                Tile.countId tile |> _.CountId |> float32
            )

        let water = chunk.GetWater()

        let addWaterTriangle verticesArr =
            water.AddTriangle(verticesArr |> Array.map env.Perturb, MeshUtil.triArr HexMeshConstant.weights1, tis = ids)

        if simple then
            addWaterTriangle [| centroid; e1.V1; e1.V5 |]
        else
            addWaterTriangle [| centroid; e1.V1; e1.V2 |]
            addWaterTriangle [| centroid; e1.V2; e1.V3 |]
            addWaterTriangle [| centroid; e1.V3; e1.V4 |]
            addWaterTriangle [| centroid; e1.V4; e1.V5 |]
        // 使用邻居的水表面高度的话，就是希望考虑岸边地块的实际水位。(不然强行拉平岸边的话，角落两个水面不一样高时太多复杂逻辑，bug 太多)
        let neighborWaterHeight =
            neighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight

        let n1FaceCenter =
            tile |> Tile.hexFaceIds |> TileHexFaceIds.item idx |> env.GetFaceCenter

        let cn1 =
            Tile.getCornerByFaceCenterWithRadiusAndSize
                n1FaceCenter
                (radius + neighborWaterHeight)
                HexMetrics.SolidFactor
                neighbor

        let n2FaceCenter =
            tile
            |> Tile.hexFaceIds
            |> TileHexFaceIds.item (Tile.hexFaceIds tile |> _.Length |> HexIndexUtil.nextIdx idx)
            |> env.GetFaceCenter

        let cn2 =
            Tile.getCornerByFaceCenterWithRadiusAndSize
                n2FaceCenter
                (radius + neighborWaterHeight)
                HexMetrics.SolidFactor
                neighbor

        let e2 = EdgeVertices(cn1, cn2)
        let neighborIdx = Tile.getNeighborTileIdx tile neighbor
        let waterShore = chunk.GetWaterShore()

        let addWaterShoreQuad verticesArr =
            waterShore.AddQuad(
                verticesArr |> Array.map env.Perturb,
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                MeshUtil.quadUv 0f 0f 0f 1f,
                tis = ids
            )

        if simple then
            addWaterShoreQuad [| e1.V1; e1.V5; e2.V1; e2.V5 |]
        elif tile |> Tile.flag |> TileFlag.hasRiver neighborIdx then
            tile
            |> Tile.flag
            |> TileFlag.hasRiverIn neighborIdx
            |> triangulateEstuary env chunk e1 e2 ids
        else
            addWaterShoreQuad [| e1.V1; e1.V2; e2.V1; e2.V2 |]
            addWaterShoreQuad [| e1.V2; e1.V3; e2.V2; e2.V3 |]
            addWaterShoreQuad [| e1.V3; e1.V4; e2.V3; e2.V4 |]
            addWaterShoreQuad [| e1.V4; e1.V5; e2.V4; e2.V5 |]

        let nextNeighbor =
            Tile.hexFaceIds tile
            |> _.Length
            |> HexIndexUtil.nextIdx idx
            |> env.GetNeighborTileByIdx tile

        let nextNeighborWaterHeight =
            nextNeighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight

        let cnn =
            Tile.getCornerByFaceCenterWithRadiusAndSize
                n2FaceCenter
                (radius + nextNeighborWaterHeight)
                (if nextNeighbor |> Tile.value |> TileValue.isUnderwater then
                     HexMetrics.waterFactor
                 else
                     HexMetrics.SolidFactor)
                nextNeighbor

        ids.Z <- float32 (Tile.countId nextNeighbor).CountId

        chunk
            .GetWaterShore()
            .AddTriangle(
                [| e1.V5; e2.V5; cnn |] |> Array.map env.Perturb,
                [| HexMeshConstant.weights1
                   HexMeshConstant.weights2
                   HexMeshConstant.weights3 |],
                [| Vector2(0f, 0f)
                   Vector2(0f, 1f)
                   Vector2(
                       0f,
                       if nextNeighbor |> Tile.value |> TileValue.isUnderwater then
                           0f
                       else
                           1f
                   ) |],
                tis = ids
            )

    let private triangulateOpenWater
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IFaceQuery
                and 'E :> ITileQuery)
        (chunk: IChunk)
        (tile: Entity)
        (neighbor: Entity)
        (centroid: Vector3)
        (idx: int)
        (waterHeight: float32)
        =
        let planetConfig = env.PlanetConfig
        let radius = planetConfig.Radius
        let unitHeight = planetConfig.UnitHeight

        let c1 =
            Tile.cornerWithRadius TileUnitCorners.getFirstWaterCornerWithRadius idx (radius + waterHeight) tile

        let c2 =
            Tile.cornerWithRadius TileUnitCorners.getSecondWaterCornerWithRadius idx (radius + waterHeight) tile

        let mutable ids = Vector3.One * float32 (Tile.countId tile).CountId
        let water = chunk.GetWater()

        water.AddTriangle(
            [| centroid; c1; c2 |] |> Array.map env.Perturb,
            MeshUtil.triArr HexMeshConstant.weights1,
            tis = ids
        )
        // 由更大 Id 的地块绘制水域连接，或者是由编辑地块绘制和不编辑的邻接地块间的连接
        if tile.Id > neighbor.Id then
            let neighborWaterHeight =
                neighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight

            let n1FaceCenter =
                tile |> Tile.hexFaceIds |> TileHexFaceIds.item idx |> env.GetFaceCenter

            let cn1 =
                Tile.getCornerByFaceCenterWithRadiusAndSize
                    n1FaceCenter
                    (radius + neighborWaterHeight)
                    HexMetrics.waterFactor
                    neighbor

            let n2FaceCenter =
                tile
                |> Tile.hexFaceIds
                |> TileHexFaceIds.item (Tile.hexFaceIds tile |> _.Length |> HexIndexUtil.nextIdx idx)
                |> env.GetFaceCenter

            let cn2 =
                Tile.getCornerByFaceCenterWithRadiusAndSize
                    n2FaceCenter
                    (radius + neighborWaterHeight)
                    HexMetrics.waterFactor
                    neighbor

            ids.Y <- float32 (Tile.countId neighbor).CountId

            water.AddQuad(
                [| c1; c2; cn1; cn2 |] |> Array.map env.Perturb,
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                tis = ids
            )
            // 由最大 Id 的地块绘制水域角落三角形，或者是由编辑地块绘制和不编辑的两个邻接地块间的连接
            let nextNeighbor =
                Tile.hexFaceIds tile
                |> _.Length
                |> HexIndexUtil.nextIdx idx
                |> env.GetNeighborTileByIdx tile

            if
                tile.Id > nextNeighbor.Id
                && nextNeighbor |> Tile.value |> TileValue.isUnderwater
            then

                let cnn =
                    Tile.getCornerByFaceCenterWithRadiusAndSize
                        n2FaceCenter
                        (radius + (nextNeighbor |> Tile.value |> TileValue.waterSurfaceY unitHeight))
                        HexMetrics.waterFactor
                        nextNeighbor

                ids.Z <- float32 (Tile.countId nextNeighbor).CountId

                water.AddTriangle(
                    [| c2; cn2; cnn |] |> Array.map env.Perturb,
                    [| HexMeshConstant.weights1
                       HexMeshConstant.weights2
                       HexMeshConstant.weights3 |],
                    tis = ids
                )

    let private triangulateWater
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> ITileQuery)
        (chunk: IChunk)
        (tile: Entity)
        (idx: int)
        (centroid: Vector3)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig

        let waterHeight =
            tile |> Tile.value |> TileValue.waterSurfaceY planetConfig.UnitHeight

        let centroid =
            Math3dUtil.ProjectToSphere(centroid, planetConfig.Radius + waterHeight)

        let neighbor = env.GetNeighborTileByIdx tile idx

        if neighbor |> Tile.value |> TileValue.isUnderwater |> not then
            triangulateWaterShore env chunk tile neighbor centroid idx waterHeight simple
        else
            triangulateOpenWater env chunk tile neighbor centroid idx waterHeight

    /// Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    let private triangulateHex
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> ITileQuery
                and 'E :> IChunkQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (idx: int)
        =
        let planetConfig = env.PlanetConfig
        let unitHeight = planetConfig.UnitHeight
        let radius = planetConfig.Radius
        let tile = env.GetTile tileId
        let height = env.GetHeight tile

        let v1 =
            Tile.cornerWithRadius TileUnitCorners.getFirstSolidCornerWithRadius idx (radius + height) tile

        let v2 =
            Tile.cornerWithRadius TileUnitCorners.getSecondSolidCornerWithRadius idx (radius + height) tile

        let mutable e = EdgeVertices(v1, v2)
        let centroid = TileUnitCentroid.scaled <| radius + height <| Tile.unitCentroid tile
        let chunkLod = env.GetChunkLod (Tile.chunkId tile).ChunkId

        let simple =
            if chunkLod = ChunkLodEnum.Full then
                false
            else
                let neighbor = env.GetNeighborTileByIdx tile idx

                if (Tile.chunkId neighbor).ChunkId = (Tile.chunkId tile).ChunkId then
                    true
                else
                    env.GetChunkLod (Tile.chunkId neighbor).ChunkId < ChunkLodEnum.Full

        if tile |> Tile.flag |> TileFlag.hasRivers then
            if tile |> Tile.flag |> TileFlag.hasRiver idx then
                e.V3 <-
                    Math3dUtil.ProjectToSphere(e.V3, radius + (tile |> Tile.value |> TileValue.streamBedY unitHeight))

                if tile |> Tile.flag |> TileFlag.hasRiverBeginOrEnd then
                    triangulateWithRiverBeginOrEnd env chunk tile centroid e
                else
                    triangulateWithRiver env chunk tile idx height centroid e
            else
                triangulateAdjacentToRiver env chunk tile idx height centroid e simple
        else
            triangulateWithoutRiver env chunk tile idx centroid e simple

        triangulateConnection env chunk tile idx e simple

        if tile |> Tile.value |> TileValue.isUnderwater then
            triangulateWater env chunk tile idx centroid simple

    let triangulate (env: #IEntityStoreQuery) : Triangulate =
        fun (chunk: IChunk) (tileId: TileId) ->
            let tile = env.GetEntityById tileId
            let tileChunkId = tile.GetComponent<TileChunkId>()
            let tileChunk = env.GetEntityById tileChunkId.ChunkId
            let chunkLod = tileChunk.GetComponent<ChunkLod>().Lod

            if chunkLod = ChunkLodEnum.JustHex then
                triangulateJustHex env chunk tileId
            elif chunkLod = ChunkLodEnum.PlaneHex then
                triangulatePlaneHex env chunk tileId
            else
                let tileFaceIds = tile.GetComponent<TileHexFaceIds>()

                for i in 0 .. tileFaceIds.Length - 1 do
                    triangulateHex env chunk tileId i
