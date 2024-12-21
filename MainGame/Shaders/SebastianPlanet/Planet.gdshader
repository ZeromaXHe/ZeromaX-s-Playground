// Visual Shader 是真的难用啊…… 还是老老实实写 gdshader
shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

global uniform vec2 elevation_min_max;
uniform sampler2D planet_texture : repeat_disable;

float InverseLerp(float a, float b, float t) {
	return clamp((t - a) / (b - a), 0.0, 1.0);
}

void fragment() {
	//vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	//float elevation = length(world_pos);
	//float diff = max_elevation - min_elevation;
	//float percent = (elevation - min_elevation) / diff;
	float biome_idx = UV.x;
	float elevation = UV.y;
	float min_elevation = elevation_min_max.x;
	float max_elevation = elevation_min_max.y;
	float ocean_percent = InverseLerp(min_elevation, 0.0, elevation);
	float ocean1_mask = floor(ocean_percent);
	float x1 = mix(0.0, 0.5, ocean_percent);
	float land_percent = InverseLerp(0.0, max_elevation, elevation);
	float x2 = mix(0.5, 1.0, land_percent);
	vec2 pos = vec2(x1 * (1.0 - ocean1_mask) + x2 * ocean1_mask, biome_idx);
	vec4 color = texture(planet_texture, pos);
	ALBEDO = vec3(color.xyz);
	ROUGHNESS = ocean1_mask;
}
