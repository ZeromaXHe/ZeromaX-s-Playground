using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 20:03:18
public interface IMiniMapManagerRepo : ISingletonNodeRepo<IMiniMapManager>
{
    // 必须重新实现与节点事件同名的事件，因为节点自己的生命周期和 Repo 不同。单例 Repo 才能保证非空
    delegate void ClickedEvent(Vector3 posDirection);

    event ClickedEvent? Clicked;
}