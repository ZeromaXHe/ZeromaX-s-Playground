namespace Domains.Models.ValueObjects.PlanetGenerates;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-11 19:09:11
public enum ChunkLod
{
    JustHex, // 每个地块只有六个平均高度点组成的六边形（非平面）
    PlaneHex, // 高度立面，无特征，无河流的六边形
    SimpleHex, // 最简单的 Solid + 斜面六边形 
    TerracesHex, // 增加台阶
    Full, // 增加边细分
}