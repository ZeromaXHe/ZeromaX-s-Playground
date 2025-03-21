using System;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-15 20:11
public static class Math3dUtil
{
    public static Vector3[] Subdivide(Vector3 from, Vector3 target, int count)
    {
        var segments = new Vector3[count + 1];
        segments[0] = from;
        for (var i = 1; i < count; i++)
            // 注意这里用 Slerp 而不是 Lerp，让所有的点都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
            segments[i] = from.Slerp(target, (float)i / count);
        segments[count] = target;
        return segments;
    }

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
        if (float.IsNaN(angle)) return 0f;
        if (!signed) return angle;
        // signed 需要返回范围 [-Pi, Pi] 的带方向角度
        var cross = aProj.Cross(bProj);
        float sign = Mathf.Sign(cross.Dot(dir.Normalized()));
        return sign * angle;
    }

    public static Transform3D PlaceOnSphere(Basis basis, Vector3 position, Vector3 scale,
        float addHeight = 0, Vector3 alignForward = default)
    {
        var transform = AlignYAxisToDirection(basis, position, alignForward);
        transform = transform.Scaled(scale);
        transform.Origin = position.Normalized() * (position.Length() + addHeight * scale.Y);
        return transform;
    }

    /// <summary>
    /// 对齐基变换的正方向到指定的方向向量。
    /// </summary>
    /// <param name="basis">基</param>
    /// <param name="direction">目标方向向量</param>
    /// <param name="alignForward">希望对齐向前的方向（不传则默认不调整）</param>
    /// <param name="global">Y 轴是否使用全局基（注意：全局基模式未经测试）</param>
    public static Transform3D AlignYAxisToDirection(Basis basis, Vector3 direction, Vector3 alignForward = default,
        bool global = false)
    {
        var transform = Transform3D.Identity;
        transform.Basis = basis;
        // 确保方向是单位向量
        direction = direction.Normalized();
        // 当前 Y 轴
        var yAxis = global ? Vector3.Up : basis.Y;
        // 计算旋转轴
        var rotationAxis = yAxis.Cross(direction);
        // 如果旋转轴长度为 0，说明方向相同或相反
        if (rotationAxis.Length() == 0)
        {
            if (yAxis.Dot(direction) > 0) return transform; // 方向相同
            // 方向相反，绕 X 轴转 180 度
            if (global)
                transform.Rotated(Vector3.Right, Mathf.Pi);
            else
                transform.RotatedLocal(Vector3.Right, Mathf.Pi);
            return transform;
        }

        // 计算旋转角度
        var angle = yAxis.AngleTo(direction);
        if (global)
            transform = transform.Rotated(rotationAxis.Normalized(), angle);
        else
            transform = transform.RotatedLocal(rotationAxis.Normalized(), angle);
        alignForward = alignForward.Normalized();
        if (alignForward != default && alignForward != direction)
        {
            // 如果有指定向前对齐方向，则对齐向前（-Z）到最近的方向
            var forward = global ? Vector3.Forward : -transform.Basis.Z;
            var zAngle = GetPlanarAngle(forward, alignForward, direction, true);
            if (global)
                transform = transform.Rotated(direction, zAngle);
            else
                transform = transform.RotatedLocal(direction, zAngle);
        }

        return transform;
    }

    /// <summary>
    /// 计算从点 A 在球面上对齐到点 B 的最短路径方向的朝向向量
    /// </summary>
    /// <param name="pointA"></param>
    /// <param name="pointB"></param>
    /// <returns></returns>
    public static Vector3 DirectionBetweenPointsOnSphere(Vector3 pointA, Vector3 pointB)
    {
        var sphereCenter = Vector3.Zero;
        var vectorToA = pointA - sphereCenter;
        var vectorToB = pointB - sphereCenter;
        var greatCircleNormal = vectorToA.Cross(vectorToB).Normalized();
        return greatCircleNormal.Cross(vectorToA).Normalized();
    }
}