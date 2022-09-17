using UnityEngine;
using UnityEditor;

public class WallFX_GUI_v2 : ShaderGUI
{

    MaterialEditor editor;
    MaterialProperty[] properties;
    bool TargetMode;

    //get preperties function
    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }
    //

    ////
    static GUIContent staticLabel = new GUIContent();
    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }
    ////

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        DoMain();

    }


    // GUI FUNCTION	
    void DoMain()
    {
        //--- Logo
        Texture2D myGUITexture = (Texture2D)Resources.Load("WallFX_PACK");
        GUILayout.Label(myGUITexture, EditorStyles.centeredGreyMiniLabel);

        //LABELS
        GUILayout.Label("/---------------/ WALL FX PACK /---------------/", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Label("MAPS", EditorStyles.helpBox);

        // MASK
        // get properties
        MaterialProperty _DistortionMask = ShaderGUI.FindProperty("_DistortionMask", properties);

        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_DistortionMask, "Mask Map"), _DistortionMask, FindProperty("_MaskDistortion"));

        GUILayout.Label("Colors Settings", EditorStyles.helpBox);

        MaterialProperty _MainColor = FindProperty("_MainColor");
        editor.ShaderProperty(_MainColor, MakeLabel(_MainColor));

        MaterialProperty _EmissiveMult = FindProperty("_EmissiveMult");
        editor.ShaderProperty(_EmissiveMult, MakeLabel(_EmissiveMult));

        MaterialProperty _WallColorMult = FindProperty("_WallColorMult");
        editor.ShaderProperty(_WallColorMult, MakeLabel(_WallColorMult));

        MaterialProperty _WallOutlineColorMult = FindProperty("_WallOutlineColorMult");
        editor.ShaderProperty(_WallOutlineColorMult, MakeLabel(_WallOutlineColorMult));

        MaterialProperty _OutlinesMult = FindProperty("_OutlinesMult");
        editor.ShaderProperty(_OutlinesMult, MakeLabel(_OutlinesMult));


        GUILayout.Label("Displacement Settings", EditorStyles.helpBox);
        MaterialProperty _StretchingAnime = FindProperty("_StretchingAnime");
        editor.ShaderProperty(_StretchingAnime, MakeLabel(_StretchingAnime));


        GUILayout.Label("Frame Settings", EditorStyles.helpBox);
        MaterialProperty _DisplayFrame = FindProperty("_DisplayFrame");
        editor.ShaderProperty(_DisplayFrame, MakeLabel(_DisplayFrame));

        MaterialProperty _FrameEmissive = FindProperty("_FrameEmissive");
        editor.ShaderProperty(_FrameEmissive, MakeLabel(_FrameEmissive));


        GUILayout.Label("Dynamic Settings", EditorStyles.helpBox);
        MaterialProperty _WallOpening = FindProperty("_WallOpening");
        editor.ShaderProperty(_WallOpening, MakeLabel(_WallOpening));

        MaterialProperty _AnimatedOpening = FindProperty("_AnimatedOpening");
        editor.ShaderProperty(_AnimatedOpening, MakeLabel(_AnimatedOpening));

        MaterialProperty _SpeedAnimatedMode = FindProperty("_SpeedAnimatedMode");
        editor.ShaderProperty(_SpeedAnimatedMode, MakeLabel(_SpeedAnimatedMode));






    }
}