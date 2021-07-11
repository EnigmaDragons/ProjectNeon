public interface ClinicCostCalculator
{
    int GetFullHealCost(Hero hero);
    int GetInjuryHealCost();
    void RequestClinicHealService();
}