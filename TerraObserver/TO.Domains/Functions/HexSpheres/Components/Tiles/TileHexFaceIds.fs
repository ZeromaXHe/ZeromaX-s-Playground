namespace TO.Domains.Functions.HexSpheres.Components.Tiles

open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:44:29
module TileHexFaceIds =
    let item idx (this: TileHexFaceIds) =
        if idx < 0 || idx >= this.Length then
            failwith "TileHexFaceIds invalid index"

        match idx with
        | 0 -> this.FaceId0
        | 1 -> this.FaceId1
        | 2 -> this.FaceId2
        | 3 -> this.FaceId3
        | 4 -> this.FaceId4
        | 5 -> this.FaceId5
        | _ -> failwith "TileHexFaceIds invalid index"
