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
    }

    #endregion

    // 值可能为 null，为 null 时说明分块需要初始化
    private readonly Dictionary<int, HexGridChunk> _gridChunks = new();

    // 表示当前可视分块 Set 的 _insightChunkIds 索引
    private int _insightSetIdx;
    private readonly HashSet<int>[] _insightChunkIds = [[], []];
    private readonly Queue<int> _chunkQueryQueue = [];
    private readonly HashSet<int> _visitedChunkIds = [];
    private readonly HashSet<int> _rimChunkIds = [];
    private int _camNearestChunkId;

    private bool _ready;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
            SignalBus.Instance.CameraMoved += (pos, _) => UpdateInsightChunk(pos);
        _ready = true;
    }

    public override void _ExitTree() => CleanEventListeners();

    private void UpdateInsightChunk(Vector3 pos)
    {
        var camera = GetViewport().GetCamera3D();
        var nearestChunkId = _chunkService.SearchNearest(pos).Id;
        if (nearestChunkId == _camNearestChunkId)
            return;
        _camNearestChunkId = nearestChunkId;
        // GD.Print($"UpdateInsightChunk 当前相机位置：{pos}, Z 方向: {camera.GlobalBasis.Z}");
        var nextIdx = _insightSetIdx == 0 ? 1 : 0;
        // 隐藏边缘分块
        foreach (var chunkId in _rimChunkIds)
            _gridChunks[chunkId].Hide();
        _rimChunkIds.Clear();

        foreach (var preInsightChunkId in _insightChunkIds[_insightSetIdx])
        {
            var preInsightChunk = _chunkService.GetById(preInsightChunkId);
            _visitedChunkIds.Add(preInsightChunkId);
            if (!IsChunkInsight(preInsightChunk, camera))
            {
                // 分块不在视野范围内，隐藏它
                _gridChunks[preInsightChunkId].Hide();
                continue;
            }

            _insightChunkIds[nextIdx].Add(preInsightChunkId);
            // 分块在视野内，他的邻居才比较可能是在视野内
            // 将之前不在但现在可能在视野范围内的 id 加入带查询队列
            SearchNeighbor(preInsightChunk, _insightChunkIds[_insightSetIdx]);
        }

        // 有种极端情况，就是新的视野范围内一个旧视野范围分块都没有！
        // 这时放开限制进行 BFS，直到找到第一个可见的分块
        // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
        if (_insightChunkIds[nextIdx].Count == 0)
        {
            foreach (var chunk in _insightChunkIds[_insightSetIdx]
                         .Select(id => _chunkService.GetById(id)))
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

        // BFS 查询那些原来不在视野范围内的分块
        while (_chunkQueryQueue.Count > 0)
        {
            var chunkId = _chunkQueryQueue.Dequeue();
            var chunk = _chunkService.GetById(chunkId);
            if (!IsChunkInsight(chunk, camera)) continue;
            _insightChunkIds[nextIdx].Add(chunkId);
            ShowChunk(chunkId);
            SearchNeighbor(chunk, _visitedChunkIds);
        }

        // 清理好各个数据结构，等下一次调用直接使用
        _chunkQueryQueue.Clear();
        _visitedChunkIds.Clear();
        _insightChunkIds[_insightSetIdx].Clear();
        _insightSetIdx = nextIdx;

        ShowOutRimChunks();

        return;

        void SearchNeighbor(Chunk chunk, HashSet<int> filterSet)
        {
            foreach (var neighbor in _chunkService.GetNeighbors(chunk))
            {
                if (filterSet.Contains(neighbor.Id)) continue;
                _chunkQueryQueue.Enqueue(neighbor.Id);
                _visitedChunkIds.Add(neighbor.Id);
            }
        }
    }

    // 显示的分块向外多生成一圈，防止缺失进入视野的边缘瓦片
    private void ShowOutRimChunks()
    {
        foreach (var neighbor in from chunkId in _insightChunkIds[_insightSetIdx]
                 select _chunkService.GetById(chunkId)
                 into chunk
                 from neighbor in _chunkService.GetNeighbors(chunk)
                 where !_insightChunkIds[_insightSetIdx].Contains(neighbor.Id)
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
            var hexGridChunk = InitHexGridChunk(chunkId);
            _gridChunks[chunkId] = hexGridChunk;
        }
        else
            _gridChunks[chunkId].Show();
    }

    public void ClearOldData()
    {
        _gridChunks.Clear();
        foreach (var child in GetChildren())
            child.QueueFree();
        _insightChunkIds[_insightSetIdx].Clear();
        _insightSetIdx = 0;
        _camNearestChunkId = 0;
    }

    public void InitChunkNodes()
    {
        var time = Time.GetTicksMsec();
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

            var hexGridChunk = InitHexGridChunk(id);
            _gridChunks.Add(id, hexGridChunk);
            _insightChunkIds[_insightSetIdx].Add(id);
        }

        ShowOutRimChunks();
        _camNearestChunkId = _chunkService.SearchNearest(camera.GlobalPosition).Id;
        GD.Print($"BuildMesh cost: {Time.GetTicksMsec() - time} ms");
    }

    private HexGridChunk InitHexGridChunk(int id)
    {
        var hexGridChunk = _gridChunkScene.Instantiate<HexGridChunk>();
        hexGridChunk.Name = $"HexGridChunk{id}";
        AddChild(hexGridChunk); // 必须先加入场景树，让 _Ready() 先于 Init() 执行
        hexGridChunk.Init(id);
        return hexGridChunk;
    }

    // 注意，判断是否在摄像机内，不是用 GetViewport().GetVisibleRect().HasPoint(camera.UnprojectPosition(chunk.Pos))
    // 因为后面要根据相机位置动态更新可见区域，这个仅仅是对应初始时的可见区域
    private bool IsChunkInsight(Chunk chunk, Camera3D camera) =>
        Mathf.Cos(chunk.Pos.Normalized().AngleTo(camera.GlobalPosition.Normalized()))
        > _planetSettingService.Radius / camera.GlobalPosition.Length()
        && camera.IsPositionInFrustum(chunk.Pos);
}