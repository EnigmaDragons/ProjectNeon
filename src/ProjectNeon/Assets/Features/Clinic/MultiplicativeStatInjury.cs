using System;

[Serializable]
public class MultiplicativeStatInjury : HeroInjury
{    
    public StringReference Name = new StringReference("Injury");
    public StringReference Stat;
    public float Amount;

    public string InjuryName => Name;
    public string Description => $"{Amount}x {Stat}";
}
