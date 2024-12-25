// Shadertoy教程
// 20——地面渲染技巧: https://www.bilibili.com/video/BV1ud4y1s7c1
// 21——制作天空: https://www.bilibili.com/video/BV1UY411q7tE
// 22——随机地形生成方法: https://www.bilibili.com/video/BV1Mo4y1t72X
// 23——制作山体: https://www.bilibili.com/video/BV18P411i7PF
// 24——山体形状与性能优化: https://www.bilibili.com/video/BV1nX4y1W7Lj
shader_type canvas_item;

#define EPSILON 0.001
#define MAX_DIST 1000.
#define MAX_ITER 300

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float Hash12(vec2 p) {
	vec3 p3 = fract(vec3(p.xyx) * .1031);
	p3 += dot(p3, p3.yzx + 33.33);
	return fract((p3.x + p3.y) * p3.z);
}

float Random(vec2 pos) {
	return Hash12(pos);
	// 不知道为啥在 Godot 这种实现就会有突变？视频里正常
	//return abs(fract(78523.215 * sin(dot(pos, vec2(25.32, 547.23)))));
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

float Ground(vec2 x) {
	vec2 p = 0.03 * x;
	float a = 0.;
	float b = 1.;
	vec2 d = vec2(0);
	for (int i = 0; i < 8; i++) {
		vec3 n = Noise(p);
		d += n.yz;
		a += b * n.x / (1. + dot(d, d));
		p = mat * p * 2.;
		b *= 0.5;
	}
	return 20. * a;
}

float GroundH(vec2 x) {
	vec2 p = 0.03 * x;
	float a = 0.;
	float b = 1.;
	vec2 d = vec2(0);
	for (int i = 0; i < 16; i++) {
		vec3 n = Noise(p);
		d += n.yz;
		a += b * n.x / (1. + dot(d, d));
		p = mat * p * 2.;
		b *= 0.5;
	}
	return 20. * a;
}

float GroundL(vec2 x) {
	vec2 p = 0.03 * x;
	float a = 0.;
	float b = 1.;
	vec2 d = vec2(0);
	for (int i = 0; i < 3; i++) {
		vec3 n = Noise(p);
		d += n.yz;
		a += b * n.x / (1. + dot(d, d));
		p = mat * p * 2.;
		b *= 0.5;
	}
	return 20. * a;
}

float RayMatch(vec3 ro, vec3 rd, float tmin, float tmax) {
	float t = tmin;
	for (int i = 0; i < MAX_ITER; i++) {
		vec3 p = ro + t * rd;
		float h = p.y - Ground(p.xz); // 取巧的简便写法，直接取垂直距离，不准确但够用
		if (abs(h) < EPSILON * t || t > tmax) break;
		t += 0.4 * h;
	}
	return t;
}

vec3 CalcNorm(vec3 p, float t) {
	vec2 epsilon = vec2(0.001 * t, 0);
	return normalize(vec3(GroundH(p.xz + epsilon.xy) - GroundH(p.xz - epsilon.xy),
		2.0 * epsilon.x,
		GroundH(p.xz + epsilon.yx) - GroundH(p.xz - epsilon.yx)
		));
}

mat3 SetCamera(vec3 ro, vec3 target, float cr) {
	vec3 z = normalize(target - ro);
	vec3 up = normalize(vec3(sin(cr), cos(cr), 0));
	vec3 x = cross(z, up);
	vec3 y = cross(x, z);
	return mat3(x, y, z);
}

vec3 Render(vec2 uv) {
	float an = 0.001 * TIME; // 摄像机绕行圆心角度
	float r = 500.0; // 摄像机绕行半径
	vec2 pos2d = vec2(r * sin(an), r * cos(an)); // 平面坐标
	float h = GroundL(pos2d) + 10.;

	vec3 ro = vec3(pos2d.x, h, pos2d.y);
	vec3 target = vec3(r * sin(an + 0.01), h, r * cos(an + 0.01));
	mat3 cam = SetCamera(ro, target, 0.);
	float fl = 1.;
	vec3 rd = normalize(cam * vec3(uv, fl));
	// 天空蓝色，y 轴越高越深（二次方渐变）
	vec3 col = vec3(0.3, 0.5, 0.85) - rd.y * rd.y * 0.5;
	// 地平线的淡灰色渐变
	col = mix(col, 0.85 * vec3(0.7, 0.75, 0.85), pow(1.0 - max(rd.y, 0.0), 4.0));
	// 太阳
	vec3 sunLight = normalize(vec3(0., 0.4, -1.0));
	float sunDot = clamp(dot(rd, sunLight), 0.0, 1.0);
	col += 0.25 * vec3(1.0, 0.7, 0.4) * pow(sunDot, 5.0);
	col += 0.25 * vec3(1.0, 0.7, 0.4) * pow(sunDot, 64.0);
	col += 0.25 * vec3(1.0, 0.7, 0.4) * pow(sunDot, 512.0);
	// 地平线以下的深蓝
	col = mix(col, 0.68 * vec3(0.4, 0.65, 1.0), pow(1.0 - max(rd.y, 0.0), 16.0));
	float tmin = 0.01;
	float tmax = MAX_DIST;
	float maxh = 200.;
	float tp = (maxh - ro.y) / rd.y;
	if (tp > 0.) {
		if (rd.y > 0.) {
			tmax = min(tp, tmax);
		} else {
			tmin = max(tp, tmin);
		}
	}
	float t = RayMatch(ro, rd, tmin, tmax);
	if (t < tmax) {
		vec3 p = ro + t * rd;
		vec3 n = CalcNorm(p, t);
		vec3 difColor = vec3(0.67, 0.57, 0.44);
		col = difColor * dot(n, vec3(0, 1, 0));
	}
	return sqrt(col); // gamma 校正
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec3 col = Render(uv);
	COLOR = vec4(col, 1.);
}
