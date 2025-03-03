global uniform sampler2D hex_tile_data: source_color, filter_nearest;
global uniform vec4 hex_tile_data_texel_size;
global uniform bool hex_map_edit_mode;

#include "HexMetrics.gdshaderinc"

vec4 FilterCellData (vec4 data) {
	if (hex_map_edit_mode) {
		data.xy = vec2(1.0); // 编辑模式下，可见和探索都置为 1
	}
	return data;
}

vec4 GetCellDataById(float id) {
	vec2 uv;
	uv.x = (id + 0.5) * hex_tile_data_texel_size.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * hex_tile_data_texel_size.y;
	vec4 data = textureLod(hex_tile_data, uv, 0);
	data.w *= 255.0;
	return FilterCellData(data);
}

vec4 GetCellData(vec4 texcoord2, int index) {
	return GetCellDataById(texcoord2[index]);
}
