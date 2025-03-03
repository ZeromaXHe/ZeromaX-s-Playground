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
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y + cell2.x * COLOR.z;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z;
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec2 sphere_uv = WorldPosToSphereUV(world_pos);
	float shore = UV.y;
	float foam = Foam(shore, sphere_uv, main_tex);
	float waves = Waves(sphere_uv, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + max(foam, waves), 0.0, 1.0);
	float explored = visibility.y;
	// 设置输出
	ALBEDO = c.rgb * visibility.x;
	SPECULAR = specular * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = c.a * explored;
}