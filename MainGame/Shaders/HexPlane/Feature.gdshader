shader_type spatial;
//render_mode depth_prepass_alpha;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;
uniform vec3 background_color: source_color = vec3(0.2, 0.169, 0.133);//vec3(0.033, 0.0245, 0.016);
uniform sampler2D grid_coordinates: source_color, filter_nearest, repeat_enable;

#include "HexCellData.gdshaderinc"
varying vec2 visibility;

void vertex() {
	// in mat4 MODEL_MATRIX 模型空间到世界空间变换.
	vec3 pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec2 gridUV = vec2(pos.xz);
	gridUV.x *= 1.0 / (4.0 * 8.66025404);
	gridUV.y *= 1.0 / (2.0  * 15.0);
	vec2 cellDataCoordinates = floor(gridUV.xy) + textureLod(grid_coordinates, gridUV, 0.0).rg;
	cellDataCoordinates *= 2.0;
	vec4 cellData = GetCellData2(cellDataCoordinates);
	visibility.x = cellData.x;
	visibility.x = mix(0.25, 1.0, visibility.x);
	visibility.y = cellData.y;
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

