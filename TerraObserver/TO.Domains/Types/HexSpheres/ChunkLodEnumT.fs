namespace TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-08 15:20:08
type ChunkLodEnum =
    | JustHex = 0 // 每个地块只有六个平均高度点组成的六边形（非平面）
    | PlaneHex = 1 // 高度立面，无特征，无河流的六边形
    | SimpleHex = 2 // 最简单的 Solid + 斜面六边形
    | TerracesHex = 3 // 增加台阶
    | Full = 4 // 增加边细分
