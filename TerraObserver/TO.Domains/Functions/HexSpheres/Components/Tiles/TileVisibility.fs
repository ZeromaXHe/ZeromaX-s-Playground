namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:50:29
module TileVisibility =
    let isVisible (tileFlag: TileFlag) (this: TileVisibility) =
        this.Visibility > 0 && TileFlag.isExplorable tileFlag
