using Infras.Readers.Abstractions.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Abstractions.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:20:18
public interface ILongitudeLatitudeRepo : ISingletonNodeRepo<ILongitudeLatitude>
{
    event Action<bool>? FixFullVisibilityChanged;

    // 锁定经纬网的显示
    void FixLatLon(bool toggle);
}