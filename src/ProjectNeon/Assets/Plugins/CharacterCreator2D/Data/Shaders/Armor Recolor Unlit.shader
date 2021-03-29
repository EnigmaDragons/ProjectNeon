// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CC2D/Unlit/Armor Recolor"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_ColorMask("Color Mask", 2D) = "black" {}
		_Color1("Color 1", Color) = (0.4980392,0.4980392,0.4980392,1)
		_Color2("Color 2", Color) = (0.4431373,0.4431373,0.4431373,1)
		_Color3("Color 3", Color) = (0.4431373,0.4431373,0.4431373,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color1;
			uniform sampler2D _ColorMask;
			uniform float4 _ColorMask_ST;
			uniform fixed4 _Color2;
			uniform fixed4 _Color3;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode11 = tex2D( _MainTex, uv_MainTex );
				float2 uv_ColorMask = IN.texcoord.xy * _ColorMask_ST.xy + _ColorMask_ST.zw;
				float4 tex2DNode2 = tex2D( _ColorMask, uv_ColorMask );
				float4 lerpResult42 = lerp( ( fixed4(0.4980392,0.4980392,0.4980392,1) * tex2DNode11 ) , ( _Color1 * tex2DNode11 ) , tex2DNode2.r);
				float4 lerpResult43 = lerp( lerpResult42 , ( _Color2 * tex2DNode11 ) , tex2DNode2.g);
				float4 lerpResult44 = lerp( lerpResult43 , ( _Color3 * tex2DNode11 ) , tex2DNode2.b);
				float4 appendResult129 = (float4(( 2.0 * lerpResult44 ).rgb , tex2DNode11.a));
				
				fixed4 c = ( appendResult129 * _Color );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}