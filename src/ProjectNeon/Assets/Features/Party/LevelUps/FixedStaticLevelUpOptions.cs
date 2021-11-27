using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/FixedStaticOptions")]
public class FixedStaticLevelUpOptions : LevelUpOptions
{
    [SerializeField] private StaticHeroLevelUpOption[] options;

    public override LevelUpOption[] Generate(Hero h) 
        => options.Cast<LevelUpOption>().ToArray();
}