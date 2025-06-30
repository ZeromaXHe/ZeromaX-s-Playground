namespace TO.Domains.Types.HexSpheres.Components.Tiles

open Friflo.Engine.ECS
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:15:06
[<Struct>]
type TileHexFaceIds =
    interface IComponent
    val Length: FaceId
    val FaceId0: FaceId
    val FaceId1: FaceId
    val FaceId2: FaceId
    val FaceId3: FaceId
    val FaceId4: FaceId
    val FaceId5: FaceId

    new(faceIds: FaceId array) =
        if faceIds.Length <> 5 && faceIds.Length <> 6 then
            failwith "TileHexFaceIds must init with length 5 or 6"

        { Length = faceIds.Length
          FaceId0 = faceIds[0]
          FaceId1 = faceIds[1]
          FaceId2 = faceIds[2]
          FaceId3 = faceIds[3]
          FaceId4 = faceIds[4]
          FaceId5 = if faceIds.Length > 5 then faceIds[5] else faceIds[0] }
