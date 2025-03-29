shader_type canvas_item;

#ifndef GEOMATH_INCLUDED
	#include "GeoMath.gdshaderinc"
#endif

uniform float lon; // 经度（角度制）。不能叫 longitude，因为和引入的变量重名
uniform float lat; // 纬度（角度制）。不能叫 lattitude，因为和引入的变量重名
//uniform vec3 pos_normal = vec3(0., 0., -1.); // 位置法线（其实也就是映射到单位球方向向量）
uniform float angle_to_north; // 距离前方向北的角度差（弧度制）

// 围着指定的轴和角度来旋转点（按照弧度制）
// （参见 Rodrigues' 罗德里格旋转公式）
vec3 RotateAroundAxis(vec3 p, vec3 axis, float angle) {
	return p * cos(angle) + cross(axis, p) * sin(angle)
		+ axis * dot(axis, p) * (1. - cos(angle));
}

vec2 RotateAroundAxis(vec2 p, vec2 axis, float angle) {
	vec2 res = p - axis;
	res = vec2(res.x * cos(angle) - res.y * sin(angle), res.y * cos(angle));
	return res += axis;
}

void fragment() {
	// 转换纹理坐标到球上的点
	vec3 sphere_pos = UvToPointOnSphere(UV);
	// 按照指定角度（角度制），绕着垂直和水平轴，旋转球点
	vec3 rotated_sphere_pos = RotateAroundAxis(
		sphere_pos, vec3(0., 0., 1.), radians(lat));
	rotated_sphere_pos = RotateAroundAxis(
		rotated_sphere_pos, vec3(0., 1., 0.), -radians(lon));
	// 按照将正上方旋转到向前方向旋转
	vec3 axis = LongitudeLatitudeToPoint(vec2(radians(lon), radians(lat)));
	rotated_sphere_pos = RotateAroundAxis(
		rotated_sphere_pos, axis, angle_to_north);
	// 转换旋转过的球点，回到纹理坐标
	vec2 rotated_tex_coord = PointToUV(rotated_sphere_pos);
	COLOR = texture(TEXTURE, rotated_tex_coord);
}
