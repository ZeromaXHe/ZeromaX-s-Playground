namespace TO.FSharp.Apps.Planets

open Friflo.Engine.ECS
open Godot
open Godot.Abstractions.Extensions.Planets
open TO.FSharp.Commons.DataStructures
open TO.FSharp.Repos.Functions
open TO.FSharp.Repos.Models.HexSpheres.Chunks
open TO.FSharp.Repos.Models.HexSpheres.Tiles
open TO.FSharp.Services.Functions

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 19:41:30
type PlanetApp(store: EntityStore, chunkVpTree: Vector3 VpTree, tileVpTree: Vector3 VpTree) =
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

    member this.DrawHexSphereMesh(planet: IPlanet) =
        let time = Time.GetTicksMsec()

        GD.Print
            $"[===DrawHexSphereMesh===] radius {planet.Radius},
        divisions {planet.Divisions}, start at: {time}"

        hexSphereServiceDep.InitHexSphere planet
