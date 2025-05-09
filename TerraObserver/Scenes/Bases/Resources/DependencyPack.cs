using Godot;

namespace TerraObserver.Scenes.Bases.Resources;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 15:41
[GlobalClass]
[Tool]
public partial class DependencyPack : Resource
{
    [Export]
    public Godot.Collections.Array<Dependency?>? Dependencies
    {
        get => _dependencies;
        set
        {
            // 增加和删除、修改 Array 元素都会触发 setter！
            if (_dependencies != null)
                foreach (var dependency in _dependencies)
                    if (dependency != null)
                        dependency.Changed -= EmitChanged;
            _dependencies = value;
            if (_dependencies != null)
                foreach (var dependency in _dependencies)
                    if (dependency != null)
                        dependency.Changed += EmitChanged;
            EmitChanged();
        }
    }

    // 不能赋值 = [], 否则报错：ERROR: Script class can only be set together with base class name
    // https://github.com/godotengine/godot/issues/103343
    private Godot.Collections.Array<Dependency?>? _dependencies;

    public string Validate()
    {
        var result = "";
        var idx = 0;
        if (Dependencies == null)
            return result;
        foreach (var dependency in Dependencies)
        {
            if (dependency == null)
            {
                result += $"\n| 依赖{idx}: 不应该为空; ";
            }
            else
            {
                var warning = dependency.Validate();
                if (!"".Equals(warning))
                    result += $"\n| 依赖{idx}: {warning}";
            }

            idx++;
        }

        return result;
    }
}