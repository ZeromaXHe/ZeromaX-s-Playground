using TO.Infras.Readers.Abstractions.Bases;
using TO.Nodes.Abstractions;

namespace TO.Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-28 21:55:28
public interface IPlanetRepo : ISingletonNodeRepo<IPlanet>
{
    event Action<double>? Processed;
    event Action<float, float>? RadiusChanged;
}