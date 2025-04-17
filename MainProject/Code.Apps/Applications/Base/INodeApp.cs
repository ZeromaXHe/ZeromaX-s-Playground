namespace Apps.Applications.Base;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-17 09:45:24
public interface INodeApp
{
    // 需要在 OnReady 置为 true，OnExitTree 置为 false
    bool NodeReady { get; set; }
    void OnReady();
    void OnProcess(double delta);
    void OnExitTree();
}