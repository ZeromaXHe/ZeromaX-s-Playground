using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexPlaneGrid;

/// <summary>
/// 参考教程链接：https://www.redblobgames.com/grids/hexagons/
/// </summary>
public static class HexPlaneGridUtil
{
    /// <summary>
    /// 在常规六边形中内角为 120°。会有六个“楔子”，每个是一个等边三角形，内角都为 60°。
    /// 每个角落距离 `center` 为 `size` 单位。
    /// 
    /// In a regular hexagon the interior angles are 120°.
    /// There are six “wedges”, each an equilateral triangle with 60° angles inside.
    /// Each corner is `size` units away from the `center`.
    /// </summary>
    /// <param name="center">中心坐标</param>
    /// <param name="size">角到中心的距离</param>
    /// <param name="i">第几个角</param>
    /// <returns>角的坐标</returns>
    public static Vector2 PointyHexCorner(Vector2 center, float size, int i)
    {
        var angleDeg = 60 * i - 30;
        var angleRad = Mathf.DegToRad(angleDeg);
        return new Vector2(center.X + size * Mathf.Cos(angleRad), center.Y + size * Mathf.Sin(angleRad));
    }
}