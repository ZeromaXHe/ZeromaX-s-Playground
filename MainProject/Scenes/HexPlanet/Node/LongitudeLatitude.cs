using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.GlobalNode;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Node;

[Tool]
public partial class LongitudeLatitude : Node3D
{
    [ExportToolButton("手动触发重绘经纬线", Icon = "WorldEnvironment")]
    public Callable Redraw => Callable.From(DoDraw);

    [Export(PropertyHint.Range, "0, 180")] public int LongitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "0, 90")] public int LatitudeInterval { get; set; } = 15;
    [Export(PropertyHint.Range, "1, 100")] public int Segments { get; set; } = 30; // 每个 90 度的弧线被划分多少段
    [Export] public Material LineMaterial { get; set; } = new StandardMaterial3D { VertexColorUseAsAlbedo = true };
    [ExportGroup("颜色设置")] [Export] public Color NormalLineColor { get; set; } = Colors.SkyBlue;
    [Export] public Color DeeperLineColor { get; set; } = Colors.DeepSkyBlue;
    [Export] public int DeeperLineInterval { get; set; } = 3; // 更深颜色的线多少条出现一次
    [Export] public Color TropicColor { get; set; } = Colors.Green; // 南北回归线颜色
    [Export] public Color CircleColor { get; set; } = Colors.Aqua; // 南北极圈颜色
    [Export] public Color EquatorColor { get; set; } = Colors.Yellow; // 赤道颜色
    [Export] public Color Degree90LongitudeColor { get; set; } = Colors.Orange; // 东西经 90 度颜色
    [Export] public Color MeridianColor { get; set; } = Colors.Red; // 子午线颜色

    [ExportGroup("开关特定线显示")] [Export] public bool DrawTropicOfCancer { get; set; } = true; // 是否绘制北回归线
    [Export] public bool DrawTropicOfCapricorn { get; set; } = true; // 是否绘制南回归线
    [Export] public bool DrawArcticCircle { get; set; } = true; // 是否绘制北极圈
    [Export] public bool DrawAntarcticCircle { get; set; } = true; // 是否绘制南极圈

    [ExportGroup("透明度设置")]
    [Export(PropertyHint.Range, "0.01, 5")]
    public float FullVisibilityTime { get; set; } = 0.25f;

    [Export(PropertyHint.Range, "0, 1")] public float FullVisibility { get; set; } = 0.5f;
    private bool _fixFullVisibility;

    // 是否锁定开启完全显示
    [Export]
    public bool FixFullVisibility
    {
        get => _fixFullVisibility;
        set
        {
            _fixFullVisibility = value;
            if (value)
            {
                Visibility = FullVisibility;
                _fadeVisibility = false;
                Show();
                if (_ready)
                    SignalBus.Instance.CameraMoved -= OnCameraMoved;
            }
            else
            {
                if (_ready)
                    SignalBus.Instance.CameraMoved += OnCameraMoved;
                SetProcess(true);
            }
        }
    }

    // 对应着色器中的 alpha_factor
    private float _visibility = 1f;

    private float Visibility
    {
        get => _visibility;
        set
        {
            _visibility = Mathf.Clamp(value, 0f, FullVisibility);
            if (_ready && LineMaterial is ShaderMaterial shaderMaterial)
                shaderMaterial.SetShaderParameter("alpha_factor", _visibility);
        }
    }

    private bool _fadeVisibility;

    private float _radius = 110;
    private MeshInstance3D _meshIns;

    private bool _ready;

    public override void _Ready()
    {
        _meshIns = new MeshInstance3D();
        AddChild(_meshIns);
        if (!Engine.IsEditorHint())
            SignalBus.Instance.CameraMoved += OnCameraMoved;
        _ready = true;
        // 在 _ready = true 后面，触发 setter 的着色器参数初始化
        Visibility = FullVisibility;
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() || FixFullVisibility)
        {
            SetProcess(false); // 编辑器下以及锁定显示时，无需处理显隐
            return;
        }

        if (_fadeVisibility)
            Visibility -= (float)delta / FullVisibilityTime;
        _fadeVisibility = true;

        if (Visibility > 0) return;
        Hide();
        SetProcess(false);
    }

    private void OnCameraMoved(Vector3 pos, float delta)
    {
        if (FixFullVisibility) return; // 锁定显示时不处理事件
        Show();
        Visibility += delta / FullVisibilityTime;
        _fadeVisibility = false;
        SetProcess(true);
    }

    public void Draw(float radius)
    {
        _radius = radius;
        DoDraw();
    }

    private void DoDraw()
    {
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Lines);
        for (var i = -180 + LongitudeInterval; i <= 180; i += LongitudeInterval)
        {
            var longitudeRadian = Mathf.DegToRad(i);
            var color = i is 0 or 180
                ? MeridianColor
                : i is -90 or 90
                    ? Degree90LongitudeColor
                    : i % (LatitudeInterval * DeeperLineInterval) == 0
                        ? DeeperLineColor
                        : NormalLineColor;
            DrawLongitude(surfaceTool, longitudeRadian, color);
        }

        for (var i = -90 + LatitudeInterval; i < 90; i += LatitudeInterval)
        {
            var color = i == 0
                ? EquatorColor
                : i % (LatitudeInterval * DeeperLineInterval) == 0
                    ? DeeperLineColor
                    : NormalLineColor;
            var latitudeRadian = Mathf.DegToRad(i);
            DrawLatitude(surfaceTool, latitudeRadian, color);
        }

        // 北极圈：北纬 66°34′
        if (DrawArcticCircle)
            DrawLatitude(surfaceTool, Mathf.DegToRad(66.567f), CircleColor, true);
        // 北回归线：北纬 23°26′
        if (DrawTropicOfCancer)
            DrawLatitude(surfaceTool, Mathf.DegToRad(23.433f), TropicColor, true);
        // 南回归线：南纬 23°26′
        if (DrawTropicOfCapricorn)
            DrawLatitude(surfaceTool, Mathf.DegToRad(-23.433f), TropicColor, true);
        // 南极圈：南纬 66°34′
        if (DrawAntarcticCircle)
            DrawLatitude(surfaceTool, Mathf.DegToRad(-66.567f), CircleColor, true);

        surfaceTool.SetMaterial(LineMaterial);
        _meshIns.Mesh = surfaceTool.Commit();
    }

    /// <summary>
    /// 绘制指定经线
    /// </summary>
    /// <param name="surfaceTool"></param>
    /// <param name="longitudeRadian">经度转为弧度制后的表示，+ 代表西经，- 代表东经（顺时针方向）</param>
    /// <param name="color"></param>
    private void DrawLongitude(SurfaceTool surfaceTool, float longitudeRadian, Color color)
    {
        var equatorDirection = new Vector3(Mathf.Cos(longitudeRadian), 0, Mathf.Sin(longitudeRadian));

        // 北面
        Draw90Degrees(surfaceTool, color, equatorDirection, Vector3.Up);
        // 南面
        Draw90Degrees(surfaceTool, color, equatorDirection, Vector3.Down);
    }

    /// <summary>
    /// 绘制指定纬线
    /// </summary>
    /// <param name="surfaceTool"></param>
    /// <param name="latitudeRadian">维度转为弧度制后的表示，+ 表示北纬，- 表示南纬（上方取正）</param>
    /// <param name="color"></param>
    /// <param name="dash">是否按虚线绘制</param>
    private void DrawLatitude(SurfaceTool surfaceTool, float latitudeRadian, Color color, bool dash = false)
    {
        var cos = Mathf.Cos(latitudeRadian); // 对应相比赤道应该缩小的比例
        var sin = Mathf.Sin(latitudeRadian); // 对应固定的高度
        // 本初子午线
        var primeMeridianDirection = new Vector3(cos, 0, 0);
        // 西经 90 度
        var west90Direction = new Vector3(0, 0, cos);
        Draw90Degrees(surfaceTool, color, primeMeridianDirection, west90Direction, Vector3.Up * sin, dash);
        // 对向子午线
        var antiMeridianDirection = new Vector3(-cos, 0, 0);
        Draw90Degrees(surfaceTool, color, west90Direction, antiMeridianDirection, Vector3.Up * sin, dash);
        // 东经 90 度
        var east90Direction = new Vector3(0, 0, -cos);
        Draw90Degrees(surfaceTool, color, antiMeridianDirection, east90Direction, Vector3.Up * sin, dash);
        Draw90Degrees(surfaceTool, color, east90Direction, primeMeridianDirection, Vector3.Up * sin, dash);
    }

    private void Draw90Degrees(SurfaceTool surfaceTool, Color color, Vector3 from, Vector3 to,
        Vector3 origin = default, bool dash = false)
    {
        var preDirection = from;
        for (var i = 1; i <= Segments; i++)
        {
            var currentDirection = from.Slerp(to, (float)i / Segments);
            if (!dash || i % 2 == 0)
            {
                surfaceTool.SetColor(color);
                // 【切记】：Mesh.PrimitiveType.Lines 绘制方式时，必须自己指定法线！！！否则没颜色
                surfaceTool.SetNormal(origin + preDirection);
                surfaceTool.AddVertex((origin + preDirection) * _radius);
                surfaceTool.SetColor(color);
                surfaceTool.SetNormal(origin + currentDirection);
                surfaceTool.AddVertex((origin + currentDirection) * _radius);
            }

            preDirection = currentDirection;
        }
    }
}