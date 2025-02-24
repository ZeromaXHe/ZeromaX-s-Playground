shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

#include "Water.gdshaderinc"

void fragment() {
	float river = River(UV, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + river, 0.0, 1.0);
	// 设置漫反射颜色
	ALBEDO = c.rgb;
	SPECULAR = specular;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = c.a;
}