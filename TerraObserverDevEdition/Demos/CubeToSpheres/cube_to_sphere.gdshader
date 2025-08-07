shader_type spatial;

//uniform float radius: hint_range(0.1, 10.0, 0.01) = 0.5; // 想要生成的球体半径
uniform float progress: hint_range(0.0, 1.0, 0.001) = 0.0; // 代表从方块到球体的进度，0 为方块，1 为球体
uniform float average: hint_range(0.0, 1.0, 0.001) = 1.0; // 代表地块是否进行尽量平均化，0 为否，1 为是

void vertex() {
	vec3 p = VERTEX;
	float x2 = p.x * p.x;
	float y2 = p.y * p.y;
	float z2 = p.z * p.z;
	float x = p.x * sqrt(1. - (y2 + z2) / 2. + (y2 * z2) / 3.);
	float y = p.y * sqrt(1. - (z2 + x2) / 2. + (z2 * x2) / 3.);
	float z = p.z * sqrt(1. - (x2 + y2) / 2. + (x2 * y2) / 3.);
	vec3 avg_sphere_p = normalize(vec3(x, y, z));
	vec3 map_sphere_p = normalize(vec3(p.x, p.y, p.z));
	vec3 sphere_p = mix(map_sphere_p, avg_sphere_p, average);
	VERTEX = mix(p, sphere_p, progress);
	NORMAL = mix(NORMAL, sphere_p, progress); // 临时这样做
}

void fragment() {
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
