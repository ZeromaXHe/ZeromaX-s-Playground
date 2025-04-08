shader_type spatial;
//render_mode unshaded, depth_draw_never, depth_prepass_alpha;
//render_mode world_vertex_coords;

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.9; // 光滑度
uniform vec4 deep_col : source_color = vec4(0.0, 0.4, 0.7, 1.0);
uniform vec4 shallow_col : source_color = vec4(0.118, 0.573, 1., 1.0);
uniform sampler2D wave_normal_A;
uniform sampler2D wave_normal_B;
//uniform sampler2D foam_noise_tex;
global uniform float max_height;
global uniform float radius;
global uniform vec3 dir_to_sun; // 默认 Y 轴正方向相对 Z 轴高度角 23.4 度 vec3(0., 0.39714789, -0.9177546)
global uniform mat4 inv_planet_matrix; // 逆本地坐标矩阵

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#ifndef HEX_CELL_DATA_INCLUDED
	#include "HexCellData.gdshaderinc"
#endif

#ifndef WATER_INCLUDED
	#include "Water.gdshaderinc"
#endif

varying vec4 civ;
varying vec2 visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y + cell2.x * COLOR.z;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z;

	vec3 civ1 = GetTileCivDataById(CUSTOM0[0]).rgb;
	vec3 civ2 = GetTileCivDataById(CUSTOM0[1]).rgb;
	vec3 civ3 = GetTileCivDataById(CUSTOM0[2]).rgb;
	civ.rgb = civ1 * COLOR.x + civ2 * COLOR.y + civ3 * COLOR.z;

	// 波浪动画
	vec3 cam_pos_local = (inv_planet_matrix * vec4(CAMERA_POSITION_WORLD, 1.)).xyz;
	float wave_vertex_amplitude = WaveAnim(cam_pos_local, VERTEX);
	// 使用归一化 VERTEX 而不是 NORMAL，这样不同三角面间的相同顶点不会产生接缝
	VERTEX += normalize(VERTEX) * wave_vertex_amplitude;
}

float CalculateSpecularHighlight(vec3 normal, vec3 view_dir, float smoothness) {
	float specular_angle = acos(dot(normalize(dir_to_sun - view_dir), normal));
	float specular_exponent = specular_angle / (1. - smoothness);
	float specular_intensity = 1.;
	float specular_highlight = exp(-specular_exponent * specular_exponent) * specular_intensity;
	return specular_highlight;
}

void fragment() {
	// 获取世界/本地坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.)).xyz; // 点用 1.，因为要应用平移
	vec3 local_pos = (inv_planet_matrix * vec4(world_pos, 1.)).xyz;
	vec3 cam_pos_local = (inv_planet_matrix * vec4(CAMERA_POSITION_WORLD, 1.)).xyz;
	vec3 view_dir = normalize(local_pos - cam_pos_local);
	// 获取世界/本地坐标下的法线
	//vec3 world_normal = normalize(INV_VIEW_MATRIX * vec4(NORMAL, 0.)).xyz; // 向量用 0.，因为无需平移
	//vec3 world_normal_by_pos = normalize(world_pos);
	vec3 local_normal_by_pos = normalize(local_pos);
	//// 水面法线贴图
	float wave_speed = 0.35;
	float wave_normal_scale = 0.025;
	float wave_strength = 0.3; // 现在强度越强，离真实反射位置越远……
	// Unity Shader 的 _Time float4 4个分量分别是 (t/20, t, t*2, t*3)，t 是自该场景加载开始所经过的时间
	vec2 wave_offset_A = vec2(TIME / 20. * wave_speed, TIME / 20. * wave_speed * 0.8);
	vec2 wave_offset_B = vec2(TIME / 20. * wave_speed * -0.8, TIME / 20. * wave_speed * -0.3);
	// 这里就是不能用 triplanarNormal，没搞懂为啥……
	vec3 wave_normal1 = triplanarNormalUDN(local_pos, local_normal_by_pos, wave_normal_scale, wave_offset_A, wave_normal_A);
	vec3 wave_normal2 = triplanarNormalUDN(local_pos, local_normal_by_pos, wave_normal_scale, wave_offset_B, wave_normal_B);
	vec3 wave_normal = triplanarNormalUDN(local_pos, wave_normal1, wave_normal_scale, wave_offset_B, wave_normal_B);
	vec3 spec_wave_normal = normalize(mix(local_normal_by_pos, wave_normal, wave_strength));

	// glitter 不知道有啥用，我这里是均匀的零点几的值
	//float f2 = dot(world_normal, dir_to_sun);
	//f2 = smoothstep(0, 0.2, f2);
	//float g = step((pow(dot(wave_normal1, world_normal), 0.9)), 0.93);
	//float g2 = step((pow(dot(wave_normal2, world_normal), 0.9)), 0.93);
	//float glitter = g * g2 * 0.2 * f2;

	//// 水面高光处理逻辑
	float specular_highlight = CalculateSpecularHighlight(spec_wave_normal, view_dir, glossiness);
	float stepped_specular_highlight = 0.;
	float spec_threshold = 0.5;
	stepped_specular_highlight += step(spec_threshold, specular_highlight); // specular_highlight > 0.7 时为 1.
	stepped_specular_highlight += step(spec_threshold * 0.4, specular_highlight) * 0.4;
	stepped_specular_highlight += step(spec_threshold * 0.2, specular_highlight) * 0.2;
	specular_highlight = stepped_specular_highlight;

	//// 计算水下深度
	float dst_to_terrain = -depth(DEPTH_TEXTURE, SCREEN_UV, INV_PROJECTION_MATRIX).z;
	float dst_to_water = -VERTEX.z;
	float water_view_depth = dst_to_terrain - dst_to_water;
	//// 泡沫（由 WaterShore 绘制）
	//float foam_size = 4.;
	//float foam_speed = 0.5;
	//float foam_noise_scale = 13.;
	//float foam_noise_strength = 4.7;
	//float num_foam_lines = 2.5;
	//vec2 noise_scroll = vec2(TIME / 20., -TIME / 20. * 0.3333) * 0.25;
	//float foam_noise = triplanarOffset(world_pos, world_normal, foam_noise_scale, noise_scroll, foam_noise_tex).w;
	//foam_noise = smoothstep(0.2, .8, foam_noise);
	//float foam_t = saturate(1. / foam_size); // water_density_map
	//float foam_time = TIME * foam_speed;
	//float mask = 1. - foam_t;
	//float mask2 = smoothstep(1., 0.6, foam_t) * (foam_noise - .5);
	//mask2 = mix(1., mask2, 1. - mask * mask);
	//float v = sin(foam_t * PI * (1. + 2. * (num_foam_lines - 1.)) - foam_time) * step(0., mask);
	//v = saturate(v) + (step(foam_t, 0.35) + foam_noise * 0.15);
	//float foam_alpha = smoothstep(1., 0.7, foam_t);
	//foam_alpha *= f2;
	//float foam = (step(1., v) - mask2) * foam_alpha;

	//// 水的透明度
	float fresnel_pow = 2.; // 待提成 uniform
	float fresnel = 1. - saturate(pow(saturate(dot(-view_dir, local_normal_by_pos)), fresnel_pow));
	fresnel = max(0.7, fresnel);
	float edge_fade = 1.; // 待提成 uniform
	float shore_fade = 1. - exp(-water_view_depth * edge_fade);
	//float opaque_water = max(0., max(foam, step(0.5, specular_highlight)));
	//float water_alpha = saturate(max(opaque_water, shore_fade * fresnel));

	//// 灯光（好像这个乘一下很麻烦，也不太理解干什么用，直接注释掉）
	//float lighting = saturate(dot(world_normal, dir_to_sun));

	//// 颜色输出
	// 不需要 clamp（话说 clamp 的 min\max 传 vec3 时的逻辑好像会比较奇怪）
	vec3 water_col = mix(shallow_col, deep_col, 1. - exp(-water_view_depth / max_height)).rgb;

	float waves = Waves(local_pos, main_tex);
	water_col = saturate3(water_col + specular_highlight + waves);
	//water_col = water_col + glitter + foam;
	float water_alpha = max(fresnel * shore_fade, 0.7 + 0.3 * smoothstep(0.01, 0.5, waves));

	float explored = visibility.y;
	// 设置输出
	ALBEDO = (0.25 * water_col + 0.75 * civ.rgb) * visibility.x;
	SPECULAR = 0.15 * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = water_alpha * explored;
	NORMAL = (VIEW_MATRIX * vec4(MODEL_NORMAL_MATRIX * spec_wave_normal, 0.)).xyz;
}