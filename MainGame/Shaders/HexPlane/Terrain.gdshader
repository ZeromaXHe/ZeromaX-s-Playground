// Catlike Coding - Hex Map 14 地形纹理 - 2.5 着色器
// https://catlikecoding.com/unity/tutorials/hex-map/part-14/#2.5
shader_type spatial;
// 这个 depth_prepass_alpha 很关键，如果是写了 ALPHA = c.a 就不能少了
// 不然阴影又是一坨稀饭、还会把遮挡的东西透到前面
//render_mode depth_prepass_alpha;

// 定义 Uniform 变量
uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2DArray main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform bool grid_on = true;
uniform sampler2D grid_tex: source_color;
uniform float glossiness: hint_range(0.0, 1.0) = 0.3;
uniform float specular = 0.2;
uniform vec3 background_color: source_color = vec3(0.2, 0.169, 0.133);
global uniform bool show_map_data = false;

#include "HexCellData.gdshaderinc"

varying vec4 terrain; // 用 varying 在 vertex 和 fragment 间传递数据
varying vec4 visibility;
varying float map_data;

vec3 ColorizeSubmergence(vec3 baseColor, float surfaceY, float waterY) {
	float submergence = waterY - max(surfaceY, 0);
	vec3 colorFilter = vec3(0.25, 0.25, 0.75);
	float filterRange = 1.0 / 15.0;
	return baseColor * mix(vec3(1.0), colorFilter,
		clamp(submergence * filterRange, 0.0, 1.0));
}

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	terrain.x = cell0.w;
	terrain.y = cell1.w;
	terrain.z = cell2.w;
	terrain.w = max(max(cell0.b, cell1.b), cell2.b) * 30.0;
	visibility.x = cell0.x;
	visibility.y = cell1.x;
	visibility.z = cell2.x;
	visibility.xyz = mix(vec3(0.25), vec3(1.0), visibility.xyz);
	visibility.w = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z;
	if (show_map_data) {
		map_data = cell0.z * COLOR.x + cell1.z * COLOR.y + cell2.z * COLOR.z;
	}
}

vec4 GetTerrainColor(vec3 world_pos, int index, vec4 weights, vec4 inVisibility) {
	vec3 uvw = vec3(world_pos.xz * (2.0 * TILING_SCALE), terrain[index]);
	vec4 c = texture(main_tex, uvw);
	return c * (weights[index] * inVisibility[index]);
}

// Apply an 80% darkening grid outline at hex center distance 0.965-1.
vec3 ApplyGrid(vec3 baseColor, HexGridData h) {
	return baseColor * (0.2 + 0.8 * Smoothstep10(h, 0.965));
}

// Apply a white outline at hex center distance 0.68-0.8.
vec3 ApplyHighlight (vec3 baseColor, HexGridData h) {
	return clamp(SmoothstepRange(h, 0.68, 0.8) + baseColor.rgb, 0.0, 1.0);
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec4 inColor = COLOR;
	vec4 c = GetTerrainColor(world_pos, 0, inColor, visibility) +
		GetTerrainColor(world_pos, 1, inColor, visibility) +
		GetTerrainColor(world_pos, 2, inColor, visibility);
	vec3 baseColor = ColorizeSubmergence(c.rgb, world_pos.y, terrain.w);
	// 渲染六边形网格
	HexGridData hgd = GetHexGridData(world_pos.xz);
	hgd.distanceSmoothing = fwidth(hgd.distanceToCenter);
	if (grid_on) {
		baseColor = ApplyGrid(baseColor, hgd);
	}
	if (IsHighlighted(hgd)) {
		baseColor = ApplyHighlight(baseColor, hgd);
	}
	float explored = visibility.w;
	// 设置漫反射颜色
	ALBEDO = baseColor * color.rgb * explored;
	if (show_map_data) {
		ALBEDO = vec3(map_data);
	}
	SPECULAR = specular * explored;
	AO = explored;
	EMISSION = background_color * (1.0 - explored);
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	// 直接设置这个会导致河面、道路、水面等等涉及到透明度的和地形前后渲染关系混乱。
	// 而且标签的黑边也会消失……
	// 需要同时调整其他这些的材质的渲染优先度，将其他的优先度设置为 1，这里地形默认 0
	// 目前因为都是地形都是不透明的，直接不设置透明度方便些，这样也省的给标签加材质，再改渲染优先度
	//ALPHA = c.a;
}