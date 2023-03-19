using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/FixedStaticOptions")]
public class FixedStaticLevelUpOptions : LevelUpOptions, ILocalizeTerms
{
    [SerializeField] private string choiceDescriptionTerm;
    [SerializeField] private StaticHeroLevelUpOption[] options;

    public override string ChoiceDescriptionTerm => string.IsNullOrWhiteSpace(choiceDescriptionTerm) ? "LevelUps/ChooseNewBasic" : choiceDescriptionTerm;

    public override LevelUpOption[] Generate(Hero h) 
        => options.Cast<LevelUpOption>().ToArray().Shuffled();

    public string[] GetLocalizeTerms()
        => new[] { ChoiceDescriptionTerm };
}