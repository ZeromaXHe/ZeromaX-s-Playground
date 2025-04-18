using Domains.Models.Entities.PlanetGenerates;

namespace Domains.Services.Abstractions.Events;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-13 07:39:24
public class TileShaderEvent
{
    public static TileShaderEvent Instance { get; } = new();

    public delegate void RangeVisibilityIncreasedEvent(Tile tile, int range);

    public event RangeVisibilityIncreasedEvent? RangeVisibilityIncreased;

    public static void EmitRangeVisibilityIncreased(Tile tile, int range) =>
        Instance.RangeVisibilityIncreased?.Invoke(tile, range);

    // 对应第一次增加可视度（Visibility）
    public delegate void TileExploredEvent(Tile tile);

    public event TileExploredEvent? TileExplored;

    public static void EmitTileExplored(Tile tile) => Instance.TileExplored?.Invoke(tile);
}