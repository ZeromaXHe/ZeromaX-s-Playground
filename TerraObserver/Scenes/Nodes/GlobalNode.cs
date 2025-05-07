using System.Reflection;
using System.Runtime.Loader;
using Godot;

namespace TerraObserver.Scenes.Nodes;

[Tool]
public partial class GlobalNode : Node, ISerializationListener
{
    public static GlobalNode? Instance { get; private set; }

    public int Count { get; set; }

    public GlobalNode()
    {
        GD.Print("GlobalNode _Init");
        Instance = this;
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;
        context.Unloading += _ => GD.Print("GlobalNode AssemblyLoadContext Unloading");
    }

    public override void _EnterTree()
    {
        GD.Print("GlobalNode _EnterTree");
    }

    public override void _Ready()
    {
        GD.Print("GlobalNode _Ready");
    }

    public override void _ExitTree()
    {
        GD.Print("GlobalNode _ExitTree");
    }

    public override void _Notification(int what)
    {
        switch (what)
        {
            case (int)NotificationPredelete:
                GD.Print("GlobalNode _Notification Pre-delete");
                break;
        }
    }

    public void OnBeforeSerialize()
    {
        GD.Print("GlobalNode OnBeforeSerialize");
    }

    public void OnAfterDeserialize()
    {
        GD.Print("GlobalNode OnAfterDeserialize");
    }
}