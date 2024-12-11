// 通义灵码根据 Catlike Coding - Hex Map 8 水 - 7.5 河口流量 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-8/#7.5
shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#include "Water.gdshaderinc"
#include "HexCellData.gdshaderinc"
varying float visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	visibility = cell0.x * COLOR.x + cell1.x * COLOR.y;
	visibility = mix(0.25, 1, visibility);
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;

	float shore = UV.y;
	float foam = Foam(shore, world_pos.xz, main_tex);
	float waves = Waves(world_pos.xz, main_tex);
	float shoreWater = max(foam, waves);
	float river = River(UV2, main_tex);
	float water = mix(shoreWater, river, UV.x);
	// 计算最终颜色
	vec4 c = clamp(color + water, 0.0, 1.0);

	// 设置输出
	ALBEDO = c.rgb * visibility;
	METALLIC = metallic;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = c.a;
}