using System;

[Serializable]
public class AdditiveStatInjury : HeroInjury
{
    public StringReference Name = new StringReference("Injury");
    public StringReference Stat;
    public float Amount;

    public string Description => $"{Amount} {Stat}";
}
