#if UNITY_EDITOR
using System;
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
    private static string Archetype;
    
    void OnGUI()
    {
        Archetype = GUILayout.TextField(Archetype);
        if (GUILayout.Button("Archetype Content Summary"))
        {
            var cards = GetAllInstances<CardType>()
                .Where(c => c.IncludeInPools && c.Archetypes.Contains(Archetype))
                .Select(c => $"Card - {c.Name}");

            var equipments = GetAllInstances<StaticEquipment>()
                .Where(c => c.IncludeInPools && c.Archetypes.Contains(Archetype))
                .Select(e => $"Gear - {e.Name}");

            GetWindow<ListDisplayWindow>()
                .Initialized($"Content By Archetype", "", cards.Concat(equipments).ToArray())
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Cards By Archetypes"))
        {
            var result = GetAllInstances<CardType>()
                .Where(c => c.IncludeInPools)
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
                .Where(c => c.IncludeInPools)
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
        
        if (GUILayout.Button("Gear By Archetypes"))
        {
            var result = GetAllInstances<StaticEquipment>()
                .Where(c => c.IncludeInPools)
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
                .Initialized($"Gear By Archetype", "", result)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        HeroName = GUILayout.TextField(HeroName);
        if (GUILayout.Button("Hero Content Summary"))
        {
            var hero = GetAllInstances<BaseHero>().FirstOrDefault(x => x.NameTerm().ToEnglish().Equals(HeroName, StringComparison.InvariantCultureIgnoreCase));
            if (hero == null)
                GUIUtility.ExitGUI();

            var (result, hasEverything) = GetHeroContentResult(hero);
            
            GetWindow<ListDisplayWindow>()
                .Initialized($"Hero Content Summary", "", result.ToArray())
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("All Heroes Content Summary"))
        {
            var result = new List<string>();
            var heroes = GetAllInstances<Library>().First().UnlockedHeroes.OrderBy(x => x.NameTerm().ToEnglish());

            foreach (var hero in heroes)
            {
                var (heroResultDetail, hasEverything) = GetHeroContentResult(hero);
                result.Add($"{heroResultDetail.First()}");
            }
            
            GetWindow<ListDisplayWindow>()
                .Initialized($"All Heroes Content Summary", "", result.ToArray())
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

    private class HeroContentSummaryResult
    {
        public BaseHero Hero { get; }
    }
    
    private (List<string>, bool) GetHeroContentResult(BaseHero hero)
    {
        var shouldLog = true;
        var result = new List<string>();
        if (shouldLog)
            Log.Info($"Cards for {hero.NameTerm().ToEnglish()}");
        
        var archetypeKeys = hero.ArchetypeKeys;
        var paragonCards = hero.ParagonCards.ToHashSet();
        var cards = GetAllInstances<CardType>()
            .Where(c => c.IncludeInPools && archetypeKeys.Contains(c.GetArchetypeKey()) && !hero.ExcludedCards.Contains(c) && archetypeKeys.Any() && !paragonCards.Contains(c))

            .GroupBy(c => c.GetArchetypeKey())
            .ToDictionary(
                x => x.Key, // By Archetype 
                x => x
                    .Select(c =>
                    {
                        if (shouldLog)
                            Log.Info(c.Name);
                        return c;
                    })
                    .GroupBy(g => g.Rarity).OrderBy(r => (int)r.Key) // By Rarity
                    .ToDictionary(
                        r => r.Key,
                        r => r.Count()));
        
        var equipments = GetAllInstances<StaticEquipment>()
            .Where(e => e.IncludeInPools && archetypeKeys.Contains(e.ArchetypeKey) && e.Archetypes.Any())
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
            var checkChar = CheckChar(numCards >= archExpected);
            result.Add(hasValue
                ? $"{checkChar} - {arch} Cards - Total {numCards} - {string.Join(", ", a.Select(v => $"{v.Key}: {v.Value}{TargetCardNumbers(arch, v.Key)}"))}"
                : $"{checkChar} - {arch} Cards - All {ArchCardsExpectedStr(arch)} Missing");
        }

        var expectedEquipCount = 0;
        var expectedEquipRaritiesCounter = 0;
        var presentEquipRaritiesCounter = 0;
        var presentEquipCounter = 0;
        foreach (var arch in archetypeKeys.Where(a => !a.Contains("+")))
        {
            var hasValue = equipments.TryGetValue(arch, out var e);
            var numEquipRarities = hasValue ? e.Count(v => v.Value > 0) : 0;
            var numEquip = hasValue ? e.Sum(v => v.Value) : 0;
            var equipExpected = 8;
            var archRaritiesExpected = 4;
            expectedEquipCount += equipExpected;
            presentEquipCounter += numEquip;
            expectedEquipRaritiesCounter += archRaritiesExpected;
            presentEquipRaritiesCounter += numEquipRarities;
            var checkChar = CheckChar(numEquipRarities >= archRaritiesExpected && numEquip >= equipExpected);
            result.Add(hasValue
                ? $"{checkChar} - {arch} Equips - Total {numEquip} - Rarities {numEquipRarities} - {string.Join(", ", e.Select(v => $"{v.Key}: {v.Value}{TargetEquipmentNumbers(arch, v.Key)}"))}"
                : $"{checkChar} - {arch} Equips - All 4 Rarities Missing");
        }

        var expectedAllCounter = expectedCardsCounter + expectedEquipCount;
        var presentAllCounter = Math.Min(presentCardsCounter, expectedCardsCounter)
                                + Math.Max(Math.Min(presentEquipCounter, expectedEquipCount), Math.Min(presentEquipRaritiesCounter, expectedEquipRaritiesCounter));
        var percentage = expectedAllCounter > 0 
            ? presentAllCounter/(float)expectedAllCounter
            : 0;

        var paragonsV4Count = hero.LevelUpTreeV4 != null ? hero.ParagonCards.Length : 0;
        var extraCards = Math.Max(0, presentCardsCounter - expectedCardsCounter);
        // Wrong Content Complete Check
        var isContentComplete = presentAllCounter >= expectedAllCounter && paragonsV4Count == 3;
        var finalCheckChar = CheckChar(isContentComplete);
        result.Add($"{finalCheckChar} - {hero.NameTerm().ToEnglish()} - {percentage:P} - All {presentAllCounter}/{expectedAllCounter} - Missing {expectedAllCounter - presentAllCounter} - " +
                   $"Extra Cards {extraCards} " +
                   $"V4 Paragons {paragonsV4Count}/3 " +
                   $"Cards {presentCardsCounter}/{expectedCardsCounter} " +
                   $"Equip {presentEquipCounter}/{expectedEquipCount} " +
                   $"Equip Rarities {presentEquipRaritiesCounter}/{expectedEquipRaritiesCounter}");
        result = result.OrderBy(r => r.Contains("%") ? -1 : 0).ToList();
        return (result, isContentComplete);
    }
    
    private string CheckChar(bool isComplete) => isComplete ? "✓" : "✗";
    
    private string ArchCardsExpectedStr(string arch) => ArchCardsExpected(arch).ToString();
    private int ArchCardsExpected(string arch) => arch.Contains("+") ? 6 : 12;
    
    private string TargetEquipmentNumbers(string arch, Rarity r)
    {
        if (arch.Contains("+"))
            return "";
        
        return r switch
        {
            Rarity.Common => "/2",
            Rarity.Uncommon => "/2",
            Rarity.Rare => "/2",
            Rarity.Epic => "/2",
            _ => ""
        };
    }
    
    private string TargetCardNumbers(string arch, Rarity r)
    {
        if (arch.Contains("+"))
        {
            return r switch
            {
                Rarity.Common => "/1",
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
