Shader "Hidden/FogPlus"
{
	Properties
	{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" { }
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always Fog
		{
			Mode Off
		}
		
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			
			sampler2D _MainTex;
			
			sampler2D _DistanceLUT;
			fixed _Near;
			fixed _Far;
			half _UseDistanceFog;
			half _UseDistanceFogOnSky;
			
			sampler2D _HeightLUT;
			fixed _LowWorldY;
			fixed _HighWorldY;
			half _UseHeightFog;
			half _UseHeightFogOnSky;
			
			fixed _DistanceFogIntensity;
			fixed _HeightFogIntensity;
			fixed _DistanceHeightBlend;
			
			#define ALMOST_ONE 0.999
			
			struct v2f
			{
				float2 uv: TEXCOORD0;
				float4 vertex: SV_POSITION;
				float2 depth_uv: TEXCOORD1;
				float3 screen_pos: TEXCOORD2;
			};
			
			v2f vert(appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screen_pos = ComputeScreenPos(o.vertex);
				o.uv = v.texcoord;
				o.depth_uv = v.texcoord;
				return o;
			}
			
			fixed4 frag(v2f i): SV_Target
			{
				fixed4 original = tex2D(_MainTex, i.uv);
				
				float depthPacked = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.depth_uv);
				float depthEye = LinearEyeDepth(depthPacked);
				float depthCameraPlanes = Linear01Depth(depthPacked);
				float depthAbsolute = _ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y) * depthCameraPlanes;
				float depthFogPlanes = saturate((depthAbsolute - _Near) / (_Far - _Near));
				float isSky = step(ALMOST_ONE, depthCameraPlanes);
				
				float4 distanceFog = tex2D(_DistanceLUT, float2(depthFogPlanes, 0.5));
				distanceFog.a *= step(isSky, _UseDistanceFogOnSky);
				distanceFog.a *= _UseDistanceFog * _DistanceFogIntensity;
				
				float3 worldPos = i.screen_pos * depthEye + _WorldSpaceCameraPos;
				float heightUV = saturate((worldPos.y - _LowWorldY) / (_HighWorldY - _LowWorldY));
				fixed4 heightFog = tex2D(_HeightLUT, float2(heightUV, 0.5));
				heightFog.a *= step(isSky, _UseHeightFogOnSky);
				heightFog.a *= _UseHeightFog * _HeightFogIntensity;
				
				fixed fogBlend = _DistanceHeightBlend;
				if (!_UseDistanceFog) fogBlend = 1.0;
				if(!_UseHeightFog) fogBlend = 0.0;
				fixed4 fog = lerp(distanceFog, heightFog, fogBlend);
				
				fixed4 final = lerp(original, fog, fog.a);
				final.a = original.a;
				return final;
			}
			ENDCG
			
		}
	}
}
