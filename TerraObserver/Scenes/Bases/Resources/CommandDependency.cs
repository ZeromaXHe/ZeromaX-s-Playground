using Godot;
using TO.Commons.Enums;

namespace TerraObserver.Scenes.Bases.Resources;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 16:14:14
[GlobalClass]
[Tool]
public partial class CommandDependency : Dependency
{
    public override string Validate()
    {
        var result = base.Validate();
        if (Dependencies != null)
        {
            var idx = 0;
            var depResult = "";
            foreach (var dependency in Dependencies)
            {
                // 不允许命令相互之间依赖
                if (dependency.IsValid() && dependency < DependencyEnum.Services)
                {
                    if (depResult.Length > 0)
                        depResult += ", ";
                    depResult += idx;
                }

                idx++;
            }

            if (depResult.Length > 0)
                result += $"子依赖{depResult}层级并非更低，不推荐; ";
        }

        return result;
    }
}