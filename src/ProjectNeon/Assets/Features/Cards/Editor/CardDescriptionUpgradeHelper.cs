using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardDescriptionUpgradeHelper
{
    [MenuItem("Neon/Card Descriptions/Upgrade All Card Descriptions %#_u")]
    static void UpgradeAllCardDescriptions()
    {
        CleanBadV2CardDescriptions();
        UpgradeCardDescriptions(99999);
    }

    [MenuItem("Neon/Card Descriptions/Select Next Auto Card %#_z")]
    static void SelectNextAutoCard()
    {
        GetAllInstances<CardType>()
            .Where(c => !c.IsWip)
            .Where(c => c.description.ContainsAnyCase("auto"))
            .Take(1)
            .FirstAsMaybe()
            .IfPresent(c =>
            {
                Log.Info($"Selecting Card {c.Name}");
                Selection.objects = new Object[] { c };
            });
    }
    
    private static void CleanBadV2CardDescriptions()
    {
        GetAllInstances<CardType>()
            .Where(c => c.DescriptionV2.IsUsable() && c.DescriptionV2.Preview().ContainsAnyCase("{Auto}"))
            .ForEach(c =>
            {
                c.descriptionV2 = new CardDescriptionV2();
                EditorUtility.SetDirty(c);
            });
    }
    
    private static void UpgradeCardDescriptions(int max = 1)
    {
        var cardsRemaining = GetAllInstances<CardType>()
            .Where(c => !c.IsWip && !c.DescriptionV2.IsUsable());
            
        if (!cardsRemaining.Any())
            Log.Info("All Finished! Yay!");

        cardsRemaining
            .Take(max)
            .ForEach(c =>
            {
                var d1 = c.description;
                if (d1.ContainsAnyCase("{Auto}"))
                {
                    Log.Warn($"Card {c.name} cannot be converted because it's Auto");
                    return;
                }

                var d2 = CardDescriptionV2.FromDescriptionV1(d1);

                var matches = d1.Equals(d2.Preview());
                if (matches)
                {
                    c.descriptionV2 = d2;
                    EditorUtility.SetDirty(c);
                    Log.Info($"Upversioned Description for Card {c.Name}");
                }

                if (!matches)
                    Log.Warn($"Unable to Auto-Convert Description for Card {c.Name}");
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
