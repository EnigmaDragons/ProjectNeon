using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/LevelUpTree")]
public class HeroLevelUpPathway : ScriptableObject
{
    [SerializeField] private HeroLevelUpOptions defaultLevelUp;
    [SerializeField] private HeroLevelUpOptions[] options;

    public HeroLevelUpOption[] ForLevel(int level) => options.Length >= level && options[level - 1].options.Any() 
        ? options[level - 1].options.ToArray()
        : defaultLevelUp.options.ToArray();
}
