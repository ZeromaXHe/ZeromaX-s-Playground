using Godot;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:35:17
public interface IHexUnit : ICsgBox3D
{
    delegate void TileIdChangedEvent(int pre, int now);

    event TileIdChangedEvent? TileIdChanged;
    event Action? Died;
    int Id { get; set; }
    int TileId { get; set; }
    Vector3 BeginRotation { get; set; }
    IHexUnitPath? Path { get; set; }
    int PathTileIdx { get; set; }

    bool PathOriented { get; set; }

    // 修改 TileId, 并不发出 TileIdChanged 信号
    void SetTravelTileId(int id);
    void FinishPath();
    void Die();
}