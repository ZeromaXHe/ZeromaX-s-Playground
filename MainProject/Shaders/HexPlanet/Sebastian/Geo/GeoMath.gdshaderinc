// 参考：https://github.com/SebLague/Geographical-Adventures/blob/82fcda20bebb033c749b2339e9ce3a6e58007699/Assets/Scripts/Shader%20Common/GeoMath.hlsl
#ifndef GEOMATH_INCLUDED
#define GEOMATH_INCLUDED

#ifndef MATH_GDSHADERINC
	#include "res://Shaders/HexPlanet/Sebastian/Math.gdshaderinc"
#endif

// 通过（弧度制的）经纬度获取球面的点
vec3 LongitudeLatitudeToPoint(vec2 longLat) {
	float longitude = longLat.x;
	float latitude = longLat.y;
	float y = sin(latitude);
	float r = cos(latitude); // 在 y 处切开球的 2d 圆半径
	float z = sin(longitude) * r;
	float x = cos(longitude) * r;
	return vec3(x, y, z);
}

// 缩放（弧度制的）经纬度到 [0, 1] 的范围
vec2 LongitudeLatitudeToUV(vec2 longLat) {
	float longitude = longLat.x; // 范围在 [-PI, PI]
	float latitude = longLat.y; // 范围在 [-PI / 2, PI / 2]
	float u = -longitude / 2. / PI + 0.5;
	float v = 0.5 - latitude / PI; // Godot UV 的 v 方向从上到下 0 ~ 1
	return vec2(u, v);
}

vec2 UvToLongitudeLatitude(vec2 uv) {
	float longitude = (1. - 2. * uv.x) * PI;
	float latitude = (0.5 - uv.y) * PI; // Godot UV 的 v 方向从上到下 0 ~ 1
	return vec2(longitude, latitude);
}

// 将单位球上的点转换为经纬度
vec2 PointToLongitudeLatitude(vec3 p) {
	float longitude = atan(p.z, p.x);
	float latitude = asin(p.y);
	return vec2(longitude, latitude);
}

// 将单位球上的点转换为 uv texCoord
vec2 PointToUV(vec3 p) {
	vec2 longLat = PointToLongitudeLatitude(p);
	return LongitudeLatitudeToUV(longLat);
}

vec3 UvToPointOnSphere(vec2 uv) {
	return LongitudeLatitudeToPoint(UvToLongitudeLatitude(uv));
}

float DistanceBetweenPointsOnUnitSphere(vec3 a, vec3 b) {
	return acos(saturate(dot(a, b)));
}

#endif