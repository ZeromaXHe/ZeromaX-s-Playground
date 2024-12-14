global uniform sampler2D hex_cell_data: source_color, filter_nearest;
global uniform vec4 hex_cell_data_texel_size;
global uniform bool hex_map_edit_mode;

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