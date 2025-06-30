namespace TO.Domains.Functions.HexGridCoords

open Godot
open TO.Domains.Types.HexGridCoords

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-29 17:05:29
module LonLatCoords =
    let fromVector2 (v: Vector2) = LonLatCoords(v.X, v.Y)

    let toVector2 (this: LonLatCoords) =
        LonLatCoords(this.Longitude, Mathf.Clamp(this.Latitude, -90f, 90f))
    // UV 转换
    let fromUv (u: float32) (v: float32) =
        LonLatCoords(180f - u * 360f, 90f - v * 180f)

    let toUv (this: LonLatCoords) =
        LonLatCoords(-this.Longitude / 360f + 0.5f, 0.5f - this.Latitude / 180f)
    // 映射关系：
    // 经纬度以 X 轴方向为本初子午线方向，顺时针的西经方向作为经度正方向
    // Y 轴方向为北极点方向，赤道向北纬是纬度正方向
    let fromVector3 (v: Vector3) =
        if (v = Vector3.Zero) then
            failwith "v 不能是零向量" // TODO: 去掉异常

        if v.X = 0f && v.Z = 0f then
            LonLatCoords(0f, if v.Y > 0f then 90f else -90f)
        else
            let xzVec = Vector2(v.X, v.Z)
            let longitude = Mathf.RadToDeg(xzVec.Angle())
            let latitude = Mathf.RadToDeg(Mathf.Atan2(v.Y, xzVec.Length()))
            LonLatCoords(longitude, latitude)

    let toDirectionVector3 (this: LonLatCoords) =
        if this.Latitude >= 90f then
            Vector3.Up
        elif this.Latitude <= -90f then
            Vector3.Down
        else
            let xzVec = Vector2.Right.Rotated <| Mathf.DegToRad(this.Longitude)
            let y = Mathf.Tan(Mathf.DegToRad this.Latitude)
            Vector3(xzVec.X, y, xzVec.Y).Normalized()

    let lerp (toLonLat: LonLatCoords) (weight: float32) (this: LonLatCoords) =
        fromVector3
        <| (toDirectionVector3 this).Lerp(toDirectionVector3 toLonLat, weight)

    let slerp (toLonLat: LonLatCoords) (weight: float32) (this: LonLatCoords) =
        fromVector3
        <| (toDirectionVector3 this).Slerp(toDirectionVector3 toLonLat, weight)

    let getLonLatString (isLon: bool) (this: LonLatCoords) =
        let lonLatType =
            if isLon then
                if this.Longitude > 0f then "W"
                elif this.Longitude = 0f then " "
                else "E"
            else if this.Latitude > 0f then
                "N"
            elif this.Latitude = 0f then
                " "
            else
                "S"

        let abs = Mathf.Abs(if isLon then this.Longitude else this.Latitude)
        let mutable degreeInt = int abs
        let mutable minuteInt = int <| abs % 1f * 60f
        let mutable secondInt = Mathf.RoundToInt(abs % 1f * 60f % 1f * 60f)

        if secondInt = 60 then
            secondInt <- 0
            minuteInt <- minuteInt + 1

            if minuteInt = 60 then
                minuteInt <- 0
                degreeInt <- degreeInt + 1

        if isLon then
            $"{lonLatType}{degreeInt, 3}°{minuteInt:D2}'{secondInt:D2}\""
        else
            $"{lonLatType}{degreeInt, 2}°{minuteInt:D2}'{secondInt:D2}\""

    let toString (this: LonLatCoords) =
        $"{getLonLatString true this}, {getLonLatString false this}"
