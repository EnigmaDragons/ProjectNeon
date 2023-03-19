using System;
using UnityEngine;

public sealed class BattleCounter
{
    private float _maxAdjustment;
    private readonly Func<float> _getCurrentMaxAmount;
    public string Name { get; }
    public int Amount { get; private set; }
    public int Max => Round(_getCurrentMaxAmount() + _maxAdjustment);

    public BattleCounter(TemporalStatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.GetString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(StatType type, float initialAmount, Func<float> getCurrentMaxAmount) 
        : this(type.GetString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(string name, float initialAmount, Func<float> getCurrentMaxAmount)
    {
        _getCurrentMaxAmount = getCurrentMaxAmount;
        Name = name;
        Amount = Round(initialAmount);
    }

    public void ChangeBy(float delta) => Amount = Round(Mathf.Clamp(Amount + Round(delta), 0, Max));
    public void Set(float value) => Amount = Round(Mathf.Clamp(value, 0, Max));
    public void AdjustMax(float value) => _maxAdjustment += Round(value);

    public override string ToString() => $"{Name} - {Amount}/{Max}";

    private static int Round(float amount) => amount > 0 ? Mathf.CeilToInt(amount) : Mathf.FloorToInt(amount);
}
