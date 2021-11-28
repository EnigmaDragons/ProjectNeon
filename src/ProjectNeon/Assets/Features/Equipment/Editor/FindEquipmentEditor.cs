#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindEquipmentEditor : EditorWindow
{
    [MenuItem("Neon/Find Equipments")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindEquipmentEditor)).Show();
    }

    // By Effect Type
    private EffectType _effectType;
    private string[] GetAllEffectsWithType(EffectType effectType) =>
        GetAllInstances<StaticEquipment>()
            .Where(e => e.AllEffects.Any(x => x.EffectType == effectType))
            .Select(e => e.name)
            .ToArray();
    
    // By Description Search String
    private string _searchString;
    private string[] GetAllWithDescription(string s) =>
        GetAllInstances<StaticEquipment>()
            .Where(c => c.Description.ContainsAnyCase(s))
            .Select(e => e.name)
            .ToArray();
    
    // By Archetype 
    private string _archetype;
    
    // By Set of Archetypes
    private string _archetypesString;

    // By Hero
    private string _heroString;
    
    //By ID
    private int _id;
    
    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        if (GUILayout.Button("Search By Effect Type"))
        {
            var effects = GetAllEffectsWithType(_effectType);
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_effectType} - {effects.Length} uses", effects)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        var raritySortOrder = new List<Rarity> {Rarity.Basic, Rarity.Starter, Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic};
        static string WipWord(bool isWip) => isWip ? "WIP - " : "";
        _archetype = GUILayout.TextField(_archetype);
        if (GUILayout.Button("Find By Archetype"))
        {
            var items = GetAllInstances<StaticEquipment>()
                .Where(c => c.GetArchetypeKey().Equals(_archetype))
                .OrderBy(e => e.IsWip ? 99 : 0)
                .ThenBy(e => raritySortOrder.IndexOf(e.Rarity))
                .Select(e => e.EditorName)
                .ToArray();
            Show($"Archetype {_archetype}", items);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Show All Unused Effects"))
        {
            var zeroUsageResults = Enum.GetValues(typeof(EffectType)).Cast<EffectType>()
                .Select(effectType => (effectType, GetAllEffectsWithType(effectType)))
                .Where(e => e.Item2.Length == 0)
                .Where(e => e.effectType != EffectType.Nothing)
                .Select(e => e.effectType.ToString())
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized("Unused Effect Types", zeroUsageResults)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _searchString = GUILayout.TextField(_searchString);
        if (GUILayout.Button("Find Term in Equipment Description"))
        {
            var items = GetAllWithDescription(_searchString);
            Show($"Description Containing {_searchString}", items);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _archetypesString = GUILayout.TextField(_archetypesString);
        if (GUILayout.Button("Find By Set of Comma-Separated Archetypes"))
        {
            var archetypes = _archetypesString.Split(',').Select(s => s.Trim()).ToList();
            var archetypeKeys = new HashSet<string>();
            
            // Singles
            archetypes.ForEach(a => archetypeKeys.Add(a));
            // Duals
            archetypes.Permutations(2).Select(p => string.Join(" + ", p))
                .ForEach(a => archetypeKeys.Add(a));
            // Triple
            archetypeKeys.Add(string.Join(" + ", archetypes));

            var items = GetAllInstances<StaticEquipment>()
                .Where(c => archetypeKeys.Contains(c.GetArchetypeKey()))
                .OrderBy(e => e.IsWip ? 99 : 0)
                .ThenBy(e => raritySortOrder.IndexOf(e.Rarity))
                .Select(e => e.EditorName)
                .ToArray();
            Show($"Archetype Set Is {_archetypesString}. Total: {items.Length}", items);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _heroString = GUILayout.TextField(_heroString);
        if (GUILayout.Button("Find By Hero"))
        {
            var heroes = GetAllInstances<BaseHero>()
                .Where(h => h.Name.Equals(_heroString, StringComparison.InvariantCultureIgnoreCase));
            if (!heroes.Any())
                GUIUtility.ExitGUI();

            var hero = heroes.First();
            var archetypeKeys = hero.ArchetypeKeys;

            var items = GetAllInstances<StaticEquipment>()
                .Where(c => archetypeKeys.Contains(c.GetArchetypeKey()))
                .OrderBy(e => e.IsWip ? 99 : 0)
                .ThenBy(e => raritySortOrder.IndexOf(e.Rarity))
                .Select(e => e.EditorName)
                .ToArray();
            Show($"Hero {_heroString}. Total: {items.Length}", items);
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        int.TryParse(GUILayout.TextField(_id.ToString()), out _id);
        if (GUILayout.Button("Find By ID"))
        {
            var items = GetAllInstances<StaticEquipment>()
                .Where(c => c.id == _id)
                .Select(e => e.EditorName)
                .ToArray();
            Show($"Archetype {_archetype}", items);
            GUIUtility.ExitGUI();
        }
    }
    
    private void Show(string description, string[] items) 
        => GetWindow<ListDisplayWindow>()
            .Initialized(description, items)
            .Show();
    
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

    private void DrawUILine() => DrawUILine(Color.black);
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}

#endif
