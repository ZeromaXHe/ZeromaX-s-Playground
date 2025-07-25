using System;
using Commons.Constants;
using Commons.Utils;
using Commons.Utils.HexSphereGrid;
using Contexts;
using Godot;
using Godot.Collections;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-12 21:07
[Tool]
public partial class HexPlanetManager : Node3D, IHexPlanetManager, ISerializationListener
{
    // Godot C# 的生命周期方法执行顺序：
    // 父节点构造函数 -> 子节点构造函数
    // （构造函数这个顺序存疑。目前 4.4 发现闪退现象，因为 HexGridChunk 构造函数优先于 HexPlanetManager 构造函数执行了！
    // 目前猜想可能是因为引入 uid 后，[Export] PackageScene 挂载的脚本构造函数生命周期提前了？）
    // -> 父节点 _EnterTree() -> 子节点 _EnterTree()（从上到下）
    // -> 子节点 _Ready()（从下到上） -> 父节点 _Ready() 【特别注意这里的顺序！！！】
    // -> 父节点 _Process() -> 子节点 _Process()（从上到下）
    // -> 子节点 _ExitTree()（从下到上） -> 父节点 _ExitTree() 【特别注意这里的顺序！！！】
    public HexPlanetManager()
    {
        // 现在 4.4 甚至构造函数会执行两次！奇了怪了，不知道之前 4.3 是不是也是这样
        // 调用两次构造函数（_EnterTree()、_Ready() 也一样）居然是好久以前（2020 年 7 月 3.2.2）以来一直的问题：
        // https://github.com/godotengine/godot-docs/issues/2930#issuecomment-662407208
        // https://github.com/godotengine/godot/issues/40970
        Context.RegisterToHolder<IHexPlanetManager>(this);
    }

    public event Action? NewPlanetGenerated;
    public event Action<float>? RadiusChanged;

    public NodeEvent NodeEvent { get; } = new(process: true);

    private bool NodeReady { get; set; }

    public override void _Ready()
    {
        GD.Print("HexPlanetManager _Ready start");
        InitOnReadyNodes();
        GD.Print("HexPlanetManager _Ready end");
    }

    public override void _ExitTree()
    {
        GD.Print("HexPlanetManager _ExitTree");
        NodeReady = false;
    }

    public override void _Process(double delta) => NodeEvent.EmitProcessed(delta);

    private float _radius = 100f;

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if (NodeReady)
            {
                RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.Radius, _radius);
                CalcUnitHeight();
                var camAttr = PlanetCamera!.Attributes as CameraAttributesPractical;
                camAttr?.SetDofBlurFarDistance(_radius);
                camAttr?.SetDofBlurFarTransition(_radius / 2);
                camAttr?.SetDofBlurNearDistance(_radius / 10);
                camAttr?.SetDofBlurNearTransition(_radius / 20);
                _orbitCamera!.Reset(value);
                var groundSphere = GroundPlaceHolder!.Mesh as SphereMesh;
                groundSphere?.SetRadius(_radius * 0.99f);
                groundSphere?.SetHeight(_radius * 1.98f);
                PlanetAtmosphere!.Set("planet_radius", _radius);
                PlanetAtmosphere.Set("atmosphere_height", _radius * 0.25f);
                _longitudeLatitude!.Draw(_radius + MaxHeight * 1.25f);
                RadiusChanged?.Invoke(value);
            }
        }
    }

    private int _divisions = 20;

    [Export(PropertyHint.Range, "1, 200")]
    public int Divisions
    {
        get => _divisions;
        set
        {
            _divisions = value;
            _chunkDivisions = Mathf.Min(Mathf.Max(1, _divisions / 10), _chunkDivisions);
            if (NodeReady)
            {
                RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.Divisions, _divisions);
                SphereAxial.Div = _divisions; // TODO：后续修改这个逻辑，临时在这里处理以方便测试 SphereAxial
                CalcUnitHeight();
                _orbitCamera!.Reset(_radius);
            }
        }
    }

    private int _chunkDivisions = 5;

    [Export(PropertyHint.Range, "1, 20")]
    public int ChunkDivisions
    {
        get => _chunkDivisions;
        set
        {
            _chunkDivisions = value;
            _divisions = Mathf.Max(Mathf.Min(200, _chunkDivisions * 10), _divisions);
            if (NodeReady)
            {
                RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.Divisions, _divisions);
                SphereAxial.Div = _divisions; // TODO：后续修改这个逻辑，临时在这里处理以方便测试 SphereAxial
                CalcUnitHeight();
                _orbitCamera!.Reset(_radius);
            }
        }
    }

    // 其实这里可以直接导入 Image, 在导入界面选择导入类型。但是导入 Image 的场景 tscn 文件会大得吓人……（等于直接按像素写一遍）
    [Export] public Texture2D? NoiseSource { get; set; }
    [Export] public ulong Seed { get; set; } = 1234;

    // 行星公转
    [ExportGroup("天体运动")] [Export] public bool PlanetRevolution { get; set; } = true;

    // 行星自转
    [Export] public bool PlanetRotation { get; set; } = true;

    // 卫星公转
    [Export] public bool SatelliteRevolution { get; set; } = true;

    // 卫星自转
    [Export] public bool SatelliteRotation { get; set; } = true;

    public float OldRadius { get; set; }
    public int OldDivisions { get; set; }
    public int OldChunkDivisions { get; set; }
    public float LastUpdated { get; set; }

    #region on-ready nodes

    public Node3D? PlanetContainer { get; private set; } // 所有行星相关节点的父节点，用于整体一起自转

    // 行星节点
    public Node3D? PlanetAtmosphere { get; private set; } // 大气层插件的 GDScript 节点
    public MeshInstance3D? GroundPlaceHolder { get; private set; } // 球面占位网格
    private OrbitCamera? _orbitCamera;
    public Camera3D? PlanetCamera { get; private set; } // 行星主摄像机
    private LongitudeLatitude? _longitudeLatitude;

    private void InitOnReadyNodes()
    {
        PlanetContainer = GetNode<Node3D>("%PlanetContainer");
        // 行星节点
        PlanetAtmosphere = GetNode<Node3D>("%PlanetAtmosphere");
        GroundPlaceHolder = GetNode<MeshInstance3D>("%GroundPlaceHolder");
        // 此处要求 OrbitCamera 也是 [Tool]，否则编辑器里会转型失败
        _orbitCamera = GetNode<OrbitCamera>("%OrbitCamera");
        PlanetCamera = GetNode<Camera3D>("%PlanetCamera");
        _longitudeLatitude = GetNode<LongitudeLatitude>("%LongitudeLatitude");
        NodeReady = true;
    }

    #endregion

    #region 噪声扰动

    public Image? NoiseSourceImage { get; set; }

    #endregion

    #region 星球设置

    // 单位高度
    public float UnitHeight { get; private set; } = 1.5f;
    public float MaxHeight { get; private set; } = 15f;
    public float MaxHeightRatio { get; private set; } = 0.1f;
    private const float MaxHeightRadiusRatio = 0.2f;

    // [Export(PropertyHint.Range, "10, 15")]
    public int ElevationStep { get; set; } = 10; // 这里对应含义是 Elevation 分为几级

    private void CalcUnitHeight()
    {
        MaxHeightRatio = StandardScale * MaxHeightRadiusRatio;
        MaxHeight = Radius * MaxHeightRatio;
        RenderingServer.GlobalShaderParameterSet(GlobalShaderParam.MaxHeight, MaxHeight);
        UnitHeight = MaxHeight / ElevationStep;
    }

    public float StandardScale => Radius / HexMetrics.StandardRadius * HexMetrics.StandardDivisions / Divisions;

    // 默认水面高度 [Export(PropertyHint.Range, "1, 5")]
    public int DefaultWaterLevel { get; set; } = 5;

    #endregion

    private Dictionary GetTileCollisionResult()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var camera = GetViewport().GetCamera3D();
        var mousePos = GetViewport().GetMousePosition();
        var origin = camera.ProjectRayOrigin(mousePos);
        var end = origin + camera.ProjectRayNormal(mousePos) * 2000f;
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        return spaceState.IntersectRay(query);
    }

    public Vector3 GetTileCollisionPositionUnderCursor()
    {
        var result = GetTileCollisionResult();
        if (result is { Count: > 0 } && result.TryGetValue("position", out var position))
            return ToPlanetLocal(position.AsVector3());
        return Vector3.Zero;
    }

    public void EmitNewPlanetGenerated()
    {
        NewPlanetGenerated?.Invoke(); // 触发事件
    }

    public Vector3 ToPlanetLocal(Vector3 global) => PlanetContainer!.ToLocal(global);

    public void OnBeforeSerialize()
    {
        GD.Print("OnBeforeSerialize");
        // 解决 .NET: Failed to unload assemblies. Please check <this issue> for more information. 问题
        // GitHub issue 78513：https://github.com/godotengine/godot/issues/78513
        Context.UnloadBeanContext();
    }

    public void OnAfterDeserialize()
    {
        GD.Print("OnAfterDeserialize");
        Context.InitBeanContext();
    }
}