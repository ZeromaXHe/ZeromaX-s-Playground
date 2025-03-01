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

    private Node3D _container;

    public void Clear()
    {
        _container?.QueueFree();
        _container = new Node3D();
        AddChild(_container);
        _walls.Clear();
    }

    public void Apply() => _walls.Apply();

    public void AddTower(Vector3 left, Vector3 right)
    {
        var tower = _wallTowerScene.Instantiate<CsgBox3D>();
        var position = (left + right) * 0.5f;
        Node3dUtil.PlaceOnSphere(tower, position, 0.5f * 1.25f * tower.Size.Y);
        var rightDirection = right - left;
        tower.Rotate(position.Normalized(),
            tower.Basis.X.SignedAngleTo(rightDirection, position.Normalized()));
        _container.AddChild(tower);
    }

    public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
    {
        roadCenter1 = HexMetrics.Perturb(roadCenter1);
        roadCenter2 = HexMetrics.Perturb(roadCenter2);
        var bridge = _bridgeScene.Instantiate<CsgBox3D>();
        var position = (roadCenter1 + roadCenter2) * 0.5f;
        var length = roadCenter1.DistanceTo(roadCenter2);
        var scale = position.Length() / HexMetrics.StandardRadius;
        bridge.Scale = new Vector3(length / HexMetrics.BridgeDesignLength, scale, scale); // 沿着桥梁方向拉伸长度（X 轴）
        bridge.Position =
            position.Normalized() * (position.Length() + 0.7f * scale * bridge.Size.Y); // 0.7f 是桥梁需要略微抬高一点
        Node3dUtil.AlignYAxisToDirection(bridge, position);
        bridge.Rotate(position.Normalized(),
            bridge.Basis.X.SignedAngleTo(roadCenter2 - roadCenter1, position.Normalized()));
        _container.AddChild(bridge);
    }

    public void AddSpecialFeature(Tile tile, Vector3 position)
    {
        var instance = _specialScenes[tile.SpecialIndex - 1].Instantiate<CsgBox3D>();
        position = HexMetrics.Perturb(position);
        Node3dUtil.PlaceOnSphere(instance, position, 0.5f * instance.Size.Y);
        var hash = HexMetrics.SampleHashGrid(position);
        instance.Rotate(position.Normalized(), hash.E * Mathf.Tau); // 入参 axis 还是得用全局坐标
        _container.AddChild(instance);
    }

    public void AddFeature(Tile tile, Vector3 position)
    {
        if (tile.IsSpecial) return;
        var hash = HexMetrics.SampleHashGrid(position);
        var scene = PickScene(_urbanScenes, tile.UrbanLevel, hash.A, hash.D);
        var otherScene = PickScene(_farmScenes, tile.FarmLevel, hash.B, hash.D);
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

        otherScene = PickScene(_plantScenes, tile.PlantLevel, hash.C, hash.D);
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
        _container.AddChild(instance);
    }

    private PackedScene PickScene(Array<Array<PackedScene>> scenes, int level, float hash, float choice)
    {
        if (level <= 0) return null;
        var thresholds = HexMetrics.GetFeatureThreshold(level - 1);
        for (var i = 0; i < thresholds.Length; i++)
            if (hash < thresholds[i])
                return scenes[i][(int)(choice * scenes[i].Count)];
        return null;
    }

    public void AddWall(EdgeVertices near, Tile nearTile, EdgeVertices far, Tile farTile,
        bool hasRiver, bool hasRoad)
    {
        if (nearTile.Walled == farTile.Walled
            || nearTile.IsUnderwater || farTile.IsUnderwater
            || nearTile.GetEdgeType(farTile) == HexEdgeType.Cliff)
            return;
        AddWallSegment(near.V1, far.V1, near.V2, far.V2);
        if (hasRiver || hasRoad)
        {
            AddWallCap(near.V2, far.V2);
            AddWallCap(far.V4, near.V4);
        }
        else
        {
            AddWallSegment(near.V2, far.V2, near.V3, far.V3);
            AddWallSegment(near.V3, far.V3, near.V4, far.V4);
        }

        AddWallSegment(near.V4, far.V4, near.V5, far.V5);
    }

    public void AddWall(Vector3 c1, Tile tile1, Vector3 c2, Tile tile2, Vector3 c3,
        Tile tile3)
    {
        if (tile1.Walled)
        {
            if (tile2.Walled)
            {
                if (!tile3.Walled)
                    AddWallSegment(c3, tile3, c1, tile1, c2, tile2);
            }
            else if (tile3.Walled)
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1);
            else
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3);
        }
        else if (tile2.Walled)
            if (tile3.Walled)
                AddWallSegment(c1, tile1, c2, tile2, c3, tile3);
            else
                AddWallSegment(c2, tile2, c3, tile3, c1, tile1);
        else if (tile3.Walled)
            AddWallSegment(c3, tile3, c1, tile1, c2, tile2);
    }

    private void AddWallSegment(Vector3 nearLeft, Vector3 farLeft,
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
        _walls.AddQuadUnperturbed([v1, v2, v3, v4]);
        Vector3 t1 = v3, t2 = v4;
        v1 = v3 = HexMetrics.WallThicknessOffset(nearLeft, farLeft, false, thickness);
        v2 = v4 = HexMetrics.WallThicknessOffset(nearRight, farRight, false, thickness);
        v3 = Math3dUtil.ProjectToSphere(v3, leftTop);
        v4 = Math3dUtil.ProjectToSphere(v4, rightTop);
        _walls.AddQuadUnperturbed([v2, v1, v4, v3]);
        _walls.AddQuadUnperturbed([t1, t2, v3, v4]);

        if (addTower)
            AddTower(left, right);
    }

    // pivot 有墙，left\right 没有墙的情况
    private void AddWallSegment(Vector3 pivot, Tile pivotTile,
        Vector3 left, Tile leftTile, Vector3 right, Tile rightTile)
    {
        if (pivotTile.IsUnderwater) return;
        var hasLeftWall = !leftTile.IsUnderwater && pivotTile.GetEdgeType(leftTile) != HexEdgeType.Cliff;
        var hasRightWall = !rightTile.IsUnderwater && pivotTile.GetEdgeType(rightTile) != HexEdgeType.Cliff;
        if (hasLeftWall)
        {
            if (hasRightWall)
            {
                var hasTower = false;
                if (leftTile.Elevation == rightTile.Elevation)
                {
                    var hash = HexMetrics.SampleHashGrid((pivot + left + right) / 3f);
                    hasTower = hash.E < HexMetrics.WallTowerThreshold;
                }

                AddWallSegment(pivot, left, pivot, right, hasTower);
            }
            else if (leftTile.Elevation < rightTile.Elevation)
                AddWallWedge(pivot, left, right);
            else
                AddWallCap(pivot, left);
        }
        else if (hasRightWall)
        {
            if (rightTile.Elevation < leftTile.Elevation)
                AddWallWedge(right, pivot, left);
            else
                AddWallCap(right, pivot);
        }
    }

    private void AddWallCap(Vector3 near, Vector3 far)
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
        _walls.AddQuadUnperturbed([v1, v2, v3, v4]);
    }

    private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
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
        _walls.AddQuadUnperturbed([v1, point, v3, pointTop]);
        _walls.AddQuadUnperturbed([point, v2, pointTop, v4]);
        _walls.AddTriangleUnperturbed([pointTop, v3, v4]);
    }
}