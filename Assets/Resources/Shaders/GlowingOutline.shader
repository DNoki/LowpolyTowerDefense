Shader "TD/GlowingOutline"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth ("Outline Width", float) = 0.01
		[PowerSlider(3.0)] _OutLightStrength("Transparency", Range(2, 128)) = 15 //光晕强度
	}
	SubShader
	{
		//Tags 
		//{ 
		//	"RenderType"="Opaque" 
		//	"IgnoreProjector"="True"
		//	"Queue" = "Geometry"
		//}
		//LOD 100

		//Pass
		//{
		//	Cull Front

		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
			
		//	#include "UnityCG.cginc"

		//	struct v2f
		//	{
		//		float4 pos : SV_POSITION;
		//	};

		//	uniform float4 _OutlineColor;
		//	uniform float _OutlineWidth;
			
		//	v2f vert (appdata_base v)
		//	{
		//		v2f o;

		//		v.vertex += float4(v.normal * _OutlineWidth, 0);
		//		o.pos = UnityObjectToClipPos(v.vertex);

		//		return o;
		//	}

		//	fixed4 frag (v2f i) : SV_Target
		//	{
		//		return _OutlineColor;
		//	}
		//	ENDCG
		//}

		// 生成光晕
		Pass
		{
			Name "Glow"
			Tags{ "LightMode" = "Always" }
			Cull Front
			Blend SrcAlpha One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag 
			#include "UnityCG.cginc"

			//uniform float4 _Color;
			uniform float4 _OutlineColor;
			uniform float _OutlineWidth;
			uniform float _OutLightStrength;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				//float4 color : COLOR;
				//float4 uv : TEXCOORD0;
				float3 posWorld : TEXCOORD1;
				//UNITY_FOG_COORDS(5)
			};

			v2f vert(appdata_base v)
			{
				v2f o;

				v.vertex.xyz += v.normal * _OutlineWidth;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				i.normal = normalize(i.normal);
				float3 viewDir = normalize(i.posWorld.xyz - _WorldSpaceCameraPos.xyz);
				float4 color = _OutlineColor;
				color.a = pow(saturate(dot(viewDir, i.normal)), _OutLightStrength);
				color.a *= _OutLightStrength * dot(viewDir, i.normal);
				return color;
			}

			ENDCG
		}
		
	}

	FallBack "LightweightPipeline/Standard (Physically Based)"
}
