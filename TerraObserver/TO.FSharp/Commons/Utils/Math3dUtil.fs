namespace TO.FSharp.Commons.Utils

open Godot
open Microsoft.FSharp.Core

/// 数学 3D 工具类
/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
module Math3dUtil =
    let subdivide from target count =
        let segments: Vector3 array = Array.zeroCreate <| count + 1
        segments[0] <- from

        for i in 1..count do
            // 注意这里用 Slerp 而不是 Lerp，让所有的点都在单位球面而不是单位正二十面体上，方便我们后面 VP 树找最近点
            segments[i] <- from.Slerp(target, float32 i / float32 count)

        segments[count] <- target
        segments

    let getNormal (v0: Vector3) v1 v2 =
        let side1 = v1 - v0
        let side2 = v2 - v0
        -side1.Cross(side2).Normalized()

    let isNormalAwayFromOrigin (surface: Vector3) normal origin = (surface - origin).Dot normal > 0f

    /// 判断是否 v0, v1, v2 的顺序是合适的缠绕方向（正面顺时针）
    let isRightVSeq origin v0 v1 v2 =
        let center: Vector3 = (v0 + v1 + v2) / 3f
        // 决定缠绕顺序
        let normal = getNormal v0 v1 v2
        isNormalAwayFromOrigin center normal origin

    /// <summary>
    /// 计算从点 A 在球面上对齐到点 B 的最短路径方向的朝向向量
    /// </summary>
    /// <param name="pointA"></param>
    /// <param name="pointB"></param>
    /// <returns></returns>
    let directionBetweenPointsOnSphere (pointA: Vector3) (pointB: Vector3) =
        let sphereCenter = Vector3.Zero
        let vectorToA = pointA - sphereCenter
        let vectorToB = pointB - sphereCenter
        let greatCircleNormal = vectorToA.Cross(vectorToB).Normalized()
        greatCircleNormal.Cross(vectorToA).Normalized()

/// 作为对其他 .NET 公开的 F# 库（目前是使用了可选参数），需要遵循《F# 组件设计准则 - 命名空间和类型设计 - 使用命名空间、类型和成员作为组件的主要组织结构》
/// https://learn.microsoft.com/zh-cn/dotnet/fsharp/style-guide/component-design-guidelines#use-namespaces-types-and-members-as-the-primary-organizational-structure-for-your-components
[<AbstractClass; Sealed>]
type Math3dUtil =
    static member ProjectToSphere(p: Vector3, radius, ?scale: float32) =
        let scale = defaultArg scale 1f
        let projectionPoint = radius / p.Length()
        p * projectionPoint * scale

    /// <summary>计算两个向量在垂直于 dir 的平面上的夹角（弧度）</summary>
    /// <param name="a">向量 a</param>
    /// <param name="b">向量 b</param>
    /// <param name="dir">方向</param>
    /// <param name="signed">是否返回带符号的夹角</param>
    /// <returns>两个投影向量间的夹角（弧度制）</returns>
    /// <exception cref="Exception">输入向量不能为零向量</exception>
    static member GetPlanarAngle(a: Vector3, b: Vector3, dir: Vector3, ?signed: bool) =
        // 异常处理：入参向量均不能为零
        if a = Vector3.Zero || b = Vector3.Zero || dir = Vector3.Zero then
            failwith "Input vectors cannot be zero" // TODO: 去掉异常

        let signed = defaultArg signed false
        // 1. 获取垂直于 dir 的投影平面法线
        let planeNormal = dir.Normalized()
        // 2. 投影向量到平面
        let aProj = a - planeNormal * a.Dot planeNormal
        let bProj = b - planeNormal * b.Dot planeNormal

        if aProj = Vector3.Zero || bProj = Vector3.Zero then
            // 3.1 处理零向量特殊情况
            0f // 或根据需求抛出异常
        // TODO：后续使用需要检验修改后的逻辑是否正确，目前下方代码实现和原来 C# 不同
        // var angle = Mathf.Acos(aProj.Normalized().Dot(bProj.Normalized()));
        // if (float.IsNaN(angle)) return 0f;
        // if (!signed) return angle;
        // // signed 需要返回范围 [-Pi, Pi] 的带方向角度
        // var cross = aProj.Cross(bProj);
        // float sign = Mathf.Sign(cross.Dot(dir.Normalized()));
        // return sign * angle;
        elif not signed then
            // 3.2 计算投影向量的夹角（弧度制）
            aProj.AngleTo(bProj)
        else
            aProj.SignedAngleTo(bProj, planeNormal)

    static member AlignYAxisToDirection(basis: Basis, direction: Vector3, ?alignForward: Vector3, ?inGlobal: bool) =
        let alignForward = defaultArg alignForward Vector3.Zero
        let inGlobal = defaultArg inGlobal false
        let mutable transform = Transform3D.Identity
        transform.Basis <- basis
        // 确保方向是单位向量
        let direction = direction.Normalized()
        // 当前 Y 轴
        let yAxis = if inGlobal then Vector3.Up else basis.Y
        // 计算旋转轴
        let rotationAxis = yAxis.Cross(direction)
        // 如果旋转轴长度为 0，说明方向相同或相反
        if rotationAxis.Length() = 0f then
            if yAxis.Dot direction > 0f then
                // 方向相同
                transform
            elif inGlobal then
                // 方向相反，绕 X 轴转 180 度
                transform.Rotated(Vector3.Right, Mathf.Pi)
            else
                transform.RotatedLocal(Vector3.Right, Mathf.Pi)
        else
            // 计算旋转角度
            let angle = yAxis.AngleTo direction

            if inGlobal then
                transform <- transform.Rotated(rotationAxis.Normalized(), angle)
            else
                transform <- transform.RotatedLocal(rotationAxis.Normalized(), angle)

            let alignForward = alignForward.Normalized()

            if alignForward <> Vector3.Zero && alignForward <> direction then
                // 如果有指定向前对齐方向，则对齐向前（-Z）到最近的方向
                let forward = if inGlobal then Vector3.Forward else -transform.Basis.Z
                let zAngle = Math3dUtil.GetPlanarAngle(forward, alignForward, direction, true)

                if inGlobal then
                    transform <- transform.Rotated(direction, zAngle)
                else
                    transform <- transform.RotatedLocal(direction, zAngle)

            transform


    static member PlaceOnSphere
        (basis: Basis, position: Vector3, scale: Vector3, ?addHeight: float32, ?alignForward: Vector3)
        =
        let addHeight = defaultArg addHeight 0f
        let alignForward = defaultArg alignForward Vector3.Zero

        let mutable transform =
            Math3dUtil.AlignYAxisToDirection(basis, position, alignForward)

        transform <- transform.Scaled scale
        transform.Origin <- position.Normalized() * (position.Length() + addHeight * scale.Y)
        transform
