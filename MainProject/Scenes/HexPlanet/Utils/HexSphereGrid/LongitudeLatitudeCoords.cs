using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-10 20:35
public readonly struct LongitudeLatitudeCoords(float longitude, float latitude)
{
    // 角度制的经度，西经为正
    public readonly float Longitude = Mathf.Wrap(longitude, -180f, 180f);

    // 角度制的纬度，北纬为正
    // 【注意】:纬度不具有循环性，所以上限需要保证 90f 不会变成 -90f
    public readonly float Latitude = Mathf.Wrap(latitude, -90f, 90.001f);

    public static LongitudeLatitudeCoords From(Vector2 v) => new(v.X, v.Y);
    public Vector2 ToVector2() => new(Longitude, Mathf.Clamp(Latitude, -90f, 90f));

    // UV 转换
    public static LongitudeLatitudeCoords FromUv(float u, float v) => new(180f - 360f * u, 90f - 180f * v);
    public Vector2 ToUv() => new(-Longitude / 360f + 0.5f, 0.5f - Latitude / 180f);

    // 映射关系：
    // 经纬度以 X 轴方向为本初子午线方向，顺时针的西经方向作为经度正方向
    // Y 轴方向为北极点方向，赤道向北纬是纬度正方向
    public static LongitudeLatitudeCoords From(Vector3 v)
    {
        if (v == Vector3.Zero) throw new ArgumentException("v 不能是零向量");
        if (v is { X: 0, Z: 0 })
            return new LongitudeLatitudeCoords(0, v.Y > 0 ? 90f : -90f);
        var xzVec = new Vector2(v.X, v.Z);
        var longitude = Mathf.RadToDeg(xzVec.Angle());
        var latitude = Mathf.RadToDeg(Mathf.Atan2(v.Y, xzVec.Length()));
        return new LongitudeLatitudeCoords(longitude, latitude);
    }

    public Vector3 ToDirectionVector3()
    {
        switch (Latitude)
        {
            case >= 90:
                return Vector3.Up;
            case <= -90:
                return Vector3.Down;
        }

        var xzVec = Vector2.Right.Rotated(Mathf.DegToRad(Longitude));
        var y = Mathf.Tan(Mathf.DegToRad(Latitude));
        return new Vector3(xzVec.X, y, xzVec.Y).Normalized();
    }

    public LongitudeLatitudeCoords Lerp(LongitudeLatitudeCoords to, float weight) =>
        From(ToDirectionVector3().Lerp(to.ToDirectionVector3(), weight));

    public LongitudeLatitudeCoords Slerp(LongitudeLatitudeCoords to, float weight) =>
        From(ToDirectionVector3().Slerp(to.ToDirectionVector3(), weight));

    public override string ToString() => $"{GetLongitudeString()}, {GetLatitudeString()}";


    public string GetLongitudeString()
    {
        var longitudeType = Longitude > 0 ? "W" : Longitude == 0 ? " " : "E";
        var longitudeAbs = Mathf.Abs(Longitude);
        var longitudeDegreeInt = (int)longitudeAbs;
        var longitudeMinuteInt = (int)(longitudeAbs % 1 * 60);
        var longitudeSecondInt = Mathf.RoundToInt(longitudeAbs % 1 * 60 % 1 * 60);
        if (longitudeSecondInt == 60)
        {
            longitudeSecondInt = 0;
            longitudeMinuteInt++;
            if (longitudeMinuteInt == 60)
            {
                longitudeMinuteInt = 0;
                longitudeDegreeInt++;
            }
        }

        return $"{longitudeType}{longitudeDegreeInt,3}°{longitudeMinuteInt:D2}'{longitudeSecondInt:D2}\"";
    }

    public string GetLatitudeString()
    {
        var latitudeType = Latitude > 0 ? "N" : Latitude == 0 ? " " : "S";
        var latitudeAbs = Mathf.Abs(Latitude);
        var latitudeDegreeInt = (int)latitudeAbs;
        var latitudeMinuteInt = (int)(latitudeAbs % 1 * 60);
        var latitudeSecondInt = Mathf.RoundToInt(latitudeAbs % 1 * 60 % 1 * 60);
        if (latitudeSecondInt == 60)
        {
            latitudeSecondInt = 0;
            latitudeMinuteInt++;
            if (latitudeMinuteInt == 60)
            {
                latitudeMinuteInt = 0;
                latitudeDegreeInt++;
            }
        }

        return $"{latitudeType}{latitudeDegreeInt,2}°{latitudeMinuteInt:D2}'{latitudeSecondInt:D2}\"";
    }
}