namespace Commons.Frameworks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-11 17:40:11
public interface IContext
{
    // 仿 setter 注入写法：
    // private readonly Lazy<ITileRepo> _tileRepo = new(() => Context.GetSingleton<ITileRepo>());
    T? GetBean<T>() where T : class;
}