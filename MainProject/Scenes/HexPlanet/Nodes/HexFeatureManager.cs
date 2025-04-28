using System;
using System.Collections.Generic;
using Commons.Constants;
using Commons.Enums;
using Commons.Utils;
using Contexts;
using Domains.Models.Entities.PlanetGenerates;
using Domains.Models.ValueObjects.PlanetGenerates;
using Godot;
using GodotNodes.Abstractions.Addition;
using Infras.Readers.Abstractions.Nodes.Singletons;
using Infras.Writers.Abstractions.PlanetGenerates;
using Nodes.Abstractions;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Nodes;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-25 23:58
[Tool]
public partial class HexFeatureManager : Node3D, IHexFeatureManager
{
    public HexFeatureManager() => InitServices();
    public NodeEvent? NodeEvent => null;

    [Export] private HexMesh? _walls;
    [Export] private bool _preview;

    #region 服务

    // TODO: 节点层不应该有直接调用服务或仓储，应该放到上下文层内依赖注入
    private static IHexPlanetManagerRepo? _hexPlanetManagerRepo;
    private static IFeatureRepo? _featureRepo;

    private void InitServices()
    {
        _hexPlanetManagerRepo ??= Context.GetBeanFromHolder<IHexPlanetManagerRepo>();
        _featureRepo ??= Context.GetBeanFromHolder<IFeatureRepo>();
    }

    #endregion

    private static float GetHeight(FeatureType type) =>
        type switch
        {
            // 城市
            FeatureType.UrbanHigh1 => 2.5f,
            FeatureType.UrbanHigh2 => 1.5f,
            FeatureType.UrbanMid1 => 1f,
            FeatureType.UrbanMid2 => 0.75f,
            FeatureType.UrbanLow1 => 0.5f,
            FeatureType.UrbanLow2 => 0.5f,

            // 农田
            FeatureType.FarmHigh1 or FeatureType.FarmHigh2 or FeatureType.FarmMid1
                or FeatureType.FarmMid2 or FeatureType.FarmLow1 or FeatureType.FarmLow2 => 0.05f,

            // 植被
            FeatureType.PlantHigh1 => 2.25f,
            FeatureType.PlantHigh2 => 1.5f,
            FeatureType.PlantMid1 => 1.5f,
            FeatureType.PlantMid2 => 0.75f,
            FeatureType.PlantLow1 => 1f,
            FeatureType.PlantLow2 => 0.5f,

            // 特殊
            FeatureType.Tower => 2f, // 塔高 4f
            FeatureType.Bridge => 0.7f, // 0.7f 是桥梁需要略微抬高一点
            FeatureType.Castle => 2f,
            FeatureType.Ziggurat => 1.25f,
            FeatureType.MegaFlora => 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "new type no deal")
        };

    // 保存所有的显示中的地块 id
    private readonly List<Tile> _tiles = [];

    private void SaveTileFeatureInfo(FeatureType featureType, Transform3D transform, Tile tile)
    {
        _featureRepo!.Add(featureType, transform, tile.Id, _preview);
        _tiles.Add(tile);
    }

    public void Clear(bool preview, Action<Tile, bool> clearOrHideFeatures)
    {
        foreach (var tile in _tiles)
            clearOrHideFeatures(tile, preview);
        _tiles.Clear();
        _walls!.Clear();
    }

    public void Apply() => _walls!.Apply();

    private void AddTower(Tile tile, Vector3 left, Vector3 right)
    {
        var position = (left + right) * 0.5f;
        var transform = Math3dUtil.PlaceOnSphere(Basis.Identity, position,
            Vector3.One * _hexPlanetManagerRepo!.StandardScale, GetHeight(FeatureType.Tower));
        var rightDirection = right - left;
        transform = transform.Rotated(position.Normalized(),
            transform.Basis.X.SignedAngleTo(rightDirection, position.Normalized()));
        SaveTileFeatureInfo(FeatureType.Tower, transform, tile);
        // 不能采用 instance shader uniform 的解决方案，编辑器里会大量报错：
        // ERROR: servers/rendering/renderer_rd/storage_rd/material_storage.cpp:1791 - Condition "global_shader_uniforms.instance_buffer_pos.has(p_instance)" is true. Returning: -1
        // ERROR: Too many instances using shader instance variables. Increase buffer size in Project Settings.
        // tower.SetInstanceShaderParameter("tile_id", tile.Id);
    }

    public void AddBridge(Tile tile, Vector3 roadCenter1, Vector3 roadCenter2)
    {
        roadCenter1 = _hexPlanetManagerRepo!.Perturb(roadCenter1);
        roadCenter2 = _hexPlanetManagerRepo.Perturb(roadCenter2);
        var position = (roadCenter1 + roadCenter2) * 0.5f;
        var length = roadCenter1.DistanceTo(roadCenter2);
        var scale = _hexPlanetManagerRepo.StandardScale;
        // 缩放需要沿着桥梁方向拉伸长度（X 轴）
        var transform = Math3dUtil.PlaceOnSphere(Basis.Identity, position,
            new Vector3(length / HexMetrics.BridgeDesignLength, scale, scale), GetHeight(FeatureType.Bridge));
        transform = transform.Rotated(position.Normalized(),
            transform.Basis.X.SignedAngleTo(roadCenter2 - roadCenter1, position.Normalized()));
        SaveTileFeatureInfo(FeatureType.Bridge, transform, tile);
    }

    public void AddSpecialFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider)
    {
        var specialType = overrider.SpecialIndex(tile) switch
        {
            1 => FeatureType.Castle,
            2 => FeatureType.Ziggurat,
            3 => FeatureType.MegaFlora,
            _ => throw new Exception($"Special feature index {overrider.SpecialIndex(tile)} is invalid")
        };
        position = _hexPlanetManagerRepo!.Perturb(position);
        var transform = Math3dUtil.PlaceOnSphere(Basis.Identity, position,
            Vector3.One * _hexPlanetManagerRepo.StandardScale, GetHeight(specialType));
        var hash = _hexPlanetManagerRepo.SampleHashGrid(position);
        transform = transform.Rotated(position.Normalized(), hash.E * Mathf.Tau); // 入参 axis 还是得用全局坐标
        SaveTileFeatureInfo(specialType, transform, tile);
    }

    public void AddFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider)
    {
        if (overrider.IsSpecial(tile)) return;
        var hash = _hexPlanetManagerRepo!.SampleHashGrid(position);
        var type = PickFeatureSizeType(FeatureType.UrbanHigh1, overrider.UrbanLevel(tile), hash.A, hash.D);
        var othertype = PickFeatureSizeType(FeatureType.FarmHigh1, overrider.FarmLevel(tile), hash.B, hash.D);
        var usedHash = hash.A;
        if (type != null)
        {
            if (othertype != null && hash.B < hash.A)
            {
                type = othertype;
                usedHash = hash.B;
            }
        }
        else if (othertype != null)
        {
            type = othertype;
            usedHash = hash.B;
        }

        othertype = PickFeatureSizeType(FeatureType.PlantHigh1, overrider.PlantLevel(tile), hash.C, hash.D);
        if (type != null)
        {
            if (othertype != null && hash.C < usedHash)
                type = othertype;
        }
        else if (othertype != null)
            type = othertype;
        else return;

        var featureType = (FeatureType)type;
        position = _hexPlanetManagerRepo.Perturb(position);
        var transform = Math3dUtil.PlaceOnSphere(Basis.Identity, position,
            Vector3.One * _hexPlanetManagerRepo.StandardScale, GetHeight(featureType));
        transform = transform.Rotated(position.Normalized(), hash.E * Mathf.Tau); // 入参 axis 还是得用全局坐标
        SaveTileFeatureInfo(featureType, transform, tile);
    }

    private static FeatureType? PickFeatureSizeType(FeatureType baseType, int level, float hash, float choice)
    {
        if (level <= 0) return null;
        var thresholds = _hexPlanetManagerRepo!.GetFeatureThreshold(level - 1);
        for (var i = 0; i < thresholds.Length; i++)
            if (hash < thresholds[i])
                return (FeatureType)(int)baseType + i * 2 + (int)(choice * 2);
        return null;
    }

    public void AddWall(EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad, HexTileDataOverrider overrider, ChunkLod lod)
    {
        if (overrider.Walled(nearTile) == overrider.Walled(farTile)
            || overrider.IsUnderwater(nearTile) || overrider.IsUnderwater(farTile)
            || overrider.GetEdgeType(nearTile, farTile) == HexEdgeType.Cliff)
            return;
        if (lod < ChunkLod.Full && !hasRiver && !hasRoad)
        {
            AddWallSegment(nearTile, farTile, near.V1, far.V1, near.V5, far.V5, lod);
            return;
        }

        AddWallSegment(nearTile, farTile, near.V1, far.V1, near.V2, far.V2, lod);
        if (hasRiver || hasRoad)
        {
            AddWallCap(nearTile, near.V2, farTile, far.V2);
            AddWallCap(farTile, far.V4, nearTile, near.V4);
        }
        else
        {
            AddWallSegment(nearTile, farTile, near.V2, far.V2, near.V3, far.V3, lod);
            AddWallSegment(nearTile, farTile, near.V3, far.V3, near.V4, far.V4, lod);
        }

        AddWallSegment(nearTile, farTile, near.V4, far.V4, near.V5, far.V5, lod);
    }

    public void AddWall(Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3,
        Tile tile3, HexTileDataOverrider overrider, ChunkLod lod)
    {
        if (overrider.Walled(tile1))
        {
            if (overrider.Walled(tile2))
            {
                if (!overrider.Walled(tile3))
                    AddWallSegment(c3, tile3, c1, tile1, c2, tile2, overrider, lod);
            }
            else if (overrider.Walled(tile3))
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1, overrider, lod);
            else
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3, overrider, lod);
        }
        else if (overrider.Walled(tile2))
            if (overrider.Walled(tile3))
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3, overrider, lod);
            else
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1, overrider, lod);
        else if (overrider.Walled(tile3))
            AddWallSegment(c3, tile3, c1, tile1, c2, tile2, overrider, lod);
    }

    private void AddWallSegment(Tile nearTile, Tile farTile, Vector3 nearLeft, Vector3 farLeft,
        Vector3 nearRight, Vector3 farRight, ChunkLod lod, bool addTower = false)
    {
        nearLeft = _hexPlanetManagerRepo!.Perturb(nearLeft);
        farLeft = _hexPlanetManagerRepo.Perturb(farLeft);
        nearRight = _hexPlanetManagerRepo.Perturb(nearRight);
        farRight = _hexPlanetManagerRepo.Perturb(farRight);
        var height = _hexPlanetManagerRepo.GetWallHeight();
        var thickness = _hexPlanetManagerRepo.GetWallThickness();
        var left = _hexPlanetManagerRepo.WallLerp(nearLeft, farLeft);
        var right = _hexPlanetManagerRepo.WallLerp(nearRight, farRight);
        var leftTop = left.Length() + height;
        var rightTop = right.Length() + height;
        var ids = new Vector3(nearTile.Id, farTile.Id, nearTile.Id);
        Vector3 v1, v2, v3, v4;
        v1 = v3 = _hexPlanetManagerRepo.WallThicknessOffset(nearLeft, farLeft, true, thickness);
        v2 = v4 = _hexPlanetManagerRepo.WallThicknessOffset(nearRight, farRight, true, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        _walls!.AddQuadUnperturbed([v1, v2, v3, v4], HexMeshConstant.QuadArr(HexMeshConstant.Weights1), tis: ids);
        Vector3 t1 = v3, t2 = v4;
        v1 = v3 = _hexPlanetManagerRepo.WallThicknessOffset(nearLeft, farLeft, false, thickness);
        v2 = v4 = _hexPlanetManagerRepo.WallThicknessOffset(nearRight, farRight, false, thickness);
        if (lod == ChunkLod.Full)
        {
            v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
            v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
            _walls.AddQuadUnperturbed([v2, v1, v4, v3], HexMeshConstant.QuadArr(HexMeshConstant.Weights2), tis: ids);
            _walls.AddQuadUnperturbed([t1, t2, v3, v4],
                HexMeshConstant.QuadArr(HexMeshConstant.Weights1, HexMeshConstant.Weights2), tis: ids);
        }
        else
            _walls.AddQuadUnperturbed([v2, v1, t2, t1], HexMeshConstant.QuadArr(HexMeshConstant.Weights2), tis: ids);

        if (addTower)
            AddTower(nearTile, left, right);
    }

    // pivot 有墙，left\right 没有墙的情况
    private void AddWallSegment(Vector3 pivot, Tile pivotTile, Vector3 left, Tile leftTile,
        Vector3 right, Tile rightTile, HexTileDataOverrider overrider, ChunkLod lod)
    {
        if (overrider.IsUnderwater(pivotTile)) return;
        var hasLeftWall = !overrider.IsUnderwater(leftTile)
                          && overrider.GetEdgeType(pivotTile, leftTile) != HexEdgeType.Cliff;
        var hasRightWall = !overrider.IsUnderwater(rightTile)
                           && overrider.GetEdgeType(pivotTile, rightTile) != HexEdgeType.Cliff;
        if (hasLeftWall)
        {
            if (hasRightWall)
            {
                var hasTower = false;
                if (overrider.Elevation(leftTile) == overrider.Elevation(rightTile))
                {
                    var hash = _hexPlanetManagerRepo!.SampleHashGrid((pivot + left + right) / 3f);
                    hasTower = hash.E < HexMetrics.WallTowerThreshold;
                }

                // 这里入参还得观察一下会不会 bug
                AddWallSegment(pivotTile, leftTile, pivot, left, pivot, right, lod, hasTower);
            }
            else if (overrider.Elevation(leftTile) < overrider.Elevation(rightTile))
                AddWallWedge(pivotTile, pivot, leftTile, left, rightTile, right);
            else
                AddWallCap(pivotTile, pivot, leftTile, left);
        }
        else if (hasRightWall)
        {
            if (overrider.Elevation(rightTile) < overrider.Elevation(leftTile))
                AddWallWedge(rightTile, right, pivotTile, pivot, leftTile, left);
            else
                AddWallCap(rightTile, right, pivotTile, pivot);
        }
    }

    private void AddWallCap(Tile nearTile, Vector3 near, Tile farTile, Vector3 far)
    {
        near = _hexPlanetManagerRepo!.Perturb(near);
        far = _hexPlanetManagerRepo.Perturb(far);
        var center = _hexPlanetManagerRepo.WallLerp(near, far);
        var thickness = _hexPlanetManagerRepo.GetWallThickness();
        var height = _hexPlanetManagerRepo.GetWallHeight();
        var centerTop = center.Length() + height;
        Vector3 v1, v2, v3, v4;
        v1 = v3 = _hexPlanetManagerRepo.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = _hexPlanetManagerRepo.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        _walls!.AddQuadUnperturbed([v1, v2, v3, v4],
            [HexMeshConstant.Weights1, HexMeshConstant.Weights2, HexMeshConstant.Weights1, HexMeshConstant.Weights2],
            tis: new Vector3(nearTile.Id, farTile.Id, nearTile.Id));
    }

    private void AddWallWedge(Tile nearTile, Vector3 near, Tile farTile, Vector3 far, Tile pointTile, Vector3 point)
    {
        near = _hexPlanetManagerRepo!.Perturb(near);
        far = _hexPlanetManagerRepo.Perturb(far);
        point = _hexPlanetManagerRepo.Perturb(point);
        var center = _hexPlanetManagerRepo.WallLerp(near, far);
        var thickness = _hexPlanetManagerRepo.GetWallThickness();
        var height = _hexPlanetManagerRepo.GetWallHeight();
        var centerTop = center.Length() + height;
        point = Math3dUtil.ProjectToSphere(point, center.Length());
        var pointTop = Math3dUtil.ProjectToSphere(point, centerTop);
        Vector3 v1, v2, v3, v4;
        v1 = v3 = _hexPlanetManagerRepo.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = _hexPlanetManagerRepo.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        var ids = new Vector3(nearTile.Id, farTile.Id, pointTile.Id);
        _walls!.AddQuadUnperturbed([v1, point, v3, pointTop],
            [HexMeshConstant.Weights1, HexMeshConstant.Weights3, HexMeshConstant.Weights1, HexMeshConstant.Weights3],
            tis: ids);
        _walls.AddQuadUnperturbed([point, v2, pointTop, v4],
            [HexMeshConstant.Weights2, HexMeshConstant.Weights3, HexMeshConstant.Weights2, HexMeshConstant.Weights3],
            tis: ids);
        _walls.AddTriangleUnperturbed([pointTop, v3, v4],
            [HexMeshConstant.Weights3, HexMeshConstant.Weights1, HexMeshConstant.Weights2], tis: ids);
    }
}