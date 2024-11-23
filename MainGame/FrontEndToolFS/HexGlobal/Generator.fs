namespace FrontEndToolFS.HexGlobal

open FrontEndCommonFS.DataStructure
open FrontEndCommonFS.Util
open Domain
open HexDependency
open Godot
open FSharpPlus
open FSharpPlus.Data

module Generator =
    let mutable generating = false

    let getClosestPoints center (toVec3: 'a -> Vector3) (points: 'a seq) =
        let closest7 =
            points
            |> Seq.sortBy (fun t -> (toVec3 t - toVec3 center).LengthSquared())
            |> Seq.truncate 7 // take 7 不行，因为 6 个点的情况 take 会报错
            |> Seq.toList

        if
            closest7.Length < 7
            || (toVec3 closest7[6] - toVec3 center).Length() > (toVec3 closest7[5] - toVec3 center).Length() * 1.5f
        then
            closest7 |> List.take 6 // 第七个点比第六个点远了 1.5 倍的话，截取六个点（五边形）
        else
            closest7
        |> List.skip 1 // 排除掉自己

    let generateTile vertOctree maxDistBetweenNeighbors vert =
        let angleAxis = Vector3.Up + Vector3.One * 0.1f

        let closest =
            Octree.getPoints vert (Vector3.One * maxDistBetweenNeighbors) vertOctree
            |> getClosestPoints vert id // 小心 id 是 F# 自带的单位元（identity）函数，别随便用这个命名
            // 按随索引增加，逆时针排序的顺序排列最近的点
            // BUG: This is a hack and might result in bugs
            |> List.sortBy (fun v -> -(v - vert).SignedAngleTo(angleAxis, vert))

        let hexVert j =
            let v = vert.Lerp(closest[j], 0.66666666f)
            let vPlus1 = vert.Lerp(closest[(j + 1) % closest.Length], 0.66666666f)
            v.Lerp(vPlus1, 0.5f)

        let hexVerts = [ 0 .. closest.Length - 1 ] |> List.map hexVert

        let center = List.sum hexVerts / float32 hexVerts.Length

        monad {
            let! di = Reader.ask |> StateT.lift
            return! di.HexTileFactory center hexVerts |> StateT.hoist
        }

    let fillTileNeighbors (hexOctree: Octree.Tree<HexTile>) maxDistBetweenNeighbors (tile: HexTile) =
        let closest =
            Octree.getPoints tile.Center (Vector3.One * maxDistBetweenNeighbors) hexOctree
            |> getClosestPoints tile _.Center

        monad {
            let! di = Reader.ask |> StateT.lift

            return!
                di.HexTileUpdater
                    { tile with
                        NeighborIDs =
                            tile.Vertices
                            |> List.mapi (fun i v ->
                                closest
                                |> List.sortBy (fun t ->
                                    -((v + tile.Vertices[(i + 1) % tile.Vertices.Length]) / 2f)
                                        .Normalized()
                                        .Dot(t.Center.Normalized())))
                            |> List.filter (fun tiles -> tiles.Length > 0)
                            |> List.map _.Head.Id }
                |> StateT.hoist
        }

    let generateHexTiles verts =
        monad {
            let! di = Reader.ask |> StateT.lift

            let defaultOctree =
                Octree.initDefault (di.Radius * -1.1f * Vector3.One) (di.Radius * 1.1f * Vector3.One)
            // 使用所有将成为六边形的中心的点，构建八叉树
            let vertOctree =
                verts
                |> List.fold (fun octree v -> Octree.insertPoint (BackEndUtil.to3 v) v octree) defaultOctree
            // 两个相邻地块的最大距离
            let maxDistBetweenNeighbors =
                verts
                |> List.map (fun v -> (v - verts[verts.Length - 1]).LengthSquared())
                |> List.sort
                |> List.take 7
                |> fun l -> l[6] * 1.2f
                |> Mathf.Sqrt
            // 构建瓦片
            let! hexTiles = verts |> List.traverse (generateTile vertOctree maxDistBetweenNeighbors)
            // 使用将成为六边形中心的所有点作为 key，HexTile 作为 value，来构建一个 HexTile 八叉树
            let defaultOctree =
                Octree.initDefault (di.Radius * -1.1f * Vector3.One) (di.Radius * 1.1f * Vector3.One)

            let hexOctree =
                hexTiles
                |> List.fold
                    (fun octree tile -> Octree.insertPoint (BackEndUtil.to3 tile.Center) tile octree)
                    defaultOctree
            // 找到相邻地块
            let! hexTiles' = hexTiles |> List.traverse (fillTileNeighbors hexOctree maxDistBetweenNeighbors)

            return!
                hexTiles'
                |> List.traverse (fun t ->
                    let height, color =
                        PerlinTerrain.heightColor
                            di.MaxHeight
                            di.MinHeight
                            di.Octaves
                            di.NoiseScaling
                            di.Lacunarity
                            di.Persistence
                            t.Center

                    di.HexTileUpdater
                        { t with
                            Height = height
                            Color = color })
                |> StateT.hoist
        }

    let generateHexChunks (tiles: HexTile list) chunkOrigins =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! chunks = chunkOrigins |> List.traverse di.HexChunkFactory |> StateT.hoist

            let! tiles' =
                tiles
                |> List.traverse (fun tile ->
                    let chunkId =
                        chunks
                        |> List.sortBy (fun c -> (tile.Center - c.Origin).LengthSquared())
                        |> List.head
                        |> _.Id

                    di.HexTileUpdater { tile with ChunkId = Some chunkId })
                |> StateT.hoist

            let chunkIdToTilesMap =
                tiles'
                |> List.filter _.ChunkId.IsSome
                |> List.groupBy _.ChunkId.Value
                |> Map.ofList

            let! chunks' =
                chunks
                |> List.traverse (fun c ->
                    di.HexChunkUpdater
                        { c with
                            TileIds = chunkIdToTilesMap[c.Id] |> List.map _.Id })
                |> StateT.hoist

            tiles', chunks'
        }

    let generatePlanetTilesAndChunks<'s> =
        monad {
            let! (di: Injector<'s>) = Reader.ask |> StateT.lift
            let points = GeodesicPoints.genPoints di.Subdivisions di.Radius
            let! tiles = generateHexTiles points
            let chunkOrigins = GeodesicPoints.genPoints di.ChunkSubdivision di.Radius
            return! generateHexChunks tiles chunkOrigins
        }

    let transformPoint (input: Vector3) (height: float32) =
        monad {
            let! di = Reader.ask
            input * (1.0f + height / di.Radius)
        }

    let getTileNeighbors tile =
        monad {
            let! di = Reader.ask |> StateT.lift
            return! tile.NeighborIDs |> List.traverse di.HexTileQueryById |> StateT.hoist
        }

    let generateWall (tile: HexTile) i (neighborOpt: HexTile option) vciTuple =
        monad {
            let (vertices: Vector3 list, colors, indices) = vciTuple
            let thisHeight = tile.Height

            let otherHeight =
                neighborOpt |> Option.map _.Height |> Option.defaultValue tile.Height

            if thisHeight >= otherHeight then
                vciTuple
            else
                let baseIndex = vertices.Length
                let! v1 = transformPoint tile.Vertices[(i + 1) % tile.Vertices.Length] otherHeight
                let! v2 = transformPoint tile.Vertices[i] otherHeight
                let! v3 = transformPoint tile.Vertices[(i + 1) % tile.Vertices.Length] thisHeight
                let! v4 = transformPoint tile.Vertices[i] thisHeight
                let vertices = vertices @ [ v1; v2; v3; v4 ]
                let colors = colors @ List.replicate 4 tile.Color

                let indices =
                    indices
                    @ [ baseIndex
                        baseIndex + 1
                        baseIndex + 2
                        baseIndex + 2
                        baseIndex + 1
                        baseIndex + 3 ]

                vertices, colors, indices
        }

    let generateHex (tile: HexTile) vciTuple =
        monad {
            let (vertices: Vector3 list, colors, indices) = vciTuple
            let baseIndex = vertices.Length
            let! (vAppend: Vector3) = transformPoint tile.Center tile.Height
            let! vAppend2 = tile.Vertices |> List.traverse (fun v -> transformPoint v tile.Height)
            let vertices = vertices @ (vAppend :: vAppend2)

            let colors = colors @ List.replicate (tile.Vertices.Length + 1) tile.Color

            let indices, _ =
                tile.Vertices
                |> List.fold
                    (fun (is, j) _ ->
                        let is' =
                            is
                            @ [ baseIndex; baseIndex + (j + 1) % tile.Vertices.Length + 1; baseIndex + j + 1 ]

                        let j' = j + 1
                        is', j')
                    (indices, 0)

            vertices, colors, indices
        }

    let appendTileHexWallVci (tile: HexTile) vciTuple =
        monad {
            // 生成六边形基础
            let! vciTuple = generateHex tile vciTuple |> StateT.lift
            // 如果需要生成墙壁
            let! neighborOpts = getTileNeighbors tile

            let generateWalls =
                neighborOpts |> List.mapi (generateWall tile) |> List.reduce (>=>)

            return! generateWalls vciTuple |> StateT.lift
        }

    let getMesh chunk =
        monad {
            let! di = Reader.ask |> StateT.lift
            let! (tileOpts: HexTile option list) = chunk.TileIds |> List.traverse di.HexTileQueryById |> StateT.hoist
            let tiles = tileOpts |> List.filter _.IsSome |> List.map _.Value
            let getVciTuple = tiles |> List.map appendTileHexWallVci |> List.reduce (>=>)
            // TODO：List 后续改造成 Array 会不会快一些？
            let! vertices, colors, indices = getVciTuple (List.empty, List.empty, List.empty)
            // 构造 Mesh
            let surfaceTool = new SurfaceTool()
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles)

            for color in colors do
                // BUG: 其实这样循环设置颜色没用，只是设置给下一个插入的端点颜色。只是目前颜色都是一样的，所以暂时没问题
                surfaceTool.SetColor color

            for vertex in vertices do
                surfaceTool.AddVertex vertex

            for index in indices do
                surfaceTool.AddIndex index

            let material = new StandardMaterial3D()
            material.VertexColorUseAsAlbedo <- true
            surfaceTool.SetMaterial material
            surfaceTool.GenerateNormals()

            let mesh = surfaceTool.Commit()
            mesh
        }

    let getChunkMesh chunkId =
        monad {
            let! di = Reader.ask |> StateT.lift |> OptionT.lift
            let! chunk = di.HexChunkQueryById chunkId |> StateT.hoist |> OptionT
            return! getMesh chunk |> OptionT.lift
        }
