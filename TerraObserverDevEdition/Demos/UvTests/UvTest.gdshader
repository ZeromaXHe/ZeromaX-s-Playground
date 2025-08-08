shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// 证明 Godot Shader UV 是左上角 (0,0) 开始，右下角 (1,1) 结束
	if (UV.x < 0.6 && UV.y < 0.3) {
		COLOR.a = 0.;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
