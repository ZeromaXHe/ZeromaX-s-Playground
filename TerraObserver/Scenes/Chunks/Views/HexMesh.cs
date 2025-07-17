using Godot;
using TO.Domains.Types.Chunks;

namespace TerraObserver.Scenes.Chunks.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-06 16:33:04
[Tool]
public partial class HexMesh : MeshInstance3D, IHexMesh
{
    #region Export 属性

    [Export] public bool UseCollider { get; set; }
    [Export] public bool UseCellData { get; set; }
    [Export] public bool UseUvCoordinates { get; set; }
    [Export] public bool UseUv2Coordinates { get; set; }
    [Export] public bool Smooth { get; set; }

    #endregion

    #region 普通属性

    public SurfaceTool SurfaceTool { get; set; } = new();
    public int VIdx { get; set; }

    #endregion

    /// <summary>
    /// 绘制三角形
    /// </summary>
    /// <param name="vs">顶点数组 vertices</param>
    /// <param name="tws">地块权重 tWeights</param>
    /// <param name="uvs">UV</param>
    /// <param name="uvs2">UV2</param>
    /// <param name="tis">地块ID tileIds</param>
    public void AddTriangle(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 3; i++)
        {
            if (UseCellData && tws != null)
            {
                SurfaceTool.SetColor(tws[i]);
                SurfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

            if (UseUvCoordinates && uvs != null)
                SurfaceTool.SetUV(uvs[i]);
            if (UseUv2Coordinates && uvs2 != null)
                SurfaceTool.SetUV2(uvs2[i]);
            SurfaceTool.AddVertex(vs[i]);
        }

        SurfaceTool.AddIndex(VIdx);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 2);
        VIdx += 3;
    }

    public void AddQuad(Vector3[] vs, Color[]? tws = null,
        Vector2[]? uvs = null, Vector2[]? uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 4; i++)
        {
            if (UseCellData && tws != null)
            {
                SurfaceTool.SetColor(tws[i]);
                SurfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

            if (UseUvCoordinates && uvs != null)
                SurfaceTool.SetUV(uvs[i]);
            if (UseUvCoordinates && uvs2 != null)
                SurfaceTool.SetUV2(uvs2[i]);
            SurfaceTool.AddVertex(vs[i]);
        }

        SurfaceTool.AddIndex(VIdx);
        SurfaceTool.AddIndex(VIdx + 2);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 1);
        SurfaceTool.AddIndex(VIdx + 2);
        SurfaceTool.AddIndex(VIdx + 3);
        VIdx += 4;
    }
}