// Catlike Coding - Hex Map 14 地形纹理 - 2.5 着色器
// https://catlikecoding.com/unity/tutorials/hex-map/part-14/#2.5
shader_type spatial;
// 这个 depth_prepass_alpha 很关键，如果是写了 ALPHA = c.a 就不能少了
// 不然阴影又是一坨稀饭、还会把遮挡的东西透到前面
render_mode depth_prepass_alpha;

// 定义 Uniform 变量
uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2DArray main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform bool grid_on = true;
uniform sampler2D grid_tex: source_color;
uniform float glossiness: hint_range(0.0, 1.0) = 0.3;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

varying vec3 terrain; // 用 varying 在 vertex 和 fragment 间传递数据

void vertex() {
	terrain = CUSTOM0.xyz;
}

vec4 GetTerrainColor(vec3 world_pos, int index, vec4 inColor) {
	vec3 uvw = vec3(world_pos.xz * 0.02, terrain[index]);
	vec4 c = texture(main_tex, uvw);
	return c * inColor[index];
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec4 inColor = COLOR;
	vec4 c = GetTerrainColor(world_pos, 0, inColor) +
		GetTerrainColor(world_pos, 1, inColor) +
		GetTerrainColor(world_pos, 2, inColor);
	// 渲染六边形网格
	vec4 grid = vec4(1.0);
	if (grid_on) {
		vec2 gridUV = world_pos.xz;
		gridUV.x *= 1.0 / (4.0 * 8.66025404);
		gridUV.y *= 1.0 / (2.0 * 15.0);
		grid = texture(grid_tex, gridUV);
	}
	// 设置漫反射颜色
	ALBEDO = c.rgb * grid.rgb * color.rgb;
	// 设置金属度
	METALLIC = metallic;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	// 直接设置这个会导致河面、道路、水面等等涉及到透明度的和地形前后渲染关系混乱。
	// 而且标签的黑边也会消失……
	// 需要同时调整其他这些的材质的渲染优先度，将其他的优先度设置为 1，这里地形默认 0
	// 目前因为都是地形都是不透明的，直接不设置透明度方便些，这样也省的给标签加材质，再改渲染优先度
	ALPHA = c.a;
}