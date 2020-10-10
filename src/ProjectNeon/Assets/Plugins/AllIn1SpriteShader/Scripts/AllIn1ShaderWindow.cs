#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class AllIn1ShaderWindow : EditorWindow
{
    [MenuItem("Window/AllIn1ShaderWindow")]
    public static void ShowAllIn1ShaderWindowWindow()
    {
        GetWindow<AllIn1ShaderWindow>("All In 1 Shader Window");
    }

    private DefaultAsset materialTargetFolder = null;

    private void OnGUI()
    {
        GUILayout.Label("Through this window you'll be able to set the folder where the asset will save it's Materials",
            EditorStyles.boldLabel);

        GUILayout.Space(20);

        materialTargetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
             "New Material Folder",
             materialTargetFolder,
             typeof(DefaultAsset),
             false);

        if (materialTargetFolder != null && IsAssetAFolder(materialTargetFolder))
        {
            string path = AssetDatabase.GetAssetPath(materialTargetFolder);
            PlayerPrefs.SetString("All1ShaderMaterials", path);
            EditorGUILayout.HelpBox(
                "Valid folder! Material save path: " + path,
                MessageType.Info,
                true);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Select the new Material Folder",
                MessageType.Warning,
                true);
        }
    }

    private static bool IsAssetAFolder(Object obj)
    {
        string path = "";

        if (obj == null) return false;

        path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

        if (path.Length > 0)
        {
            if (Directory.Exists(path)) return true;
            else return false;
        }
        return false;
    }
}
#endif