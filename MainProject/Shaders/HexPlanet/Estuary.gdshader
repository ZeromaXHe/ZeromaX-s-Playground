shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#include "HexCellData.gdshaderinc"
#include "Water.gdshaderinc"
varying vec2 visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y;
	// 波浪动画
	float wave_vertex_amplitude = WaveAnim(CAMERA_POSITION_WORLD, VERTEX);
	// step(UV2.y, 0.9) 控制河流出口(UV2.y = 0.8 || 0)不跟随波浪摆动（会和河流产生接缝）
	// 而且使用归一化 VERTEX 而不是 NORMAL，这样不同三角面间的相同顶点不会产生接缝
	VERTEX += max(step(UV2.y, 0.9), step(-0.1, UV2.y)) * normalize(VERTEX) * wave_vertex_amplitude;
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	float shore = UV.y;
	float foam = Foam(shore, world_pos, main_tex);
	float waves = Waves(world_pos, main_tex);
	float shoreWater = max(foam, waves);
	float river = River(UV2, main_tex);
	float water = mix(shoreWater, river, UV.x);
	// 计算最终颜色
	vec4 c = clamp(color + water, 0., 1.);
	float explored = visibility.y;
	// 设置输出
	ALBEDO = c.rgb * visibility.x;
	SPECULAR = specular * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = color.a * explored;
}