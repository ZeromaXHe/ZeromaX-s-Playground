using Commons.Utils;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Domains.Services.Abstractions.Events.Events;
using Domains.Services.Abstractions.Nodes.IdInstances;
using Godot;
using Infras.Readers.Abstractions.Caches;
using Infras.Readers.Abstractions.Nodes.IdInstances;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Readers.Abstractions.Nodes.Singletons.ChunkManagers;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace Domains.Services.Nodes.IdInstances;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-22 17:37:00
public class HexGridChunkService(
    IHexGridChunkRepo hexGridChunkRepo,
    IHexPlanetHudRepo hexPlanetHudRepo,
    IHexPlanetManagerRepo hexPlanetManagerRepo,
    IChunkLoaderRepo chunkLoaderRepo,
    IChunkRepo chunkRepo,
    ITileRepo tileRepo,
    IPointRepo pointRepo,
    IFeatureRepo featureRepo,
    ILodMeshCache lodMeshCache)
    : IHexGridChunkService
{
    public void ShowChunk(int chunkId)
    {
        var chunk = chunkRepo.GetById(chunkId)!;
        if (hexGridChunkRepo.TryGetUsingChunk(chunk.Id, out var usingChunk))
            UpdateLod(usingChunk, chunk.Lod, false);
        else
        {
            // 没有空闲分块的话，初始化新的
            var hexGridChunk = hexGridChunkRepo.NoUnusedChunk()
                ? chunkLoaderRepo.Singleton!.InstantiateHexGridChunk()
                : hexGridChunkRepo.DequeUnusedChunk();
            UsedBy(hexGridChunk, chunk);
            hexGridChunkRepo.AddUsingChunk(chunk.Id, hexGridChunk);
        }
    }

    void UpdateLod(IHexGridChunk hexGridChunk, ChunkLod lod, bool idChanged = true)
    {
        if (lod == hexGridChunk.Lod && !idChanged) return;
        hexGridChunk.Lod = lod;

        var chunk = chunkRepo.GetById(hexGridChunk.Id)!; // 获取当前分块
        if (IsHandlingLodGaps(chunk))
        {
            // 对于需要处理接缝的情况，不使用缓存
            hexGridChunk.SetProcess(true);
            return;
        }

        var meshes = lodMeshCache.GetLodMeshes(lod, hexGridChunk.Id);
        // 如果之前生成过 Lod 网格，直接应用；否则重新生成
        if (meshes != null)
        {
            hexGridChunk.GetTerrain()!.ShowMesh(meshes[(int)MeshType.Terrain]);
            hexGridChunk.GetWater()!.ShowMesh(meshes[(int)MeshType.Water]);
            hexGridChunk.GetWaterShore()!.ShowMesh(meshes[(int)MeshType.WaterShore]);
            hexGridChunk.GetEstuary()!.ShowMesh(meshes[(int)MeshType.Estuary]);
        }
        else hexGridChunk.SetProcess(true);
    }

    public bool IsHandlingLodGaps(Chunk chunk) =>
        (chunk.Lod == ChunkLod.PlaneHex && chunkRepo.GetNeighbors(chunk).Any(n => n.Lod >= ChunkLod.SimpleHex))
        || (chunk.Lod == ChunkLod.TerracesHex && chunkRepo.GetNeighbors(chunk).Any(n => n.Lod == ChunkLod.Full));

    private void UsedBy(IHexGridChunk hexGridChunk, Chunk chunk)
    {
        InitLabels(chunk.Id);
        hexGridChunk.Id = chunk.Id;
        // 默认不生成网格，而是先查缓存
        hexGridChunk.SetProcess(false);
        ShowInSight(chunk.Lod);
        return;

        void InitLabels(int id)
        {
            // 隐藏之前的标签
            foreach (var (tileId, _) in hexGridChunk.UsingTileUis)
                HideLabel(tileId);

            var tileIds = chunkRepo.GetById(id)!.TileIds;
            foreach (var tileId in tileIds)
                ShowLabel(tileId);
            // 在场景树中 _Ready 后 Label 才非 null
            RefreshTilesLabelMode(hexGridChunk, hexPlanetHudRepo.GetEditMode() ? hexPlanetHudRepo.GetLabelMode() : 0);
            return;

            void HideLabel(int tileId)
            {
                var label = hexGridChunk.UsingTileUis[tileId];
                label.Hide();
                hexGridChunk.UsingTileUis.Remove(tileId);
                hexGridChunk.UnusedTileUis.Enqueue(label);
            }

            void ShowLabel(int tileId)
            {
                var label = hexGridChunk.UnusedTileUis.Count == 0
                    ? hexGridChunk.InstantiateHexTileLabel()
                    : hexGridChunk.UnusedTileUis.Dequeue();
                var tile = tileRepo.GetById(tileId)!;
                var position = 1.01f * tile.GetCentroid(
                    hexPlanetManagerRepo.Radius + hexPlanetManagerRepo.GetHeight(tile));
                var scale = hexPlanetManagerRepo.StandardScale;
                label.Scale = Vector3.One * scale;
                label.Position = position;
                Node3dUtil.AlignYAxisToDirection(label, position, Vector3.Up);
                hexGridChunk.UsingTileUis.Add(tile.Id, label);
            }
        }

        void ShowInSight(ChunkLod lod)
        {
            hexGridChunk.Show();
            UpdateLod(hexGridChunk, lod);
            // 编辑模式下全部显示，游戏模式下仅显示探索过的
            foreach (var tile in chunkRepo.GetById(hexGridChunk.Id)!.TileIds.Select(tileRepo.GetById))
                ShowFeatures(tile!, !hexPlanetHudRepo.GetEditMode(), false);
            OnEditorEditModeChanged(hexGridChunk, hexPlanetHudRepo.GetEditMode());
        }
    }

    public void ShowFeatures(Tile tile, bool onlyExplored, bool preview)
    {
        foreach (var feature in featureRepo.GetByTileId(tile.Id)
                     .Where(f => f.MeshId == -1 && (!onlyExplored || tile.Data.IsExplored) && f.Preview == preview))
        {
            feature.MeshId = preview
                ? FeatureEvent.EmitPreviewShown(feature.Transform, feature.Type)
                : FeatureEvent.EmitMeshShown(feature.Transform, feature.Type);
        }
    }

    public void HideFeatures(Tile tile, bool preview)
    {
        foreach (var feature in featureRepo.GetByTileId(tile.Id)
                     .Where(f => f.MeshId > -1 && f.Preview == preview))
        {
            if (preview)
                FeatureEvent.EmitPreviewHidden(feature.MeshId, feature.Type);
            else
                FeatureEvent.EmitMeshHidden(feature.MeshId, feature.Type);
            feature.MeshId = -1;
        }
    }

    public void ClearFeatures(Tile tile, bool preview)
    {
        HideFeatures(tile, preview);
        featureRepo.DeleteByTileId(tile.Id);
    }

    public void ExploreFeatures(Tile tile) => ShowFeatures(tile, true, false);

    public void RefreshChunk(int id)
    {
        // 现在地图生成器也会调用，这时候分块还没创建。
        // _ready 不可或缺，否则启动失败
        if ( /*_ready && */hexGridChunkRepo.TryGetUsingChunk(id, out var chunk))
        {
            // 让所有旧的网格缓存过期
            lodMeshCache.RemoveLodMeshes(chunk.Id);
            chunk.SetProcess(true);
        }
    }

    public void OnEditorEditModeChanged(bool mode)
    {
        foreach (var hexGridChunk in hexGridChunkRepo.GetAllUsingChunk())
            OnEditorEditModeChanged(hexGridChunk, mode);
    }

    private void OnEditorEditModeChanged(IHexGridChunk hexGridChunk, bool mode) =>
        RefreshTilesLabelMode(hexGridChunk, mode ? hexPlanetHudRepo.GetLabelMode() : 0);

    public void RefreshTilesLabelMode(int mode)
    {
        foreach (var hexGridChunk in hexGridChunkRepo.GetAllUsingChunk())
            RefreshTilesLabelMode(hexGridChunk, mode);
    }

    private void RefreshTilesLabelMode(IHexGridChunk hexGridChunk, int mode)
    {
        switch (mode)
        {
            case 0:
                // 不显示
                foreach (var (_, label) in hexGridChunk.UsingTileUis)
                {
                    label.Label!.Text = "";
                    label.Label.FontSize = 64;
                    label.Hide();
                }

                break;
            case 1:
                // 坐标
                foreach (var (tileId, label) in hexGridChunk.UsingTileUis)
                {
                    var coords = pointRepo.GetSphereAxial(tileRepo.GetById(tileId)!);
                    label.Label!.Text = $"{coords.Coords}\n{coords.Type},{coords.TypeIdx}";
                    label.Label.FontSize = 24;
                    label.Show();
                }

                break;
            case 2:
                // ID
                foreach (var (tileId, label) in hexGridChunk.UsingTileUis)
                {
                    label.Label!.Text = tileId.ToString();
                    label.Label.FontSize = 64;
                    label.Show();
                }

                break;
        }
    }

    // 将之前显示的分块隐藏掉（归还到分块池中）
    public void HideChunk(int chunkId)
    {
        if (!hexGridChunkRepo.TryGetUsingChunk(chunkId, out var usingChunk)) return;
        HideOutOfSight(usingChunk);
        hexGridChunkRepo.RemoveUsingChunk(chunkId);
        hexGridChunkRepo.EnqueueUnusedChunks(usingChunk);
    }

    private void HideOutOfSight(IHexGridChunk hexGridChunk)
    {
        hexGridChunk.Hide();
        hexGridChunk.GetTerrain()!.Clear();
        hexGridChunk.GetRivers()!.Clear();
        hexGridChunk.GetRoads()!.Clear();
        hexGridChunk.GetWater()!.Clear();
        hexGridChunk.GetWaterShore()!.Clear();
        hexGridChunk.GetEstuary()!.Clear();
        hexGridChunk.GetFeatures()!.Clear(false, HideFeatures);
        hexGridChunk.Id = 0; // 重置 id，归还给池子
    }
}