// Shadertoy教程03——线段绘制、clamp与mix函数: 
// https://www.bilibili.com/video/BV1oq4y1271G
// Shadertoy教程04——smoothstep函数：
// https://www.bilibili.com/video/BV1EF411B7Mo
shader_type canvas_item;

vec2 FixUV(vec2 c, vec2 iResolution) {
	return 3. * (2. * c - iResolution) / min(iResolution.x, iResolution.y);
}

vec3 Grid(vec2 uv) {
	vec3 color = vec3(0.);
	vec2 fraction = 1. - 2. * abs(fract(uv) - 0.5);
	color = vec3(smoothstep(4. * fwidth(uv.x), 3.9 * fwidth(uv.x), fraction.x));
	color += vec3(smoothstep(4. * fwidth(uv.y), 3.9 * fwidth(uv.y), fraction.y));
	color.rb *= smoothstep(1.9 * fwidth(uv.x), 2.0 * fwidth(uv.x), abs(uv.x));
	color.gb *= smoothstep(1.9 * fwidth(uv.y), 2.0 * fwidth(uv.y), abs(uv.y));
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

float FuncPlot(vec2 uv, vec2 iRes) {
	float f = 0.;
	for (float x = 0.; x <= iRes.x; x += 1.) {
		float fx = FixUV(vec2(x, 0.), iRes).x;
		float nfx = FixUV(vec2(x + 1., 0.), iRes).x;
		f += Segment(uv, vec2(fx, Func04(fx)), vec2(nfx, Func04(nfx)), fwidth(uv.x));
	}
	return clamp(f, 0., 1.);
}

void fragment() {
	vec2 iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy, iRes);
	vec3 color = Grid(uv);
	float seg = Segment(uv, vec2(1., 2.), vec2(-2., -2.), fwidth(uv.x));
	float sinLine = FuncPlot(uv, iRes);
	color = mix(color, vec3(1., 1., 0.), seg);
	color = mix(color, vec3(1., 1., 0.), sinLine);
	COLOR = vec4(color, 1.);
}
