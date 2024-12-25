// Shadertoy教程26——Fractal Brownian Motion
// https://www.bilibili.com/video/BV1Vp4y1w7A9
shader_type canvas_item;

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float Random(vec2 p) {
	vec3 p3 = fract(vec3(p.xyx) * .1031);
	p3 += dot(p3, p3.yzx + 33.33);
	return fract((p3.x + p3.y) * p3.z);
	//vec2 p2 = 55.1876653 * fract(p * 10.1321513);
	//return fract((p2.x + p2.y) * p2.x * p2.y);
}

vec3 Noise(vec2 pos) {
	vec2 i = floor(pos);
	vec2 f = fract(pos);
	vec2 u = f * f * (3.0 - 2.0 * f);
	vec2 du = 6. * u * (1. - u);
	float a = Random(i);
	float b = Random(i + vec2(1.0, 0.));
	float c = Random(i + vec2(0.0, 1.0));
	float d = Random(i + vec2(1.0, 1.0));
	float val = a + (b - a) * u.x * (1. - u.y)
		+ (c - a) * (1.0 - u.x) * u.y
		+ (d - a) * u.x * u.y;
	vec2 derivative = du * (vec2(b - a, c - a) + (a - b - c + d) * u.yx);
	return vec3(val, derivative);
}

const mat2 mat = mat2(vec2(0.6, -0.8), vec2(0.8, 0.6));

float Fbm(vec2 p) {
	float a = 0.;
	float fac = 0.5;
	for (int i = 0; i < 4; i++) {
		a += fac * Noise(p).x;
		p = 2. * mat * p;
		fac *= 0.5;
	}
	return a;
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = 5. * FixUV(FRAGCOORD.xy);
	// TIME 太大貌似就会割裂图片…… 浮点数精度原因？
	vec3 col = vec3(smoothstep(0.4, 0.5, Fbm(uv + mod(TIME, 60))));
	COLOR = vec4(col, 1.);
}
