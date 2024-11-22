using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using FrontEnd4IdleStrategyFS.Display.HexGlobal;
using Godot;

namespace ZeromaXPlayground.game.HexGlobal.Map;

[Tool]
public partial class HexPlanetManager : HexPlanetManagerFS
{
    [Export]
    private bool Regenerate
    {
        get => _Regenerate;
        set => _Regenerate = value;
    }

    [Export(PropertyHint.Range, "1,10000")]
    public float PlanetRadius
    {
        get => _planetRadius;
        set => _planetRadius = value;
    }

    [Export(PropertyHint.Range, "0,7")]
    public int Subdivisions
    {
        get => _subdivisions;
        set => _subdivisions = value;
    }

    [Export(PropertyHint.Range, "0,6")]
    public int ChunkSubdivisions
    {
        get => _chunkSubdivisions;
        set => _chunkSubdivisions = value;
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int Octaves
    {
        get => _octaves;
        set => _octaves = value;
    }

    [Export(PropertyHint.Range, "0, 1")]
    public float Persistence
    {
        get => _persistence;
        set => _persistence = value;
    }

    [Export(PropertyHint.Range, "1, 10")]
    public float Lacunarity
    {
        get => _lacunarity;
        set => _lacunarity = value;
    }

    [Export]
    public float MinHeight
    {
        get => _minHeight;
        set => _minHeight = value;
    }

    [Export]
    public float MaxHeight
    {
        get => _maxHeight;
        set => _maxHeight = value;
    }

    [Export]
    public float NoiseScaling
    {
        get => _noiseScaling;
        set => _noiseScaling = value;
    }

    // 必须保留此处和 partial，请忽略 IDE 建议 
    public override void _Ready()
    {
        // 参考 Godot Issue #78513：https://github.com/godotengine/godot/issues/78513
        // 使用一个 strong handle 来阻塞卸载
        var handle = GCHandle.Alloc(this);

        // 不在此处向所处的程序集加载上下文中注册 Unload 事件的清理响应式编程监听的逻辑的话，会导致 [Tool] .NET 程序集卸载失败
        // 从而报错：.NET: Failed to unload assemblies
        var alc = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
        GD.Print($"C# ALC: {alc?.ToString() ?? "No ALC - No Name"} ALC is null? {alc == null}");
        alc!.Unloading += _ =>
        {
            GD.Print("Start Unloading HexPlanetManager");
            AssemblyLoadContext.GetLoadContext(typeof(HexPlanetManagerFS).Assembly)!.Unload();
            // handle.Free();
            GD.Print("End Unloading HexPlanetManager");
        };

        base._Ready();
    }
}