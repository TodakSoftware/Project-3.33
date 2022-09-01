Shader "PIDI Shaders Collection/Advanced Skin Shader 2/Realistic/Regions Map" {
	Properties {

		[Space(8)][Header(PBR Parameters)][Space(8)]
		[PerRendererData]_PSkinColor ("Color", Color) = (1,1,1,1)
		[NoScaleOffset][PerRendererData]_PSkinMainTex ("Main Texture", 2D) = "white" {}
		_PSkinBaseNormals ("Normalmap", 2D) = "bump" {}
		
		[NoScaleOffset][PerRendererData]_PSkinOcclusionMap ("Occlusion Map", 2D) = "white" {}
		[PerRendererData]_PSkinOcclusionLevel("Occlusion Level", Range(0,1)) = 1
		[Space(8)]
		[PerRendererData]_PSkinSpecMap("Specular & Smoothness Map", 2D) = "white" {}
		[PerRendererData]_PSkinSpecColor("Specular Color", Color) = (0.1,0.1,0.1,1)
		[PerRendererData]_PSkinGlossiness("Smoothness", Range(0,1)) = 0.5
		[Space(8)][Header(Skin Settings)][Space(8)]
		[PerRendererData]_PSkinMicroSkinTiling("Micro Skin Tiling", Range(0.1,32)) = 10
		[NoScaleOffset][PerRendererData]_PSkinMicroSkin("Micro Skin", 2D) = "bump" {}
		[PerRendererData]_PSkinSSSColor("Subsurface Color", Color) = (1,0,0,1)
		[PerRendererData]_PSkinTranslucencyLevel("Translucency Level", Range(0,4)) = 1
		[NoScaleOffset][PerRendererData]_PSkinTranslucencyMap("Translucency Map", 2D) = "white" {}
	
		[PerRendererData]_PSkinDebugTensionMap("Debug Tension Map",Float) = 0


		[PerRendererData] _PSkinDecal0BlendMode("Decal 0 Blend Mode", Float ) = 0
		[PerRendererData] _PSkinDecal1BlendMode("Decal 1 Blend Mode", Float ) = 0

		[PerRendererData] _PSkinDecal0Tex("Decal 0 Texture", 2D) = "black"{}
		[PerRendererData] _PSkinDecal0Color("Decal 0 Color", Color) = (1,1,1,1)
		[PerRendererData] _PSkinDecal0SpecColor("Decal 0 Spec Color", Color) = (0.15,0.15,0.15,0.5)
		[PerRendererData] _PSkinDecal0SpecMap("Decal 0 Spec (RGB) Gloss (A)", 2D) = "white"{}
		
		
		[PerRendererData] _PSkinDecal0UV("Decal 0 Spec UV Space", Float) = 0
		[PerRendererData] _PSkinDecal0UVTransform("Decal 0 Spec UV Transform", Vector) = (1,1,0,0)
				
		[PerRendererData] _PSkinDecal1UV("Decal 1 Spec UV Space", Float) = 0
		[PerRendererData] _PSkinDecal1UVTransform("Decal 1 Spec UV Transform", Vector) = (1,1,0,0)


		[PerRendererData] _PSkinDecal1Tex("Decal 1 Texture", 2D) = "black"{}
		[PerRendererData] _PSkinDecal1Color("Decal 1 Color", Color) = (1,1,1,1)
		[PerRendererData] _PSkinDecal1SpecColor("Decal 1 Spec Color", Color) = (0.15,0.15,0.15,0.5)
		[PerRendererData] _PSkinDecal1SpecMap("Decal 1 Spec (RGB) Gloss (A)", 2D) = "white"{}

		[PerRendererData] _PSkinHeadBonePos("Head Bone Position",Vector) = (0,0,0,0)
		[PerRendererData] _PSkinHeadBoneRotC("Head Bone Current Rot",Vector) = (0,0,0,0)


		[PerRendererData] _PSkinDeferredLightCount("Deferred Lights Count", Float ) = 4
		
		[PerRendererData] _PSkinWorldPos("World Position",Vector) = (0,0,0,0)
		[PerRendererData] _PSkinViewDir("View Direction",Vector) = (0,0,0,0)

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


		float4 MulQuaternions( float4 q1, float4 q2 ){
			
			float4 q = {
				q1.x*q2.x - q1.y*q2.y - q1.z*q2.z - q1.w*q2.w,
				q1.x*q2.y + q1.y*q2.x + q1.z*q2.w - q1.w*q2.z,
				q1.x*q2.z - q1.y*q2.w + q1.z*q2.x + q1.w*q2.y,
				q1.x*q2.w + q1.y*q2.z - q1.z*q2.y + q1.w*q2.x
			};

			return q;
		}


		float3 rotateVector(float3 v, float4 q){
		
			float4 vecQ = float4(0, v.xyz);
			float4 invQ = float4(q.x,-q.y,-q.z,-q.w);
			return MulQuaternions(MulQuaternions(q,vecQ),invQ).yzw;

		}


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
			
			
			
			
			half2 decal0UV = lerp(IN.uv_PSkinMainTex,IN.uv2_PSkinMicroSkin,_PSkinDecal0UV)*_PSkinDecal0UVTransform.xy+_PSkinDecal0UVTransform.zw;
			half2 decal1UV = lerp(IN.uv_PSkinMainTex,IN.uv2_PSkinMicroSkin,_PSkinDecal1UV)*_PSkinDecal1UVTransform.xy+_PSkinDecal1UVTransform.zw;

			half4 decal0 = tex2D(_PSkinDecal0Tex, decal0UV);
			half4 decal0Spec = tex2D(_PSkinDecal0SpecMap, decal0UV);
		
			half decal0Blend = lerp(decal0.a,0, saturate(_PSkinDecal0BlendMode));

			decal0.rgb *= _PSkinDecal0Color.rgb*decal0.a;
			
			half4 decal1 = tex2D(_PSkinDecal1Tex, decal1UV);
			half4 decal1Spec = tex2D(_PSkinDecal1SpecMap, decal1UV);

			half decal1Blend = lerp(decal1.a,0, saturate(_PSkinDecal1BlendMode));

			decal1.rgb *= _PSkinDecal1Color.rgb*decal1.a;
			decal0.rgb *= 1-decal1Blend;
		
			half3 debugColor = lerp(half3(0,0,0),lerp(half3(0,1,0),half3(1,0,0),mapTransition),mapTransition);
			half3 realColor = c.rgb*lerp(lerp(occlusion*_PSkinSSSColor,occlusion,occlusion),1,0.25);

			realColor = realColor*(1-max(decal0Blend,decal1Blend));
			realColor = lerp(realColor+decal0.rgb,realColor*lerp(1,decal0.rgb,decal0.a),saturate(_PSkinDecal0BlendMode-1));
			realColor = lerp(realColor+decal1.rgb,realColor*lerp(1,decal1.rgb,decal1.a),saturate(_PSkinDecal1BlendMode-1));

			half3 albedo = lerp(realColor,debugColor,_PSkinDebugTensionMap);
			
			o.Albedo = albedo;
			o.Translucency = half4(_PSkinSSSColor.rgb, transTex.r*_PSkinTranslucencyLevel);
			o.Occlusion = occlusion;

			decal0Spec *= _PSkinDecal0SpecColor*decal0.a;
			decal1Spec *= _PSkinDecal1SpecColor*decal1.a;

			o.Specular = spec.rgb*_PSkinSpecColor.rgb*microSkin.b;
			
			
			o.Specular = o.Specular*(1-max(decal0Blend,decal1Blend));
			o.Specular = lerp(o.Specular+decal0Spec.rgb,o.Specular*lerp(1,decal0Spec.rgb,decal0.a),saturate(_PSkinDecal0BlendMode-1));
			o.Specular = lerp(o.Specular+decal1Spec.rgb,o.Specular*lerp(1,decal1Spec.rgb,decal1.a),saturate(_PSkinDecal1BlendMode-1));


			#if defined(UNITY_PASS_DEFERRED)

			half3 lightCol = half3(0,0,0);
			half3 lightTrans = half3(0,0,0);

			for (int i = 0; i < 4; i++){
				lightCol += DefLightPass(_PSkinDefLightsPosDir[i],_PSkinDefLightsColor[i], _PSkinDefLightsData[i], IN.vNormal.xyz, _PSkinViewDir.xyz, _PSkinWorldPos, c.rgb*occlusion, _PSkinSSSColor, transTex.r*_PSkinTranslucencyLevel );		
				lightTrans += DefLightTrans(_PSkinDefLightsPosDir[i],_PSkinDefLightsColor[i], _PSkinDefLightsData[i], IN.vNormal.xyz, _PSkinViewDir.xyz, _PSkinWorldPos,  c.rgb*occlusion, _PSkinSSSColor, transTex.r*_PSkinTranslucencyLevel );
			}
	
	
			o.Albedo += lightCol;
			o.Albedo *= lerp(saturate(_PSkinSSSColor*c.rgb*3),1.4,0.7);
			
			o.Emission = lightTrans;
			
			o.Smoothness = lerp(spec.a * _PSkinGlossiness * microSkin.b * 0.5, saturate(microSkin.b * _PSkinMicroSkinSmoothness * 0.5 + spec.a * _PSkinGlossiness * microSkin.b * 0.5), 1 - pow(abs(dot(IN.vNormal, IN.viewDirW)), _PSkinDualLobeTransition));

			o.Smoothness = o.Smoothness*(1-max(decal0Blend,decal1Blend));
			o.Smoothness = lerp(o.Smoothness+decal0Spec.a*microSkin.b*0.5,o.Smoothness*lerp(1,decal0Spec.a*microSkin.b*0.5,decal0.a),saturate(_PSkinDecal0BlendMode-1));
			o.Smoothness = lerp(o.Smoothness+decal1Spec.a*microSkin.b*0.5,o.Smoothness*lerp(1,decal1Spec.a*microSkin.b*0.5,decal1.a),saturate(_PSkinDecal1BlendMode-1));

			#else
			o.Smoothness = lerp(spec.a * _PSkinGlossiness * microSkin.b, saturate(microSkin.b * _PSkinMicroSkinSmoothness + spec.a * _PSkinGlossiness * microSkin.b), 1 - pow(abs(dot(IN.vNormal, IN.viewDirW)), _PSkinDualLobeTransition));

			o.Smoothness = o.Smoothness*(1-max(decal0Blend,decal1Blend));
			o.Smoothness = lerp(o.Smoothness+decal0Spec.a*microSkin.b,o.Smoothness*lerp(1,decal0Spec.a*microSkin.b,decal0.a),saturate(_PSkinDecal0BlendMode-1));
			o.Smoothness = lerp(o.Smoothness+decal1Spec.a*microSkin.b,o.Smoothness*lerp(1,decal1Spec.a*microSkin.b,decal1.a),saturate(_PSkinDecal1BlendMode-1));
			#endif

			
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
