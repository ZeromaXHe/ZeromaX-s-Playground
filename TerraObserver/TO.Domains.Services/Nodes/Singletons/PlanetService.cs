using Godot;
using TO.Domains.Services.Abstractions.Nodes.Singletons;
using TO.Infras.Readers.Abstractions.Nodes.Singletons;
using TO.Nodes.Abstractions;

namespace TO.Domains.Services.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 22:07:56
public class PlanetService(IPlanetRepo planetRepo) : IPlanetService
{
    private IPlanet Self => planetRepo.Singleton!;

    public void OnProcessed(double delta)
    {
        // GD.Print($"delta: {delta}, radius: {Self.Radius}");
    }

    public void OnRadiusChanged(float pre, float now)
    {
        GD.Print($"radius changed: {pre} -> {now}");
    }
}