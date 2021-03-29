Shader "FlatKit/LightPlane" {
Properties
{
    [Header(Color alpha controls opacity)]
    [HDR]_Color ("Color", Color) = (0.5, 0.5, 0.5, 1.0)
    
    [Space(15)]
    _Depth ("Depth Fade Distance", Range(1.0, 500.0)) = 100.0
    
    [Space]
    _CameraDistanceFadeFar("Camera Distance Fade Far", Float) = 10.0
    _CameraDistanceFadeClose("Camera Distance Fade Close", Float) = 0.0
    
    [Space]
    _UvFadeX("UV Fade X", Range(0, 10)) = 0.1
    _UvFadeY("UV Fade Y", Range(0, 10)) = 0.1
    [ToggleOff]_AllowAlphaOverflow("Allow Alpha Overflow", Float) = 0.0
}
 
SubShader
{
    Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
    LOD 200
    Blend SrcAlpha OneMinusSrcAlpha
    Lighting Off ZWrite Off
     
    Pass
    {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma shader_feature _ _ALLOWALPHAOVERFLOW_OFF
        #pragma multi_compile_fog;
        #pragma target 3.0
         
        #include "UnityCG.cginc"
         
        fixed4 _Color;
        float _Depth;
        float _CameraDistanceFadeFar, _CameraDistanceFadeClose;
        float _UvFadeX, _UvFadeY;
        
        sampler2D _CameraDepthTexture;
        float4 _CameraDepthTexture_ST;
         
        struct appdata{
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };
         
        struct v2f{
            float2 uv : TEXCOORD0;
            float3 dist :TEXCOORD3;
            float4 projPos : TEXCOORD2;
            UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
        };
         
        v2f vert (appdata i){
            v2f o;
            o.vertex = UnityObjectToClipPos(i.vertex);
            o.uv = i.texcoord;
            UNITY_TRANSFER_FOG(o,o.vertex);
            o.dist = UnityObjectToViewPos(i.vertex);
             
            o.projPos = ComputeScreenPos(o.vertex);
            UNITY_TRANSFER_DEPTH(o.projPos);
             
            return o;
        }
         
        fixed4 frag(v2f i) : SV_TARGET
        {
            fixed4 c = _Color;
            float scene_depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
            float object_depth = i.projPos.z + length(i.dist);
            float depth_fade = saturate((scene_depth - object_depth) / _Depth);
            c.a *= saturate((depth_fade * length(i.dist) - _CameraDistanceFadeClose) / (_CameraDistanceFadeFar - _CameraDistanceFadeClose));
            
            float fade_uv_x = pow(smoothstep(1, 0, abs(i.uv.x * 2 - 1)), _UvFadeX);
            float fade_uv_y = pow(smoothstep(1, 0, abs(i.uv.y * 2 - 1)), _UvFadeY);
            c.a *= fade_uv_x * fade_uv_y;
            
#ifdef _ALLOWALPHAOVERFLOW_OFF
            c.a = saturate(c.a);
#endif  
            UNITY_APPLY_FOG(i.fogCoord, c);
            return c;
        }
     
    ENDCG
}
}
FallBack "Diffuse"
}