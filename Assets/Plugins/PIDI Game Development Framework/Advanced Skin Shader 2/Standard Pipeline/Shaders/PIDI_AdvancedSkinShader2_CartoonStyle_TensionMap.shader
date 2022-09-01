Shader "PIDI Shaders Collection/Advanced Skin Shader 2/Cartoony/TensionMap" {
	Properties {
		[Space(8)][Header(PBR Parameters)][Space(8)]
		[PerRendererData]_PSkinColor ("Color", Color) = (1,1,1,1)
		[NoScaleOffset][PerRendererData]_PSkinMainTex ("Main Texture", 2D) = "white" {}
		[NoScaleOffset][PerRendererData]_PSkinBaseNormals ("Normalmap", 2D) = "bump" {}
		
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
		[Space(8)][Header(Wrinkle Maps)][Space(8)]
		[PerRendererData]_PSkinWrinklesMap("Wrinkle Normal Map", 2D) = "bump"{}
		[PerRendererData]_PSkinWrinkleOcclusionMap("Wrinkle Occlusion Map", 2D) = "white"{}
		[PerRendererData]_PSkinDebugTensionMap("Debug Tension Map",Float) = 0
		[PerRendererData]_PSkinVertexMinDistance("Vertex Min Distance", Range(0.0005,0.01)) = 0.0045

		[PerRendererData] _PSkinHeadBonePos("Head Bone Position",Vector) = (0,0,0,0)
		[PerRendererData] _PSkinHeadBoneRotC("Head Bone Current Rot",Vector) = (0,0,0,0)

		_CartoonWarpTexture("Cartoon Lighting Color", 2D) = "white"{}

		_PSkinRimLightingColor("Rim Color (RGB) Power(A)", Color ) = (1,1,1,1)
		_PSkinOutline("Outline Color",Color) = (0,0,0,1)
		_PSkinOutlineThickness("Outline Thickness", Range(0,0.05)) = 1
	}
	SubShader {

		Tags { "RenderType"="Opaque" }
		LOD 200
		
       
         Pass {
             Cull Front
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #include "UnityCG.cginc"
             half _PSkinOutlineThickness;
             fixed4 _PSkinOutline;
             struct v2f {
                 float4 pos : SV_POSITION;
             };
             v2f vert (appdata_base v) {
                 v2f o;
                 v.vertex.xyz += v.normal * _PSkinOutlineThickness;
                 v.normal *= -1;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 return o;
             }
             half4 frag (v2f i) : SV_Target
             {
                 return _PSkinOutline;
             }
             ENDCG
         }


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf AdvancedSkinCartoon fullforwardshadows vertex:vert
		#include "CGIncludes/AdvancedSkin2_Core.cginc"
		#include "UnityCG.cginc"
		#pragma target 2.0

		sampler2D _PSkinMainTex;
		sampler2D _PSkinBaseNormals;
		sampler2D _PSkinMicroSkin;
		sampler2D _PSkinSpecMap;
		sampler2D _PSkinOcclusionMap;
		sampler2D _PSkinTranslucencyMap;
		sampler2D _PSkinWrinklesMap;

		struct Input {
			float2 uv_PSkinMainTex;
			float2 uv2_PSkinMicroSkin;
			float4 vNormal:COLOR;
			float3 viewDir;
		};

		half _PSkinMicroSkinTiling;
		half _PSkinGlossiness;
		half _PSkinOcclusionLevel;
		half _PSkinTranslucencyLevel;
		half _PSkinVertexMinDistance;
		half _PSkinDebugTensionMap;
		
		fixed4 _PSkinColor;
		fixed4 _PSkinSpecColor;
		fixed4 _PSkinSSSColor;
		fixed4 _PSkinRimLightingColor;

		float4 _PSkinHeadBonePos;
		float4 _PSkinHeadBoneRotO;
		float4 _PSkinHeadBoneRotC;


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
			
			float3 restPose = float3(v.texcoord1.z,v.texcoord1.w,v.texcoord2.x);
			float3 expPoseA = float3(v.texcoord2.y,v.texcoord2.z,v.texcoord2.w);
			float3 expPoseB = float3(v.texcoord3.x,v.texcoord3.y,v.texcoord3.z);

			float3 vertexPos = mul(unity_ObjectToWorld,v.vertex).xyz-_PSkinHeadBonePos.xyz;
			

			vertexPos = rotateVector(vertexPos,_PSkinHeadBoneRotC);

			float distanceToRestPose = distance(vertexPos.xyz, restPose);
			float distanceToPoseA = distance(vertexPos.xyz, expPoseA)+_PSkinVertexMinDistance;
			float distanceToPoseB = distance(vertexPos.xyz, expPoseB)+_PSkinVertexMinDistance;

			float restPoseToPoseA = distance(restPose, expPoseA);
			float restPoseToPoseB = distance(restPose, expPoseB);

			float nRestToA = distanceToPoseA / restPoseToPoseA;
			float nRestToB = distanceToPoseB / restPoseToPoseB;

			
			v.color = float4(mul(unity_ObjectToWorld,float4(v.normal.x,v.normal.y,v.normal.z, 1)).xyz, saturate(1.2-min(nRestToA,nRestToB)));
		}

		void surf (Input IN, inout SurfaceOutputSkin o) {
			fixed4 c = tex2D (_PSkinMainTex, IN.uv_PSkinMainTex) * _PSkinColor;
			
			half mapTransition = saturate(pow(IN.vNormal.a*1.5,1.5));

			float4 normalA = tex2D(_PSkinBaseNormals,IN.uv_PSkinMainTex);
			float4 normalB = tex2D(_PSkinWrinklesMap, IN.uv_PSkinMainTex);

			half3 baseNormal = UnpackNormalScaled(lerp(normalA,normalB, saturate(mapTransition)), 0.85);

			half4 microSkin = tex2D(_PSkinMicroSkin, IN.uv2_PSkinMicroSkin*_PSkinMicroSkinTiling);
			
			half4 occA = tex2D(_PSkinOcclusionMap, IN.uv_PSkinMainTex);

			half4 spec = tex2D(_PSkinSpecMap, IN.uv_PSkinMainTex);
			half occlusion = lerp(1, lerp(occA.r,occA.a, saturate(mapTransition)), _PSkinOcclusionLevel);
			half4 transTex = tex2D(_PSkinTranslucencyMap, IN.uv_PSkinMainTex);

			
			o.WarpNormal = IN.vNormal.xyz;
			o.Normal = baseNormal; 
			o.Smoothness = spec.a*_PSkinGlossiness*microSkin.b;
			o.Specular = spec.rgb*_PSkinSpecColor.rgb*microSkin.b;
			
			half3 debugColor = lerp(half3(0,0,0),lerp(half3(0,1,0),half3(1,0,0),mapTransition),mapTransition);
			half3 realColor = c.rgb*lerp(lerp(occlusion*_PSkinSSSColor,occlusion,occlusion),1,0.25);
			o.Albedo = lerp(realColor,debugColor,_PSkinDebugTensionMap);
			o.Translucency = half4(_PSkinSSSColor.rgb, transTex.r*_PSkinTranslucencyLevel);
			o.Occlusion = occlusion;
			o.Alpha = 1;
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Albedo += 4*_PSkinRimLightingColor.rgb * _LightColor0 * pow (rim, _PSkinRimLightingColor.a*4);
			o.Emission += 0.1*_PSkinRimLightingColor.rgb * _LightColor0 * pow (rim, _PSkinRimLightingColor.a*4);

			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
