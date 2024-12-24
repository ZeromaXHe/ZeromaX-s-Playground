// Shadertoy教程07——Ray Marching
// https://www.bilibili.com/video/BV18q4y1271t
shader_type canvas_item;

#define TMIN 0.1
#define TMAX 20.
#define RAYMARCH_TIME 120
#define PRECISION .001
#define AA 3

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	return (2. * c - iRes) / min(iRes.x, iRes.y);
}

float SdfSphere(vec3 p) {
	return length(p - vec3(0., 0., 2.)) - 1.5;
}

float RayMatch(vec3 ro, vec3 rd) {
	float t = TMIN;
	for (int i = 0; i < RAYMARCH_TIME && t < TMAX; i++) {
		vec3 p = ro + t * rd;
		float d = SdfSphere(p);
		if (d < PRECISION) {
			break;
		}
		t += d;
	}
	return t;
}

vec3 CalcNormal(vec3 p) {
	const float h = 0.0001;
	const vec2 k = vec2(1, -1);
	return normalize(k.xyy * SdfSphere(p + k.xyy * h)
		+ k.yyx * SdfSphere(p + k.yyx * h)
		+ k.yxy * SdfSphere(p + k.yxy * h)
		+ k.xxx * SdfSphere(p + k.xxx * h));
}

vec3 Render(vec2 uv) {
	vec3 color = vec3(0.);
	vec3 ro = vec3(0., 0., -1.5);
	vec3 rd = normalize(vec3(uv, 0.) - ro);
	float t = RayMatch(ro, rd);
	if (t < TMAX) {
		vec3 p = ro + t * rd;
		vec3 n = CalcNormal(p);
		vec3 light = vec3(2. * cos(TIME), 1., 2. * sin(TIME) + 2.);
		float dif = clamp(dot(normalize(light - p), n), 0., 1.);
		float amb = 0.5 + 0.5 * dot(n, vec3(0., 1., 0.)); // 环境光
		color = amb * vec3(0.25, 0.23, 0.23) + dif * vec3(1.);
	}
	return sqrt(color); // gamma 矫正
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec3 color = vec3(0.);
	for (int m = 0; m < AA; m++) {
		for (int n = 0; n < AA; n++) {
			vec2 offset = 2. * (vec2(float(m), float(n)) / float(AA) - 0.5);
			vec2 uv = FixUV(FRAGCOORD.xy + offset);
			color += Render(uv);
		}
	}
	COLOR = vec4(color / float(AA * AA), 1.);
}
