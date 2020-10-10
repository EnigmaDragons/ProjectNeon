using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

[CanEditMultipleObjects]
public class AllIn1SpriteShaderStencil : ShaderGUI
{
    Material targetMat;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        targetMat = materialEditor.target as Material;
        string[] oldKeyWords = targetMat.shaderKeywords;
        GUIStyle style = EditorStyles.helpBox;
        style.margin = new RectOffset(0, 0, 0, 0);

        materialEditor.ShaderProperty(properties[0], properties[0].displayName);
        materialEditor.ShaderProperty(properties[1], properties[1].displayName);
        materialEditor.ShaderProperty(properties[2], properties[2].displayName);

        //Not needed since Unity batches sprites on its own
        //EditorGUILayout.Separator();
        //materialEditor.EnableInstancingField();
        //Debug.Log(materialEditor.IsInstancingEnabled() + "  " + Application.isBatchMode);

        EditorGUILayout.Separator();
        SpriteAtlas(materialEditor, style, oldKeyWords.Contains("ATLAS_ON"), "Sprite inside an atlas?", "ATLAS_ON");

        EditorGUILayout.Separator();
        GUILayout.Label("___Color Effects___", EditorStyles.boldLabel);

        Glow(materialEditor, properties, style, oldKeyWords.Contains("GLOW_ON"));
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("FADE_ON"), "2.Fade", "FADE_ON", 7, 13);
        Outline(materialEditor, properties, style, oldKeyWords.Contains("OUTBASE_ON"));
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("GRADIENT_ON"), "4.Gradient", "GRADIENT_ON", 31, 35);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("COLORSWAP_ON"), "5.Color Swap", "COLORSWAP_ON", 36, 42);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("HSV_ON"), "6.Hue Shift", "HSV_ON", 43, 45);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("CHANGECOLOR_ON"), "7.Change 1 Color", "CHANGECOLOR_ON", 123, 126);
        ColorRamp(materialEditor, properties, style, oldKeyWords.Contains("COLORRAMP_ON"));
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("HITEFFECT_ON"), "9.Hit Effect", "HITEFFECT_ON", 46, 48);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("NEGATIVE_ON"), "10.Negative", "NEGATIVE_ON", 49, 49);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("PIXELATE_ON"), "11.Pixelate", "PIXELATE_ON", 50, 50);
        GreyScale(materialEditor, properties, style, oldKeyWords.Contains("GREYSCALE_ON"));
        Posterize(materialEditor, properties, style, oldKeyWords.Contains("POSTERIZE_ON"));
        Blur(materialEditor, properties, style, oldKeyWords.Contains("BLUR_ON"));
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("MOTIONBLUR_ON"), "15.Motion Blur", "MOTIONBLUR_ON", 62, 63);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("GHOST_ON"), "16.Ghost", "GHOST_ON", 64, 65);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("INNEROUTLINE_ON"), "17.Inner Outline", "INNEROUTLINE_ON", 66, 69);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("HOLOGRAM_ON"), "18.Hologram", "HOLOGRAM_ON", 73, 77);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("CHROMABERR_ON"), "19.Chromatic Aberration", "CHROMABERR_ON", 78, 79);
        Glitch(materialEditor, properties, style, oldKeyWords.Contains("GLITCH_ON"), "20.Glitch", "GLITCH_ON");
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("FLICKER_ON"), "21.Flicker", "FLICKER_ON", 81, 83);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("SHADOW_ON"), "22.Shadow", "SHADOW_ON", 84, 87);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("SHINE_ON"), "23.Shine", "SHINE_ON", 130, 134);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("ALPHACUTOFF_ON"), "24.Alpha Cutoff", "ALPHACUTOFF_ON", 70, 70);

        EditorGUILayout.Separator();
        GUILayout.Label("___UV Effects___", EditorStyles.boldLabel);

        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("DOODLE_ON"), "25.Hand Drawn", "DOODLE_ON", 88, 89);
        Grass(materialEditor, properties, style, oldKeyWords.Contains("WIND_ON"));
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("WAVEUV_ON"), "27.Wave", "WAVEUV_ON", 94, 98);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("ROUNDWAVEUV_ON"), "28.Round Wave", "ROUNDWAVEUV_ON", 127, 128);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("RECTSIZE_ON"), "29.Rect Size (Enable wireframe to see result)", "RECTSIZE_ON", 99, 99);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("OFFSETUV_ON"), "30.Offset", "OFFSETUV_ON", 100, 101);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("CLIPPING_ON"), "31.Clipping / Fill Amount", "CLIPPING_ON", 102, 105);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("TEXTURESCROLL_ON"), "32.Texture Scroll", "TEXTURESCROLL_ON", 106, 107);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("ZOOMUV_ON"), "33.Zoom", "ZOOMUV_ON", 108, 108);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("DISTORT_ON"), "34.Distortion", "DISTORT_ON", 109, 112);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("TWISTUV_ON"), "35.Twist", "TWISTUV_ON", 113, 116);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("ROTATEUV_ON"), "36.Rotate", "ROTATEUV_ON", 117, 117);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("POLARUV_ON"), "37.Polar Coordinates (Tile texture for good results)", "POLARUV_ON", -1, -1);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("FISHEYE_ON"), "38.Fish Eye", "FISHEYE_ON", 118, 118);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("PINCH_ON"), "39.Pinch", "PINCH_ON", 119, 119);
        GenericEffect(materialEditor, properties, style, oldKeyWords.Contains("SHAKEUV_ON"), "40.Shake", "SHAKEUV_ON", 120, 122);
    }

    private void SpriteAtlas(MaterialEditor materialEditor, GUIStyle style, bool toggle, string inspector, string flag)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(flag);
            EditorGUILayout.BeginVertical(style);
            {
                GUILayout.Label("Make sure SpriteAtlasUV component is added \n " +
                    "*Check documentation if unsure what this does or how it works", style);
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword(flag);
        EditorGUILayout.EndToggleGroup();
    }

    private void Outline(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("3.Outline", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("OUTBASE_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[14], properties[14].displayName);
                materialEditor.ShaderProperty(properties[15], properties[15].displayName);
                materialEditor.ShaderProperty(properties[16], properties[16].displayName);
                materialEditor.ShaderProperty(properties[17], properties[17].displayName);
                MaterialProperty outline8dir = ShaderGUI.FindProperty("_Outline8Directions", properties);
                if (outline8dir.floatValue == 1) targetMat.EnableKeyword("OUTBASE8DIR_ON");
                else targetMat.DisableKeyword("OUTBASE8DIR_ON");

                materialEditor.ShaderProperty(properties[19], properties[19].displayName);
                MaterialProperty outlinePixel = ShaderGUI.FindProperty("_OutlineIsPixel", properties);
                if (outlinePixel.floatValue == 1)
                {
                    targetMat.EnableKeyword("OUTBASEPIXELPERF_ON");
                    materialEditor.ShaderProperty(properties[20], properties[20].displayName);
                }
                else
                {
                    targetMat.DisableKeyword("OUTBASEPIXELPERF_ON");
                    materialEditor.ShaderProperty(properties[18], properties[18].displayName);
                }

                materialEditor.ShaderProperty(properties[21], properties[21].displayName);
                MaterialProperty outlineTex = ShaderGUI.FindProperty("_OutlineTexToggle", properties);
                if (outlineTex.floatValue == 1)
                {
                    targetMat.EnableKeyword("OUTTEX_ON");
                    materialEditor.ShaderProperty(properties[22], properties[22].displayName);
                    materialEditor.ShaderProperty(properties[23], properties[23].displayName);
                    materialEditor.ShaderProperty(properties[24], properties[24].displayName);
                    materialEditor.ShaderProperty(properties[25], properties[25].displayName);
                    MaterialProperty outlineTexGrey = ShaderGUI.FindProperty("_OutlineTexGrey", properties);
                    if (outlineTexGrey.floatValue == 1) targetMat.EnableKeyword("OUTGREYTEXTURE_ON");
                    else targetMat.DisableKeyword("OUTGREYTEXTURE_ON");
                }
                else targetMat.DisableKeyword("OUTTEX_ON");

                materialEditor.ShaderProperty(properties[26], properties[26].displayName);
                MaterialProperty outlineDistort = ShaderGUI.FindProperty("_OutlineDistortToggle", properties);
                if (outlineDistort.floatValue == 1)
                {
                    targetMat.EnableKeyword("OUTDIST_ON");
                    materialEditor.ShaderProperty(properties[27], properties[27].displayName);
                    materialEditor.ShaderProperty(properties[28], properties[28].displayName);
                    materialEditor.ShaderProperty(properties[29], properties[29].displayName);
                    materialEditor.ShaderProperty(properties[30], properties[30].displayName);
                }
                else targetMat.DisableKeyword("OUTDIST_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("OUTBASE_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void GenericEffect(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle, string inspector, string flag, int first, int last)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(flag);
            if (first > 0)
            {
                EditorGUILayout.BeginVertical(style);
                {
                    for (int i = first; i <= last; i++) materialEditor.ShaderProperty(properties[i], properties[i].displayName);
                }
                EditorGUILayout.EndVertical();
            }
        }
        else targetMat.DisableKeyword(flag);
        EditorGUILayout.EndToggleGroup();
    }

    private void Glow(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("1.Glow", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("GLOW_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[3], properties[3].displayName);
                materialEditor.ShaderProperty(properties[4], properties[4].displayName);
                materialEditor.ShaderProperty(properties[5], properties[5].displayName);
                MaterialProperty useGlowTex = ShaderGUI.FindProperty("_GlowTexUsed", properties);
                if (useGlowTex.floatValue == 1)
                {
                    targetMat.EnableKeyword("GLOWTEX_ON");
                    materialEditor.ShaderProperty(properties[6], properties[6].displayName);
                }
                else targetMat.DisableKeyword("GLOWTEX_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("GLOW_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void ColorRamp(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("8.Color Ramp", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("COLORRAMP_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[51], properties[51].displayName);
                materialEditor.ShaderProperty(properties[52], properties[52].displayName);
                materialEditor.ShaderProperty(properties[53], properties[53].displayName);
                MaterialProperty colorRampOut = ShaderGUI.FindProperty("_ColorRampOutline", properties);
                if (colorRampOut.floatValue == 1) targetMat.EnableKeyword("COLORRAMPOUTLINE_ON");
                else targetMat.DisableKeyword("COLORRAMPOUTLINE_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("COLORRAMP_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void GreyScale(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("12.Greyscale", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("GREYSCALE_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[54], properties[54].displayName);
                materialEditor.ShaderProperty(properties[55], properties[55].displayName);
                materialEditor.ShaderProperty(properties[56], properties[56].displayName);
                MaterialProperty greyScaleOut = ShaderGUI.FindProperty("_GreyscaleOutline", properties);
                if (greyScaleOut.floatValue == 1) targetMat.EnableKeyword("GREYSCALEOUTLINE_ON");
                else targetMat.DisableKeyword("GREYSCALEOUTLINE_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("GREYSCALE_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void Posterize(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("13.Posterize", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("POSTERIZE_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[57], properties[57].displayName);
                materialEditor.ShaderProperty(properties[58], properties[58].displayName);
                materialEditor.ShaderProperty(properties[59], properties[59].displayName);
                MaterialProperty posterizeOut = ShaderGUI.FindProperty("_PosterizeOutline", properties);
                if (posterizeOut.floatValue == 1) targetMat.EnableKeyword("POSTERIZEOUTLINE_ON");
                else targetMat.DisableKeyword("POSTERIZEOUTLINE_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("POSTERIZE_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void Blur(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("14.Blur", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("BLUR_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[60], properties[60].displayName);
                materialEditor.ShaderProperty(properties[61], properties[61].displayName);
                MaterialProperty blurIsHd = ShaderGUI.FindProperty("_BlurHD", properties);
                if (blurIsHd.floatValue == 1) targetMat.EnableKeyword("BLURISHD_ON");
                else targetMat.DisableKeyword("BLURISHD_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("BLUR_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void Grass(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup("26.Grass Movement / Wind", toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword("WIND_ON");
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[90], properties[90].displayName);
                materialEditor.ShaderProperty(properties[91], properties[91].displayName);
                materialEditor.ShaderProperty(properties[92], properties[92].displayName);
                materialEditor.ShaderProperty(properties[93], properties[93].displayName);
                MaterialProperty grassManual = ShaderGUI.FindProperty("_GrassManualToggle", properties);
                if (grassManual.floatValue == 1) targetMat.EnableKeyword("MANUALWIND_ON");
                else targetMat.DisableKeyword("MANUALWIND_ON");
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword("WIND_ON");
        EditorGUILayout.EndToggleGroup();
    }

    private void Glitch(MaterialEditor materialEditor, MaterialProperty[] properties, GUIStyle style, bool toggle, string inspector, string flag)
    {
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(flag);
            EditorGUILayout.BeginVertical(style);
            {
                materialEditor.ShaderProperty(properties[80], properties[80].displayName);
                materialEditor.ShaderProperty(properties[135], properties[135].displayName);
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword(flag);
        EditorGUILayout.EndToggleGroup();
    }
}