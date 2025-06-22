namespace TO.Controllers.Services.Planets

open Friflo.Engine.ECS
open TO.Abstractions.Models.Planets
open TO.Domains.Components.HexSpheres.Tiles
open TO.Repos.Commands.HexSpheres
open TO.Repos.Data.PathFindings
open TO.Repos.Data.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let initHexSphere
        (planet: IPlanet)
        (store: EntityStore)
        (tileShaderData: TileShaderData)
        (tileSearcher: TileSearcher)
        (repoEnv: #IHexSphereInitCommand)
        =
        repoEnv.InitChunks planet.ChunkDivisions <| planet.Radius + planet.MaxHeight
        repoEnv.InitTiles planet.Divisions
        tileShaderData.Initialize planet.Divisions
        tileSearcher.InitSearchData
        <| store.Query<TileUnitCentroid>().Count
