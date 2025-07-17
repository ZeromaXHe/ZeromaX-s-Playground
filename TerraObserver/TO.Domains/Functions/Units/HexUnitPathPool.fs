namespace TO.Domains.Functions.Units

open TO.Domains.Types.HexSpheres
open TO.Domains.Types.Units

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-11 09:47:11
module HexUnitPathPoolCommand =
    let private fetchPath (pool: IHexUnitPathPool) =
        let path =
            pool.Paths
            |> Seq.tryFind (fun p -> not p.Working)
            |> Option.defaultWith (fun () ->
                let p = pool.InstantiatePath()
                pool.Paths.Add p
                p)

        path.Working <- true
        path

    let newUnitPathTask (env: #IHexUnitPathPoolQuery) : NewUnitPathTask =
        fun (pathTiles: TileId array) ->
            let pathOpt = env.HexUnitPathPoolOpt |> Option.map fetchPath
            pathOpt |> Option.iter (HexUnitPathCommand.unitPathTaskStart env pathTiles)
            pathOpt
