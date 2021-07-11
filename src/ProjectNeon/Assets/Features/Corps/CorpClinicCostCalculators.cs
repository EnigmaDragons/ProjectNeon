using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CorpClinicCostCalculators")]
public class CorpClinicCostCalculators : ScriptableObject
{
    [SerializeField] private StaticCorp[] healthPercentCorp;
    [SerializeField] private StaticCorp[] credDependentCorp;
    [SerializeField] private PartyAdventureState party;

    public ClinicCostCalculator Get(Corp corp)
    {
        if (healthPercentCorp.Contains(corp))
            return new HealthDependantClinicCostCalculator();
        if (credDependentCorp.Contains(corp))
            return new CredDepnedantClinicCostCalculator(party);
        return new ClinicCostCalculatorV1();
    }
}