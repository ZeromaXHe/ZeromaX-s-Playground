using System.Linq;
using Godot;
using Godot.Collections;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Struct;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexFeatureManager : Node3D
{
    /// 特征场景
    /// 这种嵌套的数组不能用 .NET 的 array，必须用 Godot.Collections.Array。否则编辑器编译不通过
    /// 而且这样写以后，其实编辑器里是判断不了 PackedScene 类型的，必须自己手动选成 Object（Godot.GodotObject 或其他派生类型）再用。
    [Export] private Array<Array<PackedScene>> _urbanScenes;

    [Export] private Array<Array<PackedScene>> _farmScenes;
    [Export] private Array<Array<PackedScene>> _plantScenes;
    [Export] private HexMesh _walls;
    [Export] private PackedScene _wallTowerScene;
    [Export] private PackedScene _bridgeScene;
    [Export] private PackedScene[] _specialScenes;
    [Export] private Material _overrideMaterial;

    private Node3D _container;

    private readonly System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Node3D>>
        _unexploredContainer = new();

    public void ExploreFeatures(int tileId)
    {
        if (!_unexploredContainer.TryGetValue(tileId, out var list) || list is not { Count: > 0 }) return;
        foreach (var unexplored in list)
            unexplored.Visible = true;
        _unexploredContainer.Remove(tileId);
    }

    public void ShowUnexploredFeatures(bool show)
    {
        if (_unexploredContainer.Count <= 0) return;
        foreach (var feature in _unexploredContainer.Values.SelectMany(list => list))
            feature.Visible = show;
    }

    public void Clear()
    {
        _container?.QueueFree();
        _container = new Node3D();
        AddChild(_container);
        _walls.Clear();
    }

    public void Apply() => _walls.Apply();

    public void AddTower(Tile tile, Vector3 left, Vector3 right)
    {
        var tower = _wallTowerScene.Instantiate<CsgBox3D>();
        var position = (left + right) * 0.5f;
        Node3dUtil.PlaceOnSphere(tower, position, 0.5f * 1.25f * tower.Size.Y);
        var rightDirection = right - left;
        tower.Rotate(position.Normalized(),
            tower.Basis.X.SignedAngleTo(rightDirection, position.Normalized()));

        if (_overrideMaterial != null)
            tower.SetMaterial(_overrideMaterial);
        _container.AddChild(tower);
        if (tile.Data.IsExplored) return;
        if (_unexploredContainer.TryGetValue(tile.Id, out var list))
            list.Add(tower);
        else
            _unexploredContainer.Add(tile.Id, [tower]);
        // 不能采用 instance shader uniform 的解决方案，编辑器里会大量报错：
        // ERROR: servers/rendering/renderer_rd/storage_rd/material_storage.cpp:1791 - Condition "global_shader_uniforms.instance_buffer_pos.has(p_instance)" is true. Returning: -1
        // ERROR: Too many instances using shader instance variables. Increase buffer size in Project Settings.
        // tower.SetInstanceShaderParameter("tile_id", tile.Id);
    }

    public void AddBridge(Tile tile, Vector3 roadCenter1, Vector3 roadCenter2)
    {
        roadCenter1 = HexMetrics.Perturb(roadCenter1);
        roadCenter2 = HexMetrics.Perturb(roadCenter2);
        var bridge = _bridgeScene.Instantiate<CsgBox3D>();
        var position = (roadCenter1 + roadCenter2) * 0.5f;
        var length = roadCenter1.DistanceTo(roadCenter2);
        var scale = HexMetrics.StandardScale;
        bridge.Scale = new Vector3(length / HexMetrics.BridgeDesignLength, scale, scale); // 沿着桥梁方向拉伸长度（X 轴）
        bridge.Position =
            position.Normalized() * (position.Length() + 0.7f * scale * bridge.Size.Y); // 0.7f 是桥梁需要略微抬高一点
        Node3dUtil.AlignYAxisToDirection(bridge, position);
        bridge.Rotate(position.Normalized(),
            bridge.Basis.X.SignedAngleTo(roadCenter2 - roadCenter1, position.Normalized()));

        if (_overrideMaterial != null)
            bridge.SetMaterial(_overrideMaterial);
        _container.AddChild(bridge);
        if (tile.Data.IsExplored) return;
        if (_unexploredContainer.TryGetValue(tile.Id, out var list))
            list.Add(bridge);
        else
            _unexploredContainer.Add(tile.Id, [bridge]);
    }

    public void AddSpecialFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider)
    {
        var instance = _specialScenes[overrider.SpecialIndex(tile) - 1].Instantiate<CsgBox3D>();
        position = HexMetrics.Perturb(position);
        Node3dUtil.PlaceOnSphere(instance, position, 0.5f * instance.Size.Y);
        var hash = HexMetrics.SampleHashGrid(position);
        instance.Rotate(position.Normalized(), hash.E * Mathf.Tau); // 入参 axis 还是得用全局坐标

        if (_overrideMaterial != null)
            instance.SetMaterial(_overrideMaterial);
        _container.AddChild(instance);
        if (tile.Data.IsExplored) return;
        if (_unexploredContainer.TryGetValue(tile.Id, out var list))
            list.Add(instance);
        else
            _unexploredContainer.Add(tile.Id, [instance]);
    }

    public void AddFeature(Tile tile, Vector3 position, HexTileDataOverrider overrider)
    {
        if (overrider.IsSpecial(tile)) return;
        var hash = HexMetrics.SampleHashGrid(position);
        var scene = PickScene(_urbanScenes, overrider.UrbanLevel(tile), hash.A, hash.D);
        var otherScene = PickScene(_farmScenes, overrider.FarmLevel(tile), hash.B, hash.D);
        var usedHash = hash.A;
        if (scene != null)
        {
            if (otherScene != null && hash.B < hash.A)
            {
                scene = otherScene;
                usedHash = hash.B;
            }
        }
        else if (otherScene != null)
        {
            scene = otherScene;
            usedHash = hash.B;
        }

        otherScene = PickScene(_plantScenes, overrider.PlantLevel(tile), hash.C, hash.D);
        if (scene != null)
        {
            if (otherScene != null && hash.C < usedHash)
                scene = otherScene;
        }
        else if (otherScene != null)
            scene = otherScene;
        else return;

        var instance = scene.Instantiate<CsgBox3D>();
        position = HexMetrics.Perturb(position);
        Node3dUtil.PlaceOnSphere(instance, position, 0.5f * instance.Size.Y);
        instance.Rotate(position.Normalized(), hash.E * Mathf.Tau); // 入参 axis 还是得用全局坐标

        if (_overrideMaterial != null)
            instance.SetMaterial(_overrideMaterial);
        _container.AddChild(instance);
        if (tile.Data.IsExplored) return;
        if (_unexploredContainer.TryGetValue(tile.Id, out var list))
            list.Add(instance);
        else
            _unexploredContainer.Add(tile.Id, [instance]);
    }

    private static PackedScene PickScene(Array<Array<PackedScene>> scenes, int level, float hash, float choice)
    {
        if (level <= 0) return null;
        var thresholds = HexMetrics.GetFeatureThreshold(level - 1);
        for (var i = 0; i < thresholds.Length; i++)
            if (hash < thresholds[i])
                return scenes[i][(int)(choice * scenes[i].Count)];
        return null;
    }

    public void AddWall(EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad, HexTileDataOverrider overrider)
    {
        if (overrider.Walled(nearTile) == overrider.Walled(farTile)
            || overrider.IsUnderwater(nearTile) || overrider.IsUnderwater(farTile)
            || overrider.GetEdgeType(nearTile, farTile) == HexEdgeType.Cliff)
            return;
        AddWallSegment(nearTile, farTile, near.V1, far.V1, near.V2, far.V2);
        if (hasRiver || hasRoad)
        {
            AddWallCap(nearTile, near.V2, farTile, far.V2);
            AddWallCap(farTile, far.V4, nearTile, near.V4);
        }
        else
        {
            AddWallSegment(nearTile, farTile, near.V2, far.V2, near.V3, far.V3);
            AddWallSegment(nearTile, farTile, near.V3, far.V3, near.V4, far.V4);
        }

        AddWallSegment(nearTile, farTile, near.V4, far.V4, near.V5, far.V5);
    }

    public void AddWall(Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3,
        Tile tile3, HexTileDataOverrider overrider)
    {
        if (overrider.Walled(tile1))
        {
            if (overrider.Walled(tile2))
            {
                if (!overrider.Walled(tile3))
                    AddWallSegment(c3, tile3, c1, tile1, c2, tile2, overrider);
            }
            else if (overrider.Walled(tile3))
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1, overrider);
            else
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3, overrider);
        }
        else if (overrider.Walled(tile2))
            if (overrider.Walled(tile3))
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3, overrider);
            else
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1, overrider);
        else if (overrider.Walled(tile3))
            AddWallSegment(c3, tile3, c1, tile1, c2, tile2, overrider);
    }

    private void AddWallSegment(Tile nearTile, Tile farTile, Vector3 nearLeft, Vector3 farLeft,
        Vector3 nearRight, Vector3 farRight, bool addTower = false)
    {
        nearLeft = HexMetrics.Perturb(nearLeft);
        farLeft = HexMetrics.Perturb(farLeft);
        nearRight = HexMetrics.Perturb(nearRight);
        farRight = HexMetrics.Perturb(farRight);
        var height = HexMetrics.GetWallHeight();
        var thickness = HexMetrics.GetWallThickness();
        var left = HexMetrics.WallLerp(nearLeft, farLeft);
        var right = HexMetrics.WallLerp(nearRight, farRight);
        var leftTop = left.Length() + height;
        var rightTop = right.Length() + height;
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(nearLeft, farLeft, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(nearRight, farRight, true, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        var ids = new Vector3(nearTile.Id, farTile.Id, nearTile.Id);
        _walls.AddQuadUnperturbed([v1, v2, v3, v4], HexMesh.QuadArr(HexMesh.Weights1), tis: ids);
        Vector3 t1 = v3, t2 = v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(nearLeft, farLeft, false, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(nearRight, farRight, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        _walls.AddQuadUnperturbed([v2, v1, v4, v3], HexMesh.QuadArr(HexMesh.Weights2), tis: ids);
        _walls.AddQuadUnperturbed([t1, t2, v3, v4], HexMesh.QuadArr(HexMesh.Weights1, HexMesh.Weights2), tis: ids);

        if (addTower)
            AddTower(nearTile, left, right);
    }

    // pivot 有墙，left\right 没有墙的情况
    private void AddWallSegment(Vector3 pivot, Tile pivotTile, Vector3 left, Tile leftTile,
        Vector3 right, Tile rightTile, HexTileDataOverrider overrider)
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
                    var hash = HexMetrics.SampleHashGrid((pivot + left + right) / 3f);
                    hasTower = hash.E < HexMetrics.WallTowerThreshold;
                }

                // 这里入参还得观察一下会不会 bug
                AddWallSegment(pivotTile, leftTile, pivot, left, pivot, right, hasTower);
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
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        var center = HexMetrics.WallLerp(near, far);
        var thickness = HexMetrics.GetWallThickness();
        var height = HexMetrics.GetWallHeight();
        var centerTop = center.Length() + height;
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        _walls.AddQuadUnperturbed([v1, v2, v3, v4],
            [HexMesh.Weights1, HexMesh.Weights2, HexMesh.Weights1, HexMesh.Weights2],
            tis: new Vector3(nearTile.Id, farTile.Id, nearTile.Id));
    }

    private void AddWallWedge(Tile nearTile, Vector3 near, Tile farTile, Vector3 far, Tile pointTile, Vector3 point)
    {
        near = HexMetrics.Perturb(near);
        far = HexMetrics.Perturb(far);
        point = HexMetrics.Perturb(point);
        var center = HexMetrics.WallLerp(near, far);
        var thickness = HexMetrics.GetWallThickness();
        var height = HexMetrics.GetWallHeight();
        var centerTop = center.Length() + height;
        point = Math3dUtil.ProjectToSphere(point, center.Length());
        var pointTop = Math3dUtil.ProjectToSphere(point, centerTop);
        Vector3 v1, v2, v3, v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(near, far, true, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(near, far, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, centerTop);
        v4 = Math3dUtil.ProjectToSphere(v4, centerTop);
        var ids = new Vector3(nearTile.Id, farTile.Id, pointTile.Id);
        _walls.AddQuadUnperturbed([v1, point, v3, pointTop],
            [HexMesh.Weights1, HexMesh.Weights3, HexMesh.Weights1, HexMesh.Weights3], tis: ids);
        _walls.AddQuadUnperturbed([point, v2, pointTop, v4],
            [HexMesh.Weights2, HexMesh.Weights3, HexMesh.Weights2, HexMesh.Weights3], tis: ids);
        _walls.AddTriangleUnperturbed([pointTop, v3, v4],
            [HexMesh.Weights3, HexMesh.Weights1, HexMesh.Weights2], tis: ids);
    }
}