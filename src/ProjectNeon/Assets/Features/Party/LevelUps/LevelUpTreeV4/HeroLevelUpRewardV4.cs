using System;
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
}