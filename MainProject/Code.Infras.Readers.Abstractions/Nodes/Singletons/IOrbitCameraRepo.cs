using Godot;
using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:23:18
public interface IOrbitCameraRepo : ISingletonNodeRepo<IOrbitCamera>
{
    // 必须重新实现与节点事件同名的事件，因为节点自己的生命周期和 Repo 不同。单例 Repo 才能保证非空
    delegate void MovedEvent(Vector3 pos, float delta);

    event MovedEvent? Moved;

    delegate void TransformedEvent(Transform3D transform, float delta);

    event TransformedEvent? Transformed;
}