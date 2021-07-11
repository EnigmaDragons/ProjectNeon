using System;

public class CredDepnedantClinicCostCalculator : HealthDependantClinicCostCalculator
{
    private readonly float _multiplier;
    
    public CredDepnedantClinicCostCalculator(PartyAdventureState party)
    {
        if (party.Credits <= 30)
            _multiplier = 0.0f;
        else if (party.Credits <= 060)
            _multiplier = 0.3f;
        else if (party.Credits <= 090)
            _multiplier = 0.5f;
        else if (party.Credits <= 120)
            _multiplier = 0.7f;
        else if (party.Credits <= 150)
            _multiplier = 0.8f;
        else if (party.Credits <= 180)
            _multiplier = 0.9f;
        else if (party.Credits <= 210)
            _multiplier = 1.0f;
        else if (party.Credits <= 250)
            _multiplier = 1.1f;
        else if (party.Credits <= 300)
            _multiplier = 1.2f;
        else if (party.Credits <= 360)
            _multiplier = 1.3f;
        else if (party.Credits <= 430)
            _multiplier = 1.4f;
        else if (party.Credits <= 510)
            _multiplier = 1.5f;
        else if (party.Credits <= 600)
            _multiplier = 1.6f;
        else if (party.Credits <= 700)
            _multiplier = 1.7f;
        else if (party.Credits <= 810)
            _multiplier = 1.8f;
        else if (party.Credits <= 930)
            _multiplier = 1.9f;
        else
            _multiplier = 2.0f;
    }
    
    public new int GetFullHealCost(Hero hero) => (int)Math.Floor(_multiplier * base.GetFullHealCost(hero));
    public new int GetInjuryHealCost() => (int)Math.Floor(_multiplier * base.GetInjuryHealCost());
}