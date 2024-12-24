// Shadertoy教程17——贴图
// https://www.bilibili.com/video/BV1T5411X7dR
shader_type canvas_item;

global uniform vec3 iMouse;
// 默认 repeat_disable： 等于 shadertoy 的 clamp 模式
uniform sampler2D iChannel0: repeat_enable;

varying vec2 iRes;

#define TMIN 0.1
#define TMAX 20.
#define RAYMARCH_TIME 128
#define PRECISION .001
#define AA 3
#define PI 3.14159265

vec2 FixUV(vec2 c) {
	vec2 uv = (2. * c - iRes) / min(iRes.x, iRes.y);
	uv.y *= -1.; // 如果在 Godot 中使用 Shadertoy/GLSL 的 UV, 则 y 坐标将颠倒翻转.
	return uv;
}

float sdfRect(in vec3 p, vec3 b) {
	vec3 d = abs(p) - b;
	return length(max(d, 0.)) + min(max(max(d.x, d.y),d.z), 0.);
}

float map(in vec3 p) {
	float d = sdfRect(p, vec3(.7));
	d = length(p) - 1.;
	return d;
}

float rayMarch(in vec3 ro, in vec3 rd) {
	float t = TMIN;
	for(int i = 0; i < RAYMARCH_TIME && t < TMAX; i++) {
		vec3 p = ro + t * rd;
		float d = map(p);
		if(d < PRECISION)
			break;
		t += d;
	}
	return t;
}

// https://iquilezles.org/articles/normalsSDF
vec3 calcNormal(in vec3 p) {
	const float h = 0.0001;
	const vec2 k = vec2(1, -1);
	return normalize(k.xyy * map(p + k.xyy * h) +
		k.yyx * map(p + k.yyx * h) +
		k.yxy * map(p + k.yxy * h) +
		k.xxx * map(p + k.xxx * h));
}

mat3 setCamera(vec3 ta, vec3 ro, float cr) {
	vec3 z = normalize(ta - ro);
	vec3 cp = vec3(sin(cr), cos(cr), 0.);
	vec3 x = normalize(cross(z, cp));
	vec3 y = cross(x, z);
	return mat3(x, y, z);
}

vec3 render(vec2 uv) {
	vec3 color = vec3(0.);
	vec3 ro = vec3(2. * cos(TIME), 1., 2. * sin(TIME));
	if (iMouse.z > 0.01) {
		float theta = iMouse.x / iRes.x * 2. * PI;
		ro = vec3(2. * cos(theta), 2. * (-2. * iMouse.y / iRes.y + 1.), 2. * sin(theta));
	}
	vec3 ta = vec3(0.);
	mat3 cam = setCamera(ta, ro, 0.);
	vec3 rd = normalize(cam * vec3(uv, 1.));
	float t = rayMarch(ro, rd);
	if (t < TMAX) {
		vec3 p = ro + t * rd;
		vec3 n = calcNormal(p);
		vec3 light = vec3(2., 1., 0.);
		float dif = clamp(dot(normalize(light - p), n), 0., 1.);
		float amb = 0.5 + 0.5 * dot(n, vec3(0., 1., 0.));

		// color = amb * vec3(.7) + dif * vec3(1.);
		vec3 colorXY = texture(iChannel0, p.xy * .5 + .5).rgb;
		vec3 colorXZ = texture(iChannel0, p.xz * .5 + .5).rgb;
		vec3 colorYZ = texture(iChannel0, p.yz * .5 + .5).rgb;
		n = abs(n);
		n = pow(n, vec3(10.)); // 使得边界清晰，次方越多越清晰
		n /= n.x + n.y + n.z; // 正则化，消除边界的黑色
		color = colorXY * n.z + colorXZ * n.y + colorYZ * n.x;
		//color = n;
	}
	return color;
}

void fragment() {
	iRes = 1.0 / SCREEN_PIXEL_SIZE;

	vec3 color = vec3(0.);
	for(int m = 0; m < AA; m++) {
		for(int n = 0; n < AA; n++) {
			vec2 offset = 2. * (vec2(float(m), float(n)) / float(AA) - .5);
			vec2 uv = FixUV(FRAGCOORD.xy + offset);
			color += render(uv);
		}
	}
	COLOR = vec4(color / float(AA * AA), 1.);
	
	if (COLOR.rgb == vec3(0.)) {
		vec2 uv = FRAGCOORD.xy / iRes;
		uv *= 3.;
		vec3 col = texture(iChannel0, uv).rgb;
		COLOR = 0.5 * vec4(col, 1.0);
	}
}
