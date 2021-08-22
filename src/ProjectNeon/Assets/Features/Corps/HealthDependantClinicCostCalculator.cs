using System;

public class HealthDependantClinicCostCalculator : ClinicCostCalculator
{
    private readonly PartyAdventureState _party;
    private readonly string _corp;

    public HealthDependantClinicCostCalculator(PartyAdventureState party, string corp)
    {
        _party = party;
        _corp = corp;
    }
    
    public int GetFullHealCost(Hero hero) => (int)Math.Floor((1 -((float)hero.CurrentHp / (float)hero.Stats.MaxHp())) * 60f * _party.GetCostFactorForClinic(_corp));
    public int GetInjuryHealCost() => (int)Math.Floor(30 * _party.GetCostFactorForClinic(_corp));
    public void RequestClinicHealService() { }
}