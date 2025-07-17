namespace TO.Domains.Functions.HexMetrics

open TO.Domains.Types.HexMetrics

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:12:29
module EdgeVertices =
    let toString (this: EdgeVertices) =
        $"EdgeVertices:[{this.V1}, {this.V2}, {this.V3}, {this.V4}, {this.V5}]"
