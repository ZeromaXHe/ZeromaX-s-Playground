using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Bases;
using Nodes.Abstractions;

namespace Infras.Readers.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-27 15:48:03
public class HexUnitRepo : IdInstanceNodeRepo<IHexUnit>, IHexUnitRepo
{
    public event IHexUnitRepo.TileIdChangedEvent? TileIdChanged;

    private IHexUnit.TileIdChangedEvent OnTileIdChanged(IHexUnit unit) =>
        (pre, now) => TileIdChanged?.Invoke(unit, pre, now);

    public event Action<IHexUnit>? Died;
    private Action OnDied(IHexUnit unit) => () => Died?.Invoke(unit);

    private record EventListeners(IHexUnit.TileIdChangedEvent OnTileIdChanged, Action OnDied);

    private readonly Dictionary<IHexUnit, EventListeners> _unregisters = new();

    protected override void RegisterHook(IHexUnit instance)
    {
        var eventListeners = new EventListeners(OnTileIdChanged(instance), OnDied(instance));
        instance.TileIdChanged += eventListeners.OnTileIdChanged;
        instance.Died += eventListeners.OnDied;
        _unregisters.Add(instance, eventListeners);
    }

    protected override void UnregisterHook(IHexUnit instance)
    {
        var eventListeners = _unregisters[instance];
        instance.TileIdChanged -= eventListeners.OnTileIdChanged;
        instance.Died -= eventListeners.OnDied;
        _unregisters.Remove(instance);
    }
}