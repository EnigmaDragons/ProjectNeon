using System.Collections.Generic;
using UnityEngine;

public class MemberStatDiffPanel : MonoBehaviour
{
    [SerializeField] private GameObject statParent;
    [SerializeField] private StatDiffPresenter diffPresenterPrototype;

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

        ShownStatTypes.ForEach(s =>
        {
            Instantiate(diffPresenterPrototype, statParent.transform)
                .Initialized(s.ToString(), baseStats[s].CeilingInt(), difference[s].CeilingInt(), AlwaysShowStatTypes.Contains(s));
        });

        return this;
    }
}
