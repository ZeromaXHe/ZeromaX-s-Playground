// 通义灵码根据 Catlike Coding - Hex Map 8 水 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-8/
shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: source_color, filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float specular = 0.2;

// #include 和 gdshaderinc 在官方文档“着色器预处理器”讲的
#include "HexCellData.gdshaderinc"
#include "Water.gdshaderinc"

varying vec2 visibility;

void vertex() {
	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	vec4 cell2 = GetCellData(CUSTOM0, 2);
	visibility.x = cell0.x * COLOR.x + cell1.x * COLOR.y + cell2.x * COLOR.z;
	visibility.x = mix(0.25, 1, visibility.x);
	visibility.y = cell0.y * COLOR.x + cell1.y * COLOR.y + cell2.y * COLOR.z;
}

void fragment() {
	// 获取世界坐标
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;

	float shore = UV.y;
	float foam = Foam(shore, world_pos.xz, main_tex);
	float waves = Waves(world_pos.xz, main_tex);
	// 计算最终颜色
	vec4 c = clamp(color + max(foam, waves), 0.0, 1.0);
	float explored = visibility.y;
	// 设置输出
	ALBEDO = c.rgb * visibility.x;
	SPECULAR = specular * explored;
	AO = explored;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = c.a * explored;
}
// Unity Shader 代码原文：

//Shader "Custom/Water" {
//	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//	}
//	SubShader {
//		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
//		LOD 200
//
//		CGPROGRAM
//		#pragma surface surf Standard alpha
//		#pragma target 3.0
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//			float3 worldPos;
//		};
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			float2 uv1 = IN.worldPos.xz;
//			uv1.y += _Time.y;
//			float4 noise1 = tex2D(_MainTex, uv1 * 0.025);
//
//			float2 uv2 = IN.worldPos.xz;
//			uv2.x += _Time.y;
//			float4 noise2 = tex2D(_MainTex, uv2 * 0.025);
//
//			//float waves = noise1.z + noise2.x;
//			//waves = smoothstep(0.75, 2, waves);
//
//			float blendWave = sin(
//				(IN.worldPos.x + IN.worldPos.z) * 0.1 +
//				(noise1.y + noise2.z) + _Time.y);
//			blendWave *= blendWave;
//
//			float waves =
//				lerp(noise1.z, noise1.w, blendWave) +
//				lerp(noise2.x, noise2.y, blendWave);
//			waves = smoothstep(0.75, 2, waves);
//
//			fixed4 c = saturate(_Color + waves);
//			o.Albedo = c.rgb;
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}