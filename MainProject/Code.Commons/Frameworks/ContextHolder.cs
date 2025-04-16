namespace Commons.Frameworks;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-11 17:42:35
public static class ContextHolder
{
    // TODO: 一时权宜之计，放在最底层为了兼容 HexTileData、HexTileDataOverrider 里面的逻辑，后续重构掉
    public static IContext? BeanContext { get; set; }
}