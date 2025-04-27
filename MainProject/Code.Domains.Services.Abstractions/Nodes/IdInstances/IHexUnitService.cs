using Nodes.Abstractions;

namespace Domains.Services.Abstractions.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-27 16:02:49
public interface IHexUnitService
{
    void Travel(IHexUnit unit, IHexUnitPath path);
    void ValidateLocation(IHexUnit unit);
}