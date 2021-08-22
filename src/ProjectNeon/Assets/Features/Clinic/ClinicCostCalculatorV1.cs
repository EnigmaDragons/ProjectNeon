using System;

public class ClinicCostCalculatorV1 : ClinicCostCalculator
{
    private const int _subsequentServiceCost = 20;
    private int _cost = 0;
    private PartyAdventureState _party;
    private string _corp;

    public ClinicCostCalculatorV1(PartyAdventureState party, string corp)
    {
        _party = party;
        _corp = corp;
    } 

    public int GetFullHealCost(Hero hero) => (int)Math.Ceiling(_cost * _party.GetCostFactorForClinic(_corp));
    public int GetInjuryHealCost() => (int)Math.Ceiling(_cost * _party.GetCostFactorForClinic(_corp));
    public void RequestClinicHealService() => _cost = _subsequentServiceCost;
}