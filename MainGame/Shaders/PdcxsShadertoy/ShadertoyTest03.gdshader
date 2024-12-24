// Shadertoy教程03——线段绘制、clamp与mix函数: 
// https://www.bilibili.com/video/BV1oq4y1271G
// Shadertoy教程04——smoothstep函数：
// https://www.bilibili.com/video/BV1EF411B7Mo
// Shadertoy教程05——函数绘制的优化、mod与subsampling
// https://www.bilibili.com/video/BV1Ka411r7yK
shader_type canvas_item;

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	return 3. * (2. * c - iRes) / min(iRes.x, iRes.y);
}

vec3 Grid(vec2 uv) {
	vec3 color = vec3(0.4);
	vec2 grid = floor(mod(uv, 2.));
	if (grid.x == grid.y) color = vec3(0.6);
	color = mix(color, vec3(0.), smoothstep(1.1 * fwidth(uv.x), fwidth(uv.x), abs(uv.x)));
	color = mix(color, vec3(0.), smoothstep(1.1 * fwidth(uv.y), fwidth(uv.y), abs(uv.y)));
	return color;
}

float Segment(vec2 p, vec2 a, vec2 b, float w) {
	float f = 0.;
	vec2 ba = b - a;
	vec2 pa = p - a;
	float proj = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
	float d = length(proj * ba - pa);
	f = smoothstep(w, 0.95 * w, d);
	return f;
}

float Func(float x) {
	float T = 3. + sin(TIME);
	return sin(2. * PI / T * x);
}

float Func04(float x) {
	return smoothstep(0.5, 0., x);
}

float Func05(float x) {
	return floor(mod(x, 2.));
}

float FuncPlot(vec2 uv) {
	float f = Func(uv.x);
	return smoothstep(f - .01, f + .01, uv.y);
}

#define AA 4

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec3 color = Grid(uv);
	float seg = Segment(uv, vec2(1., 2.), vec2(-2., -2.), fwidth(uv.x));
	float sin_line = 0.;
	for (int m = 0; m < AA; m++) {
		for (int n = 0; n < AA; n++) {
			vec2 offset = (vec2(float(m), float(n)) - .5 * float(AA)) / float(AA) * 2.;
			sin_line += FuncPlot(FixUV(FRAGCOORD.xy + offset));
		}
	}
	if (sin_line > float(AA * AA) / 2.)
		sin_line = float(AA * AA) - sin_line;
	sin_line = sin_line * 2. / float(AA * AA);
	color = mix(color, vec3(1., 1., 0.), seg);
	color = mix(color, vec3(1., 1., 0.), sin_line);
	COLOR = vec4(color, 1.);
}
