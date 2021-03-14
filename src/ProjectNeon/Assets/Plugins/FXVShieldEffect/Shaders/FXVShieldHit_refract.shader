// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXV/FXVShieldHitRefract" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_PatternTex("Albedo (RGB)", 2D) = "black" {}
		_PatternScale("PatternScale", Range(0.01,100)) = 1.0

		_RippleTex("Albedo (RGB)", 2D) = "black" {}
		_RippleScale("RippleScale", Range(0.1,10)) = 1.0
		_RippleDistortion("RippleDistortion", Range(0.01,1)) = 1.0

		_HitAttenuation("HitAttenuation", Range(0.01,100)) = 1.0
		_HitPower("HitPower", Range(0.01,20)) = 1.0
		_HitRadius("HitRadius", Range(0.01,20)) = 0.25

		_RefractionScale("RefractionScale", Range(0.001,1)) = 0.05
			
		_HitPos("HitPos", Vector) = (0,0,-0.5)
		_HitTan1("HitTan1", Vector) = (0,1,0)
		_HitTan2("HitTan2", Vector) = (1,0,0)
		_WorldScale("WorldScale", Vector) = (1,1,1)

		_BlendSrcMode("BlendSrcMode", Int) = 0
		_BlendDstMode("BlendDstMode", Int) = 0
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300

		BlendOp Add
		Blend[_BlendSrcMode][_BlendDstMode]
		ZWrite Off

		GrabPass
		{
			"_ShieldGrabTexture"
		}

		Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

			#pragma multi_compile USE_PATTERN_TEXTURE __
			#pragma multi_compile USE_DISTORTION_FOR_PATTERN_TEXTURE __
			#pragma multi_compile USE_REFRACTION __

            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // texture coordinate
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 pos : SV_POSITION; // clip space position
 
				float depth : TEXCOORD3;
				float4 objectSpacePos : TEXCOORD4;
				float4 screenPos : TEXCOORD5;
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.objectSpacePos = v.vertex;

				o.screenPos = ComputeScreenPos(o.pos);

                float3 p = UnityObjectToViewPos(v.vertex);
				o.depth = -p.z;

                return o;
            }
            
			sampler2D _PatternTex;
			sampler2D _RippleTex;
			sampler2D _ShieldGrabTexture;
            fixed4 _Color;
			float _PatternScale;
			float3 _HitPos;
			float3 _HitTan1;
			float3 _HitTan2;
			float3 _WorldScale;
			float _HitAttenuation;
			float _HitPower;
			float _HitRadius;
			float _HitShieldCovering;
			float _RippleScale;
			float _RippleDistortion;
			float _RefractionScale;

			struct fragOutput 
			{
				fixed4 color0 : SV_Target;
			};

            fragOutput frag (v2f i)
			{
				float3 diff = _HitPos - i.objectSpacePos;

				float distOrg = length(diff);

				diff.x *= _WorldScale.x;
				diff.y *= _WorldScale.y;
				diff.z *= _WorldScale.z;

				float dist = length(diff);

				fixed4 pattern = fixed4(1.0, 1.0, 1.0, 1.0);

#if USE_REFRACTION
				float4 grabCoords = UNITY_PROJ_COORD(i.screenPos);
#endif

#if USE_PATTERN_TEXTURE
	#if USE_DISTORTION_FOR_PATTERN_TEXTURE
				float3 dirHit = diff / dist;
				float2 dirFX = normalize(float2(dot(dirHit, _HitTan1), dot(dirHit, _HitTan2)));

				fixed4 ripple = tex2D(_RippleTex, float2(distOrg*_Color.a*_RippleScale, 0.5));// +diff*_Color.a);
				pattern = tex2D(_PatternTex, i.uv*_PatternScale +_RippleDistortion*dirFX*ripple.r);

		#if USE_REFRACTION
				grabCoords.xy += _RippleDistortion*dirFX*ripple.r*_HitShieldCovering*_RefractionScale;
		#endif
	#else
				pattern = tex2D(_PatternTex, i.uv*_PatternScale);

		#if USE_REFRACTION
				grabCoords.xy += pattern.r*_RefractionScale;
		#endif
	#endif
#endif
				float att = (1.0 - min(dist / _HitRadius, 1.0));

				fragOutput o;
#if USE_REFRACTION
				fixed4 grabColor = tex2Dproj(_ShieldGrabTexture, grabCoords);

				o.color0.rgb = grabColor.rgb + pattern.rgb * _Color.rgb * _HitPower * att * _Color.a;
				o.color0.a = clamp(0.0, 1.0, _HitPower * att * _HitShieldCovering * _Color.a);
#else
				o.color0 = pattern * _Color * _HitPower * att;
#endif
                return o;
            }
            ENDCG
        }
	}
	CustomEditor "FXVShieldHitMaterialEditor"
	
	FallBack "Diffuse"
}
