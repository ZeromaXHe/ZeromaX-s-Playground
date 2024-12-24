// Shadertoy教程02——坐标系统绘制与fwidth函数
// https://www.bilibili.com/video/BV1Xm4y1X7qU
shader_type canvas_item;

vec3 GetGridColor(vec2 uv) {
	vec3 col = vec3(0.);
	vec2 cell = fract(uv);
	if (cell.x < fwidth(uv.x)) {
		col += vec3(1.);
	}
	if (cell.y < fwidth(uv.y)) {
		col += vec3(1.);
	}
	if (abs(uv.y) < fwidth(uv.y)) {
		col = vec3(1., 0., 0.);
	}
	if (abs(uv.x) < fwidth(uv.x)) {
		col = vec3(0., 1., 0.);
	}
	return col;
}

void fragment() {
	vec2 iResolution = 1.0 / SCREEN_PIXEL_SIZE;
	vec2 uv = 3. * (2. * FRAGCOORD.xy - iResolution)
		/ min(iResolution.x, iResolution.y);
	COLOR = vec4(GetGridColor(uv), 1.);
}
