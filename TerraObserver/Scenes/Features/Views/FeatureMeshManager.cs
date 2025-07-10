using System;
using System.Collections.Generic;
using Godot;
using TO.Domains.Types.Chunks;

namespace TerraObserver.Scenes.Features.Views;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-07-09 14:06:40
[Tool]
public partial class FeatureMeshManager : Node3D, IFeatureMeshManager
{
    #region export 变量

    [Export] public PackedScene[]? UrbanScenes { get; set; }
    [Export] public PackedScene[]? FarmScenes { get; set; }
    [Export] public PackedScene[]? PlantScenes { get; set; }
    [Export] public PackedScene? WallTowerScene { get; set; }
    [Export] public PackedScene? BridgeScene { get; set; }
    [Export] public PackedScene[]? SpecialScenes { get; set; }

    #endregion

    #region 普通属性

    public MultiMeshInstance3D[]? MultiUrbans { get; set; }
    public MultiMeshInstance3D[]? MultiFarms { get; set; }
    public MultiMeshInstance3D[]? MultiPlants { get; set; }
    public MultiMeshInstance3D? MultiTowers { get; set; }
    public MultiMeshInstance3D? MultiBridges { get; set; }
    public MultiMeshInstance3D[]? MultiSpecials { get; set; }
    public Dictionary<FeatureType, HashSet<int>> HidingIds { get; } = new();

    #endregion

    #region on-ready 节点

    public Node3D? Urbans { get; private set; }
    public Node3D? Farms { get; private set; }
    public Node3D? Plants { get; private set; }
    public Node3D? Others { get; private set; }

    #endregion

    #region 生命周期

    public override void _Ready()
    {
        Urbans = GetNode<Node3D>("%Urbans");
        Farms = GetNode<Node3D>("%Farms");
        Plants = GetNode<Node3D>("%Plants");
        Others = GetNode<Node3D>("%Others");
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
        foreach (var type in Enum.GetValues<FeatureType>())
            HidingIds[type] = [];
        return;

        void InitMultiMeshInstancesForCsgBox(string name, MultiMeshInstance3D[] multi,
            Node3D baseNode, PackedScene[] scenes, int instanceCount)
        {
            for (var i = 0; i < scenes.Length; i++)
            {
                multi[i] = InitMultiMeshIns($"{name}{i}", scenes[i], instanceCount);
                baseNode.AddChild(multi[i]);
            }
        }

        MultiMeshInstance3D InitMultiMeshIns(string name, PackedScene scene, int instanceCount)
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
    }

    #endregion
}