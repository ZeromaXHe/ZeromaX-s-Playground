namespace TO.FSharp.Apps

open System
open Friflo.Engine.ECS
open Godot
open Microsoft.FSharp.Core.LanguagePrimitives
open TO.FSharp.Commons.DataStructures
open TO.FSharp.GodotAbstractions.Extensions.Planets
open TO.FSharp.Repos.Functions
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Services.Functions

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(planet: IPlanet) =
    let store = EntityStore()

    let truncatePoints = PointRepo.truncate store
    let truncateFaces = FaceRepo.truncate store
    let truncateTiles = TileRepo.truncate store
    let truncateChunks = ChunkRepo.truncate store

    let clearOldData =
        HexSphereService.clearOldData truncatePoints truncateFaces truncateTiles truncateChunks

    let chunkDep =
        { Store = store
          TagChunk = Tags.Get<TagChunk>()
          TagTile = Tags.Get<TagTile>() }

    let forEachFaceByChunky = FaceRepo.forEachByChunky chunkDep
    let forEachPointByChunky = PointRepo.forEachByChunky chunkDep
    let chunkVpTree = VpTree<Vector3>()
    let tileVpTree = VpTree<Vector3>()
    let searchNearestCenterPos = PointRepo.searchNearestCenterPos chunkVpTree tileVpTree
    let tryHeadPointByPosition = PointRepo.tryHeadByPosition chunkDep
    let tryHeadChunkByCenterId = ChunkRepo.tryHeadByCenterId store
    let tryHeadTileByCenterId =  TileRepo.tryHeadByCenterId store
    let getOrderedFaces = FaceRepo.getOrderedFaces store
    let getNeighborCenterPointIds =  PointRepo.getNeighborCenterPointIds chunkDep
    let createVpTree = PointRepo.createVpTree chunkDep chunkVpTree tileVpTree
    let addPoint = PointRepo.add chunkDep
    let addFace = FaceRepo.add chunkDep
    let addTile = TileRepo.add store
    let addChunk = ChunkRepo.add store
    let allTilesSeq = TileRepo.allSeq store

    let initHexSphere () =
        HexSphereService.initHexSphere
            planet
            forEachFaceByChunky
            forEachPointByChunky
            searchNearestCenterPos
            tryHeadPointByPosition
            tryHeadChunkByCenterId
            tryHeadTileByCenterId
            getOrderedFaces
            getNeighborCenterPointIds
            createVpTree
            addPoint
            addFace
            addTile
            addChunk
            allTilesSeq

    let addFaceIndex (surfaceTool: SurfaceTool) i0 i1 i2 =
        surfaceTool.AddIndex i0
        surfaceTool.AddIndex i1
        surfaceTool.AddIndex i2

    let generateMesh (tiles: TileComponent seq) =
        for child in planet.GetChildren() do
            child.QueueFree()

        let meshIns = new MeshInstance3D()
        let surfaceTool = new SurfaceTool()
        surfaceTool.Begin Mesh.PrimitiveType.Triangles
        surfaceTool.SetSmoothGroup UInt32.MaxValue
        let mutable vi = 0

        for tile in tiles do
            surfaceTool.SetColor <| Color.FromHsv(GD.Randf(), GD.Randf(), GD.Randf())
            let points = tile.GetCorners planet.Radius

            for point in points do
                surfaceTool.AddVertex point

            addFaceIndex surfaceTool vi <| vi + 1 <| vi + 2
            addFaceIndex surfaceTool vi <| vi + 2 <| vi + 3
            addFaceIndex surfaceTool vi <| vi + 3 <| vi + 4

            if points.Length > 5 then
                addFaceIndex surfaceTool vi <| vi + 4 <| vi + 5

            vi <- vi + points.Length

        surfaceTool.GenerateNormals()
        let material = new StandardMaterial3D()
        material.VertexColorUseAsAlbedo <- true
        surfaceTool.SetMaterial material
        meshIns.Mesh <- surfaceTool.Commit()
        // 貌似不能只传一个参数，尴尬……
        planet.AddChild(meshIns, false, EnumOfValue<int64, Node.InternalMode> 0L)

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()

        GD.Print
            $"[===DrawHexSphereMesh===] radius {planet.Radius},
        divisions {planet.Divisions}, start at: {time}"

        clearOldData ()
        let tiles = initHexSphere ()
        generateMesh tiles
