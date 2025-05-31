namespace TO.FSharp.Apps

open Friflo.Engine.ECS
open Godot
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
    let chunkVpTree = VpTree<Vector3>()
    let tileVpTree = VpTree<Vector3>()

    let chunkDep =
        { Store = store
          TagChunk = Tags.Get<TagChunk>()
          TagTile = Tags.Get<TagTile>() }

    let pointRepoDep = PointRepo.getDependency chunkDep chunkVpTree tileVpTree
    let faceRepoDep = FaceRepo.getDependency chunkDep
    let tileRepoDep = TileRepo.getDependency store
    let chunkRepoDep = ChunkRepo.getDependency store

    let hexSphereServiceDep =
        HexSphereService.getDependency pointRepoDep faceRepoDep tileRepoDep chunkRepoDep

    member this.DrawHexSphereMesh() =
        let time = Time.GetTicksMsec()

        GD.Print
            $"[===DrawHexSphereMesh===] radius {planet.Radius},
        divisions {planet.Divisions}, start at: {time}"

        hexSphereServiceDep.InitHexSphere planet
