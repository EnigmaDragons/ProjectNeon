#if UNITY_EDITOR

using I2.Loc;
using UnityEditor;
using UnityEngine;

public class LocalizationImporter
{
    [MenuItem("Neon/Localization/Import Tutorial Slides")]
    public static void ImportTutorialSlides()
    {
        LocalizationManager.UpdateSources();  
        GetAllInstances<TutorialSlide>().ForEach(x =>
        {
            var text = LocalizationManager.GetTranslation(x.Term);
            if (!string.IsNullOrWhiteSpace(text) || text != x.Term)
                x.text = text;
        });
    }
    
    private static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);
        var a = new T[guids.Length];
        for(int i =0; i<guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
 
        return a;
    }
}

#endif