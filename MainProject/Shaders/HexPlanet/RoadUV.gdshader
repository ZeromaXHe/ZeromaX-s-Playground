shader_type spatial;
// 设置渲染模式为无光照模式，并且深度绘制为不透明对象。
// 这相当于 Unity 中的 RenderType="Opaque" 和 Queue="Geometry+1"。
render_mode unshaded, depth_draw_opaque;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

// 设置深度偏移
void vertex() {
	// 设置深度偏移，相当于 Unity 中的 Offset -1, -1。
	VERTEX *= 1.005;
}

void fragment() {
	vec2 uv = UV;

	// 创建颜色向量，使用 UV 坐标作为 RGB 值，Alpha 为 1
	vec4 c = vec4(uv, 1.0, 1.0);

	// 设置漫反射颜色
	ALBEDO = c.rgb;
	// 设置金属度
	METALLIC = metallic;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = c.a;
}