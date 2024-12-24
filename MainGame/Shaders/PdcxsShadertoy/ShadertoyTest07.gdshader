// Shadertoy教程
// 07——Ray Marching：https://www.bilibili.com/video/BV18q4y1271t
// 08——3D摄像机设置：https://www.bilibili.com/video/BV1PS4y1j7Xg
// 09——矩形的SDF函数： https://www.bilibili.com/video/BV1zS4y157de
// 10——多物体与阴影： https://www.bilibili.com/video/BV1HL411F74d
// 12——多物体着色与棋盘贴图： https://www.bilibili.com/video/BV1bm4y1f7B5
// 13——制作一个B站小电视： https://www.bilibili.com/video/BV1kY41147MB
// 14——布尔运算： https://www.bilibili.com/video/BV1pL4y1u7Cz
// 15——Smooth Minimum： https://www.bilibili.com/video/BV1CB4y1m7V8
// 16——环境光遮蔽： https://www.bilibili.com/video/BV1KL4y1c79A
shader_type canvas_item;

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

float SdfBox(vec3 p, vec3 b, float rad) {
	vec3 d = abs(p) - b;
	return length(max(d, 0.)) + min(max(d.x, max(d.y, d.z)), 0.) - rad;
}

// 圆环
float SdTorus(vec3 p, vec2 t) {
	vec2 q = vec2(length(p.xz) - t.x, p.y);
	return length(q) - t.y;
}

float SdCapsule(vec3 p, vec3 a, vec3 b, float r) {
	vec3 pa = p - a, ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * h) - r;
}

float SdCappedTorus(vec3 p, vec2 sc, float ra, float rb) {
	p.x = abs(p.x);
	float k = (sc.y * p.x > sc.x * p.y) ? dot(p.xy, sc) : length(p.xy);
	return sqrt(dot(p, p) + ra * ra - 2.0 * ra * k) - rb;
}

float SdfPlane(vec3 p) {
	return p.y;
}

float Shape1(vec3 p) {
	// union
	float d = SdfSphere(p);
	d = min(d, SdfBox(p - vec3(0., 1., 0.), vec3(.4), 0.)); // 并集
	return d;
}

float Shape2(vec3 p) {
	// intersection
	float d = SdfSphere(p);
	d = max(d, SdfBox(p - vec3(0., 1., 0.), vec3(.4, 2., .4), 0.)); // 交集
	return d;
}

float Shape3(vec3 p) {
	// difference
	float d = SdfSphere(p);
	d = max(d, -1. * SdfBox(p - vec3(0., 0., 0.), vec3(2., .4, .4), 0.)); // 求差
	return d;
}

vec4 OpElongate(vec3 p, vec3 h) {
	// return vec4(p - clamp(p, -h, h), 0.0); // faster, but produces zero in the interior elongation
	vec3 q = abs(p) - h;
	return vec4(max(q, 0.0), min(max(q.x, max(q.y, q.z)), 0.0));
}

// 用于圆滑连接边界
// https://iquilezles.org/articles/smin
float Smin(float a, float b, float k) {
	float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
	return mix(b, a, h) - k * h * (1.0 - h);
}

vec2 OpU(vec2 a, vec2 b) {
	// 用 Smin 的话，隔空的物体之间有点搞笑，哈哈哈~
	//float smin = Smin(a.x, b.x, 1. + 0.5 * sin(TIME));
	//if (a.x < b.x) {
		//a.x = smin;
		//return a;
	//} else {
		//b.x = smin;
		//return b;
	//}
	return a.x < b.x ? a: b;
}

vec2 Map(vec3 p) {
	p.xz *= mat2(vec2(0., 1.), vec2(1., 0.)); // 逆时针旋转 90 度
	vec2 d = vec2(SdfBox(p - vec3(0., 1.3, 0.), vec3(1.5, .9, 1.), .3), 4.);
	{
		// 小电视框
		vec4 w = OpElongate(p.xzy - vec3(0., 1.3, 1.3), vec3(1.2, 0., 0.55));
		float t = w.w + SdTorus(w.xyz, vec2(.3, .05));
		d = OpU(d, vec2(t, 5.));
	}
	{
		// 眼睛
		d = OpU(d, vec2(SdCapsule(p, vec3(.4, 1.6, 1.3), vec3(1., 1.3, 1.3), .1), 5.));
		d = OpU(d, vec2(SdCapsule(p, vec3(-.4, 1.6, 1.3), vec3(-1., 1.3, 1.3), .1), 5.));
	}
	{
		// 嘴巴
		float an = 70. / 180. * PI;
		d = OpU(d, vec2(SdCappedTorus(p * vec3(1., -1., 1.) - vec3(.3, -1., 1.3), vec2(sin(an), cos(an)), .3, .07), 5.));
		d = OpU(d, vec2(SdCappedTorus(p * vec3(1., -1., 1.) - vec3(-.3, -1., 1.3), vec2(sin(an), cos(an)), .3, .07), 5.));
	}
	{
		// 天线
		d = OpU(d, vec2(SdCapsule(p, vec3(1.3, 3.0, 1.), vec3(.2, 2.0, 1.), .1), 5.));
		d = OpU(d, vec2(SdCapsule(p, vec3(-1.3, 3.0, 1.), vec3(-.2, 2.0, 1.), .1), 5.));
	}
	{
		// 脚
		d = OpU(d, vec2(SdCapsule(vec3(abs(p.x), p.y, abs(p.z)), vec3(1.3, .5, 1.), vec3(1.5, .1, 1.3), .1), 5.));
	}
	d = OpU(d, vec2(Shape1(p - vec3(-2.5, 3., 1.)), 2.));
	d = OpU(d, vec2(Shape2(p - vec3(1., 3.5, -2.5)), 3.));
	d = OpU(d, vec2(Shape3(p - vec3(2., 2.5, 2.5)), 6.));
	return d;
}

vec2 RayMatch(vec3 ro, vec3 rd) {
	float t = 0.1;
	float tmax = 40.;
	vec2 res = vec2(-1.);
	if (rd.y < 0.) {
		float tp = -ro.y / rd.y;
		tmax = min(tmax, tp);
		res = vec2(tp, 1.);
	}
	for (int i = 0; i < RAYMARCH_TIME && t < tmax; i++) {
		vec3 p = ro + t * rd;
		vec2 d = Map(p);
		if (d.x < PRECISION) {
			res = vec2(t, d.y);
			break;
		}
		t += d.x;
	}
	return res;
}

// https://iquilezles.org/articles/normalsSDF
vec3 CalcNormal(vec3 p) {
	const float h = 0.0001;
	const vec2 k = vec2(1, -1);
	return normalize(k.xyy * Map(p + k.xyy * h).x
		+ k.yyx * Map(p + k.yyx * h).x
		+ k.yxy * Map(p + k.yxy * h).x
		+ k.xxx * Map(p + k.xxx * h).x);
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
	float tmin = .1;
	float tmax = 10.;
	for (float t = tmin; t < tmax;) {
		// 这里循环得小心，尽量别在使用中修改。
		// 写到一半时假如被 Godot 编译成了死循环，就会直接卡死…… 坑啊！
		float h = Map(ro + rd * t).x;
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

// triangular signal
vec2 Tri(vec2 x) {
	vec2 h = fract(x * .5) - .5;
	return 1. - 2. * abs(h);
}

// https://iquilezles.org/articles/checkerfiltering
float CheckersGrad(vec2 uv, vec2 ddx, vec2 ddy) {
	vec2 w = max(abs(ddx), abs(ddy)) + 0.01; // filter kernel
	vec2 i = (Tri(uv + 0.5 * w) - Tri(uv - 0.5 * w)) / w; // analytical integral (box filter)
	return 0.5 - 0.5 * i.x * i.y; // xor pattern
}

// n: 法向量
// https://iquilezles.org/articles/nvscene2008/rwwtt.pdf
float CalcAO(vec3 p, vec3 n) {
	float occ = 0.0;
	float sca = 1.0;
	for (int i = 0; i < 5; i++) {
		float h = 0.01 + 0.03 * float(i);
		float d = Map(p + n * h).x;
		occ += (h - d) * sca;
		sca *= 0.95;
		if (occ > 0.35)
			break;
	}
	return clamp(1. - 3. * occ, 0., 1.);
}

vec3 Render(vec2 uv, vec2 px, vec2 py) {
	vec3 camLoc = vec3(4. * cos(.1 * TIME), 4. + .5 * sin(.3 * TIME), 4. * sin(.1 * TIME));
	if (iMouse.z > 0.01) {
		float theta = iMouse.x / iRes.x * 2. * PI;
		camLoc = vec3(4. * cos(theta), 2., 4. * sin(theta));
	}
	vec3 target = vec3(0., 1., 0.); // 摄像机目标
	mat3 camMat = SetCamera(target, camLoc, 0.);
	float fl = 1.; // 焦距
	vec3 rd = normalize(camMat * vec3(uv, fl));
	vec3 bg = vec3(.7, .7, .9); // 背景颜色
	vec3 color = bg - rd.y * vec3(.1);
	vec2 t = RayMatch(camLoc, rd);
	if (t.y > 0.) {
		vec3 p = camLoc + t.x * rd;
		vec3 n = t.y < 1.1 ? vec3(0., 1., 0.) /*平面的情况*/ : CalcNormal(p);
		vec3 lightLoc = vec3(3. + 3. * cos(TIME), 10., 2.5 + 2.5 * sin(TIME));
		vec3 lightColor = vec3(.7);
		float dif = clamp(dot(normalize(lightLoc - p), n), 0., 1.);
		p += PRECISION * n;
		float shadow = SoftShadow(p, normalize(lightLoc - p), 10.);
		vec3 ambDirect = vec3(0., 1., 0.);
		float amb = 0.5 + 0.5 * dot(n, normalize(ambDirect)); // 环境光
		vec3 ambColor = vec3(0.);
		if (t.y > 1.9 && t.y < 2.1) {
			// 2 红色
			ambColor = vec3(1., 0., 0.);
		} else if (t.y > 2.9 && t.y < 3.1) {
			// 3 黄色
			ambColor = vec3(1., 1., 0.);
		} else if (t.y > 5.9 && t.y < 6.1) {
			// 6 青色
			ambColor = vec3(0., 1., 1.);
		} else if (t.y > 3.9 && t.y < 4.1) {
			// 4 小电视主体盒子
			ambColor = vec3(1.);
		} else if (t.y > 4.9 && t.y < 5.1) {
			// 5 小电视黑色组件
			ambColor = vec3(0.);
		} else if (t.y > 0.9 && t.y < 1.1) {
			// 1 平面
			vec3 rdx = normalize(camMat * vec3(px, fl));
			vec3 rdy = normalize(camMat * vec3(py, fl));
			vec3 ddx = camLoc.y * (rd/rd.y - rdx/rdx.y);
			vec3 ddy = camLoc.y * (rd/rd.y - rdy/rdy.y);
			// 棋盘纹理
			ambColor = vec3(.1) + 0.1 * CheckersGrad(p.xz, ddx.xz, ddy.xz);
		}
		color = amb * ambColor + dif * lightColor * shadow;
		// 计算环境光遮蔽
		float ao = CalcAO(p, n);
		color *= ao;
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
			vec2 px = FixUV(FRAGCOORD.xy + vec2(1., 0.) + offset);
			vec2 py = FixUV(FRAGCOORD.xy + vec2(0., 1.) + offset);
			color += Render(uv, px, py);
		}
	}
	COLOR = vec4(color / float(AA * AA), 1.);
}
