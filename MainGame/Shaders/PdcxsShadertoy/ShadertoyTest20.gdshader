// Shadertoy教程20——地面渲染技巧
// https://www.bilibili.com/video/BV1ud4y1s7c1
// Shadertoy教程21——制作天空
// https://www.bilibili.com/video/BV1UY411q7tE
shader_type canvas_item;

#define EPSILON 0.001
#define MAX_DIST 200.

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float Ground(vec2 p) {
	return sin(p.x) * sin(p.y);
}

float RayMatch(vec3 ro, vec3 rd) {
	float t = 0.;
	for (int i = 0; i < 128; i++) {
		vec3 p = ro + t * rd;
		float h = p.y - Ground(p.xz); // 取巧的简便写法，直接取垂直距离，不准确但够用
		if (abs(h) < EPSILON || t > MAX_DIST) break;
		t += h;
	}
	return t;
}

vec3 CalcNorm(vec3 p) {
	vec2 epsilon = vec2(1e-5, 0);
	return normalize(vec3(Ground(p.xz + epsilon.xy) - Ground(p.xz - epsilon.xy),
		2.0 * epsilon.x,
		Ground(p.xz + epsilon.yx) - Ground(p.xz - epsilon.yx)
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
	float an = 0.2 * TIME; // 摄像机绕行圆心角度
	float r = 10.; // 摄像机绕行半径
	vec3 ro = vec3(r * sin(an), 5, r * cos(an));
	vec3 target = vec3(-4, 3, -7);
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
	col = sqrt(col); // gamma 校正
	float t = RayMatch(ro, rd);
	if (t < MAX_DIST) {
		vec3 p = ro + t * rd;
		vec3 n = CalcNorm(p);
		vec3 difColor = vec3(0.9, 0.8, 0.);
		col = 0.2 + 0.8 * difColor * dot(n, vec3(0, 1, 0));
	}
	return col;
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec3 col = Render(uv);
	COLOR = vec4(col, 1.);
}
