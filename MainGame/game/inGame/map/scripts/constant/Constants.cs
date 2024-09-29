using Godot;

namespace ZeromaXPlayground.game.inGame.map.scripts.constant;

public class Constants
{
    public const int NullId = -1;

    public static readonly Color[] PlayerColors =
    {
        Colors.Red, Colors.Yellow, Colors.Green, Colors.Aqua,
        Colors.Blue, Colors.Purple, Colors.DeepPink, Colors.Orange
    };

    public static readonly string[] PlayerNames =
    {
        "Player_Name_1", "Player_Name_2", "Player_Name_3", "Player_Name_4",
        "Player_Name_5", "Player_Name_6", "Player_Name_7", "Player_Name_8"
    };
}