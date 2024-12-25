// Shadertoy教程
// 20——地面渲染技巧: https://www.bilibili.com/video/BV1ud4y1s7c1
// 21——制作天空: https://www.bilibili.com/video/BV1UY411q7tE
// 22——随机地形生成方法: https://www.bilibili.com/video/BV1Mo4y1t72X
// 23——制作山体: https://www.bilibili.com/video/BV18P411i7PF
// 24——山体形状与性能优化: https://www.bilibili.com/video/BV1nX4y1W7Lj
// 25——雾气与户外光照: https://www.bilibili.com/video/BV1xN41167Wv
// 26——Fractal Brownian Motion: https://www.bilibili.com/video/BV1Vp4y1w7A9
shader_type canvas_item;

#define EPSILON 0.001
#define MAX_DIST 300.
#define MAX_ITER 128

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float Hash12(vec2 p) {
	// 少量对接不上的接缝
	vec3 p3 = fract(vec3(p.xyx) * .1031);
	p3 += dot(p3, p3.yzx + 33.33);
	return fract((p3.x + p3.y) * p3.z); 
	// 用下面这个会大量撕裂的黑色斑点接缝
	//vec2 p2 = 55.1876653 * fract(p * 10.1321513);
	//return fract((p2.x + p2.y) * p2.x * p2.y);
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
	vec2 p = 0.003 * x;
	float a = 0.;
	float b = 1.;
	vec2 d = vec2(0);
	for (int i = 0; i < 8; i++) { // 多层噪声叠加
		vec3 n = Noise(p);
		d += n.yz;
		a += b * n.x / (1. + dot(d, d));
		p = mat * p * 2.;
		b *= 0.5; // 噪声因子每次缩小一半
	}
	return 120. * a;
}

float GroundH(vec2 x) {
	vec2 p = 0.003 * x;
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
	return 120. * a;
}

float GroundL(vec2 x) {
	vec2 p = 0.003 * x;
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
	return 120. * a;
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

float SoftShadow(vec3 ro, vec3 rd, float dis) {
	float minStep = clamp(0.01 * dis, 0.5, 50.0);
	float res = 1.0;
	float t = 0.001;
	for (int i = 0; i < 80; i++){
		vec3 p = ro + t * rd;
		float h = p.y - Ground(p.xz);
		res = min(res, 8.0 * h / t);
		t += max(minStep, h);
		if (res < 0.001 || p.y > 200.0) break;
	}
	return clamp(res, 0.0, 1.0);
}

vec3 CalcNorm(vec3 p, float t) {
	vec2 epsilon = vec2(0.001 * t, 0);
	return normalize(vec3(GroundH(p.xz - epsilon.xy) - GroundH(p.xz + epsilon.xy),
		2.0 * epsilon.x,
		GroundH(p.xz - epsilon.yx) - GroundH(p.xz + epsilon.yx)
		));
}

mat3 SetCamera(vec3 ro, vec3 target, float cr) {
	vec3 z = normalize(target - ro);
	vec3 up = normalize(vec3(sin(cr), cos(cr), 0));
	vec3 x = cross(z, up);
	vec3 y = cross(x, z);
	return mat3(x, y, z);
}

float Fbm(vec2 p) {
	float f = 0.;
	float fac = 0.5;
	for (int i = 0; i < 4; i++) {
		f += fac * Noise(p).x;
		p = 2. * mat * p;
		fac *= 0.5;
	}
	return f;
}

vec3 Render(vec2 uv) {
	float an = 0.05 * TIME; // 摄像机绕行圆心角度
	float r = 300.0; // 摄像机绕行半径
	vec2 pos2d = vec2(r * sin(an), r * cos(an)); // 平面坐标
	float h = GroundL(pos2d) + 25.;

	vec3 ro = vec3(pos2d.x, h, pos2d.y);
	vec3 target = vec3(r * sin(an + 0.01), h, r * cos(an + 0.01));
	mat3 cam = SetCamera(ro, target, 0.);
	float fl = 1.;
	vec3 rd = normalize(cam * vec3(uv, fl));
	vec3 col = vec3(0);
	float tmin = 0.01;
	float tmax = 1000.;
	float maxh = 300.;
	float tp = (maxh - ro.y) / rd.y;
	if (tp > 0.) {
		if (maxh > rd.y) {
			tmax = min(tp, tmax);
		} else {
			tmin = max(tp, tmin);
		}
	}
	float t = RayMatch(ro, rd, tmin, tmax);
	// 太阳
	vec3 sunLight = normalize(vec3(0.8, 0.5, -0.2));
	float sunDot = clamp(dot(rd, sunLight), 0.0, 1.0);
	if (t > tmax) {
		// 天空蓝色，y 轴越高越深（二次方渐变）
		col = vec3(0.3, 0.5, 0.85) - rd.y * rd.y * 0.5;
		// 地平线的淡灰色渐变
		col = mix(col, vec3(0.7, 0.75, 0.85), pow(1.0 - max(rd.y, 0.0), 4.0));
		// 太阳三层光圈
		col += 0.25 * vec3(1.0, 0.7, 0.4) * pow(sunDot, 5.0);
		col += 0.25 * vec3(1.0, 0.8, 0.6) * pow(sunDot, 64.0);
		col += 0.2 * vec3(1.0, 0.8, 0.6) * pow(sunDot, 512.0);
		// 地平线以下的深蓝
		col = mix(col, 0.68 * vec3(0.4, 0.65, 1.0), pow(1.0 - max(rd.y, 0.0), 16.0));
		// 云朵
		float cloudHeight = 200.;
		vec2 cloudOffset = (cloudHeight - ro.y) / rd.y * rd.xz;
		vec2 cloudUV = ro.xz + cloudOffset + TIME * 5.;
		// 远处的云锯齿明显，限制一下
		if (length(cloudOffset) < tmax) {
			col = mix(col, vec3(1., 0.95, 1.),
				0.5 * smoothstep(0.4, 0.8, Fbm(cloudUV * 0.01)));
		}
	} else {
		vec3 p = ro + t * rd;
		vec3 n = CalcNorm(p, t);
		vec3 difColor = vec3(0.67, 0.57, 0.44);
		col = 0.1 * difColor; // 漫反射作为基础颜色
		vec3 lin = vec3(0.); // 线性空间
		float dif = clamp(dot(sunLight, n), 0., 1.);
		float sh = SoftShadow(p + 0.01 * sunLight, sunLight, t); // 阴影
		float amb = clamp(0.5 + 0.5 * n.y, 0., 1.); // 环境光遮罩
		float bac = clamp(0.2 + 0.8 * dot(vec3(-sunLight.x, 0., sunLight.z), n),
			0., 1.); // 背景光照
		lin += dif * vec3(8.0, 5.0, 3.0) * 1.3
			* vec3(sh, sh * sh * 0.5 + 0.5 * sh, sh * sh * 0.8 + 0.2 * sh);
		lin += amb * vec3(0.4, 0.6, 1.) * 5.2;
		lin += bac * vec3(0.4, 0.5, 0.6) * 1.5;
		col *= lin;
		// 雾效果
		col = mix(col, 0.65 * vec3(0.5, 0.75, 1.0), 1. - exp(-pow(0.002 * t, 1.5)));
	}
	col += 0.3 * vec3(1.0, 0.7, 0.3) * pow(sunDot, 8.0); // 太阳光晕
	//return sqrt(col); // gamma 校正
	return col;
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec3 col = Render(uv);
	COLOR = vec4(col, 1.);
}
