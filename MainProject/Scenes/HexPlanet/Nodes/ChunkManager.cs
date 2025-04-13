using Apps.Events;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes.ChunkManagers;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-03-12 12:46
[Tool]
public partial class ChunkManager : Node3D
{
    #region on-ready 节点

    private FeatureMeshManager _featureMeshManager;
    private FeaturePreviewManager _featurePreviewManager;
    private ChunkLoader _chunkLoader;

    private void InitOnReadyNodes()
    {
        _featureMeshManager = GetNode<FeatureMeshManager>("%FeatureMeshManager");
        _featurePreviewManager = GetNode<FeaturePreviewManager>("%FeaturePreviewManager");
        _chunkLoader = GetNode<ChunkLoader>("%ChunkLoader");
    }

    #endregion

    #region 动态加载特征

    private void OnHideFeature(int id, FeatureType type, bool preview)
    {
        if (preview)
            _featurePreviewManager.OnHideFeature(id, type);
        else
            _featureMeshManager.OnHideFeature(id, type);
    }

    private int OnShowFeature(Transform3D transform, FeatureType type, bool preview) =>
        preview
            ? _featurePreviewManager.OnShowFeature(transform, type, _featureMeshManager.GetMultiMesh(type).Mesh)
            : _featureMeshManager.OnShowFeature(transform, type);

    #endregion

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        _featureMeshManager.InitMultiMeshInstances();
        _ready = true;
    }

    public override void _ExitTree() => CleanEventListeners();

    private void CleanEventListeners()
    {
        _ready = false;
        FeatureEvent.Instance.Shown -= OnShowFeature;
        FeatureEvent.Instance.Hidden -= OnHideFeature;
    }

    public void ClearOldData()
    {
        // 清空分块
        FeatureEvent.Instance.Shown -= OnShowFeature;
        FeatureEvent.Instance.Hidden -= OnHideFeature;
        _chunkLoader.ClearOldData();
        _featurePreviewManager.ClearForData();
        _featureMeshManager.ClearOldData();
    }

    public void InitChunkNodes()
    {
        var time = Time.GetTicksMsec();
        FeatureEvent.Instance.Shown += OnShowFeature;
        FeatureEvent.Instance.Hidden += OnHideFeature;
        _chunkLoader.InitChunkNodes();
        GD.Print($"InitChunkNodes cost: {Time.GetTicksMsec() - time} ms");
    }
}