#ifndef HEX_METRICS_INCLUDED
#define HEX_METRICS_INCLUDED

#define OUTER_TO_INNER 0.866025404
#define OUTER_RADIUS 10.0
#define CHUNK_SIZE_X 5
#define TILING_SCALE (1.0/ (float(CHUNK_SIZE_X) * 2.0 * OUTER_RADIUS / OUTER_TO_INNER))

vec2 WorldToHexSpace(vec2 p) {
	return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}

// 之前临时用这个算 vec3 转球面 UV，但现在都改用三平面投射了
//vec2 WorldPosToSphereUV(vec3 world_pos) {
	//return vec2(world_pos.x - world_pos.y * 0.5 - world_pos.z * 0.5,
		//(world_pos.y - world_pos.z) * OUTER_TO_INNER);
//}
#endif