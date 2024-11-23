namespace FrontEndToolFS.HexGlobal

open HexDependency
open FSharp.Control.Reactive
open FSharpPlus.Data

type HexEntry
    (
        subdivisions: int,
        chunkSubdivision: int,
        radius: float32,
        minHeight: float32,
        maxHeight: float32,
        noiseScaling: float32,
        octaves: int,
        lacunarity: float32,
        persistence: float32
    ) =
    /// 星球状态
    let planetSubject = Subject.behavior Repository.emptyPlanet

    let injectorSubject =
        Subject.behavior
            {
              // 属性
              Subdivisions = subdivisions
              ChunkSubdivision = chunkSubdivision
              Radius = radius
              MinHeight = minHeight
              MaxHeight = maxHeight
              NoiseScaling = noiseScaling
              Octaves = octaves
              Lacunarity = lacunarity
              Persistence = persistence
              // 仓储
              HexTileFactory = Repository.insertHexTile
              HexTileUpdater = Repository.updateHexTile
              HexTileQueryById = Repository.getHexTile
              HexChunkFactory = Repository.insertHexChunk
              HexChunkUpdater = Repository.updateHexChunk
              HexChunkQueryById = Repository.getHexChunk }

    /// StateT Reader 星球状态更新器
    let planetUpdater resultHandler updater =
        let res, planet' =
            updater |> StateT.run <| planetSubject.Value |> Reader.run
            <| injectorSubject.Value

        planetSubject.OnNext planet'
        resultHandler res

    /// OptionT StateT Reader 星球状态更新器
    let planetOptionUpdater
        resultHandler
        defaultVal
        (updater: OptionT<StateT<Repository.Planet, Reader<Injector<Repository.Planet>, 'a option * Repository.Planet>>>)
        =
        let opt, gameState' =
            updater |> OptionT.run |> StateT.run <| planetSubject.Value |> Reader.run
            <| injectorSubject.Value

        if opt.IsSome then
            planetSubject.OnNext gameState'
            resultHandler opt.Value
        else
            defaultVal
    
    member this.GeneratePlanetTilesAndChunks() =
        Generator.generatePlanetTilesAndChunks
        |> planetUpdater snd

    member this.GetHexChunkMesh chunkId =
        Generator.getChunkMesh chunkId |> planetOptionUpdater id null
