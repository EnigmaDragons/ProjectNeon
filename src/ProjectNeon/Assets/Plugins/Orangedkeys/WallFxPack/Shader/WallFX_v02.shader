// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WallFX_v02"
{
	Properties
	{
		_DistortionMask("DistortionMask", 2D) = "white" {}
		_MaskDistortion("MaskDistortion", Float) = 0.25
		_MainColor("MainColor", Color) = (0.9323065,0.3368019,0.131792,0)
		_EmissiveMult("EmissiveMult", Range( 0 , 10)) = 4
		_WallColorMult("WallColorMult", Range( 0 , 1)) = 0.2
		_WallOutlineColorMult("WallOutlineColorMult", Range( 0 , 5)) = 0.02
		_OutlinesMult("OutlinesMult", Range( 0 , 4)) = 1
		_SpeedAnimatedMode("Speed (AnimatedMode)", Float) = 1
		_StretchingAnime("StretchingAnime", Range( -10 , 10)) = 1
		[Toggle]_DisplayFrame("Display Frame", Float) = 1
		_FrameEmissive("FrameEmissive", Range( 0 , 1)) = 0.15
		_WallOpening("WallOpening", Range( 0 , 1)) = 0
		_AlphaFactor("AlphaFactor", Range( 0 , 1)) = 0.1
		[Toggle]_AnimatedOpening("AnimatedOpening", Float) = 1
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 screenPos;
			float2 uv2_texcoord2;
		};

		uniform float _AnimatedOpening;
		uniform float _WallOpening;
		uniform float _SpeedAnimatedMode;
		uniform sampler2D _DistortionMask;
		uniform float4 _DistortionMask_ST;
		uniform float _MaskDistortion;
		uniform float _StretchingAnime;
		uniform float _WallOutlineColorMult;
		uniform float4 _MainColor;
		uniform float _OutlinesMult;
		uniform float _EmissiveMult;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _WallColorMult;
		uniform float _DisplayFrame;
		uniform float _FrameEmissive;
		uniform float _AlphaFactor;


		float3 MyCustomExpression80( float4 _HueShift , float3 c , float aHue )
		{
			/*c.rgb = lerp(c.rgb, c.gbr, _HueShift.r); 
			c.rgb = lerp(c.rgb, c.gbr, _HueShift.g); 
			c.rgb = lerp(c.rgb, c.bgr, _HueShift.b); 
			*/
			float angle = radians(aHue);
			float3 k = float3(0.57735, 0.57735, 0.57735);
			float cosAngle = cos(angle);
			 //Rodrigues' rotation formula
			return c.rgb;// * cosAngle + cross(k, c.rgb) * sin(angle) + k * dot(k, c.rgb) * (1 - cosAngle);
			return c;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (( ( _WallOpening - 0.5 ) * 2.0 )).xx;
			float mulTime69 = _Time.y * _SpeedAnimatedMode;
			float2 uv2_DistortionMask = v.texcoord1 * _DistortionMask_ST.xy + _DistortionMask_ST.zw;
			float2 uv2_TexCoord1 = v.texcoord1.xy + ( (( _AnimatedOpening )?( ( float2( 1,1 ) * sin( mulTime69 ) ) ):( temp_cast_0 )) + ( tex2Dlod( _DistortionMask, float4( uv2_DistortionMask, 0, 0.0) ).r * _MaskDistortion ) );
			float saferPower27 = max( uv2_TexCoord1.y , 0.0001 );
			float clampResult24 = clamp( pow( saferPower27 , 5.0 ) , 0.0 , 1.0 );
			float lerpResult19 = lerp( 0.0 , v.color.g , ( clampResult24 * ( -2.0 * _StretchingAnime ) ));
			float4 appendResult21 = (float4(0.0 , lerpResult19 , 0.0 , 0.0));
			v.vertex.xyz += ( appendResult21 * v.color.b ).xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _HueShift80 = float4( 0,0,0,0 );
			float3 c80 = _MainColor.rgb;
			float aHue80 = 0.0;
			float3 localMyCustomExpression80 = MyCustomExpression80( _HueShift80 , c80 , aHue80 );
			float temp_output_57_0 = ( i.vertexColor.r * _OutlinesMult );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor43 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPosNorm.xy);
			float3 desaturateInitialColor54 = screenColor43.rgb;
			float desaturateDot54 = dot( desaturateInitialColor54, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar54 = lerp( desaturateInitialColor54, desaturateDot54.xxx, 0.75 );
			float2 temp_cast_3 = (( ( _WallOpening - 0.5 ) * 2.0 )).xx;
			float mulTime69 = _Time.y * _SpeedAnimatedMode;
			float2 uv2_DistortionMask = i.uv2_texcoord2 * _DistortionMask_ST.xy + _DistortionMask_ST.zw;
			float2 uv2_TexCoord1 = i.uv2_texcoord2 + ( (( _AnimatedOpening )?( ( float2( 1,1 ) * sin( mulTime69 ) ) ):( temp_cast_3 )) + ( tex2D( _DistortionMask, uv2_DistortionMask ).r * _MaskDistortion ) );
			float saferPower27 = max( uv2_TexCoord1.y , 0.0001 );
			float clampResult24 = clamp( pow( saferPower27 , 5.0 ) , 0.0 , 1.0 );
			float temp_output_36_0 = ( 1.0 - clampResult24 );
			float3 lerpResult40 = lerp( ( temp_output_57_0 + ( localMyCustomExpression80 * _EmissiveMult ) ) , ( pow( desaturateVar54 , 1.0 ) + ( localMyCustomExpression80 * _WallColorMult ) ) , pow( temp_output_36_0 , 10.0 ));
			float temp_output_62_0 = ( 1.0 - i.vertexColor.b );
			float3 lerpResult63 = lerp( ( ( i.vertexColor.r * _WallOutlineColorMult * localMyCustomExpression80 ) + lerpResult40 ) , localMyCustomExpression80 , temp_output_62_0);
			o.Emission = lerpResult63 * _AlphaFactor;
			o.Alpha = (( ( pow( temp_output_36_0 , 0.2 ) * i.vertexColor.b ) + ( temp_output_62_0 * (( _DisplayFrame )?( _FrameEmissive ):( 0.0 )) ) )) * _AlphaFactor;
		}

		ENDCG
	}
	CustomEditor "WallFX_GUI_v2"
}
/*ASEBEGIN
Version=18100
-1920;520;1403;465;1365.34;472.0668;2.138463;True;True
Node;AmplifyShaderEditor.RangedFloatNode;68;-3081.099,310.1051;Inherit;False;Property;_SpeedAnimatedMode;Speed (AnimatedMode);7;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;69;-2839.298,416.8052;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-3102.813,-162.2752;Inherit;False;Property;_WallOpening;WallOpening;11;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;71;-2661.298,415.8052;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-2760.009,182.1396;Inherit;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;90;-2814.813,-210.2752;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2754.51,782.6395;Inherit;False;Property;_MaskDistortion;MaskDistortion;1;0;Create;True;0;0;False;0;False;0.25;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-2671.813,-147.2752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-2868.31,545.7397;Inherit;True;Property;_DistortionMask;DistortionMask;0;0;Create;True;0;0;False;0;False;-1;None;61c0b9c0523734e0e91bc6043c72a490;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2582.21,303.1396;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;93;-2468.813,-153.2752;Inherit;False;Property;_AnimatedOpening;AnimatedOpening;12;0;Create;True;0;0;False;0;False;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2545.21,675.1398;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-2405.21,394.1396;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2224.35,260.1301;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;42;-1119.238,-888.9603;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;39;-1208.775,-443.9203;Inherit;False;Property;_MainColor;MainColor;2;0;Create;True;0;0;False;0;False;0.9323065,0.3368019,0.131792,0;1,0.4002926,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;43;-815.0121,-889.1075;Inherit;False;Global;_GrabScreen2;Grab Screen 2;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;27;-1916.995,393.1272;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-1228.152,-129.9839;Inherit;False;Constant;_Hue;Hue;6;0;Create;True;0;0;False;0;False;0;180;-180;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-988.2797,20.93154;Inherit;False;Property;_EmissiveMult;EmissiveMult;3;0;Create;True;0;0;False;0;False;4;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;54;-596.8239,-1055.084;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.75;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-221.7047,-839.1017;Inherit;False;Property;_OutlinesMult;OutlinesMult;6;0;Create;True;0;0;False;0;False;1;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;56;-119.3864,-1012.811;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;-728.1649,-723.1846;Inherit;False;Property;_WallColorMult;WallColorMult;4;0;Create;True;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1788.258,951.9791;Inherit;False;Property;_StretchingAnime;StretchingAnime;8;0;Create;True;0;0;False;0;False;1;-0.5;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;80;-921.9905,-266.0584;Inherit;False;/*c.rgb = lerp(c.rgb, c.gbr, _HueShift.r)@ $c.rgb = lerp(c.rgb, c.gbr, _HueShift.g)@ $c.rgb = lerp(c.rgb, c.bgr, _HueShift.b)@ $*/$float angle = radians(aHue)@$float3 k = float3(0.57735, 0.57735, 0.57735)@$float cosAngle = cos(angle)@$ //Rodrigues' rotation formula$return c.rgb@// * cosAngle + cross(k, c.rgb) * sin(angle) + k * dot(k, c.rgb) * (1 - cosAngle)@$$return c@;3;False;3;True;_HueShift;FLOAT4;0,0,0,0;In;;Inherit;False;False;c;FLOAT3;0,0,0;In;;Inherit;False;True;aHue;FLOAT;0;In;;Float;False;My Custom Expression;True;False;0;3;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;24;-1595.509,392.7232;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1658.359,698.3321;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;False;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1475.963,814.1307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;36;-1338.502,310.4594;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;51;-369.966,-911.3829;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-681.4799,-183.168;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-519.9061,-655.6128;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.1886792;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;68.45299,-908.9583;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1334.148,694.1437;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;164.8669,-1108.425;Inherit;False;Property;_WallOutlineColorMult;WallOutlineColorMult;5;0;Create;True;0;0;False;0;False;0.02;0.1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-181.3349,-676.0471;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;52;-399.7527,-165.9834;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;8;-636.877,485.9745;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;55;129.741,-478.7867;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-374.877,83.99596;Inherit;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-612.8772,24.24809;Inherit;False;Property;_FrameEmissive;FrameEmissive;10;0;Create;True;0;0;False;0;False;0.15;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;111.1056,-205.7827;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;62;-468.7952,225.4126;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;75;-156.8824,-1.50209;Inherit;False;Property;_DisplayFrame;Display Frame;9;0;Create;True;0;0;False;0;False;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-368.6283,642.6696;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;6.2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;447.3334,-988.3124;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.05;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;37;-57.13348,314.3411;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;406.186,-231.1149;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;45.11291,621.7906;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;323.3081,310.438;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;89.54932,225.5738;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-470.3553,820.7819;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;63;680.076,-325.8001;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;241.5201,-701.8679;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;675.8625,206.4137;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;6;-3008.71,-62.56038;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;661.7065,543.111;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;870.1445,13.72642;Float;False;True;-1;2;WallFX_GUI_v2;0;0;Unlit;WallFX_v02;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;69;0;68;0
WireConnection;71;0;69;0
WireConnection;90;0;89;0
WireConnection;91;0;90;0
WireConnection;5;0;4;0
WireConnection;5;1;71;0
WireConnection;93;0;91;0
WireConnection;93;1;5;0
WireConnection;30;0;29;1
WireConnection;30;1;31;0
WireConnection;28;0;93;0
WireConnection;28;1;30;0
WireConnection;1;1;28;0
WireConnection;43;0;42;0
WireConnection;27;0;1;2
WireConnection;54;0;43;0
WireConnection;80;1;39;0
WireConnection;80;2;78;0
WireConnection;24;0;27;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;36;0;24;0
WireConnection;51;0;54;0
WireConnection;83;0;80;0
WireConnection;83;1;84;0
WireConnection;49;0;80;0
WireConnection;49;1;50;0
WireConnection;57;0;56;1
WireConnection;57;1;67;0
WireConnection;26;0;24;0
WireConnection;26;1;74;0
WireConnection;48;0;51;0
WireConnection;48;1;49;0
WireConnection;52;0;36;0
WireConnection;55;0;57;0
WireConnection;55;1;83;0
WireConnection;40;0;55;0
WireConnection;40;1;48;0
WireConnection;40;2;52;0
WireConnection;62;0;8;3
WireConnection;75;0;77;0
WireConnection;75;1;76;0
WireConnection;19;1;8;2
WireConnection;19;2;26;0
WireConnection;59;0;56;1
WireConnection;59;1;66;0
WireConnection;59;2;80;0
WireConnection;37;0;36;0
WireConnection;58;0;59;0
WireConnection;58;1;40;0
WireConnection;21;1;19;0
WireConnection;64;0;37;0
WireConnection;64;1;8;3
WireConnection;65;0;62;0
WireConnection;65;1;75;0
WireConnection;94;0;8;2
WireConnection;63;0;58;0
WireConnection;63;1;80;0
WireConnection;63;2;62;0
WireConnection;82;0;57;0
WireConnection;82;1;80;0
WireConnection;61;0;64;0
WireConnection;61;1;65;0
WireConnection;60;0;21;0
WireConnection;60;1;8;3
WireConnection;0;2;63;0
WireConnection;0;9;61;0
WireConnection;0;11;60;0
ASEEND*/
//CHKSM=09E2736CA53AAF41F29E111E87AB6F35C7AFB650