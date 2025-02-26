using Godot;
using Godot.Collections;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entity;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node.Interface;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Util;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class HexFeatureManager : Node3D, IHexFeatureManager
{
    /// 特征场景
    /// 这种嵌套的数组不能用 .NET 的 array，必须用 Godot.Collections.Array。否则编辑器编译不通过
    /// 而且这样写以后，其实编辑器里是判断不了 PackedScene 类型的，必须自己手动选成 Object（Godot.GodotObject 或其他派生类型）再用。
    [Export] private Array<Array<PackedScene>> _urbanScenes;

    [Export] private Array<Array<PackedScene>> _farmScenes;
    [Export] private Array<Array<PackedScene>> _plantScenes;
    [Export] private HexMesh _walls;
    public IHexMesh Walls => _walls;
    [Export] private PackedScene _wallTowerScene;
    [Export] private PackedScene _bridgeScene;
    [Export] private PackedScene[] _specialScenes;

    private Node3D _container;
    private const float StandardRadius = 150f; // 150f 半径时才是标准大小，其他时候需要按比例缩放

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
        var scale = position.Length() / StandardRadius;
        tower.Scale = Vector3.One * scale;
        tower.Position = position.Normalized() * (position.Length() + 0.5f * 1.25f * scale * tower.Size.Y);
        Node3dUtil.AlignYAxisToDirection(tower, position);
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
        var scale = position.Length() / StandardRadius;
        bridge.Scale = new Vector3(length / HexMetrics.BridgeDesignLength, scale, scale); // 沿着桥梁方向拉伸长度（X 轴）
        bridge.Position = position.Normalized() * (position.Length() + 0.7f * scale * bridge.Size.Y); // 0.7f 是桥梁需要略微抬高一点
        Node3dUtil.AlignYAxisToDirection(bridge, position);
        bridge.Rotate(position.Normalized(),
            bridge.Basis.X.SignedAngleTo(roadCenter2 - roadCenter1, position.Normalized()));
        _container.AddChild(bridge);
    }

    public void AddSpecialFeature(Tile tile, Vector3 position)
    {
        var instance = _specialScenes[tile.SpecialIndex - 1].Instantiate<CsgBox3D>();
        position = HexMetrics.Perturb(position);
        var scale = position.Length() / StandardRadius;
        instance.Scale = Vector3.One * scale;
        instance.Position = position.Normalized() * (position.Length() + 0.5f * scale * instance.Size.Y);
        Node3dUtil.AlignYAxisToDirection(instance, position);
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
        var scale = position.Length() / StandardRadius;
        instance.Scale = Vector3.One * scale;
        instance.Position = position.Normalized() * (position.Length() + 0.5f * scale * instance.Size.Y);
        Node3dUtil.AlignYAxisToDirection(instance, position);
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
}