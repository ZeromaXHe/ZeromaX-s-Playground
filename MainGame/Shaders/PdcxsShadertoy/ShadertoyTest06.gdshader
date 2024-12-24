// Shadertoy教程06——2D Signed Distance Function
// https://www.bilibili.com/video/BV1Ci4y1X7EG
// Shadertoy教程09——矩形的SDF函数
// https://www.bilibili.com/video/BV1zS4y157de
shader_type canvas_item;

#define r .5 + .2 * sin(TIME)

global uniform vec3 iMouse;

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	return 2. * (2. * c - iRes) / min(iRes.x, iRes.y);
}

float SdfCircle(vec2 p) {
	return length(p) - r;
}

float SdfRect(vec2 p, vec2 b) {
	vec2 d = abs(p) - b;
	return length(max(d, 0.)) + min(max(d.x, d.y), 0.);
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec2 b = vec2(1. + .2 * sin(TIME), 1. + .3 * cos(TIME));
	float d = SdfRect(uv, b);
	vec3 color = 1. - sign(d) * vec3(.2, .33, .46);
	color *= 1. - exp(-3. * abs(d)); // 黑圈边缘
	color *= .8 + .2 * sin(150. * abs(d)); // 等高线
	color = mix(color, vec3(1.), smoothstep(.005, .004, abs(d)));
	
	if (iMouse.z > 0.1) {
		vec2 m = FixUV(iMouse.xy);
		float currentDistance = abs(SdfRect(m, b));
		color = mix(color, vec3(1., 1., 0.),
			smoothstep(0.01, 0., abs(length(uv - m) - currentDistance)));
		color = mix(color, vec3(1., 1., 0.),
			smoothstep(.02, .01, length(uv - m)));
	}
	
	COLOR = vec4(color, 1.);
}
