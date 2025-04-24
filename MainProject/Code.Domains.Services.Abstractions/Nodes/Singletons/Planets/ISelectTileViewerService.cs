using Godot;

namespace Domains.Services.Abstractions.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:10:18
public interface ISelectTileViewerService
{
    void ClearPath();
    void Update(int pathFromTileId, Vector3 position);
}