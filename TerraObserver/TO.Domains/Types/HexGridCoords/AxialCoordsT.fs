namespace TO.Domains.Types.HexGridCoords

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-16 19:59:16
[<Struct>]
type AxialCoords =
    val Q: int
    val R: int
    new(q, r) = { Q = q; R = r }
    override this.ToString() = $"({this.Q},{this.R})"
