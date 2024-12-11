// 通义灵码根据 Catlike Coding - Hex Map 7 道路 - 5 道路外观 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-7/#5
// 这次转的比较失败，很多是我自己改好的。它写了很多 fragment() 和 vertex() 的入参，感觉和之前几次比变蠢了……
shader_type spatial;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: filter_linear_mipmap, repeat_enable;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

#include "HexCellData.gdshaderinc"
varying float visibility;

// 设置深度偏移
void vertex() {
	VERTEX.y += 0.1;

	vec4 cell0 = GetCellData(CUSTOM0, 0);
	vec4 cell1 = GetCellData(CUSTOM0, 1);
	visibility = cell0.x * COLOR.x + cell1.x * COLOR.y;
	visibility = mix(0.25, 1, visibility);
}

void fragment() {
	vec2 uv = UV;
	// INV_VIEW_MATRIX 视图空间向世界空间变换.
	// VERTEX 来自顶点函数的顶点(默认情况下, 在视图空间中).
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;

	vec4 noise = texture(main_tex, world_pos.xz * 0.025);
	vec4 c = color * (noise.y * 0.75 + 0.25) * visibility;
	float blend = uv.x;
	blend *= noise.x + 0.5;
	blend = smoothstep(0.4, 0.7, blend);

	ALBEDO = c.rgb;
	METALLIC = metallic;
	ROUGHNESS = 1.0 - glossiness;
	ALPHA = blend;
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
//		#pragma surface surf Standard fullforwardshadows decal:blend
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
//			float4 noise = tex2D(_MainTex, IN.worldPos.xz * 0.025);
//			fixed4 c = _Color * (noise.y * 0.75 + 0.25);
//			float blend = IN.uv_MainTex.x;
//			blend *= noise.x + 0.5;
//			blend = smoothstep(0.4, 0.7, blend);
//
//			o.Albedo = c.rgb;
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = blend;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}
