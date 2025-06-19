namespace TO.Domains.Utils.HexSpheres

open TO.Domains.Interfaces.Commons.WithLength

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 10:46:06
module HexIndexUtil =
    let isPentagon (withLength: 'T IWithLength) = withLength.Length = 5
    let previousIdx (length: int) (idx: int) = if idx = 0 then length - 1 else idx - 1

    let previous2Idx (length: int) (idx: int) =
        if idx <= 1 then length - 2 + idx else idx - 2

    let nextIdx (length: int) (idx: int) = (idx + 1) % length
    let next2Idx (length: int) (idx: int) = (idx + 2) % length
    let oppositeIdx (length: int) (idx: int) = (idx + 3) % length
