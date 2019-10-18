using System;
using UnityEngine;

public sealed class BattleCounter
{
    private readonly Func<int> _getCurrentMaxAmount;
    public string Name { get; }
    public int Amount { get; private set; }

    public BattleCounter(StatType type, int initialAmount, Func<int> getCurrentMaxAmount) 
        : this(type.ToString(), initialAmount, getCurrentMaxAmount) {}
    
    public BattleCounter(string name, int initialAmount, Func<int> getCurrentMaxAmount)
    {
        _getCurrentMaxAmount = getCurrentMaxAmount;
        Name = name;
        Amount = initialAmount;
    }

    private int RoundUp(float amount) => Convert.ToInt32(Math.Ceiling(amount));
    public void ChangeBy(float delta) => Amount = RoundUp(Mathf.Clamp(Amount + delta, 0, _getCurrentMaxAmount()));
}
