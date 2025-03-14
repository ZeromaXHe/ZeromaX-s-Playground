using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Dependency;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Service;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class ChunkManager : Node3D
{
    public ChunkManager() => InitServices();

    [Export] private PackedScene _gridChunkScene;
    [ExportSubgroup("特征场景")] [Export] private PackedScene[] _urbanScenes;
    [Export] private PackedScene[] _farmScenes;
    [Export] private PackedScene[] _plantScenes;
    [Export] private PackedScene _wallTowerScene;
    [Export] private PackedScene _bridgeScene;
    [Export] private PackedScene[] _specialScenes;
    [ExportSubgroup("特征预览")] [Export] private Material _urbanPreviewOverrideMaterial;
    [Export] private Material _plantPreviewOverrideMaterial;
    [Export] private Material _farmPreviewOverrideMaterial;

    #region services

    private IChunkService _chunkService;
    private ITileService _tileService;
    private ITileShaderService _tileShaderService;
    private IPlanetSettingService _planetSettingService;

    private void InitServices()
    {
        _chunkService = Context.GetBean<IChunkService>();
        _tileService = Context.GetBean<ITileService>();
        _tileShaderService = Context.GetBean<ITileShaderService>();
        _planetSettingService = Context.GetBean<IPlanetSettingService>();
        _tileShaderService.TileExplored += ExploreFeatures;
        _chunkService.RefreshChunk += OnChunkServiceRefreshChunk;
        _chunkService.RefreshChunkTileLabel += OnChunkServiceRefreshChunkTileLabel;
        if (!Engine.IsEditorHint())
        {
            EventBus.Instance.CameraTransformed += UpdateInsightChunks;
        }
    }

    private void ExploreFeatures(int tileId)
    {
        var chunkId = _tileService.GetById(tileId).ChunkId;
        // BUG: 动态加载的分块会显示未探索的特征
        _gridChunks[chunkId]?.ExploreFeatures(tileId);
    }

    private void OnChunkServiceRefreshChunk(int id)
    {
        // 现在地图生成器也会调用，这时候分块还没创建
        if (_ready && _gridChunks.TryGetValue(id, out var chunk))
            chunk?.Refresh();
    }

    private void OnChunkServiceRefreshChunkTileLabel(int chunkId, int tileId, string text) =>
        _gridChunks[chunkId]?.RefreshTileLabel(tileId, text);

    private void CleanEventListeners()
    {
        // 不小心忽视了事件的解绑，会在编辑器下"重载已保存场景"时出问题报错！
        // （比如地图生成器逻辑会发出分块刷新信号，这时候老的场景代码貌似还在内存里，
        // 它接到事件后处理时，字典里的 Chunk 场景都已经释放，不存在了所以报错）
        // （对于新的场景，新分块字典里没数据，没有问题）
        // ERROR: /root/godot/modules/mono/glue/GodotSharp/GodotSharp/Core/NativeInterop/ExceptionUtils.cs:113 - System.ObjectDisposedException: Cannot access a disposed object.
        // ERROR: Object name: 'ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.HexGridChunk'.
        // 【切记】所以这里需要在退出场景树时清理事件监听！！！
        _ready = false;
        _tileShaderService.TileExplored -= ExploreFeatures;
        _chunkService.RefreshChunk -= OnChunkServiceRefreshChunk;
        _chunkService.RefreshChunkTileLabel -= OnChunkServiceRefreshChunkTileLabel;
        EventBus.Instance.ShowFeature -= OnShowFeature;
        EventBus.Instance.HideFeature -= OnHideFeature;
        if (!Engine.IsEditorHint())
        {
            EventBus.Instance.CameraTransformed -= UpdateInsightChunks;
        }
    }

    #endregion

    #region on-ready 节点

    private Node3D _urbans;
    private Node3D _farms;
    private Node3D _plants;
    private Node3D _others;
    private Node3D _featurePreviews;
    private Node3D _chunks;

    private void InitOnReadyNodes()
    {
        _urbans = GetNode<Node3D>("%Urbans");
        _farms = GetNode<Node3D>("%Farms");
        _plants = GetNode<Node3D>("%Plants");
        _others = GetNode<Node3D>("%Others");
        _featurePreviews = GetNode<Node3D>("%FeaturePreviews");
        _chunks = GetNode<Node3D>("%Chunks");
    }

    #endregion

    #region 特征 MultiMesh

    private MultiMeshInstance3D[] _multiUrbans;
    private MultiMeshInstance3D[] _multiFarms;
    private MultiMeshInstance3D[] _multiPlants;
    private MultiMeshInstance3D _multiTowers;
    private MultiMeshInstance3D _multiBridges;
    private MultiMeshInstance3D[] _multiSpecials;

    private void InitMultiMeshInstances()
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

    private void ClearOldDataForFeatureMultiMeshes()
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
    }

    private MultiMesh GetMultiMesh(FeatureType type) => type switch
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

    private Material GetPreviewOverrideMaterial(FeatureType type) => type switch
    {
        // 城市（红色）
        FeatureType.UrbanHigh1 or FeatureType.UrbanHigh2 or FeatureType.UrbanMid1 or FeatureType.UrbanMid2
            or FeatureType.UrbanLow1 or FeatureType.UrbanLow2 or FeatureType.Tower or FeatureType.Bridge
            or FeatureType.Castle or FeatureType.Ziggurat => _urbanPreviewOverrideMaterial,
        // 农田（黄绿色）
        FeatureType.FarmHigh1 or FeatureType.FarmHigh2 or FeatureType.FarmMid1 or FeatureType.FarmMid2
            or FeatureType.FarmLow1 or FeatureType.FarmLow2 => _farmPreviewOverrideMaterial,
        // 植被（绿色）
        FeatureType.PlantHigh1 or FeatureType.PlantHigh2 or FeatureType.PlantMid1 or FeatureType.PlantMid2
            or FeatureType.PlantLow1 or FeatureType.PlantLow2
            or FeatureType.MegaFlora => _plantPreviewOverrideMaterial,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
    };

    #endregion

    #region 动态加载特征

    private readonly Dictionary<FeatureType, HashSet<int>> _hidingIds = new();

    private void InitHidingIds()
    {
        foreach (var type in System.Enum.GetValues<FeatureType>())
            _hidingIds[type] = [];
    }

    private int _previewCount;
    private readonly HashSet<int> _emptyPreviewIds = [];

    private void ClearOldDataForDynamicFeatures()
    {
        // 清空动态加载特征相关数据结构
        _previewCount = 0;
        _emptyPreviewIds.Clear();
        foreach (var (_, set) in _hidingIds)
            set.Clear();
        foreach (var child in _featurePreviews.GetChildren())
            child.QueueFree();
        ClearOldDataForFeatureMultiMeshes();
    }

    // 将特征缩小并放到球心，表示不可见
    private static readonly Transform3D HideTransform3D = Transform3D.Identity.Scaled(Vector3.One * 0.0001f);

    private void OnHideFeature(int id, FeatureType type, bool preview)
    {
        if (preview)
        {
            if (id >= _previewCount) return; // 说明更新过星球
            _featurePreviews.GetChild<MeshInstance3D>(id).Mesh = null;
            _emptyPreviewIds.Add(id);
            return;
        }

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

    private int OnShowFeature(Transform3D transform, FeatureType type, bool preview)
    {
        var mesh = GetMultiMesh(type);
        if (preview)
        {
            MeshInstance3D meshIns;
            int previewId;
            if (_emptyPreviewIds.Count == 0)
            {
                // 没有供复用的 MeshInstance3D，必须新建
                meshIns = new MeshInstance3D();
                _featurePreviews.AddChild(meshIns);
                previewId = _previewCount++;
            }
            else
            {
                // 复用已经存在的 MeshInstance3D
                previewId = _emptyPreviewIds.First();
                meshIns = _featurePreviews.GetChild<MeshInstance3D>(previewId);
                _emptyPreviewIds.Remove(previewId);
            }

            meshIns.Mesh = mesh.Mesh;
            meshIns.MaterialOverride = GetPreviewOverrideMaterial(type);
            meshIns.Transform = transform;
            return previewId;
        }

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

    #endregion

    #region 动态加载分块

    // 值可能为 null，为 null 时说明分块需要初始化
    private readonly Dictionary<int, HexGridChunk> _gridChunks = new();

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    private int _insightSetIdx;
    private readonly HashSet<int>[] _insightChunkIds = [[], []];
    private HashSet<int> InsightChunkIdsNow => _insightChunkIds[_insightSetIdx];
    private HashSet<int> InsightChunkIdsNext => _insightChunkIds[_insightSetIdx ^ 1];
    private readonly Queue<int> _chunkQueryQueue = [];
    private readonly HashSet<int> _visitedChunkIds = [];

    private readonly HashSet<int> _rimChunkIds = [];

    // 暂时简单用时间控制视野更新频率
    private float _insightUpdateTime;
    private const float InsightUpdateInterval = 0.2f;

    private void UpdateInHorizonSetNextIdx() => _insightSetIdx ^= 1;

    private void ClearOldDataForDynamicChunks()
    {
        // 清空动态加载分块相关数据结构
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        _rimChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        _insightSetIdx = 0;
        _insightUpdateTime = 0;
    }

    private void UpdateInsightChunks(float delta)
    {
        var camera = GetViewport().GetCamera3D();
        _insightUpdateTime += delta;
        if (_insightUpdateTime < InsightUpdateInterval)
            return;
        // 隐藏边缘分块
        foreach (var chunkId in _rimChunkIds)
            _gridChunks[chunkId].HideOutOfSight();
        _rimChunkIds.Clear();
        foreach (var preInHorizonChunkId in InsightChunkIdsNow)
        {
            var preInsightChunk = _chunkService.GetById(preInHorizonChunkId);
            _visitedChunkIds.Add(preInHorizonChunkId);
            if (!IsChunkInsight(preInsightChunk, camera))
            {
                // 分块不在地平线范围内，隐藏它
                _gridChunks[preInHorizonChunkId].HideOutOfSight();
                continue;
            }

            InsightChunkIdsNext.Add(preInHorizonChunkId);
            // 分块在地平线内，他的邻居才比较可能是在地平线内
            // 将之前不在但现在可能在地平线范围内的 id 加入带查询队列
            SearchNeighbor(preInsightChunk, InsightChunkIdsNow);
        }

        // 有种极端情况，就是新的地平线范围内一个旧地平线范围分块都没有！
        // 这时放开限制进行 BFS，直到找到第一个可见的分块
        // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
        if (InsightChunkIdsNext.Count == 0)
        {
            foreach (var chunk in InsightChunkIdsNow.Select(_chunkService.GetById))
                SearchNeighbor(chunk, _visitedChunkIds); // 搜索所有外缘邻居

            while (_chunkQueryQueue.Count > 0)
            {
                var chunkId = _chunkQueryQueue.Dequeue();
                var chunk = _chunkService.GetById(chunkId);
                if (IsChunkInsight(chunk, camera))
                {
                    // 找到第一个可见分块，重新入队，后面进行真正的处理
                    _chunkQueryQueue.Enqueue(chunkId);
                    break;
                }

                SearchNeighbor(chunk, _visitedChunkIds);
            }
        }

        // BFS 查询那些原来不在地平线范围内的分块
        while (_chunkQueryQueue.Count > 0)
        {
            var chunkId = _chunkQueryQueue.Dequeue();
            var chunk = _chunkService.GetById(chunkId);
            if (!IsChunkInsight(chunk, camera)) continue;
            InsightChunkIdsNext.Add(chunkId);
            ShowChunk(chunkId);
            SearchNeighbor(chunk, _visitedChunkIds);
        }

        // 清理好各个数据结构，等下一次调用直接使用
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        InsightChunkIdsNow.Clear();
        UpdateInHorizonSetNextIdx();
        // 显示外缘分块
        InitOutRimChunks();
    }

    private void SearchNeighbor(Chunk chunk, HashSet<int> filterSet)
    {
        foreach (var neighbor in _chunkService.GetNeighbors(chunk))
        {
            if (filterSet.Contains(neighbor.Id)) continue;
            _chunkQueryQueue.Enqueue(neighbor.Id);
            _visitedChunkIds.Add(neighbor.Id);
        }
    }

    #endregion

    private bool _ready;

    public override void _Ready()
    {
        InitOnReadyNodes();
        InitMultiMeshInstances();
        InitHidingIds();
        _ready = true;
    }

    public override void _ExitTree() => CleanEventListeners();

    // 显示的分块向外多生成一圈，防止缺失进入视野的边缘瓦片
    private void InitOutRimChunks()
    {
        foreach (var neighbor in from chunkId in InsightChunkIdsNow
                 select _chunkService.GetById(chunkId)
                 into chunk
                 from neighbor in _chunkService.GetNeighbors(chunk)
                 where !InsightChunkIdsNow.Contains(neighbor.Id)
                 select neighbor)
        {
            _rimChunkIds.Add(neighbor.Id);
            ShowChunk(neighbor.Id);
        }
    }

    private void ShowChunk(int chunkId)
    {
        // 第一次可见时，初始化
        if (_gridChunks[chunkId] == null)
        {
            var hexGridChunk = AddChildHexGridChunk(chunkId);
            _gridChunks[chunkId] = hexGridChunk;
        }

        _gridChunks[chunkId].ShowInSight();
    }

    public void ClearOldData()
    {
        // 清空分块
        EventBus.Instance.ShowFeature -= OnShowFeature;
        EventBus.Instance.HideFeature -= OnHideFeature;
        _gridChunks.Clear();
        foreach (var child in _chunks.GetChildren())
            child.QueueFree();
        ClearOldDataForDynamicChunks();
        ClearOldDataForDynamicFeatures();
    }

    public void InitChunkNodes()
    {
        var time = Time.GetTicksMsec();
        EventBus.Instance.ShowFeature += OnShowFeature;
        EventBus.Instance.HideFeature += OnHideFeature;
        var camera = GetViewport().GetCamera3D();
        foreach (var chunk in _chunkService.GetAll())
        {
            var id = chunk.Id;
            // 此时拿不到真正 focusBase 的位置，暂且用相机自己的代替
            if (!IsChunkInsight(chunk, camera))
            {
                _gridChunks.Add(id, null);
                continue;
            }

            var hexGridChunk = AddChildHexGridChunk(id);
            hexGridChunk.ShowInSight();
            _gridChunks.Add(id, hexGridChunk);
            InsightChunkIdsNow.Add(id);
        }

        InitOutRimChunks();
        GD.Print($"InitChunkNodes cost: {Time.GetTicksMsec() - time} ms");
    }

    private HexGridChunk AddChildHexGridChunk(int id)
    {
        var hexGridChunk = _gridChunkScene.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{id}";
        _chunks.AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        hexGridChunk.Init(id);
        return hexGridChunk;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，上面方法这个仅仅是对应初始时的可见区域
    private bool IsChunkInsight(Chunk chunk, Camera3D camera) =>
        Mathf.Cos(chunk.Pos.Normalized().AngleTo(camera.GlobalPosition.Normalized()))
        > _planetSettingService.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(chunk.Pos);
}