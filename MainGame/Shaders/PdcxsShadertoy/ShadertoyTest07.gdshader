// Shadertoy教程07——Ray Marching
// https://www.bilibili.com/video/BV18q4y1271t
// Shadertoy教程08——3D摄像机设置
// https://www.bilibili.com/video/BV1PS4y1j7Xg
// Shadertoy教程09——矩形的SDF函数
// https://www.bilibili.com/video/BV1zS4y157de
// Shadertoy教程10——多物体与阴影
// https://www.bilibili.com/video/BV1HL411F74d
shader_type canvas_item;

#define TMIN 0.1
#define TMAX 100.
#define RAYMARCH_TIME 120
#define PRECISION .001
#define AA 3

global uniform vec3 iMouse;

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float SdfSphere(vec3 p) {
	return length(p) - 1.;
}

float SdfRect(vec3 p, vec3 b) {
	vec3 d = abs(p) - b;
	return length(max(d, 0.)) - min(max(d.x, max(d.y, d.z)), 0.);
}

float SdfPlane(vec3 p) {
	return p.y;
}

float Map(vec3 p) {
	float d = SdfSphere(p);
	d = min(d, SdfPlane(p + vec3(0., 1., 0.)));
	return d;
}

float RayMatch(vec3 ro, vec3 rd) {
	float t = TMIN;
	for (int i = 0; i < RAYMARCH_TIME && t < TMAX; i++) {
		vec3 p = ro + t * rd;
		float d = Map(p);
		if (d < PRECISION) {
			break;
		}
		t += d;
	}
	return t;
}

// https://iquilezles.org/articles/normalsSDF
vec3 CalcNormal(vec3 p) {
	const float h = 0.0001;
	const vec2 k = vec2(1, -1);
	return normalize(k.xyy * Map(p + k.xyy * h)
		+ k.yyx * Map(p + k.yyx * h)
		+ k.yxy * Map(p + k.yxy * h)
		+ k.xxx * Map(p + k.xxx * h));
}

mat3 SetCamera(vec3 ta, vec3 ro, float cr) {
	vec3 z = normalize(ta - ro);
	vec3 cp = vec3(sin(cr), cos(cr), 0.);
	vec3 x = normalize(cross(z, cp));
	vec3 y = cross(x, z);
	return mat3(x, y, z);
}

// https://iquilezles.org/articles/rmshadows
float SoftShadow(vec3 ro, vec3 rd, float k) {
	float res = 1.0;
	float ph = 1e20;
	for (float t = TMIN; t < TMAX;) {
		// 这里循环得小心，尽量别在使用中修改。
		// 写到一半时假如被 Godot 编译成了死循环，就会直接卡死…… 坑啊！
		float h = Map(ro + rd * t);
		if (h < .001) 
			return 0.0;
		float y = h * h / (2.0 * ph);
		float d = sqrt(h * h - y * y);
		res = min(res, k * d / max(0.0, t - y));
		ph = h;
		t += h;
	}
	return res;
}

vec3 Render(vec2 uv) {
	vec3 color = vec3(0.);
	vec3 ro = vec3(4. * cos(TIME), 1., 4. * sin(TIME));
	if (iMouse.z > 0.01) {
		float theta = iMouse.x / iRes.x * 2. * PI;
		ro = vec3(4. * cos(theta), 1., 4. * sin(theta));
	}
	vec3 ta = vec3(0.);
	mat3 cam = SetCamera(ta, ro, 0.);
	vec3 rd = normalize(cam * vec3(uv, 1.));
	float t = RayMatch(ro, rd);
	if (t < TMAX) {
		vec3 p = ro + t * rd;
		vec3 n = CalcNormal(p);
		vec3 light = vec3(2., 3., 0.);// vec3(2. * cos(TIME), 1., 2. * sin(TIME) + 2.);
		float dif = clamp(dot(normalize(light - p), n), 0., 1.);
		p += PRECISION * n;
		float st = SoftShadow(p, normalize(light - p), 10.);
		dif *= st;
		float amb = 0.5 + 0.5 * dot(n, vec3(0., 1., 0.)); // 环境光
		color = amb * vec3(0.23) + dif * vec3(1.);
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
