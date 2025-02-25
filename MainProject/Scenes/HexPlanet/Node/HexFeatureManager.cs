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

    private Node3D _container;

    public void Clear()
    {
        _container?.QueueFree();
        _container = new Node3D();
        AddChild(_container);
    }

    public void Apply()
    {
    }

    public void AddFeature(Tile tile, Vector3 position)
    {
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
        var scale = position.Length() / 150f; // 150f 半径时是标准大小
        instance.Scale = Vector3.One * scale; 
        instance.Position = position.Normalized() * (position.Length() + 0.5f * scale * instance.Size.Y);
        Node3dUtil.AlignYAxisToDirection(instance, position);
        instance.Rotate(position.Normalized(), hash.C * Mathf.Tau); // 入参 axis 还是得用全局坐标
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