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
        (preEnv: #IPlanetQuery)
        (repoEnv:
            'RE
                when 'RE :> IHexSphereInitCommand
                and 'RE :> ITileSearcherCommand
                and 'RE :> ITileShaderDataCommand)
        =
        repoEnv.InitChunks
        <| preEnv.GetChunkDivisions()
        <| preEnv.GetRadius() + preEnv.GetMaxHeight()

        repoEnv.InitTiles <| preEnv.GetDivisions()
        repoEnv.InitShaderData <| preEnv.GetDivisions()
        repoEnv.InitSearchData()
