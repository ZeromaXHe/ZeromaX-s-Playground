// 通义灵码根据 Catlike Coding - Hex Map 6 河流 - 7 流动的河流 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-6/#7
// 除了 source_color、hint_default_white 没写出来，是我参考 Godot 文档写的，其他都没啥问题
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
	if (UV.y < 0.0) {
		uv.y += 1.0;
	}
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
// Unity Shader 代码原文：

//Shader "Custom/River" {
//	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//	}
//	SubShader {
//		Tags { "RenderType"="Opaque" }
//		LOD 200
//
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows
//		#pragma target 3.0
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//			float4 color : COLOR;
//		};
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb * IN.color;
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//			o.Albedo.rg = IN.uv_MainTex;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}