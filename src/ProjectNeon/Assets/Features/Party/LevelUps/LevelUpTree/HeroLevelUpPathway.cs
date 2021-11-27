using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/LevelUpTree")]
public class HeroLevelUpPathway : ScriptableObject
{
    [SerializeField] private StaticHeroLevelUpOptions defaultLevelUp;
    [SerializeField] private StaticHeroLevelUpOptions[] options;

    public StaticHeroLevelUpOption[] ForLevel(int level)
    {
        var index = level - 2; // First Level Up happens when you hit level 2. So, the 0 index corresponds to Level 2.
        return options.Length > index && options[index].options.Any()
            ? options[index].options.ToArray().Shuffled()
            : defaultLevelUp.options.ToArray().Shuffled();
    }
}
