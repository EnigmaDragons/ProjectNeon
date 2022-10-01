// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WallFX_Shader"
{
	Properties
	{
		_MASK_("Mask Map", 2D) = "white" {}
		_Ring("Ring Map", 2D) = "white" {}
		_Outline("Outline Map", 2D) = "white" {}
		_SurfaceOpacity("SurfaceOpacity", Range( 0 , 2)) = 0.5
		_ColorsHue("Colors Hue", Range( -100 , 100)) = 0
		_OutlineMult("OutlineMult", Range( 0 , 10)) = 5
		_TransitionOutlineMult("TransitionOutlineMult", Range( 0 , 4)) = 0.6
		_ChromaticAberationMult("ChromaticAberationMult", Range( 0 , 25)) = 6
		_SpeedAnimatedMode("Speed (AnimatedMode)", Float) = 1
		_BlurMult("BlurMult", Range( 0 , 10)) = 10
		_PushMult("PushMult", Range( -160 , 160)) = 50
		[Toggle]_DebugColor("DebugColor", Float) = 0
		_WallOpening("WallOpening", Range( 0 , 1)) = 0
		[Toggle]_AnimatedOpening("AnimatedOpening", Float) = 1
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
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
			float4 screenPos;
			float2 uv2_texcoord2;
			float4 vertexColor : COLOR;
			float2 uv3_texcoord3;
		};

		uniform sampler2D _MASK_;
		uniform float _AnimatedOpening;
		uniform float _WallOpening;
		uniform float _SpeedAnimatedMode;
		uniform float _PushMult;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DebugColor;
		uniform sampler2D _Ring;
		uniform float _ChromaticAberationMult;
		uniform sampler2D _Outline;
		uniform float _OutlineMult;
		uniform float _TransitionOutlineMult;
		uniform float _BlurMult;
		uniform float _SurfaceOpacity;
		uniform float _ColorsHue;


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


		float3 MyCustomExpression165( float4 _HueShift , float3 c , float aHue )
		{
			/*c.rgb = lerp(c.rgb, c.gbr, _HueShift.r); 
			c.rgb = lerp(c.rgb, c.gbr, _HueShift.g); 
			c.rgb = lerp(c.rgb, c.bgr, _HueShift.b); 
			*/
			float angle = radians(aHue);
			float3 k = float3(0.57735, 0.57735, 0.57735);
			float cosAngle = cos(angle);
			 //Rodrigues' rotation formula
			return c.rgb * cosAngle + cross(k, c.rgb) * sin(angle) + k * dot(k, c.rgb) * (1 - cosAngle);
			return c;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (0.5).xx;
			float2 temp_cast_1 = (_WallOpening).xx;
			float mulTime240 = _Time.y * _SpeedAnimatedMode;
			float2 temp_output_5_0 = ( ( ( v.texcoord1.xy - temp_cast_0 ) * ( 1.0 - pow( (( _AnimatedOpening )?( abs( ( float2( 1,1 ) * sin( mulTime240 ) ) ) ):( temp_cast_1 )) , 0.2 ) ) ) + 0.5 );
			float4 tex2DNode1 = tex2Dlod( _MASK_, float4( temp_output_5_0, 0, 0.0) );
			float Temp158 = tex2DNode1.r;
			float3 VerOffset156 = ( ( 1.0 - Temp158 ) * ( float3(1,0,0) / _PushMult ) );
			v.vertex.xyz += VerOffset156;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _HueShift165 = float4( 0,0,0,0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 temp_cast_0 = (0.5).xx;
			float2 temp_cast_1 = (_WallOpening).xx;
			float mulTime240 = _Time.y * _SpeedAnimatedMode;
			float2 temp_output_5_0 = ( ( ( i.uv2_texcoord2 - temp_cast_0 ) * ( 1.0 - pow( (( _AnimatedOpening )?( abs( ( float2( 1,1 ) * sin( mulTime240 ) ) ) ):( temp_cast_1 )) , 0.2 ) ) ) + 0.5 );
			float4 tex2DNode113 = tex2D( _Ring, temp_output_5_0 );
			float2 temp_cast_2 = (0.5).xx;
			float4 tex2DNode1 = tex2D( _MASK_, temp_output_5_0 );
			float clampResult181 = clamp( ( tex2DNode113.r + tex2DNode1.r ) , 0.0 , 1.0 );
			float temp_output_77_0 = pow( (( _DebugColor )?( 1.0 ):( ceil( clampResult181 ) )) , 1.0 );
			float temp_output_30_0 = ( temp_output_77_0 * ( ( _ChromaticAberationMult / 1000.0 ) + 0.0 ) );
			float4 screenColor12 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( temp_output_30_0 * 1.5 * i.vertexColor ) ).xy);
			float4 screenColor21 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( temp_output_30_0 * ( 1.5 / 2.0 ) * i.vertexColor ) ).xy);
			float4 screenColor22 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( temp_output_30_0 * ( 1.5 * -1.0 ) * i.vertexColor ) ).xy);
			float4 appendResult27 = (float4(screenColor12.r , screenColor21.g , screenColor22.b , 0.0));
			float4 color54 = IsGammaSpace() ? float4(0,1.498039,1.254902,0) : float4(0,2.433049,1.647941,0);
			float4 color32 = IsGammaSpace() ? float4(0.495283,1,0.9271631,0) : float4(0.2097011,1,0.842238,0);
			float4 temp_output_96_0 = ( float4(0.001,0,0,0) * temp_output_77_0 * _BlurMult );
			float4 screenColor75 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( temp_output_96_0 * float4( -1,-1,-1,-1 ) ) ).xy);
			float4 screenColor58 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPosNorm.xy);
			float4 screenColor59 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + temp_output_96_0 ).xy);
			float3 desaturateInitialColor217 = ( ( screenColor75 + screenColor58 + screenColor59 ) / 3.0 ).rgb;
			float desaturateDot217 = dot( desaturateInitialColor217, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar217 = lerp( desaturateInitialColor217, desaturateDot217.xxx, 0.0 );
			float4 temp_output_86_0 = ( i.vertexColor * 0.5 );
			float3 c165 = ( ( ( ( ( appendResult27 + ( ( ( tex2D( _Outline, i.uv3_texcoord3 ).r * color54 * ( i.vertexColor + 0.02 ) ) * _OutlineMult * temp_output_77_0 ) + ( tex2DNode113.r * _TransitionOutlineMult * color32 ) ) ) * 0.5 ) + ( float4( desaturateVar217 , 0.0 ) * _SurfaceOpacity * color32 ) ) / 1.0 ) * ( temp_output_86_0 + color32 ) ).xyz;
			float aHue165 = ( _ColorsHue * 10.0 );
			float3 localMyCustomExpression165 = MyCustomExpression165( _HueShift165 , c165 , aHue165 );
			float3 Emissive153 = localMyCustomExpression165;
			o.Emission = Emissive153;
			float Opacity154 = ( temp_output_77_0 * pow( (( _DebugColor )?( 1.0 ):( tex2D( _MASK_, ( ( ( i.uv2_texcoord2 + float2( -0.5,-0.5 ) ) * float2( 0.02,0.02 ) ) + float2( 0.5,0.5 ) ) ).r )) , 0.8 ) );
			o.Alpha = Opacity154;
		}

		ENDCG
	}
	CustomEditor "WallFX_GUI"
}
/*ASEBEGIN
Version=18100
201.3333;340.6667;1920;1018;3286.176;612.3408;1.306899;True;False
Node;AmplifyShaderEditor.RangedFloatNode;243;-2802.803,-70.1515;Inherit;False;Property;_SpeedAnimatedMode;Speed (AnimatedMode);8;0;Create;False;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;240;-2561.002,34.94171;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;244;-2481.713,-199.7239;Inherit;False;Constant;_Vector2;Vector 0;1;0;Create;True;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;241;-2383.002,33.94171;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-2271.914,-82.72391;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;42;-2273.792,-283.7728;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;236;-2422.009,-640.5556;Inherit;False;Property;_WallOpening;WallOpening;12;0;Create;False;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;239;-2115.316,-517.4289;Inherit;False;Property;_AnimatedOpening;AnimatedOpening;13;0;Create;True;0;0;False;0;False;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1964.299,-248.5921;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;43;-1865.897,-384.146;Inherit;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1858.591,-86.33624;Inherit;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-1672.55,-387.4554;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;7;-1669.513,-249.3414;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1509.17,-250.6877;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-1330.591,-109.021;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;232;-1656.287,-2340.786;Inherit;True;Property;_MASK_;Mask Map;0;0;Create;False;0;0;False;0;False;None;8a351348f2b6d454f882192b4ef45e8e;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;113;-1123.514,-519.7527;Inherit;True;Property;_Ring;Ring Map;1;0;Create;False;0;0;False;0;False;-1;None;50d031ca38fd60e4194e7c8a6f833a40;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1136.707,-131.5408;Inherit;True;Property;_Mask;Mask;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;114;-662.3189,-500.9442;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;181;-574.4773,-368.6799;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;100;-428.6454,-369.5261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-567.114,-3.656311;Inherit;False;Constant;_Float12;Float 12;10;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-609.3093,187.0516;Inherit;False;Property;_ChromaticAberationMult;ChromaticAberationMult;7;0;Create;True;0;0;False;0;False;6;6;0;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;170;-436.5789,-126.2164;Inherit;False;Property;_DebugColor;DebugColor;11;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;77;-240.7481,-117.3352;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;80;-141.3793,2333.208;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;False;0;False;0.001,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;119;65.19965,2487.539;Inherit;False;Property;_BlurMult;BlurMult;9;0;Create;True;0;0;False;0;False;10;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-321.873,262.7781;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;111;-154.7311,262.8839;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-14.46423,647.6141;Inherit;False;Constant;_Float6;Float 6;2;0;Create;True;0;0;False;0;False;1.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;227.6558,2338.653;Inherit;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;143.5414,779.6186;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;14;-383.826,1385.559;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;102;209.6314,-1675.092;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;236.9518,-1484.203;Inherit;False;Constant;_Float7;Float 7;2;0;Create;True;0;0;False;0;False;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;11.42659,159.7703;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.015;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;378.8528,2125.569;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;-1,-1,-1,-1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;79;-77.45096,408.2877;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-540.8398,-1863.981;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;179.5426,657.6146;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;655.3943,2309.824;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.005,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;49;-132.3012,-1863.869;Inherit;True;Property;_Outline;Outline Map;2;0;Create;False;0;0;False;0;False;-1;99ab2eec64ba6c441a63e4602559da38;c6a12e9d2fdb5fa498ef82d89ae58a40;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;404.0601,358.1495;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;54;313.6752,-2105.014;Inherit;False;Constant;_Color1;Color 1;2;1;[HDR];Create;True;0;0;False;0;False;0,1.498039,1.254902,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;405.9258,553.8608;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;442.977,-1678.289;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;375.4142,157.7148;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;641.5082,2045.933;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;-0.005,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;59;873.6328,2398.041;Inherit;False;Global;_GrabScreen5;Grab Screen 5;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;58;866.7651,2214.23;Inherit;False;Global;_GrabScreen4;Grab Screen 4;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;812.7201,522.8344;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;808.7201,341.8344;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;32;2131.089,593.1696;Inherit;False;Constant;_MainColor;MainColor;4;1;[HDR];Create;True;0;0;False;0;False;0.495283,1,0.9271631,0;0.495283,1,0.9271631,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;117;822.0659,-396.3619;Inherit;False;Property;_TransitionOutlineMult;TransitionOutlineMult;6;0;Create;True;0;0;False;0;False;0.6;0.6;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;788.0323,-1757.208;Inherit;False;Property;_OutlineMult;OutlineMult;5;0;Create;True;0;0;False;0;False;5;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;794.4172,135.0986;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;574.4525,-1856.376;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;75;873.6708,2040.245;Inherit;False;Global;_GrabScreen3;Grab Screen 3;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;21;991.7202,333.8344;Inherit;False;Global;_GrabScreen1;Grab Screen 1;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;1202.012,-506.8446;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;2;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;137;1496.656,-2265.444;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;12;992.1158,132.7336;Inherit;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;22;999.7202,512.8344;Inherit;False;Global;_GrabScreen2;Grab Screen 2;1;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;1035.544,-1859.406;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;70;1362.147,2345.622;Inherit;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;1162.25,2197.583;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;69;1500.008,2208.081;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;1394.304,-917.9432;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;1205.72,361.8344;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;1798.281,-2264.341;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;-0.5,-0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;108;1586.349,93.09271;Inherit;False;Property;_SurfaceOpacity;SurfaceOpacity;3;0;Create;True;0;0;False;0;False;0.5;0.7;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;1529.071,-97.24976;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;1939.244,-2173.414;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.02,0.02;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;107;1695.472,-27.1148;Inherit;False;Constant;_Float8;Float 8;3;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;217;1740.557,2213.463;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;97;1650.162,411.1752;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;145;2117.361,-2320.465;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;1996.089,74.5323;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;87;1658.163,609.7624;Inherit;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;1855.171,-97.12787;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;136;2331.595,-2346.419;Inherit;True;Property;_Mask2;Mask2;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-737.1346,-114.1503;Inherit;False;Temp;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;2154.329,-97.4159;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;1864.321,409.6782;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;74;2204.412,-207.4869;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;171;2766.692,-2324.102;Inherit;False;Property;_DebugColor;DebugColor;10;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;2536.893,629.6191;Inherit;False;Property;_ColorsHue;Colors Hue;4;0;Create;False;0;0;False;0;False;0;-21.5;-100;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;150;3231.099,591.0596;Inherit;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;False;0;False;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;72;2385.844,-97.1984;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;3360.405,88.66576;Inherit;False;158;Temp;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;2366.249,411.8995;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;163;3245.018,805.85;Inherit;False;Property;_PushMult;PushMult;10;0;Create;True;0;0;False;0;False;50;-80;-160;160;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;161;3527.667,280.1683;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;2606.414,385.8162;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;164;3444.36,765.5337;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;2670.737,715.1479;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;147;2992.917,-2318.572;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;3159.249,-147.6546;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;3613.782,513.0109;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;165;2937.05,496.2241;Inherit;False;/*c.rgb = lerp(c.rgb, c.gbr, _HueShift.r)@ $c.rgb = lerp(c.rgb, c.gbr, _HueShift.g)@ $c.rgb = lerp(c.rgb, c.bgr, _HueShift.b)@ $*/$float angle = radians(aHue)@$float3 k = float3(0.57735, 0.57735, 0.57735)@$float cosAngle = cos(angle)@$ //Rodrigues' rotation formula$return c.rgb * cosAngle + cross(k, c.rgb) * sin(angle) + k * dot(k, c.rgb) * (1 - cosAngle)@$$return c@;3;False;3;True;_HueShift;FLOAT4;0,0,0,0;In;;Inherit;False;False;c;FLOAT3;0,0,0;In;;Inherit;False;True;aHue;FLOAT;0;In;;Float;False;My Custom Expression;True;False;0;3;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;4107.419,505.6277;Inherit;False;VerOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;3291.155,-345.0819;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;3259.727,490.8112;Inherit;False;Emissive;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;5230.986,160.652;Inherit;False;154;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;218;1347.203,423.2447;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;3022.802,60.33991;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;5256.251,12.06224;Inherit;False;153;Emissive;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;226;2144.1,408.27;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;220;2856.961,207.7536;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;222;379.3627,-1930.309;Inherit;False;Constant;_Float11;Float 11;11;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;225;1786.03,267.9241;Inherit;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;5351.876,387.8229;Inherit;False;156;VerOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;227;2015.099,502.27;Inherit;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;False;0.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;2166.268,269.5349;Inherit;False;Constant;_Float13;Float 13;12;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;219;3163.422,216.5501;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;174;2137.058,782.3173;Inherit;False;Constant;_Color5;Color 5;11;0;Create;True;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;41;-2514.792,-350.7728;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5552.686,-24.98839;Float;False;True;-1;2;WallFX_GUI;0;0;Unlit;WallFX_Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;240;0;243;0
WireConnection;241;0;240;0
WireConnection;242;0;244;0
WireConnection;242;1;241;0
WireConnection;42;0;242;0
WireConnection;239;0;236;0
WireConnection;239;1;42;0
WireConnection;43;0;239;0
WireConnection;19;0;43;0
WireConnection;7;0;2;0
WireConnection;7;1;6;0
WireConnection;8;0;7;0
WireConnection;8;1;19;0
WireConnection;5;0;8;0
WireConnection;5;1;6;0
WireConnection;113;1;5;0
WireConnection;1;0;232;0
WireConnection;1;1;5;0
WireConnection;114;0;113;1
WireConnection;114;1;1;1
WireConnection;181;0;114;0
WireConnection;100;0;181;0
WireConnection;170;0;100;0
WireConnection;170;1;168;0
WireConnection;77;0;170;0
WireConnection;112;0;231;0
WireConnection;111;0;112;0
WireConnection;96;0;80;0
WireConnection;96;1;77;0
WireConnection;96;2;119;0
WireConnection;93;0;92;0
WireConnection;30;0;77;0
WireConnection;30;1;111;0
WireConnection;81;0;96;0
WireConnection;94;0;92;0
WireConnection;61;0;14;0
WireConnection;61;1;96;0
WireConnection;49;1;48;0
WireConnection;39;0;30;0
WireConnection;39;1;94;0
WireConnection;39;2;79;0
WireConnection;40;0;30;0
WireConnection;40;1;93;0
WireConnection;40;2;79;0
WireConnection;101;0;102;0
WireConnection;101;1;103;0
WireConnection;38;0;30;0
WireConnection;38;1;92;0
WireConnection;38;2;79;0
WireConnection;60;0;14;0
WireConnection;60;1;81;0
WireConnection;59;0;61;0
WireConnection;58;0;14;0
WireConnection;24;0;14;0
WireConnection;24;1;40;0
WireConnection;23;0;14;0
WireConnection;23;1;39;0
WireConnection;17;0;14;0
WireConnection;17;1;38;0
WireConnection;53;0;49;1
WireConnection;53;1;54;0
WireConnection;53;2;101;0
WireConnection;75;0;60;0
WireConnection;21;0;23;0
WireConnection;116;0;113;1
WireConnection;116;1;117;0
WireConnection;116;2;32;0
WireConnection;12;0;17;0
WireConnection;22;0;24;0
WireConnection;55;0;53;0
WireConnection;55;1;56;0
WireConnection;55;2;77;0
WireConnection;65;0;75;0
WireConnection;65;1;58;0
WireConnection;65;2;59;0
WireConnection;69;0;65;0
WireConnection;69;1;70;0
WireConnection;115;0;55;0
WireConnection;115;1;116;0
WireConnection;27;0;12;1
WireConnection;27;1;21;2
WireConnection;27;2;22;3
WireConnection;144;0;137;0
WireConnection;50;0;27;0
WireConnection;50;1;115;0
WireConnection;143;0;144;0
WireConnection;217;0;69;0
WireConnection;145;0;143;0
WireConnection;106;0;217;0
WireConnection;106;1;108;0
WireConnection;106;2;32;0
WireConnection;105;0;50;0
WireConnection;105;1;107;0
WireConnection;136;0;232;0
WireConnection;136;1;145;0
WireConnection;158;0;1;1
WireConnection;71;0;105;0
WireConnection;71;1;106;0
WireConnection;86;0;97;0
WireConnection;86;1;87;0
WireConnection;171;0;136;1
WireConnection;72;0;71;0
WireConnection;72;1;74;0
WireConnection;85;0;86;0
WireConnection;85;1;32;0
WireConnection;161;0;159;0
WireConnection;31;0;72;0
WireConnection;31;1;85;0
WireConnection;164;0;150;0
WireConnection;164;1;163;0
WireConnection;177;0;167;0
WireConnection;147;0;171;0
WireConnection;120;0;77;0
WireConnection;120;1;147;0
WireConnection;151;0;161;0
WireConnection;151;1;164;0
WireConnection;165;1;31;0
WireConnection;165;2;177;0
WireConnection;156;0;151;0
WireConnection;154;0;120;0
WireConnection;153;0;165;0
WireConnection;218;0;27;0
WireConnection;221;0;220;1
WireConnection;226;0;86;0
WireConnection;226;1;227;0
WireConnection;225;0;97;0
WireConnection;219;0;221;0
WireConnection;219;1;165;0
WireConnection;0;2;152;0
WireConnection;0;9;155;0
WireConnection;0;11;157;0
ASEEND*/
//CHKSM=3394B74A181F3E65FC1DB290947A36ED66813F27