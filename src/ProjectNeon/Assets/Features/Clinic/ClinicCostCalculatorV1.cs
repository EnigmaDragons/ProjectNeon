public class ClinicCostCalculatorV1 : ClinicCostCalculator
{
    private const int _subsequentServiceCost = 20;
    private int _cost = 0;

    public int GetFullHealCost(Hero hero) => _cost;
    public int GetInjuryHealCost() => _cost;
    public void RequestClinicHealService() => _cost = _subsequentServiceCost;
}