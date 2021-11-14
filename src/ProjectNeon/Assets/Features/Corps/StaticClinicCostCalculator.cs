public class StaticClinicCostCalculator : ClinicCostCalculator
{
    private int _cost;

    public StaticClinicCostCalculator(int cost) => _cost = cost;
    
    public int GetFullHealCost(Hero hero) => _cost;

    public int GetInjuryHealCost() => _cost;

    public void RequestClinicHealService() {}
}