using Godot;
using TO.Commons.Enums;

namespace TerraObserver.Scenes.Bases.Resources;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 14:35
[GlobalClass]
[Tool]
public partial class Dependency : Resource
{
    // 类型
    [Export]
    public DependencyEnum Type
    {
        get => _type;
        set
        {
            _type = value;
            EmitChanged();
        }
    }

    private DependencyEnum _type = DependencyEnum.Unknown;

    // 对应节点场景
    [Export]
    public PackedScene? Scene
    {
        get => _scene;
        set
        {
            _scene = value;
            EmitChanged();
        }
    }

    private PackedScene? _scene;

    // 依赖
    [Export]
    public Godot.Collections.Array<DependencyEnum>? Dependencies
    {
        get => _dependencies;
        set
        {
            _dependencies = value;
            EmitChanged();
        }
    }

    // 不能赋值 = [], 否则报错：ERROR: Script class can only be set together with base class name
    // https://github.com/godotengine/godot/issues/103343
    private Godot.Collections.Array<DependencyEnum>? _dependencies;

    // 声明 virtual 让子类覆盖掉
    public virtual string Validate()
    {
        var result = "";
        if (!Type.IsValid())
            result += "类型无效; ";
        if (Scene == null)
            result += "场景为空; ";
        if (Dependencies != null)
        {
            var idx = 0;
            var depResult = "";
            foreach (var dependency in Dependencies)
            {
                if (!dependency.IsValid())
                {
                    if (depResult.Length > 0)
                        depResult += ", ";
                    depResult += idx;
                }

                idx++;
            }

            if (depResult.Length > 0)
                result += $"子依赖{depResult}无效; ";
        }

        return result;
    }
}