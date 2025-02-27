shader_type spatial;

// 定义 Uniform 变量
uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

void fragment() {
	// Hex Map 6 河流 - 7.5 拉伸 V 坐标
	// 不过貌似不写这个，自动就包裹正确了？不太确定
	vec2 uv = UV;
	// 检查 UV 坐标的 y 分量是否小于 0
	//if (UV.y < 0.0) {
		//uv.y += 1.0;
	//}
	uv.y -= TIME;
	uv.y = fract(uv.y);
	// 从主纹理中采样颜色
	vec4 albedo_tex = texture(main_tex, uv);
	// 计算最终颜色
	vec4 final_color = albedo_tex * color;

	// 设置漫反射颜色
	ALBEDO = final_color.rgb * COLOR.rgb;
	// 设置金属度
	METALLIC = metallic;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = final_color.a;
	// 将 UV 坐标的 R 和 G 分量赋值给漫反射颜色的 R 和 G 分量
	ALBEDO.rg = uv;
}