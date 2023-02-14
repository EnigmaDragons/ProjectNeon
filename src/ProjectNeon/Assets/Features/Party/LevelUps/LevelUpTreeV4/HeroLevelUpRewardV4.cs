using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroLevelUpRewardV4
{
    [SerializeField] private LevelUpOptions options;
    [SerializeField] private int hpGain = 3;
    [SerializeField] private StatType buffStat;
    [SerializeField] private int buffStatAmount = 1;

    public LevelUpOptions Options => options;
    public int HpGain => hpGain;
    public StatType BuffStat => buffStat;
    public int BuffStatAmount => buffStatAmount;
    public string OptionsPromptTerm => options.ChoiceDescriptionTerm;
    public IStats StatBoostAmount => new StatAddends().With(StatType.MaxHP, HpGain).With(BuffStat, buffStatAmount);

    #if UNITY_EDITOR
    public void EditorSetHpGain(int newAmount) => hpGain = newAmount;
    #endif
    
    public LevelUpOption[] GenerateOptions(Hero h, PartyAdventureState party)
    {
        var baseOptions = Options.Generate(h);
        var composedOptions = baseOptions.Select(o => (LevelUpOption)new LevelUpOptionWithHpAndStatGain(party, o, hpGain, buffStat, buffStatAmount))
            .ToArray();
        return composedOptions;
    }
}