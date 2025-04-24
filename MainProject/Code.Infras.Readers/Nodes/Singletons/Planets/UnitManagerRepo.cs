using Infras.Readers.Abstractions.Nodes.Singletons.Planets;
using Infras.Readers.Bases;
using Nodes.Abstractions.Planets;

namespace Infras.Readers.Nodes.Singletons.Planets;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:42:06
public class UnitManagerRepo : SingletonNodeRepo<IUnitManager>, IUnitManagerRepo
{
    public event Action? PathFromTileIdSetZero;
    private void OnPathFromTileIdSetZero() => PathFromTileIdSetZero?.Invoke();

    protected override void ConnectNodeEvents()
    {
        Singleton!.PathFromTileIdSetZero += OnPathFromTileIdSetZero;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.PathFromTileIdSetZero -= OnPathFromTileIdSetZero;
    }
}