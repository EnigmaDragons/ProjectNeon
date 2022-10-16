#if UNITY_EDITOR
using I2.Loc;
using UnityEditor;
using UnityEngine;

public class I2LocPreview : EditorWindow
{
    [MenuItem("Neon/Localize Preview")]
    static void LocalizationPreviewByKey()
    {
        GetWindow(typeof(I2LocPreview)).Show();
    }

    private string _key;

    void OnGUI()
    {
        _key = EditorGUILayout.TextField("Raw Key:", _key);
        if (GUILayout.Button("Preview In Console"))
        {
            var val = new LocalizedString(_key).ToString();
            Debug.Log($"Key: {_key}. Val: {val}");
        }
    }
}
#endif
