shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#include "Water.gdshaderinc"

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec2 pos = vec2(world_pos.x - world_pos.y * 0.5 - world_pos.z * 0.5,
		(world_pos.y - world_pos.z) * OUTER_TO_INNER);
	float waves = Waves(pos, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + waves, 0.0, 1.0);
	// 设置输出
	ALBEDO = c.rgb;
	SPECULAR = specular;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = c.a;
}