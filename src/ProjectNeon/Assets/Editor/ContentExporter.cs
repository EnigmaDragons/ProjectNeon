using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I2.Loc;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ItemsWrapper<T>
{
    [SerializeField] public T[] Items;
}

[Serializable]
public class HeroExportData 
{
    public string name;
    
    public string sex;
    public string materialType;
    public string battleRole;
    public int startingCredits;
    public string description;
    public string backstory;
    public string className;

    public string archetype1;
    public string archetype2;
    
    public int maxHp;
    public int maxShield;
    public int startingShield ;
    public float power;
    public float armor;
    public float resistance;
    
    public int startingAegis;
    public int startingDodge;
    public int startingTaunt;
    
    public string resource1;
    public int resource1GainPerTurn;
    public int resource1StartingAmountOverride = -1;
    
    public string resource2;

    public CardExportData[] cards;
}

[Serializable]
public class CardExportData
{
    public string name;
    public string rare;
    public string cost;
    public string arch1;
    public string arch2;

    public string desc;
}

[Serializable]
public class FullDataExport
{
    public KeyValuePair<string, string>[] KeywordRules;
    public HeroExportData[] Heroes;
}

public class ContentExporter
{
    private static HashSet<string> excludeHeroes = new HashSet<string> { "Trainee", "No Hero", "Anon", "Ionus", "Andi" };

    [MenuItem("Neon/JSON/AI Data")]
    public static void ExportAiData() =>
        WithLocalization(() =>
        {
            var heroes = ExportHeroContent();
            var keywords = ScriptableExtensions.GetAllInstances<StringKeyTermCollection>().Single(x => x.name.Equals("KeywordRulesV2"));
            var keywordsArr = keywords.All.Select(k => new KeyValuePair<string, string>(k.Key.ToString(), k.Term.ToString().ToEnglish())).ToArray();

            var data = new FullDataExport
            {
                Heroes = heroes,
                KeywordRules = keywordsArr
            };

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText("./AiData.json", json);
            Log.Info("Exported Hero Data");
            return heroes;
        });

    [MenuItem("Neon/JSON/Export Hero Content")]
    public static HeroExportData[] ExportHeroContent() => 
        WithLocalization(() =>
        {
            var allCardsPool = ScriptableExtensions.GetAllInstances<ShopCardPool>().Single(x => x.name.Equals("AllCardsPool"));
            var heroes = ScriptableExtensions.GetAllInstances<AllHeroes>().First().Heroes
                .Where(x => !x.IsDisabled && !excludeHeroes.Contains(x.NameTerm().ToEnglish()))
                .Select(x =>
                    new HeroExportData
                    {
                        name = x.NameTerm().ToEnglish(),
                        sex = x.Sex.ToString(),
                        materialType = x.MaterialType.ToString(),
                        battleRole = x.BattleRole.ToString(),
                        startingCredits = x.StartingCredits,
                        description = x.DescriptionTerm().ToEnglish(),
                        className = x.ClassTerm().ToEnglish(),
                        backstory = x.BackStoryTerm().ToEnglish(),

                        archetype1 = x.Archetypes.Count > 0 ? x.Archetypes.ElementAt(0) : "None",
                        archetype2 = x.Archetypes.Count > 1 ? x.Archetypes.ElementAt(1) : "None",
                        
                        maxHp = x.maxHp,
                        maxShield = x.maxShield,
                        power = x.Stats[x.Stats.DefaultPrimaryStat()],
                        armor = x.armor,
                        resistance = x.resistance,
                        
                        startingShield = x.startingShield,
                        startingAegis = x.startingAegis,
                        startingDodge = x.startingDodge,
                        
                        resource1 = x.resource1.GetLocalizedName(),
                        resource1GainPerTurn = x.resource1GainPerTurn,
                        resource1StartingAmountOverride = x.resource1StartingAmountOverride,
                        
                        resource2 = x.resource2 != null ? x.resource2.GetLocalizedName() : "",

                        cards = GenerateCardSelection(x, allCardsPool).Select(ToExportData).ToArray()
                    })
                .ToArray();

            var json = JsonUtility.ToJson(new ItemsWrapper<HeroExportData> { Items = heroes }, true);
            File.WriteAllText("./HeroData.json", json);
            Log.Info("Exported Hero Data");
            return heroes;
        });
    

    [MenuItem("Neon/JSON/Export Card Content")]
    public static void ExportCardContent() =>
        WithLocalization(() =>
        {
            var cards = ScriptableExtensions.GetAllInstances<AllCards>().First().Cards
                .Where(x => !x.IsWip && x.IncludeInPools)
                .Select(ToExportData)
                .ToArray();

            var json = JsonUtility.ToJson(new ItemsWrapper<CardExportData> { Items = cards }, true);
            File.WriteAllText("./CardData.json", json);
            Log.Info("Exported Card Data");
        });

    private static CardExportData ToExportData(CardType x)
    {
        return new CardExportData()
        {
            name = x.NameTerm.ToEnglish(),
            desc =
                x.LocalizedDescription(Maybe<Member>.Missing(), ResourceQuantity.DontInterpolateX),

            arch1 = x.Archetypes.Count > 0 ? x.Archetypes.ElementAt(0) : "None",
            arch2 = x.Archetypes.Count > 1 ? x.Archetypes.ElementAt(1) : "None",

            rare = x.Rarity.ToString(),
            cost = x.cost.PlusXCost ? "X" : x.cost.BaseAmount.ToString()
        };
    }

    private static IEnumerable<CardType> GenerateCardSelection(BaseHero hero, ShopCardPool allCards)
    {
        var archKeys = hero.ArchetypeKeys();
        var excludedCards = hero.ExcludedCards;
        var additionalCards = hero.AdditionalStartingCards;
        var cards = allCards.All
            .Where(c => archKeys.Contains(c.GetArchetypeKey()) || c.Archetypes.None())
            .Where(c => c.Rarity != Rarity.Basic)
            .Where(c => !excludedCards.Contains(c))
            .Where(c => c.Archetypes.Any())
            .Concat(additionalCards)
            .OrderBy(c => c.Archetypes.None() ? 99 : c.Archetypes.Count)
            .ThenBy(c => c.GetArchetypeKey())
            .ThenBy(c => c.Rarity)
            .ThenBy(c => c.Cost.BaseAmount)
            .ThenBy(c => c.Name)
            .Concat(hero.BasicCard)
            .Concat(hero.ParagonCards);
        return cards;
    }

    private static void WithLocalization(Action action)
    {
        var editorLocalizationParams = new MpZeroEditorGlobalLocalizationParams();
        LocalizationManager.ParamManagers.Add(editorLocalizationParams);
        LocalizationManager.LocalizeAll(true);
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Log.Error($"Unexpected Exception of type {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        finally
        {
            LocalizationManager.ParamManagers.Remove(editorLocalizationParams);
        }
    }
    
    private static T WithLocalization<T>(Func<T> fn)
    {
        var editorLocalizationParams = new MpZeroEditorGlobalLocalizationParams();
        LocalizationManager.ParamManagers.Add(editorLocalizationParams);
        LocalizationManager.LocalizeAll(true);
        try
        {
            return fn();
        }
        catch (Exception ex)
        {
            Log.Error($"Unexpected Exception of type {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        finally
        {
            LocalizationManager.ParamManagers.Remove(editorLocalizationParams);
        }
    }
}
