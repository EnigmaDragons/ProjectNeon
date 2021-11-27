using System;
using UnityEngine;

[Serializable]
public class HeroLevelUpRewardV4
{
    [SerializeField] private HeroLevelUpOption option;
    [SerializeField] private int hpGain = 3;
    [SerializeField] private StatType buffStat;

    public HeroLevelUpOption Option => option;
    public int HpGain => hpGain;
    public StatType BuffStat => buffStat;
}