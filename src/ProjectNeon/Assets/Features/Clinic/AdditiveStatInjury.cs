using System;

[Serializable]
public class AdditiveStatInjury : HeroInjury
{
    public StringReference Stat;
    public float Amount;

    public string Description => $"{Amount} {Stat}";
}
