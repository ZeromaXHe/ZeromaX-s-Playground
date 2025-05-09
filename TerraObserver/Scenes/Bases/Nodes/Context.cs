using System.Collections.Generic;
using Godot;
using TerraObserver.Scenes.Bases.Resources;

namespace TerraObserver.Scenes.Bases.Nodes;

[GlobalClass]
[Tool]
public partial class Context : Node
{
    // 打包好的依赖组
    [Export]
    public Godot.Collections.Array<DependencyPack?>? PackedDependencies
    {
        get => _packedDependencies;
        set
        {
            // 增加和删除、修改 Array 元素都会触发 setter！
            if (_packedDependencies != null)
                foreach (var pack in _packedDependencies)
                    if (pack != null)
                        pack.Changed -= UpdateConfigurationWarnings;
            _packedDependencies = value;
            if (_packedDependencies != null)
                foreach (var pack in _packedDependencies)
                    if (pack != null)
                        pack.Changed += UpdateConfigurationWarnings;
            UpdateConfigurationWarnings();
        }
    }

    // 不能赋值 = [], 否则报错：ERROR: Script class can only be set together with base class name
    // https://github.com/godotengine/godot/issues/103343
    private Godot.Collections.Array<DependencyPack?>? _packedDependencies;

    // 零散依赖
    [Export]
    public Godot.Collections.Array<Dependency?>? Dependencies
    {
        get => _dependencies;
        set
        {
            // 增加和删除、修改 Array 元素都会触发 setter！
            // 并不需要自定义 _get_property_list()、_get()、_set()， Nice！
            // GD.Print("修改依赖");
            if (_dependencies != null)
                foreach (var dependency in _dependencies)
                    if (dependency != null)
                        dependency.Changed -= UpdateConfigurationWarnings;
            _dependencies = value;
            if (_dependencies != null)
                foreach (var dependency in _dependencies)
                    if (dependency != null)
                        dependency.Changed += UpdateConfigurationWarnings;
            UpdateConfigurationWarnings();
        }
    }

    // 不能赋值 = [], 否则报错：ERROR: Script class can only be set together with base class name
    // https://github.com/godotengine/godot/issues/103343
    private Godot.Collections.Array<Dependency?>? _dependencies;

    // 校验依赖配置的正确性
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = [];

        var idx = 0;
        if (PackedDependencies != null)
            foreach (var pack in PackedDependencies)
            {
                if (pack == null)
                    warnings.Add($"依赖包{idx}: 不应该为空");
                else
                {
                    var warning = pack.Validate();
                    if (!"".Equals(warning))
                        warnings.Add($"依赖包{idx}: {warning}");
                }

                idx++;
            }

        idx = 0;
        if (Dependencies != null)
            foreach (var dependency in Dependencies)
            {
                if (dependency == null)
                    warnings.Add($"依赖{idx}: 不应该为空");
                else
                {
                    var warning = dependency.Validate();
                    if (!"".Equals(warning))
                        warnings.Add($"依赖{idx}: {warning}");
                }

                idx++;
            }

        return warnings.ToArray();
    }
}