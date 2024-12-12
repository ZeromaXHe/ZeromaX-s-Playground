shader_type spatial;
render_mode depth_prepass_alpha;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;
uniform sampler2D grid_coordinates: source_color, filter_linear, repeat_enable;


#include "HexCellData.gdshaderinc"
varying float visibility;

void vertex() {
	// in mat4 MODEL_MATRIX 模型空间到世界空间变换.
	vec3 pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec2 gridUV = vec2(pos.xz);
	gridUV.x *= 1.0 / (4.0 * 8.66025404);
	gridUV.y *= 1.0 / (2.0  * 15.0);
	vec2 cellDataCoordinates = floor(gridUV.xy) + textureLod(grid_coordinates, gridUV, 0.0).rg;
	cellDataCoordinates *= 2.0;
	visibility = GetCellData2(cellDataCoordinates).x;
	visibility = mix(0.25, 1.0, visibility);
}

void fragment() {
	// 计算最终颜色
	vec4 c = texture(main_tex, UV) * color;

	// 设置漫反射颜色
	ALBEDO = c.rgb * visibility;
	// 设置金属度
	METALLIC = metallic;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = c.a;
}

