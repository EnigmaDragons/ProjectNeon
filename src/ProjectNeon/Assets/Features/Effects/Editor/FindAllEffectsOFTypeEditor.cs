using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindAllEffectsOfTypeEditor : EditorWindow
{
    private EffectType _effectType;
    
    [MenuItem("Tools/Neon/Find All Effects Of Type")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindAllEffectsOfTypeEditor)).Show();
    }

    private string[] GetAllEffectsWithType(EffectType effectType) =>
        GetAllInstances<CardActionsData>()
            .Where(e => e.Actions.Any(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == effectType))
            .Select(e => e.name)
            .ToArray();

    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        
        if (GUILayout.Button("Search")) 
        {
            var effects = GetAllEffectsWithType(_effectType);
            var result = GetWindow<ListDisplayWindow>();
            result.Items = effects;
            result.Show();
            GUIUtility.ExitGUI();
        }
    }
    
    private static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
        var a = new T[guids.Length];
        for(int i =0; i<guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
 
        return a;
    }
}
