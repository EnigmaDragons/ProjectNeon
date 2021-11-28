using System.Collections.Generic;
using UnityEngine;

public class MemberStatDiffPanel : MonoBehaviour
{
    [SerializeField] private GameObject statParent;
    [SerializeField] private StatDiffPresenter diffPresenterPrototype;
    [SerializeField] private float initialStatUpDelay = 0.6f;
    [SerializeField] private float delayBetweenStats = 1f;

    private static readonly StatType[] ShownStatTypes = {
        StatType.MaxHP,
        StatType.Attack,
        StatType.Magic, 
        StatType.Leadership, 
        StatType.Economy,
        StatType.StartingShield, 
        StatType.MaxShield, 
        StatType.Armor, 
        StatType.Resistance, 
    };

    private static readonly HashSet<StatType> AlwaysShowStatTypes = new HashSet<StatType>
    {
        StatType.MaxHP,
        StatType.StartingShield,
        StatType.MaxShield,
        StatType.Armor,
        StatType.Resistance,
    };

    public MemberStatDiffPanel Initialized(Hero hero)
    {
        var currentStats = hero.Stats;
        var levelUpReward = hero.NextLevelUpRewardV4;
        var afterLevelUpStats = currentStats.Plus(levelUpReward.StatBoostAmount);
        return Initialized(currentStats, afterLevelUpStats);
    }
    
    public MemberStatDiffPanel Initialized(IStats baseStats, IStats newStats)
    {
        var difference = newStats.Minus(baseStats);
        statParent.DestroyAllChildren();

        var delay = initialStatUpDelay;
        ShownStatTypes.ForEach(s =>
        {
            var originalAmount = baseStats[s].CeilingInt();
            var diffAmount = difference[s].CeilingInt();
            var willShow = AlwaysShowStatTypes.Contains(s) || originalAmount > 0 || diffAmount > 0;
            Instantiate(diffPresenterPrototype, statParent.transform)
                .Initialized(s.ToString(), originalAmount, diffAmount, delay, willShow);
            delay += willShow && diffAmount > 0 ? delayBetweenStats : 0f;
        });

        return this;
    }
}
