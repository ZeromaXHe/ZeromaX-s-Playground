namespace TO.Domains.Functions.Maths

open Godot
open TO.Domains.Types.Godots

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-12 16:07:12
/// 作为对其他 .NET 公开的 F# 库（目前是使用了可选参数），需要遵循《F# 组件设计准则 - 命名空间和类型设计 - 使用命名空间、类型和成员作为组件的主要组织结构》
/// https://learn.microsoft.com/zh-cn/dotnet/fsharp/style-guide/component-design-guidelines#use-namespaces-types-and-members-as-the-primary-organizational-structure-for-your-components
[<AbstractClass; Sealed>]
type Node3dUtil =
    /// <summary>
    /// 对齐 Node3D 的 Y 轴正方向到指定的方向向量。
    /// </summary>
    /// <param name="node">要对齐的 Node3D 节点</param>
    /// <param name="direction">目标方向向量</param>
    /// <param name="alignForward">希望对齐向前的方向（不传则默认不调整）</param>
    /// <param name="inGlobal">Y 轴是否使用全局基（注意：全局基模式未经测试）</param>
    static member AlignYAxisToDirection(node: INode3D, direction: Vector3, ?alignForward: Vector3, ?inGlobal: bool) =
        let alignForward = defaultArg alignForward Vector3.Zero
        let inGlobal = defaultArg inGlobal false
        // 暂时不知道如何整合 Node3dUtil 和 Math3dUtil 的类似方法
        // node.Transform = Math3dUtil.AlignYAxisToDirection(global ? node.GlobalBasis : node.Basis,
        //     direction, alignForward, global);
        // 确保方向是单位向量
        let direction = direction.Normalized()
        // 当前 Y 轴
        let yAxis = if inGlobal then node.GlobalBasis.Y else node.Basis.Y
        // 计算旋转轴
        let rotationAxis = yAxis.Cross(direction)
        // 如果旋转轴长度为 0，说明方向相同或相反
        if rotationAxis.Length() = 0f then
            if yAxis.Dot direction > 0f then
                // 方向相同
                () // return
            elif inGlobal then
                // 方向相反，绕 X 轴转 180 度
                node.GlobalRotate(Vector3.Right, Mathf.Pi)
            else
                node.RotateX Mathf.Pi
        else
            // 计算旋转角度
            let angle = yAxis.AngleTo direction

            if inGlobal then
                // 相当于 node.GlobalRotation = new Quaternion(rotationAxis.Normalized(), angle).GetEuler();
                node.GlobalRotate(rotationAxis.Normalized(), angle)
            else
                // 相当于 node.Rotation = new Quaternion(rotationAxis.Normalized(), angle).GetEuler();
                node.Rotate(rotationAxis.Normalized(), angle)

            let alignForward = alignForward.Normalized()

            if alignForward <> Vector3.Zero && alignForward <> direction then
                // 如果有指定向前对齐方向，则对齐向前（-Z）到最近的方向
                let forward = if inGlobal then Vector3.Forward else -node.Basis.Z
                let zAngle = Math3dUtil.GetPlanarAngle(forward, alignForward, direction, true)

                if inGlobal then
                    node.GlobalRotate(direction, zAngle)
                else
                    node.Rotate(direction, zAngle)


    static member PlaceOnSphere
        (node: INode3D, position: Vector3, scale: float32, ?addHeight: float32, ?alignForward: Vector3)
        =
        let addHeight = defaultArg addHeight 0f
        let alignForward = defaultArg alignForward Vector3.Zero
        // 暂时不知道如何整合 Node3dUtil 和 Math3dUtil 的类似方法
        // node.Transform = Math3dUtil.PlaceOnSphere(node.Basis, position, Vector3.One * scale, addHeight, alignForward);
        node.Scale <- Vector3.One * scale
        node.Position <- position.Normalized() * (position.Length() + addHeight * scale)
        Node3dUtil.AlignYAxisToDirection(node, position, alignForward)
