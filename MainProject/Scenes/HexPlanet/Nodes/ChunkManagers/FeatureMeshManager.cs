using System;
using System.Collections.Generic;
using System.Linq;
using Apps.Queries.Contexts;
using Contexts;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Nodes.Abstractions.ChunkManagers;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-26 20:53:19
[Tool]
public partial class FeatureMeshManager : Node3D, IFeatureMeshManager
{
    public FeatureMeshManager()
    {
        NodeContext.Instance.RegisterSingleton<IFeatureMeshManager>(this);
        Context.RegisterToHolder<IFeatureMeshManager>(this);
    }

    public NodeEvent? NodeEvent => null;

    public override void _Ready()
    {
        InitOnReadyNodes();
    }

    public override void _ExitTree() => NodeContext.Instance.DestroySingleton<IFeatureMeshManager>();

    #region export 变量

    [Export] private PackedScene[]? UrbanScenes { get; set; }
    [Export] private PackedScene[]? FarmScenes { get; set; }
    [Export] private PackedScene[]? PlantScenes { get; set; }
    [Export] private PackedScene? WallTowerScene { get; set; }
    [Export] private PackedScene? BridgeScene { get; set; }
    [Export] private PackedScene[]? SpecialScenes { get; set; }

    #endregion

    #region on-ready 节点

    private Node3D? Urbans { get; set; }
    private Node3D? Farms { get; set; }
    private Node3D? Plants { get; set; }
    private Node3D? Others { get; set; }

    private void InitOnReadyNodes()
    {
        Urbans = GetNode<Node3D>("%Urbans");
        Farms = GetNode<Node3D>("%Farms");
        Plants = GetNode<Node3D>("%Plants");
        Others = GetNode<Node3D>("%Others");
    }

    #endregion

    public MultiMeshInstance3D[]? MultiUrbans { get; private set; }
    public MultiMeshInstance3D[]? MultiFarms { get; private set; }
    public MultiMeshInstance3D[]? MultiPlants { get; private set; }
    public MultiMeshInstance3D? MultiTowers { get; private set; }
    public MultiMeshInstance3D? MultiBridges { get; private set; }
    public MultiMeshInstance3D[]? MultiSpecials { get; private set; }

    public void InitMultiMeshInstances()
    {
        MultiUrbans = new MultiMeshInstance3D[UrbanScenes!.Length];
        InitMultiMeshInstancesForCsgBox("Urbans", MultiUrbans, Urbans!, UrbanScenes, 10000);
        MultiFarms = new MultiMeshInstance3D[FarmScenes!.Length];
        InitMultiMeshInstancesForCsgBox("Farms", MultiFarms, Farms!, FarmScenes, 10000);
        MultiPlants = new MultiMeshInstance3D[PlantScenes!.Length];
        InitMultiMeshInstancesForCsgBox("Plants", MultiPlants, Plants!, PlantScenes, 10000);
        MultiSpecials = new MultiMeshInstance3D[SpecialScenes!.Length];
        InitMultiMeshInstancesForCsgBox("Specials", MultiSpecials, Others!, SpecialScenes, 1000);

        MultiTowers = InitMultiMeshIns("Towers", WallTowerScene!, 10000);
        Others!.AddChild(MultiTowers);
        MultiBridges = InitMultiMeshIns("Bridges", BridgeScene!, 3000);
        Others.AddChild(MultiBridges);

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
        foreach (var multi in MultiUrbans!.Concat(MultiFarms!).Concat(MultiPlants!))
        {
            multi.Multimesh.InstanceCount = 10000;
            multi.Multimesh.VisibleInstanceCount = 0;
        }

        foreach (var multi in MultiSpecials!)
        {
            multi.Multimesh.InstanceCount = 1000;
            multi.Multimesh.VisibleInstanceCount = 0;
        }

        MultiBridges!.Multimesh.InstanceCount = 3000;
        MultiBridges.Multimesh.VisibleInstanceCount = 0;
        MultiTowers!.Multimesh.InstanceCount = 10000;
        MultiTowers.Multimesh.VisibleInstanceCount = 0;
        // 清理 _hidingIds
        foreach (var (_, set) in HidingIds)
            set.Clear();
    }

    #region 动态加载特征

    public Dictionary<FeatureType, HashSet<int>> HidingIds { get; } = new();

    private void InitHidingIds()
    {
        foreach (var type in Enum.GetValues<FeatureType>())
            HidingIds[type] = [];
    }

    #endregion
}