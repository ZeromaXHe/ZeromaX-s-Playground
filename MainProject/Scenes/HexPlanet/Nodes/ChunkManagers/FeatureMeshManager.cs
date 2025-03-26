using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 20:53:19
[Tool]
public partial class FeatureMeshManager : Node3D
{
    [Export] private PackedScene[] _urbanScenes;
    [Export] private PackedScene[] _farmScenes;
    [Export] private PackedScene[] _plantScenes;
    [Export] private PackedScene _wallTowerScene;
    [Export] private PackedScene _bridgeScene;
    [Export] private PackedScene[] _specialScenes;

    #region on-ready 节点

    private Node3D _urbans;
    private Node3D _farms;
    private Node3D _plants;
    private Node3D _others;

    private void InitOnReadyNodes()
    {
        _urbans = GetNode<Node3D>("%Urbans");
        _farms = GetNode<Node3D>("%Farms");
        _plants = GetNode<Node3D>("%Plants");
        _others = GetNode<Node3D>("%Others");
    }

    #endregion

    public override void _Ready()
    {
        InitOnReadyNodes();
    }

    private MultiMeshInstance3D[] _multiUrbans;
    private MultiMeshInstance3D[] _multiFarms;
    private MultiMeshInstance3D[] _multiPlants;
    private MultiMeshInstance3D _multiTowers;
    private MultiMeshInstance3D _multiBridges;
    private MultiMeshInstance3D[] _multiSpecials;

    public void InitMultiMeshInstances()
    {
        _multiUrbans = new MultiMeshInstance3D[_urbanScenes.Length];
        InitMultiMeshInstancesForCsgBox("Urbans", _multiUrbans, _urbans, _urbanScenes, 10000);
        _multiFarms = new MultiMeshInstance3D[_farmScenes.Length];
        InitMultiMeshInstancesForCsgBox("Farms", _multiFarms, _farms, _farmScenes, 10000);
        _multiPlants = new MultiMeshInstance3D[_plantScenes.Length];
        InitMultiMeshInstancesForCsgBox("Plants", _multiPlants, _plants, _plantScenes, 10000);
        _multiSpecials = new MultiMeshInstance3D[_specialScenes.Length];
        InitMultiMeshInstancesForCsgBox("Specials", _multiSpecials, _others, _specialScenes, 1000);

        _multiTowers = InitMultiMeshIns("Towers", _wallTowerScene, 10000);
        _others.AddChild(_multiTowers);
        _multiBridges = InitMultiMeshIns("Bridges", _bridgeScene, 3000);
        _others.AddChild(_multiBridges);

        // 初始化 _hidingIds
        InitHidingIds();
    }

    private void InitMultiMeshInstancesForCsgBox(string name, MultiMeshInstance3D[] multi,
        Node3D baseNode, PackedScene[] scenes, int instanceCount)
    {
        for (var i = 0; i < scenes.Length; i++)
        {
            multi[i] = InitMultiMeshIns($"{name}{i}", scenes[i], instanceCount);
            baseNode.AddChild(multi[i]);
        }
    }

    private MultiMeshInstance3D InitMultiMeshIns(string name, PackedScene scene, int instanceCount)
    {
        var mesh = new MultiMesh();
        mesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        mesh.InstanceCount = instanceCount;
        mesh.VisibleInstanceCount = 0;

        var csgBox = scene.Instantiate<CsgBox3D>();
        // 【注意！】要延迟执行。因为需要等待一帧后 CSG 才会计算完成，否则直接调用 bakedMesh 为 null。
        // 参考 https://forum.godotengine.org/t/csg-bake-static-mesh-thorugh-code-returning-null/97080
        Callable.From(() =>
        {
            var bakedMesh = csgBox.BakeStaticMesh();
            mesh.SetMesh(bakedMesh);
            csgBox.QueueFree(); // 切记释放内存，防止最后退出场景时会报错内存泄漏
        }).CallDeferred();
        return new MultiMeshInstance3D { Name = name, Multimesh = mesh };
    }

    public void ClearOldData()
    {
        // 刷新 MultiMesh
        foreach (var multi in _multiUrbans.Concat(_multiFarms).Concat(_multiPlants))
        {
            multi.Multimesh.InstanceCount = 10000;
            multi.Multimesh.VisibleInstanceCount = 0;
        }

        foreach (var multi in _multiSpecials)
        {
            multi.Multimesh.InstanceCount = 1000;
            multi.Multimesh.VisibleInstanceCount = 0;
        }

        _multiBridges.Multimesh.InstanceCount = 3000;
        _multiBridges.Multimesh.VisibleInstanceCount = 0;
        _multiTowers.Multimesh.InstanceCount = 10000;
        _multiTowers.Multimesh.VisibleInstanceCount = 0;
        // 清理 _hidingIds
        foreach (var (_, set) in _hidingIds)
            set.Clear();
    }

    public MultiMesh GetMultiMesh(FeatureType type) => type switch
    {
        // 城市
        FeatureType.UrbanHigh1 or FeatureType.UrbanHigh2
            or FeatureType.UrbanMid1 or FeatureType.UrbanMid2
            or FeatureType.UrbanLow1 or FeatureType.UrbanLow2 =>
            _multiUrbans[type - FeatureType.UrbanHigh1].Multimesh,
        // 农田
        FeatureType.FarmHigh1 or FeatureType.FarmHigh2
            or FeatureType.FarmMid1 or FeatureType.FarmMid2
            or FeatureType.FarmLow1 or FeatureType.FarmLow2 =>
            _multiFarms[type - FeatureType.FarmHigh1].Multimesh,

        // 植被
        FeatureType.PlantHigh1 or FeatureType.PlantHigh2
            or FeatureType.PlantMid1 or FeatureType.PlantMid2
            or FeatureType.PlantLow1 or FeatureType.PlantLow2 =>
            _multiPlants[type - FeatureType.PlantHigh1].Multimesh,

        // 特殊
        FeatureType.Tower => _multiTowers.Multimesh,
        FeatureType.Bridge => _multiBridges.Multimesh,
        FeatureType.Castle or FeatureType.Ziggurat or FeatureType.MegaFlora =>
            _multiSpecials[type - FeatureType.Castle].Multimesh,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
    };

    #region 动态加载特征

    private readonly Dictionary<FeatureType, HashSet<int>> _hidingIds = new();

    private void InitHidingIds()
    {
        foreach (var type in Enum.GetValues<FeatureType>())
            _hidingIds[type] = [];
    }

    // 将特征缩小并放到球心，表示不可见
    private static readonly Transform3D HideTransform3D = Transform3D.Identity.Scaled(Vector3.One * 0.0001f);

    public void OnHideFeature(int id, FeatureType type)
    {
        var mesh = GetMultiMesh(type);
        mesh.SetInstanceTransform(id, HideTransform3D);
        if (mesh.VisibleInstanceCount - 1 == id) // 如果是最后一个，则可以考虑缩小可见实例数
        {
            var popId = id - 1;
            while (_hidingIds[type].Contains(id - 1))
            {
                _hidingIds[type].Remove(popId);
                popId--;
            }

            mesh.VisibleInstanceCount = popId + 1;
        }
        else
            _hidingIds[type].Add(id);
    }

    #endregion

    public int OnShowFeature(Transform3D transform, FeatureType type)
    {
        var mesh = GetMultiMesh(type);
        if (_hidingIds[type].Count > 0)
        {
            // 如果有隐藏的实例，则可以考虑复用
            var popId = _hidingIds[type].First();
            mesh.SetInstanceTransform(popId, transform);
            _hidingIds[type].Remove(popId);
            return popId;
        }

        if (mesh.VisibleInstanceCount == mesh.InstanceCount)
        {
            GD.PrintErr($"MultiMesh is full of {mesh.InstanceCount} {type}");
            return -1;
        }

        var id = mesh.VisibleInstanceCount;
        mesh.SetInstanceTransform(id, transform);
        mesh.VisibleInstanceCount++;
        return id;
    }
}