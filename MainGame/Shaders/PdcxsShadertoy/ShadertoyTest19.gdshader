// Shadertoy教程19——随机数的生成
// https://www.bilibili.com/video/BV1ce411g7B2
shader_type canvas_item;

#define C 0.707107

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float Noise(vec2 x) {
	return abs(fract(114514.114514 * sin(dot(x, vec2(123., 456.)))));
}

float Line(vec2 p, float dir) {
	float d = dot(p, dir > 0.5? vec2(C, C): -vec2(-C, C));
	return smoothstep(.03, 0., abs(d));
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = 5. * FixUV(FRAGCOORD.xy);
	uv += TIME / 5.;
	vec3 col = vec3(Line(fract(uv) - .5, Noise(floor(uv))));
	COLOR = vec4(col, 1.);
}
