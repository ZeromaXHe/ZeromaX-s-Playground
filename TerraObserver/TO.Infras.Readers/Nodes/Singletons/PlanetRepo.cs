using Godot;
using TO.Infras.Readers.Abstractions.Nodes.Singletons;
using TO.Infras.Readers.Bases;
using TO.Nodes.Abstractions;

namespace TO.Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 22:00:55
public class PlanetRepo : SingletonNodeRepo<IPlanet>, IPlanetRepo, IDisposable
{
    public event Action<double>? Processed;
    private void OnProcessed(double delta) => Processed?.Invoke(delta);
    public event Action<float, float>? RadiusChanged;
    private void OnRadiusChanged(float pre, float now) => RadiusChanged?.Invoke(pre, now);

    protected override void ConnectNodeEvents()
    {
        Singleton!.Processed += OnProcessed;
        Singleton.RadiusChanged += OnRadiusChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.Processed -= OnProcessed;
        Singleton.RadiusChanged -= OnRadiusChanged;
    }

    public void Dispose() => Unregister();
}