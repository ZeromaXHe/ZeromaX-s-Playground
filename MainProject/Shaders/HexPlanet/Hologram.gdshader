shader_type spatial;
render_mode unshaded, shadows_disabled, world_vertex_coords;
uniform vec3 color: source_color;
uniform float lines1: hint_range(0.0, 200.0, 1.0) = 2.0;
uniform float lines2: hint_range(0.0, 100.0, 1.0) = 2.0;
uniform float distortion_strength: hint_range(0.0, 1.0) = 0.4;
uniform float distortion_percentage: hint_range(0.0, 1.0) = 0.02;
uniform float _emission_min: hint_range(0.0, 10.0) = 1.0;
uniform float _emission_max: hint_range(0.0, 10.0) = 4.0;
uniform float _fresnel: hint_range(0.0, 10.0) = 1.0;
global uniform float max_height;
global uniform float radius;

float random(float x) {
	return fract(sin(x * 12.9898 * 78.233) * 43758.5433);
}

float random2_1(vec2 uv) {
	return fract(sin(dot(uv.xy + vec2(sin(TIME), cos(TIME)), vec2(12.9898, 78.233))) * 43758.5453123);
}

float fresnel(float amount, vec3 normal, vec3 view) {
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0)), amount);
}

varying float rand_dist;

void vertex() {
	float distortion = sin(TIME * 10.0 + VERTEX.y * 20.0) * 0.04;
	float rand_time = random(TIME);
	rand_dist = step(rand_time, 1. - distortion_percentage);
	VERTEX *= 1. + distortion * rand_dist * distortion_strength * max_height / radius;
}

void fragment() {
	float n = random2_1(SCREEN_UV);
	float f = fresnel(_fresnel, NORMAL, VIEW);
	// 增加奇偶线
	vec2 igrid = floor(SCREEN_UV * lines1);
	float l1 = mod(igrid.y, 2.0);
	l1 = mix(0.4, 1.0, l1);
	// 随机线条
	vec2 uv2 = SCREEN_UV;
	uv2.y -= TIME * 0.06;
	vec2 igrid2 = floor(uv2 * lines2);
	float l2 = random(igrid2.y);
	l2 = step(0.2, l2);
	l2 = mix(0.2, 1.0, l2);
	
	float m = f * n * l1 * l2;
	ALPHA = m;
	ALBEDO = color * mix(_emission_min, _emission_max, rand_dist); // 变形时高亮
}
