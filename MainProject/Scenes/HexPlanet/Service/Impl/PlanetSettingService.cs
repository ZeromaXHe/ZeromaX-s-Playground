using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service.Impl;

public class PlanetSettingService: IPlanetSettingService
{
    private float _radius = 150f;

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            CalcUnitHeight();
        }
    }

    private int _divisions = 10;

    public int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            SphereAxial.Div = _divisions; // TODO：后续修改这个逻辑，临时在这里处理以方便测试 SphereAxial
            CalcUnitHeight();
        }
    }

    public int ChunkDivisions { get; set; } = 2;

    // 单位高度
    public float UnitHeight { get; private set; } = 1.5f;
    public float MaxHeight { get; private set; } = 15f;
    public float MaxHeightRatio { get; private set; } = 0.1f;
    private const float MaxHeightRadiusRatio = 0.2f;
    public int ElevationStep { get; set; } = 10; // 这里对应含义是 Elevation 分为几级

    private void CalcUnitHeight()
    {
        MaxHeightRatio = StandardScale * MaxHeightRadiusRatio;
        MaxHeight = Radius * MaxHeightRatio;
        UnitHeight = MaxHeight / ElevationStep;
    }

    public float StandardScale => Radius / HexMetrics.StandardRadius * HexMetrics.StandardDivisions / Divisions;

    public float GetStreamBedHeight(int elevation) => (elevation + HexMetrics.StreamBedElevationOffset) * UnitHeight;
    public float GetWaterSurfaceHeight(int level) => (level + HexMetrics.WaterElevationOffset) * UnitHeight;
    
    #region 特征

    private readonly float[][] FeatureThresholds =
    [
        [0.0f, 0.0f, 0.4f],
        [0.0f, 0.4f, 0.6f],
        [0.4f, 0.6f, 0.8f]
    ];

    public float[] GetFeatureThreshold(int level) => FeatureThresholds[level];
    private const float WallHeight = 2f;
    private const float WallYOffset = -0.5f;
    private const float WallThickness = 0.375f;
    public float GetWallHeight() => UnitHeight * WallHeight;
    public float GetWallThickness() => UnitHeight * WallThickness;

    /// <summary>
    /// 按照厚度找到墙偏移向量
    /// </summary>
    /// <param name="near">近端坐标</param>
    /// <param name="far">远端坐标</param>
    /// <param name="toNear">true，则求偏移向近端的方向；false，则向远端</param>
    /// <param name="thickness">墙厚度</param>
    /// <returns>从近端和远端等平均高度位置的球面中点，向墙厚位置的偏移向量</returns>
    public Vector3 WallThicknessOffset(Vector3 near, Vector3 far, bool toNear, float thickness)
    {
        var avgHeight = (near.Length() + far.Length()) / 2f;
        near = Math3dUtil.ProjectToSphere(near, avgHeight);
        far = Math3dUtil.ProjectToSphere(far, avgHeight);
        var mid = near.Slerp(far, 0.5f);
        var sphereDistance = near.AngleTo(far) * avgHeight;
        var target = toNear ? near : far;
        return mid.Slerp(target, thickness / sphereDistance);
    }

    private const float WallElevationOffset = HexMetrics.VerticalTerraceStepSize;

    public Vector3 WallLerp(Vector3 near, Vector3 far)
    {
        var mid = near.Slerp(far, 0.5f);
        var v = near.Length() < far.Length() ? WallElevationOffset : 1f - WallElevationOffset;
        return Math3dUtil.ProjectToSphere(mid,
            Mathf.Lerp(near.Length(), far.Length(), v) + UnitHeight * WallYOffset);
    }

    #endregion
}