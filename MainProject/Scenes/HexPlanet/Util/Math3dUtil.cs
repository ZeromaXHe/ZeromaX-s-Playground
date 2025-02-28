using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class Math3dUtil
{
    public static Vector3 GetNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        var side1 = v1 - v0;
        var side2 = v2 - v0;
        return -side1.Cross(side2).Normalized();
    }

    public static bool IsNormalAwayFromOrigin(Vector3 surface, Vector3 normal, Vector3 origin) =>
        (surface - origin).Dot(normal) > 0;

    public static Vector3 ProjectToSphere(Vector3 p, float radius, float scale = 1f)
    {
        var projectionPoint = radius / p.Length();
        return p * projectionPoint * scale;
    }

    // 判断是否 v0, v1, v2 的顺序是合适的缠绕方向（正面顺时针）
    public static bool IsRightVSeq(Vector3 origin, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        var center = (v0 + v1 + v2) / 3f;
        // 决定缠绕顺序
        var normal = GetNormal(v0, v1, v2);
        return IsNormalAwayFromOrigin(center, normal, origin);
    }

    /// <summary>
    /// 计算两个向量在垂直于 dir 的平面上的夹角（弧度）
    /// </summary>
    /// <param name="a">向量 a</param>
    /// <param name="b">向量 b</param>
    /// <param name="dir">方向</param>
    /// <param name="signed">返回是否带符号，默认不带。带的话则对应 dir 角度下顺时针方向为正</param>
    /// <returns>两个投影向量间的夹角（弧度制）</returns>
    /// <exception cref="ArgumentException"></exception>
    public static float GetPlanarAngle(Vector3 a, Vector3 b, Vector3 dir, bool signed = false) 
    {
        // 异常处理：入参向量均不能为零
        if (a == Vector3.Zero || b == Vector3.Zero || dir == Vector3.Zero)
            throw new ArgumentException("Input vectors cannot be zero");
        // 1. 获取垂直于 dir 的投影平面法线
        var planeNormal = dir.Normalized();
        // 2. 投影向量到平面
        var aProj = a - planeNormal * a.Dot(planeNormal);
        var bProj = b - planeNormal * b.Dot(planeNormal);
        // 3. 处理零向量特殊情况
        if (aProj == Vector3.Zero || bProj == Vector3.Zero)
            return 0f; // 或根据需求抛出异常
        // 4. 计算投影向量的夹角（弧度制）
        var angle = Mathf.Acos(aProj.Normalized().Dot(bProj.Normalized()));
        if (!signed) return angle;
        // signed 需要返回范围 [-Pi, Pi] 的带方向角度
        var cross = aProj.Cross(bProj);
        float sign = Mathf.Sign(cross.Dot(dir.Normalized()));
        return sign * angle;
    }
}