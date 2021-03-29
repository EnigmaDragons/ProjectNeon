// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hovl/WaterOrb"
{
	Properties
	{
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 0.5
		_Color("Color", Color) = (0.07450981,0.09019608,0.1019608,1)
		_Metallic("Metallic", Range( 0 , 1)) = 1
		_Gloss("Gloss", Range( 0 , 1)) = 0.8
		_Opacity("Opacity", Range( 0 , 1)) = 0.3
		_Distortionpower("Distortion power", Float) = 0
		_Numberofwaves("Number of waves", Float) = 1
		_WavesspeedsizeXYTwistspeedsizeZW("Waves speed-size XY Twist speed-size ZW", Vector) = (-1,0.2,4,0.6)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.5
		#pragma surface surf Standard alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float4 vertexColor : COLOR;
		};

		uniform float _Numberofwaves;
		uniform float4 _WavesspeedsizeXYTwistspeedsizeZW;
		uniform sampler2D _NormalMap;
		uniform float _NormalScale;
		uniform float4 _NormalMap_ST;
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _Distortionpower;
		uniform float4 _Color;
		uniform float _Metallic;
		uniform float _Gloss;
		uniform float _Opacity;


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
			float3 ase_vertexNormal = v.normal.xyz;
			float V166 = v.texcoord.xyz.y;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime187 = _Time.y * _WavesspeedsizeXYTwistspeedsizeZW.x;
			float mulTime160 = _Time.y * _WavesspeedsizeXYTwistspeedsizeZW.z;
			float3 appendResult170 = (float3(( sin( ( 3.0 * ( ase_vertex3Pos.y + mulTime160 ) * UNITY_PI ) ) * V166 ) , 0.0 , ( V166 * sin( ( 3.0 * ( mulTime160 + ase_vertex3Pos.y + ( UNITY_PI / 2.0 ) ) * UNITY_PI ) ) )));
			v.vertex.xyz += ( ( ase_vertexNormal * ( V166 * sin( ( _Numberofwaves * ( ase_vertex3Pos.y + mulTime187 ) * UNITY_PI ) ) ) * _WavesspeedsizeXYTwistspeedsizeZW.y ) + ( _WavesspeedsizeXYTwistspeedsizeZW.w * appendResult170 ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float2 panner146 = ( 1.0 * _Time.y * float2( 0.3,0.1 ) + uv0_NormalMap);
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, panner146 ), _NormalScale );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor132 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float4( UnpackScaleNormal( tex2D( _NormalMap, panner146 ), _Distortionpower ) , 0.0 ) + ase_grabScreenPosNorm ).xy);
			o.Emission = ( float4( (screenColor132).rgb , 0.0 ) + _Color ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
			o.Alpha = ( i.vertexColor.a * _Opacity );
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=16800
409;325;1307;708;2008.196;768.1147;2.350868;True;False
Node;AmplifyShaderEditor.Vector4Node;176;-1598.419,1170.479;Float;False;Property;_WavesspeedsizeXYTwistspeedsizeZW;Waves speed-size XY Twist speed-size ZW;8;0;Create;True;0;0;False;0;-1,0.2,4,0.6;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PiNode;175;-1379.766,1762.32;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;160;-1241.744,1514.678;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;180;-1550.384,1454.214;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;164;-1188.973,1756.771;Float;False;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;-1027.669,1370.811;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;190;-1314.675,-228.6198;Float;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;147;-1336.874,-435.3392;Float;False;0;125;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PiNode;183;-1088.012,1184.474;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-1020.223,1694.706;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;187;-1220.565,1050.521;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-878.0507,1670.505;Float;False;3;3;0;FLOAT;3;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-1060.476,-272.4925;Float;False;Property;_Distortionpower;Distortion power;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;146;-1041.952,-401.1633;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.3,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;166;-1042.509,-175.8469;Float;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-1022.681,933.2717;Float;False;Property;_Numberofwaves;Number of waves;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-856.0767,1348.219;Float;False;3;3;0;FLOAT;3;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;167;-997.3698,1011.817;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;131;-740.1512,-179.8456;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;125;-804.1512,-387.8453;Float;True;Property;_NormalMap;NormalMap;0;0;Create;True;0;0;False;0;None;302951faffe230848aa0d3df7bb70faa;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-797.7867,988.2127;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;172;-727.0636,1350.635;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;168;-851.5457,1499.767;Float;False;166;V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;186;-730.2497,1679.409;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;188;-645.1116,987.4138;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-484.1472,-291.8456;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-592.0428,1621.461;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-591.5659,1352.25;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-658.8527,875.3458;Float;False;166;V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-406.7289,864.4869;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;132;-338.2314,-366.4494;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;181;-492.6368,1191.583;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;170;-418.8429,1438.192;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;162;-417.4168,715.5627;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;128;-152.7932,-356.0084;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;197;-970.5201,25.54613;Float;False;Property;_NormalScale;NormalScale;1;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-196.336,1274.38;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-136.7848,858.9548;Float;False;3;3;0;FLOAT3;1,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;31;-91.56123,-83.21339;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;0.07450981,0.09019608,0.1019608,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;62;-52.12607,461.0854;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;0.3;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;32;-9.162564,281.3069;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;198;193.5801,-103.7432;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;196;-754.3093,12.09158;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;302951faffe230848aa0d3df7bb70faa;True;0;True;bump;Auto;True;Instance;125;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;161;93.58709,971.8987;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;193;255.0455,152.8629;Float;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;306.3595,355.6106;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;231.0035,246.1541;Float;False;Property;_Gloss;Gloss;4;0;Create;True;0;0;False;0;0.8;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;145;698.8021,52.80696;Float;False;True;3;Float;;0;0;Standard;Hovl/WaterOrb;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;160;0;176;3
WireConnection;164;0;175;0
WireConnection;179;0;180;2
WireConnection;179;1;160;0
WireConnection;169;0;160;0
WireConnection;169;1;180;2
WireConnection;169;2;164;0
WireConnection;187;0;176;1
WireConnection;171;1;169;0
WireConnection;171;2;183;0
WireConnection;146;0;147;0
WireConnection;166;0;190;2
WireConnection;165;1;179;0
WireConnection;165;2;183;0
WireConnection;167;0;180;2
WireConnection;167;1;187;0
WireConnection;125;1;146;0
WireConnection;125;5;124;0
WireConnection;163;0;184;0
WireConnection;163;1;167;0
WireConnection;163;2;183;0
WireConnection;172;0;165;0
WireConnection;186;0;171;0
WireConnection;188;0;163;0
WireConnection;126;0;125;0
WireConnection;126;1;131;0
WireConnection;174;0;168;0
WireConnection;174;1;186;0
WireConnection;185;0;172;0
WireConnection;185;1;168;0
WireConnection;178;0;177;0
WireConnection;178;1;188;0
WireConnection;132;0;126;0
WireConnection;181;0;176;2
WireConnection;170;0;185;0
WireConnection;170;2;174;0
WireConnection;128;0;132;0
WireConnection;182;0;176;4
WireConnection;182;1;170;0
WireConnection;189;0;162;0
WireConnection;189;1;178;0
WireConnection;189;2;181;0
WireConnection;198;0;128;0
WireConnection;198;1;31;0
WireConnection;196;1;146;0
WireConnection;196;5;197;0
WireConnection;161;0;189;0
WireConnection;161;1;182;0
WireConnection;88;0;32;4
WireConnection;88;1;62;0
WireConnection;145;1;196;0
WireConnection;145;2;198;0
WireConnection;145;3;193;0
WireConnection;145;4;194;0
WireConnection;145;9;88;0
WireConnection;145;11;161;0
ASEEND*/
//CHKSM=BFE9EC11D3AA4CB37410F126ED19ED91C19FE38E