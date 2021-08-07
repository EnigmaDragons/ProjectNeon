#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class ContentSummarizerEditor : EditorWindow
{
    [MenuItem("Neon/Content Summary")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(ContentSummarizerEditor)).Show();
    }

    private static string HeroName;
    
    void OnGUI()
    {
        if (GUILayout.Button("Cards By Archetypes"))
        {
            var result = GetAllInstances<CardType>()
                .Where(c => !c.IsWip)
                .GroupBy(x => x.GetArchetypeKey())
                .ToDictionary(
                    x => x.Key, // By Archetype 
                    x => x.GroupBy(g => g.Rarity).OrderBy(r => (int)r.Key) // By Rarity
                        .ToDictionary(
                            r => r.Key, 
                            r => r.Count()))
                .OrderByDescending(x => x.Value.Sum(v => v.Value))
                .Select(x => $"{x.Key} - Total {x.Value.Sum(v => v.Value)} - {string.Join(", " , x.Value.Select(v => $"{v.Key}: {v.Value}"))}")
                .ToArray();

            GetWindow<ListDisplayWindow>()
                .Initialized($"Cards By Archetype", "", result)
                .Show();
            GUIUtility.ExitGUI();
        }
        
        if (GUILayout.Button("Cards By Cost"))
        {
            var result = GetAllInstances<CardType>()
                .Where(c => !c.IsWip)
                .GroupBy(x => x.GetArchetypeKey())
                .ToDictionary(
                    x => x.Key, // By Archetype 
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
                .GroupBy(x => x.GetArchetypeKey())
                .ToDictionary(
                    x => x.Key, // By Archetype 
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
        
        DrawUILine();
        HeroName = GUILayout.TextField(HeroName);
        if (GUILayout.Button("Hero Content Summary"))
        {
            var hero = GetAllInstances<BaseHero>().FirstOrDefault(x => x.Name.Equals(HeroName));
            if (hero == null)
                GUIUtility.ExitGUI();
            
            var result = new List<string>();
            
            var archetypeKeys = hero.ArchetypeKeys;
            var cards = GetAllInstances<CardType>()
                .Where(c => !c.IsWip && archetypeKeys.Contains(c.GetArchetypeKey()))
                .GroupBy(c => c.GetArchetypeKey())
                .ToDictionary(
                    x => x.Key, // By Archetype 
                    x => x.GroupBy(g => g.Rarity).OrderBy(r => (int)r.Key) // By Rarity
                        .ToDictionary(
                            r => r.Key,
                            r => r.Count()));
            
            var equipments = GetAllInstances<StaticEquipment>()
                .Where(e => archetypeKeys.Contains(e.ArchetypeKey))
                .GroupBy(c => c.ArchetypeKey)
                .ToDictionary(
                    x => x.Key, // By Archetype 
                    x => x.GroupBy(g => g.Rarity).OrderBy(r => (int)r.Key) // By Rarity
                        .ToDictionary(
                            r => r.Key,
                            r => r.Count()));

            var expectedCardsCounter = 0;
            var presentCardsCounter = 0;
            foreach (var arch in archetypeKeys.Where(a => a.Count(c => c.Equals('+')) < 2))
            {
                var hasValue = cards.TryGetValue(arch, out var a);
                var numCards = hasValue ? a.Sum(v => v.Value) : 0;
                var archExpected = ArchCardsExpected(arch);
                expectedCardsCounter += archExpected;
                presentCardsCounter += numCards;
                var checkChar = numCards >= archExpected ? "✓" : "✗";
                result.Add(hasValue
                    ? $"{checkChar} - {arch} Cards - Total {numCards} - {string.Join(", ", a.Select(v => $"{v.Key}: {v.Value}{TargetCardNumbers(arch, v.Key)}"))}"
                    : $"{checkChar} - {arch} Cards - All {ArchCardsExpectedStr(arch)} Missing");
            }

            var expectedEquipCounter = 0;
            var presentEquipCounter = 0;
            foreach (var arch in archetypeKeys.Where(a => !a.Contains("+")))
            {
                var hasValue = equipments.TryGetValue(arch, out var e);
                var numEquips = hasValue ? e.Count(v => v.Value > 0) : 0;
                var archRaritiesExpected = 4;
                expectedEquipCounter += archRaritiesExpected;
                presentEquipCounter += numEquips;
                var checkChar = numEquips >= archRaritiesExpected ? "✓" : "✗";
                result.Add(hasValue
                    ? $"{checkChar} - {arch} Equips - Total {numEquips} - {string.Join(", ", e.Select(v => $"{v.Key}: {v.Value}{TargetEquipmentNumbers(arch, v.Key)}"))}"
                    : $"{checkChar} - {arch} Equips - All 4 Rarities Missing");
            }

            var expectedAllCounter = expectedCardsCounter + expectedEquipCounter;
            var presentAllCounter = presentCardsCounter + presentEquipCounter;
            var percentage = expectedAllCounter > 0 
                ? presentAllCounter/(float)expectedAllCounter
                : 0;
            var finalCheckChar = presentAllCounter >= expectedAllCounter ? "✓" : "✗";
            result.Add($"{finalCheckChar} - {HeroName} - {percentage:P} - All {presentAllCounter}/{expectedAllCounter} Cards {presentCardsCounter}/{expectedCardsCounter} Equips {presentEquipCounter}/{expectedEquipCounter}");
            result = result.OrderBy(r => r.Contains("%") ? -1 : 0).ToList();
            
            GetWindow<ListDisplayWindow>()
                .Initialized($"Hero Content Summary", "", result.ToArray())
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Augments By Corp"))
        {
            var result = GetAllInstances<StaticEquipment>()
                .Where(x => x.Slot == EquipmentSlot.Augmentation)
                .GroupBy(x => x.Corp)
                .ToDictionary(
                    x => x.Key, // By Corp 
                    x => x.GroupBy(g => g.Rarity).OrderBy(r => r.Key) // By Rarity
                        .ToDictionary(
                            r => r.Key, 
                            r => r.Count()))
                .OrderByDescending(x => x.Value.Sum(v => v.Value))
                .Select(x => $"{x.Key} - Total {x.Value.Sum(v => v.Value)} - {string.Join(", " , x.Value.Select(v => $"{v.Key}: {v.Value}"))}")
                .ToArray();

            GetWindow<ListDisplayWindow>()
                .Initialized($"Augments By Corp", "", result)
                .Show();
            GUIUtility.ExitGUI();
        }
    }

    private string ArchCardsExpectedStr(string arch) => ArchCardsExpected(arch).ToString();
    private int ArchCardsExpected(string arch) => arch.Contains("+") ? 5 : 12;
    
    private string TargetEquipmentNumbers(string arch, Rarity r)
    {
        if (arch.Contains("+"))
            return "";
        
        return r switch
        {
            Rarity.Common => "/1",
            Rarity.Uncommon => "/1",
            Rarity.Rare => "/1",
            Rarity.Epic => "/1",
            _ => ""
        };
    }
    
    private string TargetCardNumbers(string arch, Rarity r)
    {
        if (arch.Contains("+"))
        {
            return r switch
            {
                Rarity.Uncommon => "/2",
                Rarity.Rare => "/2",
                Rarity.Epic => "/1",
                _ => ""
            };
        }

        return r switch
        {
            Rarity.Common => "/4",
            Rarity.Uncommon => "/3",
            Rarity.Rare => "/2",
            Rarity.Epic => "/1",
            _ => ""
        };
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
