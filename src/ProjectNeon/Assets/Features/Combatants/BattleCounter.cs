using System;
using UnityEngine;

public sealed class BattleCounter
{
    private readonly Func<float> _getCurrentMaxAmount;
    public string Name { get; }
    public int Amount { get; private set; }

    public BattleCounter(TemporalStatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.ToString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(StatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.ToString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(string name, float initialAmount, Func<float> getCurrentMaxAmount)
    {
        _getCurrentMaxAmount = getCurrentMaxAmount;
        Name = name;
        Amount = RoundUp(initialAmount);
    }

    private static int RoundUp(float amount) => Convert.ToInt32(Math.Ceiling(amount));
    public void ChangeBy(float delta) => Amount = RoundUp(Mathf.Clamp(Amount + delta, 0, _getCurrentMaxAmount()));
}
