using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.Singletons;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-18 19:21:29
public class LongitudeLatitudeRepo : SingletonNodeRepo<ILongitudeLatitude>, ILongitudeLatitudeRepo
{
    public event Action<bool>? FixFullVisibilityChanged;

    private void OnFixFullVisibilityChanged(bool value) => FixFullVisibilityChanged?.Invoke(value);

    protected override void ConnectNodeEvents()
    {
        Singleton!.FixFullVisibilityChanged += OnFixFullVisibilityChanged;
    }

    protected override void DisconnectNodeEvents()
    {
        Singleton!.FixFullVisibilityChanged -= OnFixFullVisibilityChanged;
    }

    // 锁定经纬网的显示
    public void FixLatLon(bool toggle) => Singleton!.FixFullVisibility = toggle;
}