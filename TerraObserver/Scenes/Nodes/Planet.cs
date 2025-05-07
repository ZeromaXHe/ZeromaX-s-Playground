using System;
using System.Reflection;
using System.Runtime.Loader;
using Godot;
using TO.Contexts;
using TO.Nodes.Abstractions;

namespace TerraObserver.Scenes.Nodes;

[Tool]
public partial class Planet : Node3D, IPlanet, ISerializationListener
{
    [ExportToolButton("Print!")]
    private Callable Print => Callable.From(() => GD.Print($"Print {GlobalNode.Instance?.Count}"));

    private Object? _testUnload = new();

    public Planet()
    {
        GD.Print("Planet _Init");
        // Context.RegisterToHolder<IPlanet>(this);
        
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;
        context.Unloading += Unload;
    }

    private void Unload(AssemblyLoadContext assemblyLoadContext)
    {
        GD.Print(_testUnload?.ToString());
        _testUnload = null;
        // Context.UnloadThis(assemblyLoadContext);
        GD.Print("Planet AssemblyLoadContext Unloading");
    }

    public override void _EnterTree()
    {
        GD.Print("Planet _EnterTree");
    }

    public override void _Ready()
    {
        // 解决 .NET: Failed to unload assemblies. Please check <this issue> for more information. 问题
        // GitHub issue 78513：https://github.com/godotengine/godot/issues/78513
        // register cleanup code to prevent unloading issues
        GD.Print("Planet _Ready");
        // var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;
        // context.Unloading += Context.UnloadThis;
        if (GlobalNode.Instance != null) GlobalNode.Instance.Count++;
        GD.Print($"Planet Global Count:{GlobalNode.Instance?.Count}");
    }

    public override void _ExitTree()
    {
        GD.Print("Planet _ExitTree");
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case (int)NotificationPredelete:
                GD.Print("Planet _Notification Pre-delete");
                break;
        }
    }

    public event Action<double>? Processed;

    public override void _Process(double delta) => Processed?.Invoke(delta);
    public event Action<float, float>? RadiusChanged;

    private float _radius = 100f;

    [Export(PropertyHint.Range, "5, 1000")]
    public float Radius
    {
        get => _radius;
        set
        {
            var pre = _radius;
            _radius = value;
            RadiusChanged?.Invoke(pre, value);
        }
    }

    public void OnBeforeSerialize()
    {
        GD.Print("Planet OnBeforeSerialize");
        // Context.UnloadThis(AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!);
    }

    public void OnAfterDeserialize()
    {
        GD.Print("Planet OnAfterDeserialize");
    }
}