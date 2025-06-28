namespace TO.Controllers.Services.Planets

open TO.Presenters.Models.Planets
open TO.Repos.Commands.HexSpheres
open TO.Repos.Commands.PathFindings
open TO.Repos.Commands.Shaders

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 12:46:30
module HexSphereService =
    let initHexSphere
        (env:
            'E
                when 'E :> IPlanetQuery
                and 'E :> IHexSphereInitCommand
                and 'E :> ITileSearcherCommand
                and 'E :> ITileShaderDataCommand)
        =
        env.InitChunks
        <| env.GetChunkDivisions()
        <| env.GetRadius() + env.GetMaxHeight()

        env.InitTiles <| env.GetDivisions()
        env.InitShaderData <| env.GetDivisions()
        env.InitSearchData()
