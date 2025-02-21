using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

public static class Math3dUtil
{
    public static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var side1 = v2 - v1;
        var side2 = v3 - v1;
        return side1.Cross(side2).Normalized();
    }

    public static bool IsNormalAwayFromOrigin(Vector3 surface, Vector3 normal, Vector3 origin) =>
        (surface - origin).Dot(normal) > 0;

    public static Vector3 ProjectToSphere(Vector3 p, float radius, float scale = 1f)
    {
        var projectionPoint = radius / p.Length();
        return p * projectionPoint * scale;
    }


    /// <summary>
    /// 排序顶点，使缠绕方向朝外
    /// </summary>
    /// <param name="origin">在形状内部的一个起点</param>
    /// <param name="vs">顶点数组（目前支持 3 个和 4 个）</param>
    /// <returns>使缠绕方向朝外，且方便 AddTriangle/AddQuad 入参的顶点数组索引排序</returns>
    public static int[] SortVertices(Vector3 origin, Vector3[] vs)
    {
        var center = (vs[0] + vs[1] + vs[2]) / 3f;
        // 决定缠绕顺序
        var normal = GetNormal(vs[0], vs[1], vs[2]);
        var isRightSeq = IsNormalAwayFromOrigin(center, normal, origin);
        if (vs.Length == 3)
            return isRightSeq
                ? [0, 2, 1]
                : [0, 1, 2];
        // 目前只有 3 个顶点和 4 个顶点两种入参
        // 四个顶点的入参时，请按照类似 [左下, 右下, 左上, 右上] 的顶点顺序传入
        return isRightSeq
                ? [0, 1, 2, 3]
                : [1, 0, 3, 2];
            
    }
}