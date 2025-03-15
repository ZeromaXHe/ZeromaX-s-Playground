shader_type spatial;
render_mode shadows_disabled, world_vertex_coords;
uniform vec3 color: source_color;
// 奇偶线条数量(沿视图空间 y 轴划分)
uniform float lines1: hint_range(0.0, 200.0, 1.0) = 2.0;
// 随机线条数量(沿视图空间 y 轴划分)
uniform float lines2: hint_range(0.0, 100.0, 1.0) = 2.0;
uniform float distortion_strength: hint_range(0.0, 1.0) = 0.4;
uniform float distortion_percentage: hint_range(0.0, 1.0) = 0.02;
uniform float _emission_min: hint_range(0.0, 10.0) = 1.0;
uniform float _emission_max: hint_range(0.0, 10.0) = 4.0;
uniform float _fresnel: hint_range(0.0, 10.0) = 1.0;
// 视图 FOV
uniform float viewmodel_fov = 75.0f;
global uniform float max_height;
global uniform float radius;
global uniform int divisions;

float random(float x) {
	return fract(sin(x * 12.9898 * 78.233) * 43758.5433);
}

float random2_1(vec2 uv) {
	return fract(sin(dot(uv.xy + vec2(sin(TIME), cos(TIME)), vec2(12.9898, 78.233))) * 43758.5453123);
}

float fresnel(float amount, vec3 normal, vec3 view) {
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0)), amount);
}

varying float rand_dist;

void vertex() {
	float distortion = sin(TIME * 10.0 + VERTEX.y * 20.0) * 0.04;
	float rand_time = random(TIME);
	rand_dist = step(rand_time, 1. - distortion_percentage);
	VERTEX *= 1. + distortion * rand_dist * distortion_strength * max_height / radius;
	
	// 使用裁剪视图空间的方式，使得低于地面的全息图也能显示（就是对应 FPS 处理武器穿模的 Shader 代码）
	// E12 教程视频里的 Godot Shader 流程图：
	//
	// Vertex Shader:
	// 模型/本地空间 Model/Local Space
	// -(乘以 MODEL_MATRIX)-> 世界/全局空间 World/Global Space ->
	// -(乘以 VIEW_MATRIX)-> 视图/相机空间 View/Camera Space ->
	// -(乘以 PROJECTION_MATRIX)-> 剪切空间 Clip Space ->
	// |-----------------------
	// -> 透视除法 Perspective Division: vec3 ndc = clip.xyz / clip.w ->
	// |-----------------------
	// | Fragment Shader
	// -> 归一化设备坐标 Normalized Device Coordinates -> 屏幕空间 Screen Space
	/* VIEW MODEL Z CLIP FIX CODE */
	if (viewmodel_fov > 0.0) {
		float onetanfov = 1.0f / tan(0.5f * (viewmodel_fov * PI / 180.0f));
		float aspect = VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;
		// Optional: Modify projection matrix to change the FOV
		PROJECTION_MATRIX[1][1] = -onetanfov;
		PROJECTION_MATRIX[0][0] = onetanfov / aspect;
	}
	// 注意我们开启了 world_vertex_coords，所以这里乘 VIEW_MATRIX 而不是 MODELVIEW_MATRIX
	POSITION = PROJECTION_MATRIX * VIEW_MATRIX * vec4(VERTEX.xyz, 1.0);
	// Mix 顶点 Z 到超级接近 clip 平面, 仍然留了一点距离
	//POSITION.z = mix(POSITION.z, 0.0, 0.999); // 老版本。 向 0.0 进行 Mix (接近 z 平面)
	// 上面代码对于 Godot 4.3 无效，因为 Z 被反转了: https://godotengine.org/article/introducing-reverse-z/
	// 现在我们需要向 1.0 mix, 但在 clip 空间会除以 POSITION.w，所以仍然没法生效。
	// 所以我们向 POSITION.w mix z，这样在透视除法（Persperctive Division）后，它将变成 1.0 (接近 buffer 因为 Z 被反转)
	POSITION.z = mix(POSITION.z, POSITION.w, 0.9); // 可以取更高的 mix 值，像 0.999，但我发现 0.9 防止 99% clipping w/ 比 0.999 更少失真
	/* END VIEW MODEL Z CLIP FIX CODE */
}

void fragment() {
	float n = mix(0.4, 1., random2_1(SCREEN_UV));
	float f = fresnel(_fresnel, NORMAL, VIEW);
	vec2 deep_uv = VERTEX.xy * float(divisions) / (radius + max_height) / 2.0;
	// 增加奇偶线
	vec2 igrid = floor(deep_uv * lines1);
	float l1 = mod(igrid.y, 2.0);
	l1 = mix(0.4, 1.0, l1);
	// 随机线条
	vec2 uv2 = deep_uv;
	uv2.y -= TIME * 0.06;
	vec2 igrid2 = floor(uv2 * lines2);
	float l2 = random(igrid2.y);
	l2 = step(0.2, l2);
	l2 = mix(0.2, 1.0, l2);
	
	float m = f * n * l1 * l2;
	ALPHA = m;
	ALBEDO = color * mix(_emission_min, _emission_max, rand_dist); // 变形时高亮
}
