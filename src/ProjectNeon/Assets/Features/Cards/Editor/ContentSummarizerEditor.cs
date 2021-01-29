using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Features.Cards.Editor
{
    public sealed class ContentSummarizerEditor : EditorWindow
    {
        [MenuItem("Tools/Neon/Content Summary")]
        static void SelectGameObjects()
        {
            GetWindow(typeof(ContentSummarizerEditor)).Show();
        }
        
        void OnGUI()
        {
            if (GUILayout.Button("Cards By Class"))
            {
                var result = GetAllInstances<CardType>()
                    .GroupBy(x => x.LimitedToClass.Select(c => c.Name, "Enemy"))
                    .ToDictionary(
                        x => x.Key, // By Class 
                        x => x.GroupBy(g => g.Rarity).OrderBy(r => (int)r.Key) // By Rarity
                            .ToDictionary(
                                r => r.Key, 
                                r => r.Count()))
                    .OrderByDescending(x => x.Value.Sum(v => v.Value))
                    .Select(x => $"{x.Key} - Total {x.Value.Sum(v => v.Value)} - {string.Join(", " , x.Value.Select(v => $"{v.Key}: {v.Value}"))}")
                    .ToArray();

                GetWindow<ListDisplayWindow>()
                    .Initialized($"Cards By Class", "", result)
                    .Show();
                GUIUtility.ExitGUI();
            }
            
            if (GUILayout.Button("Cards By Cost"))
            {
                var result = GetAllInstances<CardType>()
                    .GroupBy(x => x.LimitedToClass.Select(c => c.Name, "Enemy"))
                    .ToDictionary(
                        x => x.Key, // By Class 
                        x => x.GroupBy(g => (g.Cost.ToString(), g.Cost.BaseAmount)).OrderBy(r => r.Key.BaseAmount) // By Cost
                            .ToDictionary(
                                r => r.Key.Item1, 
                                r => r.Count()))
                    .OrderByDescending(x => x.Value.Sum(v => v.Value))
                    .Select(x => $"{x.Key} - Total {x.Value.Sum(v => v.Value)} - {string.Join(", " , x.Value.Select(v => $"{v.Key}: {v.Value}"))}")
                    .ToArray();

                GetWindow<ListDisplayWindow>()
                    .Initialized($"Cards By Cost", "", result)
                    .Show();
                GUIUtility.ExitGUI();
            }
            
            if (GUILayout.Button("Cards By Type"))
            {
                var result = GetAllInstances<CardType>()
                    .GroupBy(x => x.LimitedToClass.Select(c => c.Name, "Enemy"))
                    .ToDictionary(
                        x => x.Key, // By Class 
                        x => x.GroupBy(g => g.TypeDescription).OrderBy(r => r.Key) // By Type
                            .ToDictionary(
                                r => r.Key, 
                                r => r.Count()))
                    .OrderByDescending(x => x.Value.Sum(v => v.Value))
                    .Select(x => $"{x.Key} - Total {x.Value.Sum(v => v.Value)} - {string.Join(", " , x.Value.Select(v => $"{v.Key}: {v.Value}"))}")
                    .ToArray();

                GetWindow<ListDisplayWindow>()
                    .Initialized($"Cards By Type", "", result)
                    .Show();
                GUIUtility.ExitGUI();
            }
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
}
