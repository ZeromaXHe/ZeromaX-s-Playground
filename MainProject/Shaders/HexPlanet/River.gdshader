shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

#include "HexCellData.gdshaderinc"
#include "Water.gdshaderinc"

varying vec2 visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y;
}

void fragment() {
	float river = River(UV, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + river, 0.0, 1.0);
	float explored = visibility.y;
	// 设置漫反射颜色
	ALBEDO = c.rgb * visibility.x;
	SPECULAR = specular * explored;
	AO = explored;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = c.a * explored;
}