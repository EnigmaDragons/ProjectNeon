using UnityEngine;
using UnityEditor;

public class WallFX_GUI : ShaderGUI
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
        MaterialProperty _MASK_ = ShaderGUI.FindProperty("_MASK_", properties);

        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_MASK_, "Mask Map"), _MASK_);

        // Ring
        // get properties
        MaterialProperty _Ring = ShaderGUI.FindProperty("_Ring", properties);

        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_Ring, "Ring Map"), _Ring);



        //Outline Map
        // get properties
        MaterialProperty _Outline = ShaderGUI.FindProperty("_Outline", properties);

        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_Outline, "Outline Map"), _Outline);


        GUILayout.Label("Colors Settings", EditorStyles.helpBox);

        MaterialProperty _SurfaceOpacity = FindProperty("_SurfaceOpacity");
        editor.ShaderProperty(_SurfaceOpacity, MakeLabel(_SurfaceOpacity));

        MaterialProperty _ColorsHue = FindProperty("_ColorsHue");
        editor.ShaderProperty(_ColorsHue, MakeLabel(_ColorsHue));

        GUILayout.Label("Outline Settings", EditorStyles.helpBox);
        MaterialProperty _OutlineMult = FindProperty("_OutlineMult");
        editor.ShaderProperty(_OutlineMult, MakeLabel(_OutlineMult));

        MaterialProperty _TransitionOutlineMult = FindProperty("_TransitionOutlineMult");
        editor.ShaderProperty(_TransitionOutlineMult, MakeLabel(_TransitionOutlineMult));

        GUILayout.Label("Image Distortion Settings", EditorStyles.helpBox);
        MaterialProperty _ChromaticAberationMult = FindProperty("_ChromaticAberationMult");
        editor.ShaderProperty(_ChromaticAberationMult, MakeLabel(_TransitionOutlineMult));

        MaterialProperty _BlurMult = FindProperty("_BlurMult");
        editor.ShaderProperty(_BlurMult, MakeLabel(_BlurMult));

        GUILayout.Label("Displacement Settings", EditorStyles.helpBox);
        MaterialProperty _PushMult = FindProperty("_PushMult");
        editor.ShaderProperty(_PushMult, MakeLabel(_PushMult));

        GUILayout.Label("Dynamic Settings", EditorStyles.helpBox);
        MaterialProperty _WallOpening = FindProperty("_WallOpening");
        editor.ShaderProperty(_WallOpening, MakeLabel(_WallOpening));

        MaterialProperty _SpeedAnimatedMode = FindProperty("_SpeedAnimatedMode");
        editor.ShaderProperty(_SpeedAnimatedMode, MakeLabel(_SpeedAnimatedMode));

        MaterialProperty _AnimatedOpening = FindProperty("_AnimatedOpening");
        editor.ShaderProperty(_AnimatedOpening, MakeLabel(_AnimatedOpening));


        GUILayout.Label("Debug Settings", EditorStyles.helpBox);
        MaterialProperty _DebugColor = FindProperty("_DebugColor");
        editor.ShaderProperty(_DebugColor, MakeLabel(_DebugColor));




    }
}