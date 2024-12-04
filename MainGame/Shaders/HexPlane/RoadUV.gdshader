// 通义灵码根据 Catlike Coding - Hex Map 7 道路 - 3 三角剖分道路 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-7/#3
shader_type spatial;
// 设置渲染模式为无光照模式，并且深度绘制为不透明对象。
// 这相当于 Unity 中的 RenderType="Opaque" 和 Queue="Geometry+1"。
render_mode unshaded, depth_draw_opaque;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

// 设置深度偏移
void vertex() {
	// 设置深度偏移，相当于 Unity 中的 Offset -1, -1。
	VERTEX.y += 1.0;
}

void fragment() {
	vec2 uv = UV;

	// 创建颜色向量，使用 UV 坐标作为 RGB 值，Alpha 为 1
	vec4 c = vec4(uv, 1.0, 1.0);

	// 设置漫反射颜色
	ALBEDO = c.rgb;
	// 设置金属度
	METALLIC = metallic;
	// 设置光滑度
	ROUGHNESS = 1.0 - glossiness;
	// 设置透明度
	ALPHA = c.a;
}

// Unity Shader 代码原文：

//Shader "Custom/Road" {
//	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//	}
//	SubShader {
//		Tags {
//			"RenderType"="Opaque"
//			"Queue" = "Geometry+1"
//		}
//		LOD 200
//		Offset -1, -1
//
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows
//		#pragma target 3.0
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//		};
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			fixed4 c = fixed4(IN.uv_MainTex, 1, 1);
//			o.Albedo = c.rgb;
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}
