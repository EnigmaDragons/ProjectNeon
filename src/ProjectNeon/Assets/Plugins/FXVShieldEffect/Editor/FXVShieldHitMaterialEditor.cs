using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FXVShieldHitMaterialEditor : ShaderGUI 
{
    public enum ACTIVATION_TYPE_OPTIONS
    {
        FXV_TEXTURE = 0,
        FXV_UV = 1
    }

    bool firstTimeApply = true;

    MaterialEditor materialEditor;

    MaterialProperty _ColorProperty = null;

    MaterialProperty _RippleTexProperty = null;
    MaterialProperty _RippleScaleProperty = null;
    MaterialProperty _RippleDistortionProperty = null;

    MaterialProperty _PatternTexProperty = null;
    MaterialProperty _PatternScaleProperty = null;

    MaterialProperty _HitAttenuationProperty = null;
    MaterialProperty _HitPowerProperty = null;

    MaterialProperty _RefractionScaleProperty = null;

    public void FindProperties(MaterialProperty[] props)
    {
        _ColorProperty = FindProperty("_Color", props);

        _RippleTexProperty = FindProperty("_RippleTex", props);
        _RippleScaleProperty = FindProperty("_RippleScale", props);
        _RippleDistortionProperty = FindProperty("_RippleDistortion", props);

        _PatternTexProperty = FindProperty("_PatternTex", props);
        _PatternScaleProperty = FindProperty("_PatternScale", props);

        _HitAttenuationProperty = FindProperty("_HitAttenuation", props);
        _HitPowerProperty = FindProperty("_HitPower", props);

        _RefractionScaleProperty = FindProperty("_RefractionScale", props, false);

    }

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly

        materialEditor = editor;
        Material material = materialEditor.target as Material;

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a standard shader.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (firstTimeApply)
        {
            //MaterialChanged(material, m_WorkflowMode);
            firstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }


    public void ShaderPropertiesGUI(Material targetMat)
    {
        string[] keyWords = targetMat.shaderKeywords;

        bool usePatternTexture = keyWords.Contains("USE_PATTERN_TEXTURE");
        bool useDistortionForPatternTexture = keyWords.Contains("USE_DISTORTION_FOR_PATTERN_TEXTURE");
        bool useRefraction = keyWords.Contains("USE_REFRACTION");

        EditorGUI.BeginChangeCheck();

        targetMat.SetVector("_HitPos", new Vector3(0.0f, 0.0f, -0.5f));

        GUILayout.Label("Blend Mode", EditorStyles.boldLabel);
        {
            FXV_BLEND_MODE_OPTIONS blendModeType = FXV_BLEND_MODE_OPTIONS.FXV_ADDITIVE_BLEND;
            if (targetMat.GetInt("_BlendSrcMode") == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
                targetMat.GetInt("_BlendDstMode") == (int)UnityEngine.Rendering.BlendMode.One)
                blendModeType = FXV_BLEND_MODE_OPTIONS.FXV_ADDITIVE_BLEND;
            if (targetMat.GetInt("_BlendSrcMode") == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
                targetMat.GetInt("_BlendDstMode") == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
                blendModeType = FXV_BLEND_MODE_OPTIONS.FXV_ALPHA_BLEND;

            if (useRefraction)
            {
                blendModeType = FXV_BLEND_MODE_OPTIONS.FXV_ALPHA_BLEND;
                GUILayout.Label("cant change when refraction is on", EditorStyles.label);
            }
            else
                blendModeType = (FXV_BLEND_MODE_OPTIONS)EditorGUILayout.EnumPopup("", blendModeType);

            if (blendModeType == FXV_BLEND_MODE_OPTIONS.FXV_ADDITIVE_BLEND)
            {
                targetMat.SetInt("_BlendSrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                targetMat.SetInt("_BlendDstMode", (int)UnityEngine.Rendering.BlendMode.One);
            }
            else if (blendModeType == FXV_BLEND_MODE_OPTIONS.FXV_ALPHA_BLEND)
            {
                targetMat.SetInt("_BlendSrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                targetMat.SetInt("_BlendDstMode", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            }
        }

        GUILayout.Label("Color", EditorStyles.boldLabel);

        materialEditor.ColorProperty(_ColorProperty, "Color");
        materialEditor.ShaderProperty(_HitAttenuationProperty, "Hit Attenuation");
        materialEditor.ShaderProperty(_HitPowerProperty, "Hit Power");

        GUILayout.Label("Pattern Texture", EditorStyles.boldLabel);

        usePatternTexture = EditorGUILayout.Toggle("Enabled", usePatternTexture);

        if (usePatternTexture)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _PatternTexProperty);
            materialEditor.ShaderProperty(_PatternScaleProperty, "Pattern Scale");

            GUILayout.Label("Distortion Ripple Texture", EditorStyles.boldLabel);

            useDistortionForPatternTexture = EditorGUILayout.Toggle("Enabled", useDistortionForPatternTexture);

            if (useDistortionForPatternTexture)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _RippleTexProperty);
                materialEditor.ShaderProperty(_RippleScaleProperty, "Ripple Scale");
                materialEditor.ShaderProperty(_RippleDistortionProperty, "Ripple Distortion");
            }
        }

        if (_RefractionScaleProperty != null)
        {
            GUILayout.Label("Refraction", EditorStyles.boldLabel);

            useRefraction = EditorGUILayout.Toggle("Enabled", useRefraction);

            if (useRefraction)
            {
                materialEditor.ShaderProperty(_RefractionScaleProperty, "Refraction Scale");
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            List<string> keywords = new List<string>();

            if (usePatternTexture)
                keywords.Add("USE_PATTERN_TEXTURE");

            if (useDistortionForPatternTexture)
                keywords.Add("USE_DISTORTION_FOR_PATTERN_TEXTURE");

            if (useRefraction)
                keywords.Add("USE_REFRACTION");

            targetMat.shaderKeywords = keywords.ToArray();
            EditorUtility.SetDirty(targetMat);
        }
    }
}
