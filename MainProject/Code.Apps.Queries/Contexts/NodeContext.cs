using Godot;

namespace Apps.Queries.Contexts;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 14:28:26
public class NodeContext
{
    public static NodeContext Instance { get; } = new();

    private readonly Dictionary<string, object> _singletons = new();

    // 目前字典本身 object 不校验对应 string 类型名字是否正确，依赖于添加时自己保证正确
    // nameof(T) 结果是 "T"，想要获取原类名字符串，需要使用 typeof(T).Name;
    public void RegisterSingleton<T>(T bean) where T : class
    {
        if (Engine.IsEditorHint())
        {
            // Godot C# 编辑器工具编译后第一次运行会调两次构造函数、_EnterTree()、_Ready()
            // 2020 年 7 月至今一直没修，文档也不写！好蠢！
            // https://github.com/godotengine/godot-docs/issues/2930#issuecomment-662407208
            // https://github.com/godotengine/godot/issues/40970
            if (_singletons.TryAdd(typeof(T).Name, bean)) return;
            if (_singletons[typeof(T).Name] == bean) return;
            GD.Print($"{typeof(T).Name} 单例之前已存在不同的实例，正在覆盖！");
            _singletons[typeof(T).Name] = bean; // 事实证明第二次才是真正的实例
        }
        else
            _singletons.Add(typeof(T).Name, bean);
    }

    public bool DestroySingleton<T>() where T : class => _singletons.Remove(typeof(T).Name);

    private readonly Dictionary<string, Dictionary<int, object>> _idInstances = new();

    public void RegisterIdInstance<T>(int id, T bean) where T : class
    {
        var name = typeof(T).Name;
        if (_idInstances.TryGetValue(name, out var classDict))
            classDict.Add(id, bean);
        else
            _idInstances.Add(name, new Dictionary<int, object> { [id] = bean });
    }

    public bool DestroyIdInstance<T>(int id) where T : class
    {
        var name = typeof(T).Name;
        if (!_idInstances.TryGetValue(name, out var classDict))
            return false;
        var result = classDict.Remove(id);
        if (classDict.Count == 0)
            _idInstances.Remove(name);
        return result;
    }

    public void Reboot()
    {
        _singletons.Clear();
        _idInstances.Clear();
    }

    public T? GetSingleton<T>() where T : class
    {
        return _singletons.GetValueOrDefault(typeof(T).Name) as T;
    }

    public T? GetIdInstance<T>(int id) where T : class
    {
        if (!_idInstances.TryGetValue(typeof(T).Name, out var classDict))
            return null;
        return classDict.GetValueOrDefault(id) as T;
    }

    // 让场景根节点 _Ready 调用，进行各个单例之间依赖注入、信号绑定等
    public void Init()
    {
        // ContextHolder.BeanContext!.GetBean<IHexPlanetHudApplication>()!.InjectNodesAndConnectEvents();
    }
}