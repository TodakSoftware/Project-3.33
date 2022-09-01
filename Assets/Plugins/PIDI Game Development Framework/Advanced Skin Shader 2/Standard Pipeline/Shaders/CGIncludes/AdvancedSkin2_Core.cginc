#include "UnityPBSLighting.cginc"

inline fixed3 UnpackMixedNormal(fixed2 packednormal, float scale) {
	fixed3 normal;
	normal.xy = (packednormal.xy * 2 - 1) * scale;
	normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
	return normal;
}

inline fixed3 UnpackNormalScaled(fixed4 packednormal, float scale) {
#ifndef UNITY_NO_DXT5nm
	// Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
	// Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
	packednormal.x *= packednormal.w;
#endif
	fixed3 normal;
	normal.xy = (packednormal.xy * 2 - 1) * scale;
	normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
	return normal;
}

struct SurfaceOutputSkin {
	half3 Albedo;
	half3 Normal;
	half3 WarpNormal;
	half SSSWarp;
	half Smoothness;
	half3 Specular;
	half Occlusion;
	half4 Translucency;
	half3 Emission;
	half Alpha;
};

sampler2D _CartoonWarpTexture;

half4 PIDI_SSS2_BRDF(half3 diffColor, half3 specColor, half4 sssData, half oneMinusReflectivity, half smoothness, half3 normal, half3 vNormal, half3 viewDir, UnityLight light, UnityIndirect gi) {

	float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);


#if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV
	// The amount we shift the normal toward the view vector is defined by the dot product.
	half shiftAmount = dot(normal, viewDir);
	normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f) : normal;

	shiftAmount = dot(vNormal, viewDir);
	vNormal = shiftAmount < 0.0f ? vNormal + viewDir * (-shiftAmount + 1e-5f) : vNormal;

	// A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
	//normal = normalize(normal);

	half nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
#else
	half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
#endif

	float3 sssColor = sssData.rgb;
	float translucency = sssData.a;

	half3 softNormal = lerp(normal,vNormal,saturate(pow(1-dot(normal, light.dir),24)));
	half realNL = saturate(dot(normal, light.dir));
	half nl = saturate(dot(softNormal, light.dir)*0.65+0.18);
	half warpNL = saturate(dot(normal, light.dir)*0.75+0.15 );


	float nh = saturate(dot(normal, halfDir));
	float lh = saturate(dot(light.dir, halfDir));

	half perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
	half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX

	half a = roughness;
	float a2 = a * a;

	float d = nh * nh * (a2 - 1.f) + 1.00001f;
#ifdef UNITY_COLORSPACE_GAMMA
	float specularTerm = a / (max(0.32f, lh) * (1.5f + roughness) * d);
#else
	float specularTerm = a2 / (max(0.1f, lh*lh) * (roughness + 0.5f) * (d * d) * 4);
#endif

#if defined (SHADER_API_MOBILE)
	specularTerm = specularTerm - 1e-4f;
#endif

#else

	
	half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);

	half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness;
	half invF = lh;

	half specularTerm = ((specularPower + 1) * pow(nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
	specularTerm = sqrt(max(1e-4f, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
	specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
	specularTerm = 0.0;
#endif


#ifdef UNITY_COLORSPACE_GAMMA
	half surfaceReduction = 0.28;
#else
	half surfaceReduction = (0.6 - 0.08*perceptualRoughness);
#endif
	
	half3 rampL = tex2D(_CartoonWarpTexture,half2(warpNL,0)).rgb;

	half3 scatter = diffColor*rampL;

	

	half3 transColor = nv*saturate(lerp(pow(diffColor,4)*2,pow(sssColor,3)*1.5,saturate(pow(dot(light.dir,viewDir),4))))*(1-nl);

	surfaceReduction = 1.0 - roughness * perceptualRoughness*surfaceReduction;
	half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
	half3 color = (saturate(scatter*2)  + specularTerm * specColor) * length(rampL*0.65) * light.color
		+ transColor*translucency*light.color
		+ gi.diffuse * diffColor;
	return half4(color, 1);
}


half4 PIDI_SSS_BRDF(half3 diffColor, half3 specColor, half4 sssData, half oneMinusReflectivity, half smoothness, half3 normal, half3 vNormal, half3 viewDir, UnityLight light, UnityIndirect gi) {

	float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);


#if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV
	// The amount we shift the normal toward the view vector is defined by the dot product.
	half shiftAmount = dot(normal, viewDir);
	normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f) : normal;

	shiftAmount = dot(vNormal, viewDir);
	vNormal = shiftAmount < 0.0f ? vNormal + viewDir * (-shiftAmount + 1e-5f) : vNormal;

	// A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
	//normal = normalize(normal);

	half nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
#else
	half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
#endif

	float3 sssColor = sssData.rgb;
	float translucency = sssData.a;

	half3 softNormal = lerp(normal,vNormal,saturate(pow(1-dot(normal, light.dir),24)));
	half realNL = saturate(dot(normal, light.dir));
	half nl = saturate(dot(softNormal, light.dir)*0.65+0.18);
	half warpNL = saturate(dot(normal, light.dir)*0.75+0.15 );

	float nh = saturate(dot(normal, halfDir));
	float lh = saturate(dot(light.dir, halfDir));

	half perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
	half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX

	half a = roughness;
	float a2 = a * a;

	float d = nh * nh * (a2 - 1.f) + 1.00001f;
#ifdef UNITY_COLORSPACE_GAMMA
	float specularTerm = a / (max(0.32f, lh) * (1.5f + roughness) * d);
#else
	float specularTerm = a2 / (max(0.1f, lh*lh) * (roughness + 0.5f) * (d * d) * 4);
#endif

#if defined (SHADER_API_MOBILE)
	specularTerm = specularTerm - 1e-4f;
#endif

#else

	
	half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);

	half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness;
	half invF = lh;

	half specularTerm = ((specularPower + 1) * pow(nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
	specularTerm = sqrt(max(1e-4f, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
	specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
	specularTerm = 0.0;
#endif


#ifdef UNITY_COLORSPACE_GAMMA
	half surfaceReduction = 0.28;
#else
	half surfaceReduction = (0.6 - 0.08*perceptualRoughness);
#endif

	half3 scatter = lerp(diffColor,diffColor*sssColor, saturate(pow(1-warpNL,2)));

	half3 transColor = nv*saturate(lerp(pow(diffColor,4)*2,pow(sssColor,3)*1.5,saturate(pow(dot(light.dir,viewDir),4))))*(1-nl);

	surfaceReduction = 1.0 - roughness * perceptualRoughness*surfaceReduction;
	half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
	half3 color = (saturate(scatter*2)  + specularTerm * specColor)* warpNL * light.color
		+ transColor*translucency*light.color
		+ gi.diffuse * diffColor
		+ surfaceReduction * gi.specular * FresnelLerpFast(specColor, grazingTerm, nv);
	return half4(color, 1);
}



inline half3 DefLightPass(float4 lightPosDir, float4 lightColor, float4 lightData, float3 normal, float3 viewDir, float3 worldPos, float3 Albedo, float3 SSSColor, float Translucency ){
	
	half3 softNormal = normal;

	half3 lightDir = lerp(lightPosDir.xyz,normalize(lightPosDir.xyz-worldPos),saturate(lightPosDir.w));
	
	half realNL = saturate(dot(softNormal, lightDir));
	half nl = saturate(dot(softNormal, lightDir)*0.65+0.3);
	half nv = abs(dot(normal, viewDir));
	half lv = dot(lightDir,viewDir);

	half dist = distance(lightPosDir.xyz,worldPos.xyz);
	half lightAtten = lerp(1,saturate(lightColor.w)*pow(1-(dist/lightData.x),2), saturate(lightPosDir.w));

	half3 scatter = lerp(0,saturate(Albedo*2*SSSColor), saturate(pow(1-realNL,2.5))) * lightColor.rgb * lightAtten * pow(nl,1);

	return saturate(scatter*1.3);
}


inline half3 DefLightTrans(float4 lightPosDir, float4 lightColor, float4 lightData, float3 normal, float3 viewDir, float3 worldPos, float3 Albedo, float3 SSSColor, float Translucency ){
	
	half3 softNormal = normal;

	half3 lightDir = lerp(lightPosDir.xyz,normalize(lightPosDir.xyz-worldPos),saturate(lightPosDir.w));
	
	half realNL = saturate(dot(softNormal, lightDir));
	half nl = saturate(dot(softNormal, lightDir)*0.65+0.3);
	half nv = abs(dot(normal, viewDir));
	half lv = saturate(dot(lightDir,viewDir));
	half invLV = dot(-lightDir,viewDir);

	half dist = distance(lightPosDir.xyz,worldPos.xyz);

	half lightShadow = lerp(1,dot(lightDir,viewDir),lightData.y);
	half lightAtten = lerp(1,saturate(lightColor.w)*pow(1-(dist/lightData.x),2), saturate(lightPosDir.w));


	half3 transColor = nv*saturate(lerp(pow(Albedo,4)*2,pow(SSSColor,2)*1.5,saturate(pow(invLV,4))))*pow(1-realNL,2)* lightColor.rgb * lightShadow * lightAtten ;
	//half3 transColor = saturate(lerp(pow(Albedo,4)*2,pow(SSSColor,2)*1.5,saturate(pow(1-abs(invLV),4))))*dot(1-lightDir,viewDir);
	return saturate(transColor*Translucency);
}

inline half4 LightingAdvancedSkin(SurfaceOutputSkin s, half3 viewDir, UnityGI gi) {
	//s.Normal = normalize(s.Normal);

	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular(s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);
	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	half4 c = PIDI_SSS_BRDF(s.Albedo, s.Specular, s.Translucency, oneMinusReflectivity, s.Smoothness, s.Normal, s.WarpNormal, viewDir, gi.light, gi.indirect);
	c.a = outputAlpha;
	return c;
}

inline half4 LightingAdvancedSkinCartoon(SurfaceOutputSkin s, half3 viewDir, UnityGI gi) {
	//s.Normal = normalize(s.Normal);

	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular(s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);
	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	half4 c = PIDI_SSS2_BRDF(s.Albedo, s.Specular, s.Translucency, oneMinusReflectivity, s.Smoothness, s.Normal, s.WarpNormal, viewDir, gi.light, gi.indirect);
	c.a = outputAlpha;
	return c;
}


inline half4 LightingAdvancedSkin_Deferred(SurfaceOutputSkin s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal) {
	// energy conservation

	half3 transColor = pow(saturate(normalize(s.Albedo)), 1.2)*1.3;

	//half3 specColor = lerp(transColor*s.Specular*2,s.Specular*2,0.5);

	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular(s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	half4 c = PIDI_SSS_BRDF(s.Albedo, s.Specular, s.Translucency, oneMinusReflectivity, s.Smoothness, s.Normal, s.WarpNormal, viewDir, gi.light, gi.indirect);


	outDiffuseOcclusion = half4(s.Albedo + c.rgb, s.Occlusion);
	outSpecSmoothness = half4(s.Specular * 2, s.Smoothness * 2);
	outNormal = half4(s.Normal * 0.5 + 0.5, 1);



	half4 emission = half4(s.Emission + c.rgb, 1);
	return emission;
}

inline void LightingAdvancedSkin_GI(SurfaceOutputSkin s, UnityGIInput data, inout UnityGI gi) {
	
	UNITY_GI(gi, s, data);
}

inline void LightingAdvancedSkinCartoon_GI(SurfaceOutputSkin s, UnityGIInput data, inout UnityGI gi) {
	
	UNITY_GI(gi, s, data);
}