using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroLevelUpRewardV4
{
    [SerializeField] private LevelUpOptions options;
    [SerializeField] private int hpGain = 3;
    [SerializeField] private StatType buffStat;

    public LevelUpOptions Options => options;
    public int HpGain => hpGain;
    public StatType BuffStat => buffStat;
    public string OptionsPrompt => options.ChoiceDescription;
    
    public LevelUpOption[] GenerateOptions(Hero h)
    {
        var baseOptions = Options.Generate(h);
        var composedOptions = baseOptions.Select(o => (LevelUpOption)new LevelUpOptionWithHpAndStatGain(o, hpGain, buffStat))
            .ToArray();
        return composedOptions;
    }
}