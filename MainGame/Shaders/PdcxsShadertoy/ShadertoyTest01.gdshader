// Shadertoy教程01——简介以及坐标操作:
// https://www.bilibili.com/video/BV1ua411k7DY
shader_type canvas_item;

// Shadertoy 默认颜色
//void fragment() {
//	vec2 iResolution = 1.0 / SCREEN_PIXEL_SIZE;
//	vec2 uv = FRAGCOORD.xy / iResolution;
//	vec3 col = 0.5 + 0.5 * cos(TIME + uv.xyx + vec3(0, 2, 4));
//	COLOR = vec4(col, 1);
//}

// UV 测试
void fragment() {
	vec2 iResolution = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = (FRAGCOORD.xy - 0.5 * iResolution.xy) / iResolution.xx;
	float c = 0.; //length(uv);
	float r = 0.3;
	if (length(uv) < r) {
		c = 1.;
	}
	COLOR = vec4(vec3(c), 1);
}
