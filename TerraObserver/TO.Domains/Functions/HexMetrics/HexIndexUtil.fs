namespace TO.Domains.Functions.HexMetrics

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 10:46:06
module HexIndexUtil =
    let previousIdx (idx: int) (length: int) = if idx = 0 then length - 1 else idx - 1

    let previous2Idx (idx: int) (length: int) =
        if idx <= 1 then length - 2 + idx else idx - 2

    let nextIdx (idx: int) (length: int) = (idx + 1) % length
    let next2Idx (idx: int) (length: int) = (idx + 2) % length
    let oppositeIdx (idx: int) (length: int) = (idx + 3) % length
