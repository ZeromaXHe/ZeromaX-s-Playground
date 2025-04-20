using Godot;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:04:30
public class MiniMapManagerRepo : SingletonNodeRepo<IMiniMapManager>, IMiniMapManagerRepo
{
    public event IMiniMapManagerRepo.ClickedEvent? Clicked;

    protected override void ConnectNodeEvents()
    {
        Singleton!.Clicked += OnClicked;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.Clicked -= OnClicked;
    }

    private void OnClicked(Vector3 posDir) => Clicked?.Invoke(posDir);
}