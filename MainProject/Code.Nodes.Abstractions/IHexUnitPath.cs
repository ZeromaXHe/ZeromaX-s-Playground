using Domains.Models.Entities.PlanetGenerates;
using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:35:17
public interface IHexUnitPath : IPath3D
{
    List<Tile>? Tiles { get; }
    List<float>? Progresses { get; }
    void StartMove(IHexUnit unit);
    float GetProgress();
}