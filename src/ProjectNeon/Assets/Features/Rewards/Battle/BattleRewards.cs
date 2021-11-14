using System;
using UnityEngine;

public abstract class BattleRewards : ScriptableObject
{
    public abstract void GrantVictoryRewardsAndThen(Action onFinished, LootPicker lootPicker);
}