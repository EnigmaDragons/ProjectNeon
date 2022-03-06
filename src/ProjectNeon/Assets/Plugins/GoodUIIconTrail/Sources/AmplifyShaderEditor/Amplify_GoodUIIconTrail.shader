// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShader/GoodUIIconTrail"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_ColorOverlay("Color Overlay", Color) = (1,1,1,1)
		_GlobalAlphaIntensity("Global Alpha Intensity", Range( 0 , 5)) = 2
		_GlobalAlphaFalloff("Global Alpha Falloff", Range( 1 , 5)) = 1
		_TrailLengthFalloff("Trail Length Falloff", Range( 1 , 10)) = 1.5
		_HighLightIntensity("High Light Intensity", Range( 0 , 3)) = 0.1
		_HighLightFalloff("High Light Falloff", Range( 1 , 5)) = 1
		_TimeSpeed("Time Speed", Range( 0 , 5)) = 1
		[IntRange]_TrailNumber("Trail Number", Range( 1 , 4)) = 2
		[Toggle(_USECLOCKWISE_ON)] _UseClockwise("Use Clockwise ?", Float) = 0
		[Toggle(_ENABLESECONDALPHASHADE_ON)] _EnableSecondAlphaShade("Enable Second AlphaShade", Float) = 0
		[Toggle(_INVERTSECONDALPHASHADE_ON)] _InvertSecondAlphaShade("Invert Second AlphaShade?", Float) = 0
		[Toggle(_USEDISTORTION_ON)] _UseDistortion("Use Distortion?", Float) = 0
		_DistortionIntensity("Distortion Intensity", Range( 0 , 0.25)) = 0
		_BaseTexture("Base Texture", 2D) = "white" {}
		_RotatingTexture("Rotating Texture", 2D) = "white" {}
		_SecondIconAlphaTexture("Second Icon Alpha Texture", 2D) = "white" {}
		[HideInInspector]_T_CloudNoise("T_CloudNoise", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"
			#pragma shader_feature _USEDISTORTION_ON
			#pragma shader_feature _USECLOCKWISE_ON
			#pragma shader_feature _ENABLESECONDALPHASHADE_ON
			#pragma shader_feature _INVERTSECONDALPHASHADE_ON

			
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
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _ColorOverlay;
			uniform sampler2D _BaseTexture;
			uniform sampler2D _T_CloudNoise;
			uniform float _DistortionIntensity;
			uniform float _HighLightIntensity;
			uniform float _HighLightFalloff;
			uniform float _TrailNumber;
			uniform sampler2D _RotatingTexture;
			uniform float _TimeSpeed;
			uniform float _TrailLengthFalloff;
			uniform float _GlobalAlphaIntensity;
			uniform float _GlobalAlphaFalloff;
			uniform sampler2D _SecondIconAlphaTexture;
			uniform float4 _SecondIconAlphaTexture_ST;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float3 appendResult139 = (float3(_ColorOverlay.r , _ColorOverlay.g , _ColorOverlay.b));
				float2 uv0127 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv0123 = IN.texcoord.xy * float2( 1.5,1.5 ) + float2( 0,0 );
				float2 panner122 = ( 1.0 * _Time.y * float2( 0.1,0.1 ) + uv0123);
				float lerpResult162 = lerp( -0.25 , 0.15 , tex2D( _T_CloudNoise, panner122 ).r);
				float2 appendResult125 = (float2(lerpResult162 , lerpResult162));
				#ifdef _USEDISTORTION_ON
				float2 staticSwitch114 = ( uv0127 + ( appendResult125 * _DistortionIntensity ) );
				#else
				float2 staticSwitch114 = uv0127;
				#endif
				float4 tex2DNode84 = tex2D( _BaseTexture, staticSwitch114 );
				float temp_output_135_0 = pow( ( tex2DNode84.r * _HighLightIntensity ) , _HighLightFalloff );
				float3 appendResult138 = (float3(temp_output_135_0 , temp_output_135_0 , temp_output_135_0));
				float2 uv016 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult20 = (float4(( 1.0 - uv016.x ) , uv016.y , 0.0 , 0.0));
				#ifdef _USECLOCKWISE_ON
				float4 staticSwitch21 = appendResult20;
				#else
				float4 staticSwitch21 = float4( uv016, 0.0 , 0.0 );
				#endif
				float mulTime57 = _Time.y * ( _TimeSpeed * 3.0 );
				float cos13 = cos( mulTime57 );
				float sin13 = sin( mulTime57 );
				float2 rotator13 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos13 , -sin13 , sin13 , cos13 )) + float2( 0.5,0.5 );
				float4 tex2DNode12 = tex2D( _RotatingTexture, rotator13 );
				float cos55 = cos( ( mulTime57 + 3.15 ) );
				float sin55 = sin( ( mulTime57 + 3.15 ) );
				float2 rotator55 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos55 , -sin55 , sin55 , cos55 )) + float2( 0.5,0.5 );
				float4 tex2DNode56 = tex2D( _RotatingTexture, rotator55 );
				float cos75 = cos( ( mulTime57 + 1.575 ) );
				float sin75 = sin( ( mulTime57 + 1.575 ) );
				float2 rotator75 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos75 , -sin75 , sin75 , cos75 )) + float2( 0.5,0.5 );
				float cos77 = cos( ( mulTime57 + 4.725 ) );
				float sin77 = sin( ( mulTime57 + 4.725 ) );
				float2 rotator77 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos77 , -sin77 , sin77 , cos77 )) + float2( 0.5,0.5 );
				float temp_output_79_0 = ( tex2DNode12.r + tex2DNode56.r + tex2D( _RotatingTexture, rotator75 ).r + tex2D( _RotatingTexture, rotator77 ).r );
				float cos62 = cos( ( mulTime57 + 2.1 ) );
				float sin62 = sin( ( mulTime57 + 2.1 ) );
				float2 rotator62 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos62 , -sin62 , sin62 , cos62 )) + float2( 0.5,0.5 );
				float cos64 = cos( ( mulTime57 + 4.2 ) );
				float sin64 = sin( ( mulTime57 + 4.2 ) );
				float2 rotator64 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos64 , -sin64 , sin64 , cos64 )) + float2( 0.5,0.5 );
				float temp_output_70_0 = ( tex2DNode12.r + tex2D( _RotatingTexture, rotator62 ).r + tex2D( _RotatingTexture, rotator64 ).r );
				float ifLocalVar53 = 0;
				if( _TrailNumber >= 4.0 )
				ifLocalVar53 = temp_output_79_0;
				else
				ifLocalVar53 = temp_output_70_0;
				float temp_output_61_0 = ( tex2DNode12.r + tex2DNode56.r );
				float ifLocalVar51 = 0;
				if( _TrailNumber > 3.0 )
				ifLocalVar51 = ifLocalVar53;
				else if( _TrailNumber == 3.0 )
				ifLocalVar51 = temp_output_70_0;
				else if( _TrailNumber < 3.0 )
				ifLocalVar51 = temp_output_61_0;
				float ifLocalVar49 = 0;
				if( _TrailNumber > 2.0 )
				ifLocalVar49 = ifLocalVar51;
				else if( _TrailNumber == 2.0 )
				ifLocalVar49 = temp_output_61_0;
				else if( _TrailNumber < 2.0 )
				ifLocalVar49 = tex2DNode12.r;
				float ifLocalVar25 = 0;
				if( _TrailNumber <= 1.0 )
				ifLocalVar25 = tex2DNode12.r;
				else
				ifLocalVar25 = ifLocalVar49;
				float clampResult113 = clamp( pow( pow( ifLocalVar25 , 1.1 ) , _TrailLengthFalloff ) , 0.0 , 1.0 );
				float clampResult145 = clamp( pow( ( ( ( tex2DNode84.r * clampResult113 ) * 1.5 ) * _GlobalAlphaIntensity ) , _GlobalAlphaFalloff ) , 0.0 , 1.0 );
				float2 uv_SecondIconAlphaTexture = IN.texcoord.xy * _SecondIconAlphaTexture_ST.xy + _SecondIconAlphaTexture_ST.zw;
				float4 tex2DNode91 = tex2D( _SecondIconAlphaTexture, uv_SecondIconAlphaTexture );
				#ifdef _INVERTSECONDALPHASHADE_ON
				float staticSwitch87 = ( 1.0 - tex2DNode91.r );
				#else
				float staticSwitch87 = tex2DNode91.r;
				#endif
				#ifdef _ENABLESECONDALPHASHADE_ON
				float staticSwitch88 = staticSwitch87;
				#else
				float staticSwitch88 = 1.0;
				#endif
				float4 appendResult141 = (float4(( appendResult139 + appendResult138 ) , ( clampResult145 * staticSwitch88 )));
				
				half4 color = appendResult141;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16700
2318.491;31.69811;1727;926;-846.6635;724.8807;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;97;-4065.526,-392.072;Float;False;3913.256;1809.133;Rotate Trail Alpha;52;21;20;19;16;108;25;49;27;50;51;53;61;52;70;26;79;54;78;76;65;56;63;12;13;75;62;104;64;55;77;74;95;67;103;72;69;107;71;102;73;105;66;96;106;68;57;100;99;60;83;159;160;Rotate Trail Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;83;-2704.183,-290.1916;Float;True;Property;_RotatingTexture;Rotating Texture;14;0;Create;True;0;0;False;0;022fc00a43c652a4abbd611547159c55;022fc00a43c652a4abbd611547159c55;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-3915.863,27.45613;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;60;-3913.077,432.4283;Float;False;Property;_TimeSpeed;Time Speed;6;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-3585.917,441.0128;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-3607.515,154.7474;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;99;-2299.559,-17.25276;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;100;-2254.559,226.7472;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleTimeNode;57;-3416.077,438.4282;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-3402.948,170.3374;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2950.029,1230.928;Float;False;Constant;_Float10;Float 10;7;0;Create;True;0;0;False;0;4.725;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-2975.029,1066.928;Float;False;Constant;_Float9;Float 9;7;0;Create;True;0;0;False;0;1.575;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;102;-2202.496,467.9387;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StaticSwitch;21;-3202.333,30.96192;Float;False;Property;_UseClockwise;Use Clockwise ?;8;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-2988.029,692.9285;Float;False;Constant;_Float7;Float 7;7;0;Create;True;0;0;False;0;2.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;105;-3140.48,1092.967;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;107;-3132.128,747.8092;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-3003.438,486.7415;Float;False;Constant;_Float5;Float 5;7;0;Create;True;0;0;False;0;3.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2981.029,833.9287;Float;False;Constant;_Float8;Float 8;7;0;Create;True;0;0;False;0;4.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;106;-3112.644,960.7496;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;108;-2695.936,46.68478;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-2784.73,976.5266;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-2778.094,411.9216;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-2771.029,545.9284;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-2773.029,765.9285;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;103;-2196.465,730.0089;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-2722.029,1163.928;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;159;-2886.623,149.3411;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;75;-2381.855,984.3104;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;77;-2418.199,1189.366;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;104;-2140.465,962.0087;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RotatorNode;55;-2405.504,272.4806;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;13;-2487.394,28.53302;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;62;-2391.939,493.2935;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;64;-2385.939,729.2937;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;129;-2321.483,-1185.624;Float;False;2302.395;664.2067;Distortion UV Setting;10;128;162;114;126;121;127;125;120;122;123;Distortion UV Setting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;56;-2075.854,226.9564;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;76;-2076.119,977.5636;Float;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-2085.498,-18.13444;Float;True;Property;_T_Rotate_Alpha_Sharp;T_Rotate_Alpha_Sharp;0;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;-2272.551,-923.2106;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.5,1.5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;63;-2074.119,469.5632;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2074.74,1202.627;Float;True;Property;_TextureSample4;Texture Sample 4;6;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;65;-2066.703,725.5634;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;ea4e7e49c16b5e8429bf4eae36b10a70;ea4e7e49c16b5e8429bf4eae36b10a70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-1367.875,-88.7952;Float;False;Property;_TrailNumber;Trail Number;7;1;[IntRange];Create;True;0;0;False;0;2;2;1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1371.284,820.8567;Float;False;Constant;_Float4;Float 4;5;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;122;-1913.703,-912.4612;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;-1382.824,939.7138;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-1561.03,502.4282;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1176.344,486.7433;Float;False;Constant;_Float3;Float 3;5;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;53;-1139.562,687.3701;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-1431.448,268.632;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;120;-1622.76,-938.8636;Float;True;Property;_T_CloudNoise;T_CloudNoise;16;1;[HideInInspector];Create;True;0;0;False;0;4f2103b473ff057489cc27cf4b97882f;4f2103b473ff057489cc27cf4b97882f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;162;-1266.193,-900.1655;Float;False;3;0;FLOAT;-0.25;False;1;FLOAT;0.15;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1030.014,232.7588;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;51;-935.0858,477.4862;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;49;-750.0346,224.8672;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-1106.728,-645.6047;Float;False;Property;_DistortionIntensity;Distortion Intensity;12;0;Create;True;0;0;False;0;0;0.1953;0;0.25;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;125;-1050.904,-907.7952;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-721.5906,42.88343;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;25;-485.9927,24.27539;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;149;-109.5134,-104.4923;Float;False;1095.691;297.1073;Base Alpha Setting;4;113;110;111;154;Base Alpha Setting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-735.3678,-767.1103;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;127;-799.2565,-1085.563;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;111;159.4864,109.4073;Float;False;Property;_TrailLengthFalloff;Trail Length Falloff;3;0;Create;True;0;0;False;0;1.5;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;154;-52.12738,24.13674;Float;False;2;0;FLOAT;0;False;1;FLOAT;1.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;130;139.8704,-1099.434;Float;False;770.9084;550.7146;Base Textures;2;84;85;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-550.5152,-843.7781;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;85;207.3222,-1033.773;Float;True;Property;_BaseTexture;Base Texture;13;0;Create;True;0;0;False;0;453c45f7caeade3489c6ac39d07f4e69;6d7ecb969878995468f83a2bc2d2e796;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;109;605.9807,280.8512;Float;False;1912.997;426.4314;Second Icon Mask Setting ;6;88;89;87;92;91;90;Second Icon Mask Setting ;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;110;459.5647,34.88591;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;114;-317.5621,-903.9816;Float;True;Property;_UseDistortion;Use Distortion?;11;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;90;655.9807,476.2827;Float;True;Property;_SecondIconAlphaTexture;Second Icon Alpha Texture;15;0;Create;True;0;0;False;0;0567ec71ab3136944a20404df5e5c47f;0567ec71ab3136944a20404df5e5c47f;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;84;499.2331,-881.4307;Float;True;Property;_T_Laser_Box;T_Laser_Box;5;0;Create;True;0;0;False;0;453c45f7caeade3489c6ac39d07f4e69;453c45f7caeade3489c6ac39d07f4e69;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;148;1336.081,-411.3377;Float;False;1447.254;324.0993;Alpha Falloff;7;164;165;112;161;145;144;143;Alpha Falloff;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;113;832.5526,37.06884;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;146;1491.658,-883.7488;Float;False;991.4525;367.9003;Add HighLight;5;137;134;136;135;138;Add HighLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;165;1441.137,-243.5125;Float;False;Constant;_Float6;Float 6;17;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;153;1161.556,-368.7558;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;983.0848,446.4201;Float;True;Property;_TextureSample5;Texture Sample 5;5;0;Create;True;0;0;False;0;453c45f7caeade3489c6ac39d07f4e69;453c45f7caeade3489c6ac39d07f4e69;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;1608.137,-360.5125;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;1699.032,-229.0123;Float;False;Property;_GlobalAlphaIntensity;Global Alpha Intensity;1;0;Create;True;0;0;False;0;2;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;92;1437.155,558.4113;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;1548.827,-673.14;Float;False;Property;_HighLightIntensity;High Light Intensity;4;0;Create;True;0;0;False;0;0.1;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;2051.821,-358.2762;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;2052.387,-224.9723;Float;False;Property;_GlobalAlphaFalloff;Global Alpha Falloff;2;0;Create;True;0;0;False;0;1;4.44;1;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;147;2021.15,-1166.315;Float;False;286.3774;253.0377;Tint Color;1;39;Tint Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;1900.378,-836.6283;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;87;1676.055,437.3237;Float;False;Property;_InvertSecondAlphaShade;Invert Second AlphaShade?;10;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;1842.533,330.8513;Float;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;1872.658,-666.4453;Float;False;Property;_HighLightFalloff;High Light Falloff;5;0;Create;True;0;0;False;0;1;1;1;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;144;2365.966,-367.9048;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;39;2071.15,-1116.315;Float;False;Property;_ColorOverlay;Color Overlay;0;0;Create;True;0;0;False;0;1,1,1,1;0.05660371,0.05633672,0.05633672,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;135;2158.814,-830.7488;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;88;2129.883,325.6292;Float;False;Property;_EnableSecondAlphaShade;Enable Second AlphaShade;9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;139;2370.964,-1071.341;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;163;2866.712,262.9252;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;145;2629.616,-365.2534;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;138;2347.971,-837.3295;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;2914.264,-369.8154;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;133;2765.021,-1048.408;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;3002.01,-1044.611;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;157;3223.277,-1043.296;Float;False;True;2;Float;ASEMaterialInspector;0;4;AmplifyShader/GoodUIIconTrail;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;160;0;60;0
WireConnection;19;0;16;1
WireConnection;99;0;83;0
WireConnection;100;0;99;0
WireConnection;57;0;160;0
WireConnection;20;0;19;0
WireConnection;20;1;16;2
WireConnection;102;0;100;0
WireConnection;21;1;16;0
WireConnection;21;0;20;0
WireConnection;105;0;57;0
WireConnection;107;0;57;0
WireConnection;106;0;57;0
WireConnection;108;0;21;0
WireConnection;72;0;106;0
WireConnection;72;1;71;0
WireConnection;95;0;57;0
WireConnection;95;1;96;0
WireConnection;67;0;57;0
WireConnection;67;1;66;0
WireConnection;69;0;107;0
WireConnection;69;1;68;0
WireConnection;103;0;102;0
WireConnection;74;0;105;0
WireConnection;74;1;73;0
WireConnection;159;0;57;0
WireConnection;75;0;108;0
WireConnection;75;2;72;0
WireConnection;77;0;108;0
WireConnection;77;2;74;0
WireConnection;104;0;103;0
WireConnection;55;0;108;0
WireConnection;55;2;95;0
WireConnection;13;0;108;0
WireConnection;13;2;159;0
WireConnection;62;0;108;0
WireConnection;62;2;67;0
WireConnection;64;0;108;0
WireConnection;64;2;69;0
WireConnection;56;0;100;0
WireConnection;56;1;55;0
WireConnection;76;0;104;0
WireConnection;76;1;75;0
WireConnection;12;0;99;0
WireConnection;12;1;13;0
WireConnection;63;0;102;0
WireConnection;63;1;62;0
WireConnection;78;0;104;0
WireConnection;78;1;77;0
WireConnection;65;0;103;0
WireConnection;65;1;64;0
WireConnection;122;0;123;0
WireConnection;79;0;12;1
WireConnection;79;1;56;1
WireConnection;79;2;76;1
WireConnection;79;3;78;1
WireConnection;70;0;12;1
WireConnection;70;1;63;1
WireConnection;70;2;65;1
WireConnection;53;0;26;0
WireConnection;53;1;54;0
WireConnection;53;2;79;0
WireConnection;53;3;79;0
WireConnection;53;4;70;0
WireConnection;61;0;12;1
WireConnection;61;1;56;1
WireConnection;120;1;122;0
WireConnection;162;2;120;1
WireConnection;51;0;26;0
WireConnection;51;1;52;0
WireConnection;51;2;53;0
WireConnection;51;3;70;0
WireConnection;51;4;61;0
WireConnection;49;0;26;0
WireConnection;49;1;50;0
WireConnection;49;2;51;0
WireConnection;49;3;61;0
WireConnection;49;4;12;1
WireConnection;125;0;162;0
WireConnection;125;1;162;0
WireConnection;25;0;26;0
WireConnection;25;1;27;0
WireConnection;25;2;49;0
WireConnection;25;3;12;1
WireConnection;25;4;12;1
WireConnection;121;0;125;0
WireConnection;121;1;128;0
WireConnection;154;0;25;0
WireConnection;126;0;127;0
WireConnection;126;1;121;0
WireConnection;110;0;154;0
WireConnection;110;1;111;0
WireConnection;114;1;127;0
WireConnection;114;0;126;0
WireConnection;84;0;85;0
WireConnection;84;1;114;0
WireConnection;113;0;110;0
WireConnection;153;0;84;1
WireConnection;153;1;113;0
WireConnection;91;0;90;0
WireConnection;164;0;153;0
WireConnection;164;1;165;0
WireConnection;92;0;91;1
WireConnection;112;0;164;0
WireConnection;112;1;161;0
WireConnection;136;0;84;1
WireConnection;136;1;137;0
WireConnection;87;1;91;1
WireConnection;87;0;92;0
WireConnection;144;0;112;0
WireConnection;144;1;143;0
WireConnection;135;0;136;0
WireConnection;135;1;134;0
WireConnection;88;1;89;0
WireConnection;88;0;87;0
WireConnection;139;0;39;1
WireConnection;139;1;39;2
WireConnection;139;2;39;3
WireConnection;163;0;88;0
WireConnection;145;0;144;0
WireConnection;138;0;135;0
WireConnection;138;1;135;0
WireConnection;138;2;135;0
WireConnection;86;0;145;0
WireConnection;86;1;163;0
WireConnection;133;0;139;0
WireConnection;133;1;138;0
WireConnection;141;0;133;0
WireConnection;141;3;86;0
WireConnection;157;0;141;0
ASEEND*/
//CHKSM=2EE2B6D6DA14E6B4531539FFB4F65BA085EBE453