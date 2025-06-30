namespace TO.Domains.Types.HexGridCoords

open Godot

/// 经纬度坐标
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 08:54:17
[<Struct>]
type LonLatCoords =
    // 角度制的经度，西经为正
    val Longitude: float32
    // 角度制的纬度，北纬为正
    // 【注意】:纬度不具有循环性，所以上限需要保证 90f 不会变成 -90f
    val Latitude: float32

    new(longitude: float32, latitude: float32) =
        { Longitude = Mathf.Wrap(longitude, -180f, 180f)
          Latitude = Mathf.Wrap(latitude, -90f, 90.001f) }
