// 通义灵码根据 Catlike Coding - Hex Map 6 河流 - 8 河流动画 中的 Unity Shader 直接转写过来的
// https://catlikecoding.com/unity/tutorials/hex-map/part-6/#8
shader_type spatial;
// 设置渲染模式为混合模式（透明效果）和无光照模式
render_mode blend_mix, unshaded;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D main_tex: hint_default_white;
uniform float glossiness: hint_range(0.0, 1.0) = 0.5;
uniform float metallic: hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec2 uv = UV;
    
    // 第一次采样
    uv.x = uv.x * 0.0625 + TIME * 0.005;
    uv.y -= TIME * 0.25;
    vec4 noise = texture(main_tex, uv);
    
    // 第二次采样
    vec2 uv2 = UV;
    uv2.x = uv2.x * 0.0625 - TIME * 0.0052;
    uv2.y -= TIME * 0.23;
    vec4 noise2 = texture(main_tex, uv2);
    
    // 计算最终颜色
    vec4 c = clamp(color + noise.r * noise2.a, 0.0, 1.0);
    
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

//Shader "Custom/River" {
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
//		#pragma surface surf Standard alpha // fullforwardshadows
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
//			float2 uv = IN.uv_MainTex;
//			uv.x = uv.x * 0.0625 + _Time.y * 0.005;
//			uv.y -= _Time.y * 0.25;
//			float4 noise = tex2D(_MainTex, uv);
//			
//			float2 uv2 = IN.uv_MainTex;
//			uv2.x = uv2.x * 0.0625 - _Time.y * 0.0052;
//			uv2.y -= _Time.y * 0.23;
//			float4 noise2 = tex2D(_MainTex, uv2);
//			
//			fixed4 c = saturate(_Color + noise.r * noise2.a);
//			o.Albedo = c.rgb;
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}