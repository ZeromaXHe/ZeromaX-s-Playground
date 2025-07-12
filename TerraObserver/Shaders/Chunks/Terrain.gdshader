shader_type spatial;
// 这个 depth_prepass_alpha 很关键，如果是写了 ALPHA = c.a 就不能少了
// 不然阴影又是一坨稀饭、还会把遮挡的东西透到前面
render_mode depth_prepass_alpha;

// 定义 Uniform 变量
uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2DArray main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.3;
uniform float specular = 0.2;
uniform vec3 background_color: source_color = vec3(0.2, 0.169, 0.133);

global uniform float radius;
global uniform float max_height;

#ifndef HEX_CELL_DATA_INCLUDED
	#include "HexCellData.gdshaderinc"
#endif

#ifndef TRIPLANAR_GDSHADERINC
	#include "../Geos/Triplanar.gdshaderinc"
#endif

varying vec4 terrain; // 用 varying 在 vertex 和 fragment 间传递数据
varying vec4 civ;
varying vec4 visibility;

vec3 ColorizeSubmergence(vec3 baseColor, float surfaceY, float waterY) {
	float submergence = waterY - max(surfaceY, 0);
	vec3 colorFilter = vec3(0.25, 0.25, 0.75);
	float filterRange = 2.0 / max_height;
	return baseColor * mix(vec3(1.0), colorFilter,
		clamp(submergence * filterRange, 0.0, 1.0));
}

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	terrain.x = cell0.w; // w: 地形类型
	terrain.y = cell1.w;
	terrain.z = cell2.w;
	terrain.w = max(max(cell0.z, cell1.z), cell2.z) * max_height; // z: 水面高度
	visibility.x = cell0.x; // x: 可见
	visibility.y = cell1.x;
	visibility.z = cell2.x;
	visibility.xyz = mix(vec3(0.25), vec3(1.0), visibility.xyz);
	visibility.w = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z; // y: 已探索

	vec3 civ1 = GetTileCivDataById(CUSTOM0[0]).rgb;
	vec3 civ2 = GetTileCivDataById(CUSTOM0[1]).rgb;
	vec3 civ3 = GetTileCivDataById(CUSTOM0[2]).rgb;
	civ.rgb = civ1 * COLOR.x + civ2 * COLOR.y + civ3 * COLOR.z;

	// 将未探索地块高度放大到最大高度，防止特征显现
	float len = length(VERTEX);
	VERTEX *= mix(1., (radius + max_height) / len, 1. - visibility.w);
}

vec4 GetTerrainColor(vec3 world_pos, vec3 world_normal, int index, vec4 weights, vec4 inVisibility) {
	vec4 c = triplanarArray(world_pos, world_normal, 10. * TILING_SCALE, main_tex, terrain[index]);
	return c * (weights[index] * inVisibility[index]);
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz; // 位置需位移，补 1.0
	// 获取世界法线
	vec3 world_normal = (INV_VIEW_MATRIX * vec4(NORMAL, 0.)).xyz; // 法线向量无需位移，补 0.
	vec4 inColor = COLOR;
	vec4 c = GetTerrainColor(world_pos, world_normal, 0, inColor, visibility) +
		GetTerrainColor(world_pos, world_normal, 1, inColor, visibility) +
		GetTerrainColor(world_pos, world_normal, 2, inColor, visibility);
	vec3 baseColor = ColorizeSubmergence(c.rgb, length(world_pos) - radius, terrain.w);
	float explored = visibility.w;
	// 设置漫反射颜色
	ALBEDO = baseColor * color.rgb * explored; // 先显示纹理颜色
	//ALBEDO = (0.03 * baseColor + 0.97 * civ.rgb) * color.rgb * explored;
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
	ALPHA = c.a;
}