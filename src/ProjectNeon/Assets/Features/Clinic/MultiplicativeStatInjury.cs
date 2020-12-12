using System;

[Serializable]
public class MultiplicativeStatInjury : HeroInjury
{
    public StringReference Stat;
    public float Amount;

    public string Description => $"{Amount}x {Stat}";
}
