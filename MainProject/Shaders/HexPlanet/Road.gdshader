shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

#include "HexCellData.gdshaderinc"
#include "Sebastian/Triplanar.gdshaderinc"
varying vec2 visibility;

// 设置深度偏移
void vertex() {
	VERTEX *= 1.005;
	
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y;
}

void fragment() {
	vec2 uv = UV;
	// INV_VIEW_MATRIX 视图空间向世界空间变换.
	// VERTEX 来自顶点函数的顶点(默认情况下, 在视图空间中).
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz; // 位置需位移，补 1.0
	vec3 world_normal = (INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz; // 法线不位移，补 0.0
	vec4 noise = triplanar(world_pos, world_normal, 15.0 * TILING_SCALE, main_tex);
	vec4 c = color * (noise.y * 0.75 + 0.25) * visibility.x;
	float blend = uv.x;
	blend *= noise.x + 0.5;
	blend = smoothstep(0.4, 0.7, blend);
	float explored = visibility.y;
	ALBEDO = c.rgb;
	SPECULAR = specular * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = blend * explored;
}