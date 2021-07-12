using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CorpClinicProvider")]
public class CorpClinicProvider : ScriptableObject
{
    [SerializeField] private StaticCorp[] healthPercentCorp;
    [SerializeField] private StaticCorp[] credDependentCorp;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private StaticCorp[] procedureCorps;
    [SerializeField] private StaticCorp[] blessingCorps;

    public ClinicCostCalculator GetCostCalculator(Corp corp)
    {
        if (healthPercentCorp.Contains(corp))
            return new HealthDependantClinicCostCalculator();
        if (credDependentCorp.Contains(corp))
            return new CredDepnedantClinicCostCalculator(party);
        return new ClinicCostCalculatorV1();
    }

    public ClinicServiceProvider GetServices(Corp corp)
    {
        if (procedureCorps.Contains(corp))
            return new ImplantClinicServiceProvider(party);
        return new NoClinicServiceProvider();
    }
}