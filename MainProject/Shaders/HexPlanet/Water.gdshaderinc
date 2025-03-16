#include "HexMetrics.gdshaderinc"
#include "Sebastian/Triplanar.gdshaderinc"

float River (vec2 riverUV, sampler2D noiseTex) {
	vec2 uv = riverUV;
	// 第一次采样
	uv.x = uv.x * 0.0625 + TIME * 0.005;
	uv.y -= TIME * 0.25;
	vec4 noise = texture(noiseTex, uv);
	// 第二次采样
	vec2 uv2 = riverUV;
	uv2.x = uv2.x * 0.0625 - TIME * 0.0052;
	uv2.y -= TIME * 0.23;
	vec4 noise2 = texture(noiseTex, uv2);
	return noise.r * noise2.a;
}

float Foam (float shore, vec3 world_pos, sampler2D noiseTex) {
	// sqrt 使近岸的泡沫会更强
	shore = sqrt(shore) * 0.9;
	// 扰动泡沫
	vec4 noise = triplanarOffset(world_pos, normalize(world_pos), 10.0 * TILING_SCALE, vec2(TIME / 20. * 0.25), noiseTex);
	
	float distortion1 = noise.x * (1.0 - shore);
	// sin 形成周期波浪
	float foam1 = sin((shore + distortion1) * 10.0 - TIME);
	foam1 *= foam1;

	float distortion2 = noise.x * (1.0 - shore);
	// sin 形成周期波浪
	float foam2 = sin((shore + distortion2) * 10.0 + TIME + 2.0);
	foam2 *= foam2;

	return max(foam1, foam2) * shore;
}

float Waves (vec3 world_pos, sampler2D noiseTex) {
	// 计算两个不同的 UV 坐标
	vec3 normal = normalize(world_pos);
	vec4 noise1 = triplanarOffset(world_pos, normal, 2.5 * TILING_SCALE, vec2(0., TIME / 20.), noiseTex);
	vec4 noise2 = triplanarOffset(world_pos, normal, 3.0 * TILING_SCALE, vec2(TIME / 20., 0.), noiseTex);

	// 计算波浪效果
	float blendWave = sin(
		(world_pos.x + world_pos.z) * 0.1 +
		(noise1.y + noise2.z) + TIME);
	blendWave *= blendWave;

	float waves =
		mix(noise1.z, noise1.w, blendWave) +
		mix(noise2.x, noise2.y, blendWave);
	return smoothstep(0.75, 2.0, waves);
}

float WaveAnim(vec3 world_cam_pos, vec3 world_vertex) {
	float vertex_anim_weight = length(world_vertex - world_cam_pos);
	vertex_anim_weight = clamp(pow(vertex_anim_weight / 10., 3.), 0., 1.);
	// 波浪动画
	float wave_anim_detail = 100.;
	float max_wave_amplitude = 0.1 * vertex_anim_weight;
	float wave_anim_speed = 1.;
	float theta = acos(normalize(world_vertex).z); // 使用归一化 VERTEX 而不是 NORMAL，这样才不会产生接缝
	float phi = atan(world_vertex.y, world_vertex.x);
	float wave_A = sin(TIME * wave_anim_speed + theta * wave_anim_detail);
	float wave_B = sin(TIME * wave_anim_speed + phi * wave_anim_detail);
	return (wave_A + wave_B) * max_wave_amplitude + 1.3 * max_wave_amplitude; // 保证海水波浪尽量高于原海面（2 * 时一定高于）
}