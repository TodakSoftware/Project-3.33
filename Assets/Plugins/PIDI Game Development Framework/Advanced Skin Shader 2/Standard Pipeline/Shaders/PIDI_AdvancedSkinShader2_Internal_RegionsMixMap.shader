Shader "PIDI Shaders Collection/Advanced Skin Shader 2/Internal/RegionsMixMap"
{
	Properties
	{
		_MainTex ("Base Map", 2D) = "white" {}
		_MixToMap("Mix with Map", 2D) = "white" {}
		_RegionsMap("Regions Map", 2D ) = "black"{}

		_RegionsBlendA("Regions Blend A", Vector) = (0,0,0,0)
		_RegionsBlendB("Regions Blend B", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _MixToMap;
			sampler2D _RegionsMap;

			float4 _RegionsBlendA;
			float4 _RegionsBlendB;

			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_MixToMap, i.uv);
				fixed4 reg = tex2D(_RegionsMap, i.uv);

				fixed blending = saturate(reg.r*_RegionsBlendA.x+reg.g*_RegionsBlendA.y+reg.b*_RegionsBlendA.z+(1-reg.a)*_RegionsBlendA.w);

				col = lerp(col,col2, blending);

				return col;
			}
			ENDCG
		}
	}
}
