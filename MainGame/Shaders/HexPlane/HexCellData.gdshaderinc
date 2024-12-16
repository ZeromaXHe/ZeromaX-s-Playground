global uniform sampler2D hex_cell_data: source_color, filter_nearest;
global uniform vec4 hex_cell_data_texel_size;
global uniform bool hex_map_edit_mode;
global uniform vec4 cell_highlighting;

#include "HexMetrics.gdshaderinc"

vec4 FilterCellData (vec4 data) {
	if (hex_map_edit_mode) {
		data.xy = vec2(1.0);
	}
	return data;
}

vec4 GetCellData(vec4 texcoord2, int index) {
	vec2 uv;
	uv.x = (texcoord2[index] + 0.5) * hex_cell_data_texel_size.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * hex_cell_data_texel_size.y;
	vec4 data = textureLod(hex_cell_data, uv, 0);
	data.w *= 255.0;
	return FilterCellData(data);
}

vec4 GetCellData2(vec2 cellDataCoordinates) {
	vec2 uv = cellDataCoordinates + 0.5;
	uv.x *= hex_cell_data_texel_size.x;
	uv.y *= hex_cell_data_texel_size.y;
	return FilterCellData(textureLod(hex_cell_data, uv, 0.0));
}

struct HexGridData {
	// Cell center in hex space
	vec2 cellCenter;
	// Approximate cell offset coordinates. Good enough for point-filtered sampling.
	vec2 cellOffsetCoordinates;
	// For potential future use. U covers entire cell, V wraps a bit.
	vec2 cellUV;
	// Hexagonal distance to cell center, 0 at center, 1 at edges.
	float distanceToCenter;
	// Smoothstep smoothing for cell center distance transitions.
	// Based on screen-space derivatives.
	float distanceSmoothing;
};

// Smoothstep from 0 to 1 at cell center distance threshold.
float Smoothstep01 (HexGridData data, float threshold) {
	return smoothstep(threshold - data.distanceSmoothing,
		threshold + data.distanceSmoothing, data.distanceToCenter);
}

// Smoothstep from 1 to 0 at cell center distance threshold.
float Smoothstep10 (HexGridData data, float threshold) {
	return smoothstep(threshold + data.distanceSmoothing,
		threshold - data.distanceSmoothing, data.distanceToCenter);
}

// Smoothstep from 0 to 1 inside cell center distance range.
float SmoothstepRange (HexGridData data, float innerThreshold, float outerThreshold) {
	return Smoothstep01(data, innerThreshold) * Smoothstep10(data, outerThreshold);
}

// Is highlighed if square distance from cell to highlight center is below threshold.
// Works up to brush size 6.
bool IsHighlighted (HexGridData data) {
	vec2 cellToHighlight = abs(cell_highlighting.xy - data.cellCenter);
	// Adjust for world X wrapping if needed.
	if (cellToHighlight.x > cell_highlighting.w * 0.5) {
		cellToHighlight.x -= cell_highlighting.w;
	}
	return dot(cellToHighlight, cellToHighlight) < cell_highlighting.z;
}

#define HEX_ANGLED_EDGE_VECTOR vec2(1, sqrt(3))

// Calculate hexagonal center-edge distance for point relative to center in hex space.
// 0 at cell center and 1 at edges.
float HexagonalCenterToEdgeDistance (vec2 p) {
	// Reduce problem to one quadrant.
	p = abs(p);
	// Calculate distance to angled edge.
	float d = dot(p, normalize(HEX_ANGLED_EDGE_VECTOR));
	// Incorporate distance to vertical edge.
	d = max(d, p.x);
	// Double to increase range from center to edge to 0-1.
	return 2.0 * d;
}

// Calculate hex-based modulo to find position vector.
vec2 HexModulo (vec2 p) {
	return p - HEX_ANGLED_EDGE_VECTOR * floor(p / HEX_ANGLED_EDGE_VECTOR);
}

// Get hex grid data analytically derived from world-space XZ position.
HexGridData GetHexGridData (vec2 worldPositionXZ) {
	vec2 p = WorldToHexSpace(worldPositionXZ);
	// Vectors from nearest two cell centers to position.
	vec2 gridOffset = HEX_ANGLED_EDGE_VECTOR * 0.5;
	vec2 a = HexModulo(p) - gridOffset;
	vec2 b = HexModulo(p - gridOffset) - gridOffset;
	bool aIsNearest = dot(a, a) < dot(b, b);
	vec2 vectorFromCenterToPosition = aIsNearest ? a : b;
	HexGridData d;
	d.cellCenter = p - vectorFromCenterToPosition;
	d.cellOffsetCoordinates.x = d.cellCenter.x - (aIsNearest ? 0.5 : 0.0);
	d.cellOffsetCoordinates.y = d.cellCenter.y / OUTER_TO_INNER;
	d.cellUV = vectorFromCenterToPosition + 0.5;
	d.distanceToCenter = HexagonalCenterToEdgeDistance(vectorFromCenterToPosition);
	// Godot 好像 fwidth 没法在 gdshaderinc 中使用，记得在调用后外面自己补这个逻辑
	//d.distanceSmoothing = fwidth(d.distanceToCenter);
	return d;
}