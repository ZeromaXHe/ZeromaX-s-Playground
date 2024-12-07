// 没调好，直接先用 Godot 默认设置支持的功能吧
shader_type spatial;
render_mode depth_test_disabled, depth_prepass_alpha;

uniform sampler2D main_tex : hint_default_white;
uniform vec4 color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform bool pixel_snap = false;
uniform sampler2D alpha_tex : hint_default_white;
uniform bool enable_external_alpha = false;

void fragment() {
	vec2 uv = UV;
	if (pixel_snap) {
		uv = floor(uv + vec2(0.5));
	}

	vec4 c = texture(main_tex, uv) * color;
	if (enable_external_alpha) {
		c.a *= texture(alpha_tex, uv).a;
	}

	ALBEDO = c.rgb;
	ALPHA = c.a;
}

