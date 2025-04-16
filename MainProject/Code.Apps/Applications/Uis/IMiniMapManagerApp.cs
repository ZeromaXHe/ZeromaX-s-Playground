using Godot;

namespace Apps.Applications.Uis;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 20:44:16
public interface IMiniMapManagerApp
{
    #region 上下文节点

    void OnReady();
    void OnExitTree();

    #endregion

    void Init(Vector3 orbitCamPos);
}