using Godot;

namespace ZeromaXPlayground.demo.FPS.States;

public abstract partial class PlayerMovementState : State
{
    protected FpsController Player;
    protected AnimationPlayer Animation;

    public override async void _Ready()
    {
        await ToSignal(Owner, Node.SignalName.Ready);
        Player = Owner as FpsController;
        Animation = Player!.AnimationPlayer;
    }
}