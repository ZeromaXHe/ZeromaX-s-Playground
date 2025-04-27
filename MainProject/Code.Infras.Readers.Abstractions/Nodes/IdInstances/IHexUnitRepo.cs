using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-27 15:45:27
public interface IHexUnitRepo : IIdInstanceNodeRepo<IHexUnit>
{
    delegate void TileIdChangedEvent(IHexUnit unit, int pre, int now);
    event TileIdChangedEvent? TileIdChanged;
    event Action<IHexUnit>? Died;
}