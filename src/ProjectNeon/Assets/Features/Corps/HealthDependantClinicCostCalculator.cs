using System;

public class HealthDependantClinicCostCalculator : ClinicCostCalculator
{
    public int GetFullHealCost(Hero hero) => (int)Math.Floor((1 -((float)hero.CurrentHp / (float)hero.Stats.MaxHp())) * 60f);
    public int GetInjuryHealCost() => 30;
    public void RequestClinicHealService() { }
}