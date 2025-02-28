using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class Node3dUtil
{
    /// <summary>
    /// 对齐 Node3D 的 Y 轴正方向到指定的方向向量。
    /// </summary>
    /// <param name="node">要对齐的 Node3D 节点</param>
    /// <param name="direction">目标方向向量</param>
    /// <param name="alignForward">希望对齐向前的方向（不传则默认不调整）</param>
    /// <param name="global">Y 轴是否使用全局基（注意：全局基模式未经测试）</param>
    public static void AlignYAxisToDirection(in Node3D node, Vector3 direction, Vector3 alignForward = default, bool global = false)
    {
        // 确保方向是单位向量
        direction = direction.Normalized();
        // 当前 Y 轴
        var yAxis = global ? node.GlobalBasis.Y : node.Basis.Y;
        // 计算旋转轴
        var rotationAxis = yAxis.Cross(direction);
        // 如果旋转轴长度为 0，说明方向相同或相反
        if (rotationAxis.Length() == 0)
        {
            if (yAxis.Dot(direction) > 0) return; // 方向相同
            // 方向相反，绕 X 轴转 180 度
            if (global)
                node.GlobalRotate(Vector3.Right, Mathf.Pi);
            else
                node.RotateX(Mathf.Pi);
            return;
        }

        // 计算旋转角度
        var angle = yAxis.AngleTo(direction);
        if (global)
            // 相当于 node.GlobalRotation = new Quaternion(rotationAxis.Normalized(), angle).GetEuler();
            node.GlobalRotate(rotationAxis.Normalized(), angle);
        else
            // 相当于 node.Rotation = new Quaternion(rotationAxis.Normalized(), angle).GetEuler();
            node.Rotate(rotationAxis.Normalized(), angle);

        alignForward = alignForward.Normalized();
        if (alignForward != default && alignForward != direction)
        {
            // 如果有指定向前对齐方向，则对齐向前（-Z）到最近的方向
            var forward = -(global ? node.GlobalBasis.Z : node.Basis.Z);
            var zAngle = Math3dUtil.GetPlanarAngle(forward, alignForward, direction, true);
            if (global)
                node.GlobalRotate(direction, zAngle);
            else
                node.Rotate(direction, zAngle);
        }
    }
}