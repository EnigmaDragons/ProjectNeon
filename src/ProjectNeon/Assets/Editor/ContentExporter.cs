using System;
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
    public string archetype1;
    public string archetype2;
    public int startingCredits = 100;
    public string description;
    public string backstory;
    public string className;

    public int maxHp = 40;
    public int maxShield = 12;
    public int startingShield = 0;
    public int attack = 8;
    public int magic = 0;
    public int armor = 0;
    public int resistance = 0;
    public int leadership = 0;
    public int startingAegis;
    public int startingDodge;
    public int startingTaunt;
    public ResourceType resource1;
    public int resource1GainPerTurn;
    public int resource1StartingAmountOverride = -1;
}

public class CardExportData
{
    public string name;
    public string rarity;
    public string cost;
    public string archetype1;
    public string archetype2;

    public string description;
}

public class ContentExporter
{
    [MenuItem("Neon/JSON/Export Hero Content")]
    public static void ExportHeroContent()
    {
        var heroes = ScriptableExtensions.GetAllInstances<AllHeroes>().First().Heroes
            .Where(x => !x.IsDisabled)
            .Select(x =>
                new HeroExportData 
                {
                    name = x.NameTerm().ToEnglish(),
                    sex = x.Sex.ToString(),
                    materialType = x.MaterialType.ToString(),
                    battleRole = x.BattleRole.ToString(),
                    description = x.DescriptionTerm().ToEnglish(),
                    className = x.ClassTerm().ToEnglish(),
                    backstory = x.BackStoryTerm().ToEnglish(),
                    
                    archetype1 = x.Archetypes.Count > 0 ? x.Archetypes.ElementAt(0) : "None",
                    archetype2 = x.Archetypes.Count > 1 ? x.Archetypes.ElementAt(1) : "None",
                    maxHp = x.maxHp,
                })
            .ToArray();

        var json = JsonUtility.ToJson(new ItemsWrapper<HeroExportData> { Items = heroes }, true);
        File.WriteAllText("./HeroData.json", json);
        Log.Info("Exported Hero Data");
    }

    [MenuItem("Neon/JSON/Export Card Content")]
    public static void ExportCardContent() =>
        WithLocalization(() =>
        {
            var cards = ScriptableExtensions.GetAllInstances<AllCards>().First().Cards
                .Where(x => !x.IsWip && x.IncludeInPools)
                .Select(x =>
                    new CardExportData()
                    {
                        name = x.NameTerm.ToEnglish(),
                        description =
                            x.LocalizedDescription(Maybe<Member>.Missing(), ResourceQuantity.DontInterpolateX),

                        archetype1 = x.Archetypes.Count > 0 ? x.Archetypes.ElementAt(0) : "None",
                        archetype2 = x.Archetypes.Count > 1 ? x.Archetypes.ElementAt(1) : "None",

                        rarity = x.Rarity.ToString(),
                        cost = x.cost.PlusXCost ? "X" : x.cost.BaseAmount.ToString()
                    })
                .ToArray();

            var json = JsonUtility.ToJson(new ItemsWrapper<CardExportData> { Items = cards }, true);
            File.WriteAllText("./CardData.json", json);
            Log.Info("Exported Card Data");
        });

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
}
