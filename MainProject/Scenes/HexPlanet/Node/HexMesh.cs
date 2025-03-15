using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexMesh : MeshInstance3D
{
    public HexMesh() => InitServices();
    [Export] public bool UseCollider { get; set; }
    [Export] public bool UseCellData { get; set; }
    [Export] public bool UseUvCoordinates { get; set; }
    [Export] public bool UseUv2Coordinates { get; set; }
    [Export] public bool Smooth { get; set; }

    #region 服务

    private static INoiseService _noiseService;

    private static void InitServices()
    {
        _noiseService ??= Context.GetBean<INoiseService>();
    }

    #endregion

    private SurfaceTool _surfaceTool = new();
    private int _vIdx;

    private Mesh[] _lodMeshes = new Mesh[System.Enum.GetValues<ChunkLod>().Length];

    public static readonly Color Weights1 = Colors.Red;
    public static readonly Color Weights2 = Colors.Green;
    public static readonly Color Weights3 = Colors.Blue;
    public static T[] TriArr<T>(T c) => [c, c, c];
    public static T[] QuadArr<T>(T c) => [c, c, c, c];
    public static T[] QuadArr<T>(T c1, T c2) => [c1, c1, c2, c2];

    public void Clear()
    {
        // 清理之前的碰撞体
        foreach (var child in GetChildren())
            child.QueueFree();
        // 清理所有的 Lod 网格
        for (var i = 0; i < _lodMeshes.Length; i++)
            _lodMeshes[i] = null;
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        if (!Smooth)
            _surfaceTool.SetSmoothGroup(uint.MaxValue);
        if (UseCellData)
            _surfaceTool.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbFloat);
    }

    public void Apply(ChunkLod lod = ChunkLod.Full)
    {
        _surfaceTool.GenerateNormals();
        Mesh = _surfaceTool.Commit();
        _lodMeshes[(int)lod] = Mesh;
        // 仅在游戏中生成碰撞体
        if (!Engine.IsEditorHint() && UseCollider)
            CreateTrimeshCollision();
        _surfaceTool.Clear(); // 释放 SurfaceTool 中的内存
        _vIdx = 0;
    }

    public void ShowLod(ChunkLod lod) => Mesh = _lodMeshes[(int)lod];

    /// <summary>
    /// 绘制三角形
    /// </summary>
    /// <param name="vs">顶点数组 vertices</param>
    /// <param name="tws">地块权重 tWeights</param>
    /// <param name="uvs">UV</param>
    /// <param name="uvs2">UV2</param>
    /// <param name="tis">地块ID tileIds</param>
    public void AddTriangle(Vector3[] vs, Color[] tws = null,
        Vector2[] uvs = null, Vector2[] uvs2 = null, Vector3 tis = default) =>
        AddTriangleUnperturbed(vs.Select(_noiseService.Perturb).ToArray(), tws, uvs, uvs2, tis);

    public void AddTriangleUnperturbed(Vector3[] vs, Color[] tws = null,
        Vector2[] uvs = null, Vector2[] uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 3; i++)
        {
            if (UseCellData && tws != null)
            {
                _surfaceTool.SetColor(tws[i]);
                _surfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

            if (UseUvCoordinates && uvs != null)
                _surfaceTool.SetUV(uvs[i]);
            if (UseUv2Coordinates && uvs2 != null)
                _surfaceTool.SetUV2(uvs2[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _vIdx += 3;
    }

    public void AddQuad(Vector3[] vs, Color[] tws = null,
        Vector2[] uvs = null, Vector2[] uvs2 = null, Vector3 tis = default) =>
        AddQuadUnperturbed(vs.Select(_noiseService.Perturb).ToArray(), tws, uvs, uvs2, tis);

    public void AddQuadUnperturbed(Vector3[] vs, Color[] tws = null,
        Vector2[] uvs = null, Vector2[] uvs2 = null, Vector3 tis = default)
    {
        for (var i = 0; i < 4; i++)
        {
            if (UseCellData && tws != null)
            {
                _surfaceTool.SetColor(tws[i]);
                _surfaceTool.SetCustom(0, new Color(tis.X, tis.Y, tis.Z));
            }

            if (UseUvCoordinates && uvs != null)
                _surfaceTool.SetUV(uvs[i]);
            if (UseUvCoordinates && uvs2 != null)
                _surfaceTool.SetUV2(uvs2[i]);
            _surfaceTool.AddVertex(vs[i]);
        }

        _surfaceTool.AddIndex(_vIdx);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 1);
        _surfaceTool.AddIndex(_vIdx + 2);
        _surfaceTool.AddIndex(_vIdx + 3);
        _vIdx += 4;
    }
}