using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/FixedStaticOptions")]
public class FixedStaticLevelUpOptions : LevelUpOptions
{
    [SerializeField] private StaticHeroLevelUpOption[] options;

    public override LevelUpOption[] Generate(Hero h) 
        => options.Cast<LevelUpOption>().ToArray();
}