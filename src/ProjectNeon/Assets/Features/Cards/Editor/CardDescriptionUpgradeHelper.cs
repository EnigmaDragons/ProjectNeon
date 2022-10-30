using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardDescriptionUpgradeHelper
{
    [MenuItem ("Neon/Next Card %#_z")]
    static void SelectNextCard()
    {
        var cards = GetAllInstances<CardType>()
            .Where(c => !c.IsWip && !c.DescriptionV2.IsUsable());
        
        if (cards.Any())
            Selection.objects = new Object[] { cards.First() };
        else
            Log.Info("All Finished! Yay!");
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
