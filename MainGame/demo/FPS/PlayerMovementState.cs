using Godot;

namespace ZeromaXPlayground.demo.FPS;

public abstract partial class PlayerMovementState : State
{
    protected FpsController Player;
    protected AnimationPlayer AnimationPlayer;

    public override async void _Ready()
    {
        await ToSignal(Owner, Node.SignalName.Ready);
        Player = Owner as FpsController;
        AnimationPlayer = Player!.AnimationPlayer;
    }
}