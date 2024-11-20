namespace FrontEnd4IdleStrategyFS.PreBack.HexGlobal

open Godot
open FSharpPlus.Data

module Domain =

    /// 六边形地块
    type HexTile =
        { Id: int
          NeighborIDs: int list
          ChunkId: int option
          Center: Vector3
          Vertices: Vector3 list
          Height: float32
          Color: Color }

    /// 六边形地块工厂
    type HexTileFactory<'s> = Vector3 -> Vector3 list -> State<'s, HexTile>
    /// 保存六边形地块
    type HexTileUpdater<'s> = HexTile -> State<'s, HexTile>
    /// 按 Id 查询六边形地块
    type HexTileQueryById<'r> = int -> State<'r, HexTile option>

    // 六边形组块
    type HexChunk =
        { Id: int
          Origin: Vector3
          TileIds: int list }

    /// 六边形组块工厂
    type HexChunkFactory<'s> = Vector3 -> State<'s, HexChunk>
    /// 保存六边形组块
    type HexChunkUpdater<'s> = HexChunk -> State<'s, HexChunk>
    /// 按 Id 查询六边形组块
    type HexChunkQueryById<'r> = int -> State<'r, HexChunk option>
