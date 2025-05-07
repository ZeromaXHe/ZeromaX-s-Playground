namespace TO.Domains.Services.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 22:07:28
public interface IPlanetService
{
    void OnProcessed(double delta);
    void OnRadiusChanged(float pre, float now);
}