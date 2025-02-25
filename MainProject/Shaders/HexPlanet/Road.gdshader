shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

#include "HexMetrics.gdshaderinc"

// 设置深度偏移
void vertex() {
	VERTEX *= 1.005;
}

void fragment() {
	vec2 uv = UV;
	// INV_VIEW_MATRIX 视图空间向世界空间变换.
	// VERTEX 来自顶点函数的顶点(默认情况下, 在视图空间中).
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;

	vec4 noise = texture(main_tex, world_pos.xz * (3.0 * TILING_SCALE));
	vec4 c = color * (noise.y * 0.75 + 0.25);
	float blend = uv.x;
	blend *= noise.x + 0.5;
	blend = smoothstep(0.4, 0.7, blend);
	ALBEDO = c.rgb;
	SPECULAR = specular;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = blend;
}