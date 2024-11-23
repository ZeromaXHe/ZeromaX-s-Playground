namespace FrontEndToolFS.HexGlobal

open Domain

module HexDependency =
    type Injector<'s> =
        {
          // 属性
          Subdivisions: int
          ChunkSubdivision: int
          Radius: float32
          MinHeight: float32
          MaxHeight: float32
          NoiseScaling: float32
          Octaves: int
          Lacunarity: float32
          Persistence: float32
          // 仓储
          HexTileFactory: HexTileFactory<'s>
          HexTileUpdater: HexTileUpdater<'s>
          HexTileQueryById: HexTileQueryById<'s>
          HexChunkFactory: HexChunkFactory<'s>
          HexChunkUpdater: HexChunkUpdater<'s>
          HexChunkQueryById: HexChunkQueryById<'s> }
