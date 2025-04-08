shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;
global uniform mat4 inv_planet_matrix; // 逆本地坐标矩阵

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#ifndef HEX_CELL_DATA_INCLUDED
	#include "HexCellData.gdshaderinc"
#endif

#ifndef WATER_INCLUDED
	#include "Water.gdshaderinc"
#endif

varying vec4 civ;
varying vec2 visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y + cell2.x * COLOR.z;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z;

	vec3 civ1 = GetTileCivDataById(CUSTOM0[0]).rgb;
	vec3 civ2 = GetTileCivDataById(CUSTOM0[1]).rgb;
	vec3 civ3 = GetTileCivDataById(CUSTOM0[2]).rgb;
	civ.rgb = civ1 * COLOR.x + civ2 * COLOR.y + civ3 * COLOR.z;

	// 波浪动画
	vec3 cam_pos_local = (inv_planet_matrix * vec4(CAMERA_POSITION_WORLD, 1.)).xyz;
	float wave_vertex_amplitude = WaveAnim(cam_pos_local, VERTEX);
	// 使用归一化 VERTEX 而不是 NORMAL，这样不同三角面间的相同顶点不会产生接缝
	VERTEX += normalize(VERTEX) * wave_vertex_amplitude;
}

void fragment() {
	// 获取世界/本地坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec3 local_pos = (inv_planet_matrix * vec4(world_pos, 1.)).xyz;
	float shore = UV.y;
	float foam = Foam(shore, local_pos, main_tex);
	float waves = Waves(local_pos, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + max(foam, waves), 0.0, 1.0);
	float explored = visibility.y;
	// 设置输出
	ALBEDO = (0.5 * c.rgb + 0.5 * civ.rgb) * visibility.x;
	SPECULAR = specular * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = color.a * explored;
}