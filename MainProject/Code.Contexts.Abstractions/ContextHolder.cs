namespace Contexts.Abstractions;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-11 17:42:35
public static class ContextHolder
{
    // 用来给 Mod 等获取想要的依赖？
    public static IContext? BeanContext { get; set; }
}