namespace TO.Domains.Functions.Chunks

open Godot
open TO.Domains.Functions.HexMeshes
open TO.Domains.Functions.HexMetrics
open TO.Domains.Functions.HexSpheres.Components.Tiles
open TO.Domains.Functions.Maths
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexMetrics
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Chunks
open TO.Domains.Types.HexSpheres.Components.Faces
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-30 07:55:30
module ChunkTriangulationCommand =
    /// 仅绘制六边形（无扰动，点平均周围地块高度）
    let private triangulateJustHex
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileCountId = tile.GetComponent<TileCountId>()
        let ids = Vector3.One * float32 tileCountId.CountId
        let tileCorners = tile.GetComponent<TileUnitCorners>()
        let tileFaceIds = tile.GetComponent<TileHexFaceIds>()
        let tileValue = tile.GetComponent<TileValue>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let height = env.GetHeight tileValue tileUnitCentroid
        let unitHeight = planetConfig.UnitHeight
        let waterHeight = TileValue.waterSurfaceY unitHeight tileValue

        let mutable preNeighbor =
            env.GetNeighborByIdAndIdx tileId
            <| HexIndexUtil.previousIdx tileFaceIds.Length 0
            |> Option.get

        let mutable neighbor = env.GetNeighborByIdAndIdx tileId 0 |> Option.get

        let mutable nextNeighbor =
            env.GetNeighborByIdAndIdx tileId <| HexIndexUtil.nextIdx tileFaceIds.Length 0
            |> Option.get

        let mutable v0 = Vector3.Zero
        let mutable vw0 = Vector3.Zero
        let radius = planetConfig.Radius

        for i in 0 .. tileFaceIds.Length - 1 do
            let neighborValue = neighbor.GetComponent<TileValue>()
            let neighborUnitCentroid = neighbor.GetComponent<TileUnitCentroid>()
            let neighborHeight = env.GetHeight neighborValue neighborUnitCentroid
            let neighborWaterHeight = TileValue.waterSurfaceY unitHeight neighborValue
            let preValue = preNeighbor.GetComponent<TileValue>()
            let preUnitCentroid = preNeighbor.GetComponent<TileUnitCentroid>()
            let preHeight = env.GetHeight preValue preUnitCentroid
            let preWaterHeight = TileValue.waterSurfaceY unitHeight preValue
            let nextValue = nextNeighbor.GetComponent<TileValue>()
            let nextUnitCentroid = nextNeighbor.GetComponent<TileUnitCentroid>()
            let nextHeight = env.GetHeight nextValue nextUnitCentroid
            let nextWaterHeight = TileValue.waterSurfaceY unitHeight nextValue
            let avgHeight1 = (preHeight + neighborHeight + height) / 3f
            let avgHeight2 = (neighborHeight + nextHeight + height) / 3f
            let avgWaterHeight1 = (preWaterHeight + neighborWaterHeight + waterHeight) / 3f
            let avgWaterHeight2 = (neighborWaterHeight + nextWaterHeight + waterHeight) / 3f

            let v1 =
                tileCorners
                |> TileUnitCorners.getFirstCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + avgHeight1)

            if i = 0 then
                v0 <- v1

            let v2 =
                tileCorners
                |> TileUnitCorners.getSecondCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + avgHeight2)

            let vw1 =
                tileCorners
                |> TileUnitCorners.getFirstCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + avgWaterHeight1)

            if i = 0 then
                vw0 <- vw1

            let vw2 =
                tileCorners
                |> TileUnitCorners.getSecondCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + avgWaterHeight2)

            if i > 0 && i < tileFaceIds.Length - 1 then
                // 绘制地面
                chunk
                    .GetTerrain()
                    .AddTriangle(
                        [| v0; v1; v2 |],
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = ids
                    )
                // 绘制水面
                if TileValue.isUnderwater tileValue then
                    chunk
                        .GetWater()
                        .AddTriangle([| vw0; vw1; vw2 |], MeshUtil.triArr HexMeshConstant.weights1, tis = ids)

            preNeighbor <- neighbor
            neighbor <- nextNeighbor

            nextNeighbor <-
                env.GetNeighborByIdAndIdx tileId <| HexIndexUtil.next2Idx tileFaceIds.Length i
                |> Option.get

    /// 绘制平面六边形（有高度立面、处理接缝、但无特征、无河流）
    let private triangulatePlaneHex
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileCountId = tile.GetComponent<TileCountId>()
        let ids = Vector3.One * float32 tileCountId.CountId
        let tileCorners = tile.GetComponent<TileUnitCorners>()
        let tileFaceIds = tile.GetComponent<TileHexFaceIds>()
        let tileValue = tile.GetComponent<TileValue>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let height = env.GetHeight tileValue tileUnitCentroid
        let unitHeight = planetConfig.UnitHeight
        let waterHeight = TileValue.waterSurfaceY unitHeight tileValue
        let radius = planetConfig.Radius

        let v0 =
            tileCorners
            |> TileUnitCorners.getFirstCornerWithRadius tileUnitCentroid.UnitCentroid 0 (radius + height)

        let mutable v1 = v0

        let vw0 =
            tileCorners
            |> TileUnitCorners.getFirstCornerWithRadius tileUnitCentroid.UnitCentroid 0 (radius + waterHeight)

        let mutable vw1 = vw0

        for i in 0 .. tileFaceIds.Length - 1 do
            let v2 =
                tileCorners
                |> TileUnitCorners.getSecondCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + height)

            let vw2 =
                tileCorners
                |> TileUnitCorners.getSecondCornerWithRadius tileUnitCentroid.UnitCentroid i (radius + waterHeight)

            let neighbor = env.GetNeighborByIdAndIdx tileId i |> Option.get
            let nIds = Vector3(float32 tileId, float32 neighbor.Id, float32 tileId)
            let neighborValue = neighbor.GetComponent<TileValue>()
            let neighborUnitCentroid = neighbor.GetComponent<TileUnitCentroid>()
            let neighborHeight = env.GetHeight neighborValue neighborUnitCentroid
            let neighborWaterHeight = TileValue.waterSurfaceY unitHeight neighborValue
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
            if TileValue.isUnderwater tileValue && neighborWaterHeight < waterHeight then
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
            let neighborChunkId = neighbor.GetComponent<TileChunkId>()
            let tileChunkId = tile.GetComponent<TileChunkId>()

            if
                neighborChunkId <> tileChunkId
                && env.GetEntityById(neighborChunkId.ChunkId).GetComponent<ChunkLod>().Lod
                   >= ChunkLodEnum.SimpleHex
            then
                let n1Face =
                    env
                        .GetEntityById(TileHexFaceIds.item i tileFaceIds)
                        .GetComponent<FaceComponent>()

                let vn1 =
                    neighborUnitCentroid
                    |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                        n1Face.Center
                        (radius + neighborHeight)
                        HexMetrics.SolidFactor

                let n2Face =
                    env
                        .GetEntityById(TileHexFaceIds.item <| HexIndexUtil.next2Idx tileFaceIds.Length i <| tileFaceIds)
                        .GetComponent<FaceComponent>()

                let vn2 =
                    neighborUnitCentroid
                    |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                        n2Face.Center
                        (radius + neighborHeight)
                        HexMetrics.SolidFactor

                chunk
                    .GetTerrain()
                    .AddQuad(
                        [| v1; v2; vn1; vn2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = nIds
                    )

                if TileValue.isUnderwater tileValue then
                    let vnw1 =
                        neighborUnitCentroid
                        |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                            n1Face.Center
                            (radius + neighborWaterHeight)
                            HexMetrics.waterFactor

                    let vnw2 =
                        neighborUnitCentroid
                        |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                            n2Face.Center
                            (radius + neighborWaterHeight)
                            HexMetrics.waterFactor

                    chunk
                        .GetWater()
                        .AddQuad(
                            [| vw1; vw2; vnw1; vnw2 |] |> Array.map env.Perturb,
                            MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                            tis = nIds
                        )

            if i > 0 && i < tileFaceIds.Length - 1 then
                // 绘制地面
                chunk
                    .GetTerrain()
                    .AddTriangle(
                        [| v0; v1; v2 |] |> Array.map env.Perturb,
                        MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                        tis = ids
                    )
                // 绘制水面
                if TileValue.isUnderwater tileValue then
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
        (tileCountId: TileCountId)
        (simple: bool)
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
        (tileCountId: TileCountId)
        (tileValue: TileValue)
        (tileFlag: TileFlag)
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
            tileCountId
            e
            HexMeshConstant.weights1
            tileCountId
            false
            false

        triangulateEdgeFan env chunk centroid m tileCountId false

        if not <| TileValue.isUnderwater tileValue then
            let reversed = TileFlag.hasIncomingRiver tileFlag
            let ids = Vector3.One * float32 tileCountId.CountId
            let planetConfig = env.PlanetConfig

            let riverHeight =
                planetConfig.Radius + TileValue.riverSurfaceY planetConfig.UnitHeight tileValue

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
        (tileCountId: TileCountId)
        (tileValue: TileValue)
        (tileFlag: TileFlag)
        (tileUnitCorners: TileUnitCorners)
        (idx: int)
        (height: float32)
        (unitCentroid: Vector3)
        (centroid: Vector3)
        (e: EdgeVertices)
        =
        let planetConfig = env.PlanetConfig
        let radius = planetConfig.Radius

        let centerL, centerR =
            // 注意五边形没有对边的情况
            if
                tileUnitCorners.Length = 6
                && TileFlag.hasRiverThroughEdge
                   <| HexIndexUtil.oppositeIdx tileUnitCorners.Length idx
                   <| tileFlag
            then
                // 直线河流
                (tileUnitCorners
                 |> TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                     unitCentroid
                     (HexIndexUtil.previousIdx tileUnitCorners.Length idx)
                     (radius + height)
                     0.25f),
                (tileUnitCorners
                 |> TileUnitCorners.getSecondSolidCornerWithRadiusAndSize
                     unitCentroid
                     (HexIndexUtil.nextIdx tileUnitCorners.Length idx)
                     (radius + height)
                     0.25f)
            elif
                TileFlag.hasRiverThroughEdge
                <| HexIndexUtil.nextIdx tileUnitCorners.Length idx
                <| tileFlag
            then
                // 锐角弯
                centroid, centroid.Lerp(e.V5, 2f / 3f)
            elif
                TileFlag.hasRiverThroughEdge
                <| HexIndexUtil.previousIdx tileUnitCorners.Length idx
                <| tileFlag
            then
                // 锐角弯
                centroid.Lerp(e.V1, 2f / 3f), centroid
            elif
                TileFlag.hasRiverThroughEdge
                <| HexIndexUtil.next2Idx tileUnitCorners.Length idx
                <| tileFlag
            then
                // 钝角弯
                centroid,
                (tileUnitCorners
                 |> TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                     unitCentroid
                     (HexIndexUtil.nextIdx tileUnitCorners.Length idx)
                     (radius + height)
                     (0.5f * HexMetrics.InnerToOuter))
            elif
                TileFlag.hasRiverThroughEdge
                <| HexIndexUtil.previous2Idx tileUnitCorners.Length idx
                <| tileFlag
            then
                // 钝角弯
                (tileUnitCorners
                 |> TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                     unitCentroid
                     (HexIndexUtil.previousIdx tileUnitCorners.Length idx)
                     (radius + height)
                     (0.5f * HexMetrics.InnerToOuter)),
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
            tileCountId
            e
            HexMeshConstant.weights1
            tileCountId
            false
            false

        let ids = Vector3.One * float32 tileCountId.CountId
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

        if not <| TileValue.isUnderwater tileValue then
            let reversed = TileFlag.hasIncomingRiverThoughEdge idx tileFlag
            let riverHeight = TileValue.riverSurfaceY <| planetConfig.UnitHeight <| tileValue
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
        (tileCountId: TileCountId)
        (tileFlag: TileFlag)
        (tileUnitCorners: TileUnitCorners)
        (idx: int)
        (height: float32)
        (unitCentroid: Vector3)
        (centroid: Vector3)
        (e: EdgeVertices)
        =
        let cornerCount = tileUnitCorners.Length
        let hasRoadThroughEdge = TileFlag.hasRoadThroughEdge idx tileFlag

        let previousHasRiver =
            TileFlag.hasRiverThroughEdge
            <| HexIndexUtil.previousIdx cornerCount idx
            <| tileFlag

        let nextHasRiver =
            TileFlag.hasRiverThroughEdge <| HexIndexUtil.nextIdx cornerCount idx <| tileFlag

        let incomingRiverIdx = TileFlag.riverInDirection tileFlag
        let outgoingRiverIdx = TileFlag.riverOutDirection tileFlag
        let radius = env.PlanetConfig.Radius
        let mutable roadCenter = centroid
        let mutable centroid = centroid
        let mutable returnNow = false

        if TileFlag.hasRiverBeginOrEnd tileFlag then
            let riverBeginOrEndIdx =
                if TileFlag.hasIncomingRiver tileFlag then
                    incomingRiverIdx
                else
                    outgoingRiverIdx

            if tileUnitCorners.Length = 5 then
                roadCenter <-
                    tileUnitCorners
                    |> TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                        unitCentroid
                        (HexIndexUtil.oppositeIdx cornerCount riverBeginOrEndIdx)
                        (radius + height)
                        (HexMetrics.OuterToInner / 3f)
            else
                roadCenter <-
                    tileUnitCorners
                    |> TileUnitCorners.getFirstSolidCornerWithRadiusAndSize
                        unitCentroid
                        (HexIndexUtil.oppositeIdx cornerCount riverBeginOrEndIdx)
                        (radius + height)
                        (1f / 3f)
        elif
            tileUnitCorners.Length = 6
            && incomingRiverIdx = HexIndexUtil.oppositeIdx cornerCount outgoingRiverIdx
        then
            // 河流走势是对边（直线）的情况（需要注意五边形没有对边的概念）
            let mutable corner = Vector3.Zero

            if previousHasRiver then
                if
                    not hasRoadThroughEdge
                    && not
                       <| TileFlag.hasRoadThroughEdge (HexIndexUtil.nextIdx cornerCount idx) tileFlag
                then
                    returnNow <- true
                else
                    corner <-
                        tileUnitCorners
                        |> TileUnitCorners.getSecondSolidCornerWithRadius unitCentroid idx (radius + height)
            else if
                not hasRoadThroughEdge
                && not
                   <| TileFlag.hasRoadThroughEdge (HexIndexUtil.previousIdx cornerCount idx) tileFlag
            then
                returnNow <- true
            else
                corner <-
                    tileUnitCorners
                    |> TileUnitCorners.getSecondSolidCornerWithRadius unitCentroid idx (radius + height)

            if not returnNow then
                roadCenter <- roadCenter + (corner - centroid) * 0.5f
                centroid <- centroid + (corner - centroid) * 0.25f
        elif incomingRiverIdx = HexIndexUtil.previousIdx cornerCount outgoingRiverIdx then
            // 河流走势是逆时针锐角的情况
            roadCenter <-
                roadCenter
                - TileUnitCorners.getSecondCornerWithRadiusAndSize
                    unitCentroid
                    incomingRiverIdx
                    (radius + height)
                    0.2f
                    tileUnitCorners
                + centroid
        elif incomingRiverIdx = HexIndexUtil.nextIdx cornerCount outgoingRiverIdx then
            // 河流走势是顺时针锐角的情况
            roadCenter <-
                roadCenter
                - TileUnitCorners.getFirstCornerWithRadiusAndSize
                    unitCentroid
                    incomingRiverIdx
                    (radius + height)
                    0.2f
                    tileUnitCorners
                + centroid
        elif previousHasRiver && nextHasRiver then
            // 河流走势是钝角的情况，且当前方向被夹在河流出入角中间
            if not hasRoadThroughEdge then
                returnNow <- true
            else
                let offset =
                    tileUnitCorners
                    |> TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                        unitCentroid
                        idx
                        (radius + height)
                        HexMetrics.InnerToOuter

                roadCenter <- roadCenter + (offset - centroid) * 0.7f
                centroid <- centroid + (offset - centroid) * 0.5f
        elif tileUnitCorners.Length = 5 then
            // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：五边形有两个方向可能）
            let firstIdx =
                if previousHasRiver then
                    idx
                else
                    HexIndexUtil.previousIdx cornerCount idx // 两个可能方向中的顺时针第一个

            if
                not <| TileFlag.hasRoadThroughEdge firstIdx tileFlag
                && not
                   <| TileFlag.hasRoadThroughEdge (HexIndexUtil.nextIdx cornerCount firstIdx) tileFlag
            then
                returnNow <- true
            else
                let offset =
                    tileUnitCorners
                    |> TileUnitCorners.getSecondSolidCornerWithRadius unitCentroid firstIdx (radius + height)

                roadCenter <- roadCenter + (offset - centroid) * 0.25f * HexMetrics.OuterToInner
        else
            // 河流走势是钝角的情况，且当前方向在河流出入角外（即更宽阔的方向：六边形有三个方向可能）
            let middleIdx =
                if previousHasRiver then
                    HexIndexUtil.nextIdx cornerCount idx
                elif nextHasRiver then
                    HexIndexUtil.previousIdx cornerCount idx
                else
                    idx

            if
                not <| TileFlag.hasRoadThroughEdge middleIdx tileFlag
                && not
                   <| TileFlag.hasRoadThroughEdge (HexIndexUtil.previousIdx cornerCount middleIdx) tileFlag
                && not
                   <| TileFlag.hasRoadThroughEdge (HexIndexUtil.nextIdx cornerCount middleIdx) tileFlag
            then
                returnNow <- true
            else
                let offset =
                    tileUnitCorners
                    |> TileUnitCorners.getSolidEdgeMiddleWithRadius unitCentroid middleIdx (radius + height)

                roadCenter <- roadCenter + (offset - centroid) * 0.25f

        if not returnNow then
            let interpolator = TileFlag.getRoadInterpolator cornerCount idx tileFlag
            let mL = roadCenter.Lerp(e.V1, interpolator.X)
            let mR = roadCenter.Lerp(e.V5, interpolator.Y)
            triangulateRoad env chunk roadCenter mL mR e hasRoadThroughEdge tileCountId

            if previousHasRiver then
                triangulateRoadEdge env chunk roadCenter centroid mL tileCountId

            if nextHasRiver then
                triangulateRoadEdge env chunk roadCenter mR centroid tileCountId

    let private triangulateAdjacentToRiver
        (env: #IPlanetConfigQuery)
        (chunk: IChunk)
        (tileCountId: TileCountId)
        (tileFlag: TileFlag)
        (tileUnitCorners: TileUnitCorners)
        (idx: int)
        (height: float32)
        (unitCentroid: Vector3)
        (centroid: Vector3)
        (e: EdgeVertices)
        (simple: bool)
        =
        if TileFlag.hasRoads tileFlag then
            triangulateRoadAdjacentToRiver
                env
                chunk
                tileCountId
                tileFlag
                tileUnitCorners
                idx
                height
                unitCentroid
                centroid
                e

        let radius = env.PlanetConfig.Radius

        let centroid =
            if
                TileFlag.hasRiverThroughEdge
                <| HexIndexUtil.nextIdx tileUnitCorners.Length idx
                <| tileFlag
            then
                if
                    TileFlag.hasRiverThroughEdge
                    <| HexIndexUtil.previousIdx tileUnitCorners.Length idx
                    <| tileFlag
                then
                    tileUnitCorners
                    |> TileUnitCorners.getSolidEdgeMiddleWithRadiusAndSize
                        unitCentroid
                        idx
                        (radius + height)
                        (0.5f * HexMetrics.InnerToOuter)
                elif
                    tileUnitCorners.Length = 6
                    && TileFlag.hasRiverThroughEdge
                       <| HexIndexUtil.previous2Idx tileUnitCorners.Length idx
                       <| tileFlag
                then
                    // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                    tileUnitCorners
                    |> TileUnitCorners.getFirstSolidCornerWithRadiusAndSize unitCentroid idx (radius + height) 0.25f
                else
                    centroid
            elif
                tileUnitCorners.Length = 6
                && TileFlag.hasRiverThroughEdge
                   <| HexIndexUtil.previousIdx tileUnitCorners.Length idx
                   <| tileFlag
                && TileFlag.hasRiverThroughEdge
                   <| HexIndexUtil.next2Idx tileUnitCorners.Length idx
                   <| tileFlag
            then
                // 注意五边形没有直线河流，一边临河另一边隔一个方向临河的情况是对应钝角河的外河岸，依然在 centroid
                tileUnitCorners
                |> TileUnitCorners.getSecondSolidCornerWithRadiusAndSize unitCentroid idx (radius + height) 0.25f
            else
                centroid

        let m = EdgeVertices(centroid.Lerp(e.V1, 0.5f), centroid.Lerp(e.V5, 0.5f))

        triangulateEdgeStrip
            env
            chunk
            m
            HexMeshConstant.weights1
            tileCountId
            e
            HexMeshConstant.weights1
            tileCountId
            false
            simple

        triangulateEdgeFan env chunk centroid m tileCountId simple

    let private triangulateWithoutRiver
        env
        (chunk: IChunk)
        (tileCountId: TileCountId)
        (tileFlag: TileFlag)
        (tileUnitCorners: TileUnitCorners)
        (idx: int)
        (centroid: Vector3)
        (e: EdgeVertices)
        (simple: bool)
        =
        triangulateEdgeFan env chunk centroid e tileCountId simple

        if TileFlag.hasRoads tileFlag then
            let interpolator = TileFlag.getRoadInterpolator tileUnitCorners.Length idx tileFlag
            let mL = centroid.Lerp(e.V1, interpolator.X)
            let mR = centroid.Lerp(e.V5, interpolator.Y)
            let hasRoadThroughEdge = TileFlag.hasRoadThroughEdge idx tileFlag
            triangulateRoad env chunk centroid mL mR e hasRoadThroughEdge tileCountId

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
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (idx: int)
        (e: EdgeVertices)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileValue = tile.GetComponent<TileValue>()
        let tileFlag = tile.GetComponent<TileFlag>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let tileHeight = env.GetHeight tileValue tileUnitCentroid
        let neighbor = env.GetNeighborByIdAndIdx tileId idx |> Option.get
        let neighborValue = neighbor.GetComponent<TileValue>()
        let neighborUnitCentroid = neighbor.GetComponent<TileUnitCentroid>()
        let neighborHeight = env.GetHeight neighborValue neighborUnitCentroid
        let standardScale = planetConfig.StandardScale
        // 连接将由更低的地块或相同高度时 Id 更大的地块生成，或者是编辑地块与非编辑地块间的连接
        if
            tileHeight > neighborHeight
            || (Mathf.Abs(tileHeight - neighborHeight) < 0.0001f * standardScale
                && tileId < neighbor.Id)
        then
            () // 忽略
        else
            let tileFaceIds = tile.GetComponent<TileHexFaceIds>()

            let n1Face =
                env
                    .GetEntityById(TileHexFaceIds.item idx tileFaceIds)
                    .GetComponent<FaceComponent>()

            let radius = planetConfig.Radius
            let unitHeight = planetConfig.UnitHeight

            let vn1 =
                neighborUnitCentroid
                |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                    n1Face.Center
                    (radius + neighborHeight)
                    HexMetrics.SolidFactor

            let n2Face =
                env
                    .GetEntityById(
                        TileHexFaceIds.item
                        <| HexIndexUtil.nextIdx tileFaceIds.Length idx
                        <| tileFaceIds
                    )
                    .GetComponent<FaceComponent>()

            let vn2 =
                neighborUnitCentroid
                |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                    n2Face.Center
                    (radius + neighborHeight)
                    HexMetrics.SolidFactor

            let mutable en = EdgeVertices(vn1, vn2)
            let hasRiver = TileFlag.hasRiverThroughEdge idx tileFlag
            let hasRoad = TileFlag.hasRoadThroughEdge idx tileFlag
            let tileCountId = tile.GetComponent<TileCountId>()
            let neighborCountId = neighbor.GetComponent<TileCountId>()

            if hasRiver then
                en.V3 <- Math3dUtil.ProjectToSphere(en.V3, radius + TileValue.streamBedY unitHeight neighborValue)

                let ids =
                    Vector3(float32 tileCountId.CountId, float32 neighborCountId.CountId, float32 tileCountId.CountId)

                let tileRiverHeight = radius + TileValue.riverSurfaceY unitHeight tileValue
                let neighborRiverHeight = radius + TileValue.riverSurfaceY unitHeight neighborValue

                if not <| TileValue.isUnderwater tileValue then
                    if not <| TileValue.isUnderwater neighborValue then
                        let reversed =
                            TileFlag.hasIncomingRiver tileFlag
                            && TileFlag.hasIncomingRiverThoughEdge idx tileFlag

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
                    elif TileValue.elevation tileValue > TileValue.elevation neighborValue then
                        let neighborWaterHeight = radius + TileValue.waterSurfaceY unitHeight neighborValue

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
                    not <| TileValue.isUnderwater neighborValue
                    && TileValue.elevation neighborValue > TileValue.elevation tileValue
                then
                    let tileWaterHeight = radius + TileValue.waterSurfaceY unitHeight tileValue

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

            let tileChunkId = tile.GetComponent<TileChunkId>()

            let chunkLod = env.GetEntityById(tileChunkId.ChunkId).GetComponent<ChunkLod>().Lod

            if
                chunkLod > ChunkLodEnum.SimpleHex
                && TileValue.getEdgeType neighborValue tileValue = HexEdgeType.Slope
            then
                triangulateEdgeTerraces env chunk e tileCountId en neighborCountId hasRoad (not hasRiver && simple)
            else
                triangulateEdgeStrip
                    env
                    chunk
                    e
                    HexMeshConstant.weights1
                    tileCountId
                    en
                    HexMeshConstant.weights2
                    neighborCountId
                    hasRoad
                    (not hasRiver && simple)

            let preIdx = HexIndexUtil.previousIdx tileFaceIds.Length idx
            let preNeighbor = env.GetNeighborByIdAndIdx tileId preIdx |> Option.get
            let preNeighborValue = preNeighbor.GetComponent<TileValue>()
            let preNeighborUnitCentroid = preNeighbor.GetComponent<TileUnitCentroid>()
            let preNeighborCountId = preNeighbor.GetComponent<TileCountId>()
            let preNeighborHeight = env.GetHeight preNeighborValue preNeighborUnitCentroid
            // 连接角落的三角形由周围 3 个地块中最低或者一样高时 Id 最大的生成，或者是编辑地块与非编辑地块间的连接三角形
            if
                tileHeight < preNeighborHeight
                || (Mathf.Abs(tileHeight - preNeighborHeight) < 0.0001f * standardScale
                    && tileId > preNeighbor.Id)
            then
                let vpn =
                    preNeighborUnitCentroid
                    |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                        n1Face.Center
                        (radius + preNeighborHeight)
                        HexMetrics.SolidFactor

                triangulateCorner
                    env
                    chunk
                    chunkLod
                    e.V1
                    tileCountId
                    tileValue
                    vpn
                    preNeighborCountId
                    preNeighborValue
                    vn1
                    neighborCountId
                    neighborValue

    let private triangulateEstuary
        (env: #ICatlikeCodingNoiseQuery)
        (chunk: IChunk)
        (e1: EdgeVertices)
        (e2: EdgeVertices)
        (incomingRiver: bool)
        (ids: Vector3)
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
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (neighborId: TileId)
        (centroid: Vector3)
        (idx: int)
        (waterHeight: float32)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileUnitCorners = tile.GetComponent<TileUnitCorners>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let radius = planetConfig.Radius
        let unitHeight = planetConfig.UnitHeight

        let e1 =
            EdgeVertices(
                tileUnitCorners
                |> TileUnitCorners.getFirstWaterCornerWithRadius
                    tileUnitCentroid.UnitCentroid
                    idx
                    (radius + waterHeight),
                tileUnitCorners
                |> TileUnitCorners.getSecondWaterCornerWithRadius
                    tileUnitCentroid.UnitCentroid
                    idx
                    (radius + waterHeight)
            )

        let tileCountId = float32 <| tile.GetComponent<TileCountId>().CountId
        let neighbor = env.GetEntityById neighborId
        let neighborCountId = float32 <| neighbor.GetComponent<TileCountId>().CountId
        let mutable ids = Vector3(tileCountId, neighborCountId, tileCountId)
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
        let neighborValue = neighbor.GetComponent<TileValue>()
        let neighborUnitCentroid = neighbor.GetComponent<TileUnitCentroid>()
        let neighborWaterHeight = TileValue.waterSurfaceY unitHeight neighborValue
        let tileFaceIds = tile.GetComponent<TileHexFaceIds>()

        let n1Face =
            env
                .GetEntityById(TileHexFaceIds.item idx tileFaceIds)
                .GetComponent<FaceComponent>()

        let cn1 =
            neighborUnitCentroid
            |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                n1Face.Center
                (radius + neighborWaterHeight)
                HexMetrics.SolidFactor

        let n2Face =
            env
                .GetEntityById(
                    TileHexFaceIds.item
                    <| HexIndexUtil.nextIdx tileFaceIds.Length idx
                    <| tileFaceIds
                )
                .GetComponent<FaceComponent>()

        let cn2 =
            neighborUnitCentroid
            |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                n2Face.Center
                (radius + neighborWaterHeight)
                HexMetrics.SolidFactor

        let e2 = EdgeVertices(cn1, cn2)
        let neighborIdx = env.GetNeighborIdx tileId neighborId
        let waterShore = chunk.GetWaterShore()
        let tileFlag = tile.GetComponent<TileFlag>()

        let addWaterShoreQuad verticesArr =
            waterShore.AddQuad(
                verticesArr |> Array.map env.Perturb,
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                MeshUtil.quadUv 0f 0f 0f 1f,
                tis = ids
            )

        if simple then
            addWaterShoreQuad [| e1.V1; e1.V5; e2.V1; e2.V5 |]
        elif TileFlag.hasRiverThroughEdge neighborIdx tileFlag then
            triangulateEstuary env chunk e1 e2 (TileFlag.hasIncomingRiverThoughEdge neighborIdx tileFlag) ids
        else
            addWaterShoreQuad [| e1.V1; e1.V2; e2.V1; e2.V2 |]
            addWaterShoreQuad [| e1.V2; e1.V3; e2.V2; e2.V3 |]
            addWaterShoreQuad [| e1.V3; e1.V4; e2.V3; e2.V4 |]
            addWaterShoreQuad [| e1.V4; e1.V5; e2.V4; e2.V5 |]

        let nextNeighbor =
            env.GetNeighborByIdAndIdx tileId <| HexIndexUtil.nextIdx tileFaceIds.Length idx
            |> Option.get

        let nextNeighborValue = nextNeighbor.GetComponent<TileValue>()
        let nextNeighborWaterHeight = TileValue.waterSurfaceY unitHeight nextNeighborValue
        let nextNeighborUnitCentroid = nextNeighbor.GetComponent<TileUnitCentroid>()

        let cnn =
            nextNeighborUnitCentroid
            |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                n2Face.Center
                (radius + nextNeighborWaterHeight)
                (if TileValue.isUnderwater nextNeighborValue then
                     HexMetrics.waterFactor
                 else
                     HexMetrics.SolidFactor)

        let nextNeighborCountId = nextNeighbor.GetComponent<TileCountId>()
        ids.Z <- float32 nextNeighborCountId.CountId

        chunk
            .GetWaterShore()
            .AddTriangle(
                [| e1.V5; e2.V5; cnn |] |> Array.map env.Perturb,
                [| HexMeshConstant.weights1
                   HexMeshConstant.weights2
                   HexMeshConstant.weights3 |],
                [| Vector2(0f, 0f)
                   Vector2(0f, 1f)
                   Vector2(0f, if TileValue.isUnderwater nextNeighborValue then 0f else 1f) |],
                tis = ids
            )

    let private triangulateOpenWater
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (neighborId: TileId)
        (centroid: Vector3)
        (idx: int)
        (waterHeight: float32)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileCountId = tile.GetComponent<TileCountId>()
        let tileUnitCorners = tile.GetComponent<TileUnitCorners>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let radius = planetConfig.Radius
        let unitHeight = planetConfig.UnitHeight

        let c1 =
            tileUnitCorners
            |> TileUnitCorners.getFirstWaterCornerWithRadius tileUnitCentroid.UnitCentroid idx (radius + waterHeight)

        let c2 =
            tileUnitCorners
            |> TileUnitCorners.getSecondWaterCornerWithRadius tileUnitCentroid.UnitCentroid idx (radius + waterHeight)

        let mutable ids = Vector3.One * float32 tileCountId.CountId
        let water = chunk.GetWater()

        water.AddTriangle(
            [| centroid; c1; c2 |] |> Array.map env.Perturb,
            MeshUtil.triArr HexMeshConstant.weights1,
            tis = ids
        )
        // 由更大 Id 的地块绘制水域连接，或者是由编辑地块绘制和不编辑的邻接地块间的连接
        if tileId > neighborId then
            let neighbor = env.GetEntityById neighborId
            let neighborUnitCentroid = neighbor.GetComponent<TileUnitCentroid>()
            let neighborValue = neighbor.GetComponent<TileValue>()
            let neighborWaterHeight = TileValue.waterSurfaceY unitHeight neighborValue
            let tileFaceIds = tile.GetComponent<TileHexFaceIds>()

            let n1Face =
                env
                    .GetEntityById(TileHexFaceIds.item idx tileFaceIds)
                    .GetComponent<FaceComponent>()

            let cn1 =
                neighborUnitCentroid
                |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                    n1Face.Center
                    (radius + neighborWaterHeight)
                    HexMetrics.waterFactor

            let n2Face =
                env
                    .GetEntityById(
                        TileHexFaceIds.item
                        <| HexIndexUtil.nextIdx tileFaceIds.Length idx
                        <| tileFaceIds
                    )
                    .GetComponent<FaceComponent>()

            let cn2 =
                neighborUnitCentroid
                |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                    n2Face.Center
                    (radius + neighborWaterHeight)
                    HexMetrics.waterFactor

            let neighborCountId = neighbor.GetComponent<TileCountId>()
            ids.Y <- float32 neighborCountId.CountId

            water.AddQuad(
                [| c1; c2; cn1; cn2 |] |> Array.map env.Perturb,
                MeshUtil.quad2Arr HexMeshConstant.weights1 HexMeshConstant.weights2,
                tis = ids
            )
            // 由最大 Id 的地块绘制水域角落三角形，或者是由编辑地块绘制和不编辑的两个邻接地块间的连接
            let nextNeighbor =
                env.GetNeighborByIdAndIdx tileId <| HexIndexUtil.nextIdx tileFaceIds.Length idx
                |> Option.get

            let nextNeighborValue = nextNeighbor.GetComponent<TileValue>()

            if tileId > nextNeighbor.Id && TileValue.isUnderwater nextNeighborValue then
                let nextNeighborUnitCentroid = nextNeighbor.GetComponent<TileUnitCentroid>()

                let cnn =
                    nextNeighborUnitCentroid
                    |> TileUnitCentroid.getCornerByFaceCenterWithRadiusAndSize
                        n2Face.Center
                        (radius + TileValue.waterSurfaceY unitHeight nextNeighborValue)
                        HexMetrics.waterFactor

                let nextNeighborCountId = nextNeighbor.GetComponent<TileCountId>()
                ids.Z <- float32 nextNeighborCountId.CountId

                water.AddTriangle(
                    [| c2; cn2; cnn |] |> Array.map env.Perturb,
                    [| HexMeshConstant.weights1
                       HexMeshConstant.weights2
                       HexMeshConstant.weights3 |],
                    tis = ids
                )

    let private triangulateWater
        (env: 'E when 'E :> IPlanetConfigQuery and 'E :> IEntityStoreQuery and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (idx: int)
        (centroid: Vector3)
        (simple: bool)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileValue = tile.GetComponent<TileValue>()
        let waterHeight = TileValue.waterSurfaceY planetConfig.UnitHeight tileValue

        let centroid =
            Math3dUtil.ProjectToSphere(centroid, planetConfig.Radius + waterHeight)

        let neighbor = env.GetNeighborByIdAndIdx tileId idx |> Option.get
        let neighborValue = neighbor.GetComponent<TileValue>()

        if not <| TileValue.isUnderwater neighborValue then
            triangulateWaterShore env chunk tileId neighbor.Id centroid idx waterHeight simple
        else
            triangulateOpenWater env chunk tileId neighbor.Id centroid idx waterHeight

    /// Godot 缠绕顺序是正面顺时针，所以从 i1 对应角落到 i2 对应角落相对于 tile 重心需要是顺时针
    let private triangulateHex
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> ICatlikeCodingNoiseQuery
                and 'E :> IEntityStoreQuery
                and 'E :> IPointQuery)
        (chunk: IChunk)
        (tileId: TileId)
        (idx: int)
        =
        let planetConfig = env.PlanetConfig
        let tile = env.GetEntityById tileId
        let tileCountId = tile.GetComponent<TileCountId>()
        let tileUnitCorners = tile.GetComponent<TileUnitCorners>()
        let tileValue = tile.GetComponent<TileValue>()
        let tileUnitCentroid = tile.GetComponent<TileUnitCentroid>()
        let height = env.GetHeight tileValue tileUnitCentroid
        let radius = planetConfig.Radius
        let unitHeight = planetConfig.UnitHeight

        let v1 =
            tileUnitCorners
            |> TileUnitCorners.getFirstSolidCornerWithRadius tileUnitCentroid.UnitCentroid idx (radius + height)

        let v2 =
            tileUnitCorners
            |> TileUnitCorners.getSecondSolidCornerWithRadius tileUnitCentroid.UnitCentroid idx (radius + height)

        let mutable e = EdgeVertices(v1, v2)
        let centroid = TileUnitCentroid.scaled <| radius + height <| tileUnitCentroid
        let tileChunkId = tile.GetComponent<TileChunkId>()

        let chunkLod = env.GetEntityById(tileChunkId.ChunkId).GetComponent<ChunkLod>().Lod

        let simple =
            if chunkLod = ChunkLodEnum.Full then
                false
            else
                let neighbor = env.GetNeighborByIdAndIdx tileId idx |> Option.get
                let neighborChunkId = neighbor.GetComponent<TileChunkId>()
                let tileChunkId = tile.GetComponent<TileChunkId>()

                if neighborChunkId = tileChunkId then
                    true
                else
                    let neighborChunk = env.GetEntityById neighborChunkId.ChunkId
                    let neighborChunkLod = neighborChunk.GetComponent<ChunkLod>().Lod
                    neighborChunkLod < ChunkLodEnum.Full

        let tileFlag = tile.GetComponent<TileFlag>()

        if TileFlag.hasRivers tileFlag then
            if TileFlag.hasRiverThroughEdge idx tileFlag then
                e.V3 <- Math3dUtil.ProjectToSphere(e.V3, radius + TileValue.streamBedY unitHeight tileValue)

                if TileFlag.hasRiverBeginOrEnd tileFlag then
                    triangulateWithRiverBeginOrEnd env chunk tileCountId tileValue tileFlag centroid e
                else
                    triangulateWithRiver
                        env
                        chunk
                        tileCountId
                        tileValue
                        tileFlag
                        tileUnitCorners
                        idx
                        height
                        tileUnitCentroid.UnitCentroid
                        centroid
                        e
            else
                triangulateAdjacentToRiver
                    env
                    chunk
                    tileCountId
                    tileFlag
                    tileUnitCorners
                    idx
                    height
                    tileUnitCentroid.UnitCentroid
                    centroid
                    e
                    simple
        else
            triangulateWithoutRiver env chunk tileCountId tileFlag tileUnitCorners idx centroid e simple

        triangulateConnection env chunk tileId idx e simple

        if TileValue.isUnderwater tileValue then
            triangulateWater env chunk tileId idx centroid simple

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
