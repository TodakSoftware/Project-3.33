Shader "PIDI Shaders Collection/Advanced Skin Shader 2/Realistic/Simple" {
	Properties {

		[Space(8)][Header(PBR Parameters)][Space(8)]
		_PSkinColor ("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_PSkinMainTex ("Main Texture", 2D) = "white" {}
		_PSkinBaseNormals ("Normalmap", 2D) = "bump" {}
		
		[NoScaleOffset]_PSkinOcclusionMap ("Occlusion Map", 2D) = "white" {}
		_PSkinOcclusionLevel("Occlusion Level", Range(0,1)) = 1
		[Space(8)]
		_PSkinSpecMap("Specular & Smoothness Map", 2D) = "white" {}
		_PSkinSpecColor("Specular Color", Color) = (0.1,0.1,0.1,1)
		_PSkinGlossiness("Smoothness", Range(0,1)) = 0.5
		[Space(8)][Header(Skin Settings)][Space(8)]
		_PSkinMicroSkinTiling("Micro Skin Tiling", Range(0.1,32)) = 10
		[NoScaleOffset]_PSkinMicroSkin("Micro Skin", 2D) = "bump" {}
		_PSkinSSSColor("Subsurface Color", Color) = (1,0,0,1)
		_PSkinTranslucencyLevel("Translucency Level", Range(0,4)) = 1
		[NoScaleOffset]_PSkinTranslucencyMap("Translucency Map", 2D) = "white" {}

		[Space(8)][Header(Secondary Specular)][Space(8)]_PSkinMicroSkinSmoothness("Micro Skin Smoothness", Range(0,1)) = 0
		_PSkinDualLobeTransition("Dual Lobe Transition", Range(0.1, 16)) = 8


	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM


		#pragma surface surf AdvancedSkin vertex:vert fullforwardshadows addshadow
		#include "CGIncludes/AdvancedSkin2_Core.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0

		sampler2D _PSkinMainTex;
		sampler2D _PSkinBaseNormals;
		sampler2D _PSkinMicroSkin;
		sampler2D _PSkinSpecMap;
		sampler2D _PSkinOcclusionMap;
		sampler2D _PSkinTranslucencyMap;
		sampler2D _PSkinWrinklesMap;

		sampler2D _PSkinDecal0Tex;
		sampler2D _PSkinDecal0SpecMap;

		sampler2D _PSkinDecal1Tex;
		sampler2D _PSkinDecal1SpecMap;

		struct Input {
			float2 uv_PSkinMainTex;
			float2 uv2_PSkinMicroSkin;
			float4 vNormal:COLOR;
			float3 viewDir;
			float3 viewDirW:TEXCOORD3;
		};

		half _PSkinMicroSkinTiling;
		half _PSkinGlossiness;
		half _PSkinOcclusionLevel;
		half _PSkinTranslucencyLevel;
		half _PSkinVertexMinDistance;
		half _PSkinDebugTensionMap;

		half _PSkinDecal0UV;
		half _PSkinDecal1UV;
		half _PSkinDecal0BlendMode;
		half _PSkinDecal1BlendMode;

		fixed4 _PSkinColor;
		fixed4 _PSkinSpecColor;
		fixed4 _PSkinSSSColor;

		fixed4 _PSkinDecal0Color;
		fixed4 _PSkinDecal1Color;
		fixed4 _PSkinDecal0SpecColor;
		fixed4 _PSkinDecal1SpecColor;


		float4 _PSkinHeadBonePos;
		float4 _PSkinHeadBoneRotO;
		float4 _PSkinHeadBoneRotC;
				
		float4 _PSkinWorldPos;
		float4 _PSkinViewDir;

		float4 _PSkinDecal0UVTransform;
		float4 _PSkinDecal1UVTransform;
			   
		int _PSkinDeferredLightCount;

		float4 _PSkinDefLightsPosDir[32];
		float4 _PSkinDefLightsColor[32];
		float4 _PSkinDefLightsData[32];


		float _PSkinMicroSkinSmoothness;
		float _PSkinDualLobeTransition;
		
		struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
            float4 color : COLOR;
            float4 tangent : TANGENT;
        };


		

		void vert (inout appdata v) {
			
						
			v.color = float4(mul(unity_ObjectToWorld,float4(v.normal.x,v.normal.y,v.normal.z, 1)).xyz, 0);
			v.texcoord2 = mul(unity_ObjectToWorld, v.vertex);
			v.texcoord3.xyz = WorldSpaceViewDir(v.vertex);

		}

		float _PSkinPhong;
        float _PSkinEdgeLength;

        

		void surf (Input IN, inout SurfaceOutputSkin o) {
			fixed4 c = tex2D (_PSkinMainTex, IN.uv_PSkinMainTex) * _PSkinColor;
			
			half mapTransition = saturate(pow(IN.vNormal.a*1.5,1.5));

			float4 normalA = tex2D(_PSkinBaseNormals,IN.uv_PSkinMainTex);

			half3 baseNormal = UnpackNormalScaled(normalA, 0.85);

			half4 microSkin = tex2D(_PSkinMicroSkin, IN.uv2_PSkinMicroSkin*_PSkinMicroSkinTiling);

			half3 microSkinN = UnpackMixedNormal(microSkin.rg, 0.15);


			half4 occA = tex2D(_PSkinOcclusionMap, IN.uv_PSkinMainTex);

			half4 spec = tex2D(_PSkinSpecMap, IN.uv_PSkinMainTex);
			half occlusion = lerp(1, occA.r, _PSkinOcclusionLevel);
			half4 transTex = tex2D(_PSkinTranslucencyMap, IN.uv_PSkinMainTex);

			o.WarpNormal = IN.vNormal.xyz;
			o.Normal = baseNormal;
			
			half3 realColor = c.rgb*lerp(lerp(occlusion*_PSkinSSSColor,occlusion,occlusion),1,0.25);


			half3 albedo = realColor;
			
			o.Albedo = albedo;
			o.Translucency = half4(_PSkinSSSColor.rgb, transTex.r*_PSkinTranslucencyLevel);
			o.Occlusion = occlusion;

			o.Specular = spec.rgb*_PSkinSpecColor.rgb*microSkin.b;


			
			o.Smoothness = lerp(spec.a * _PSkinGlossiness * microSkin.b, saturate(microSkin.b * _PSkinMicroSkinSmoothness + spec.a * _PSkinGlossiness * microSkin.b), 1 - pow(abs(dot(IN.vNormal, IN.viewDirW)), _PSkinDualLobeTransition));

			
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
