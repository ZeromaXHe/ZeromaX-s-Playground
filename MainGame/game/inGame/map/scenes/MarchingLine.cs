using BackEnd4IdleStrategyFS.Game;
using Godot;
using ZeromaXPlayground.game.inGame.map.scripts.constant;
using ZeromaXPlayground.game.inGame.map.scripts.eventBus;

public partial class MarchingLine : Line2D
{
    #region on-ready nodes

    private PanelContainer _panelContainer;
    private Label _populationLabel;
    private ProgressBar _progressBar;

    #endregion

    private int _marchingArmyId = Constants.NullId;
    private int _speed = 25;

    public void Init(DomainT.MarchingArmy marchingArmy, Vector2 from, Vector2 to, Color color)
    {
        _marchingArmyId = marchingArmy.id.Item;
        _speed = marchingArmy.population switch
        {
            < 10 => 50, // 人数小于 10 人，2 秒后到达目的地
            < 50 => 25, // 小于 50 人，4 秒后
            < 200 => 15, // 小于 200 人，7 秒左右后
            < 1000 => 10, // 小于 1000 人，10 秒后
            _ => 5 // 大于 1000 人，20 秒后
        };
        // 线条
        Position = from;
        Points = new[] { Vector2.Zero, to - from }; // Points[1] = to - from; 这种写法无法生效
        // GD.Print($"MarchingLine Init Points: {string.Join(",", Points)}");
        DefaultColor = color;
        // 信息栏
        _populationLabel.Text = $"{marchingArmy.population}";
        _panelContainer.Position = (to - from) / 2 - _panelContainer.Size / 2;
        // 进度条
        _progressBar.Value = 0;
        if (to.X < from.X)
        {
            _progressBar.SetFillMode((int)ProgressBar.FillModeEnum.EndToBegin);
        }

        var styleBoxFlat = new StyleBoxFlat();
        styleBoxFlat.BgColor = color;
        _progressBar.Set("theme_override_styles/fill", styleBoxFlat);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _panelContainer = GetNode<PanelContainer>("PanelContainer");
        _populationLabel = GetNode<Label>("PanelContainer/VBoxContainer/Population");
        _progressBar = GetNode<ProgressBar>("PanelContainer/VBoxContainer/ProgressBar");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_progressBar.Value >= 100)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.MarchingArmyArrivedDestination, _marchingArmyId);
            QueueFree();
        }

        _progressBar.Value += _speed * delta;
    }
}