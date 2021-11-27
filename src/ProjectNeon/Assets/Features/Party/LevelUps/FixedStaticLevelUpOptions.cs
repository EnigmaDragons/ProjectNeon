using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/FixedStaticOptions")]
public class FixedStaticLevelUpOptions : LevelUpOptions
{
    [SerializeField] private string choiceDescription;
    [SerializeField] private StaticHeroLevelUpOption[] options;

    public override string ChoiceDescription => string.IsNullOrWhiteSpace(choiceDescription) ? "Choose a New Basic" : choiceDescription;

    public override LevelUpOption[] Generate(Hero h) 
        => options.Cast<LevelUpOption>().ToArray();
}