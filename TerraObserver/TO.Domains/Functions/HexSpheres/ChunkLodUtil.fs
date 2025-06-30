namespace TO.Domains.Functions.HexSpheres

open Godot
open TO.Domains.Types.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-22 15:52:22
module ChunkLodUtil =
    let calcLod (tileLen: float32) =
        function
        | distance when distance > tileLen * 160f -> ChunkLodEnum.JustHex
        | distance when distance > tileLen * 80f -> ChunkLodEnum.PlaneHex
        | distance when distance > tileLen * 40f -> ChunkLodEnum.SimpleHex
        | distance when distance > tileLen * 20f -> ChunkLodEnum.TerracesHex
        | _ -> ChunkLodEnum.Full

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    let isChunkInsight (chunkPos: Vector3) (camera: Camera3D) (radius: float32) =
        let cosine = radius / camera.GlobalPosition.Length()

        Mathf.Cos(chunkPos.Normalized().AngleTo(camera.GlobalPosition.Normalized())) > cosine
        && camera.IsPositionInFrustum chunkPos
