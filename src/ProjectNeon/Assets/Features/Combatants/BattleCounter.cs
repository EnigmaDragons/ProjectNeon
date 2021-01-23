using System;
using UnityEngine;

public sealed class BattleCounter
{
    private readonly Func<float> _getCurrentMaxAmount;
    public string Name { get; }
    public int Amount { get; private set; }
    public int Max => RoundUp(_getCurrentMaxAmount());

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

    private static int RoundUp(float amount) => Mathf.CeilToInt(amount);
    public void ChangeBy(float delta) => Amount = RoundUp(Mathf.Clamp(Amount + delta, 0, _getCurrentMaxAmount()));
    public void Set(float value) => Amount = RoundUp(value);

    public override string ToString() => $"{Name} - {Amount}/{Max}";
}
