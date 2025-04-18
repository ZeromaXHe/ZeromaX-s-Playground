using Apps.Queries.Applications.Base;
using Godot;

namespace Apps.Queries.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 20:44:16
public interface IMiniMapManagerApp: INodeApp
{
    void Init(Vector3 orbitCamPos);
}