using GodotNodes.Abstractions;

namespace Nodes.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 10:31:17
public interface IChunkManager : INode3D
{
    void InitChunkNodes();
    void ClearOldData();
}