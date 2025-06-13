shader_type spatial;
//render_mode depth_prepass_alpha;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;
uniform vec3 background_color: source_color = vec3(0.2, 0.169, 0.133);//vec3(0.033, 0.0245, 0.016);

#ifndef HEX_CELL_DATA_INCLUDED
	#include "HexCellData.gdshaderinc"
#endif

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
	// 计算最终颜色
	vec4 c = texture(main_tex, UV) * color;
	float explored = visibility.y;
	// 设置漫反射颜色
	ALBEDO = c.rgb * (visibility.x * explored);
	SPECULAR = specular * explored;
	AO = explored;
	EMISSION = background_color * (1.0 - explored);
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	//ALPHA = c.a;
}