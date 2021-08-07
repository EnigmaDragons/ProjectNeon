using System;
using UnityEngine;

public sealed class BattleCounter
{
    private readonly Func<float> _getCurrentMaxAmount;
    public string Name { get; }
    public int Amount { get; private set; }
    public int Max => Round(_getCurrentMaxAmount());

    public BattleCounter(TemporalStatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.ToString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(StatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.ToString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(string name, float initialAmount, Func<float> getCurrentMaxAmount)
    {
        _getCurrentMaxAmount = getCurrentMaxAmount;
        Name = name;
        Amount = Round(initialAmount);
    }

    public void ChangeBy(float delta) => Amount = Round(Mathf.Clamp(Amount + Round(delta), 0, _getCurrentMaxAmount()));
    public void Set(float value) => Amount = Round(Mathf.Clamp(value, 0, _getCurrentMaxAmount()));

    public override string ToString() => $"{Name} - {Amount}/{Max}";

    private static int Round(float amount) => amount > 0 ? Mathf.CeilToInt(amount) : Mathf.FloorToInt(amount);
}
