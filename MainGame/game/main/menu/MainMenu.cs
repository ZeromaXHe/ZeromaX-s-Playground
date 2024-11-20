using FrontEnd4IdleStrategyFS.Display.InGame;

public partial class MainMenu : MainMenuFS
{
    // 虽然 IDE 会建议省略，实际上这里不能省略。否则 Godot 分部类不会正常生效
    public override void _Ready()
    {
        base._Ready();
    }
}