#define OUTER_TO_INNER 0.866025404
#define OUTER_RADIUS 10.0
#define CHUNK_SIZE_X 5
#define TILING_SCALE (1.0/ (float(CHUNK_SIZE_X) * 2.0 * OUTER_RADIUS / OUTER_TO_INNER))

vec2 WorldToHexSpace(vec2 p) {
	return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}