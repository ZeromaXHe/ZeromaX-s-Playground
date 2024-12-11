uniform sampler2D hex_cell_data;
uniform vec4 hex_cell_data_texel_size;

vec4 GetCellData(vec4 texcoord2, int index) {
    vec2 uv;
    uv.x = (texcoord2[index] + 0.5) * hex_cell_data_texel_size.x;
    float row = floor(uv.x);
    uv.x -= row;
    uv.y = (row + 0.5) * hex_cell_data_texel_size.y;
    vec4 data = textureLod(hex_cell_data, uv, 0);
    data.w *= 255.0;
    return data;
}