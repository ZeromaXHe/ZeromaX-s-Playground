// Shadertoy教程18——2D分形的绘制
// https://www.bilibili.com/video/BV12t4y1b79q
shader_type canvas_item;

varying vec2 iRes;

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

vec2 Cdiv(vec2 a, vec2 b) {
	float d = dot(b, b);
	return vec2(dot(a, b), a.y * b.x - a.x * b.y) / d;
}

vec2 Cpow(vec2 a, float n) {
	float l = length(a);
	float an = atan(a.y, a.x);
	return vec2(cos(an * n), sin(an * n)) * pow(l, n);
}

vec2 Cmul(vec2 a, vec2 b) {
	return vec2(a.x * b.x - a.y * b.y, a.y * b.x + a.x * b.y);
}

vec2 Integral(vec2 x) {
	x /= 2.;
	return floor(x) + max(2. * fract(x) - 1., 0.);
}

float Checkerboard(vec2 p) {
	vec2 fw = fwidth(p);
	vec2 i = Integral(p + 0.5 * fw) - Integral(p - 0.5 * fw);
	i /= fw;
	return i.x + i.y - 2. * i.x * i.y;
}

float Grid(vec2 p) {
	vec2 q = p * 16.;
	return 0.5 + 0.5 * Checkerboard(q);
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = FixUV(FRAGCOORD.xy);
	vec3 col = vec3(1.);
	vec2 z = uv;
	vec2 c = vec2(0.28, -0.49);
	float t = 1.;
	vec2 dz = vec2(1., 0.);
	vec2 phi = z;
	for (; t < 512.; t++) {
		if (length(z) > 1024.) break;
		dz = Cmul(z, dz) * 2.;
		//dz = Cmul(z, dz) * 2. + 1.;
		z = Cmul(z, z) + c;
		float n = pow(0.5, t);
		vec2 c = Cdiv(z, z - c);
		phi = Cmul(phi, Cpow(c, n));
	}
	if (t > 511.) {
		col = vec3(0.7, 0.4, 0.1);
	} else {
		float d = length(z) * log(length(z)) / length(dz);
		col *= Grid(phi);
		col *= smoothstep(0., 0.01, d);
		//col *= 0.7 + 0.2 * sin(120. * d); // 等高线
	}
	COLOR = vec4(0.5 * col, 1.);
	
	vec3 col2 = vec3(1.);
	vec2 z2 = vec2(0.);
	vec2 c2 = uv;
	float t2 = 1.;
	vec2 dz2 = vec2(1., 0.);
	vec2 phi2 = z2;
	for (; t2 < 512.; t2++) {
		if (length(z2) > 1024.) break;
		dz2 = Cmul(z2, dz2) * 2. + 1.;
		z2 = Cmul(z2, z2) + c2;
		float n2 = pow(0.5, t2);
		vec2 c2 = Cdiv(z2, z2 - c2);
		phi2 = Cmul(phi2, Cpow(c2, n2));
	}
	if (t2 > 511.) {
		col2 = vec3(0.7, 0.4, 0.1);
	} else {
		float d2 = length(z2) * log(length(z2)) / length(dz2);
		col2 *= smoothstep(0., 0.001, d2);
		col2 *= 0.7 + 0.2 * sin(120. * d2); // 等高线
	}
	COLOR += vec4(0.5 * col2, 1.);
}
