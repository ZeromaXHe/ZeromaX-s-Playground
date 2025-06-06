namespace TO.FSharp.Repos.Models.HexSpheres.Tiles

open System.Collections
open System.Collections.Generic
open Friflo.Engine.ECS
open TO.FSharp.Commons.Utils

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 11:15:06
[<Struct>]
type TileHexFaceIds =
    interface IComponent
    val Length: int
    val FaceId0: int
    val FaceId1: int
    val FaceId2: int
    val FaceId3: int
    val FaceId4: int
    val FaceId5: int

    new(faceIds: int array) =
        if faceIds.Length <> 5 && faceIds.Length <> 6 then
            failwith "TileHexFaceIds must init with length 5 or 6"

        { Length = faceIds.Length
          FaceId0 = faceIds[0]
          FaceId1 = faceIds[1]
          FaceId2 = faceIds[2]
          FaceId3 = faceIds[3]
          FaceId4 = faceIds[4]
          FaceId5 = if faceIds.Length > 5 then faceIds[5] else faceIds[0] }

    interface int IWithLength with
        override this.Length = this.Length
        override this.GetEnumerator() : int IEnumerator = new WithLengthEnumerator<int>(this)
        override this.GetEnumerator() : IEnumerator = new WithLengthEnumerator<int>(this)
        // 只读的索引属性
        override this.Item idx =
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
