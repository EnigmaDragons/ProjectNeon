Shader "Hidden/OutlinePlus"
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
			
			#pragma shader_feature OUTLINE_USE_DEPTH
			#pragma shader_feature OUTLINE_USE_NORMALS
			
			sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			uniform sampler2D_float _CameraDepthNormalsTexture;
			
			uniform half _Thickness;
			uniform half4 _EdgeColor;
			uniform half2 _DepthThresholds;
			uniform half2 _NormalsThresholds;
			
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
				
				float offset_positive = +ceil(_Thickness * 0.5);
				float offset_negative = -floor(_Thickness * 0.5);
				float left = _MainTex_TexelSize.x * offset_negative;
				float right = _MainTex_TexelSize.x * offset_positive;
				float top = _MainTex_TexelSize.y * offset_negative;
				float bottom = _MainTex_TexelSize.y * offset_positive;
				float2 uv0 = i.uv + float2(left, top);
				float2 uv1 = i.uv + float2(right, bottom);
				float2 uv2 = i.uv + float2(right, top);
				float2 uv3 = i.uv + float2(left, bottom);
				
				#ifdef OUTLINE_USE_DEPTH
				    float d0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv0);
				    float d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv1);
				    float d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv2);
				    float d3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv3);

					d0 = lerp(Linear01Depth(d0), d0, unity_OrthoParams.w);
					d1 = lerp(Linear01Depth(d1), d1, unity_OrthoParams.w);
					d2 = lerp(Linear01Depth(d2), d2, unity_OrthoParams.w);
					d3 = lerp(Linear01Depth(d3), d3, unity_OrthoParams.w);

					float d = length(float2(d1 - d0, d3 - d2));
					d = smoothstep(_DepthThresholds.x, _DepthThresholds.y, d);
				#else
					float d = 0;
				#endif  // OUTLINE_USE_DEPTH
				
				#ifdef OUTLINE_USE_NORMALS
					float3 n0, n1, n2, n3;
					float depth_ignore;
					DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv0), depth_ignore, n0);
					DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv1), depth_ignore, n1);
					DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv2), depth_ignore, n2);
					DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv3), depth_ignore, n3);
					
					float3 nd1 = n1 - n0;
					float3 nd2 = n3 - n2;
					float n = sqrt(dot(nd1, nd1) + dot(nd2, nd2));
					n = smoothstep(_NormalsThresholds.x, _NormalsThresholds.y, n);
				#else
					float n = 0;
				#endif  // OUTLINE_USE_NORMALS
				
				half g = max(d, n);
				half3 output = lerp(original.rgb, _EdgeColor.rgb, g * _EdgeColor.a);
				return half4(output, original.a);
			}
			ENDCG
			
		}
	}
}
