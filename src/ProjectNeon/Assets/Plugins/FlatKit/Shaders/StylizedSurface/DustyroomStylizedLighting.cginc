#ifndef DUSTYROOM_STYLIZED_LIGHTING_INCLUDED
#define DUSTYROOM_STYLIZED_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"

UNITY_INSTANCING_BUFFER_START(Props)

UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)

#ifdef _CELPRIMARYMODE_SINGLE
UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorDim)
#endif  // _CELPRIMARYMODE_SINGLE

#ifdef DR_SPECULAR_ON
UNITY_DEFINE_INSTANCED_PROP(half4, _FlatSpecularColor)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatSpecularSize)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatSpecularEdgeSmoothness)
#endif  // DR_SPECULAR_ON

#ifdef DR_RIM_ON
UNITY_DEFINE_INSTANCED_PROP(half4, _FlatRimColor)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimSize)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimEdgeSmoothness)
UNITY_DEFINE_INSTANCED_PROP(float, _FlatRimLightAlign)
#endif  // DR_RIM_ON

UNITY_INSTANCING_BUFFER_END(Props)

half _SelfShadingSize;
half _ShadowEdgeSize;
half _LightContribution;
half _Flatness;

half _UnityShadowPower;
half _UnityShadowSharpness;
half3 _UnityShadowColor;

#ifdef _CELPRIMARYMODE_STEPS
fixed4 _ColorDimSteps;
sampler2D _CelStepTexture;
#endif  // _CELPRIMARYMODE_STEPS

#ifdef _CELPRIMARYMODE_CURVE
fixed4 _ColorDimCurve;
sampler2D _CelCurveTexture;
#endif  // _CELPRIMARYMODE_CURVE

#ifdef DR_CEL_EXTRA_ON
fixed4 _ColorDimExtra;
half _SelfShadingSizeExtra;
half _ShadowEdgeSizeExtra;
half _FlatnessExtra;
#endif  // DR_CEL_EXTRA_ON

#ifdef DR_GRADIENT_ON
half4 _ColorGradient;
half _GradientCenterX;
half _GradientCenterY;
half _GradientSize;
half _GradientAngle;
#endif  // DR_GRADIENT_ON

half _TextureImpact;

sampler2D _MainTex;
sampler2D _BumpMap;

float3 _LightmapDirection = float3(1, 0, 0);
float _OverrideLightmapDir = 0;

struct InputObject
{
    float2 uv_MainTex;
    float2 uv_BumpMap;

    float3 viewDir;
    float3 lightDir;
    float3 worldPos;
    float3 worldNormalCustom;  // "worldNormal" is a reserved Unity variable.

    #ifdef DR_VERTEX_COLORS_ON
        float4 color: Color;  // Vertex color
    #endif  // DR_VERTEX_COLORS_ON
};

struct SurfaceOutputDustyroom
{
    fixed3 Albedo;
    fixed3 Normal;
    half3 Emission;
    float Smoothness;
    float Metallic;
    half Occlusion;
    fixed Alpha;
    float Attenuation;
    float SpecularLightmapOcclusion;
};

inline half NdotLTransition(half3 normal, half3 lightDir, half selfShadingSize, half shadowEdgeSize, half flatness) {
    // normal and lightDir are assumed to be normalized.
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - selfShadingSize);
    half angleDiffTransition = smoothstep(0, shadowEdgeSize, angleDiff); 
    return lerp(angleDiff, angleDiffTransition, flatness);
}

inline half NdotLTransitionPrimary(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSize, _ShadowEdgeSize, _Flatness);
}

#if defined(DR_CEL_EXTRA_ON)
inline half NdotLTransitionExtra(half3 normal, half3 lightDir) { 
    return NdotLTransition(normal, lightDir, _SelfShadingSizeExtra, _ShadowEdgeSizeExtra, _FlatnessExtra);
}
#endif

inline half NdotLTransitionTexture(half3 normal, half3 lightDir, sampler2D stepTex) {
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - _SelfShadingSize);
    half4 rampColor = tex2D(stepTex, half2(angleDiff, 0.5));
    // NOTE: The color channel here corresponds to the texture format in the shader editor script.
    half angleDiffTransition = rampColor.r;
    return angleDiffTransition;
}

inline half4 LightingDustyroomStylized(inout SurfaceOutputDustyroom s, half3 viewDir, UnityGI gi) {
    half attenSharpened = saturate(s.Attenuation * _UnityShadowSharpness);

#if defined(_UNITYSHADOWMODE_COLOR) && defined(USING_DIRECTIONAL_LIGHT)
    s.Albedo = lerp(_UnityShadowColor, s.Albedo, attenSharpened);
#endif

#if defined(USING_DIRECTIONAL_LIGHT)
    half3 light = lerp(half3(1, 1, 1), _LightColor0.rgb, _LightContribution);
#else
    half3 light = gi.light.color.rgb;
#endif

    half unityShadowPower = 1;  // Corresponds to no shadow mode keyword.
#if defined(_UNITYSHADOWMODE_MULTIPLY)
    unityShadowPower = lerp(1, attenSharpened, _UnityShadowPower);
#endif

    half4 c;
    c.rgb = s.Albedo * light * unityShadowPower;

#if defined(UNITY_LIGHT_FUNCTION_APPLY_INDIRECT)
    c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

    c.a = s.Alpha;
    return c;
}

inline void LightingDustyroomStylized_GI(inout SurfaceOutputDustyroom s, UnityGIInput data, inout UnityGI gi)
{
    gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
    s.Attenuation = data.atten;
    // TODO: Customize lightmap handling. See https://docs.unity3d.com/Manual/SL-SurfaceShaderLighting.html.
}

inline half4 LightingDustyroomStylized_Deferred(
    SurfaceOutputDustyroom s, float3 viewDir, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
{
    half oneMinusReflectivity;
    half3 specColor;
    s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

    half4 c = UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);

    UnityStandardData data;
    data.diffuseColor   = s.Albedo;
    data.occlusion      = s.Occlusion;
    data.specularColor  = specColor;
    data.smoothness     = s.Smoothness;
    data.normalWorld    = s.Normal;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    half4 emission = half4(s.Emission + c.rgb, 1);
    return emission;
}

inline half4 LightingDustyroomStylizedSpecular_Deferred(
    SurfaceOutputDustyroom s, float3 viewDir, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
{
    // energy conservation
    half oneMinusReflectivity;
    s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, /*s.Specular*/s.Albedo, /*out*/ oneMinusReflectivity);
    
    half4 c = UNITY_BRDF_PBS (s.Albedo, /*s.Specular*/s.Albedo, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);

    UnityStandardData data;
    data.diffuseColor   = s.Albedo;
    data.occlusion      = s.Occlusion;
    data.specularColor  = /*s.Specular*/c;
    data.smoothness     = s.Smoothness;
    data.normalWorld    = s.Normal;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    half4 emission = half4(s.Emission + c.rgb, 1);
    return emission;
}

inline void LightingDustyroomStylizedSpecular_GI (
    SurfaceOutputDustyroom s,
    UnityGIInput data,
    inout UnityGI gi)
{
#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
    gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
#else
    Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, /*s.Specular*/s.Albedo);
    gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal, g);
#endif
}

void vertObject(inout appdata_full v, out InputObject o) {
    UNITY_INITIALIZE_OUTPUT(InputObject, o);
    o.lightDir = WorldSpaceLightDir(v.vertex);
    o.worldNormalCustom = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
}

inline half4 SurfaceCore(half3 worldNormal, half3 worldPos, half3 lightDir, half3 viewDir) {
    fixed4 c = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
    
    #if defined(_CELPRIMARYMODE_SINGLE)
        half NdotLTPrimary = NdotLTransitionPrimary(worldNormal, lightDir);
        c = lerp(UNITY_ACCESS_INSTANCED_PROP(Props, _ColorDim), c, NdotLTPrimary);
    #endif  // _CELPRIMARYMODE_SINGLE
    
    #if defined(_CELPRIMARYMODE_STEPS)
        half NdotLTSteps = NdotLTransitionTexture(worldNormal, lightDir, _CelStepTexture);
        c = lerp(_ColorDimSteps, c, NdotLTSteps);
    #endif  // _CELPRIMARYMODE_STEPS
    
    #if defined(_CELPRIMARYMODE_CURVE)
        half NdotLTCurve = NdotLTransitionTexture(worldNormal, lightDir, _CelCurveTexture);
        c = lerp(_ColorDimCurve, c, NdotLTCurve);
    #endif  // _CELPRIMARYMODE_CURVE
    
    #if defined(DR_CEL_EXTRA_ON)
        half NdotLTExtra = NdotLTransitionExtra(worldNormal, lightDir);
        c = lerp(_ColorDimExtra, c, NdotLTExtra);
    #endif  // DR_CEL_EXTRA_ON
    
    #if defined(DR_GRADIENT_ON)
        float angleRadians = _GradientAngle / 180.0 * 3.14159265359;
        float posGradRotated = (worldPos.x - _GradientCenterX) * sin(angleRadians) + 
                               (worldPos.y - _GradientCenterY) * cos(angleRadians);
        float gradientTop = _GradientCenterY + _GradientSize * 0.5;
        half gradientFactor = saturate((gradientTop - posGradRotated) / _GradientSize);
        c = lerp(c, _ColorGradient, gradientFactor);
    #endif  // DR_GRADIENT_ON

    #if defined(DR_RIM_ON)
        float rim = 1.0 - dot(viewDir, worldNormal);
        half NdotL = dot(worldNormal, lightDir);
        float rimLightAlign = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimLightAlign);
        float rimSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimSize);
        float rimSpread = 1.0 - rimSize - NdotL * rimLightAlign;
        float rimEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimEdgeSmoothness);
        float rimTransition = smoothstep(rimSpread - rimEdgeSmooth * 0.5, rimSpread + rimEdgeSmooth * 0.5, rim);
        c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatRimColor), rimTransition);
    #endif  // DR_RIM_ON

    #if defined(DR_SPECULAR_ON)
        float3 halfVector = normalize(lightDir + viewDir);
        float NdotH = dot(worldNormal, halfVector) * 0.5 + 0.5;
        float specularSize = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularSize);
        float specEdgeSmooth = UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularEdgeSmoothness);
        float specular = saturate(pow(NdotH, 100.0 * (1.0 - specularSize) * (1.0 - specularSize)));
        float specularTransition = smoothstep(0.5 - specEdgeSmooth * 0.1, 0.5 + specEdgeSmooth * 0.1, specular);
        c = lerp(c, UNITY_ACCESS_INSTANCED_PROP(Props, _FlatSpecularColor), specularTransition);
    #endif  // DR_SPECULAR_ON
    
    return c;
}

// https://www.gamedev.net/forums/topic/678043-how-to-blend-world-space-normals/5287707/?view=findpost&p=5287707
float3 ReorientNormal(in float3 u, in float3 t, in float3 s)
{
    // Build the shortest-arc quaternion
    float4 q = float4(cross(s, t), dot(s, t) + 1) / sqrt(2 * (dot(s, t) + 1));
 
    // Rotate the normal
    return u * (q.w * q.w - dot(q.xyz, q.xyz)) + 2 * q.xyz * dot(q.xyz, u) + 2 * q.w * cross(q.xyz, u);
}

void surfObject(InputObject IN, inout SurfaceOutputDustyroom o) {
    #if !defined(TERRAIN_STANDARD_SHADER)
        half3 mapNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        mapNormal = UnityObjectToWorldNormal(mapNormal);
        half3 zNormal = UnityObjectToWorldNormal(half3(0, 0, 1));
        half3 blendedNormal = normalize(ReorientNormal(IN.worldNormalCustom, mapNormal, zNormal));
    #else
        half3 blendedNormal = IN.worldNormalCustom;
    #endif

    // If all light in the scene is baked, we use custom light direction for the cel shading.
    // IN.lightDir = lerp(IN.lightDir, _LightmapDirection, _EnableLightmapDir);

    half4 c = SurfaceCore(blendedNormal, IN.worldPos, IN.lightDir, IN.viewDir);

    {
        #if defined(_TEXTUREBLENDINGMODE_ADD)
            c += lerp(half4(0.0, 0.0, 0.0, 0.0), tex2D(_MainTex, IN.uv_MainTex), _TextureImpact);
        #else  // _TEXTUREBLENDINGMODE_MULTIPLY
            // This is the default blending mode for compatibility with the v.1 of the asset.
            c *= lerp(half4(1.0, 1.0, 1.0, 1.0), tex2D(_MainTex, IN.uv_MainTex), _TextureImpact);
        #endif
    }

    {
        #ifdef DR_VERTEX_COLORS_ON
            c *= IN.color;
        #endif  // DR_VERTEX_COLORS_ON
    }

    o.Albedo = c.rgb;
    o.Alpha = c.a;
    // o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}

#endif // DUSTYROOM_STYLIZED_LIGHTING_INCLUDED
