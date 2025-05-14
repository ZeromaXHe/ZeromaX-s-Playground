namespace TerraObserver.Scenes.Bases.Enums;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-05-09 14:08:09
public enum DependencyEnum
{
    // 未知（默认情况，往往说明未设置）
    Unknown = 0,

    // 命令层
    Commands = 100000,

    // 服务层
    Services = 200000,

    // 显示层
    Views = 300000,

    // 存储层
    Repos = 400000,
}

public static class DependencyEnumExt
{
    public static bool IsValid(this DependencyEnum self) =>
        // self != DependencyEnum.Unknown && self != DependencyEnum.Commands
        // && self != DependencyEnum.Services && self != DependencyEnum.Views
        // && self != DependencyEnum.Repos;
        (int)self % (int)DependencyEnum.Commands != 0;
}